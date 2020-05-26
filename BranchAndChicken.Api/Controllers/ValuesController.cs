using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BranchAndChicken.Api.Controllers
{
    [Route("api/values")] // attribute -  adds metadata, updated to name of class for clarity's sake
    [ApiController]  // attribute that tells the app that it's only going to be returning JSON, instead of an HTTP string/URL
    public class ValuesController : ControllerBase // controller names always end in 'Controller' and will always inherit from ControllerBase 
    {
        // GET api/values
        [HttpGet]  // this attribute defines which HTTP verb need to access this method
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")] // this attribute adds a parameter to the method, so this needs to be the same as the parameter in the method below
        public ActionResult<string> Get(int id)
        {
            return id.ToString();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
