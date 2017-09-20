using BatteryCommander.Web.Models;
using System;
using System.Collections.Generic;
using Xunit;
using BatteryCommander.Web.Services;

namespace BatteryCommander.Tests
{
    public class FiscalYearTests
    {
        [Fact]

        public void Check_FY_2017_Start()
        {
            Assert.Equal(new DateTime(2016, 10, 1), FiscalYear.FY2017.Start());
        }

        [Fact]
        public void Check_FY_2017_Next()
        {
            Assert.Equal(FiscalYear.FY2018, FiscalYear.FY2017.Next());
        }

        [Fact]
        public void Check_FY_2017_End()
        {
            Assert.Equal(new DateTime(2017, 9, 30), FiscalYear.FY2017.End());
        }
    }
}