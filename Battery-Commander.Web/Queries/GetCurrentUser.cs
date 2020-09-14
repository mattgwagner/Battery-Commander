using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

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
                    .Name?
                    .ToLower();

                if (string.IsNullOrWhiteSpace(email)) return default(Soldier);

                return await GetSoldier.GetAsync(db, soldier => soldier.CivilianEmail.ToLower() == email);
            }
        }
    }
}
