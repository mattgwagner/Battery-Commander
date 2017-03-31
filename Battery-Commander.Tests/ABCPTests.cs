using BatteryCommander.Web.Models;
using System;
using Xunit;

namespace BatteryCommander.Tests
{
    public class ABCPTests
    {
        [Fact]
        public void Score_1()
        {
            var score = new ABCP
            {
                Soldier = new Soldier
                {
                    Gender = Gender.Male,
                    DateOfBirth = DateTime.Today.AddYears(-29)
                },
                Height = 73,
                Weight = 170
            };

            Assert.Equal(205, score.Screening_Weight);
            Assert.False(score.RequiresTape);
        }
    }
}