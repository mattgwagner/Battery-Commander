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
    public class AddSUTAComment : IRequest
    {
        [FromRoute]
        public int Id { get; set; }

        [FromForm]
        public string Comment { get; set; }

        private class Handler : AsyncRequestHandler<AddSUTAComment>
        {
            private readonly Database db;
            private readonly IMediator dispatcher;

            public Handler(Database db, IMediator dispatcher)
            {
                this.db = db;
                this.dispatcher = dispatcher;
            }

            protected override async Task Handle(AddSUTAComment request, CancellationToken cancellationToken)
            {
                var suta =
                    await db
                    .SUTAs
                    .Where(s => s.Id == request.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                var current_user = await dispatcher.Send(new GetCurrentUser { });

                suta.Events.Add(new SUTA.Event
                {
                    Author = $"{current_user}",
                    Message = request.Comment
                });

                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
