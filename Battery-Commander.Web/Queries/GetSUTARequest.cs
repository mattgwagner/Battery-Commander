﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Queries
{
    public class GetSUTARequest : IRequest<SUTA>
    {
        [FromRoute]
        public int Id { get; set; }

        private class Handler : IRequestHandler<GetSUTARequest, SUTA>
        {
            private readonly Database db;

            public Handler(Database db)
            {
                this.db = db;
            }

            public async Task<SUTA> Handle(GetSUTARequest request, CancellationToken cancellationToken)
            {
                return await ById(db, request.Id);
            }
        }

        public static async Task<SUTA> ById(Database db, int id)
        {
            return
                await GetSUTARequests
                .AsQueryable(db)
                .Where(suta => suta.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}
