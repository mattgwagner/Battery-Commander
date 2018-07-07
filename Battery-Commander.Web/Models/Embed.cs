using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public class Embed
    {
        // FIXME: Need to figure out how to deconflict multiple embeds with the same route, i.e. multiple units allocated /Calendar or /SUTA.

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        [StringLength(20)]
        public String Name { get; set; }

        [DataType(DataType.Url)]
        public String Source { get; set; }

        [StringLength(30)]
        public String Route { get; set; }

        // public Boolean IsNavItem { get; set; }

        // Require Auth

        // Size

        // Height

        // Css Class
    }
}