using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphT.EfCore.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFun = table.Column<bool>(type: "bit", nullable: true),
                    IsProductive = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Complexity = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    Relevance = table.Column<int>(type: "int", nullable: true),
                    TaskType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskStreams",
                columns: table => new
                {
                    UpstreamTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DownstreamTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStreams", x => new { x.UpstreamTaskId, x.DownstreamTaskId });
                    table.ForeignKey(
                        name: "FK_TaskStreams_TodoTasks_DownstreamTaskId",
                        column: x => x.DownstreamTaskId,
                        principalTable: "TodoTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskStreams_TodoTasks_UpstreamTaskId",
                        column: x => x.UpstreamTaskId,
                        principalTable: "TodoTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStreams_DownstreamTaskId",
                table: "TaskStreams",
                column: "DownstreamTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoTasks_Id",
                table: "TodoTasks",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskStreams");

            migrationBuilder.DropTable(
                name: "TodoTasks");
        }
    }
}
