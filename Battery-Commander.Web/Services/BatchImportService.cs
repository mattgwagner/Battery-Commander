using BatteryCommander.Web.Models;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
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

            using (var excel = ExcelReaderFactory.CreateBinaryReader(stream))
            {
                // Unit First Name Last Name Middle Initial EDIPI   Personnel Status    Rank Pay Grade Skill Level PMOS    SMOS BASD    PCS DOR ETS Height  Weight HW Pass Body Fat Percentage Body Fat Pass Weapon Type Weapon Assignment Primary Weapon Latest Qualification Date   Number of Hits / Points Weapon Qualification Rating Form Number Night Fire CBRN Fire With Optics

                // Process Excel upload

                do
                {
                    while (excel.Read())
                    {
                        // Skip header row

                        if (excel.GetString(0) != "Unit" && excel[4] != null)
                        {
                            var dodId = excel.GetString(4);

                            var soldier =
                                await db
                                .Soldiers
                                .Where(_ => _.DoDId == dodId || (_.LastName.ToUpper() == excel.GetString(2).ToUpper() && _.FirstName.ToUpper() == excel.GetString(1).ToUpper()))
                                .FirstOrDefaultAsync();

                            if (soldier == null)
                            {
                                soldier = new Soldier
                                {
                                    UnitId = unitId,

                                    FirstName = excel.GetString(1),
                                    LastName = excel.GetString(2)
                                };

                                db.Soldiers.Add(soldier);
                            }

                            soldier.MiddleName = excel.GetString(3);

                            soldier.DoDId = dodId;

                            soldier.Rank = RankExtensions.Parse(excel.GetString(6));

                            if (excel[13] != null)
                                soldier.DateOfRank = excel.GetDateTime(13);

                            if (excel[14] != null)
                                soldier.ETSDate = excel.GetDateTime(14);

                            // TODO Load MOS, Weapons Qual, Latest HT/WT

                            try
                            {
                                await db.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                throw new System.Exception("Error importing DODID " + dodId, ex);
                            }
                        }
                    }
                }
                while (excel.NextResult());
            }

            return changed;
        }
    }
}