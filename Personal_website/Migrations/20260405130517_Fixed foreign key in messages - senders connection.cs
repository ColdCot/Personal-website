using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Personal_website.Migrations
{
    /// <inheritdoc />
    public partial class Fixedforeignkeyinmessagessendersconnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Senders_senderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_senderId",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_senderId",
                table: "Messages",
                column: "senderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Senders_senderId",
                table: "Messages",
                column: "senderId",
                principalTable: "Senders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
