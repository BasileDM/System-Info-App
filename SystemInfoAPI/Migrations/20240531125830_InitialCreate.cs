using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemInfoApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    id_client = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.id_client);
                });

            migrationBuilder.CreateTable(
                name: "Client_Machine",
                columns: table => new
                {
                    id_client_machine = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_client = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client_Machine", x => x.id_client_machine);
                    table.ForeignKey(
                        name: "FK_Client_Machine_Client_id_client",
                        column: x => x.id_client,
                        principalTable: "Client",
                        principalColumn: "id_client",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client_Machine_Disque",
                columns: table => new
                {
                    id_client_machine_disque = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Free_Space = table.Column<long>(type: "bigint", nullable: false),
                    Total_Space = table.Column<long>(type: "bigint", nullable: false),
                    Free_Space_Percentage = table.Column<int>(type: "int", nullable: false),
                    Is_System_Drive = table.Column<int>(type: "int", nullable: false),
                    id_client_machine = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client_Machine_Disque", x => x.id_client_machine_disque);
                    table.ForeignKey(
                        name: "FK_Client_Machine_Disque_Client_Machine_id_client_machine",
                        column: x => x.id_client_machine,
                        principalTable: "Client_Machine",
                        principalColumn: "id_client_machine",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client_Machine_Disque_Os",
                columns: table => new
                {
                    id_client_machine_disque_os = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Directory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Architecture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Product_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Release_Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Current_Build = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ubr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_client_machine_disque = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client_Machine_Disque_Os", x => x.id_client_machine_disque_os);
                    table.ForeignKey(
                        name: "FK_Client_Machine_Disque_Os_Client_Machine_Disque_id_client_machine_disque",
                        column: x => x.id_client_machine_disque,
                        principalTable: "Client_Machine_Disque",
                        principalColumn: "id_client_machine_disque",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_Machine_id_client",
                table: "Client_Machine",
                column: "id_client");

            migrationBuilder.CreateIndex(
                name: "IX_Client_Machine_Disque_id_client_machine",
                table: "Client_Machine_Disque",
                column: "id_client_machine");

            migrationBuilder.CreateIndex(
                name: "IX_Client_Machine_Disque_Os_id_client_machine_disque",
                table: "Client_Machine_Disque_Os",
                column: "id_client_machine_disque",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Client_Machine_Disque_Os");

            migrationBuilder.DropTable(
                name: "Client_Machine_Disque");

            migrationBuilder.DropTable(
                name: "Client_Machine");

            migrationBuilder.DropTable(
                name: "Client");
        }
    }
}
