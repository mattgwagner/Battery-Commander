using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class DTMSService
    {
        public static async Task<IEnumerable<Soldier>> ImportSoldiers(Database db, IFormFile file)
        {
            var changed = new List<Soldier>();

            using (var excel = new ExcelPackage(file.OpenReadStream()))
            {
                // LastName MiddleName  FirstName RankShort   Last4 PayGradeCode    MOS SMOS    DateOfRank BASD UnitAssigned UnitAssigned    Status

                // Process Excel upload

                var sheet = excel.Workbook.Worksheets.First();

                for (int row = 1; row <= sheet.Dimension.Rows; row++)
                {
                    // Parse into soldier

                    // Check for existing, update if found, create if not

                    changed.Add(new Soldier
                    {
                        LastName = $"{sheet.Cells[row, 1].Value}",
                        MiddleName = $"{sheet.Cells[row, 2].Value}",
                        FirstName = $"{sheet.Cells[row, 3].Value}",
                        Rank = RankExtensions.Parse($"{sheet.Cells[row, 4]}"),
                        DateOfRank = Convert.ToDateTime(sheet.Cells[row, 9].Value)
                    });
                }
            }

            return changed;
        }
    }
}