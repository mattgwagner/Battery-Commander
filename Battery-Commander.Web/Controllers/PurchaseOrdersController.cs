using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> VendorRequest(int id)
        {
            // Get PO data from AirTable

            // Fill in the DTO for the PDF Service

            // Return the PDF data

            // var order = await db.PurchaseOrders.FindAsync(id);

            var order = new PurchaseOrder { };

            var filename = $"FLNG_49D_{order.Unit?.Name}_{order.Vendor?.Name}_{order.Date:yyyyMMdd}.pdf";

            return File(PDFService.Generate_Vendor_Request_Form(order), "application/pdf", filename);
        }
    }
}