using System.Data.SqlClient;

namespace SystemInfoApi.Classes
{
    public delegate Task<T> TransactionOperation<T>(SqlConnection connection, SqlTransaction transaction);

    public class Database
    {
        protected readonly string? _ConnectionString;
        protected readonly IConfigurationSection _DbConfig;
        protected readonly IConfigurationSection _TablesNames;

        public Database(IConfiguration configuration, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ConnectionString = configuration.GetSection("ConnectionStrings")["SystemInfoDbDev"];
                _DbConfig = configuration.GetSection("DatabaseConfigDev");
            }
            else
            {
                _ConnectionString = configuration.GetSection("ConnectionStrings")["SysteminfoDb"];
                _DbConfig = configuration.GetSection("DatabaseConfig");
            }
            _TablesNames = _DbConfig.GetSection("TablesNames");

        }

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(_ConnectionString);
        }

        public void Init(WebApplication app)
        {
            try
            {
                using SqlConnection connection = CreateConnection();
                TryOpenConnection(connection);
                if (!DoTablesExist(connection))
                {
                    PromptTablesCreation();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed:\r\n{ex.Message}\r\n");
                app.StopAsync();
                return;
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

        private bool DoTablesExist(SqlConnection connection)
        {
            try
            {
                string checkTablesSql = @$"
                    SELECT COUNT(*) 
                    FROM information_schema.tables 
                    WHERE table_name = '{_TablesNames["MachinesTable"]}'";

                using SqlCommand cmd = new(checkTablesSql, connection);
                int? tableCount = (int?)cmd.ExecuteScalar();
                return tableCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error verifying database tables: {ex.Message}");
            }
        }

        private void PromptTablesCreation()
        {
            string? answer;
            do
            {
                Console.WriteLine("Database tables not detected. Do you want to create them ? y/n ");
                answer = Console.ReadLine()?.ToLower();

            } while (answer != "y" && answer != "n");

            if (answer == "y")
            {
                CreateTablesAsync().Wait();
            }
            else if (answer == "n")
            {
                throw new Exception("Table creation has been aborted, the app will shut down.");
            }
        }

        private async Task<bool> CreateTablesAsync()
        {
            return await MakeTransactionAsync(async (connection, transaction) =>
            {
                Console.WriteLine("Creating tables...");

                string migrationPath = AppDomain.CurrentDomain.BaseDirectory + "/Migrations/SysteminfoDb.sql";
                string script = File.ReadAllText(migrationPath);

                script = script.Replace("CustomerTableName", _TablesNames["CustomerTableName"]);

                await using SqlCommand cmd = new(script, connection, transaction);
                await cmd.ExecuteNonQueryAsync();
                return true;
            });
        }


        /// <summary>
        ///     This method is a transaction wrapper for a database operation that contains multiple commands.
        ///     It handles <see cref="SqlConnection"/> and <see cref="SqlTransaction"/> management for a specified operation.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the transaction operation.</typeparam>
        /// <param name="operation">
        ///     A delegate: <see cref="TransactionOperation{T}"/> that represents the operation to be executed within the transaction.
        ///     The operation receives a <see cref="SqlConnection"/> and a <see cref="SqlTransaction"/> as parameters.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> that represents the asynchronous operation. The task result contains the result of the executed operation.
        /// </returns>
        /// <remarks>
        ///     This method opens a connection, begins a transaction, executes the provided <see cref="TransactionOperation{T}"/>,
        ///     and then commits or rolls back the transaction based on the success or failure of the operation.
        /// </remarks>
        protected async Task<T> MakeTransactionAsync<T>(TransactionOperation<T> operation)
        {
            await using SqlConnection connection = CreateConnection();
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
                throw new ArgumentException("Error finalising the transaction with the database.", ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Rolling back transaction due to an unexpected error:\r\n" + ex.Message);
                Console.ForegroundColor = currentColor;
                throw new ApplicationException("Error finalising the transaction with the database.", ex);
            }
            finally
            {
                await transaction.DisposeAsync();
                await connection.CloseAsync();
                Console.WriteLine("Freeing up ressources...\r\n");
            }
        }
    }
}
