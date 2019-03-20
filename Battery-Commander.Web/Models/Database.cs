using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Web.Models
{
    public class Database : DbContext, IDataProtectionKeyContext
    {
        public virtual DbSet<Soldier> Soldiers { get; set; }

        public virtual DbSet<Evaluation> Evaluations { get; set; }

        public virtual DbSet<APFT> APFTs { get; set; }

        public virtual DbSet<ABCP> ABCPs { get; set; }

        public virtual DbSet<Unit> Units { get; set; }

        public virtual DbSet<Vehicle> Vehicles { get; set; }

        public virtual DbSet<Weapon> Weapons { get; set; }

        public virtual DbSet<Embed> Embeds { get; set; }

        public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Data Source=Data.db");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Unit>()
                .HasIndex(unit => unit.UIC)
                .IsUnique();

            builder
                .Entity<Soldier>()
                .HasIndex(soldier => new { soldier.FirstName, soldier.MiddleName, soldier.LastName })
                .IsUnique();

            builder
                .Entity<Soldier>()
                .HasIndex(soldier => soldier.DoDId)
                .IsUnique();

            builder
                .Entity<Vehicle>()
                .HasIndex(vehicle => vehicle.Registration)
                .IsUnique();

            builder
                .Entity<Vehicle>()
                .HasIndex(vehicle => vehicle.Serial)
                .IsUnique();

            builder.Entity<Vehicle>()
                .HasIndex(vehicle => new { vehicle.UnitId, vehicle.Bumper })
                .IsUnique();

            builder
                .Entity<Weapon>()
                .HasIndex(weapon => new { weapon.Serial, weapon.Type })
                .IsUnique();

            builder
                .Entity<Vehicle.Passenger>()
                .ToTable("Passengers")
                .HasKey(passenger => new { passenger.VehicleId, passenger.SoldierId });
        }

        public static void Init(Database db)
        {
            db.Database.Migrate();
        }
    }
}