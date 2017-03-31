using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Models
{
    public class Database : DbContext
    {
        private DbSet<Soldier> Soldiers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Data Source=Data.db");
        }
    }
}