using Microsoft.EntityFrameworkCore.Migrations;

namespace PlatePortal.Repository.Migrations
{
    public partial class floattodouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MenuItemPrice",
                table: "Products",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "MenuItemPrice",
                table: "Products",
                type: "real",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
