using Newtonsoft.Json;

namespace DiscordBot.Core.RaiderIOLibrary.Entities
{
    public class Guild
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("realm")]
        public string Realm { get; set; }
    }
}
