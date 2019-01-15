using BatteryCommander.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class UnitService
    {
        public static async Task<Unit> Get(Database db, int unitId)
        {
            return (await List(db)).Single(unit => unit.Id == unitId);
        }

        public static async Task<IEnumerable<Unit>> List(Database db)
        {
            return
                await db
                .Units
                .Include(unit => unit.Vehicles)
                .Include(unit => unit.Soldiers)
                    .ThenInclude(soldier => soldier.ABCPs)
                .Include(unit => unit.Soldiers)
                    .ThenInclude(soldier => soldier.APFTs)
                .Include(unit => unit.Soldiers)
                    .ThenInclude(soldier => soldier.SSDSnapshots)
                .OrderBy(unit => unit.Name)
                .ToListAsync();
        }
    }
}