using com.dtvla.newk2view.clases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace com.dtvla.newk2view.api.view360.Controllers
{
    [Route("api/GetCustomer360View")]
    [ApiController]
    public class View360Controller : ControllerBase
    {
        private readonly ILogger<View360Controller> _logger;

        private MongoClient? dbClient;
        private IMongoDatabase database;
        private IMongoCollection<Customer> colCustomer;
        private IMongoCollection<CustomerProductSingle> colProducts;

        public View360Controller(ILogger<View360Controller> logger)
        {
            _logger = logger;

            dbClient = new MongoClient("mongodb://localhost:27017");
            database = dbClient.GetDatabase("dtv");
            colCustomer = database.GetCollection<Customer>("customer");
            colProducts = database.GetCollection<CustomerProductSingle>("CustomerProductListSingle");

        }

        //api/GetCustomer360View
        [HttpGet]
        public ActionResult<Models.GetCustomer360ViewResult> Get(string token, string i_customer, string i_requestId, string i_systemId, string i_country, string i_sync_on, string format)
        {
            //validar parametros
            if (string.IsNullOrEmpty(i_country) || string.IsNullOrEmpty(i_country))
            {
                return BadRequest();
            }

            //buscar cliente
            var idCliente = string.Format("{0}_1_{1}", i_customer, "1");

            var builder = Builders<Customer>.Filter;
            var filterCust = builder.Eq("_id", idCliente);
            var docCust = colCustomer.Find(filterCust).FirstOrDefault();

            //si no existe 404
            if (docCust is null)
            {
                return NotFound();
            }

            var res = new Models.GetCustomer360ViewResult();
            res.Customer = docCust;

            //buscar productos
            var builderProd = Builders<CustomerProductSingle>.Filter;
            var filterProd = builderProd.Eq("CustomerKey", i_customer);
            var docProduct = colProducts.Find(filterProd).ToList();

            //armar respuesta
            res.CustomerProductList = new Models.CustomerProductList()
            {
                Product = docProduct.ToArray()
            };


            //retornar resultado
            return Ok(res);
        }
    }
}
