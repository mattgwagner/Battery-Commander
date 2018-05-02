using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static BatteryCommander.Web.Services.PDFService;

namespace BatteryCommander.Web.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> VendorRequest(String id)
        {
            // Unique record IDs look like reclJK6G3IFjFlXE1

            // Get PO data from AirTable

            // Fill in the DTO for the PDF Service

            // Return the PDF data

            // var order = await db.PurchaseOrders.FindAsync(id);

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

            var filename = $"FLNG_49D_{order.Unit?.Name}_{order.Vendor?.Name}_{order.Date:yyyyMMdd}.pdf";

            return File(PDFService.Generate_Vendor_Request_Form(order), "application/pdf", filename);
        }
    }
}