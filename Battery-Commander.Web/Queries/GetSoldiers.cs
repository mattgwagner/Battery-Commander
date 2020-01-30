using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Queries
{
    public class GetSoldiers : IRequest<IEnumerable<Soldier>>
    {
        // TODO Port over the filters from SoldierService

        private class Handler : IRequestHandler<GetSoldiers, IEnumerable<Soldier>>
        {
            private readonly Database db;

            public Handler(Database db)
            {
                this.db = db;
            }

            public async Task<IEnumerable<Soldier>> Handle(GetSoldiers request, CancellationToken cancellationToken)
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

                return soldiers;
            }
        }
    }
}
