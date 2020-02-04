using DiscordBot.Core.Services;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordBot.Core
{
    public class StreamsHandler
    {
        private readonly TwitchService _twitchService;
        private readonly GuildBot _bot;
        private Timer _timer;

        public StreamsHandler(TwitchService twitchService, GuildBot bot)
        {
            _twitchService = twitchService;
            _bot = bot;
        }

        public async Task LaunchAsync()
        {
            await _twitchService.ResetStreamers();

            _timer = new Timer(TimeSpan.FromMinutes(3).TotalMilliseconds)
            {
                AutoReset = true
            };
            _timer.Elapsed += /*new ElapsedEventHandler(OnElapsed);*/OnElapsed;
            _timer.Start();

            //return Task.CompletedTask;
        }

        private async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            await HandleStreams();
        }

        public async Task HandleStreams()
        {
            if (_bot.IsRunning())
            {
                await _twitchService.InitializeService();
            }
        }
    }
}
