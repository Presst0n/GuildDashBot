using Newtonsoft.Json;

namespace DiscordBot.Server.Bot.RaiderIOLibrary.Entities.MythicPlus
{
    public class MythicPlusScore
    {
        [JsonProperty("all")]
        public float Overall { get; set; }

        [JsonProperty("dps")]
        public float DPS { get; set; }

        [JsonProperty("healer")]
        public float Healer { get; set; }

        [JsonProperty("tank")]
        public float Tank { get; set; }
    }
}
