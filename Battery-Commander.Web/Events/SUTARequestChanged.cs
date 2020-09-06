using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using MediatR;

namespace BatteryCommander.Web.Events
{
    public class SUTARequestChanged : INotification
    {
        public int Id { get; set; }

        public EventType Event { get; set; }

        public enum EventType
        {
            Created,
            Updated,
            Commented,
            SupervisorSigned,
            Approved
        }

        private class Handler : INotificationHandler<SUTARequestChanged>
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

            public async Task Handle(SUTARequestChanged notification, CancellationToken cancellationToken)
            {
                var suta = await dispatcher.Send(new GetSUTARequest { Id = notification.Id });

                var recipients = new List<Address>();

                recipients.AddRange(suta.Supervisor.GetEmails());

                foreach (var soldier in await SoldierService.Filter(db, new SoldierService.Query { Ranks = new[] { Rank.E7, Rank.E8, Rank.E9, Rank.O1, Rank.O2, Rank.O3 }, Unit = suta.Soldier.UnitId }))
                {
                    if (soldier.Id == suta.SupervisorId) continue;

                    recipients.AddRange(soldier.GetEmails());
                }

                var subject = notification.Event switch
                {
                    EventType.Created => $"[Action Required] SUTA Request {suta.Soldier}",
                    EventType.Updated => $"SUTA Request {suta.Soldier} - Updated",
                    EventType.Commented => $"SUTA Request {suta.Soldier} - Comment Added",
                    EventType.Approved => $"[Approved] SUTA Request {suta.Soldier}",
                    EventType.SupervisorSigned => $"[Approved] SUTA Request {suta.Soldier}"
                };

                await
                    emailSvc
                    .Create()
                    .To(recipients)
                    .Subject(subject)
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Events/SUTARequestChanged.html", suta)
                    .SendWithErrorCheck();
            }
        }
    }
}
