using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Azure.Services;
using Online.Store.Core.DTOs;
using Online.Store.ViewModels;
using Microsoft.AspNetCore.Identity;
using Online.Store.Models;

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
        public async Task<UserCartViewModel> GetAsync()
        {
            UserCartViewModel userCart = null;

            string cartId = Request.Cookies["cart"];

            var cart = string.IsNullOrEmpty(cartId) ? null : await _storeService.GetCart(cartId);

            userCart = new UserCartViewModel()
            {
                Cart = cart
            };

            if (User.Identity.IsAuthenticated)
            {
                userCart.User = new UserViewModel()
                {
                    Id = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").First().Value,
                    Username = User.Claims.Where(c => c.Type == "name").First().Value
                };
            }

            return userCart;
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
        public async Task<CartDTO> DeleteAsync(string id)
        {
            string cartId = Request.Cookies["cart"];

            var cart = await _storeService.RemoveProductFromCart(cartId, id);

            return cart;
        }

    }
}
