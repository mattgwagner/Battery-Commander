using BatteryCommander.Web.Events;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Commands
{
    public class UpdateSUTARequest : IRequest
    {
        public static UpdateSUTARequest Build(SUTA model)
        {
            return new UpdateSUTARequest
            {
                Id = model.Id,
                Body = new Detail
                {
                    Supervisor = model.SupervisorId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Reasoning = model.Reasoning,
                    MitigationPlan = model.MitigationPlan,
                    Archived = model.Archived
                }
            };
        }

        [FromRoute]
        public int Id { get; set; }

        [FromForm]
        public Detail Body { get; set; }

        public class Detail
        {
            public int Supervisor { get; set; }

            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }

            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; }

            [Display(Name = "The reason to request missing IDT:")]
            public String Reasoning { get; set; }

            [Display(Name = "If requesting late arrival, what is the expected ETA? How are we mitigating the loss of this Soldier during the requested period?")]
            public String MitigationPlan { get; set; }

            public Boolean Archived { get; set; }
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

                var suta = await GetSUTARequest.ById(db, request.Id);

                suta.SupervisorId = request.Body.Supervisor;
                suta.StartDate = request.Body.StartDate;
                suta.EndDate = request.Body.EndDate;
                suta.Reasoning = request.Body.Reasoning;
                suta.MitigationPlan = request.Body.MitigationPlan;
                suta.Archived = request.Body.Archived;

                suta.Events.Add(new SUTA.Event
                {
                    Author = $"{current_user}",
                    Message = "Request Updated"
                });

                await db.SaveChangesAsync(cancellationToken);

                if(!request.Body.Archived)
                {
                    await dispatcher.Publish(new SUTARequestChanged
                    {
                        Id = suta.Id,
                        Event = SUTARequestChanged.EventType.Updated
                    });
                }
            }
        }
    }
}
