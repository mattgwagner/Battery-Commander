﻿using BatteryCommander.Web.Events;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Commands
{
    public class AddSUTARequest : IRequest<int>
    {
        public int Soldier { get; set; }

        public int Supervisor { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "The reason to request missing IDT:")]
        public String Reasoning { get; set; }

        [Display(Name = "If requesting late arrival, what is the expected ETA? How are we mitigating the loss of this Soldier during the requested period?")]
        public String MitigationPlan { get; set; }

        private class Handler : IRequestHandler<AddSUTARequest, int>
        {
            private readonly Database db;
            private readonly IMediator dispatcher;

            public Handler(Database db, IMediator dispatcher)
            {
                this.db = db;
                this.dispatcher = dispatcher;
            }

            public async Task<int> Handle(AddSUTARequest request, CancellationToken cancellationToken)
            {
                var current_user = await dispatcher.Send(new GetCurrentUser { });

                var soldier =
                    await db
                    .Soldiers
                    .Include(s => s.Unit)
                    .Where(s => s.Id == request.Soldier)
                    .SingleOrDefaultAsync(cancellationToken);

                var suta = new SUTA
                {
                    SoldierId = request.Soldier,
                    SupervisorId = request.Supervisor,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Reasoning = request.Reasoning,
                    MitigationPlan = request.MitigationPlan
                };

                if (await db.SUTAs.AnyAsync(s => s.SoldierId == suta.SoldierId && s.StartDate == suta.StartDate && s.EndDate == suta.EndDate))
                {
                    // Already exists
                    return suta.Id;
                }

                suta.Events.Add(new SUTA.Event
                {
                    Author = $"{current_user ?? soldier}", // User MAY not be logged in
                    Message = "Created"
                });

                db.SUTAs.Add(suta);

                await db.SaveChangesAsync(cancellationToken);

                await dispatcher.Publish(new SUTARequestChanged
                {
                    Id = suta.Id,
                    Event = SUTARequestChanged.EventType.Created
                });

                return suta.Id;
            }
        }
    }
}
