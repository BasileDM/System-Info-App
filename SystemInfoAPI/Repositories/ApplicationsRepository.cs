using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class ApplicationsRepository(Database db)
    {
        public async Task<ApplicationModel> InsertAsync(ApplicationModel app, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                app.CreationDate = DateTime.Now.ToLocalTime();
                var appsDrivesRTable = db.AppsDrivesRelationTableNames;

                string query = @$"
                    INSERT INTO {appsDrivesRTable.TableName}
                        ({appsDrivesRTable.DriveId},
                         {appsDrivesRTable.AppId},
                         {appsDrivesRTable.Comments},
                         {appsDrivesRTable.CompanyName},
                         {appsDrivesRTable.FileBuildPart},
                         {appsDrivesRTable.FileDescription},
                         {appsDrivesRTable.FileMajorPart},
                         {appsDrivesRTable.FileMinorPart},
                         {appsDrivesRTable.FileName},
                         {appsDrivesRTable.FilePrivatePart},
                         {appsDrivesRTable.FileVersion},
                         {appsDrivesRTable.InternalName},
                         {appsDrivesRTable.IsDebug},
                         {appsDrivesRTable.IsPatched},
                         {appsDrivesRTable.IsPreRelease},
                         {appsDrivesRTable.IsPrivateBuild},
                         {appsDrivesRTable.IsSpecialBuild},
                         {appsDrivesRTable.Language},
                         {appsDrivesRTable.Copyright},
                         {appsDrivesRTable.Trademarks},
                         {appsDrivesRTable.OriginalFilename},
                         {appsDrivesRTable.PrivateBuild},
                         {appsDrivesRTable.ProductBuildPart},
                         {appsDrivesRTable.ProductMajorPart},
                         {appsDrivesRTable.ProductMinorPart},
                         {appsDrivesRTable.ProductName},
                         {appsDrivesRTable.ProductPrivatePart},
                         {appsDrivesRTable.ProductVersion},
                         {appsDrivesRTable.SpecialBuild},
                         {appsDrivesRTable.AppRelationCreationDate})
                    VALUES 
                        (@id_client_machine_disque,
                         @id_client_machine_disque_app,
                         @Comments,
                         @Company_Name,
                         @File_Build_Part,
                         @File_Description,
                         @File_Major_Part,
                         @File_Minor_Part,
                         @File_Name,
                         @File_Private_Part,
                         @File_Version,
                         @Internal_Name,
                         @Is_Debug,
                         @Is_Patched,
                         @Is_Pre_Release,
                         @Is_Private_Build,
                         @Is_Special_Build,
                         @Language,
                         @Legal_Copyright,
                         @Legal_Trademarks,
                         @Original_Filename,
                         @Private_Build,
                         @Product_Build_Part,
                         @Product_Major_Part,
                         @Product_Minor_Part,
                         @Product_Name,
                         @Product_Private_Part,
                         @Product_Version,
                         @Special_Build,
                         @creationDate);";

                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@id_client_machine_disque", app.DriveId);
                    cmd.Parameters.AddWithValue("@id_client_machine_disque_app", app.Id);
                    cmd.Parameters.AddWithValue("@Comments", app.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Name", app.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@File_Build_Part", app.FileBuildPart);
                    cmd.Parameters.AddWithValue("@File_Description", app.FileDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@File_Major_Part", app.FileMajorPart);
                    cmd.Parameters.AddWithValue("@File_Minor_Part", app.FileMinorPart);
                    cmd.Parameters.AddWithValue("@File_Name", app.FileName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@File_Private_Part", app.FilePrivatePart);
                    cmd.Parameters.AddWithValue("@File_Version", app.FileVersion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Internal_Name", app.InternalName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Is_Debug", app.IsDebug);
                    cmd.Parameters.AddWithValue("@Is_Patched", app.IsPatched);
                    cmd.Parameters.AddWithValue("@Is_Pre_Release", app.IsPreRelease);
                    cmd.Parameters.AddWithValue("@Is_Private_Build", app.IsPrivateBuild);
                    cmd.Parameters.AddWithValue("@Is_Special_Build", app.IsSpecialBuild);
                    cmd.Parameters.AddWithValue("@Language", app.Language ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Legal_Copyright", app.LegalCopyright ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Legal_Trademarks", app.LegalTrademarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Original_Filename", app.OriginalFilename ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Private_Build", app.PrivateBuild ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Product_Build_Part", app.ProductBuildPart);
                    cmd.Parameters.AddWithValue("@Product_Major_Part", app.ProductMajorPart);
                    cmd.Parameters.AddWithValue("@Product_Minor_Part", app.ProductMinorPart);
                    cmd.Parameters.AddWithValue("@Product_Name", app.ProductName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Product_Private_Part", app.ProductPrivatePart);
                    cmd.Parameters.AddWithValue("@Product_Version", app.ProductVersion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Special_Build", app.SpecialBuild ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@creationDate", app.CreationDate);

                    await cmd.ExecuteScalarAsync();

                    return app;
                };
            }
            catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error number
            {
                throw new ArgumentException($"Error for app {app.Name}. App Id {app.Id} is invalid or does not exist in the database.");
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<ApplicationModel> UpdateAsync(ApplicationModel app, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                app.CreationDate = DateTime.Now.ToLocalTime();
                var appsDrivesRTable = db.AppsDrivesRelationTableNames;

                string query = @$"
                    UPDATE {appsDrivesRTable.TableName}
                    SET 
                        {appsDrivesRTable.Comments} = @Comments,
                        {appsDrivesRTable.CompanyName} = @Company_Name,
                        {appsDrivesRTable.FileBuildPart} = @File_Build_Part,
                        {appsDrivesRTable.FileDescription} = @File_Description,
                        {appsDrivesRTable.FileMajorPart} = @File_Major_Part,
                        {appsDrivesRTable.FileMinorPart} = @File_Minor_Part,
                        {appsDrivesRTable.FileName} = @File_Name,
                        {appsDrivesRTable.FilePrivatePart} = @File_Private_Part,
                        {appsDrivesRTable.FileVersion} = @File_Version,
                        {appsDrivesRTable.InternalName} = @Internal_Name,
                        {appsDrivesRTable.IsDebug} = @Is_Debug,
                        {appsDrivesRTable.IsPatched} = @Is_Patched,
                        {appsDrivesRTable.IsPreRelease} = @Is_Pre_Release,
                        {appsDrivesRTable.IsPrivateBuild} = @Is_Private_Build,
                        {appsDrivesRTable.IsSpecialBuild} = @Is_Special_Build,
                        {appsDrivesRTable.Language} = @Language,
                        {appsDrivesRTable.Copyright} = @Legal_Copyright,
                        {appsDrivesRTable.Trademarks} = @Legal_Trademarks,
                        {appsDrivesRTable.OriginalFilename} = @Original_Filename,
                        {appsDrivesRTable.PrivateBuild} = @Private_Build,
                        {appsDrivesRTable.ProductBuildPart} = @Product_Build_Part,
                        {appsDrivesRTable.ProductMajorPart} = @Product_Major_Part,
                        {appsDrivesRTable.ProductMinorPart} = @Product_Minor_Part,
                        {appsDrivesRTable.ProductName} = @Product_Name,
                        {appsDrivesRTable.ProductPrivatePart} = @Product_Private_Part,
                        {appsDrivesRTable.ProductVersion} = @Product_Version,
                        {appsDrivesRTable.SpecialBuild} = @Special_Build,
                        {appsDrivesRTable.AppRelationCreationDate} = @creationDate
                    WHERE {appsDrivesRTable.DriveId} = @id_client_machine_disque 
                    AND {appsDrivesRTable.AppId} = @id_client_machine_disque_app;";

                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@id_client_machine_disque", app.DriveId);
                    cmd.Parameters.AddWithValue("@id_client_machine_disque_app", app.Id);
                    cmd.Parameters.AddWithValue("@Comments", app.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Name", app.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@File_Build_Part", app.FileBuildPart);
                    cmd.Parameters.AddWithValue("@File_Description", app.FileDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@File_Major_Part", app.FileMajorPart);
                    cmd.Parameters.AddWithValue("@File_Minor_Part", app.FileMinorPart);
                    cmd.Parameters.AddWithValue("@File_Name", app.FileName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@File_Private_Part", app.FilePrivatePart);
                    cmd.Parameters.AddWithValue("@File_Version", app.FileVersion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Internal_Name", app.InternalName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Is_Debug", app.IsDebug);
                    cmd.Parameters.AddWithValue("@Is_Patched", app.IsPatched);
                    cmd.Parameters.AddWithValue("@Is_Pre_Release", app.IsPreRelease);
                    cmd.Parameters.AddWithValue("@Is_Private_Build", app.IsPrivateBuild);
                    cmd.Parameters.AddWithValue("@Is_Special_Build", app.IsSpecialBuild);
                    cmd.Parameters.AddWithValue("@Language", app.Language ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Legal_Copyright", app.LegalCopyright ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Legal_Trademarks", app.LegalTrademarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Original_Filename", app.OriginalFilename ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Private_Build", app.PrivateBuild ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Product_Build_Part", app.ProductBuildPart);
                    cmd.Parameters.AddWithValue("@Product_Major_Part", app.ProductMajorPart);
                    cmd.Parameters.AddWithValue("@Product_Minor_Part", app.ProductMinorPart);
                    cmd.Parameters.AddWithValue("@Product_Name", app.ProductName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Product_Private_Part", app.ProductPrivatePart);
                    cmd.Parameters.AddWithValue("@Product_Version", app.ProductVersion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Special_Build", app.SpecialBuild ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@creationDate", app.CreationDate);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected <= 0)
                    {
                        throw new ArgumentException(
                            $"Failed updating the application {app.Name} with id {app.Id} in the database. 0 rows affected.");
                    }

                    return app;
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> InsertHistoryAsync(ApplicationModel app, SqlConnection connection, SqlTransaction transaction, int driveHistoryId)
        {
            try
            {
                app.CreationDate = DateTime.Now.ToLocalTime();
                var appsDrivesRHistoryTable = db.AppsDrivesRelationHistoryTableNames;

                string query = @$"
                    INSERT INTO {appsDrivesRHistoryTable.TableName}
                        ({appsDrivesRHistoryTable.DriveId},
                         {appsDrivesRHistoryTable.AppId},
                         {appsDrivesRHistoryTable.Comments},
                         {appsDrivesRHistoryTable.CompanyName},
                         {appsDrivesRHistoryTable.FileBuildPart},
                         {appsDrivesRHistoryTable.FileDescription},
                         {appsDrivesRHistoryTable.FileMajorPart},
                         {appsDrivesRHistoryTable.FileMinorPart},
                         {appsDrivesRHistoryTable.FileName},
                         {appsDrivesRHistoryTable.FilePrivatePart},
                         {appsDrivesRHistoryTable.FileVersion},
                         {appsDrivesRHistoryTable.InternalName},
                         {appsDrivesRHistoryTable.IsDebug},
                         {appsDrivesRHistoryTable.IsPatched},
                         {appsDrivesRHistoryTable.IsPreRelease},
                         {appsDrivesRHistoryTable.IsPrivateBuild},
                         {appsDrivesRHistoryTable.IsSpecialBuild},
                         {appsDrivesRHistoryTable.Language},
                         {appsDrivesRHistoryTable.Copyright},
                         {appsDrivesRHistoryTable.Trademarks},
                         {appsDrivesRHistoryTable.OriginalFilename},
                         {appsDrivesRHistoryTable.PrivateBuild},
                         {appsDrivesRHistoryTable.ProductBuildPart},
                         {appsDrivesRHistoryTable.ProductMajorPart},
                         {appsDrivesRHistoryTable.ProductMinorPart},
                         {appsDrivesRHistoryTable.ProductName},
                         {appsDrivesRHistoryTable.ProductPrivatePart},
                         {appsDrivesRHistoryTable.ProductVersion},
                         {appsDrivesRHistoryTable.SpecialBuild},
                         {appsDrivesRHistoryTable.AppRelationCreationDate})
                    VALUES 
                        (@id_client_machine_disque,
                         @id_client_machine_disque_app,
                         @Comments,
                         @Company_Name,
                         @File_Build_Part,
                         @File_Description,
                         @File_Major_Part,
                         @File_Minor_Part,
                         @File_Name,
                         @File_Private_Part,
                         @File_Version,
                         @Internal_Name,
                         @Is_Debug,
                         @Is_Patched,
                         @Is_Pre_Release,
                         @Is_Private_Build,
                         @Is_Special_Build,
                         @Language,
                         @Legal_Copyright,
                         @Legal_Trademarks,
                         @Original_Filename,
                         @Private_Build,
                         @Product_Build_Part,
                         @Product_Major_Part,
                         @Product_Minor_Part,
                         @Product_Name,
                         @Product_Private_Part,
                         @Product_Version,
                         @Special_Build,
                         @creationDate);";

                int historyAppId;
                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@id_client_machine_disque", driveHistoryId);
                    cmd.Parameters.AddWithValue("@id_client_machine_disque_app", app.Id);
                    cmd.Parameters.AddWithValue("@Comments", app.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Name", app.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@File_Build_Part", app.FileBuildPart);
                    cmd.Parameters.AddWithValue("@File_Description", app.FileDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@File_Major_Part", app.FileMajorPart);
                    cmd.Parameters.AddWithValue("@File_Minor_Part", app.FileMinorPart);
                    cmd.Parameters.AddWithValue("@File_Name", app.FileName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@File_Private_Part", app.FilePrivatePart);
                    cmd.Parameters.AddWithValue("@File_Version", app.FileVersion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Internal_Name", app.InternalName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Is_Debug", app.IsDebug);
                    cmd.Parameters.AddWithValue("@Is_Patched", app.IsPatched);
                    cmd.Parameters.AddWithValue("@Is_Pre_Release", app.IsPreRelease);
                    cmd.Parameters.AddWithValue("@Is_Private_Build", app.IsPrivateBuild);
                    cmd.Parameters.AddWithValue("@Is_Special_Build", app.IsSpecialBuild);
                    cmd.Parameters.AddWithValue("@Language", app.Language ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Legal_Copyright", app.LegalCopyright ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Legal_Trademarks", app.LegalTrademarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Original_Filename", app.OriginalFilename ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Private_Build", app.PrivateBuild ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Product_Build_Part", app.ProductBuildPart);
                    cmd.Parameters.AddWithValue("@Product_Major_Part", app.ProductMajorPart);
                    cmd.Parameters.AddWithValue("@Product_Minor_Part", app.ProductMinorPart);
                    cmd.Parameters.AddWithValue("@Product_Name", app.ProductName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Product_Private_Part", app.ProductPrivatePart);
                    cmd.Parameters.AddWithValue("@Product_Version", app.ProductVersion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Special_Build", app.SpecialBuild ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@creationDate", app.CreationDate);

                    var obj = await cmd.ExecuteScalarAsync();
                    historyAppId = Convert.ToInt32(obj);
                };
                return historyAppId;
            }
            catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error number
            {
                throw new ArgumentException($"Error for app {app.Name}. App Id {app.Id} is invalid or does not exist in the database.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> DeleteRelationAsync(int appId, int driveId, SqlConnection connection, SqlTransaction transaction)
        {
            var appsDrivesRTable = db.AppsDrivesRelationTableNames;
            string query = @$"
                DELETE FROM {appsDrivesRTable.TableName}
                WHERE {appsDrivesRTable.AppId} = @appId
                AND {appsDrivesRTable.DriveId} = @driveId;";

            try
            {
                object? result;
                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@appId", appId);
                    cmd.Parameters.AddWithValue("@driveId", driveId);

                    result = await cmd.ExecuteScalarAsync();
                }

                Console.WriteLine($"Result of scalar cmd: {result}");// remove this and make command nonquery
                return appId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete app {appId}:" + ex);
            }
        }
    }
}
