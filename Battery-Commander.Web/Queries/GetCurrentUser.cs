using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Queries
{
    public class GetCurrentUser : IRequest<Soldier>
    {
        private class Handler : IRequestHandler<GetCurrentUser, Soldier>
        {
            private readonly Database db;
            private readonly IHttpContextAccessor http;

            public Handler(Database db, IHttpContextAccessor http)
            {
                this.db = db;
                this.http = http;
            }

            public async Task<Soldier> Handle(GetCurrentUser request, CancellationToken cancellationToken)
            {
                var email =
                    http
                    .HttpContext
                    .User?
                    .Identity?
                    .Name;

                if (string.IsNullOrWhiteSpace(email)) return default(Soldier);

                var soldier =
                    await db
                    .Soldiers
                    .Include(s => s.Supervisor)
                    .Include(s => s.SSDSnapshots)
                    .Include(s => s.ABCPs)
                    .Include(s => s.ACFTs)
                    .Include(s => s.APFTs)
                    .Include(s => s.Unit)
                    .Where(s => s.CivilianEmail == email)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(cancellationToken);

                return soldier;
            }
        }
    }
}
