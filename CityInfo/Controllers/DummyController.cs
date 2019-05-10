using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CityInfo.Controllers
{
    public class DummyController : Controller
    {
        private Entities.CityInfoContext _ctx;

        public DummyController(Entities.CityInfoContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        [Route("api/testdatabase")]
        public IActionResult TestDatabase()
        {
            return Ok();
        }

    }
}
