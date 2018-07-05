using FluentScheduler;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace BatteryCommander.Web.Models
{
    public static class ExtensionMethods
    {
        public static TimeZoneInfo EASTERN_TIME => TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public static IDayRestrictableUnit AtEst(this DayUnit schedule, int hours, int minutes = 0)
        {
            var now = DateTime.UtcNow;

            var time_in_est = new DateTime(now.Year, now.Month, now.Day, hours, minutes, second: 0);

            var hours_in_utc = TimeZoneInfo.ConvertTimeToUtc(time_in_est, EASTERN_TIME);

            return schedule.At(hours_in_utc.Hour, minutes);
        }

        public static String ToDateTimeGroup(this DateTime timestamp)
        {
            return timestamp.ToString("ddHH00MMMMyyyy");
        }

        public static DateTime ToEst(this DateTime timeInUtc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(timeInUtc, EASTERN_TIME);
        }

        public static string ToTitleCase(this string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }

        public static string ToTitleCase(this TextInfo textInfo, string str)
        {
            if (String.IsNullOrWhiteSpace(str)) return string.Empty;

            var tokens = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token.Substring(0, 1).ToUpper() + token.Substring(1);
            }

            return string.Join(" ", tokens);
        }

        public static string DisplayName(this Enum val)
        {
            return GetDisplayValue(val, a => a.Name);
        }

        public static string ShortName(this Enum val)
        {
            return GetDisplayValue(val, a => a.ShortName);
        }

        private static string GetDisplayValue(Enum val, Func<DisplayAttribute, String> selector)
        {
            FieldInfo fi = val.GetType().GetField(val.ToString());

            DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return selector.Invoke(attributes[0]);
            }

            return val.ToString();
        }
    }
}