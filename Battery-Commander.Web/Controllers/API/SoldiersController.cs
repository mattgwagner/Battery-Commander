using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    public class SoldiersController : ApiController
    {
        public SoldiersController(Database db) : base(db)
        {
            // Nothing to do here
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