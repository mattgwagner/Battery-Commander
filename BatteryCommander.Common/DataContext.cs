using BatteryCommander.Common.Migrations;
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

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder
                .Entity<Qualification>()
                .HasOptional(q => q.ParentTask)
                .WithMany(q => q.Tasks)
                .WillCascadeOnDelete(false);

            //builder
            //    .Entity<Qualification>()
            //    .HasMany(q => q.Tasks)
            //    .WithRequired(t => t.ParentTask)
            //    .WillCascadeOnDelete(true);

            // TODO
        }
    }
}