using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Server.Data.Migrations
{
    public partial class AddingNewTablesTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "GuildMessages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Notify = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TwitchChannel",
                columns: table => new
                {
                    Channel_Id = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchChannel", x => x.Channel_Id);
                });

            migrationBuilder.CreateTable(
                name: "TwitchStreamers",
                columns: table => new
                {
                    UniqueID = table.Column<string>(nullable: false),
                    StreamerLogin = table.Column<string>(nullable: true),
                    StreamerId = table.Column<string>(nullable: true),
                    SentMessage = table.Column<bool>(nullable: false),
                    IsStreaming = table.Column<bool>(nullable: false),
                    UrlAddress = table.Column<string>(nullable: true),
                    PlayedGame = table.Column<string>(nullable: true),
                    Viewers = table.Column<int>(nullable: false),
                    ProfileImage = table.Column<string>(nullable: true),
                    StreamTitle = table.Column<string>(nullable: true),
                    TotalFollows = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchStreamers", x => x.UniqueID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildMessages");

            migrationBuilder.DropTable(
                name: "GuildNotifications");

            migrationBuilder.DropTable(
                name: "TwitchChannel");

            migrationBuilder.DropTable(
                name: "TwitchStreamers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
