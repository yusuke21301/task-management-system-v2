using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductionProgress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessToWorkTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "process_id",
                table: "work_tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_tasks_process_id",
                table: "work_tasks",
                column: "process_id");

            migrationBuilder.AddForeignKey(
                name: "FK_work_tasks_processes_process_id",
                table: "work_tasks",
                column: "process_id",
                principalTable: "processes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_work_tasks_processes_process_id",
                table: "work_tasks");

            migrationBuilder.DropIndex(
                name: "IX_work_tasks_process_id",
                table: "work_tasks");

            migrationBuilder.DropColumn(
                name: "process_id",
                table: "work_tasks");
        }
    }
}
