using BatteryCommander.Web.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace BatteryCommander.Tests
{
    public class RankTests
    {
        [Theory]
        [MemberData(nameof(ShortNames), MemberType = typeof(RankTests))]
        public void Parse_Rank_Short(String value, Rank expected)
        {
            Assert.Equal(expected, RankExtensions.Parse(value));
        }

        [Theory]
        [MemberData(nameof(EnumNames), MemberType = typeof(RankTests))]
        public void Parse_Rank_Enums(String value, Rank expected)
        {
            Assert.Equal(expected, RankExtensions.Parse(value));
        }

        [Theory]
        [MemberData(nameof(DisplayNames), MemberType = typeof(RankTests))]
        public void Parse_Rank_Display(String value, Rank expected)
        {
            Assert.Equal(expected, RankExtensions.Parse(value));
        }

        public static IEnumerable<object[]> DisplayNames
        {
            get
            {
                yield return new object[] { "Colonel", Rank.O6 };
            }
        }

        public static IEnumerable<object[]> EnumNames
        {
            get
            {
                yield return new object[] { "O2", Rank.O2 };
            }
        }

        public static IEnumerable<object[]> ShortNames
        {
            get
            {
                yield return new object[] { "PFC", Rank.E3 };
                yield return new object[] { "SSG", Rank.E6 };
                yield return new object[] { "pv2", Rank.E2 };
                yield return new object[] { "SpC", Rank.E4 };
            }
        }
    }
}