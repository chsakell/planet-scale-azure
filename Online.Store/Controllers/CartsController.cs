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
using Microsoft.Extensions.Configuration;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Carts")]
    public class CartsController : Controller
    {
        IStoreService _storeService;
        private UserManager<ApplicationUser> _userManager;
        private readonly bool _useIdentity;
        private readonly IHttpContextAccessor _contextAccessor;

        public CartsController(IStoreService storeService,
                IConfiguration configuration,
                IHttpContextAccessor contextAccessor)
        {
            _storeService = storeService;
            bool.TryParse(configuration["UseIdentity"], out _useIdentity);
            _contextAccessor = contextAccessor;
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
                Cart = cart,
                UseIdentity = _useIdentity
            };

            if (User.Identity.IsAuthenticated)
            {
                if (!_useIdentity)
                {
                    userCart.User = new UserViewModel()
                    {
                        Id = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").First().Value,
                        Username = User.Claims.Where(c => c.Type == "name").First().Value,
                    };
                }
                else
                {
                    _userManager = (UserManager<ApplicationUser>)_contextAccessor.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
                    var user = await _userManager.GetUserAsync(User);
                    userCart.User = new UserViewModel()
                    {
                        Id = user.Id,
                        Username = user.UserName
                    };
                }
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
