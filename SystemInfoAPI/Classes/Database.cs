using System.Data.SqlClient;

namespace SystemInfoApi.Classes
{
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

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_ConnectionString);
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
    }
}
