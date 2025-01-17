using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestWebApi.Models;

namespace TestWebApi.Controllers
{
    public class TestController : ApiController
    {
        // GET: api/Test
        public IEnumerable<Text> Get()
        {
            return new Text[] { new Text() { Id = 1, Nombre = "Nombre 1"  }, new Text() { Id = 2, Nombre = "Nombre 2" } };
        }

        // GET: api/Test/5
        public Text Get(int id)
        {
            return new Text() { Id = id, Nombre = "Nombre " + id.ToString() };
        }

        // POST: api/Test
        public void Post([FromBody]Text value)
        {
        }

        // PUT: api/Test/5
        public void Put(int id, [FromBody]Text value)
        {
        }

        // DELETE: api/Test/5
        public void Delete(int id)
        {
        }
    }
}
