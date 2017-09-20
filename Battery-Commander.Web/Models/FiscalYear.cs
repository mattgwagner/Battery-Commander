using System;
using System.Collections.Generic;

namespace BatteryCommander.Web.Models
{
    public enum FiscalYear : byte
    {
        // Oct 1 2016 - Sept 30 2017

        FY2017,

        // Oct 1 2017 - Sept 30 2018

        FY2018,

        // Oct 1 2018 - Sept 30 2019

        FY2019,

        // Oct 1 2019 - Sept 30 2020

        FY2020
    }

    public static class FiscalYearExtensions
    {
        public static IEnumerable<FiscalYear> Values()
        {
            foreach (FiscalYear year in Enum.GetValues(typeof(FiscalYear)))
            {
                yield return year;
            }
        }

        public static FiscalYear Next(this FiscalYear fy)
        {
            return
                fy
                .End()
                .AddDays(1)
                .ToFiscalYear();
        }

        public static DateTime Start(this FiscalYear fy)
        {
            switch (fy)
            {
                case FiscalYear.FY2017:
                    return new DateTime(2016, 10, 1);

                case FiscalYear.FY2018:
                    return new DateTime(2017, 10, 1);

                case FiscalYear.FY2019:
                    return new DateTime(2018, 10, 1);

                case FiscalYear.FY2020:
                    return new DateTime(2019, 10, 1);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DateTime End(this FiscalYear fy)
        {
            switch (fy)
            {
                case FiscalYear.FY2017:
                    return new DateTime(2017, 9, 30);

                case FiscalYear.FY2018:
                    return new DateTime(2018, 9, 30);

                case FiscalYear.FY2019:
                    return new DateTime(2019, 9, 30);

                case FiscalYear.FY2020:
                    return new DateTime(2020, 9, 30);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static FiscalYear ToFiscalYear(this DateTime date)
        {
            foreach (var year in Values())
            {
                if (year.Start() <= date && date <= year.End())
                {
                    return year;
                }
            }

            throw new ArgumentOutOfRangeException($"{date} is not included in calculated FY timelines");
        }
    }
}