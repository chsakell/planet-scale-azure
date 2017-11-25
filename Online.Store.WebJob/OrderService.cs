using Online.Store.Core.DTOs;
using Online.Store.SqlServer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.WebJob
{
    public class OrderService : IOrderService
    {
        ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
    }

    public interface IOrderService
    {
        Task AddOrderAsync(Order order);
    }
}
