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
                        ({otn.DriveId}, {otn.Directory}, {otn.Architecture}, {otn.Version}, {otn.ProductName}, {otn.ReleaseId}, {otn.CurrentBuild}, {otn.Ubr}, {otn.OsCreationDate})
                    VALUES 
                        (@driveId, @directory, @architecture, @version, @productName, @releaseId, @currentBuild, @ubr, @creationDate);

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
                    cmd.Parameters.AddWithValue("@creationDate", os.CreationDate);

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
        public async Task<OsModel> UpdateAsync(OsModel os, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                var ohtn = db.OsHistoryTableNames;
                var otn = db.OsTableNames;

                string query = @$"
                    UPDATE {otn.TableName}
                    SET
                        {otn.DriveId} = @driveId, 
                        {otn.Directory} = @directory, 
                        {otn.Architecture} = @architecture, 
                        {otn.Version} = @version, 
                        {otn.ProductName} = @productName, 
                        {otn.ReleaseId} = @releaseId, 
                        {otn.CurrentBuild} = @currentBuild, 
                        {otn.Ubr} = @ubr,
                        {otn.OsCreationDate} = @creationDate
                    WHERE {otn.DriveId} = @driveId

                    SELECT {otn.Id} 
                    FROM {otn.TableName}
                    WHERE {otn.DriveId} = @driveId";

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
                    cmd.Parameters.AddWithValue("@creationDate", os.CreationDate);

                    var obj = await cmd.ExecuteScalarAsync() ??
                        throw new ArgumentException("OS not found.");

                    os.Id = Convert.ToInt32(obj);
                }

                return os;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occured updating OS {os.Id} for drive {os.DriveId}: {ex}", ex);
            }
        }
        public async Task<int> InsertHistoryAsync(OsModel os, SqlConnection connection, SqlTransaction transaction, int historyDriveId)
        {
            try
            {
                var ohtn = db.OsHistoryTableNames;

                string query = @$"
                    INSERT INTO {ohtn.TableName}
                        ({ohtn.DriveId}, {ohtn.Directory}, {ohtn.Architecture}, {ohtn.Version}, {ohtn.ProductName}, {ohtn.ReleaseId}, {ohtn.CurrentBuild}, {ohtn.Ubr}, {ohtn.OsCreationDate})
                    VALUES 
                        (@driveId, @directory, @architecture, @version, @productName, @releaseId, @currentBuild, @ubr, @creationDate);

                    SELECT SCOPE_IDENTITY();";

                int historyOsId;
                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@driveId", historyDriveId);
                    cmd.Parameters.AddWithValue("@directory", os.Directory);
                    cmd.Parameters.AddWithValue("@architecture", os.Architecture);
                    cmd.Parameters.AddWithValue("@version", os.Version);
                    cmd.Parameters.AddWithValue("@productName", os.ProductName);
                    cmd.Parameters.AddWithValue("@releaseId", os.ReleaseId);
                    cmd.Parameters.AddWithValue("@currentBuild", os.CurrentBuild);
                    cmd.Parameters.AddWithValue("@ubr", os.Ubr);
                    cmd.Parameters.AddWithValue("@creationDate", os.CreationDate);

                    var obj = await cmd.ExecuteScalarAsync();
                    historyOsId = Convert.ToInt32(obj);
                }
                return historyOsId;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occured inserting the OS history into the database: {ex}", ex);
            }
        }
    }
}
