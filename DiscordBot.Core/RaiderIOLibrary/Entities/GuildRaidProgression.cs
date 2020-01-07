using Newtonsoft.Json;

namespace DiscordBot.Core.RaiderIOLibrary.Entities
{
    public class GuildRaidProgression
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("faction")]
        public string Faction { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("realm")]
        public string Realm { get; set; }

        [JsonProperty("profile_url")]
        public string URL { get; set; }

        [JsonProperty("raid_progression")]
        public Progression RaidInfo { get; set; }
    }
}
