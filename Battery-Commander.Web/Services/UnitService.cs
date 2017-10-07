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
        public static async Task<IEnumerable<Unit>> List(Database db, Boolean includeIgnored = false)
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
                .Where(unit => includeIgnored || !unit.IgnoreForReports)
                .OrderBy(unit => unit.Name)
                .ToListAsync();
        }
    }
}