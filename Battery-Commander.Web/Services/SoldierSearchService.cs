using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class SoldierSearchService
    {
        public static async Task<IEnumerable<Soldier>> Subordinates(Database db, int soldierId)
        {
            var subordinates = new List<Soldier>();

            // Recursively call and find subordinates of the given soldier

            foreach (Soldier soldier in db.Soldiers)
            {
                if (soldier.SupervisorId == soldierId)
                {
                    subordinates.Add(soldier);

                    subordinates.AddRange(await Subordinates(db, soldier.Id));
                }
            }

            return subordinates;
        }

        public static async Task<IEnumerable<Soldier>> Filter(Database db, Query query)
        {
            IEnumerable<Soldier> soldiers =
                await db
                .Soldiers
                .Include(s => s.Supervisor)
                .Include(s => s.SSDSnapshots)
                .Include(s => s.ABCPs)
                .Include(s => s.APFTs)
                .Include(s => s.Unit)
                .ToListAsync();

            if (query.Id.HasValue)
            {
                soldiers = soldiers.Where(_ => _.Id == query.Id);
            }

            if (query.Ids?.Any() == true)
            {
                soldiers = soldiers.Where(_ => query.Ids.Contains(_.Id));
            }

            if (query.Unit.HasValue)
            {
                soldiers = soldiers.Where(_ => _.UnitId == query.Unit);
            }
            else if (query.IncludeIgnoredUnits == false)
            {
                soldiers = soldiers.Where(_ => !_.Unit.IgnoreForReports);
            }

            if (query.OnlyEnlisted == true)
            {
                query.Ranks = RankExtensions.Enlisted;
            }

            if (query.Ranks?.Any() == true)
            {
                soldiers = soldiers.Where(_ => query.Ranks.Contains(_.Rank));
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

            if (query.DSCA.HasValue)
            {
                soldiers = soldiers.Where(_ => _.DscaQualified == query.DSCA);
            }

            if (query.Gender.HasValue)
            {
                soldiers = soldiers.Where(_ => _.Gender == query.Gender);
            }

            if (query.IWQ.HasValue)
            {
                soldiers = soldiers.Where(_ => _.IwqQualified == query.IWQ);
            }

            if (query.EducationComplete.HasValue)
            {
                soldiers = soldiers.Where(_ => _.IsEducationComplete == query.EducationComplete);
            }

            if (query.CLS.HasValue)
            {
                soldiers = soldiers.Where(_ => _.ClsQualified == query.CLS);
            }

            if (query.Status.HasValue)
            {
                soldiers = soldiers.Where(_ => _.Status == query.Status);
            }

            if (!String.IsNullOrWhiteSpace(query.Email))
            {
                soldiers = soldiers.Where(soldier => soldier.CivilianEmail == query.Email || soldier.MilitaryEmail == query.Email);
            }

            return
                soldiers
                .OrderBy(soldier => soldier.LastName)
                .ThenBy(soldier => soldier.FirstName)
                .ToList();
        }

        public class Query
        {
            // TODO Filtering by MOS, Position, Name, Status

            public int? Id { get; set; }

            public int[] Ids { get; set; }

            public int? Unit { get; set; }

            public IEnumerable<Rank> Ranks { get; set; }

            public Boolean? OnlyEnlisted { get; set; }

            public Boolean? IncludeIgnoredUnits { get; set; } = false;

            public EventStatus? ABCP { get; set; }

            public EventStatus? APFT { get; set; }

            public Boolean? DSCA { get; set; }

            public Boolean? IWQ { get; set; }

            public Gender? Gender { get; set; }

            public Boolean? SSD { get; set; }

            public Boolean? CLS { get; set; }

            public Boolean? EducationComplete { get; set; }

            public Soldier.SoldierStatus? Status { get; set; }

            public String Email { get; set; }
        }
    }

    public static class SoldierHtmlHelper
    {
        public static IHtmlContent Soldiers(this HtmlHelper helper, String link_text, SoldierSearchService.Query query)
        {
            return helper.RouteLink(link_text, "Soldiers.List", query);
        }
    }
}