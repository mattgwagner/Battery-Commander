using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Models
{
    public class Database : DbContext
    {
        public virtual DbSet<Soldier> Soldiers { get; set; }

        public virtual DbSet<Evaluation> Evaluations { get; set; }

        public virtual DbSet<APFT> APFTs { get; set; }

        public virtual DbSet<ABCP> ABCPs { get; set; }

        public virtual DbSet<Unit> Units { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Data Source=Data.db");
        }

        public static void Init(Database db)
        {
            db.Database.Migrate();
        }
    }
}