using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApiProj.Migrations
{
    /// <inheritdoc />
    public partial class isapproveds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedToBudgetTraker",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprovedToBudgetTraker",
                table: "Users");
        }
    }
}
