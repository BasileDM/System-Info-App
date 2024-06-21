using System.Data.SqlClient;
using SystemInfoApi.Models;
using SystemInfoApi.Classes;

namespace SystemInfoApi.Repositories
{
    public class MachinesRepository(Database db)
    {
        private readonly MachinesTableNames _machinesTable = db.MachinesTableNames;
        private readonly DrivesTableNames _drivesTable = db.DrivesTableNames;
        private readonly OsTableNames _osTable = db.OsTableNames;
        private readonly AppsDrivesRelationTableNames _appsDrivesRTable = db.AppsDrivesRelationTableNames;
        private readonly ApplicationsTableNames _appsTable = db.ApplicationsTableNames;

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
                    INSERT INTO {_machinesTable.TableName} ({_machinesTable.CustomerId}, {_machinesTable.MachineName}) 
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

        public async Task UpdateAsync(MachineModel machine, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                string query = @$"
                    UPDATE {_machinesTable.TableName} 
                    SET {_machinesTable.CustomerId} = @customerID, {_machinesTable.MachineName} = @machineName
                    WHERE {_machinesTable.Id} = @machineId;";

                using SqlCommand cmd = new(query, connection, transaction);
                cmd.Parameters.AddWithValue("@customerId", machine.CustomerId);
                cmd.Parameters.AddWithValue("@machineName", machine.Name);
                cmd.Parameters.AddWithValue("@machineId", machine.Id);

                await cmd.ExecuteNonQueryAsync();
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

                string query =
                    $"SELECT * FROM {_machinesTable.TableName}";

                using (SqlCommand cmd = new(query, connection))
                {

                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        machinesList.Add(new MachineModel
                        {
                            Id = Convert.ToInt32(reader[$"{_machinesTable.Id}"]), // (reader.GetOrdinal("id_client_machine") ?
                            Name = Convert.ToString((string)reader[$"{_machinesTable.MachineName}"]),
                            CustomerId = Convert.ToInt32(reader[$"{_machinesTable.CustomerId}"]),
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

            string GetQuery()
            {
                string machinesTableName = _machinesTable.TableName;
                string drivesTableName = _drivesTable.TableName;
                string osTableName = _osTable.TableName;
                string appsDrivesRTableName = _appsDrivesRTable.TableName;
                string appsTableName = _appsTable.TableName;

                 string query = @$"
                    SELECT Machine.{_machinesTable.CustomerId} AS Customer_Id,
                        Machine.{_machinesTable.Id} AS Machine_Id,
                        Machine.{_machinesTable.MachineName} AS Machine_Name, 
                        Drive.{_drivesTable.Id} AS Drive_Id, 
                        Drive.{_drivesTable.DriveName} AS Drive_Name, 
                        {_drivesTable.RootDirectory}, 
                        {_drivesTable.Label}, 
                        {_drivesTable.Type}, 
                        {_drivesTable.Format}, 
                        {_drivesTable.Size}, 
                        {_drivesTable.FreeSpace}, 
                        {_drivesTable.TotalSpace}, 
                        {_drivesTable.FreeSpacePercentage}, 
                        {_drivesTable.IsSystemDrive}, 
                        {_osTable.Id} AS Os_Id, 
                        {_osTable.Directory}, 
                        {_osTable.Architecture}, 
                        {_osTable.Version}, 
                        Os.{_osTable.ProductName} AS Os_Product_Name, 
                        {_osTable.ReleaseId}, 
                        {_osTable.CurrentBuild}, 
                        {_osTable.Ubr},
	                    App.{_appsTable.Id} AS Application_Id,
	                    App.{_appsTable.AppName} AS Application_Name,
	                    {_appsDrivesRTable.Comments},
	                    {_appsDrivesRTable.CompanyName},
	                    {_appsDrivesRTable.FileBuildPart},
	                    {_appsDrivesRTable.FileDescription},
	                    {_appsDrivesRTable.FileMajorPart},
	                    {_appsDrivesRTable.FileMinorPart},
	                    {_appsDrivesRTable.FileName},
	                    {_appsDrivesRTable.FilePrivatePart},
	                    {_appsDrivesRTable.FileVersion},
	                    {_appsDrivesRTable.InternalName},
	                    {_appsDrivesRTable.IsDebug},
	                    {_appsDrivesRTable.IsPatched},
	                    {_appsDrivesRTable.IsPreRelease},
	                    {_appsDrivesRTable.IsPrivateBuild},
	                    {_appsDrivesRTable.IsSpecialBuild},
	                    {_appsDrivesRTable.Language},
	                    {_appsDrivesRTable.Copyright},
	                    {_appsDrivesRTable.Trademarks},
	                    {_appsDrivesRTable.OriginalFilename},
	                    {_appsDrivesRTable.PrivateBuild},
	                    {_appsDrivesRTable.ProductBuildPart},
	                    {_appsDrivesRTable.ProductMajorPart},
	                    {_appsDrivesRTable.ProductMinorPart},
	                    AppRelation.{_appsDrivesRTable.ProductName} AS App_Product_Name,
	                    {_appsDrivesRTable.ProductPrivatePart},
	                    {_appsDrivesRTable.ProductVersion},
	                    {_appsDrivesRTable.SpecialBuild}
                    FROM {machinesTableName} AS Machine
                    LEFT OUTER JOIN {drivesTableName} AS Drive
                    ON Machine.{_machinesTable.Id} = Drive.{_drivesTable.MachineId}
                    LEFT OUTER JOIN {osTableName} AS Os
                    ON Os.{_osTable.DriveId} = Drive.{_drivesTable.Id}
                    LEFT OUTER JOIN {appsDrivesRTableName} AS AppRelation
                    ON AppRelation.{_appsDrivesRTable.DriveId} = Drive.{_drivesTable.Id}
                    LEFT OUTER JOIN {appsTableName} AS App
                    ON App.{_appsTable.Id} = AppRelation.{_appsDrivesRTable.AppId}
                    WHERE Machine.{_machinesTable.Id} = @Id";
                return query;
            }
            DriveModel CreateDriveFromReader(SqlDataReader reader)
            {
                return new DriveModel()
                {
                    Id = Convert.ToInt32(reader["Drive_Id"]),
                    Name = (string)reader["Drive_Name"],
                    RootDirectory = (string)reader[$"{_drivesTable.RootDirectory}"],
                    Label = (string)reader[$"{_drivesTable.Label}"],
                    Type = (string)reader[$"{_drivesTable.Type}"],
                    Format = (string)reader[$"{_drivesTable.Format}"],
                    Size = Convert.ToInt64(reader[$"{_drivesTable.Size}"]),
                    FreeSpace = Convert.ToInt64(reader[$"{_drivesTable.FreeSpace}"]),
                    TotalSpace = Convert.ToInt64(reader[$"{_drivesTable.TotalSpace}"]),
                    FreeSpacePercentage = Convert.ToInt32(reader[$"{_drivesTable.FreeSpacePercentage}"]),
                    IsSystemDrive = Convert.ToBoolean(reader[$"{_drivesTable.IsSystemDrive}"]),
                    MachineId = Convert.ToInt32(reader["Machine_Id"]),
                    AppList = []
                };
            }
            OsModel CreateOsFromReader(SqlDataReader reader)
            {
                return new OsModel()
                {
                    Id = Convert.ToInt32(reader["Os_Id"]),
                    Directory = (string)(reader[$"{_osTable.Directory}"]),
                    Architecture = (string)(reader[$"{_osTable.Architecture}"]),
                    Version = (string)(reader[$"{_osTable.Version}"]),
                    ProductName = (string)(reader["Os_Product_Name"]),
                    ReleaseId = (string)(reader[$"{_osTable.ReleaseId}"]),
                    CurrentBuild = (string)(reader[$"{_osTable.CurrentBuild}"]),
                    Ubr = (string)(reader[$"{_osTable.Ubr}"]),
                    DriveId = Convert.ToInt32(reader["Drive_Id"])
                };
            }
            ApplicationModel CreateApplicationFromReader(SqlDataReader reader)
            {
                return new ApplicationModel()
                {
                    Id = Convert.ToInt32(reader["Application_Id"]),
                    Name = (string)reader["Application_Name"],
                    Comments = (string)reader[$"{_appsDrivesRTable.Comments}"],
                    CompanyName = (string)reader[$"{_appsDrivesRTable.CompanyName}"],
                    FileBuildPart = Convert.ToInt32(reader[$"{_appsDrivesRTable.FileBuildPart}"]),
                    FileDescription = (string)reader[$"{_appsDrivesRTable.FileDescription}"],
                    FileMajorPart = Convert.ToInt32(reader[$"{_appsDrivesRTable.FileMajorPart}"]),
                    FileMinorPart = Convert.ToInt32(reader[$"{_appsDrivesRTable.FileMinorPart}"]),
                    FileName = (string)reader[$"{_appsDrivesRTable.FileName}"],
                    FilePrivatePart = Convert.ToInt32(reader[$"{_appsDrivesRTable.FilePrivatePart}"]),
                    FileVersion = (string)reader[$"{_appsDrivesRTable.FileVersion}"],
                    InternalName = (string)reader[$"{_appsDrivesRTable.InternalName}"],
                    IsDebug = Convert.ToBoolean(reader[$"{_appsDrivesRTable.IsDebug}"]),
                    IsPatched = Convert.ToBoolean(reader[$"{_appsDrivesRTable.IsPatched}"]),
                    IsPreRelease = Convert.ToBoolean(reader[$"{_appsDrivesRTable.IsPreRelease}"]),
                    IsPrivateBuild = Convert.ToBoolean(reader[$"{_appsDrivesRTable.IsPrivateBuild}"]),
                    IsSpecialBuild = Convert.ToBoolean(reader[$"{_appsDrivesRTable.IsSpecialBuild}"]),
                    Language = (string)reader[$"{_appsDrivesRTable.Language}"],
                    LegalCopyright = (string)reader[$"{_appsDrivesRTable.Copyright}"],
                    LegalTrademarks = (string)reader[$"{_appsDrivesRTable.Trademarks}"],
                    OriginalFilename = (string)reader[$"{_appsDrivesRTable.OriginalFilename}"],
                    PrivateBuild = (string)reader[$"{_appsDrivesRTable.PrivateBuild}"],
                    ProductBuildPart = Convert.ToInt32(reader[$"{_appsDrivesRTable.ProductBuildPart}"]),
                    ProductMajorPart = Convert.ToInt32(reader[$"{_appsDrivesRTable.ProductMajorPart}"]),
                    ProductMinorPart = Convert.ToInt32(reader[$"{_appsDrivesRTable.ProductMinorPart}"]),
                    ProductName = (string)reader[$"App_Product_Name"],
                    ProductPrivatePart = Convert.ToInt32(reader[$"{_appsDrivesRTable.ProductPrivatePart}"]),
                    ProductVersion = (string)reader[$"{_appsDrivesRTable.ProductVersion}"],
                    SpecialBuild = (string)reader[$"{_appsDrivesRTable.SpecialBuild}"],
                    DriveId = Convert.ToInt32(reader["Drive_Id"]),
                };
            }
        }
    }
}
