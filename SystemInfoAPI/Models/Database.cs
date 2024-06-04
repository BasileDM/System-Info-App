using System.Data.SqlClient;

namespace SystemInfoApi.Models
{
    public class Database
    {
        private readonly IConfiguration _configuration;
        private readonly string? _ConnectionString;
        private SqlConnection Connection { get; set; }

        public Database(IConfiguration configuration) {
            _configuration = configuration;
            _ConnectionString = _configuration.GetConnectionString("SystemInfoDbSSMS");
            Connection = new SqlConnection(_ConnectionString);
        }
    }
}
