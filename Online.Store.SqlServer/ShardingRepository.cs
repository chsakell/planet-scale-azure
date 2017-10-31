using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Online.Store.Core.DTOs;
using System.Diagnostics;
using Dapper.Contrib.Extensions;
using Dapper;
using System.Linq;
using Microsoft.Azure.SqlDatabase.ElasticScale.Query;
using System.Data;

namespace Online.Store.SqlServer
{

    public class ShardingRepository : IShardingRepository
    {
        private static ShardMapManager s_shardMapManager;
        private static string s_server;// = "chsakelldbserver.database.windows.net";
        private static string s_shardmapmgrdb; //  = "elasticmaster";
        private static SqlConnectionStringBuilder connStrBldr;
        private static string ShardMapName = "RangeShardMap";

        public ShardingRepository(IConfiguration configuration)
        {
            /* TODO
            s_server = configuration["SQL:ServerName"];
            s_shardmapmgrdb = configuration["SQL:ElasticScaleDb"];

            connStrBldr = new SqlConnectionStringBuilder
            {
                UserID = configuration["SQL:ElasticDbUsername"],
                Password = configuration["SQL:ElasticDbPassword"],
                ApplicationName = "Online_Store"
            };

            if (s_shardMapManager == null)
                s_shardMapManager = TryGetShardMapManager(
                    s_server,
                    s_shardmapmgrdb);
            */
        }

        public int? AddOrder(Order order, int shardingKey)
        {
            int? orderId = null;
            RangeShardMap<int> shardMap = TryGetShardMap();

            SqlDatabaseUtils.SqlRetryPolicy.ExecuteAction(() =>
            {
                using (SqlConnection sqlconn = shardMap.OpenConnectionForKey(
                    key: shardingKey,
                    connectionString: connStrBldr.ConnectionString,
                    options: ConnectionOptions.Validate))
                {
                    // sqlconn.Insert<OrderDTO>(order);
                    string sql = @"INSERT INTO dbo.[Order](Id,UserId,DateCreated) values(@Id,@UserId,@DateCreated); 
                                   SELECT Id From dbo.[Order] where Id = " + shardingKey;

                    orderId = sqlconn.Query<int>(sql,
                                new
                                {
                                    Id = shardingKey,
                                    UserId = order.UserId,
                                    DateCreated = order.DateCreated
                                }).SingleOrDefault();


                    foreach (var item in order.OrderDetails)
                    {
                        item.OrderId = orderId.Value;

                        sqlconn.Query<int>(@"INSERT INTO OrderDetail(OrderId, ProductId, Quantity, Price) 
                                                        values(@OrderId, @ProductId, @Quantity, @Price)
                                                        select cast(scope_identity() as int)", item).First();
                    }
                }
            });

            return orderId;
        }

        public int TotalOrders()
        {
            int totalOrders = 0;

            using (MultiShardConnection conn = new MultiShardConnection(
                                    TryGetShardMap().GetShards(),
                                    connStrBldr.ConnectionString)
      )
            {
                using (MultiShardCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Count(1) FROM dbo.[Order]";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecutionOptions = MultiShardExecutionOptions.IncludeShardNameColumn;
                    cmd.ExecutionPolicy = MultiShardExecutionPolicy.PartialResults;

                    using (MultiShardDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            totalOrders+= sdr.GetInt32(0);
                        }
                    }
                }
            }

            return totalOrders;
        }


        #region utils

        private static ShardMapManager TryGetShardMapManager(string shardMapManagerServerName, string shardMapManagerDatabaseName)
        {
            string shardMapManagerConnectionString =
                    Configuration.GetConnectionString(
                        shardMapManagerServerName,
                        shardMapManagerDatabaseName);

            if (!SqlDatabaseUtils.DatabaseExists(shardMapManagerServerName, shardMapManagerDatabaseName))
            {
                // Shard Map Manager database has not yet been created
                return null;
            }

            ShardMapManager shardMapManager;
            bool smmExists = ShardMapManagerFactory.TryGetSqlShardMapManager(
                shardMapManagerConnectionString,
                ShardMapManagerLoadPolicy.Lazy,
                out shardMapManager);

            if (!smmExists)
            {
                // Shard Map Manager database exists, but Shard Map Manager has not been created
                return null;
            }

            return shardMapManager;
        }

        private static RangeShardMap<int> TryGetShardMap()
        {
            if (s_shardMapManager == null)
            {
                Debug.WriteLine("Shard Map Manager has not yet been created");
                return null;
            }

            RangeShardMap<int> shardMap;
            bool mapExists = s_shardMapManager.TryGetRangeShardMap(ShardMapName, out shardMap);

            if (!mapExists)
            {
                Debug.WriteLine("Shard Map Manager has been created, but the Shard Map has not been created");
                return null;
            }

            return shardMap;
        }

        #endregion
    }
}
