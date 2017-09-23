using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Azure.Services;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Carts")]
    public class CartsController : Controller
    {
        IStoreService _storeService;

        public CartsController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        // GET: api/Carts/5
        [HttpGet("{id}", Name = "GetCart")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Carts
        [HttpPost]
        public async Task PostAsync([FromBody]string productId)
        {
            string cartId = Request.Cookies["cart"];

            var cart = await _storeService.AddProductToCart(cartId, productId);

            Response.Cookies.Append("cart", cart.Id);
        }
        
        // PUT: api/Carts/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

    }
}
