using BatteryCommander.Web.Events;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

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
                var suta = await GetSUTARequest.ById(db, request.Id);

                var current_user = await dispatcher.Send(new GetCurrentUser { });

                suta.Events.Add(new SUTA.Event
                {
                    Author = $"{current_user}",
                    Message = request.Comment
                });

                await db.SaveChangesAsync(cancellationToken);

                await dispatcher.Publish(new SUTARequestChanged
                {
                    Id = suta.Id,
                    Event = SUTARequestChanged.EventType.Commented
                });
            }
        }
    }
}
