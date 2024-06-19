using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class OsRepository(Database db)
    {
        /// <summary>Asynchronously inserts a new Operating System entry in the database.</summary>
        /// <param name="os">The <see cref="OsModel"/> to add to the DB.</param>
        /// <param name="connection">The <see cref="SqlConnection"/> to use.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> to use.</param>
        /// <returns>
        ///     The <see cref="OsModel"/> with the newly created ID from the database.
        /// </returns>
        public async Task<OsModel> InsertAsync(OsModel os, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                var otn = db.OsTableNames;

                string query = @$"
                    INSERT INTO {otn.TableName}
                        ({otn.DriveId}, {otn.Directory}, {otn.Architecture}, {otn.Version}, {otn.ProductName}, {otn.ReleaseId}, {otn.CurrentBuild}, {otn.Ubr})
                    VALUES 
                        (@driveId, @directory, @architecture, @version, @productName, @releaseId, @currentBuild, @ubr);

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new(query, connection, transaction))
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
                return os;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occured inserting the OS into the database: {ex}", ex);
            }
        }
    }
}
