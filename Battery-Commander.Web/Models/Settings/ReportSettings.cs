﻿using BatteryCommander.Web.Models.Reports;
using BatteryCommander.Web.Models.Settings;
using FluentEmail.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static BatteryCommander.Web.Models.Reports.Report;

namespace BatteryCommander.Web.Models.Settings
{
    public class ReportSettings
    {
        public ReportType Type { get; set; }

        public Boolean Enabled { get; set; }

        public IList<Address> Recipients { get; set; } = new List<Address>();

        public Address From { get; set; } = new Address { };
    }
}

namespace BatteryCommander.Web.Models
{
    public partial class Unit
    {
        public String ReportSettingsJson { get; set; } = "[]";

        [NotMapped]
        public ICollection<ReportSettings> ReportSettings
        {
            get { return JsonConvert.DeserializeObject<List<ReportSettings>>(String.IsNullOrWhiteSpace(ReportSettingsJson) ? "[]" : ReportSettingsJson); }
            set { ReportSettingsJson = JsonConvert.SerializeObject(value); }
        }

        [NotMapped]
        public virtual Red1_Perstat PERSTAT
        {
            get { return new Red1_Perstat(this); }
        }

        [NotMapped]
        public virtual Green3_SensitiveItems SensitiveItems
        {
            get { return new Green3_SensitiveItems(this); }
        }
    }
}