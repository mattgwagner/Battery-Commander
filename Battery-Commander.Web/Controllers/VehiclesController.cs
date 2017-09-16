using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly Database db;

        public VehiclesController(Database db)
        {
            this.db = db;
        }

        // List of Vehicles - by unit, by status

        // Add Vehicle

        // Edit Vehicle

        // Delete Vehicle (?)
    }
}
