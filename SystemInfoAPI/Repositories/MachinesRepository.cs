using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class MachinesRepository(IConfiguration configuration) : Database(configuration)
    {

        /// <summary>Gets all the machines without details (embedded models).</summary>
        /// <returns>
        ///   A <see cref="List{MachineModel}"/> of instantiated <see cref="MachineModel"/>.
        /// </returns>
        public List<MachineModel> GetAll() {

            string sqlRequest = "SELECT * FROM Client_Machine";
            SqlCommand cmd = new(sqlRequest, _SqlConnection);

            List<MachineModel> machinesList = [];

            _SqlConnection.Open();
            
            using (SqlDataReader reader = cmd.ExecuteReader()) {
                while (reader.Read()) {

                    MachineModel newMachine = new() {
                        Id = Convert.ToInt32(reader["id_client_machine"]),
                        Name = Convert.ToString(reader["Name"]),
                        CustomerId = Convert.ToInt32(reader["id_client"]),
                    };
                    machinesList.Add(newMachine);
                }
            }
            _SqlConnection.Close();
            return machinesList;
        }

        /// <summary>Gets a machine with details (embedded Drives, OS etc.).</summary>
        /// <param name="id">The id of the machine in the database.</param>
        /// <returns>
        ///   A <see cref="MachineModel"/> instanciated from the data from the DB.
        /// </returns>
        public MachineModel GetMachineById(int id) {

            string sqlRequest = $"SELECT * FROM Client_Machines WHERE id_client_machine = {id}";

    }
}
