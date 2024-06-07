using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class OsRepository(IConfiguration config) : Database(config)
    {
        /// <summary>Asynchronously inserts a new Operating System entry in the database.</summary>
        /// <param name="os">The <see cref="OsModel"/> to add to the DB.</param>
        /// <returns>
        ///     The <see cref="OsModel"/> with the newly created ID from the database.
        /// </returns>
        public async Task<OsModel> InsertAsync(OsModel os)
        {
            try
            {
                await using SqlConnection connection = GetConnection();
                await connection.OpenAsync();

                string sqlRequest = @"
                    INSERT INTO Client_Machine_Disque_Os
                        (id_client_machine_disque, Directory, Architecture, Version, Product_Name, Release_Id, Current_Build, Ubr)
                    VALUES 
                        (@driveId, @directory, @architecture, @version, @productName, @releaseId, @currentBuild, @ubr);

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new(sqlRequest, connection))
                {
                    cmd.Parameters.AddWithValue("@driveId", os.DriveId);
                    cmd.Parameters.AddWithValue("@directory", os.Directory);
                    cmd.Parameters.AddWithValue("@architecture", os.Architecture);
                    cmd.Parameters.AddWithValue("@version", os.Version);
                    cmd.Parameters.AddWithValue("@productName", os.ProductName);
                    cmd.Parameters.AddWithValue("@releaseId", os.ReleaseId);
                    cmd.Parameters.AddWithValue("@currentBuild", os.CurrentBuild);
                    cmd.Parameters.AddWithValue("@ubr", os.Ubr);

                    var newOsId = await cmd.ExecuteScalarAsync();
                    os.Id = Convert.ToInt32(newOsId);
                }

                await connection.CloseAsync();
                return os;

            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occured inserting the OS into the database.", ex);
            }
        }
    }
}
