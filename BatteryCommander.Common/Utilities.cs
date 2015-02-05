using System;
using System.Linq;
using System.Reflection;

namespace BatteryCommander.Common
{
    public class Utilities
    {
        private static Lazy<String> version = new Lazy<String>(() => Assembly.GetExecutingAssembly().GetCustomAttributes(false).OfType<AssemblyInformationalVersionAttribute>().Single().InformationalVersion);

        /// <summary>
        /// Returns the AssemblyFileVersion attribute of the library, or what version of the code is running
        /// </summary>
        public static String Version { get { return version.Value; } }
    }
}