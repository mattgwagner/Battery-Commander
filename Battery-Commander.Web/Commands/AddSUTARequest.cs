using System;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Queries;
using BatteryCommander.Web.Models;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Commands
{
    public class AddSUTARequest : IRequest<int>
    {
        public int Soldier { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public String Reasoning { get; set; }

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
                    .Where(s => s.Id == request.Soldier)
                    .SingleOrDefaultAsync(cancellationToken);

                var suta = new SUTA
                {
                    SoldierId = request.Soldier,
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

                return suta.Id;
            }
        }
    }
}
