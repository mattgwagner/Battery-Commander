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

        private AirTableService Service
        {
            get
            {
                var options = Options.Create(new AirTableSettings { AppKey = AppKey, BaseId = BaseId });

                var service = new AirTableService(options);

                return service;
            }
        }

        [Fact]
        public async Task Try_Get_Records()
        {
            if (String.IsNullOrWhiteSpace(AppKey)) return;

            var records = await Service.GetRecords();

            Assert.NotEmpty(records);
        }

        [Fact]
        public async Task Get_Purchase_Order()
        {
            if (String.IsNullOrWhiteSpace(AppKey)) return;

            var order = await Service.GetPurchaseOrder(id: "reclJK6G3IFjFlXE1");

            // TODO
        }

        [Fact]
        public async Task Get_Unit()
        {
            if (String.IsNullOrWhiteSpace(AppKey)) return;

            var unit = await Service.GetUnit(id: "rece9eMRk3joCkNYG");

            // TODO
        }
    }
}