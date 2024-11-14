using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication_Authorisation.Migrations
{
    public partial class AddExpiryTimeToIdentityUserToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryTime",
                table: "AspNetUserTokens",
                type: "datetime",
                nullable: true);  
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryTime",
                table: "AspNetUserTokens");
        }
    }
}
