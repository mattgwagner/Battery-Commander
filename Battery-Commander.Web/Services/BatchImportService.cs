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
        public static async Task<IEnumerable<Soldier>> ImportSoldiers(Database db, Stream stream, int unitId)
        {
            var changed = new List<Soldier>();

            var units = db.Units.ToList();

            using (var excel = new ExcelPackage(stream))
            {
                // Unit First Name Last Name Middle Initial EDIPI   Personnel Status    Rank Pay Grade Skill Level PMOS    SMOS BASD    PCS DOR ETS Height  Weight HW Pass Body Fat Percentage Body Fat Pass Weapon Type Weapon Assignment Primary Weapon Latest Qualification Date   Number of Hits / Points Weapon Qualification Rating Form Number Night Fire CBRN Fire With Optics

                // Process Excel upload

                var sheet = excel.Workbook.Worksheets.First();

                // Skip header row

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var soldier =
                        await db
                        .Soldiers
                        .Where(_ => _.LastName.ToUpper() == $"{sheet.Cells[row, 3].Value}".ToUpper())
                        .Where(_ => _.FirstName.ToUpper() == $"{sheet.Cells[row, 2].Value}".ToUpper())
                        .Where(_ => _.UnitId == unitId)
                        .FirstOrDefaultAsync();

                    if (soldier == null)
                    {
                        soldier = new Soldier
                        {
                            UnitId = unitId,

                            FirstName = $"{sheet.Cells[row, 2].Value}",
                            LastName = $"{sheet.Cells[row, 3].Value}"
                        };

                        db.Soldiers.Add(soldier);
                    }

                    soldier.MiddleName = $"{sheet.Cells[row, 4].Value}";

                    soldier.DoDId = $"{sheet.Cells[row, 5]}";

                    soldier.Rank = RankExtensions.Parse($"{sheet.Cells[row, 6].Value}");

                    soldier.DateOfRank = Convert.ToDateTime(sheet.Cells[row, 13].Value);

                    soldier.ETSDate = Convert.ToDateTime(sheet.Cells[row, 14].Value);

                    // TODO Load MOS, Weapons Qual, Latest HT/WT
                }
            }

            await db.SaveChangesAsync();

            return changed;
        }
    }
}