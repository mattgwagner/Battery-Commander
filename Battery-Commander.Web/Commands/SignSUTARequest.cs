using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
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
            private readonly IFluentEmailFactory emailSvc;

            public Handler(Database db, IMediator dispatcher, IFluentEmailFactory emailSvc)
            {
                this.db = db;
                this.dispatcher = dispatcher;
                this.emailSvc = emailSvc;
            }

            protected override async Task Handle(SignSUTARequest request, CancellationToken cancellationToken)
            {
                var suta = await GetSUTARequest.ById(db, request.Id);

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

                    suta.Status = SUTA.SUTAStatus.Approved;
                }

                suta.Events.Add(new SUTA.Event
                {
                    Author = $"{current_user}",
                    Message = "Signature added"
                });

                await db.SaveChangesAsync(cancellationToken);

                if(suta.Status == SUTA.SUTAStatus.Approved)
                {
                    await Notify_Leadership_Of_Approval(suta.Id);
                }
            }

            private async Task Notify_Leadership_Of_Approval(int id)
            {
                var suta = await dispatcher.Send(new GetSUTARequest { Id = id });

                var recipients = new List<Address>();

                foreach (var email in suta.Supervisor.GetEmails()) recipients.Add(email);

                foreach (var soldier in await SoldierService.Filter(db, new SoldierService.Query { Ranks = new[] { Rank.E7, Rank.E8, Rank.E9, Rank.O1, Rank.O2, Rank.O3 }, Unit = suta.Soldier.UnitId }))
                {
                    if (soldier.CanLogin)
                    {
                        foreach (var email in soldier.GetEmails()) recipients.Add(email);
                    }
                }

                await
                    emailSvc
                    .Create()
                    .To(recipients)
                    .BCC("SUTAs@RedLeg.app")
                    .Subject($"{suta.Soldier.Unit} | SUTA Request Approved")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/SUTA/Email.html", suta)
                    .SendWithErrorCheck();
            }
        }
    }
}
