using SystemInfoApi.Classes;

namespace SystemInfoApi.Repositories
{
    public class MachinesRepository(IConfiguration config) : Database(config)
    {
        /// <summary>Asynchronously create a new machine entry in the database, Drives, Os etc. included.</summary>
        /// <param name="machine">The machine to add to the DB.</param>
        /// <returns>
        ///     The <see cref="MachineModel"/> with the newly created IDs from the database
        /// </returns>
        public async Task<MachineModel> PostAsync(MachineModel machine)
        {
            try
            {
                await using SqlConnection connection = GetConnection();
                await connection.OpenAsync();

                // Machine
                string machineSql = @"
                    INSERT INTO Client_Machine (id_client, Name) 
                    VALUES (@customerId, @machineName);

                    SELECT SCOPE_IDENTITY();";

                int machineId;

                using (SqlCommand cmd = new(machineSql, connection))
                {
                    cmd.Parameters.AddWithValue("@customerId", machine.CustomerId);
                    cmd.Parameters.AddWithValue("@machineName", machine.Name);

                    var newMachineId = await cmd.ExecuteScalarAsync();
                    machineId = Convert.ToInt32(newMachineId);
                    machine.Id = machineId;
                }

                // Drives
                string driveSql = @"
                    INSERT INTO Client_Machine_Disque 
                        (id_client_machine, Name, Root_Directory, Label, Type, Format, Size, Free_Space, Total_Space, Free_Space_Percentage, Is_System_Drive)
                    VALUES 
                        (@machineId, @driveName, @rootDir, @label, @type, @format, @size, @freeSpace, @totalSpace, @freeSpacePer, @isSystemDrive);

                    SELECT SCOPE_IDENTITY();";

                int driveId;

                foreach (var drive in machine.Drives)
                {
                    drive.MachineId = machineId;

                    using SqlCommand driveCmd = new(driveSql, connection);

                    driveCmd.Parameters.AddWithValue("@machineId", machineId);
                    driveCmd.Parameters.AddWithValue("@driveName", drive.Name);
                    driveCmd.Parameters.AddWithValue("@rootDir", drive.RootDirectory);
                    driveCmd.Parameters.AddWithValue("@label", drive.Label);
                    driveCmd.Parameters.AddWithValue("@type", drive.Type);
                    driveCmd.Parameters.AddWithValue("@format", drive.Format);
                    driveCmd.Parameters.AddWithValue("@size", drive.Size);
                    driveCmd.Parameters.AddWithValue("@freeSpace", drive.FreeSpace);
                    driveCmd.Parameters.AddWithValue("@totalSpace", drive.TotalSpace);
                    driveCmd.Parameters.AddWithValue("@freeSpacePer", drive.FreeSpacePercentage);
                    driveCmd.Parameters.AddWithValue("@isSystemDrive", drive.IsSystemDrive);

                    var newDriveId = await driveCmd.ExecuteScalarAsync();
                    driveId = Convert.ToInt32(newDriveId);
                    drive.Id = driveId;

                    int osId;

                    // Operating System
                    if (drive.Os != null)
                    {
                        drive.Os.DriveId = driveId;

                        string osSql = @"
                            INSERT INTO Client_Machine_Disque_Os
                                (id_client_machine_disque, Directory, Architecture, Version, Product_Name, Release_Id, Current_Build, Ubr)
                            VALUES 
                                (@driveId, @directory, @architecture, @version, @productName, @releaseId, @currentBuild, @ubr);

                            SELECT SCOPE_IDENTITY();";

                        using SqlCommand osCmd = new(osSql, connection);

                        osCmd.Parameters.AddWithValue("@driveId", driveId);
                        osCmd.Parameters.AddWithValue("@directory", drive.Os?.Directory);
                        osCmd.Parameters.AddWithValue("@architecture", drive.Os?.Architecture);
                        osCmd.Parameters.AddWithValue("@version", drive.Os?.Version);
                        osCmd.Parameters.AddWithValue("@productName", drive.Os?.ProductName);
                        osCmd.Parameters.AddWithValue("@releaseId", drive.Os?.ReleaseId);
                        osCmd.Parameters.AddWithValue("@currentBuild", drive.Os?.CurrentBuild);
                        osCmd.Parameters.AddWithValue("@ubr", drive.Os?.Ubr);

                        var newOsId = await osCmd.ExecuteScalarAsync();
                        osId = Convert.ToInt32(newOsId);
                        drive.Os.Id = osId;
                    }
                }
                await connection.CloseAsync();

                return machine;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "An error occured inserting the machine into the database.", ex);
            }

        }

        /// <summary>Gets all the machines without details (embedded models).</summary>
        /// <returns>
        ///   A <see cref="List{MachineModel}"/> of instantiated <see cref="MachineModel"/>.
        /// </returns>
        public async Task<List<MachineModel>> GetAllAsync()
        {
            List<MachineModel> machinesList = [];

            try
            {
                await using SqlConnection connection = GetConnection();
                await connection.OpenAsync();

                const string sqlRequest =
                    "SELECT * FROM Client_Machine";

                using (SqlCommand cmd = new(sqlRequest, connection))
                {

                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        machinesList.Add(new MachineModel
                        {
                            Id = Convert.ToInt32(reader["id_client_machine"]), // (reader.GetOrdinal("id_client_machine") ?
                            Name = Convert.ToString((string)reader["Name"]),
                            CustomerId = Convert.ToInt32(reader["id_client"]),
                        });
                    }
                }
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "Could not retrieve data from the database.", ex);
            }

            return machinesList;
        }

        /// <summary>Gets a machine with details (embedded Drives, OS etc.).</summary>
        /// <param name="id">The id of the machine in the database.</param>
        /// <returns>
        ///   A <see cref="MachineModel"/> instantiated from the data from the DB.
        /// </returns>
        public async Task<MachineModel> GetByIdAsync(int id)
        {
            MachineModel machine = new();
            List<DriveModel> drivesList = [];

            try
            {
                await using SqlConnection connection = GetConnection();
                await connection.OpenAsync();

                const string sqlRequest = @"
                    SELECT id_client AS Customer_Id,
                        Machine.id_client_machine AS Machine_Id,
                        Machine.Name AS Machine_Name, 
                        Drive.id_client_machine_disque AS Drive_Id, 
                        Drive.Name AS Drive_Name, 
                        Root_Directory, 
                        Label, 
                        Type, 
                        Format, 
                        Size, 
                        Free_Space, 
                        Total_Space, 
                        Free_Space_Percentage, 
                        Is_System_Drive, 
                        id_client_machine_disque_os AS Os_Id, 
                        Directory, 
                        Architecture, 
                        Version, 
                        Product_Name, 
                        Release_Id,  
                        Current_Build, 
                        Ubr 
                    FROM Client_Machine AS Machine 
                        LEFT OUTER JOIN Client_Machine_Disque AS Drive 
                    ON Machine.id_client_machine = Drive.id_client_machine 
                        LEFT OUTER JOIN Client_Machine_Disque_Os AS Os 
                    ON Os.id_client_machine_disque = Drive.id_client_machine_disque 
                    WHERE Machine.id_client_machine = @Id";

                using (SqlCommand cmd = new(sqlRequest, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        machine.Id = Convert.ToInt32(reader["Machine_Id"]);
                        machine.Name = (string)reader["Machine_Name"];
                        machine.CustomerId = Convert.ToInt32(reader["Customer_Id"]);

                        if (reader["Drive_Id"] != DBNull.Value)
                        {
                            DriveModel drive = new()
                            {
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

                            if (drive.IsSystemDrive && reader["Os_Id"] != DBNull.Value)
                            {
                                OsModel Os = new()
                                {
                                    Id = Convert.ToInt32(reader["Os_Id"]),
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
                        machine.Drives = drivesList;
                    }
                }
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could not retrieve data from the database", ex);
            }
            return machine;
        }
    }
}
