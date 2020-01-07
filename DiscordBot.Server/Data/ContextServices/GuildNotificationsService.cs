using DiscordBot.LibraryData;
using DiscordBot.LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Server.Data.ContextServices
{
    public class GuildNotificationsService : IGuildNotifications
    {
        private ApplicationDbContext _context;

        public GuildNotificationsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<GuildNotificationDbModel> GetNotificationStatus()
        {
            return Task.FromResult(_context.GuildNotifications.FirstOrDefault());
        }

        public async Task SetNotificationStatus(GuildNotificationDbModel input)
        {
            if(_context.GuildNotifications.Where(x => x.Id == input.Id).Count() < 1)
            {
                _context.GuildNotifications.Add(new GuildNotificationDbModel
                {
                    Id = input.Id,
                    Notify = input.Notify
                });
            }
            else
            {
                var result = _context.GuildNotifications.FirstOrDefault();
                result.Id = input.Id;
                result.Notify = input.Notify;

                _context.GuildNotifications.Update(result);
            }

            await _context.SaveChangesAsync();
        }
    }
}
