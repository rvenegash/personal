using AspNetCore1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore1.Controllers
{
    [Produces("application/json")]
    [Route("api/maker")]
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
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Maker/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // GET: api/Maker/{id}/models
        [HttpGet]
        [Route("{id:minlength(3)}/models")]
        public IEnumerable<Model> GetModels(string id)
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

            return null; // NotFound(); 
        }

        [HttpGet("StockItem/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetStockItemAsync(int id)
        {
            Logger?.LogDebug("'{0}' has been invoked", nameof(GetStockItemAsync));

            var response = new SingleResponse<StockItem>();

            try
            {
                // Get the stock item by id
                response.Model = await DbContext.GetStockItemsAsync(new StockItem(id));
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support.";

                Logger?.LogCritical("There was an error on '{0}' invocation: {1}", nameof(GetStockItemAsync), ex);
            }

            return response.ToHttpResponse();
        }
    }
}
