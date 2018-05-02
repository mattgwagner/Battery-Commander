using AirtableApiClient;
using BatteryCommander.Web.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class AirTableService
    {
        private readonly IOptions<AirTableSettings> settings;

        public AirTableService(IOptions<AirTableSettings> settings)
        {
            this.settings = settings;
        }

        public async Task<IEnumerable<AirtableRecord>> GetRecords()
        {
            string offset = null;
            string errorMessage = null;
            var records = new List<AirtableRecord>();

            using (AirtableBase airtableBase = new AirtableBase(settings.Value.AppKey, settings.Value.BaseId))
            {
                //
                // Use 'offset' and 'pageSize' to specify the records that you want
                // to retrieve.
                // Only use a 'do while' loop if you want to get multiple pages
                // of records.
                //

                do
                {
                    //Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(
                    //       YOUR_TABLE_NAME,
                    //       offset,
                    //       fieldsArray,
                    //       filterByFormula,
                    //       maxRecords,
                    //       pageSize,
                    //       sort,
                    //       view);

                    var response = await airtableBase.ListRecords(tableName: "Purchase Orders");

                    if (response.Success)
                    {
                        records.AddRange(response.Records.ToList());
                        offset = response.Offset;
                    }
                    else if (response.AirtableApiError is AirtableApiException)
                    {
                        errorMessage = response.AirtableApiError.ErrorMessage;
                        break;
                    }
                    else
                    {
                        errorMessage = "Unknown error";
                        break;
                    }
                } while (offset != null);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                // Error reporting
            }
            else
            {
                // Do something with the retrieved 'records' and the 'offset'
                // for the next page of the record list.
            }

            return records;
        }
    }
}