using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;

namespace Online.Store.ElasticScale
{
    /// <summary>
    /// Helper methods for interacting with SQL Databases.
    /// </summary>
    internal  class SqlDatabaseUtils
    {
        private readonly Configuration _configuration;

        internal SqlDatabaseUtils(Configuration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// SQL master database name.
        /// </summary>
        public const string MasterDatabaseName = "master";

        /// <summary>
        /// Returns true if we can connect to the database.
        /// </summary>
        public  bool TryConnectToSqlDatabase()
        {
            string connectionString =
                _configuration.GetConnectionString(
                    _configuration.ShardMapManagerServerName,
                    MasterDatabaseName);

            try
            {
                using (ReliableSqlConnection conn = new ReliableSqlConnection(
                    connectionString,
                    SqlRetryPolicy,
                    SqlRetryPolicy))
                {
                    conn.Open();
                }

                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public  bool DatabaseExists(string server, string db)
        {
            using (ReliableSqlConnection conn = new ReliableSqlConnection(
                _configuration.GetConnectionString(server, MasterDatabaseName),
                SqlRetryPolicy,
                SqlRetryPolicy))
            {
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select count(*) from sys.databases where name = @dbname";
                cmd.Parameters.AddWithValue("@dbname", db);
                cmd.CommandTimeout = 60;
                int count = conn.ExecuteCommand<int>(cmd);

                bool exists = count > 0;
                return exists;
            }
        }

        public  bool DatabaseIsOnline(string server, string db)
        {
            using (ReliableSqlConnection conn = new ReliableSqlConnection(
                _configuration.GetConnectionString(server, MasterDatabaseName),
                SqlRetryPolicy,
                SqlRetryPolicy))
            {
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select count(*) from sys.databases where name = @dbname and state = 0"; // online
                cmd.Parameters.AddWithValue("@dbname", db);
                cmd.CommandTimeout = 60;
                int count = conn.ExecuteCommand<int>(cmd);

                bool exists = count > 0;
                return exists;
            }
        }

        public  void CreateDatabase(string server, string db)
        {
            using (ReliableSqlConnection conn = new ReliableSqlConnection(
                _configuration.GetConnectionString(server, MasterDatabaseName),
                SqlRetryPolicy,
                SqlRetryPolicy))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                // Determine if we are connecting to Azure SQL DB
                cmd.CommandText = "SELECT SERVERPROPERTY('EngineEdition')";
                cmd.CommandTimeout = 60;
                int engineEdition = conn.ExecuteCommand<int>(cmd);

                if (engineEdition == 5)
                {
                    // Azure SQL DB
                    SqlRetryPolicy.ExecuteAction(() =>
                    {
                        if (!DatabaseExists(server, db))
                        {
                            // Begin creation (which is async for Standard/Premium editions)
                            cmd.CommandText = string.Format(
                                "CREATE DATABASE {0} (EDITION = '{1}')",
                                BracketEscapeName(db),
                                _configuration.DatabaseEdition);
                            cmd.CommandTimeout = 60;
                            cmd.ExecuteNonQuery();
                        }
                    });

                    // Wait for the operation to complete
                    while (!DatabaseIsOnline(server, db))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }

                }
                else
                {
                    // Other edition of SQL DB
                    conn.ExecuteCommand(cmd);
                }
            }
        }

        public  void DropDatabase(string server, string db)
        {
            using (ReliableSqlConnection conn = new ReliableSqlConnection(
                _configuration.GetConnectionString(server, MasterDatabaseName),
                SqlRetryPolicy,
                SqlRetryPolicy))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                // Determine if we are connecting to Azure SQL DB
                cmd.CommandText = "SELECT SERVERPROPERTY('EngineEdition')";
                cmd.CommandTimeout = 60;
                int engineEdition = conn.ExecuteCommand<int>(cmd);

                // Drop the database
                if (engineEdition == 5)
                {
                    // Azure SQL DB

                    cmd.CommandText = string.Format("DROP DATABASE {0}", BracketEscapeName(db));
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = string.Format(
                        @"ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                        DROP DATABASE {0}",
                        BracketEscapeName(db));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public  void ExecuteSqlScript(string server, string db, string schemaFile)
        {
            using (ReliableSqlConnection conn = new ReliableSqlConnection(
                _configuration.GetConnectionString(server, db),
                SqlRetryPolicy,
                SqlRetryPolicy))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                // Read the commands from the sql script file
                IEnumerable<string> commands = ReadSqlScript(schemaFile);

                foreach (string command in commands)
                {
                    cmd.CommandText = command;
                    cmd.CommandTimeout = 60;
                    conn.ExecuteCommand(cmd);
                }
            }
        }

        private  IEnumerable<string> ReadSqlScript(string scriptFile)
        {
            List<string> commands = new List<string>();
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, string.Format("App_Data\\{0}", scriptFile));
            using (TextReader tr = new StreamReader(path))
            {
                StringBuilder sb = new StringBuilder();
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    if (line == "GO")
                    {
                        commands.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            return commands;
        }

        /// <summary>
        /// Escapes a SQL object name with brackets to prevent SQL injection.
        /// </summary>
        private  string BracketEscapeName(string sqlName)
        {
            return '[' + sqlName.Replace("]", "]]") + ']';
        }

        /// <summary>
        /// Gets the retry policy to use for connections to SQL Server.
        /// </summary>
        public  RetryPolicy SqlRetryPolicy
        {
            get
            {
                return new RetryPolicy<ExtendedSqlDatabaseTransientErrorDetectionStrategy>(10, TimeSpan.FromSeconds(5));
            }
        }

        /// <summary>
        /// Extended sql transient error detection strategy that performs additional transient error
        /// checks besides the ones done by the enterprise library.
        /// </summary>
        private class ExtendedSqlDatabaseTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
        {
            /// <summary>
            /// Enterprise transient error detection strategy.
            /// </summary>
            private SqlDatabaseTransientErrorDetectionStrategy sqltransientErrorDetectionStrategy = new SqlDatabaseTransientErrorDetectionStrategy();

            /// <summary>
            /// Checks with enterprise library's default handler to see if the error is transient, additionally checks
            /// for such errors using the code in the in <see cref="IsTransientException"/> function.
            /// </summary>
            /// <param name="ex">Exception being checked.</param>
            /// <returns><c>true</c> if exception is considered transient, <c>false</c> otherwise.</returns>
            public bool IsTransient(Exception ex)
            {
                return this.sqltransientErrorDetectionStrategy.IsTransient(ex) || IsTransientException(ex);
            }

            /// <summary>
            /// Detects transient errors not currently considered as transient by the enterprise library's strategy.
            /// </summary>
            /// <param name="ex">Input exception.</param>
            /// <returns><c>true</c> if exception is considered transient, <c>false</c> otherwise.</returns>
            private  bool IsTransientException(Exception ex)
            {
                SqlException se = ex as SqlException;

                if (se != null && se.InnerException != null)
                {
                    Win32Exception we = se.InnerException as Win32Exception;

                    if (we != null)
                    {
                        switch (we.NativeErrorCode)
                        {
                            case 0x102:
                                // Transient wait expired error resulting in timeout
                                return true;
                            case 0x121:
                                // Transient semaphore wait expired error resulting in timeout
                                return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
