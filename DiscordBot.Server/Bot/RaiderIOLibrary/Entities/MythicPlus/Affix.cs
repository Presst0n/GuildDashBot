using Newtonsoft.Json;

namespace DiscordBot.Server.Bot.RaiderIOLibrary.Entities.MythicPlus
{
    public class Affix
    {
        public Affix()
        {
        }

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("wowhead_url")]
        public string URL { get; set; }
    }
}
