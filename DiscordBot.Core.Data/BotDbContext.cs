using DiscordBot.Core.Data.Config;
using DiscordBot.LibraryData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Core.Data
{
    public class BotDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<GuildMessageDbModel> GuildMessages { get; set; }
        public DbSet<GuildNotificationDbModel> GuildNotifications { get; set; }
        public DbSet<StreamerDbModel> TwitchStreamers { get; set; }
        public DbSet<ChannelDbModel> TwitchChannel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(DataConfig.dataConfig.connectionString);
        }
    }
}
