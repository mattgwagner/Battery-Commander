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

            foreach (var entry in Generate_For_Birthdays(soldiers))
            {
                sb = sb.Append(To_Event(entry));
            }

            foreach (var entry in Generate_For_Evaluations(evaluations))
            {
                sb = sb.Append(To_Event(entry));
            }

            sb = sb.AppendLine("END:VCALENDAR");

            // Return iCal Feed

            // return File(bytes, "text/calendar");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private IEnumerable<Entry> Generate_For_Evaluations(IEnumerable<Evaluation> evaluations)
        {
            foreach (var evaluation in evaluations)
            {
                yield return new Entry
                {
                    Description = $"{evaluation.Ratee} Evaluation Thru-Date",
                    Date = evaluation.ThruDate
                };
            }
        }

        private IEnumerable<Entry> Generate_For_Birthdays(IEnumerable<Soldier> soldiers)
        {
            foreach (var soldier in soldiers)
            {
                if (soldier.HasDoB)
                {
                    foreach (var year in YearsToGenerate)
                    {
                        yield return new Entry
                        {
                            Description = $"{soldier} Birthday",
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