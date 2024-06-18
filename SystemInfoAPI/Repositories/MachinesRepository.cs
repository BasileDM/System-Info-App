using System.Data.SqlClient;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class MachinesRepository
    {
        /// <summary>Asynchronously inserts a new machine entry in the database.</summary>
        /// <param name="machine">The <see cref="MachineModel"/> to add to the DB.</param>
        /// <param name="connection">The <see cref="SqlConnection"/> to use.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> to use.</param>
        /// <returns>
        ///     The <see cref="MachineModel"/> with the newly created ID from the database.
        /// </returns>
        public async Task<MachineModel> InsertAsync(MachineModel machine, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                string machineSql = @$"
                    INSERT INTO Client_Machine (id_client, Name) 
                    VALUES (@customerId, @machineName);

                    SELECT SCOPE_IDENTITY();";

                using SqlCommand cmd = new(machineSql, connection, transaction);
                cmd.Parameters.AddWithValue("@customerId", machine.CustomerId);
                cmd.Parameters.AddWithValue("@machineName", machine.Name);

                var newMachineId = await cmd.ExecuteScalarAsync();

                machine.Id = Convert.ToInt32(newMachineId);
                return machine;
            }
            catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error number
            {
                throw new ArgumentException("The provided customer ID is invalid or does not exist in the database.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occured inserting the machine into the database : {ex}", ex);
            }
        }

        /// <summary>Gets all the machines without details (embedded models).</summary>
        /// <returns>
        ///   A <see cref="List{MachineModel}"/> of instantiated <see cref="MachineModel"/>.
        /// </returns>
        public async Task<List<MachineModel>> GetAllAsync(SqlConnection connection)
        {
            List<MachineModel> machinesList = [];

            try
            {
                await connection.OpenAsync();

                const string query =
                    "SELECT * FROM Client_Machine";

                using (SqlCommand cmd = new(query, connection))
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
        public async Task<MachineModel> GetByIdAsync(int id, SqlConnection connection)
        {
            try
            {
                MachineModel machine = new();
                List<DriveModel> drivesList = [];
                string query = GetQuery();

                await connection.OpenAsync();
                using (SqlCommand cmd = new(query, connection))
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
                            // If the current drive from reader is in the list, 'drive' will NOT be null here
                            DriveModel? drive = drivesList.LastOrDefault(d => d.Id == Convert.ToInt32(reader["Drive_Id"]));

                            // So only if drive is null, create a new drive
                            if (drive == null)
                            {
                                drive = CreateDriveFromReader(reader);

                                if (drive.IsSystemDrive && reader["Os_Id"] != DBNull.Value)
                                {
                                    OsModel os = CreateOsFromReader(reader);
                                    drive.Os = os;
                                } 

                                // Add drive to the list of drives
                                drivesList.Add(drive);
                            }

                            // Add application to the drive's list of apps
                            if (reader["Application_Id"] != DBNull.Value)
                            {
                                ApplicationModel application = CreateApplicationFromReader(reader);
                                drive.AppList.Add(application);
                            }
                        }
                        // Add the list of drives to the machine
                        machine.Drives = drivesList;
                    }
                }
                return machine;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Could not retrieve data from the database: {ex.Message}. FULL EXCEPTION: {ex}", ex);
            }
            finally
            {
                await connection.CloseAsync();
            }

            static string GetQuery()
            {
                 const string query = @"
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
                        Os.Product_Name AS Os_Product_Name, 
                        Release_Id,  
                        Current_Build, 
                        Ubr,
	                    App.id_client_machine_disque_app AS Application_Id,
	                    App.Name AS Application_Name,
	                    Comments,
	                    Company_Name,
	                    File_Build_Part,
	                    File_Description,
	                    File_Major_Part,
	                    File_Minor_Part,
	                    File_Name,
	                    File_Private_Part,
	                    File_Version,
	                    Internal_Name,
	                    Is_Debug,
	                    Is_Patched,
	                    Is_Pre_Release,
	                    Is_Private_Build,
	                    Is_Special_Build,
	                    Language,
	                    Legal_Copyright,
	                    Legal_Trademarks,
	                    Original_Filename,
	                    Private_Build,
	                    Product_Build_Part,
	                    Product_Major_Part,
	                    Product_Minor_Part,
	                    AppRelation.Product_Name AS App_Product_Name,
	                    Product_Private_Part,
	                    Product_Version,
	                    Special_Build
                    FROM Client_Machine AS Machine
                    LEFT OUTER JOIN Client_Machine_Disque AS Drive
                    ON Machine.id_client_machine = Drive.id_client_machine
                    LEFT OUTER JOIN Client_Machine_Disque_Os AS Os
                    ON Os.id_client_machine_disque = Drive.id_client_machine_disque
                    LEFT OUTER JOIN Relation_Disque_Application AS AppRelation
                    ON AppRelation.id_client_machine_disque = Drive.id_client_machine_disque
                    LEFT OUTER JOIN Client_Machine_Disque_Application AS App
                    ON App.id_client_machine_disque_app = AppRelation.id_client_machine_disque_app
                    WHERE Machine.id_client_machine = @Id";
                return query;
            }
            static DriveModel CreateDriveFromReader(SqlDataReader reader)
            {
                return new DriveModel()
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
                    AppList = []
                };
            }
            static OsModel CreateOsFromReader(SqlDataReader reader)
            {
                return new OsModel()
                {
                    Id = Convert.ToInt32(reader["Os_Id"]),
                    Directory = (string)(reader["Directory"]),
                    Architecture = (string)(reader["Architecture"]),
                    Version = (string)(reader["Version"]),
                    ProductName = (string)(reader["Os_Product_Name"]),
                    ReleaseId = (string)(reader["Release_Id"]),
                    CurrentBuild = (string)(reader["Current_Build"]),
                    Ubr = (string)(reader["Ubr"]),
                    DriveId = Convert.ToInt32(reader["Drive_Id"])
                };
            }
            static ApplicationModel CreateApplicationFromReader(SqlDataReader reader)
            {
                return new ApplicationModel()
                {
                    Id = Convert.ToInt32(reader["Application_Id"]),
                    Name = (string)reader["Application_Name"],
                    Comments = (string)reader["Comments"],
                    CompanyName = (string)reader["Company_Name"],
                    FileBuildPart = Convert.ToInt32(reader["File_Build_Part"]),
                    FileDescription = (string)reader["File_Description"],
                    FileMajorPart = Convert.ToInt32(reader["File_Major_Part"]),
                    FileMinorPart = Convert.ToInt32(reader["File_Minor_Part"]),
                    FileName = (string)reader["File_Name"],
                    FilePrivatePart = Convert.ToInt32(reader["File_Private_Part"]),
                    FileVersion = (string)reader["File_Version"],
                    InternalName = (string)reader["Internal_Name"],
                    IsDebug = Convert.ToBoolean(reader["Is_Debug"]),
                    IsPatched = Convert.ToBoolean(reader["Is_Patched"]),
                    IsPreRelease = Convert.ToBoolean(reader["Is_Pre_Release"]),
                    IsPrivateBuild = Convert.ToBoolean(reader["Is_Private_Build"]),
                    IsSpecialBuild = Convert.ToBoolean(reader["Is_Special_Build"]),
                    Language = (string)reader["Language"],
                    LegalCopyright = (string)reader["Legal_Copyright"],
                    LegalTrademarks = (string)reader["Legal_Trademarks"],
                    OriginalFilename = (string)reader["Original_Filename"],
                    PrivateBuild = (string)reader["Private_Build"],
                    ProductBuildPart = Convert.ToInt32(reader["Product_Build_Part"]),
                    ProductMajorPart = Convert.ToInt32(reader["Product_Major_Part"]),
                    ProductMinorPart = Convert.ToInt32(reader["Product_Minor_Part"]),
                    ProductName = (string)reader["App_Product_Name"],
                    ProductPrivatePart = Convert.ToInt32(reader["Product_Private_Part"]),
                    ProductVersion = (string)reader["Product_Version"],
                    SpecialBuild = (string)reader["Special_Build"],
                    DriveId = Convert.ToInt32(reader["Drive_Id"]),
                };
            }
        }
    }
}
