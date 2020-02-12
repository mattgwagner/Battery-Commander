using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentScheduler;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BatteryCommander.Web.Jobs
{
    public class EvaluationDueReminderJob : IJob
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<EvaluationDueReminderJob>();

        private readonly Database db;

        private readonly IFluentEmailFactory emailSvc;

        public class Model
        {
            public IEnumerable<Evaluation> Evaluations { get; set; }

            public int Unit { get; set; }
        }

        public EvaluationDueReminderJob(Database db, IFluentEmailFactory emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public void Execute()
        {
            var soon = DateTime.Today.AddDays(15);

            Log.Information("Building Evaluations Due email for evals due before {soon}", soon);

            foreach (var unit in UnitService.List(db).GetAwaiter().GetResult())
            {
                var evaluations_due_soon =
                    EvaluationService.Filter(db, new EvaluationService.Query { Complete = false, Unit = unit.Id })
                    .Where(evaluation => evaluation.ThruDate < soon)
                    .OrderBy(evaluation => evaluation.ThruDate)
                    .ToList();

                if (evaluations_due_soon.Any())
                {
                    var recipients = SoldierService.Filter(db, new SoldierService.Query
                    {
                        Unit = unit.Id,
                        Ranks = RankExtensions.All().Where(_ => _.GetsEvaluation())
                    })
                    .GetAwaiter()
                    .GetResult()
                    .Where(soldier => soldier.CanLogin)
                    .Select(soldier => soldier.GetEmails())
                    .SelectMany(email => email)
                    .ToList();

                    if (recipients.Any())
                    {
                        emailSvc
                        .Create()
                        .BCC(recipients)
                        .To(emailAddress: "Evaluations@RedLeg.app")
                        .Subject($"{unit.UIC} | Past Due and Upcoming Evaluations")
                        .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Jobs/EvaluationsDue.html", new Model
                        {
                            Evaluations = evaluations_due_soon,
                            Unit = unit.Id
                        })
                        .SendWithErrorCheck()
                        .Wait();
                    }                    
                }
            }
        }
    }
}