using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using System;
using System.IO;
using Xunit;

namespace BatteryCommander.Tests
{
    public class PDFServiceTests
    {
        [Fact]
        public void Generate_Counseling()
        {
            using (var file = new FileStream("TestCounseling.pdf", FileMode.Create))
            {
                var data = PDFService.Generate_DA4856(new PDFService.Counseling
                {
                    Counselor = "LT Wagner, Matthew",
                    Date = DateTime.Today,
                    KeyPointsOfDiscussion = "Really, really important things go here.",
                    LeadersResponsibilities = "Lead Others.",
                    Name = "Snuffy, Joe",
                    Rank = Rank.E2,
                    Purpose = "To tell you important things that you should know.",
                    Organization = "A/2/116 FA",
                    PlanOfAction = "Go forth and conquer."
                });

                file.Write(data, 0, data.Length);
            }
        }
    }
}