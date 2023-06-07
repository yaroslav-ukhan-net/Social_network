using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialNetwork.Migrations
{
    public partial class AddSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Users_FirstFriendId",
                table: "Friends");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Users_SecondFriendId",
                table: "Friends");

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddToGroups = table.Column<int>(type: "int", nullable: false),
                    SeeMyGroups = table.Column<int>(type: "int", nullable: false),
                    WriteToMe = table.Column<int>(type: "int", nullable: false),
                    LeavePosts = table.Column<int>(type: "int", nullable: false),
                    SeeMyPosts = table.Column<int>(type: "int", nullable: false),
                    SeeMyFriends = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId",
                table: "Settings",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Users_FirstFriendId",
                table: "Friends",
                column: "FirstFriendId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Users_SecondFriendId",
                table: "Friends",
                column: "SecondFriendId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Users_FirstFriendId",
                table: "Friends");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Users_SecondFriendId",
                table: "Friends");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Users_FirstFriendId",
                table: "Friends",
                column: "FirstFriendId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Users_SecondFriendId",
                table: "Friends",
                column: "SecondFriendId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
