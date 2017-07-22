﻿using BatteryCommander.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class SoldierSearchService
    {
        public static async Task<IEnumerable<Soldier>> Filter(Database db, Query query)
        {
            IQueryable<Soldier> soldiers =
                db
                .Soldiers
                .Include(s => s.SSDSnapshots)
                .Include(s => s.ABCPs)
                .Include(s => s.APFTs)
                .Include(s => s.Unit);

            if (query.Unit.HasValue)
            {
                soldiers = soldiers.Where(_ => _.UnitId == query.Unit);
            }
            else if (query.IncludeIgnoredUnits == false)
            {
                soldiers = soldiers.Where(_ => !_.Unit.IgnoreForReports);
            }

            if (query.Rank.HasValue)
            {
                soldiers = soldiers.Where(_ => _.Rank == query.Rank);
            }

            if (query.OnlyEnlisted == true)
            {
                soldiers = soldiers.Where(_ => _.IsEnlisted);
            }

            if (query.ABCP.HasValue)
            {
                soldiers = soldiers.Where(_ => _.AbcpStatus == query.ABCP);
            }

            if (query.APFT.HasValue)
            {
                soldiers = soldiers.Where(_ => _.ApftStatus == query.APFT);
            }

            if(query.SSD.HasValue)
            {
                soldiers = soldiers.Where(_ => _.SSDStatus.CurrentProgress >= Decimal.One);
            }

            return
                await soldiers
                .OrderBy(soldier => soldier.LastName)
                .ThenBy(soldier => soldier.FirstName)
                .ToListAsync();
        }

        public class Query
        {
            // TODO Filtering by MOS, Position, Name, Status

            public int? Unit { get; set; }

            public Rank? Rank { get; set; }

            public Boolean? OnlyEnlisted { get; set; }

            public Boolean? IncludeIgnoredUnits { get; set; } = false;

            public Soldier.EventStatus? ABCP { get; set; }

            public Soldier.EventStatus? APFT { get; set; }

            public Boolean? SSD { get; set; }
        }
    }
}