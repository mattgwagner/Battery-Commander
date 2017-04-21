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
    public class DTMSService
    {
        public static async Task<IEnumerable<Soldier>> ImportSoldiers(Database db, Stream stream)
        {
            var changed = new List<Soldier>();

            using (var excel = new ExcelPackage(stream))
            {
                // LastName MiddleName  FirstName RankShort   Last4 PayGradeCode    MOS SMOS    DateOfRank BASD UnitAssigned UnitAssigned    Status

                // Process Excel upload

                var sheet = excel.Workbook.Worksheets.First();

                // Skip header row

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    // TODO Determine unit

                    // TODO Where can we get DoB?

                    var unit = db.Units.FirstOrDefault();

                    // Parse into soldier

                    var parsed = new Soldier
                    {
                        LastName = $"{sheet.Cells[row, 1].Value}",
                        MiddleName = $"{sheet.Cells[row, 2].Value}",
                        FirstName = $"{sheet.Cells[row, 3].Value}",
                        Rank = RankExtensions.Parse($"{sheet.Cells[row, 4].Value}"),
                        DateOfRank = Convert.ToDateTime(sheet.Cells[row, 9].Value),
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

                        existing.MiddleName = parsed.MiddleName;
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