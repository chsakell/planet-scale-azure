using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Core.DTOs;
using Online.Store.Azure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Online.Store.Models;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        IStoreService _storeService;

        public OrdersController(IStoreService storeService, UserManager<ApplicationUser> userManager)
        {
            _storeService = storeService;
            _userManager = userManager;
        }
       
        // POST: api/Orders
        [HttpPost]
        public async Task<int?> Post([FromBody]string cartId)
        {
            int? orderId = null;
            
            var cart = string.IsNullOrEmpty(cartId) ? null : await _storeService.GetCart(cartId);

            if(cart != null && cartId == Request.Cookies["cart"])
            {
                var user = await _userManager.GetUserAsync(User);
                OrderDTO order = new OrderDTO
                {
                    UserId = new Guid(user.Id),
                    DateCreated = DateTime.Now
                };

                foreach(var item in cart.Items)
                {
                    order.Items.Add(new OrderDetailDTO()
                    {
                        ProductId = item.Id,
                        Price = item.Price,
                        Quantity = item.Quantity
                    });
                }

                orderId = _storeService.AddOrder(order);
            }

            return orderId;
            
        }
        
        // PUT: api/Orders/5
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
