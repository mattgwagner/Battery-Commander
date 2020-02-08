using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Commands
{
    public class DeleteSUTARequest : IRequest
    {
        [FromRoute]
        public int Id { get; set; }

        private class Handler : AsyncRequestHandler<DeleteSUTARequest>
        {
            private readonly Database db;

            public Handler(Database db)
            {
                this.db = db;
            }

            protected override async Task Handle(DeleteSUTARequest request, CancellationToken cancellationToken)
            {
                var suta =
                    await db
                    .SUTAs
                    .Where(s => s.Id == request.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                db.SUTAs.Remove(suta);

                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
