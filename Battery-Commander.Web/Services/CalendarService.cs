using BatteryCommander.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class CalendarService
    {
        private const string DateFormat = "yyyyMMddTHHmmssZ";

        private static IEnumerable<int> YearsToGenerate => Enumerable.Range(DateTime.Today.Year, 3);

        //[AllowAnonymous, Route("~/Calendar")]
        public static async Task<byte[]> Generate(Database db, int unitId, String apikey)
        {
            var unit =
                await db
                .Units
                .Where(_ => _.Id == unitId)
                .SingleOrDefaultAsync();

            // TODO Verify API key

            // Get the raw data

            var soldiers = await SoldierSearchService.Filter(db, new SoldierSearchService.Query { Unit = unitId });

            var evaluations =
                await db
                .Evaluations
                .Where(evaluation => !evaluation.IsCompleted)
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
                Generate_For_Evaluations(evaluations)
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

        private static IEnumerable<Entry> Generate_For_Evaluations(IEnumerable<Evaluation> evaluations)
        {
            foreach (var evaluation in evaluations)
            {
                yield return new Entry
                {
                    Description = $"{evaluation.Ratee.Rank} {evaluation.Ratee.LastName} Eval Thru-Date",
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
                        Description = $"{soldier.Rank} {soldier.LastName} ETS",
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
                            Description = $"{soldier.Rank} {soldier.LastName} Birthday",
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