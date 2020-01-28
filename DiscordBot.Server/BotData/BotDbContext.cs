using DiscordBot.LibraryData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Server.BotData
{
    public class BotDbContext : IdentityDbContext<IdentityUser>
    {
        //public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) { }
        private readonly IConfiguration _configuration; // Throws null, I know.

        public BotDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<GuildMessageDbModel> GuildMessages { get; set; }
        public DbSet<GuildNotificationDbModel> GuildNotifications { get; set; }
        public DbSet<StreamerDbModel> TwitchStreamers { get; set; }
        public DbSet<ChannelDbModel> TwitchChannel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
