using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Queries
{
    public class GetSUTARequests : IRequest<IEnumerable<SUTA>>
    {
        [FromRoute]
        public int Unit { get; set; }

        // Search by Soldier, Status, Dates

        private class Handler : IRequestHandler<GetSUTARequests, IEnumerable<SUTA>>
        {
            private readonly Database db;

            public Handler(Database db)
            {
                this.db = db;
            }

            public async Task<IEnumerable<SUTA>> Handle(GetSUTARequests request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
