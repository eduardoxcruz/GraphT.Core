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
                name: "TaskAggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFun = table.Column<bool>(type: "bit", nullable: false),
                    IsProductive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Complexity = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Relevance = table.Column<int>(type: "int", nullable: false),
                    DateTimeInfo_CreationDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateTimeInfo_FinishDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DateTimeInfo_LimitDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DateTimeInfo_StartDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DateTimeInfo_TimeSpend = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAggregates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskLogs",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TimeSpentOnTask = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TaskStreams",
                columns: table => new
                {
                    UpstreamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DownstreamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStreams", x => new { x.UpstreamId, x.DownstreamId });
                    table.ForeignKey(
                        name: "FK_TaskStreams_TaskAggregates_DownstreamId",
                        column: x => x.DownstreamId,
                        principalTable: "TaskAggregates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskStreams_TaskAggregates_UpstreamId",
                        column: x => x.UpstreamId,
                        principalTable: "TaskAggregates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskAggregates_Id",
                table: "TaskAggregates",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStreams_DownstreamId",
                table: "TaskStreams",
                column: "DownstreamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskLogs");

            migrationBuilder.DropTable(
                name: "TaskStreams");

            migrationBuilder.DropTable(
                name: "TaskAggregates");
        }
    }
}
