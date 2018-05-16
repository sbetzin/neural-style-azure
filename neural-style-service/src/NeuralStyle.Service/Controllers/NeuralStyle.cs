using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NeuralStyle.Service.Controllers
{
    [Route("api/[controller]")]
    public class NeuralStyle : Controller
    {
        // GET api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        [HttpGet]
        public string Get([FromQuery] string source, [FromQuery] string style)
        {
            // ReSharper disable once InvokeAsExtensionMethod
            return Shell.Bash(source);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
    }
}
