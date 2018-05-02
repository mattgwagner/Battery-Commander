using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private readonly AirTableService airtable;

        public PurchaseOrdersController(AirTableService airtable)
        {
            this.airtable = airtable;
        }

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

            var order = await airtable.GetPurchaseOrder(id);

            var filename = $"FLNG_49D_{order.Unit?.Name}_{order.Vendor?.Name}_{order.Date:yyyyMMdd}.pdf";

            return File(PDFService.Generate_Vendor_Request_Form(order), "application/pdf", filename);
        }
    }
}