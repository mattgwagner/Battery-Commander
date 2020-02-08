using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Commands
{
    public class SignSUTARequest : IRequest
    {
        [FromRoute]
        public int Id { get; set; }

        [FromForm]
        public Boolean Supervisor { get; set; }

        [FromForm]
        public Boolean FirstSergeant { get; set; }

        [FromForm]
        public Boolean Commander { get; set; }

        private class Handler : AsyncRequestHandler<SignSUTARequest>
        {
            private readonly Database db;
            private readonly IMediator dispatcher;

            public Handler(Database db, IMediator dispatcher)
            {
                this.db = db;
                this.dispatcher = dispatcher;
            }

            protected override async Task Handle(SignSUTARequest request, CancellationToken cancellationToken)
            {
                var suta =
                    await db
                    .SUTAs
                    .Where(s => s.Id == request.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                var current_user = await dispatcher.Send(new GetCurrentUser { });

                if (request.Supervisor)
                {
                    suta.SupervisorSignature = $"{current_user}";
                    suta.SupervisorSignedAt = DateTime.UtcNow;
                }

                if (request.FirstSergeant)
                {
                    suta.FirstSergeantSignature = $"{current_user}";
                    suta.FirstSergeantSignedAt = DateTime.UtcNow;
                }

                if (request.Commander)
                {
                    suta.CommanderSignature = $"{current_user}";
                    suta.CommanderSignedAt = DateTime.UtcNow;
                }

                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
