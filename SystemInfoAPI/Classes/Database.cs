using System.Collections;
using System.Data.SqlClient;
using SystemInfoApi.Utilities;

namespace SystemInfoApi.Classes
{
    public delegate Task<T> TransactionOperation<T>(SqlConnection connection, SqlTransaction transaction);

    public class Database
    {
        protected readonly string? _ConnectionString;
        public readonly CustomersTableNames CustomersTableNames;
        public readonly MachinesTableNames MachinesTableNames;
        public readonly DrivesTableNames DrivesTableNames;
        public readonly OsTableNames OsTableNames;
        public readonly AppsDrivesRelationTableNames AppsDrivesRelationTableNames;
        public readonly ApplicationsTableNames ApplicationsTableNames;
        public readonly DrivesHistoryTableNames DrivesHistoryTableNames;
        public readonly OsHistoryTableNames OsHistoryTableNames;
        public readonly AppsDrivesRelationHistoryTableNames AppsDrivesRelationHistoryTableNames;

        public Database(IConfiguration configuration, IWebHostEnvironment env)
        {
            string connectionStrValue = env.IsDevelopment() ? "SystemInfoDbDev" : "SysteminfoDb";
            _ConnectionString = configuration.GetSection("ConnectionStrings")[connectionStrValue];

            CustomersTableNames = new();
            MachinesTableNames = new();
            DrivesTableNames = new();
            OsTableNames = new();
            AppsDrivesRelationTableNames = new();
            ApplicationsTableNames = new();
            DrivesHistoryTableNames = new();
            OsHistoryTableNames = new();
            AppsDrivesRelationHistoryTableNames = new();
        }

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(_ConnectionString);
        }
        public void Init(WebApplication app)
        {
            try
            {
                using SqlConnection connection = CreateConnection();
                TryOpenConnection(connection);

                // Check auto migration configuration
                string? autoMigration = app.Configuration.GetSection("AutoMigration").Value;

                if (!DoTablesExist(connection))
                {
                    if (autoMigration != null && String.Equals(autoMigration.ToLower(), "true"))
                    {
                        CreateTablesAsync().Wait();
                    }
                    else
                    {
                        throw new ApplicationException(
                            "Warning: Mandatory database tables were not detected.\r\n " +
                            "Switch auto-migration to true in appsetings.json, or make sure the tables are created in the database for the program to function.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed:\r\n{ex.Message}\r\n");
                app.StopAsync();
                return;
            }
        }
        private static void TryOpenConnection(SqlConnection connection)
        {
            try
            {
                connection.Open();
                Console.WriteLine("Connection to the database established successfully.\r\n");
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting to the database, please check your appsettings.json configuration file.", ex);
            }

        }
        private bool DoTablesExist(SqlConnection connection)
        {
            try
            {
                string checkTablesSql = @$"
                    SELECT COUNT(*) 
                    FROM information_schema.tables 
                    WHERE table_name = '{MachinesTableNames.TableName}'";

                using SqlCommand cmd = new(checkTablesSql, connection);
                int? tableCount = (int?)cmd.ExecuteScalar();
                return tableCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error verifying database tables: {ex.Message}");
            }
        }
        private async Task<bool> CreateTablesAsync()
        {
            return await MakeTransactionAsync(async (connection, transaction) =>
            {
                Console.WriteLine("Creating tables...");

                string migrationPath = AppDomain.CurrentDomain.BaseDirectory + "/Migrations/SysteminfoDb.sql";
                string script = File.ReadAllText(migrationPath);

                await using SqlCommand cmd = new(script, connection, transaction);
                await cmd.ExecuteNonQueryAsync();
                return true;
            });
        }

        /// <summary>
        ///     This method is a transaction wrapper for a database operation that contains multiple commands.
        ///     It handles <see cref="SqlConnection"/> and <see cref="SqlTransaction"/> management for a specified operation.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the transaction operation.</typeparam>
        /// <param name="operation">
        ///     A delegate: <see cref="TransactionOperation{T}"/> that represents the operation to be executed within the transaction.
        ///     The operation receives a <see cref="SqlConnection"/> and a <see cref="SqlTransaction"/> as parameters.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> that represents the asynchronous operation. The task result contains the result of the executed operation.
        /// </returns>
        /// <remarks>
        ///     This method opens a connection, begins a transaction, executes the provided <see cref="TransactionOperation{T}"/>,
        ///     and then commits or rolls back the transaction based on the success or failure of the operation.
        /// </remarks>
        protected async Task<T> MakeTransactionAsync<T>(TransactionOperation<T> operation)
        {
            await using SqlConnection connection = CreateConnection();
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            connection.StatisticsEnabled = true;
            if (ConsoleUtils._logTransactionNotice) Console.WriteLine("New database transaction initiated...");

            var currentColor = Console.ForegroundColor;

            try
            {
                T result = await operation(connection, transaction);
                await transaction.CommitAsync();
                return result;
            }
            catch (ArgumentException ex)
            {
                await transaction.RollbackAsync();
                ConsoleUtils.WriteLineColored("Rolling back transaction due to an argument error:\r\n" + ex.Message, ConsoleColor.Red);
                throw new ArgumentException("Error finalising the transaction with the database.", ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                ConsoleUtils.WriteLineColored("Rolling back transaction due to an argument error:\r\n" + ex.Message, ConsoleColor.Red);
                throw new ApplicationException("Error finalising the transaction with the database.", ex);
            }
            finally
            {
                await transaction.DisposeAsync();
                await connection.CloseAsync();
                ConsoleUtils.LogTransactionStats(connection);
            }
        }
    }

    public class CustomersTableNames
    {
        public string TableName { get; } = "Client";
        public string Id { get; } = "id_client";
        public string Name { get; } = "Nom_Client";
    }
    public class MachinesTableNames
    {
        public string TableName { get; } = "Client_machine";
        public string Id { get; } = "id_client_machine";
        public string CustomerId { get; } = "id_client";
        public string MachineName { get; } = "Nom_Machine";
        public string MachineCreationDate { get; } = "Sys_Date_Creation";
    }
    public class DrivesTableNames
    {
        public string TableName { get; } = "Client_Machine_Disque";
        public string Id { get; } = "id_client_machine_disque";
        public string MachineId { get; } = "id_client_machine";
        public string SerialNumber { get; } = "Numero_Serie_Logique";
        public string DriveName { get; } = "Nom_Disque";
        public string RootDirectory { get; } = "Dossier_Racine";
        public string Label { get; } = "Label";
        public string Type { get; } = "Type";
        public string Format { get; } = "Format";
        public string Size { get; } = "Taille";
        public string FreeSpace { get; } = "Espace_Disponible";
        public string TotalSpace { get; } = "Espace_Total";
        public string FreeSpacePercentage { get; } = "Espace_Disponible_Pourcent";
        public string IsSystemDrive { get; } = "Is_System_Drive";
        public string DriveCreationDate { get; } = "Sys_Date_Creation";
    }
    public class OsTableNames
    {
        public string TableName { get; } = "Client_Machine_Disque_Os";
        public string Id { get; } = "id_client_machine_disque_os";
        public string DriveId { get; } = "id_client_machine_disque";
        public string Directory { get; } = "Dossier";
        public string Architecture { get; } = "Architecture";
        public string Version { get; } = "Version";
        public string ProductName { get; } = "Nom_Produit";
        public string ReleaseId { get; } = "Id_Release";
        public string CurrentBuild { get; } = "Build_Actuel";
        public string Ubr { get; } = "Update_Build_Revision";
        public string OsCreationDate { get; } = "Sys_Date_Creation";
    }
    public class AppsDrivesRelationTableNames
    {
        public string TableName { get; } = "Relation_Disque_Application";
        public string DriveId { get; } = "id_client_machine_disque";
        public string AppId { get; } = "id_client_machine_disque_app";
        public string Comments { get; } = "Commentaires";
        public string CompanyName { get; } = "Nom_Entreprise";
        public string FileBuildPart { get; } = "Partie_Build_Fichier";
        public string FileDescription { get; } = "Description_Fichier";
        public string FileMajorPart { get; } = "Partie_Majeure_Fichier";
        public string FileMinorPart { get; } = "Partie_Mineure_Fichier";
        public string FileName { get; } = "Nom_Fichier";
        public string FilePrivatePart { get; } = "Partie_Privee_Fichier";
        public string FileVersion { get; } = "Version_Fichier";
        public string InternalName { get; } = "Nom_Interne";
        public string IsDebug { get; } = "Is_Debug";
        public string IsPatched { get; } = "Is_Patched";
        public string IsPreRelease { get; } = "Is_Pre_Release";
        public string IsPrivateBuild { get; } = "Is_Private_Build";
        public string IsSpecialBuild { get; } = "Is_Special_Build";
        public string Language { get; } = "Langage";
        public string Copyright { get; } = "Copyright";
        public string Trademarks { get; } = "Trademarks";
        public string OriginalFilename { get; } = "Nom_Fichier_Original";
        public string PrivateBuild { get; } = "Build_Prive";
        public string ProductBuildPart { get; } = "Partie_Build_Produit";
        public string ProductMajorPart { get; } = "Partie_Majeure_Produit";
        public string ProductMinorPart { get; } = "Partie_Mineure_Produit";
        public string ProductName { get; } = "Nom_Produit";
        public string ProductPrivatePart { get; } = "Partie_Privee_Produit";
        public string ProductVersion { get; } = "Version_Produit";
        public string SpecialBuild { get; } = "Build_Special";
        public string AppRelationCreationDate { get; } = "Sys_Date_Creation";
    }
    public class ApplicationsTableNames
    {
        public string TableName { get; } = "Client_Machine_Disque_Application";
        public string Id { get; } = "id_client_machine_disque_app";
        public string AppName { get; } = "Nom_Application";
    }
    public class DrivesHistoryTableNames
    {
        public string TableName { get; } = "Client_Machine_Disque_Historique";
        public string Id { get; } = "id_client_machine_disque_historique";
        public string MachineId { get; } = "id_client_machine";
        public string SerialNumber { get; } = "Numero_Serie_Logique";
        public string DriveName { get; } = "Nom_Disque";
        public string RootDirectory { get; } = "Dossier_Racine";
        public string Label { get; } = "Label";
        public string Type { get; } = "Type";
        public string Format { get; } = "Format";
        public string Size { get; } = "Taille";
        public string FreeSpace { get; } = "Espace_Disponible";
        public string TotalSpace { get; } = "Espace_Total";
        public string FreeSpacePercentage { get; } = "Espace_Disponible_Pourcent";
        public string IsSystemDrive { get; } = "Is_System_Drive";
        public string DriveCreationDate { get; } = "Sys_Date_Creation";
    }
    public class OsHistoryTableNames
    {
        public string TableName { get; } = "Client_Machine_Disque_Os_Historique";
        public string Id { get; } = "id_client_machine_disque_os_historique";
        public string DriveId { get; } = "id_client_machine_disque_historique";
        public string Directory { get; } = "Dossier";
        public string Architecture { get; } = "Architecture";
        public string Version { get; } = "Version";
        public string ProductName { get; } = "Nom_Produit";
        public string ReleaseId { get; } = "Id_Release";
        public string CurrentBuild { get; } = "Build_Actuel";
        public string Ubr { get; } = "Update_Build_Revision";
        public string OsCreationDate { get; } = "Sys_Date_Creation";
    }
    public class AppsDrivesRelationHistoryTableNames
    {
        public string TableName { get; } = "Relation_Disque_Application_Historique";
        public string DriveId { get; } = "id_client_machine_disque_historique";
        public string AppId { get; } = "id_client_machine_disque_app";
        public string Comments { get; } = "Commentaires";
        public string CompanyName { get; } = "Nom_Entreprise";
        public string FileBuildPart { get; } = "Partie_Build_Fichier";
        public string FileDescription { get; } = "Description_Fichier";
        public string FileMajorPart { get; } = "Partie_Majeure_Fichier";
        public string FileMinorPart { get; } = "Partie_Mineure_Fichier";
        public string FileName { get; } = "Nom_Fichier";
        public string FilePrivatePart { get; } = "Partie_Privee_Fichier";
        public string FileVersion { get; } = "Version_Fichier";
        public string InternalName { get; } = "Nom_Interne";
        public string IsDebug { get; } = "Is_Debug";
        public string IsPatched { get; } = "Is_Patched";
        public string IsPreRelease { get; } = "Is_Pre_Release";
        public string IsPrivateBuild { get; } = "Is_Private_Build";
        public string IsSpecialBuild { get; } = "Is_Special_Build";
        public string Language { get; } = "Langage";
        public string Copyright { get; } = "Copyright";
        public string Trademarks { get; } = "Trademarks";
        public string OriginalFilename { get; } = "Nom_Fichier_Original";
        public string PrivateBuild { get; } = "Build_Prive";
        public string ProductBuildPart { get; } = "Partie_Build_Produit";
        public string ProductMajorPart { get; } = "Partie_Majeure_Produit";
        public string ProductMinorPart { get; } = "Partie_Mineure_Produit";
        public string ProductName { get; } = "Nom_Produit";
        public string ProductPrivatePart { get; } = "Partie_Privee_Produit";
        public string ProductVersion { get; } = "Version_Produit";
        public string SpecialBuild { get; } = "Build_Special";
        public string AppRelationCreationDate { get; } = "Sys_Date_Creation";
    }
}
