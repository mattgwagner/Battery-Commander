using BatteryCommander.Web.Models;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;

namespace BatteryCommander.Web.Jobs
{
    internal static class JobHandler
    {
        public static void UseJobScheduler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(typeof(JobHandler));

            JobManager.JobStart += (job) => logger.LogInformation("{job} started", job.Name);

            JobManager.JobEnd += (job) => logger.LogInformation("{job} completed in {time}", job.Name, job.Duration);

            JobManager.JobException += (context) => logger.LogError(context.Exception, "{job} failed", context.Name);

            JobManager.UseUtcTime();

            JobManager.JobFactory = new JobFactory(app.ApplicationServices);

            var registry = new Registry();

            registry.Schedule<SqliteBackupJob>().ToRunEvery(1).Days().AtEst(hours: 9);

            registry.Schedule<EvaluationDueReminderJob>().ToRunEvery(0).Weeks().On(DayOfWeek.Tuesday).At(hours: 13, minutes: 0);

            registry.Schedule<PERSTATReportJob>().ToRunEvery(1).Days().AtEst(hours: 6, minutes: 30);

            registry.Schedule<SensitiveItemsReport>().WithName("Green3_AM").ToRunEvery(1).Days().AtEst(hours: 7, minutes: 30);
            registry.Schedule<SensitiveItemsReport>().WithName("Green3_PM").ToRunEvery(1).Days().AtEst(hours: 22, minutes: 30);

            JobManager.Initialize(registry);
        }

        private class JobFactory : IJobFactory
        {
            private readonly IServiceProvider serviceProvider;

            public JobFactory(IServiceProvider serviceProvider)
            {
                this.serviceProvider = serviceProvider;
            }

            public IJob GetJobInstance<T>() where T : IJob
            {
                return serviceProvider.GetService(typeof(T)) as IJob;
            }
        }
    }
}