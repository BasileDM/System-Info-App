using System.Data.SqlClient;

namespace SystemInfoApi.Classes
{
    public delegate Task<T> TransactionOperation<T>(SqlConnection connection, SqlTransaction transaction);

    public class Database
    {
        protected readonly IConfiguration _Configuration;
        protected readonly string? _ConnectionString;

        public Database(IConfiguration configuration, IWebHostEnvironment env)
        {
            _Configuration = configuration;

            _ConnectionString = env.IsDevelopment() ?
                _Configuration.GetSection("ConnectionStrings")["SystemInfoDbDev"] :
                _Configuration.GetSection("ConnectionStrings")["SysteminfoDb"];
        }

        public static void Init(WebApplication app)
        {
            string? connectionString = app.Environment.IsDevelopment() ?
                app.Configuration.GetConnectionString("SystemInfoDbDev") :
                app.Configuration.GetConnectionString("SysteminfoDb");

            using (SqlConnection connection = new(connectionString))
            {
                TryConnection(connection);
                if (!DoTablesExist(connection)) { CreateTables(connection, app); }
            };
        }

        private static void TryConnection(SqlConnection connection)
        {
            try
            {
                connection.Open();
                Console.WriteLine("Connection to the database established successfully.\r\n");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error connecting to the database, please check your appsettings.json configuration file.", ex);
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
                throw new Exception(
                    "Error verifying database tables.", ex);
            }
        }

        private static void CreateTables(SqlConnection connection, WebApplication app)
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

                    using SqlCommand cmd = new(script, connection);
                    cmd.ExecuteNonQuery();
                }
                else if (answer == "n")
                {
                    Console.WriteLine("Aborting table creation. The app will shut down.\r\n");
                    app.StopAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error executing SQL migration script. Failed to create necessary tables.", ex);
            }
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_ConnectionString);
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
                Console.WriteLine("Transaction rolled back due to an argument error: " + ex.Message);
                Console.ForegroundColor = currentColor;
                throw new ArgumentException("Error finalising the transaction with the database. Rolling back...", ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Transaction rolled back due to an unexpected error: " + ex.Message);
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
