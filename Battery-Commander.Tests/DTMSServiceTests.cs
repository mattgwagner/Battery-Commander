using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BatteryCommander.Tests
{
    public class DTMSServiceTests
    {
        [Fact]
        public async Task Parse_Import()
        {
            var db = new Database { };

            Database.Init(db);

            using (var file = new FileStream("Scrubbed.xlsx", FileMode.Open))
            {
                var soldiers = await DTMSService.ImportSoldiers(db, file);

                Assert.Equal(9, soldiers.Count());
            }
        }
    }
}