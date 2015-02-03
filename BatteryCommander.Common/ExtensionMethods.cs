using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BatteryCommander.Common
{
    public static class ExtensionMethods
    {
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