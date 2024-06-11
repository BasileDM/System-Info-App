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

        public static bool Init(WebApplication app)
        {
            string? connectionString = app.Environment.IsDevelopment() ?
                app.Configuration.GetConnectionString("SystemInfoDbDev") :
                app.Configuration.GetConnectionString("SysteminfoDb");

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connection to the database established successfully."); 
                    
                    if (!CheckIfTablesExist(connection))
                    {
                        Console.WriteLine("Tables not detected. Creating tables...");
                        string migrationPath = AppDomain.CurrentDomain.BaseDirectory + "/Migrations/SysteminfoDb.sql";
                        ExecuteSqlScript(connection, migrationPath);
                    }
                    return true;
                };
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error: Could not connect to the database, please check your appsettings.json configuration file.", ex);
            }
        }

        private static bool CheckIfTablesExist(SqlConnection connection)
        {
            string checkTablesSql = @"
                SELECT COUNT(*) 
                FROM information_schema.tables 
                WHERE table_name = 'Client_Machine'";

            using SqlCommand cmd = new(checkTablesSql, connection);
            int? tableCount = (int?)cmd.ExecuteScalar();
            return tableCount > 0;
        }

        private static void ExecuteSqlScript (SqlConnection connection, string path)
        {
            string script = File.ReadAllText(path);

            using SqlCommand cmd = new(script, connection);
            cmd.ExecuteNonQuery();
        }
    }
}
