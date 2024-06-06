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
            List<DriveModel> drivesList = [];

            const string sqlRequest =
                "SELECT id_client AS Customer_Id," +
                    "Machine.id_client_machine AS Machine_Id, " +
                    "Machine.Name AS Machine_Name, " +
                    "Drive.id_client_machine_disque AS Drive_Id, " +
                    "Drive.Name AS Drive_Name, " +
                    "Root_Directory, " +
                    "Label, " +
                    "Type, " +
                    "Format, " +
                    "Size, " +
                    "Free_Space, " +
                    "Total_Space, " +
                    "Free_Space_Percentage, " +
                    "Is_System_Drive, " +
                    "id_client_machine_disque_os, " +
                    "Directory, " +
                    "Architecture, " +
                    "Version, " +
                    "Product_Name, " +
                    "Release_Id, " +
                    "Current_Build, " +
                    "Ubr " +
                "FROM Client_Machine AS Machine " +
                    "LEFT OUTER JOIN Client_Machine_Disque AS Drive " +
                "ON Machine.id_client_machine = Drive.id_client_machine " +
                    "LEFT OUTER JOIN Client_Machine_Disque_Os AS Os " +
                "ON Os.id_client_machine_disque = Drive.id_client_machine_disque " +
                "WHERE Machine.id_client_machine = @Id";

            await _SqlConnection.OpenAsync();

            SqlCommand cmd = new(sqlRequest, _SqlConnection);
            cmd.Parameters.AddWithValue("Id", id);

            using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) {
                while (await reader.ReadAsync()) {
                    machine.Id = Convert.ToInt32(reader["Machine_Id"]);
                    machine.Name = (string)reader["Machine_Name"];
                    machine.CustomerId = Convert.ToInt32(reader["Customer_Id"]);

                    if (reader["Drive_Id"] != DBNull.Value) {
                        DriveModel drive = new() {
                            Id = Convert.ToInt32(reader["Drive_Id"]),
                            Name = (string)reader["Drive_Name"],
                            RootDirectory = (string)reader["Root_Directory"],
                            Label = (string)reader["Label"],
                            Type = (string)reader["Type"],
                            Format = (string)reader["Format"],
                            Size = Convert.ToInt64(reader["Size"]),
                            FreeSpace = Convert.ToInt64(reader["Free_Space"]),
                            TotalSpace = Convert.ToInt64(reader["Total_Space"]),
                            FreeSpacePercentage = Convert.ToInt32(reader["Free_Space_Percentage"]),
                            IsSystemDrive = Convert.ToBoolean(reader["Is_System_Drive"]),
                            MachineId = Convert.ToInt32(reader["Machine_Id"]),
                        };

                        if (drive.IsSystemDrive) {
                            OsModel Os = new() {
                                Id = Convert.ToInt32(reader["id_client_machine_disque_os"]),
                                Directory = (string)(reader["Directory"]),
                                Architecture = (string)(reader["Architecture"]),
                                Version = (string)(reader["Version"]),
                                ProductName = (string)(reader["Product_Name"]),
                                ReleaseId = (string)(reader["Release_Id"]),
                                CurrentBuild = (string)(reader["Current_Build"]),
                                Ubr = (string)(reader["Ubr"]),
                                DriveId = Convert.ToInt32(reader["Drive_Id"])
                            };
                            drive.Os = Os;
                        }

                        drivesList.Add(drive);
                    }
                }
            }
            await _SqlConnection.CloseAsync();

            machine.Drives = drivesList;

            return machine;
        }

        //public async Task<MachineModel> PostAsync(MachineModel newMachine) {
        //    string json = await JsonSerializer.DeserializeAsync<MachineModel>(newMachine);
        //    var machine = newMachine;

        //}
    }
}
