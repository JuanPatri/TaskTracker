using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSqlContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_AdministratorEmail",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "UserProject");

            migrationBuilder.DropIndex(
                name: "IX_Projects_AdministratorEmail",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "AdministratorEmail",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Projects",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectRoles",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    RoleType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRoles", x => new { x.ProjectId, x.UserEmail });
                    table.ForeignKey(
                        name: "FK_ProjectRoles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectRoles_Users_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserEmail",
                table: "Projects",
                column: "UserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRoles_UserEmail",
                table: "ProjectRoles",
                column: "UserEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_UserEmail",
                table: "Projects",
                column: "UserEmail",
                principalTable: "Users",
                principalColumn: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UserEmail",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectRoles");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserEmail",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "AdministratorEmail",
                table: "Projects",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserProject",
                columns: table => new
                {
                    ProjectsId = table.Column<int>(type: "int", nullable: false),
                    UsersEmail = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProject", x => new { x.ProjectsId, x.UsersEmail });
                    table.ForeignKey(
                        name: "FK_UserProject_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProject_Users_UsersEmail",
                        column: x => x.UsersEmail,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_AdministratorEmail",
                table: "Projects",
                column: "AdministratorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_UserProject_UsersEmail",
                table: "UserProject",
                column: "UsersEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_AdministratorEmail",
                table: "Projects",
                column: "AdministratorEmail",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
