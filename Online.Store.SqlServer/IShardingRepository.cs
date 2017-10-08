using Online.Store.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Online.Store.SqlServer
{
    public interface IShardingRepository
    {
        int? AddOrder(OrderDTO order, int shardingKey);
        int TotalOrders();
    }
}
