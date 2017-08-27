using Online.Store.Core.DTOs;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement.Schema;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Online.Store.ElasticScale
{
    public class AzureElasticScaleServices
    {
        private  ShardMapManager _shardMapManager { get; set; }
        private readonly Configuration _configuration;
        private readonly SqlDatabaseUtils _utils;
        private readonly ShardManagementUtils _shardUtils;

        #region "Public Method"

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureElasticScaleServices"/> class.
        /// </summary>
        public AzureElasticScaleServices(
            string elasticScaleDb,
            string serverName,
            string databaseEdition,
            string elasticDbUsername,
            string elasticDbPassword,
            string integratedSecurity)
        {
            _configuration = new Configuration(
                elasticScaleDb,
                serverName,
                databaseEdition,
                elasticDbUsername,
                elasticDbPassword,
                integratedSecurity);
            _utils = new SqlDatabaseUtils(_configuration);
            _shardUtils = new ShardManagementUtils(_configuration);

            // Get the shard map manager, if it already exists.
            // It is recommended that you keep only one shard map manager instance in
            // memory per AppDomain so that the mapping cache is not duplicated.
            _shardMapManager = _shardUtils.TryGetShardMapManager(
                _configuration.ShardMapManagerServerName,
                _configuration.ShardMapManagerDatabaseName);
        }

        /// <summary>
        /// Inserts the data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void InsertData(CartDTO data)
        {
            CheckForShardMap();
            RangeShardMap<int> shardMap = TryGetShardMap();
            int currentMaxHighKey = shardMap.GetMappings().Max(m => m.Value.High);

            AddOrder(shardMap, _configuration.GetCredentialsConnectionString(), data, currentMaxHighKey);
        }

        #endregion "Public Methord"

        #region "Private Methods"

        /// <summary>
        /// Checks for shard or create it.
        /// </summary>
        private void CheckForShardMap()
        {
            if (_shardMapManager == null)
            {
                // Create shard map manager database
                if (!_utils.DatabaseExists(_configuration.ShardMapManagerServerName, _configuration.ShardMapManagerDatabaseName))
                {
                    _utils.CreateDatabase(_configuration.ShardMapManagerServerName, _configuration.ShardMapManagerDatabaseName);
                }

                // Create shard map manager
                string shardMapManagerConnectionString = _configuration.GetConnectionString(_configuration.ShardMapManagerServerName, _configuration.ShardMapManagerDatabaseName);

                _shardMapManager = _shardUtils.CreateOrGetShardMapManager(shardMapManagerConnectionString);

                // Create shard map
                RangeShardMap<int> shardMap = _shardUtils.CreateOrGetRangeShardMap<int>(
                    _shardMapManager, _configuration.ShardMapName);

                CreateSchemaInfo(shardMap.Name);

                // If there are no shards, add two shards: one for [0,100) and one for [100,+inf)
                if (!shardMap.GetShards().Any())
                {
                    var shardService = new CreateShardService(_configuration);
                    //Create shard for A(1) to N(13)
                    shardService.CreateShard(shardMap, new Range<int>(1, 13));
                    //Create shard for A(13) to N(16)
                    shardService.CreateShard(shardMap, new Range<int>(13, 26));
                }

            }
        }

        /// <summary>
        /// Creates the schema information.
        /// </summary>
        /// <param name="shardMapName">Name of the shard map.</param>
        private  void CreateSchemaInfo(string shardMapName)
        {
            // Create schema info
            SchemaInfo schemaInfo = new SchemaInfo();
            schemaInfo.Add(new ReferenceTableInfo("Orders"));
            // Register it with the shard map manager for the given shard map name
            _shardMapManager.GetSchemaInfoCollection().Add(shardMapName, schemaInfo);
        }

        /// <summary>
        /// Adds an order to the orders table for the customer.
        /// </summary>
        private  void AddOrder(RangeShardMap<int> shardMap, string credentialsConnectionString, CartDTO data, int currentMaxHighKey)
        {
            _utils.SqlRetryPolicy.ExecuteAction(() =>
            {
                // Looks up the key in the shard map and opens a connection to the shard
                int shardKey = GetShardKey(data.UserName);
                using (SqlConnection conn = shardMap.OpenConnectionForKey(shardKey, credentialsConnectionString))
                {
                    foreach (var item in data.CartItems)
                    {
                        long orderId = GetNextOrderId(shardMap, conn);

                        // Create a simple command that will insert a new order
                        SqlCommand cmd = conn.CreateCommand();

                        // Create a simple command
                        cmd.CommandText = @"INSERT INTO dbo.Orders ([OrderID],[UserID],[Orderdate],[ProductID],[ProductName],[ProductPrice],[Qty],[UserName])
                                        VALUES (@OrderID,@UserID,@Orderdate,@ProductID,@ProductName,@ProductPrice,@Qty,@UserName)";
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.Parameters.AddWithValue("@UserID", data.UserId);
                        cmd.Parameters.AddWithValue("@Orderdate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ProductID", item.Products.Id);
                        cmd.Parameters.AddWithValue("@ProductName", item.Products.Title);
                        cmd.Parameters.AddWithValue("@ProductPrice", item.Products.Price);
                        cmd.Parameters.AddWithValue("@Qty", item.Qty);
                        cmd.Parameters.AddWithValue("@UserName", data.UserName);
                        cmd.CommandTimeout = 60;
                        // Execute the command
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        /// <summary>
        /// Gets the shard key.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private  int GetShardKey(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var c = name.ToCharArray().First();
                return (char.ToUpper(c) - 64);
            }
            return 1;
        }

        /// <summary>
        /// Gets the next order id Azure does not support identity..
        /// </summary>
        /// <param name="shardMap">The shard map.</param>
        /// <param name="credentialsConnectionString">The credentials connection string.</param>
        /// <returns></returns>
        private  long GetNextOrderId(ShardMap shardMap, SqlConnection conn)
        {
            long id = 1;

            // Create a simple command
            using (SqlCommand cmd = conn.CreateCommand())
            {
                // Because this query is grouped by CustomerID, which is sharded,
                // we will not get duplicate rows.
                cmd.CommandText = @"
                        SELECT 
                            TOP 1
                            MAX(OrderID)
                        FROM 
                            dbo.Orders 
                        GROUP BY 
                            OrderID
                        ORDER BY 
                            OrderID DESC";

                // Allow the entire command to take up to 30 seconds
                cmd.CommandTimeout = 30;

                // Execute the command. 
                using (var sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        var maxid = sdr.GetValue(0);
                        id = ((long)maxid + 1);
                    }
                }
            }

            return id;
        }

        /// <summary>
        /// Tries the get shard map.
        /// </summary>
        /// <returns></returns>
        private  RangeShardMap<int> TryGetShardMap()
        {
            if (_shardMapManager == null)
            {
                return null;
            }

            RangeShardMap<int> shardMap;
            bool mapExists = _shardMapManager.TryGetRangeShardMap(_configuration.ShardMapName, out shardMap);

            if (!mapExists)
            {
                return null;
            }

            return shardMap;
        }

        #endregion "Private Methords"
    }
}