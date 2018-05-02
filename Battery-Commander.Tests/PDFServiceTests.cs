using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using System;
using System.IO;
using Xunit;
using static BatteryCommander.Web.Services.PDFService;

namespace BatteryCommander.Tests
{
    public class PDFServiceTests
    {
        [Fact]
        public void Generate_FLNG_Vendor_Request_Form()
        {
            using (var file = new FileStream("Test_VendorRequest.pdf", FileMode.Create))
            {
                var order = new PurchaseOrder
                {
                    Unit = new PurchaseOrder.PurchaseOrderUnit
                    {
                        Name = "C BTRY",
                        POC = new PurchaseOrder.PointOfContact
                        {
                            Name = "Me",
                            PhoneNumber = "My #",
                            Role = "Boss"
                        },
                        CommandOrTaskForce = "53 IBCT",
                        Phone = "123456789"
                    },
                    Vendor = new PurchaseOrder.OrderVendor
                    {
                        Name = "Vendor Name",
                        BusinessPhone = "987654321",
                        FedID = "999999999",
                        PhysicalAddress = new PurchaseOrder.Address
                        {
                            City = "Lakeland",
                            Line1 = "321 Fake Street",
                            State = "TX",
                            ZipCode = "54321"
                        },
                        RemitToAddress = new PurchaseOrder.Address
                        {
                            City = "Tampa",
                            Line1 = "123 Happy Street",
                            State = "FL",
                            ZipCode = "12345"
                        },
                        POC = new PurchaseOrder.PointOfContact
                        {
                            Name = "Mr Mgr",
                            Role = "Grcery Manager"
                        }
                    }
                };

                var data = PDFService.Generate_Vendor_Request_Form(order);

                file.Write(data, 0, data.Length);
            }
        }

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
                    Description = "Rifle, 5.56mm M4",
                    From = "Arms Room",
                    Name = "Snuffy, Joe",
                    Grade = Rank.E2
                });

                file.Write(data, 0, data.Length);
            }
        }
    }
}