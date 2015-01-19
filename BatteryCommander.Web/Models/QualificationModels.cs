using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BatteryCommander.Web.Models
{
    public class QualificationEditModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public String Name { get; set; }

        public String Description { get; set; }

        // TODO
    }
}