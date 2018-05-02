using AirtableApiClient;
using BatteryCommander.Web.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class AirTableService
    {
        private readonly IOptions<AirTableSettings> settings;

        private const string PURCHASE_ORDER_TABLE = "Purchase Orders";

        public AirTableService(IOptions<AirTableSettings> settings)
        {
            this.settings = settings;
        }

        private AirtableBase Client => new AirtableBase(settings.Value.AppKey, settings.Value.BaseId);

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