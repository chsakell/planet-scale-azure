using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using System.Collections.Generic;
using System.Linq;

namespace Online.Store.ElasticScale
{
    public class CreateShardService
    {
        private readonly Configuration _configuration;
        private readonly SqlDatabaseUtils _utils;
        private readonly ShardManagementUtils _shardUtils;

        public CreateShardService(string elasticScaleDb,
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
        }

        internal CreateShardService(Configuration configuration)
        {
            _configuration = configuration;
            _utils = new SqlDatabaseUtils(configuration);
            _shardUtils = new ShardManagementUtils(configuration);
        }

        /// <summary>
        /// Format to use for creating shard name. {0} is the number of shards that have already been created.
        /// </summary>
        //private const string ShardNameFormat = string.Format("{0}_{1}",ConfigurationManager.AppSettings["ElasticScaleDb"],"Shard{0}");

        public string ShardNameFormat
        {
            get
            {
                return string.Format("{0}_{1}", _configuration.ShardMapManagerDatabaseName, "Shard{0}");
            }
        }

        /// <summary>
        /// Script file that will be executed to initialize a shard.
        /// </summary>
        private const string InitializeShardScriptFile = "InitializeShard.sql";

        /// <summary>
        /// Creates a new shard (or uses an existing empty shard), adds it to the shard map,
        /// and assigns it the specified range if possible.
        /// </summary>
        public void CreateShard(RangeShardMap<int> shardMap, Range<int> rangeForNewShard)
        {
            // Create a new shard, or get an existing empty shard (if a previous create partially succeeded).
            Shard shard = CreateOrGetEmptyShard(shardMap);

            // Create a mapping to that shard.
            RangeMapping<int> mappingForNewShard = shardMap.CreateRangeMapping(rangeForNewShard, shard);
        }

        private Shard CreateOrGetEmptyShard(RangeShardMap<int> shardMap)
        {
            // Get an empty shard if one already exists, otherwise create a new one
            Shard shard = FindEmptyShard(shardMap);
            if (shard == null)
            {
                // No empty shard exists, so create one

                // Choose the shard name
                string databaseName = string.Format(ShardNameFormat, shardMap.GetShards().Count());

                // Only create the database if it doesn't already exist. It might already exist if
                // we tried to create it previously but hit a transient fault.
                if (!_utils.DatabaseExists(_configuration.ShardMapManagerServerName, databaseName))
                {
                    _utils.CreateDatabase(_configuration.ShardMapManagerServerName, databaseName);
                }

                // Create schema and populate reference data on that database
                // The initialize script must be idempotent, in case it was already run on this database
                // and we failed to add it to the shard map previously
                _utils.ExecuteSqlScript(
                    _configuration.ShardMapManagerServerName, databaseName, InitializeShardScriptFile);

                // Add it to the shard map
                ShardLocation shardLocation = new ShardLocation(_configuration.ShardMapManagerServerName, databaseName);
                shard = _shardUtils.CreateOrGetShard(shardMap, shardLocation);
            }

            return shard;
        }

        private Shard FindEmptyShard(RangeShardMap<int> shardMap)
        {
            // Get all shards in the shard map
            IEnumerable<Shard> allShards = shardMap.GetShards();

            // Get all mappings in the shard map
            IEnumerable<RangeMapping<int>> allMappings = shardMap.GetMappings();

            // Determine which shards have mappings
            HashSet<Shard> shardsWithMappings = new HashSet<Shard>(allMappings.Select(m => m.Shard));

            // Get the first shard (ordered by name) that has no mappings, if it exists
            return allShards.OrderBy(s => s.Location.Database).FirstOrDefault(s => !shardsWithMappings.Contains(s));
        }
    }
}
