using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Queries
{
    public class GetSoldiers : IRequest<IEnumerable<Soldier>>
    {
        public int? Id { get; set; }

        public int[] Ids { get; set; }

        public int? Unit { get; set; }

        public IEnumerable<Rank> Ranks { get; set; }

        public Boolean? OnlyEnlisted { get; set; }

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

        private class Handler : IRequestHandler<GetSoldiers, IEnumerable<Soldier>>
        {
            private readonly Database db;

            public Handler(Database db)
            {
                this.db = db;
            }

            public async Task<IEnumerable<Soldier>> Handle(GetSoldiers query, CancellationToken cancellationToken)
            {
                var soldiers =
                    db
                    .Soldiers
                    .Include(s => s.Supervisor)
                    .Include(s => s.SSDSnapshots)
                    .Include(s => s.ABCPs)
                    .Include(s => s.ACFTs)
                    .Include(s => s.APFTs)
                    .Include(s => s.Unit)
                    .AsNoTracking();

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
                    soldiers = soldiers.Where(soldier => soldier.CivilianEmail.ToUpper() == query.Email.ToUpper() || soldier.MilitaryEmail.ToUpper() == query.Email.ToUpper());
                }

                return soldiers;
            }
        }
    }
}
