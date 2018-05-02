using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BatteryCommander.Tests
{
    public class AirTableServiceTests
    {
        private const String AppKey = "";

        private const String BaseId = "";

        [Fact]
        public async Task Try_Get_Records()
        {
            if (String.IsNullOrWhiteSpace(AppKey)) return;

            // Arrange

            var options = Options.Create(new AirTableSettings { AppKey = AppKey, BaseId = BaseId });

            var service = new AirTableService(options);

            // Act

            var records = await service.GetRecords();

            // Assert

            Assert.NotEmpty(records);
        }

        [Fact]
        public async Task Get_Purchase_Order()
        {
            if (String.IsNullOrWhiteSpace(AppKey)) return;

            // Arrange

            var options = Options.Create(new AirTableSettings { AppKey = AppKey, BaseId = BaseId });

            var service = new AirTableService(options);

            // Act

            var order = await service.GetPurchaseOrder(id: "reclJK6G3IFjFlXE1");

            // Assert

            // TODO
        }
    }
}