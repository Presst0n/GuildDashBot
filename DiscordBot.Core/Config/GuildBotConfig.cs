using Newtonsoft.Json;
using System.IO;

namespace DiscordBot.Core.Config
{
    public class GuildBotConfig
    {
        private const string configFolder = "../DiscordBot.Core/Config/ConfigFiles"; // For local usage: "../DiscordBot.Core/Config/ConfigFiles" "Config/ConfigFiles"
        private const string configFile = "settings.json";
        public static BotConfig bot;

        static GuildBotConfig()
        {
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }
            if (!File.Exists($"{configFolder}/{configFile}"))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText($"{configFolder}/{configFile}", json);
            }
            else
            {
                string json = File.ReadAllText($"{configFolder}/{configFile}");
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public struct BotConfig
        {
            public string cmdPrefix;
            public string twitchToken;
            public string twitchClient;
            public string discordToken;
        }
    }
}
