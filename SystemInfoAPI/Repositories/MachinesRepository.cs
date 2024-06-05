using System.Data.SqlClient;
using System.Text.Json;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class MachinesRepository(IConfiguration config) : Database(config)
    {

        /// <summary>Gets all the machines without details (embedded models).</summary>
        /// <returns>
        ///   A <see cref="List{MachineModel}"/> of instantiated <see cref="MachineModel"/>.
        /// </returns>
        public async Task<List<MachineModel>> GetAllAsync() {

            List<MachineModel> machinesList = [];

            const string sqlRequest = 
                "SELECT * FROM Client_Machine";

            await _SqlConnection.OpenAsync();
            SqlCommand cmd = new(sqlRequest, _SqlConnection);

            using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) {
                while (await reader.ReadAsync()) {
                    machinesList.Add(new MachineModel {
                        Id = Convert.ToInt32(reader["id_client_machine"]), // (reader.GetOrdinal("id_client_machine") ?
                        Name = Convert.ToString((string)reader["Name"]),
                        CustomerId = Convert.ToInt32(reader["id_client"]),
                    });
                }
            }
            await _SqlConnection.CloseAsync();

            return machinesList;
        }

        /// <summary>Gets a machine with details (embedded Drives, OS etc.).</summary>
        /// <param name="id">The id of the machine in the database.</param>
        /// <returns>
        ///   A <see cref="MachineModel"/> instanciated from the data from the DB.
        /// </returns>
        public async Task<MachineModel> GetByIdAsync(int id) {

            MachineModel machine = new();

            const string sqlRequest = 
                "SELECT * FROM Client_Machine WHERE id_client_machine = @id";

            await _SqlConnection.OpenAsync();
            SqlCommand cmd = new(sqlRequest, _SqlConnection);
            cmd.Parameters.AddWithValue("Id", id);

            using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) {
                if (await reader.ReadAsync()) {
                    machine.Id = Convert.ToInt32(reader["id_client_machine"]);
                    machine.Name = Convert.ToString((string)reader["Name"]);
                    machine.CustomerId = Convert.ToInt32(reader["id_client"]);
                }
            }
            await _SqlConnection.CloseAsync();

            return machine;
        }

        //public async Task<MachineModel> PostAsync(MachineModel newMachine) {
        //    string json = await JsonSerializer.DeserializeAsync<MachineModel>(newMachine);
        //    var machine = newMachine;

        //}
    }
}
