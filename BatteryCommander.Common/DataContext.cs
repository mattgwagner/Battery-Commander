﻿using BatteryCommander.Common.Migrations;
using BatteryCommander.Common.Models;
using System.Data.Entity;

namespace BatteryCommander.Common
{
    public class DataContext : DbContext
    {
        static DataContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
        }

        public virtual IDbSet<AppUser> Users { get; set; }

        public virtual IDbSet<Soldier> Soldiers { get; set; }

        public virtual IDbSet<Qualification> Qualifications { get; set; }

        public virtual IDbSet<SoldierQualification> SoldierQualifications { get; set; }

        // public virtual IDbSet<WorkItem> WorkItems { get; set; }

        // public virtual IDbSet<Alert> Alerts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // TODO
        }
    }
}