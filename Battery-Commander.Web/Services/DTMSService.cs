using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Http;
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

            using (var excel = new ExcelPackage())
            {
                excel.Load(stream);

                // LastName MiddleName  FirstName RankShort   Last4 PayGradeCode    MOS SMOS    DateOfRank BASD UnitAssigned UnitAssigned    Status

                // Process Excel upload

                var sheet = excel.Workbook.Worksheets.First();

                // Skip header row

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    // Parse into soldier

                    var parsed = new Soldier
                    {
                        LastName = $"{sheet.Cells[row, 1].Value}",
                        MiddleName = $"{sheet.Cells[row, 2].Value}",
                        FirstName = $"{sheet.Cells[row, 3].Value}",
                        Rank = RankExtensions.Parse($"{sheet.Cells[row, 4]}"),
                        DateOfRank = Convert.ToDateTime(sheet.Cells[row, 9].Value)
                    };

                    // Check for existing, create if not

                    // TODO Update existing entries?

                    if (!await db.Soldiers.Where(_ => _.LastName.ToUpper() == parsed.LastName.ToUpper()).Where(_ => _.FirstName.ToUpper() == parsed.FirstName.ToUpper()).AnyAsync())
                    {
                        // Soldier does not exist, create

                        await db.Soldiers.AddAsync(parsed);

                        changed.Add(parsed);
                    }
                }
            }

            await db.SaveChangesAsync();

            return changed;
        }
    }
}