namespace BatteryCommander.Common.Migrations
{
    using BatteryCommander.Common.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<BatteryCommander.Common.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BatteryCommander.Common.DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            context.Qualifications.AddOrUpdate(
                q => q.Name,

                // Field Artillery Qual Events

                new Qualification { Name = "FA Safety", Description = "BN Safety Test" },
                new Qualification { Name = "ASPT", Description = "Artillery Skills Proficiency Test" },
                new Qualification { Name = "GQT", Description = "Gunner's Qualification Test" },
                new Qualification { Name = "LDR VAL", Description = "Leader's Validation" },

                // Weapon Qual Events

                new Qualification { Name = "M-4 IWQ", Description = "M-4 Individual Weapons Qual" },
                new Qualification { Name = "M-240B", Description = "M-240B Crew Serve Weapons Qual" },
                new Qualification { Name = "M-2", Description = "M-2 Crew Serve Weapons Qual" },

                // Driver Quals

                // Otther individual warrior task quals

                new Qualification { Name = "CLS", Description = "Combat Life Saver" },
                new Qualification { Name = "DSCA", Description = "Defense Support of Civil authorities" }
                );

            context.Users.AddOrUpdate(
                u => u.UserName,
                new AppUser
                {
                    UserName = "mattgwagner@gmail.com",
                    Password = "AAFQO/7w4afWBCkdBYKVX+MmmFYneGIQv6W8cPYAU/S16yLKYkkR3zQbudWmqIvXag==",
                    EmailAddressConfirmed = true,
                    PhoneNumber = "8134136839",
                    PhoneNumberConfirmed = true
                });
        }
    }
}