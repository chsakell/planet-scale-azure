using System;
using System.Collections.Generic;
using System.Text;

namespace Online.Store.Core.DTOs
{
    public class OrderDTO
    {
        public OrderDTO()
        {
            Items = new List<OrderDetailDTO>();
        }
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateCreated { get; set; }

        public ICollection<OrderDetailDTO> Items { get; set; }
    }

    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
