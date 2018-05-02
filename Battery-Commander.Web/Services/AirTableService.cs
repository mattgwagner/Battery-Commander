using AirtableApiClient;
using BatteryCommander.Web.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BatteryCommander.Web.Services.PDFService;

namespace BatteryCommander.Web.Services
{
    public class AirTableService
    {
        private readonly IOptions<AirTableSettings> settings;

        private const string PURCHASE_ORDER_TABLE = "Purchase Orders";

        private const string UNITS_TABLE = "Units";

        public AirTableService(IOptions<AirTableSettings> settings)
        {
            this.settings = settings;
        }

        private AirtableBase Client => new AirtableBase(settings.Value.AppKey, settings.Value.BaseId);

        public async Task<PurchaseOrder.PurchaseOrderUnit> GetUnit(string id)
        {
            var record = await GetById(id, UNITS_TABLE);

            return new PurchaseOrder.PurchaseOrderUnit
            {
                Name = (String)record.GetField("Name"),
                CommandOrTaskForce = (String)record.GetField("CommandOrTaskForce"),
                Phone = (String)record.GetField("Unit Phone #"),
                POC = new PurchaseOrder.PointOfContact
                {
                    Name = (String)record.GetField("Point of Contact Name"),
                    PhoneNumber = (String)record.GetField("Point of Contact Mobile #")
                }
            };
        }

        public async Task<PurchaseOrder> GetPurchaseOrder(string id)
        {
            var record = await GetById(id, PURCHASE_ORDER_TABLE);

            var category = PurchaseCategory.Meals;

            // TODO Can we auto-generate the justification?
            // Perhaps we need another form field for # of SMs and location but that might be a vestige of past exercises

            var justification = "";

            return new PurchaseOrder
            {
                Category = category,
                Date = (DateTime)record.GetField("Date"),
                Identifier = (String)record.GetField("Purchase Request #"),
                Justification = justification,
                Operation = (String)record.GetField("Operation"),
                Unit = await GetUnit((String)record.GetField("Unit")),
                Vendor = new PurchaseOrder.OrderVendor
                {
                    BusinessPhone = (String)record.GetField(""),
                    FedID = (String)record.GetField(""),
                    Name = (String)record.GetField(""),
                    PhysicalAddress = new PurchaseOrder.Address
                    {
                        Line1 = (String)record.GetField(""),
                        City = (String)record.GetField(""),
                        State = (String)record.GetField(""),
                        ZipCode = (String)record.GetField("")
                    },
                    RemitToAddress = new PurchaseOrder.Address
                    {
                        Line1 = (String)record.GetField(""),
                        City = (String)record.GetField(""),
                        State = (String)record.GetField(""),
                        ZipCode = (String)record.GetField("")
                    },
                    POC = new PurchaseOrder.PointOfContact
                    {
                        Name = (String)record.GetField(""),
                        PhoneNumber = (String)record.GetField(""),
                        Role = (String)record.GetField("")
                    }
                }
            };
        }

        public async Task<AirtableRecord> GetById(string id, string table = PURCHASE_ORDER_TABLE)
        {
            using (AirtableBase airtableBase = Client)
            {
                var response = await airtableBase.RetrieveRecord(table, id);

                CheckForErrors(response);

                return response.Record;
            }
        }

        public async Task<IEnumerable<AirtableRecord>> GetRecords(string table = PURCHASE_ORDER_TABLE)
        {
            string offset = null;

            var records = new List<AirtableRecord>();

            using (AirtableBase airtableBase = Client)
            {
                do
                {
                    var response = await airtableBase.ListRecords(table);

                    CheckForErrors(response);

                    records.AddRange(response.Records.ToList());
                    offset = response.Offset;
                }
                while (offset != null);
            }

            return records;
        }

        private void CheckForErrors(AirtableApiResponse response)
        {
            if (!response.Success)
            {
                if (response.AirtableApiError is AirtableApiException)
                {
                    throw response.AirtableApiError;
                }

                throw new Exception("Unknown error from AirTable API");
            }
        }
    }
}