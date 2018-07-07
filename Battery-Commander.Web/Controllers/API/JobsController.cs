using BatteryCommander.Web.Models;
using FluentScheduler;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BatteryCommander.Web.Controllers.API
{
    public class JobsController : ApiController
    {
        public JobsController(Database db) : base(db)
        {
            // Nothing to do here
        }

        [HttpGet]
        public IEnumerable<Schedule> List()
        {
            return JobManager.AllSchedules;
        }

        [HttpPost("{name}")]
        public IActionResult Post(String name)
        {
            JobManager.GetSchedule(name)?.Execute();

            return Ok();
        }
    }
}