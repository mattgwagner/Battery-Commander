using System;
using System.ComponentModel.DataAnnotations;
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
    public class UpdateSUTARequest : IRequest
    {
        [FromRoute]
        public int Id { get; set; }

        [FromForm]
        public Detail Body { get; set; }

        public class Detail
        {
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }

            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; }

            public String Reasoning { get; set; }

            public String MitigationPlan { get; set; }
        }

        private class Handler : AsyncRequestHandler<UpdateSUTARequest>
        {
            private readonly Database db;
            private readonly IMediator dispatcher;

            public Handler(Database db, IMediator dispatcher)
            {
                this.db = db;
                this.dispatcher = dispatcher;
            }

            protected override async Task Handle(UpdateSUTARequest request, CancellationToken cancellationToken)
            {
                var current_user = await dispatcher.Send(new GetCurrentUser { });

                var suta =
                    await db
                    .SUTAs
                    .Where(s => s.Id == request.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                suta.StartDate = request.Body.StartDate;
                suta.EndDate = request.Body.EndDate;
                suta.Reasoning = request.Body.Reasoning;
                suta.MitigationPlan = request.Body.MitigationPlan;

                suta.Events.Add(new SUTA.Event
                {
                    Author = $"{current_user}",
                    Message = "Request Updated"
                });

                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
