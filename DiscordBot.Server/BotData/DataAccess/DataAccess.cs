using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abstractions.Db;
using DiscordBot.LibraryData.Models;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Server.BotData.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration _configuration;

        public DataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task AddStreamerAsync(StreamerDbModel streamer)
        {
            using (var database = new BotDbContext(_configuration))
            {
                if (database.TwitchStreamers.Where(x => x.StreamerLogin == streamer.StreamerLogin).Count() < 1)
                {
                    database.TwitchStreamers.Add(new StreamerDbModel
                    {
                        StreamerLogin = streamer.StreamerLogin,
                        StreamerId = streamer.StreamerId,
                        IsStreaming = streamer.IsStreaming,
                        SentMessage = streamer.SentMessage,
                        ProfileImage = streamer.ProfileImage,
                        StreamTitle = streamer.StreamTitle,
                        PlayedGame = streamer.PlayedGame,
                        TotalFollows = streamer.TotalFollows,
                        UrlAddress = streamer.UrlAddress,
                        Viewers = streamer.Viewers
                    });
                }
                else
                {
                    var existingStreamer = database.TwitchStreamers.Where(x => x.StreamerLogin == streamer.StreamerLogin).FirstOrDefault();
                    existingStreamer.StreamerLogin = streamer.StreamerLogin;
                    existingStreamer.StreamerId = streamer.StreamerId;
                    existingStreamer.IsStreaming = streamer.IsStreaming;
                    existingStreamer.SentMessage = streamer.SentMessage;
                    existingStreamer.ProfileImage = streamer.ProfileImage;
                    existingStreamer.PlayedGame = streamer.PlayedGame;
                    existingStreamer.StreamTitle = streamer.StreamTitle;
                    existingStreamer.TotalFollows = streamer.TotalFollows;
                    existingStreamer.UrlAddress = streamer.UrlAddress;
                    existingStreamer.Viewers = streamer.Viewers;

                    database.TwitchStreamers.Update(existingStreamer);
                }

                await database.SaveChangesAsync();
            }
        }

        public async Task DeleteStreamerAsync(StreamerDbModel streamer)
        {
            using (var database = new BotDbContext(_configuration))
            {
                if (database.TwitchStreamers.Where(x => x.StreamerLogin == streamer.StreamerLogin).Count() > 0)
                {
                    database.TwitchStreamers.Remove(
                        database.TwitchStreamers
                        .Where(s => s.StreamerLogin == streamer.StreamerLogin)
                        .First());

                    await database.SaveChangesAsync();
                }
            }
        }

        public Task<ChannelDbModel> GetId()
        {
            using (var database = new BotDbContext(_configuration))
            {
                return Task.FromResult(database.TwitchChannel.FirstOrDefault());
            }
        }

        public async Task SetId(ulong id)
        {
            using (var database = new BotDbContext(_configuration))
            {
                if (database.TwitchChannel.Where(x => x.Channel_Id != id).Count() >= 1)
                {
                    ChannelDbModel ChannelFromDb = database.TwitchChannel.FirstOrDefault();
                    database.TwitchChannel.Remove(ChannelFromDb);
                    database.TwitchChannel.Add(new ChannelDbModel
                    {
                        Channel_Id = id,
                    });

                }
                else if (database.TwitchChannel.Where(x => x.Channel_Id == id).Count() < 1)
                {
                    database.TwitchChannel.Add(new ChannelDbModel
                    {
                        Channel_Id = id
                    });
                }

                await database.SaveChangesAsync();
            }
        }

        public Task<GuildNotificationDbModel> GetNotificationStatus()
        {
            using (var database = new BotDbContext(_configuration))
            {
                return Task.FromResult(database.GuildNotifications.FirstOrDefault());
            }
        }

        public async Task<IEnumerable<StreamerDbModel>> GetStreamers()
        {
            using (var database = new BotDbContext(_configuration))
            {
                return await Task.FromResult<IEnumerable<StreamerDbModel>>(database.TwitchStreamers.ToList());
            }
        }

        public async Task SetNotificationStatus(GuildNotificationDbModel input)
        {
            using (var database = new BotDbContext(_configuration))
            {
                if (database.GuildNotifications.Where(x => x.Id == input.Id).Count() < 1)
                {
                    database.GuildNotifications.Add(input);
                }
                else
                {
                    var result = database.GuildNotifications.FirstOrDefault();
                    result.Notify = input.Notify;

                    database.GuildNotifications.Update(result);
                }

                await database.SaveChangesAsync();
            }
        }


        public Task<IEnumerable<GuildMessageDbModel>> GetMessages()
        {
            using (var database = new BotDbContext(_configuration))
            {
                return Task.FromResult<IEnumerable<GuildMessageDbModel>>(database.GuildMessages);
            }
        }

        public Task<GuildMessageDbModel> GetMessageById(string id)
        {
            using (var database = new BotDbContext(_configuration))
            {
                return Task.FromResult(database.GuildMessages.Where(x => x.Id == id).FirstOrDefault());
            }
        }

        public async Task AddAsync(GuildMessageDbModel message)
        {
            using (var database = new BotDbContext(_configuration))
            {
                if (database.GuildMessages.Where(x => x.Id == message.Id).Count() < 1)
                {
                    database.GuildMessages.Add(message);
                }
                else
                {
                    var result = database.GuildMessages.Where(x => x.Id == message.Id).FirstOrDefault();
                    result.Message = message.Message;

                    database.GuildMessages.Update(result);
                }

                await database.SaveChangesAsync();
            }
        }

        public Task<StreamerDbModel> GetStreamerById(string id)
        {
            throw new NotImplementedException();
        }
    }
}
