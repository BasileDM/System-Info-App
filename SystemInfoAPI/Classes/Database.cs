using System.Data.SqlClient;

namespace SystemInfoApi.Classes
{
    public delegate Task<T> TransactionOperation<T>(SqlConnection connection, SqlTransaction transaction);

    public class Database
    {
        protected readonly string? _ConnectionString;
        protected readonly IConfigurationSection _DbConfig;

        public Database(IConfiguration configuration, IWebHostEnvironment env)
        {
            _ConnectionString = env.IsDevelopment() ?
                configuration.GetSection("ConnectionStrings")["SystemInfoDbDev"] :
                configuration.GetSection("ConnectionStrings")["SysteminfoDb"];

            _DbConfig = configuration.GetSection("DatabaseConfig");
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_ConnectionString);
        }

        public void Init()
        {
            using SqlConnection connection = GetConnection();
            TryOpenConnection(connection);
            if (!DoTablesExist(connection))
            {
                CreateTables(connection);
            }
        }

        private static void TryOpenConnection(SqlConnection connection)
        {
            try
            {
                connection.Open();
                Console.WriteLine("Connection to the database established successfully.\r\n");
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting to the database, please check your appsettings.json configuration file.", ex);
            }

        }

        private static bool DoTablesExist(SqlConnection connection)
        {
            try
            {
                string checkTablesSql = @"
                    SELECT COUNT(*) 
                    FROM information_schema.tables 
                    WHERE table_name = 'Client_Machine'";

                using SqlCommand cmd = new(checkTablesSql, connection);
                int? tableCount = (int?)cmd.ExecuteScalar();
                return tableCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error verifying database tables.", ex);
            }
        }

        private void CreateTables(SqlConnection connection)
        {
            try
            {
                string? answer;
                do
                {
                    Console.WriteLine("Database tables not detected. Do you want to create them ? y/n \r\n");
                    answer = Console.ReadLine()?.ToLower();

                } while (answer != "y" && answer != "n");

                if (answer == "y")
                {
                    Console.WriteLine("Creating tables...\r\n");

                    string migrationPath = AppDomain.CurrentDomain.BaseDirectory + "/Migrations/SysteminfoDb.sql";
                    string script = File.ReadAllText(migrationPath);

                    if (_DbConfig["CustomersTableName"] != null)
                    {
                        script = script.Replace("Client", _DbConfig["CustomersTableName"]);
                    }

                    using SqlCommand cmd = new(script, connection);
                    cmd.ExecuteNonQuery();
                }
                else if (answer == "n")
                {
                    throw new Exception("Table creation has been aborted, the app will shut down.\r\n");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error during tables creation. {ex.Message}");
            }
        }


        /// <summary>
        /// Handles <see cref="SqlConnection"/> and <see cref="SqlTransaction"/> management for a specified operation.
        /// This method opens a connection, begins a transaction, executes the provided operation,
        /// and then commits or rolls back the transaction based on the success or failure of the operation.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the transaction operation.</typeparam>
        /// <param name="operation">
        /// A delegate that represents the operation to be executed within the transaction.
        /// The operation receives a <see cref="SqlConnection"/> and a <see cref="SqlTransaction"/> as parameters.
        /// </param>
        /// <returns>
        ///   A task that represents the asynchronous operation. The task result contains the result of the executed operation.
        /// </returns>
        /// <remarks>
        /// This method ensures that the transaction is committed if the operation succeeds and rolled back if any error occurs.
        /// </remarks>
        protected async Task<T> MakeTransactionAsync<T>(TransactionOperation<T> operation)
        {
            await using SqlConnection connection = GetConnection();
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            Console.WriteLine("\r\nNew database transaction initiated.");
            var currentColor = Console.ForegroundColor;

            try
            {
                T result = await operation(connection, transaction);
                await transaction.CommitAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Database transaction successful.");
                Console.ForegroundColor = currentColor;
                return result;
            }
            catch (ArgumentException ex)
            {
                await transaction.RollbackAsync();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Rolling back transaction due to an argument error:\r\n" + ex.Message);
                Console.ForegroundColor = currentColor;
                throw new ArgumentException("Error finalising the transaction with the database. Rolling back...", ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Rolling back transaction due to an unexpected error:\r\n" + ex.Message);
                Console.ForegroundColor = currentColor;
                throw new ApplicationException("Error finalising the transaction with the database. Rolling back...", ex);
            }
            finally
            {
                await transaction.DisposeAsync();
                await connection.CloseAsync();
                Console.WriteLine("Freeing up ressources...");
            }
        }
    }
}
