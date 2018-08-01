using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentScheduler;
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
                EvaluationService.Filter(db, new EvaluationService.Query { Complete = false })
                .Where(evaluation => evaluation.ThruDate < soon)
                .OrderBy(evaluation => evaluation.ThruDate)
                .ToList();

            var recipients = SoldierService.Filter(db, new SoldierService.Query
            {
                Ranks = new[] { Rank.E6, Rank.E7, Rank.E8, Rank.O1, Rank.O2, Rank.O3 }
            })
            .GetAwaiter()
            .GetResult()
            .Select(soldier => soldier.GetEmails())
            .SelectMany(email => email)
            .ToList();

            emailSvc
                .To(recipients)
                .Subject("Past Due and Upcoming Evaluations")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Reports/EvaluationsDue.cshtml", evaluations_due_soon)
                .SendWithErrorCheck();
        }
    }
}