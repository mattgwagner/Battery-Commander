using BatteryCommander.Web.Models.Data;
using System;
using Xunit;

namespace BatteryCommander.Tests
{
    public class ACFTTests
    {
        [Theory]
        [InlineData(15, 54, 84)]
        [InlineData(13, 29, 100)]
        [InlineData(23, 01, 0)]
        [InlineData(19, 05, 64)]
        [InlineData(17, 42, 72)]
        public void Calculate_Run_Score(int minutes, int seconds, int expected)
        {
            Assert.Equal(expected, ACFTScoreTables.TwoMileRun(new TimeSpan(0, minutes, seconds)));
        }

        [Theory]
        [InlineData(61, 100)]
        [InlineData(30, 70)]
        public void Calculate_Pushups(int reps, int expected)
        {
            Assert.Equal(expected, ACFTScoreTables.HandReleasePushUps(reps));
        }
    }
}
