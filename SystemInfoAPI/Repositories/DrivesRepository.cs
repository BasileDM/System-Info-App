using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class DrivesRepository(IConfiguration config) : Database(config)
    {
        /// <summary>Asynchronously inserts a new drive entry in the database.</summary>
        /// <param name="drive">The <see cref="DriveModel"/> to add to the DB.</param>
        /// <returns>
        ///     The <see cref="DriveModel"/> with the newly created ID from the database.
        /// </returns>
        public async Task<DriveModel> InsertAsync(DriveModel drive)
        {
            try
            {
                await using SqlConnection connection = GetConnection();
                await connection.OpenAsync();

                string sqlRequest = @"
                    INSERT INTO Client_Machine_Disque 
                        (id_client_machine, Name, Root_Directory, Label, Type, Format, Size, Free_Space, Total_Space, Free_Space_Percentage, Is_System_Drive)
                    VALUES 
                        (@machineId, @driveName, @rootDir, @label, @type, @format, @size, @freeSpace, @totalSpace, @freeSpacePer, @isSystemDrive);

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new(sqlRequest, connection))
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

                await connection.CloseAsync();
                return drive;

            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occured inserting the drive into the database.", ex);
            }
        }
    }
}
