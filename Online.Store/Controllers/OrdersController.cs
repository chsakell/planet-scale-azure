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
using Microsoft.Extensions.Configuration;

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
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly bool _useIdentity;
        private UserManager<ApplicationUser> _userManager;

        public OrdersController(IStoreService storeService, IServiceBusService serviceBusService, 
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor)
        {
            _storeService = storeService;
            _serviceBusService = serviceBusService;
            _context = context;
            bool.TryParse(configuration["UseIdentity"], out _useIdentity);
            _contextAccessor = contextAccessor;
        }

        // GET: api/orders/id
        [HttpGet("{id}", Name = "GetOrders")]
        [Authorize]
        public async Task<List<OrderViewModel>> Get(string id)
        {
            List<OrderViewModel> orders = new List<OrderViewModel>();

            var ordersDB = _context.Orders
                    .Include(order => order.OrderDetails).Where(o => o.UserId == id).ToList();

            var cachedOrders = await _storeService.GetOrders(id);

            if (cachedOrders != null)
            {
                foreach (var order in cachedOrders)
                {
                    if (!ordersDB.Any(o => o.DateCreated == order.DateCreated))
                    {
                        ordersDB.Add(order);
                    }
                }
            }

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
                string userId = null;

                if (!_useIdentity)
                {
                    userId = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                                       .Select(c => c.Value).SingleOrDefault();
                }
                else
                {
                    _userManager = (UserManager<ApplicationUser>)_contextAccessor.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
                    var user = await _userManager.GetUserAsync(User);
                    userId = user.Id;
                }
                Order order = new Order
                {
                    UserId = userId,
                    DateCreated = DateTime.Now.ToUniversalTime(),
                    GrandTotal = 0
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

                    order.GrandTotal += item.Price * item.Quantity;
                }

                await _serviceBusService.SubmitOrderAsync(order);

                await _storeService.RemoveCart("cart-" + cartId);

                Response.Cookies.Delete("cart");

                // Add order to cache
                await _storeService.AddOrder(userId, order);

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
