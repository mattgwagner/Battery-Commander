using BatteryCommander.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BatteryCommander.Tests
{
    public class FakeDb : Database
    {
        public FakeDb()
        {
            BatteryCommander.Web.Models.Database.Init(this);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Data Source=:memory:");
        }
    }
}