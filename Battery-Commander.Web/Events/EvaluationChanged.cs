﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Events
{
    public class EvaluationChanged : INotification
    {
        public int Id { get; set; }

        private class Handler : INotificationHandler<EvaluationChanged>
        {
            private readonly Database db;
            private readonly IFluentEmailFactory emailSvc;

            public Handler(Database db, IFluentEmailFactory emailSvc)
            {
                this.db = db;
                this.emailSvc = emailSvc;
            }

            public async Task Handle(EvaluationChanged notification, CancellationToken cancellationToken)
            {
                var eval =
                    await db
                    .Evaluations
                    .Include(evaluation => evaluation.Events)
                    .Include(evaluation => evaluation.Ratee)
                    .Include(evaluation => evaluation.Rater)
                    .Include(evaluation => evaluation.SeniorRater)
                    .Include(evaluation => evaluation.Reviewer)
                    .Where(evaluation => evaluation.Id == notification.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                emailSvc
                    .Create()
                    .To(await Get_Recipients(eval))
                    .BCC("Evaluations.RedLeg.app")
                    .Subject($"Evaluation Updated | {eval.Ratee}")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Events/EvaluationUpdated.html", eval)
                    .SendWithErrorCheck()
                    .Wait();
            }

            private class EmailComparer : IEqualityComparer<Address>
            {
                public bool Equals(Address x, Address y)
                {
                    return String.Equals(x.EmailAddress, y.EmailAddress, StringComparison.CurrentCultureIgnoreCase);
                }

                public int GetHashCode(Address obj) => obj.GetHashCode();
            }

            private async Task<List<Address>> Get_Recipients(Evaluation evaluation)
            {
                var addresses = new List<Address>();

                addresses.AddRange(evaluation.Rater.GetEmails());

                addresses.AddRange(evaluation.SeniorRater.GetEmails());

                addresses.AddRange(evaluation.Reviewer.GetEmails());

                // Include the unit 1SG on event notifications

                foreach (var soldier in await SoldierService.Filter(db, new SoldierService.Query { Ranks = new[] { Rank.E8, Rank.E9 }, Unit = evaluation.Ratee.UnitId }))
                {
                    if (soldier.Id == evaluation.RaterId) continue;
                    if (soldier.Id == evaluation.SeniorRaterId) continue;
                    if (soldier.Id == evaluation.ReviewerId) continue;

                    addresses.AddRange(soldier.GetEmails());
                }

                return
                    addresses
                    .Distinct(new EmailComparer { })
                    .ToList();
            }
        }
    }
}