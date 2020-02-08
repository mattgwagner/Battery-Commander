using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Queries
{
    public class GetSUTARequests : IRequest<IEnumerable<SUTA>>
    {
        [FromRoute]
        public int Unit { get; set; }

        // Search by Soldier, Status, Dates

        private class Handler : IRequestHandler<GetSUTARequests, IEnumerable<SUTA>>
        {
            private readonly Database db;

            public Handler(Database db)
            {
                this.db = db;
            }

            public async Task<IEnumerable<SUTA>> Handle(GetSUTARequests request, CancellationToken cancellationToken)
            {
                return
                    await AsQueryable(db)
                    .Where(suta => suta.Soldier.UnitId == request.Unit)
                    .ToListAsync(cancellationToken);
            }
        }

        public static IQueryable<SUTA> AsQueryable(Database db)
        {
            return
                 db
                 .SUTAs
                 .Include(suta => suta.Soldier)
                 .ThenInclude(soldier => soldier.Unit)
                 .Include(suta => suta.Supervisor)
                 .Include(suta => suta.Events);
        }
    }
}
