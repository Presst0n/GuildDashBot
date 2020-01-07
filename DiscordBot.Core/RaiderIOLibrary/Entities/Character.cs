using DiscordBot.Core.RaiderIOLibrary.Entities.MythicPlus;
using Newtonsoft.Json;

namespace DiscordBot.Core.RaiderIOLibrary.Entities
{
    public class Character
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("profile_url")]
        public string ProfileURL { get; set; }

        [JsonProperty("active_spec_name")]
        public string Spec { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailURL { get; set; }

        [JsonProperty("gear")]
        public Gear Gear { get; set; }

        [JsonProperty("raid_progression")]
        public Progression GetRaidProgression { get; set; }

        [JsonProperty("guild")]
        public Guild Guild { get; set; }

        [JsonProperty("mythic_plus_scores")]
        public MythicPlusScore CurrentScore { get; set; }
    }
}
