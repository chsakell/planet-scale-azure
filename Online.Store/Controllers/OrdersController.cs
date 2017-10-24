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
using Online.Store.Data;
using Online.Store.ViewModels;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    [Authorize]
    public class OrdersController : Controller
    {
        private IStoreService _storeService;
        private ApplicationDbContext _context;

        public OrdersController(IStoreService storeService, ApplicationDbContext context)
        {
            _storeService = storeService;
            _context = context;
        }
       
        // POST: api/Orders
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]string cartId)
        {
            int? orderId = null;
            
            var cart = string.IsNullOrEmpty(cartId) ? null : await _storeService.GetCart(cartId);

            if(cart != null && cartId == Request.Cookies["cart"])
            {
                string userId = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                                   .Select(c => c.Value).SingleOrDefault();

                Order order = new Order
                {
                    UserId = userId,
                    DateCreated = DateTime.Now
                };

                foreach(var item in cart.Items)
                {
                    order.OrderDetails.Add(new OrderDetail()
                    {
                        ProductId = item.Id,
                        Price = item.Price,
                        Quantity = item.Quantity
                    });
                }

                _context.Orders.Add(order);

                //orderId = _storeService.AddOrder(order);
                await _context.SaveChangesAsync();

                await _storeService.RemoveCart(cartId);
                Response.Cookies.Delete("cart");

                return Ok(new ResultViewModel()
                {
                    Result = Result.SUCCESS
                });
            }

            return Ok(new ResultViewModel()
            {
                Result = Result.ERROR
            });

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
