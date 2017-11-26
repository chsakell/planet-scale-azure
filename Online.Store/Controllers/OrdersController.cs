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
using Online.Store.SqlServer;
using Online.Store.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    [Authorize]
    public class OrdersController : Controller
    {
        private IStoreService _storeService;
        private IServiceBusService _serviceBusService;
        private ApplicationDbContext _context;

        public OrdersController(IStoreService storeService, IServiceBusService serviceBusService, ApplicationDbContext context)
        {
            _storeService = storeService;
            _serviceBusService = serviceBusService;
            _context = context;
        }

        // GET: api/orders/id
        [HttpGet("{id}", Name = "GetOrders")]
        [Authorize]
        public async Task<List<OrderViewModel>> Get(string id)
        {
            List<OrderViewModel> orders = new List<OrderViewModel>();

            var ordersDB = _context.Orders
                    .Include(order => order.OrderDetails).Where(o => o.UserId == id).ToList();

            orders = Mapper.Map<List<Order>, List<OrderViewModel>>(ordersDB);

            return orders;
        }

        // POST: api/Orders
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]string cartId)
        {
            var cart = string.IsNullOrEmpty(cartId) ? null : await _storeService.GetCart(cartId);

            if (cart != null && cartId == Request.Cookies["cart"])
            {
                string userId = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                                   .Select(c => c.Value).SingleOrDefault();

                Order order = new Order
                {
                    UserId = userId,
                    DateCreated = DateTime.Now
                };

                foreach (var item in cart.Items)
                {
                    order.OrderDetails.Add(new OrderDetail()
                    {
                        ProductId = item.Id,
                        ProductTitle = item.Title,
                        ProductModel = item.Model,
                        ProductPrice = item.Price,
                        Quantity = item.Quantity
                    });
                }

                await _serviceBusService.SubmitOrderAsync(order);

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
