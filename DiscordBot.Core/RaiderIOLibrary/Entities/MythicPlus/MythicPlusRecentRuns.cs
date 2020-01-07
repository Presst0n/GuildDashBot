using Newtonsoft.Json;

namespace DiscordBot.Core.RaiderIOLibrary.Entities.MythicPlus
{
    public class MythicPlusRecentRuns
    {
        [JsonProperty("mythic_plus_recent_runs")]
        public BaseRuns[] RecentRuns { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("race")]
        public string Race { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("active_spec_name")]
        public string Spec { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailURL { get; set; }

        [JsonProperty("profile_url")]
        public string ProfileURL { get; set; }
    }
}
