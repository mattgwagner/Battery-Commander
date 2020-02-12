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

        public string Event { get; set; }

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

                await
                    emailSvc
                    .Create()
                    .To(recipients)
                    .BCC("SUTAs@RedLeg.app")
                    .Subject($"{suta.Soldier.Unit.UIC} | SUTA Request {notification.Event}")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/SUTA/Email.html", suta)
                    .SendWithErrorCheck();
            }
        }
    }
}
