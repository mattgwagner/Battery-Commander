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

        [Fact]
        public void Generate_Equipment_Receipt()
        {
            using (var file = new FileStream("TestReceipt.pdf", FileMode.Create))
            {
                var data = PDFService.Generate_DA3749(new PDFService.EquipmentReceipt
                {
                    Unit = "C BAT 2 116th FA",
                    ReceiptNumber = "M4-01",
                    StockNumber = "1005-01-231-0973",
                    SerialNumber = "W630890",
                    ItemDescription = "Rifle, 5.56mm M4",
                    Source = "Arms Room",
                    Soldier = "Snuffy, Joe",
                    SoldierIdentifer = "XXX-XX-2890",
                    Grade = Rank.E2
                });

                file.Write(data, 0, data.Length);
            }
        }
    }
}