﻿using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BatteryCommander.Web.Jobs
{
    public class EvaluationStatusChangeJob : IJob
    {
        private static TimeSpan EditedTimespan => TimeSpan.FromMinutes(-5);

        private static readonly ILogger Log = Serilog.Log.ForContext<EvaluationStatusChangeJob>();

        private readonly Database db;

        private readonly IFluentEmailFactory emailSvc;

        public EvaluationStatusChangeJob(Database db, IFluentEmailFactory emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public void Execute()
        {
            var since = DateTime.UtcNow.Add(EditedTimespan);

            // Get all evaluations with an event since the configured timespan

            var evaluations =
                db
                .Evaluations
                .Include(evaluation => evaluation.Events)
                .Include(evaluation => evaluation.Ratee)
                .Include(evaluation => evaluation.Rater)
                .Include(evaluation => evaluation.SeniorRater)
                .Include(evaluation => evaluation.Reviewer)
                .AsEnumerable()
                .Where(evaluation => evaluation.LastUpdated > since);

            foreach (var evaluation in evaluations)
            {
                // If it's in a given window, fire off an email to the relevant people

                var recipients = Get_Recipients(evaluation).ToList();

                emailSvc
                    .Create()
                    .To(recipients)
                    .Subject($"Evaluation Updated | {evaluation.Ratee}")
                    .UsingTemplateFromEmbedded("BatteryCommander.Web.Views.Reports.EvaluationUpdated.cshtml", evaluation, Assembly.GetExecutingAssembly())
                    .SendWithErrorCheck();
            }
        }

        public IEnumerable<Address> Get_Recipients(Evaluation evaluation)
        {
            // TODO Remove after testing

            yield return new Address { EmailAddress = "Evaluations@RedLeg.app" };

            foreach (var email in evaluation.Rater.GetEmails()) yield return email;

            foreach (var email in evaluation.SeniorRater.GetEmails()) yield return email;

            foreach (var email in evaluation.Reviewer.GetEmails()) yield return email;

            // Include the unit 1SG on event notifications

            foreach (var soldier in SoldierService.Filter(db, new SoldierService.Query { Ranks = new[] { Rank.E8 }, Unit = evaluation.Ratee.UnitId }).GetAwaiter().GetResult())
            {
                foreach (var email in soldier.GetEmails()) yield return email;
            }
        }
    }
}