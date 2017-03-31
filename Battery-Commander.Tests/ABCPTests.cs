using BatteryCommander.Web.Models;
using System;
using Xunit;
using static BatteryCommander.Web.Models.ABCP;

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

        [Fact]
        public void Male_Tape_1()
        {
            var score = new ABCP
            {
                Soldier = new Soldier
                {
                    Gender = Gender.Male,
                    DateOfBirth = DateTime.Today.AddYears(-29)
                },
                Height = 69,
                Weight = 187, // Over screening weight
                Measurements = new[]
                {
                    new Measurement { Neck = 16, Waist = 49 }
                }
            };

            Assert.True(score.RequiresTape);

            Assert.Equal(39, score.BodyFatPercentage);
        }

        [Fact]
        public void Female_Tape_1()
        {
            var score = new ABCP
            {
                Soldier = new Soldier
                {
                    Gender = Gender.Female
                },
                Height = 64,
                Weight = 170, // Over screening weight
                Measurements = new[]
                {
                    new Measurement { Neck = 15, Waist = 42, Hips = 44 }
                }
            };

            Assert.Equal(47, score.BodyFatPercentage);
        }
    }
}