﻿using System.Data.SqlClient;
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
            try
            {
                var dtn = db.DrivesTableNames;

                string query = @$"
                    INSERT INTO {dtn.TableName} 
                        ({dtn.MachineId}, {dtn.DriveName}, {dtn.RootDirectory}, {dtn.Label}, {dtn.Type}, {dtn.Format}, {dtn.Size}, {dtn.FreeSpace}, {dtn.TotalSpace}, {dtn.FreeSpacePercentage}, {dtn.IsSystemDrive})
                    VALUES 
                        (@machineId, @driveName, @rootDir, @label, @type, @format, @size, @freeSpace, @totalSpace, @freeSpacePer, @isSystemDrive);

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@machineId", drive.MachineId);
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

                    var newDriveId = await cmd.ExecuteScalarAsync();
                    drive.Id = Convert.ToInt32(newDriveId);
                }

                return drive;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occured inserting the drive into the database.", ex);
            }
        }

        public async Task<DriveModel> UpdateAsync(DriveModel drive, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                var dtn = db.DrivesTableNames;

                string query = @$"
                    UPDATE {dtn.TableName}
                    SET 
                        {dtn.MachineId} = @machineId, 
                        {dtn.DriveName} = @driveName, 
                        {dtn.RootDirectory} = @rootDir, 
                        {dtn.Label} = @label, 
                        {dtn.Type} = @type, 
                        {dtn.Format} = @format, 
                        {dtn.Size} = @size, 
                        {dtn.FreeSpace} = @freeSpace, 
                        {dtn.TotalSpace} = @totalSpace, 
                        {dtn.FreeSpacePercentage} = @freeSpacePer, 
                        {dtn.IsSystemDrive} = @isSystemDrive
                    WHERE {dtn.Id} = @driveId";

                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@machineId", drive.MachineId);
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
                    cmd.Parameters.AddWithValue("@driveId", drive.Id);

                    await cmd.ExecuteNonQueryAsync();
                }

                return drive;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occured inserting the drive into the database.", ex);
            }
        }
    }
}
