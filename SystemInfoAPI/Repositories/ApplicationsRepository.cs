using System.Data.SqlClient;
using SystemInfoApi.Classes;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class ApplicationsRepository(Database db)
    {
        public async Task<int> InsertAsync(ApplicationModel app, SqlConnection conection, SqlTransaction transaction)
        {
            try
            {
                var appsDrivesRTable = db.AppsDrivesRelationTable;

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
                         {appsDrivesRTable.SpecialBuild})
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
                         @Special_Build);";

                using (SqlCommand cmd = new(query, conection, transaction))
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

                    return await cmd.ExecuteNonQueryAsync();
                };
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
