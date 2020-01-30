using System;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public String Reasoning { get; set; }

            public String MitigationPlan { get; set; }
        }

        private class Handler : AsyncRequestHandler<UpdateSUTARequest>
        {
            private readonly Database db;

            public Handler(Database db)
            {
                this.db = db;
            }

            protected override async Task Handle(UpdateSUTARequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
