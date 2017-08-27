using System.Configuration;
using System.Data.SqlClient;

namespace Online.Store.ElasticScale
{
    /// <summary>
    /// Provides access to app.config settings, and contains advanced configuration settings.
    /// </summary>
    internal  class Configuration
    {
        private readonly string _elasticScaleDb;
        private readonly string _serverName;
        private readonly string _databaseEdition;
        private readonly string _elasticDbUsername;
        private readonly string _elasticDbPassword;
        private readonly bool _integratedSecurity;

        internal Configuration(
            string elasticScaleDb, 
            string serverName, 
            string databaseEdition, 
            string elasticDbUsername, 
            string elasticDbPassword,
            string integratedSecurity)
        {
            _elasticScaleDb = elasticScaleDb;
            _serverName = serverName;
            _databaseEdition = databaseEdition;
            _elasticDbUsername = elasticDbUsername;
            _elasticDbPassword = elasticDbPassword;
            _integratedSecurity = integratedSecurity != null && bool.Parse(integratedSecurity);
        }

        ///
        /// <summary>
        /// Gets the server name for the Shard Map Manager database, which contains the shard maps.
        /// </summary>
        public  string ShardMapManagerServerName
        {
            get { return ServerName; }
        }

        /// <summary>
        /// Gets the database name for the Shard Map Manager database, which contains the shard maps.
        /// </summary>
        public  string ShardMapManagerDatabaseName
        {
            get { return _elasticScaleDb; }
        }

        /// <summary>
        /// Gets the name for the Shard Map that contains metadata for all the shards and the mappings to those shards.
        /// </summary>
        public  string ShardMapName
        {
            get { return "OrderIDShardMap"; }
        }

        /// <summary>
        /// Gets the server name from the App.config file for shards to be created on.
        /// </summary>
        private  string ServerName
        {
            get { return _serverName; }
        }

        /// <summary>
        /// Gets the edition to use for Shards and Shard Map Manager Database if the server is an Azure SQL DB server. 
        /// If the server is a regular SQL Server then this is ignored.
        /// </summary>
        public  string DatabaseEdition
        {
            get
            {
                return _databaseEdition;
            }
        }

        /// <summary>
        /// Returns a connection string that can be used to connect to the specified server and database.
        /// </summary>
        public  string GetConnectionString(string serverName, string database)
        {
            SqlConnectionStringBuilder connStr = new SqlConnectionStringBuilder(GetCredentialsConnectionString());
            connStr.DataSource = serverName;
            connStr.InitialCatalog = database;
            return connStr.ToString();
        }

        /// <summary>
        /// Returns a connection string to use for Data-Dependent Routing and Multi-Shard Query,
        /// which does not contain DataSource or InitialCatalog.
        /// </summary>
        public  string GetCredentialsConnectionString()
        {
            // Get User name and password from the app.config file. If they don't exist, default to string.Empty.
            string userId = _elasticDbUsername ?? string.Empty;
            string password = _elasticDbPassword ?? string.Empty;

            SqlConnectionStringBuilder connStr = new SqlConnectionStringBuilder
            {
                // DDR and MSQ require credentials to be set
                UserID = userId,
                Password = password,
                IntegratedSecurity = _integratedSecurity,

                // DataSource and InitialCatalog cannot be set for DDR and MSQ APIs, because these APIs will
                // determine the DataSource and InitialCatalog for you.
                //
                // DDR also does not support the ConnectRetryCount keyword introduced in .NET 4.5.1, because it
                // would prevent the API from being able to correctly kill connections when mappings are switched
                // offline.
                //
                // Other SqlClient ConnectionString keywords are supported.

                ApplicationName = "Contosodo.0",
                ConnectTimeout = 30
            };
            return connStr.ToString();
        }
    }
}
