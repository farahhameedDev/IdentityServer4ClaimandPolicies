using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy  = "IsAdmin")]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Authorize(Policy = "GetAllValues")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Authorize(Policy = "GetValues")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            Console.WriteLine("Access granted to Post Value");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            Console.WriteLine("Access granted to Put Value");
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteValues")]
        public void Delete(int id)
        {
            Console.WriteLine("Deletion access granted on Values");
        }
    }
}