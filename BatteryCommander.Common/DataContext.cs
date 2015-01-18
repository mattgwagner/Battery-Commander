using BatteryCommander.Common.Migrations;
using BatteryCommander.Common.Models;
using System.Data.Entity;

namespace BatteryCommander.Common
{
    public class DataContext : DbContext
    {
        public virtual IDbSet<AppUser> Users { get; set; }

        public virtual IDbSet<Soldier> Soldiers { get; set; }

        public virtual IDbSet<Group> Groups { get; set; }

        public virtual IDbSet<WorkItem> WorkItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // TODO
        }

        static DataContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
        }
    }
}