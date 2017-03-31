using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Models
{
    public class Database : DbContext
    {
        public DbSet<Soldier> Soldiers { get; set; }

        public DbSet<Evaluation> Evaluations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Data Source=Data.db");
        }
    }
}