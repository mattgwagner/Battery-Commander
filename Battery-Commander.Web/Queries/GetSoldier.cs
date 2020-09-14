using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
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
                return await GetAsync(db, soldier => soldier.Id == query.Id);
            }
        }

        public static async Task<Soldier> GetAsync(Database database, Expression<Func<Soldier, Boolean>> filter)
        {
            var soldier =
                await database
                .Soldiers
                .Include(s => s.Supervisor)
                .Include(s => s.SSDSnapshots)
                .Include(s => s.ABCPs)
                .Include(s => s.ACFTs)
                .Include(s => s.APFTs)
                .Include(s => s.Unit)
                .Where(filter)
                .SingleOrDefaultAsync();

            return soldier;
        }
    }
}