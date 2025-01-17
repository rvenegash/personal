using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakerController : ControllerBase
    {
        // GET: api/Maker
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Maker/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Maker
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Maker/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("{id:minlength(3)}/models")]
        public ActionResult<Model> GetModels(string id)
        {
            var c = Request.Cookies.Where(m => m.Key == "test").FirstOrDefault();

            if (c.Key == null)
            {

            }
            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            //resp.Content = new StringContent( "");

            //var nv = new NameValueCollection();
            //nv["sid"] = "12345";
            //nv["token"] = "abcdef";
            //nv["theme"] = "dark blue";
            //var cookie = new CookieHeaderValue("session", nv);

            //resp.Headers..AddCookies(new CookieHeaderValue[] { cookie });

            return NotFound();
        }
    }
}
