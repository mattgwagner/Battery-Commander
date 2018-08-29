using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models.Settings
{
    [ComplexType]
    public class UnitSettings
    {
        public Boolean SendGreen3 { get; set; }

        public Boolean SendPerstat { get; set; }
    }
}