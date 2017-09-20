using BatteryCommander.Web.Models;
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
                .Include(s => s.Supervisor)
                .Include(s => s.SSDSnapshots)
                .Include(s => s.ABCPs)
                .Include(s => s.APFTs)
                .Include(s => s.Unit);

            if (query.Id.HasValue)
            {
                soldiers = soldiers.Where(_ => _.Id == query.Id);
            }

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
                soldiers = soldiers.Where(_ => _.IsEnlisted || _.IsNCO);
            }

            if (query.ABCP.HasValue)
            {
                soldiers = soldiers.Where(_ => _.AbcpStatus == query.ABCP);
            }

            if (query.APFT.HasValue)
            {
                soldiers = soldiers.Where(_ => _.ApftStatus == query.APFT);
            }

            if (query.SSD.HasValue)
            {
                if (query.SSD == true)
                {
                    soldiers = soldiers.Where(_ => _.SSDStatus.CurrentProgress >= Decimal.One);
                }
                else
                {
                    soldiers = soldiers.Where(_ => _.SSDStatus.CurrentProgress < Decimal.One);
                }
            }
			
			if(query.DSCA.HasValue)
			{
				soldiers = soldiers.Where(_ => _.DscaQualified == query.DSCA);
			}

			if(query.Gender.HasValue)
			{
				soldiers = soldiers.Where(_ => _.Gender == query.Gender);
			}
			
            if (query.EducationComplete.HasValue)
            {
                soldiers = soldiers.Where(_ => _.IsEducationComplete == query.EducationComplete);
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

            public int? Id { get; set; }

            public int? Unit { get; set; }

            public Rank? Rank { get; set; }

            public Boolean? OnlyEnlisted { get; set; }

            public Boolean? IncludeIgnoredUnits { get; set; } = false;

            public Soldier.EventStatus? ABCP { get; set; }

            public Soldier.EventStatus? APFT { get; set; }
			
			public Boolean? DSCA { get; set; }
			
			// public Boolean? IWQ { get; set; }
			
			public Gender? Gender { get; set; }

            public Boolean? SSD { get; set; }

            public Boolean? EducationComplete { get; set; }
        }
    }
}