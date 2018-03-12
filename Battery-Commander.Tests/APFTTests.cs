using BatteryCommander.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace BatteryCommander.Tests
{
    public class APFTTests
    {
        [Fact]
        public void Walker_Score_1()
        {
            // 25 y/o Male Walker has 20.5 minutes to pass

            var score = new APFT
            {
                Soldier = new Soldier
                {
                    Gender = Gender.Male,
                    DateOfBirth = DateTime.Today.AddYears(-25)
                },
                PushUps = 100,
                SitUps = 100,
                AerobicEvent = Event.Walk,
                Run = TimeSpan.FromMinutes(16)
            };

            Assert.Equal(0, score.RunScore);
            Assert.True(score.IsAlternateAerobicEvent);
            Assert.True(score.IsPassing);
        }

        [Fact]
        public void Walker_Score_2()
        {
            // 25 y/o Male Walker has 34.5 minutes to pass

            var score = new APFT
            {
                Soldier = new Soldier
                {
                    Gender = Gender.Male,
                    DateOfBirth = DateTime.Today.AddYears(-25)
                },
                AerobicEvent = Event.Walk,
                Run = TimeSpan.FromMinutes(35)
            };

            Assert.Equal(0, score.RunScore);
            Assert.True(score.IsAlternateAerobicEvent);
            Assert.False(score.IsPassing);
        }

        [Fact]
        public void Walker_Score_3()
        {
            // 29 y/o Fe,ale Walker has 38 minutes to pass

            var score = new APFT
            {
                Soldier = new Soldier
                {
                    Gender = Gender.Male,
                    DateOfBirth = DateTime.Today.AddYears(-29)
                },
                PushUps = 100,
                SitUps = 100,
                AerobicEvent = Event.Walk,
                Run = TimeSpan.FromMinutes(30.25)
            };

            Assert.Equal(0, score.RunScore);
            Assert.True(score.IsAlternateAerobicEvent);
            Assert.True(score.IsPassing);
        }

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
                Run = new TimeSpan(0, 14, 45)
            };

            Assert.Equal(66, score.PushUpScore);
            Assert.Equal(86, score.SitUpScore);
            Assert.Equal(84, score.RunScore);
            Assert.Equal(236, score.TotalScore);
            Assert.True(score.IsPassing);
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
                Run = run
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

        [Fact]
        public void Generate_Counseling()
        {
            using (var file = new FileStream("TestCounseling_APFT.pdf", FileMode.Create))
            {
                var apft = new APFT
                {
                    Soldier = new Soldier
                    {
                        Rank = Rank.O2,
                        LastName = "Wagner",
                        FirstName = "Matthew",
                        DateOfBirth = new DateTime(1987, 5, 5),
                        Unit = new Unit { Name = "A/2/116 FA" }
                    },
                    PushUps = 45,
                    SitUps = 60,
                    Run = new TimeSpan(0, 14, 0)
                };

                var data = apft.GenerateCounseling();

                file.Write(data, 0, data.Length);
            }
        }
    }
}