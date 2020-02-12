using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Microsoft.EntityFrameworkCore;

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

        [Display(Name = "How are we mitigating the loss of this Soldier during the requested period? Include anticipate arrival time if partial request to miss.")]
        public String MitigationPlan { get; set; }

        private class Handler : IRequestHandler<AddSUTARequest, int>
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

            public async Task<int> Handle(AddSUTARequest request, CancellationToken cancellationToken)
            {
                var current_user = await dispatcher.Send(new GetCurrentUser { });

                var soldier =
                    await db
                    .Soldiers
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

                suta.Events.Add(new SUTA.Event
                {
                    Author = $"{current_user ?? soldier}", // User MAY not be logged in
                    Message = "Created"
                });

                db.SUTAs.Add(suta);

                await db.SaveChangesAsync(cancellationToken);

                await Notify_Leadership_Of_Request(suta);

                return suta.Id;
            }

            private async Task Notify_Leadership_Of_Request(SUTA suta)
            {
                // Might need to re-load the entity to get related data

                var recipients = new List<Address>();

                foreach (var soldier in await SoldierService.Filter(db, new SoldierService.Query { Ranks = new[] { Rank.E7, Rank.E8, Rank.E9, Rank.O1, Rank.O2, Rank.O3 }, Unit = suta.Soldier.UnitId }))
                {
                    if(soldier.CanLogin)
                    {
                        foreach (var email in soldier.GetEmails())
                        {
                            recipients.Add(new Address(email.EmailAddress, name: $"{soldier}"));
                        }
                    }                    
                }

                await
                    emailSvc
                    .Create()
                    .To(recipients)
                    .BCC("SUTAs@RedLeg.app")
                    .Subject($"{suta.Soldier.Unit} | SUTA Request Submitted")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/SUTA/Email.html", suta)
                    .SendWithErrorCheck();
            }
        }
    }
}
