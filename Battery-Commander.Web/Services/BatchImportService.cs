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
                // 0 LastName
                // 1 MiddleName
                // 2 FirstName
                // 3 Rank
                // 4 DateOfBirth
                // 5 ETSDate
                // 6 DateOfRank
                // 7 Gender
                // 8 Unit
                // 9 DODID
                // 10 MilitaryEmail
                // 11 EducationLevel

                // Process Excel upload

                var sheet = excel.Workbook.Worksheets.First();

                // Skip header row

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var unit =
                        units
                        .Where(u => !String.IsNullOrWhiteSpace(u.Name))
                        .Where(u => u.Name == (String)sheet.Cells[row, 8].Value)
                        .SingleOrDefault();

                    if(unit == null)
                    {
                        // Skipping
                        continue;
                    }

                    // Parse into soldier

                    var parsed = new Soldier
                    {
                        LastName = $"{sheet.Cells[row, 0].Value}",
                        MiddleName = $"{sheet.Cells[row, 1].Value}",
                        FirstName = $"{sheet.Cells[row, 2].Value}",
                        Rank = RankExtensions.Parse($"{sheet.Cells[row, 3].Value}"),
                        DateOfBirth = Convert.ToDateTime(sheet.Cells[row, 4].Value),
                        ETSDate = Convert.ToDateTime(sheet.Cells[row, 5].Value),
                        DateOfRank = Convert.ToDateTime(sheet.Cells[row, 6].Value),
                        Gender = $"{sheet.Cells[row, 7].Value}" == "Male" ? Gender.Male : Gender.Female,
                        DoDId = $"{sheet.Cells[row, 9]}",
                        MilitaryEmail = $"{sheet.Cells[row, 10]}",
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