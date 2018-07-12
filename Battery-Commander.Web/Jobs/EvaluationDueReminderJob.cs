using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace BatteryCommander.Web.Jobs
{
    public class EvaluationDueReminderJob : IJob
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<EvaluationDueReminderJob>();

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

            Log.Information("Building Evaluations Due email for evals due before {soon}", soon);

            var evaluations_due_soon =
                db
                .Evaluations
                .Include(evaluation => evaluation.Ratee)
                .Include(evaluation => evaluation.Rater)
                .Include(evaluation => evaluation.SeniorRater)
                .Include(evaluation => evaluation.Reviewer)
                .Include(evaluation => evaluation.Events)
                .Where(evaluation => evaluation.IsCompleted == false)
                .Where(evaluation => evaluation.ThruDate < soon)
                .OrderBy(evaluation => evaluation.ThruDate)
                .ToList();

            var recipients = SoldierSearchService.Filter(db, new SoldierSearchService.Query
            {
                Ranks = new[] { Rank.E6, Rank.E7, Rank.E8, Rank.O1, Rank.O2, Rank.O3 }
            })
            .GetAwaiter()
            .GetResult()
            .Where(soldier => !String.IsNullOrWhiteSpace(soldier.CivilianEmail))
            .Select(soldier => new Address { EmailAddress = soldier.CivilianEmail, Name = soldier.ToString() })
            .ToList();

            Log.Information("Sending evaluations email to {@recipients}", recipients);

            emailSvc
                .To(recipients)
                .Subject("Past Due and Upcoming Evaluations")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Reports/EvaluationsDue.cshtml", evaluations_due_soon)
                .Send();
        }
    }
}