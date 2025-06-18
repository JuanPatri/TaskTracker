using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskResourceCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskResources_Resources_ResourceId",
                table: "TaskResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskResources_Tasks_TaskTitle",
                table: "TaskResources");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskResources_Resources_ResourceId",
                table: "TaskResources",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskResources_Tasks_TaskTitle",
                table: "TaskResources",
                column: "TaskTitle",
                principalTable: "Tasks",
                principalColumn: "Title",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskResources_Resources_ResourceId",
                table: "TaskResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskResources_Tasks_TaskTitle",
                table: "TaskResources");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskResources_Resources_ResourceId",
                table: "TaskResources",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskResources_Tasks_TaskTitle",
                table: "TaskResources",
                column: "TaskTitle",
                principalTable: "Tasks",
                principalColumn: "Title",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
