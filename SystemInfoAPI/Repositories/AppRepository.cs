using System.Data.SqlClient;
using SystemInfoApi.Models;

namespace SystemInfoApi.Repositories
{
    public class AppRepository
    {
        public async Task<AppModel> CreateAsync(AppModel app, SqlConnection conection, SqlTransaction transaction)
        {
            try
            {
                string query = @"
                    INSERT INTO Client_Machine_Disque_Application
                        (Name)
                    VALUES 
                        (@appName);

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new(query, conection, transaction))
                {
                    cmd.Parameters.AddWithValue("@appName", app.Name);
                };

                string relationQuery = @"
                    INSERT INTO Relation_Disque_Application
                        (ProductName)
                    VALUES
                        (@ProductName)";

                using (SqlCommand rCmd = new(query, conection, transaction))
                {
                    rCmd.Parameters.AddWithValue("@ProductName", app.FileVersionProperties);
                };
            }
            catch (Exception ex) { }
        }
    }
}
