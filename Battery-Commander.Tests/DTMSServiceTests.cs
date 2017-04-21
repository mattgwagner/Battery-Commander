using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Http;
using Moq;
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
            var db = new Mock<Database>();

            using (var file = new FileStream("Scrubbed.xlsx", FileMode.Open))
            {
                var fileinfo = new Mock<IFormFile>();

                fileinfo.Setup(_ => _.OpenReadStream()).Returns(file);

                var soldiers = await DTMSService.ImportSoldiers(db.Object, fileinfo.Object);

                Assert.Equal(9, soldiers.Count());
            }
        }
    }
}