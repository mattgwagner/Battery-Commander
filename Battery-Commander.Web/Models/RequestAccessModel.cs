using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public class RequestAccessModel
    {
        public String Name { get; set; }

        public String Email { get; set; }

        [Display(Name = "DOD ID")]
        public String DoDId { get; set; }

        public int Unit { get; set; }

        public IEnumerable<SelectListItem> Units { get; set; }
    }
}