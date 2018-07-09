﻿using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentScheduler;
using System.Collections.Generic;
using System.IO;

namespace BatteryCommander.Web.Jobs
{
    public class PERSTATReportJob : IJob
    {
        private static IList<Address> Recipients => new List<Address>(new Address[]
        {
            FROM
            // new Address { Name = "2-116 FA BN TOC", EmailAddress = "ng.fl.flarng.list.ngfl-2-116-fa-bn-toc@mail.mil" }
        });

        internal static Address FROM => new Address { Name = "C-2-116 FA", EmailAddress = "C-2-116FA@redleg.app" };

        private readonly Database db;

        private readonly IFluentEmail emailSvc;

        public PERSTATReportJob(Database db, IFluentEmail emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public virtual void Execute()
        {
            foreach (var unit in UnitService.List(db).GetAwaiter().GetResult())
            {
                // HACK - Configure the recipients and units that this is going to be wired up for

                emailSvc
                    .To(Recipients)
                    .SetFrom(FROM.EmailAddress, FROM.Name)
                    .Subject($"{unit.Name} | RED 1 PERSTAT")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Reports/Red1_Perstat.cshtml", unit)
                    .Send();
            }
        }
    }
}