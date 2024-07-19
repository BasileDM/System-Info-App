using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class DrivesRepository(Database db)
    {
        /// <summary>Asynchronously inserts a new drive entry in the database.</summary>
        /// <param name="drive">The <see cref="DriveModel"/> to add to the DB.</param>
        /// <param name="connection">The <see cref="SqlConnection"/> to use.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> to use.</param>
        /// <returns>
        ///     The <see cref="DriveModel"/> with the newly created ID from the database.
        /// </returns>
        public async Task<DriveModel> InsertAsync(DriveModel drive, SqlConnection connection, SqlTransaction transaction)
        {
            var dtn = db.DrivesTableNames;

            string query = @$"                    
                INSERT INTO {dtn.TableName} 
                    ({dtn.MachineId}, {dtn.SerialNumber}, {dtn.DriveName}, {dtn.RootDirectory}, {dtn.Label}, {dtn.Type}, {dtn.Format}, {dtn.Size}, {dtn.FreeSpace}, {dtn.TotalSpace}, {dtn.FreeSpacePercentage}, {dtn.IsSystemDrive}, {dtn.DriveCreationDate})
                VALUES 
                    (@machineId, @serial, @driveName, @rootDir, @label, @type, @format, @size, @freeSpace, @totalSpace, @freeSpacePer, @isSystemDrive, @creationDate);

                SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@machineId", drive.MachineId);
                    cmd.Parameters.AddWithValue("@serial", drive.SerialNumber);
                    cmd.Parameters.AddWithValue("@driveName", drive.Name);
                    cmd.Parameters.AddWithValue("@rootDir", drive.RootDirectory);
                    cmd.Parameters.AddWithValue("@label", drive.Label);
                    cmd.Parameters.AddWithValue("@type", drive.Type);
                    cmd.Parameters.AddWithValue("@format", drive.Format);
                    cmd.Parameters.AddWithValue("@size", drive.Size);
                    cmd.Parameters.AddWithValue("@freeSpace", drive.FreeSpace);
                    cmd.Parameters.AddWithValue("@totalSpace", drive.TotalSpace);
                    cmd.Parameters.AddWithValue("@freeSpacePer", drive.FreeSpacePercentage);
                    cmd.Parameters.AddWithValue("@isSystemDrive", drive.IsSystemDrive);
                    cmd.Parameters.AddWithValue("@creationDate", drive.CreationDate);

                    var newDriveId = await cmd.ExecuteScalarAsync();
                    drive.Id = Convert.ToInt32(newDriveId);
                }

                return drive;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occured inserting the drive into the database: {ex.Message}", ex);
            }
        }
        public async Task<DriveModel> UpdateAsync(DriveModel drive, SqlConnection connection, SqlTransaction transaction)
        {
            var dtn = db.DrivesTableNames;

            string query = @$"
                UPDATE {dtn.TableName}
                SET 
                    {dtn.MachineId} = @machineId, 
                    {dtn.SerialNumber} = @serial,
                    {dtn.DriveName} = @driveName, 
                    {dtn.RootDirectory} = @rootDir, 
                    {dtn.Label} = @label, 
                    {dtn.Type} = @type, 
                    {dtn.Format} = @format, 
                    {dtn.Size} = @size, 
                    {dtn.FreeSpace} = @freeSpace, 
                    {dtn.TotalSpace} = @totalSpace, 
                    {dtn.FreeSpacePercentage} = @freeSpacePer, 
                    {dtn.IsSystemDrive} = @isSystemDrive,
                    {dtn.DriveCreationDate} = @creationDate
                WHERE {dtn.MachineId} = @machineId
                AND {dtn.SerialNumber} = @serial

                SELECT {dtn.Id}
                FROM {dtn.TableName}
                WHERE {dtn.MachineId} = @machineId 
                AND {dtn.SerialNumber} = @serial";

            try
            {
                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@machineId", drive.MachineId);
                    cmd.Parameters.AddWithValue("@serial", drive.SerialNumber);
                    cmd.Parameters.AddWithValue("@driveName", drive.Name);
                    cmd.Parameters.AddWithValue("@rootDir", drive.RootDirectory ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@label", drive.Label ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@type", drive.Type ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@format", drive.Format ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@size", drive.Size);
                    cmd.Parameters.AddWithValue("@freeSpace", drive.FreeSpace);
                    cmd.Parameters.AddWithValue("@totalSpace", drive.TotalSpace);
                    cmd.Parameters.AddWithValue("@freeSpacePer", drive.FreeSpacePercentage);
                    cmd.Parameters.AddWithValue("@isSystemDrive", drive.IsSystemDrive);
                    cmd.Parameters.AddWithValue("@creationDate", drive.CreationDate);

                    var obj = await cmd.ExecuteScalarAsync() ?? throw new ArgumentException("Drive not found.");

                    drive.Id = Convert.ToInt32(obj);
                }

                return drive;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occured updating the drive in the database: {ex.Message}", ex);
            }
        }
        public async Task<int> InsertHistoryAsync(DriveModel drive, SqlConnection connection, SqlTransaction transaction)
        {
            var dhtn = db.DrivesHistoryTableNames;

            string query = @$"
            INSERT INTO {dhtn.TableName}
                ({dhtn.MachineId}, {dhtn.SerialNumber}, {dhtn.DriveName}, {dhtn.RootDirectory}, {dhtn.Label}, {dhtn.Type}, {dhtn.Format}, {dhtn.Size}, {dhtn.FreeSpace}, {dhtn.TotalSpace}, {dhtn.FreeSpacePercentage}, {dhtn.IsSystemDrive}, {dhtn.DriveCreationDate})
            VALUES
                (@machineId, @serial, @driveName, @rootDir, @label, @type, @format, @size, @freeSpace, @totalSpace, @freeSpacePer, @isSystemDrive, @creationDate);
            SELECT SCOPE_IDENTITY();";

            try
            {
                int newId;
                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@machineId", drive.MachineId);
                    cmd.Parameters.AddWithValue("@serial", drive.SerialNumber);
                    cmd.Parameters.AddWithValue("@driveName", drive.Name);
                    cmd.Parameters.AddWithValue("@rootDir", drive.RootDirectory);
                    cmd.Parameters.AddWithValue("@label", drive.Label);
                    cmd.Parameters.AddWithValue("@type", drive.Type);
                    cmd.Parameters.AddWithValue("@format", drive.Format);
                    cmd.Parameters.AddWithValue("@size", drive.Size);
                    cmd.Parameters.AddWithValue("@freeSpace", drive.FreeSpace);
                    cmd.Parameters.AddWithValue("@totalSpace", drive.TotalSpace);
                    cmd.Parameters.AddWithValue("@freeSpacePer", drive.FreeSpacePercentage);
                    cmd.Parameters.AddWithValue("@isSystemDrive", drive.IsSystemDrive);
                    cmd.Parameters.AddWithValue("@creationDate", drive.CreationDate);

                    var obj = await cmd.ExecuteScalarAsync();
                    newId = Convert.ToInt32(obj);
                };

                return newId;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occured inserting the drive history into the database.", ex);
            }
        }
        public async Task<int> DeleteAsync(int driveId, SqlConnection connection, SqlTransaction transaction)
        {
            var dt = db.DrivesTableNames;
            string query = @$"
                DELETE FROM {dt.TableName}
                WHERE {dt.Id} = @driveId;";

            try
            {
                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@driveId", driveId);
                    await cmd.ExecuteNonQueryAsync();
                }

                return driveId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete drive {driveId}:" + ex);
            }
        }
    }
}
