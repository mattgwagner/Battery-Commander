using BatteryCommander.Web.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Battery_Commander.Tests
{
    public class APFTTests
    {
        [Fact]
        public void Score_1()
        {
            var score = new APFT
            {
                Soldier = new Soldier
                {
                    Gender = Gender.Male,
                    DateOfBirth = DateTime.Today.AddYears(-29)
                },
                PushUps = 45,
                SitUps = 69,
                RunSeconds = (int)new TimeSpan(0, 14, 45).TotalSeconds
            };

            Assert.Equal(66, score.PushUpScore);
            Assert.Equal(86, score.SitUpScore);
            Assert.Equal(84, score.RunScore);
            Assert.Equal(236, score.TotalScore);
            Assert.Equal(true, score.IsPassing);
        }

        [Theory]
        [MemberData(nameof(TestCases), MemberType = typeof(APFTTests))]
        public void Score_Matrix(Gender gender, int age, int pushups, int situps, TimeSpan run, int expected_total)
        {
            var score = new APFT
            {
                Soldier = new Soldier
                {
                    Gender = gender,
                    DateOfBirth = DateTime.Today.AddYears(-age)
                },
                PushUps = pushups,
                SitUps = situps,
                RunSeconds = (int)run.TotalSeconds
            };

            Assert.Equal(expected_total, score.TotalScore);
        }

        public static IEnumerable<object[]> TestCases
        {
            get
            {
                yield return new object[] { Gender.Male, 29, 45, 69, new TimeSpan(0, 14, 45), 236 };
                yield return new object[] { Gender.Male, 19, 51, 66, new TimeSpan(0, 13, 19), 247 };
                yield return new object[] { Gender.Female, 19, 51, 66, new TimeSpan(0, 13, 19), 281 };
            }
        }
    }
}