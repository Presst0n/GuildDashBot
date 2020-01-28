using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.LibraryData.Models;
using DiscordBot.Core.Modules.Helpers;
using Abstractions.Db;

namespace DiscordBot.Core.Modules
{
    public class Twitch : ModuleBase<SocketCommandContext>
    {
        private readonly IDataAccess _database;

        public Twitch(IDataAccess database)
        {
            _database = database;
        }

        [Command("add_streamer")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task AddStreamer(string userLogin, string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!result)
            {
                await ReplyAsync("Podany adres URL jest błędny");
                return;
            }

            var streamer = await TwitchHelper.GetStreamer(userLogin);

            if (streamer.Users.Length == 0)
            {
                await ReplyAsync("Podany użytkownik nie istnieje.");
                return;
            }

            await _database.AddStreamerAsync(
                new StreamerDbModel()
                {
                    StreamerLogin = userLogin,
                    UrlAddress = url,
                    StreamerId = streamer.Users[0].Id
                });

            await ReplyAsync($"Użytkownik {userLogin} został dodany");
        }

        [Command("remove_streamer")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task RemoveStreamer([Remainder]string userLogin)
        {
            var result = _database.GetStreamers().Result.Where(x => x.StreamerLogin == userLogin).FirstOrDefault();

            if (result is null)
            {
                await ReplyAsync($"Użytkownika {userLogin} nie ma w bazie danych, wobec tego nie może zostac usunięty.");
                return;
            }
            else
            {
                await _database.DeleteStreamerAsync(result);
            }

            //foreach (var item in result)
            //{
            //    if (item.StreamerLogin == userLogin)
            //    {
            //        await _database.DeleteStreamerAsync(item);

            //    }

            //    await ReplyAsync($"Użytkownika {userLogin} nie ma w bazie danych, wobec tego nie może zostac usunięty.");
            //    return;

            //}

            await ReplyAsync($"Użytkownik {userLogin} został usunięty");
        }

        [Command("set_twitch")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetChannelForTwitchNotification()
        {
            var channel = Context.Channel.Id;
            await _database.SetId(channel);
            await ReplyAsync("Powiadomienia o streamach będą wysyłane na tym kanale.");
        }
    }
}
