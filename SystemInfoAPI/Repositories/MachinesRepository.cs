using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using SystemInfoApi.Classes;

namespace SystemInfoApi.Repositories
{
    public class MachinesRepository(IConfiguration configuration) : Database(configuration)
    {
        public List<string?> GetAll() {
            string sqlRequest = "SELECT * FROM Client_Machine";
            SqlCommand cmd = new(sqlRequest, _SqlConnection);

            List<string?> machinesList = [];

            _SqlConnection.Open();
            
            using (SqlDataReader reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    string? machineName = Convert.ToString(reader["Name"]);
                    machinesList.Add(machineName);
                }
            }

            _SqlConnection.Close();

            return machinesList;
        }
    }
}
