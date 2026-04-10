using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Personal_website.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueSenderIndex : Migration
    {
        /// <summary>
        /// Applies schema changes to the Senders table: limits Name and Email to 50 characters (non-nullable) and adds a unique composite index on Name and Email.
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Senders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Senders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Name_Email",
                table: "Senders",
                columns: new[] { "Name", "Email" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Senders_Name_Email",
                table: "Senders");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Senders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Senders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
