using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphT.EfCore.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddLifeAreaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LifeAreas",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LifeAreaAggregateTaskAggregate",
                columns: table => new
                {
                    LifeAreasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TasksId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeAreaAggregateTaskAggregate", x => new { x.LifeAreasId, x.TasksId });
                    table.ForeignKey(
                        name: "FK_LifeAreaAggregateTaskAggregate_LifeAreas_LifeAreasId",
                        column: x => x.LifeAreasId,
                        principalTable: "LifeAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LifeAreaAggregateTaskAggregate_TaskAggregates_TasksId",
                        column: x => x.TasksId,
                        principalTable: "TaskAggregates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifeAreaAggregateTaskAggregate_TasksId",
                table: "LifeAreaAggregateTaskAggregate",
                column: "TasksId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeAreas_Id",
                table: "LifeAreas",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifeAreaAggregateTaskAggregate");

            migrationBuilder.DropTable(
                name: "LifeAreas");
        }
    }
}
