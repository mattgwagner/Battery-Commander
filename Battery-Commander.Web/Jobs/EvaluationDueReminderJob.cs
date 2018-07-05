using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;

namespace BatteryCommander.Web.Jobs
{
    public class EvaluationDueReminderJob : IJob
    {
        private readonly Database db;

        private readonly IFluentEmail emailSvc;

        public EvaluationDueReminderJob(Database db, IFluentEmail emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public void Execute()
        {
            var soon = DateTime.Today.AddDays(15);

            // TODO Filter this if multiple units in-use

            var evaluations_due_soon =
                db
                .Evaluations
                .Include(evaluation => evaluation.Ratee)
                .Include(evaluation => evaluation.Rater)
                .Include(evaluation => evaluation.SeniorRater)
                .Include(evaluation => evaluation.Reviewer)
                .Where(evaluation => evaluation.IsCompleted == false)
                .Where(evaluation => evaluation.ThruDate < soon)
                .OrderBy(evaluation => evaluation.ThruDate)
                .ToList();

            var sb =
                new StringBuilder()
                .Append("<ul>");

            foreach (var evaluation in evaluations_due_soon)
            {
                sb.AppendLine($"<li>{evaluation.Ratee} due {evaluation.ThruDate:d}</li>");
            }

            sb = sb.AppendLine("</ul>");

            var recipients = SoldierSearchService.Filter(db, new SoldierSearchService.Query
            {
                Ranks = new[] { Rank.E7, Rank.E8, Rank.O1, Rank.O2, Rank.O3 }
            })
            .GetAwaiter()
            .GetResult()
            .Where(soldier => !String.IsNullOrWhiteSpace(soldier.CivilianEmail))
            .Select(soldier => new Address { EmailAddress = soldier.CivilianEmail, Name = soldier.ToString() })
            .ToList();

            emailSvc
                .To(recipients)
                .Subject("Upcoming and Past Due Evaluations")
                .Body(sb.ToString())
                .Send();
        }
    }
}