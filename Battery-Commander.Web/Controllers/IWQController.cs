﻿using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class IWQController : Controller
    {
        private readonly Database db;

        public IWQController(Database db)
        {
            this.db = db;
        }

        // List status for all soldiers by - filter by unit

        // Bulk update qual status/date
    }
}
