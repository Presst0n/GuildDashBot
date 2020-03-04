using Abstractions.Db;
using DiscordBot.LibraryData.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.Data.ContextServices
{
    public class BotDbService : IBotDbService
    {
        private readonly IConfiguration _configuration;

        public BotDbService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<GuildMessageDbModel> GetMessageById(string id)
        {
            using (var context = new BotDbContext(_configuration))
            {
                return Task.FromResult(context.GuildMessages
                    .FirstOrDefault(x => x.Id == id));
            }
        }

        public Task<GuildNotificationDbModel> GetNotificationStatus()
        {
            using (var context = new BotDbContext(_configuration))
            {
                return Task.FromResult(context.GuildNotifications.FirstOrDefault(x => x.Id.Length != 0));
            }
        }

        public async Task<IEnumerable<StreamerDbModel>> GetStreamers()
        {
            using (var context = new BotDbContext(_configuration))
            {
                if (!context.TwitchStreamers.Any()) { return null; }
                return await Task.FromResult<IEnumerable<StreamerDbModel>>(context.TwitchStreamers.ToList());
            }
        }

        public Task<ChannelDbModel> GetId()
        {
            using (var context = new BotDbContext(_configuration))
            {
                return Task.FromResult(context.TwitchChannel.FirstOrDefault(x => x.Channel_Id != 0));
            }
        }
    }
}
