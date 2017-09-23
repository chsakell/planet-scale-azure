using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Azure.Services;
using Online.Store.Core.DTOs;

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

        // GET: api/Carts
        [HttpGet(Name = "GetCart")]
        public async Task<CartDTO> GetAsync()
        {
            string cartId = Request.Cookies["cart"];

            var cart = string.IsNullOrEmpty(cartId) ? null : await _storeService.GetCart(cartId);

            return cart;
        }
        
        // POST: api/Carts
        [HttpPost]
        public async Task<CartDTO> PostAsync([FromBody]string productId)
        {
            string cartId = Request.Cookies["cart"];

            var cart = await _storeService.AddProductToCart(cartId, productId);

            Response.Cookies.Append("cart", cart.Id);

            return cart;
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
