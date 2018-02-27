using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    [Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SoldiersController : Controller
    {
        private readonly Database db;

        public SoldiersController(Database db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<SoldierDTO>> Get(SoldierSearchService.Query query)
        {
            // GET: api/soldiers

            return (await SoldierSearchService.Filter(db, query))
                .Select(s => new SoldierDTO
                {
                    Id = s.Id,
                    LastName = s.LastName,
                    FirstName = s.FirstName,
                    Rank = s.Rank
                })
                .ToList();
        }

        //// GET api/soldiers/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/soldiers
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //    // TODO
        //}

        //// PUT api/soldiers/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //    // TODO
        //}

        public class SoldierDTO
        {
            public int Id { get; set; }

            public String FirstName { get; set; }

            public String LastName { get; set; }

            public Rank Rank { get; set; }
        }
    }
}