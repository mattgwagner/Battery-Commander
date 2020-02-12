using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class CalendarService
    {
        private const string DateFormat = "yyyyMMddTHHmmssZ";

        private static IEnumerable<int> YearsToGenerate => Enumerable.Range(DateTime.Today.Year, 3);

        public static String GenerateUrl(ClaimsPrincipal user, IUrlHelper urlHelper, int unitId)
        {
            var apiKey = UserService.Generate_Token(user);

            return urlHelper.Action("Calendar", "Units", new { unitId, apiKey });
        }

        public static async Task<byte[]> Generate(Database db, int unitId)
        {
            // Get the raw data

            var unit =
                await db
                .Units
                .Where(_ => _.Id == unitId)
                .SingleOrDefaultAsync();

            var soldiers = await SoldierService.Filter(db, new SoldierService.Query { Unit = unitId });

            var evaluations = EvaluationService.Filter(db, new EvaluationService.Query { Unit = unitId });

            var sutas =
                await db
                .SUTAs
                .Include(s => s.Soldier)
                .Where(s => s.Soldier.UnitId == unitId)
                .ToListAsync();

            // Build model

            var sb = new StringBuilder()
                .AppendLine("BEGIN:VCALENDAR")
                .AppendLine("PRODID:-//Red-Leg-Dev//Battery Commander//EN")
                .AppendLine("VERSION:2.0")
                .AppendLine("METHOD:PUBLISH");

            new[]
            {
                Generate_For_Birthdays(soldiers),
                Generate_For_ETS_Dates(soldiers),
                Generate_For_Evaluations(evaluations),
                Generate_For_SUTA_Requests(sutas)
            }
            .SelectMany(_ => _)
            .Select(entry => To_Event(entry))
            .ToList()
            .ForEach(entry => sb.Append(entry));

            sb = sb.AppendLine("END:VCALENDAR");

            // Return iCal Feed

            // return File(bytes, "text/calendar");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static IEnumerable<Entry> Generate_For_SUTA_Requests(IEnumerable<SUTA> sutas)
        {
            foreach (var suta in sutas)
            {
                // TODO Include all dates

                yield return new Entry
                {
                    Description = $"{suta.Soldier} SUTA - {suta.Status}",
                    Date = suta.StartDate
                };
            }
        }

        private static IEnumerable<Entry> Generate_For_Evaluations(IEnumerable<Evaluation> evaluations)
        {
            foreach (var evaluation in evaluations)
            {
                yield return new Entry
                {
                    Description = $"{evaluation.Ratee.Rank.ShortName()} {evaluation.Ratee.LastName} Eval Thru-Date",
                    Date = evaluation.ThruDate
                };
            }
        }

        private static IEnumerable<Entry> Generate_For_ETS_Dates(IEnumerable<Soldier> soldiers)
        {
            foreach (var soldier in soldiers)
            {
                if (soldier.ETSDate.HasValue)
                {
                    yield return new Entry
                    {
                        Description = $"{soldier.Rank.ShortName()} {soldier.LastName} ETS",
                        Date = soldier.ETSDate.Value
                    };
                }
            }
        }

        private static IEnumerable<Entry> Generate_For_Birthdays(IEnumerable<Soldier> soldiers)
        {
            foreach (var soldier in soldiers)
            {
                if (soldier.HasDoB)
                {
                    foreach (var year in YearsToGenerate)
                    {
                        yield return new Entry
                        {
                            Description = $"{soldier.Rank.ShortName()} {soldier.LastName} Birthday",
                            Date = new DateTime(year, soldier.DateOfBirth.Month, soldier.DateOfBirth.Day)
                        };
                    }
                }
            }
        }

        private static String To_Event(Entry entry)
        {
            return new StringBuilder()
                .AppendLine("BEGIN:VEVENT")
                .AppendLine($"SUMMARY:{entry.Description}")
                .AppendLine($"DTSTAMP:{DateTime.UtcNow.ToUniversalTime().ToString(DateFormat)}")
                .AppendLine("UID:" + (entry.Description + entry.Date).GetHashCode())
                .AppendLine("DTSTART:" + entry.Date.ToString("yyyyMMdd"))
                .AppendLine("LAST-MODIFIED:" + DateTime.UtcNow.ToUniversalTime().ToString(DateFormat))
                .AppendLine("SEQUENCE:0")
                .AppendLine("STATUS:CONFIRMED")
                .AppendLine("TRANSP:OPAQUE")
                .AppendLine("END:VEVENT")
                .ToString();
        }

        private class Entry
        {
            public String Description { get; set; }

            public DateTime Date { get; set; }
        }
    }
}