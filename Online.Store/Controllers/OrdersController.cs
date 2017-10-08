using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Core.DTOs;
using Online.Store.Azure.Services;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    public class OrdersController : Controller
    {

        IStoreService _storeService;

        public OrdersController(IStoreService storeService)
        {
            _storeService = storeService;
        }
       
        // POST: api/Orders
        [HttpPost]
        public async Task<int?> Post([FromBody]string cartId)
        {
            int? orderId = null;
            var cart = string.IsNullOrEmpty(cartId) ? null : await _storeService.GetCart(cartId);

            if(cart != null && cartId == Request.Cookies["cart"])
            {
                OrderDTO order = new OrderDTO
                {
                    UserId = Guid.NewGuid(),
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
