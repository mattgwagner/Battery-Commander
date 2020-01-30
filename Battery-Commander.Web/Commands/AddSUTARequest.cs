using System;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;

namespace BatteryCommander.Web.Commands
{
    public class AddSUTARequest : IRequest<int>
    {
        public int Soldier { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public String Reasoning { get; set; }

        public String MitigationPlan { get; set; }

        private class Handler : IRequestHandler<AddSUTARequest, int>
        {
            private readonly Database db;

            public Handler(Database db)
            {
                this.db = db;
            }

            public async Task<int> Handle(AddSUTARequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
