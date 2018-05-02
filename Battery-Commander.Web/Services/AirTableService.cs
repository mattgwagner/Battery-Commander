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
                Name = (String)record.Fields["Name"],
                CommandOrTaskForce = (String)record.Fields["CommandOrTaskForce"],
                Phone = (String)record.Fields["Unit Phone #"],
                POC = new PurchaseOrder.PointOfContact
                {
                    Name = (String)record.Fields["Point of Contact Name"],
                    PhoneNumber = (String)record.Fields["Point of Contact Mobile #"]
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
                Date = (DateTime)record.Fields["Date"],
                Identifier = (String)record.Fields["Purchase Request #"],
                Justification = justification,
                Operation = (String)record.Fields["Operation"],
                Unit = await GetUnit((String)record.Fields["Unit"]),
                Vendor = new PurchaseOrder.OrderVendor
                {
                    BusinessPhone = (String)record.Fields[""],
                    FedID = (String)record.Fields[""],
                    Name = (String)record.Fields[""],
                    PhysicalAddress = new PurchaseOrder.Address
                    {
                        Line1 = (String)record.Fields[""],
                        City = (String)record.Fields[""],
                        State = (String)record.Fields[""],
                        ZipCode = (String)record.Fields[""]
                    },
                    RemitToAddress = new PurchaseOrder.Address
                    {
                        Line1 = (String)record.Fields[""],
                        City = (String)record.Fields[""],
                        State = (String)record.Fields[""],
                        ZipCode = (String)record.Fields[""]
                    },
                    POC = new PurchaseOrder.PointOfContact
                    {
                        Name = (String)record.Fields[""],
                        PhoneNumber = (String)record.Fields[""],
                        Role = (String)record.Fields[""]
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