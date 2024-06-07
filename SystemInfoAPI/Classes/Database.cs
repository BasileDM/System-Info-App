using System.Data.SqlClient;

namespace SystemInfoApi.Classes
{
    public class Database
    {
        protected readonly IConfiguration _Configuration;
        protected readonly string? _ConnectionString;

        public Database(IConfiguration configuration)
        {
            _Configuration = configuration;
            _ConnectionString = _Configuration.GetConnectionString("SystemInfoDbSSMS");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_ConnectionString);
        }

        public IEnumerable<KeyValuePair<string, string?>> GetConnectionStrings()
        {
            return _Configuration.GetSection("ConnectionStrings").AsEnumerable();
        }

        public void LogConnectionStrings()
        {
            IEnumerable<KeyValuePair<string, string?>> coStrs = GetConnectionStrings();
            foreach (KeyValuePair<string, string?> coStr in coStrs) {
                Console.WriteLine(coStr.Key);
                Console.WriteLine(coStr.Value);
            }
        }
    }
}
