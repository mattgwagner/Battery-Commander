using BatteryCommander.Web.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class BatchImportService
    {
        public static async Task<IEnumerable<Soldier>> ImportSoldiers(Database db, Stream stream)
        {
            var changed = new List<Soldier>();

            var units = db.Units.ToList();

            using (var excel = new ExcelPackage(stream))
            {
                // Upc Rank Name Sex PMos Dob Dor Ets

                // Process Excel upload

                var sheet = excel.Workbook.Worksheets.First();

                // Skip header row

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var unit =
                        units
                        .Where(u => !String.IsNullOrWhiteSpace(u.UIC))
                        .Where(u => u.UIC.Contains((String)sheet.Cells[row, 1].Value))
                        .SingleOrDefault();

                    if(unit == null)
                    {
                        // Skipping
                        continue;
                    }

                    // Parse into soldier

                    var names = $"{sheet.Cells[row, 3].Value}".Split(' ');

                    var middle = names.Count() > 2 ? names[2] : "";

                    var ets = $"{sheet.Cells[row, 8].Value}" == "-" ? (DateTime?)null : Convert.ToDateTime(sheet.Cells[row, 8].Value);

                    var dor = Convert.ToDateTime(sheet.Cells[row, 7].Value);

                    var gender = $"{sheet.Cells[row, 4].Value}" == "M" ? Gender.Male : Gender.Female;

                    var parsed = new Soldier
                    {
                        LastName = names[0],
                        MiddleName = middle,
                        FirstName = names[1],
                        Rank = RankExtensions.Parse($"{sheet.Cells[row, 2].Value}"),
                        DateOfBirth = Convert.ToDateTime(sheet.Cells[row, 6].Value),
                        ETSDate = ets,
                        DateOfRank = dor,
                        Gender = gender,
                        Unit = unit
                    };

                    // Check for existing, create if not

                    var existing =
                        await db
                        .Soldiers
                        .Where(_ => _.LastName.ToUpper() == parsed.LastName.ToUpper())
                        .Where(_ => _.FirstName.ToUpper() == parsed.FirstName.ToUpper())
                        .FirstOrDefaultAsync();

                    if (existing == null)
                    {
                        // Soldier does not exist, create

                        await db.Soldiers.AddAsync(parsed);
                    }
                    else
                    {
                        // Update existing entries

                        if (string.IsNullOrWhiteSpace(existing.MiddleName)) existing.MiddleName = parsed.MiddleName;

                        existing.ETSDate = parsed.ETSDate;
                        existing.DateOfBirth = parsed.DateOfBirth;
                        existing.DateOfRank = parsed.DateOfRank;
                        existing.Rank = parsed.Rank;
                    }

                    changed.Add(parsed);
                }
            }

            await db.SaveChangesAsync();

            return changed;
        }
    }
}