using System.Data.SqlClient;
using System.Text;
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
        public async Task InsertListAsync(List<ApplicationModel> appsList, SqlConnection connection, SqlTransaction transaction)
        {
            var appsDrivesRTable = db.AppsDrivesRelationTableNames;
            var queryBuilder = new StringBuilder();
            queryBuilder.Append($@"
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
                     {appsDrivesRTable.AppRelationCreationDate}) VALUES ");

            var parameterIndex = 0;
            var parameterValues = new List<SqlParameter>();

            foreach (var app in appsList)
            {
                if (parameterIndex > 0)
                {
                    queryBuilder.Append(", ");
                }

                queryBuilder.Append($@"(@DriveId{parameterIndex}, @AppId{parameterIndex}, @Comments{parameterIndex}, 
                    @CompanyName{parameterIndex}, @FileBuildPart{parameterIndex}, @FileDescription{parameterIndex}, 
                    @FileMajorPart{parameterIndex}, @FileMinorPart{parameterIndex}, @FileName{parameterIndex}, 
                    @FilePrivatePart{parameterIndex}, @FileVersion{parameterIndex}, @InternalName{parameterIndex}, 
                    @IsDebug{parameterIndex}, @IsPatched{parameterIndex}, @IsPreRelease{parameterIndex}, 
                    @IsPrivateBuild{parameterIndex}, @IsSpecialBuild{parameterIndex}, @Language{parameterIndex}, 
                    @LegalCopyright{parameterIndex}, @LegalTrademarks{parameterIndex}, @OriginalFilename{parameterIndex}, 
                    @PrivateBuild{parameterIndex}, @ProductBuildPart{parameterIndex}, @ProductMajorPart{parameterIndex}, 
                    @ProductMinorPart{parameterIndex}, @ProductName{parameterIndex}, @ProductPrivatePart{parameterIndex}, 
                    @ProductVersion{parameterIndex}, @SpecialBuild{parameterIndex}, @CreationDate{parameterIndex})");

                parameterValues.AddRange(new[]
                {
                    new SqlParameter($"@DriveId{parameterIndex}", app.DriveId),
                    new SqlParameter($"@AppId{parameterIndex}", app.Id),
                    new SqlParameter($"@Comments{parameterIndex}", app.Comments ?? (object)DBNull.Value),
                    new SqlParameter($"@CompanyName{parameterIndex}", app.CompanyName ?? (object)DBNull.Value),
                    new SqlParameter($"@FileBuildPart{parameterIndex}", app.FileBuildPart),
                    new SqlParameter($"@FileDescription{parameterIndex}", app.FileDescription ?? (object)DBNull.Value),
                    new SqlParameter($"@FileMajorPart{parameterIndex}", app.FileMajorPart),
                    new SqlParameter($"@FileMinorPart{parameterIndex}", app.FileMinorPart),
                    new SqlParameter($"@FileName{parameterIndex}", app.FileName ?? (object)DBNull.Value),
                    new SqlParameter($"@FilePrivatePart{parameterIndex}", app.FilePrivatePart),
                    new SqlParameter($"@FileVersion{parameterIndex}", app.FileVersion ?? (object)DBNull.Value),
                    new SqlParameter($"@InternalName{parameterIndex}", app.InternalName ?? (object)DBNull.Value),
                    new SqlParameter($"@IsDebug{parameterIndex}", app.IsDebug),
                    new SqlParameter($"@IsPatched{parameterIndex}", app.IsPatched),
                    new SqlParameter($"@IsPreRelease{parameterIndex}", app.IsPreRelease),
                    new SqlParameter($"@IsPrivateBuild{parameterIndex}", app.IsPrivateBuild),
                    new SqlParameter($"@IsSpecialBuild{parameterIndex}", app.IsSpecialBuild),
                    new SqlParameter($"@Language{parameterIndex}", app.Language ?? (object)DBNull.Value),
                    new SqlParameter($"@LegalCopyright{parameterIndex}", app.LegalCopyright ?? (object)DBNull.Value),
                    new SqlParameter($"@LegalTrademarks{parameterIndex}", app.LegalTrademarks ?? (object)DBNull.Value),
                    new SqlParameter($"@OriginalFilename{parameterIndex}", app.OriginalFilename ?? (object)DBNull.Value),
                    new SqlParameter($"@PrivateBuild{parameterIndex}", app.PrivateBuild ?? (object)DBNull.Value),
                    new SqlParameter($"@ProductBuildPart{parameterIndex}", app.ProductBuildPart),
                    new SqlParameter($"@ProductMajorPart{parameterIndex}", app.ProductMajorPart),
                    new SqlParameter($"@ProductMinorPart{parameterIndex}", app.ProductMinorPart),
                    new SqlParameter($"@ProductName{parameterIndex}", app.ProductName ?? (object)DBNull.Value),
                    new SqlParameter($"@ProductPrivatePart{parameterIndex}", app.ProductPrivatePart),
                    new SqlParameter($"@ProductVersion{parameterIndex}", app.ProductVersion ?? (object)DBNull.Value),
                    new SqlParameter($"@SpecialBuild{parameterIndex}", app.SpecialBuild ?? (object)DBNull.Value),
                    new SqlParameter($"@CreationDate{parameterIndex}", app.CreationDate)
                });

                parameterIndex++;
            }

            queryBuilder.Append(';');

            using SqlCommand cmd = new(queryBuilder.ToString(), connection, transaction);
            cmd.Parameters.AddRange(parameterValues.ToArray());

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<ApplicationModel> UpdateAsync(ApplicationModel app, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
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
        public async Task UpdateListAsync(List<ApplicationModel> appsList, SqlConnection connection, SqlTransaction transaction)
        {
            var appsDrivesRTable = db.AppsDrivesRelationTableNames;
            var queryBuilder = new StringBuilder();

            var parameterValues = new List<SqlParameter>();
            var parameterIndex = 0;

            foreach (var app in appsList)
            {
                queryBuilder.Append($@"
                    UPDATE {appsDrivesRTable.TableName}
                    SET 
                        {appsDrivesRTable.Comments} = @Comments{parameterIndex},
                        {appsDrivesRTable.CompanyName} = @Company_Name{parameterIndex},
                        {appsDrivesRTable.FileBuildPart} = @File_Build_Part{parameterIndex},
                        {appsDrivesRTable.FileDescription} = @File_Description{parameterIndex},
                        {appsDrivesRTable.FileMajorPart} = @File_Major_Part{parameterIndex},
                        {appsDrivesRTable.FileMinorPart} = @File_Minor_Part{parameterIndex},
                        {appsDrivesRTable.FileName} = @File_Name{parameterIndex},
                        {appsDrivesRTable.FilePrivatePart} = @File_Private_Part{parameterIndex},
                        {appsDrivesRTable.FileVersion} = @File_Version{parameterIndex},
                        {appsDrivesRTable.InternalName} = @Internal_Name{parameterIndex},
                        {appsDrivesRTable.IsDebug} = @Is_Debug{parameterIndex},
                        {appsDrivesRTable.IsPatched} = @Is_Patched{parameterIndex},
                        {appsDrivesRTable.IsPreRelease} = @Is_Pre_Release{parameterIndex},
                        {appsDrivesRTable.IsPrivateBuild} = @Is_Private_Build{parameterIndex},
                        {appsDrivesRTable.IsSpecialBuild} = @Is_Special_Build{parameterIndex},
                        {appsDrivesRTable.Language} = @Language{parameterIndex},
                        {appsDrivesRTable.Copyright} = @Legal_Copyright{parameterIndex},
                        {appsDrivesRTable.Trademarks} = @Legal_Trademarks{parameterIndex},
                        {appsDrivesRTable.OriginalFilename} = @Original_Filename{parameterIndex},
                        {appsDrivesRTable.PrivateBuild} = @Private_Build{parameterIndex},
                        {appsDrivesRTable.ProductBuildPart} = @Product_Build_Part{parameterIndex},
                        {appsDrivesRTable.ProductMajorPart} = @Product_Major_Part{parameterIndex},
                        {appsDrivesRTable.ProductMinorPart} = @Product_Minor_Part{parameterIndex},
                        {appsDrivesRTable.ProductName} = @Product_Name{parameterIndex},
                        {appsDrivesRTable.ProductPrivatePart} = @Product_Private_Part{parameterIndex},
                        {appsDrivesRTable.ProductVersion} = @Product_Version{parameterIndex},
                        {appsDrivesRTable.SpecialBuild} = @Special_Build{parameterIndex},
                        {appsDrivesRTable.AppRelationCreationDate} = @CreationDate{parameterIndex}
                    WHERE {appsDrivesRTable.DriveId} = @DriveId{parameterIndex}
                    AND {appsDrivesRTable.AppId} = @AppId{parameterIndex};
                ");

                parameterValues.AddRange(new[]
                {
                    new SqlParameter($"@DriveId{parameterIndex}", app.DriveId),
                    new SqlParameter($"@AppId{parameterIndex}", app.Id),
                    new SqlParameter($"@Comments{parameterIndex}", app.Comments ?? (object)DBNull.Value),
                    new SqlParameter($"@Company_Name{parameterIndex}", app.CompanyName ?? (object)DBNull.Value),
                    new SqlParameter($"@File_Build_Part{parameterIndex}", app.FileBuildPart),
                    new SqlParameter($"@File_Description{parameterIndex}", app.FileDescription ?? (object)DBNull.Value),
                    new SqlParameter($"@File_Major_Part{parameterIndex}", app.FileMajorPart),
                    new SqlParameter($"@File_Minor_Part{parameterIndex}", app.FileMinorPart),
                    new SqlParameter($"@File_Name{parameterIndex}", app.FileName ?? (object)DBNull.Value),
                    new SqlParameter($"@File_Private_Part{parameterIndex}", app.FilePrivatePart),
                    new SqlParameter($"@File_Version{parameterIndex}", app.FileVersion ?? (object)DBNull.Value),
                    new SqlParameter($"@Internal_Name{parameterIndex}", app.InternalName ?? (object)DBNull.Value),
                    new SqlParameter($"@Is_Debug{parameterIndex}", app.IsDebug),
                    new SqlParameter($"@Is_Patched{parameterIndex}", app.IsPatched),
                    new SqlParameter($"@Is_Pre_Release{parameterIndex}", app.IsPreRelease),
                    new SqlParameter($"@Is_Private_Build{parameterIndex}", app.IsPrivateBuild),
                    new SqlParameter($"@Is_Special_Build{parameterIndex}", app.IsSpecialBuild),
                    new SqlParameter($"@Language{parameterIndex}", app.Language ?? (object)DBNull.Value),
                    new SqlParameter($"@Legal_Copyright{parameterIndex}", app.LegalCopyright ?? (object)DBNull.Value),
                    new SqlParameter($"@Legal_Trademarks{parameterIndex}", app.LegalTrademarks ?? (object)DBNull.Value),
                    new SqlParameter($"@Original_Filename{parameterIndex}", app.OriginalFilename ?? (object)DBNull.Value),
                    new SqlParameter($"@Private_Build{parameterIndex}", app.PrivateBuild ?? (object)DBNull.Value),
                    new SqlParameter($"@Product_Build_Part{parameterIndex}", app.ProductBuildPart),
                    new SqlParameter($"@Product_Major_Part{parameterIndex}", app.ProductMajorPart),
                    new SqlParameter($"@Product_Minor_Part{parameterIndex}", app.ProductMinorPart),
                    new SqlParameter($"@Product_Name{parameterIndex}", app.ProductName ?? (object)DBNull.Value),
                    new SqlParameter($"@Product_Private_Part{parameterIndex}", app.ProductPrivatePart),
                    new SqlParameter($"@Product_Version{parameterIndex}", app.ProductVersion ?? (object)DBNull.Value),
                    new SqlParameter($"@Special_Build{parameterIndex}", app.SpecialBuild ?? (object)DBNull.Value),
                    new SqlParameter($"@CreationDate{parameterIndex}", app.CreationDate)
                });

                parameterIndex++;
            }

            using SqlCommand cmd = new(queryBuilder.ToString(), connection, transaction);
            cmd.Parameters.AddRange(parameterValues.ToArray());

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> InsertHistoryAsync(ApplicationModel app, SqlConnection connection, SqlTransaction transaction, int driveHistoryId)
        {
            try
            {
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
        public async Task InsertHistoryListAsync(List<ApplicationModel> appsList, SqlConnection connection, SqlTransaction transaction, int driveHistoryId)
        {
            try
            {
                var appsDrivesRHistoryTable = db.AppsDrivesRelationHistoryTableNames;
                var queryBuilder = new StringBuilder();
                queryBuilder.Append($@"
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
                         {appsDrivesRHistoryTable.AppRelationCreationDate}) VALUES ");

                var parameterIndex = 0;
                var parameterValues = new List<SqlParameter>();

                foreach (var app in appsList)
                {
                    if (parameterIndex > 0)
                    {
                        queryBuilder.Append(", ");
                    }

                    queryBuilder.Append($@"(@DriveId{parameterIndex}, @AppId{parameterIndex}, @Comments{parameterIndex}, 
                        @CompanyName{parameterIndex}, @FileBuildPart{parameterIndex}, @FileDescription{parameterIndex}, 
                        @FileMajorPart{parameterIndex}, @FileMinorPart{parameterIndex}, @FileName{parameterIndex}, 
                        @FilePrivatePart{parameterIndex}, @FileVersion{parameterIndex}, @InternalName{parameterIndex}, 
                        @IsDebug{parameterIndex}, @IsPatched{parameterIndex}, @IsPreRelease{parameterIndex}, 
                        @IsPrivateBuild{parameterIndex}, @IsSpecialBuild{parameterIndex}, @Language{parameterIndex}, 
                        @LegalCopyright{parameterIndex}, @LegalTrademarks{parameterIndex}, @OriginalFilename{parameterIndex}, 
                        @PrivateBuild{parameterIndex}, @ProductBuildPart{parameterIndex}, @ProductMajorPart{parameterIndex}, 
                        @ProductMinorPart{parameterIndex}, @ProductName{parameterIndex}, @ProductPrivatePart{parameterIndex}, 
                        @ProductVersion{parameterIndex}, @SpecialBuild{parameterIndex}, @CreationDate{parameterIndex})");

                    parameterValues.AddRange(new[]
                    {
                        new SqlParameter($"@DriveId{parameterIndex}", driveHistoryId),
                        new SqlParameter($"@AppId{parameterIndex}", app.Id),
                        new SqlParameter($"@Comments{parameterIndex}", app.Comments ?? (object)DBNull.Value),
                        new SqlParameter($"@CompanyName{parameterIndex}", app.CompanyName ?? (object)DBNull.Value),
                        new SqlParameter($"@FileBuildPart{parameterIndex}", app.FileBuildPart),
                        new SqlParameter($"@FileDescription{parameterIndex}", app.FileDescription ?? (object)DBNull.Value),
                        new SqlParameter($"@FileMajorPart{parameterIndex}", app.FileMajorPart),
                        new SqlParameter($"@FileMinorPart{parameterIndex}", app.FileMinorPart),
                        new SqlParameter($"@FileName{parameterIndex}", app.FileName ?? (object)DBNull.Value),
                        new SqlParameter($"@FilePrivatePart{parameterIndex}", app.FilePrivatePart),
                        new SqlParameter($"@FileVersion{parameterIndex}", app.FileVersion ?? (object)DBNull.Value),
                        new SqlParameter($"@InternalName{parameterIndex}", app.InternalName ?? (object)DBNull.Value),
                        new SqlParameter($"@IsDebug{parameterIndex}", app.IsDebug),
                        new SqlParameter($"@IsPatched{parameterIndex}", app.IsPatched),
                        new SqlParameter($"@IsPreRelease{parameterIndex}", app.IsPreRelease),
                        new SqlParameter($"@IsPrivateBuild{parameterIndex}", app.IsPrivateBuild),
                        new SqlParameter($"@IsSpecialBuild{parameterIndex}", app.IsSpecialBuild),
                        new SqlParameter($"@Language{parameterIndex}", app.Language ?? (object)DBNull.Value),
                        new SqlParameter($"@LegalCopyright{parameterIndex}", app.LegalCopyright ?? (object)DBNull.Value),
                        new SqlParameter($"@LegalTrademarks{parameterIndex}", app.LegalTrademarks ?? (object)DBNull.Value),
                        new SqlParameter($"@OriginalFilename{parameterIndex}", app.OriginalFilename ?? (object)DBNull.Value),
                        new SqlParameter($"@PrivateBuild{parameterIndex}", app.PrivateBuild ?? (object)DBNull.Value),
                        new SqlParameter($"@ProductBuildPart{parameterIndex}", app.ProductBuildPart),
                        new SqlParameter($"@ProductMajorPart{parameterIndex}", app.ProductMajorPart),
                        new SqlParameter($"@ProductMinorPart{parameterIndex}", app.ProductMinorPart),
                        new SqlParameter($"@ProductName{parameterIndex}", app.ProductName ?? (object)DBNull.Value),
                        new SqlParameter($"@ProductPrivatePart{parameterIndex}", app.ProductPrivatePart),
                        new SqlParameter($"@ProductVersion{parameterIndex}", app.ProductVersion ?? (object)DBNull.Value),
                        new SqlParameter($"@SpecialBuild{parameterIndex}", app.SpecialBuild ?? (object)DBNull.Value),
                        new SqlParameter($"@CreationDate{parameterIndex}", app.CreationDate)
                    });

                    parameterIndex++;
                }

                queryBuilder.Append(';');

                using SqlCommand cmd = new(queryBuilder.ToString(), connection, transaction);
                cmd.Parameters.AddRange(parameterValues.ToArray());

                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex) when (ex.Number == 547) // Foreign key violation error number
            {
                throw new ArgumentException($"Error for one of the apps. Some App Ids are invalid or do not exist in the database.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> DeleteDriveRelationAsync(int appId, int driveId, SqlConnection connection, SqlTransaction transaction)
        {
            var appsDrivesRTable = db.AppsDrivesRelationTableNames;
            string query = @$"
                DELETE FROM {appsDrivesRTable.TableName}
                WHERE {appsDrivesRTable.AppId} = @appId
                AND {appsDrivesRTable.DriveId} = @driveId;";

            try
            {
                using (SqlCommand cmd = new(query, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@appId", appId);
                    cmd.Parameters.AddWithValue("@driveId", driveId);

                    await cmd.ExecuteNonQueryAsync();
                }

                return driveId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete app {appId} relation to drive {driveId}:" + ex);
            }
        }
        public async Task DeleteDriveRelationListAsync(List<ApplicationModel> appList, SqlConnection connection, SqlTransaction transaction)
        {
            var appsDrivesRTable = db.AppsDrivesRelationTableNames;
            var queryBuilder = new StringBuilder();
            var parameterIndex = 0;
            var parameterValues = new List<SqlParameter>();

            foreach (var app in appList)
            {
                if (parameterIndex > 0)
                {
                    queryBuilder.Append("; ");
                }

                queryBuilder.Append($@"
                    DELETE FROM {appsDrivesRTable.TableName}
                    WHERE {appsDrivesRTable.AppId} = @AppId{parameterIndex}
                    AND {appsDrivesRTable.DriveId} = @DriveId{parameterIndex}");

                parameterValues.Add(new SqlParameter($"@AppId{parameterIndex}", app.Id));
                parameterValues.Add(new SqlParameter($"@DriveId{parameterIndex}", app.DriveId));

                parameterIndex++;
            }

            queryBuilder.Append(';');

            try
            {
                using SqlCommand cmd = new(queryBuilder.ToString(), connection, transaction);
                cmd.Parameters.AddRange(parameterValues.ToArray());

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete app relations to drives: " + ex);
            }
        }

    }
}
