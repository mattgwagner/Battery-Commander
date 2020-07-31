using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Queries
{
    public class GetSoldier : IRequest<Soldier>
    {
        public int Id { get; }

        public GetSoldier(int id)
        {
            Id = id;
        }

        private class Handler : IRequestHandler<GetSoldier, Soldier>
        {
            private readonly Database db;

            public Handler(Database db)
            {
                this.db = db;
            }

            public async Task<Soldier> Handle(GetSoldier query, CancellationToken cancellationToken)
            {
                var soldier =
                    await db
                    .Soldiers
                    .Include(s => s.Supervisor)
                    .Include(s => s.SSDSnapshots)
                    .Include(s => s.ABCPs)
                    .Include(s => s.ACFTs)
                    .Include(s => s.APFTs)
                    .Include(s => s.Unit)
                    .Where(s => s.Id == query.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                return soldier;
            }
        }
    }
}