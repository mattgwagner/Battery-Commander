using BatteryCommander.Web.Models.Data;
using System;
using Xunit;

namespace BatteryCommander.Tests
{
    public class ACFTTests
    {
        [Theory]
        [InlineData(15, 54, 84)]
        public void Calculate_Run_Score(int minutes, int seconds, int expected)
        {
            Assert.Equal(expected, ACFTScoreTables.TwoMileRun(new TimeSpan(0, minutes, seconds)));
        }
    }
}
