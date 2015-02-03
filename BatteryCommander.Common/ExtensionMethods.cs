using System;
using System.ComponentModel;
using System.Reflection;

namespace BatteryCommander.Common
{
    public static class ExtensionMethods
    {
        public static string DisplayName(this Enum val)
        {
            FieldInfo fi = val.GetType().GetField(val.ToString());

            DisplayNameAttribute[] attributes = (DisplayNameAttribute[])fi.GetCustomAttributes(typeof(DisplayNameAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].DisplayName;
            }

            return val.ToString();
        }
    }
}