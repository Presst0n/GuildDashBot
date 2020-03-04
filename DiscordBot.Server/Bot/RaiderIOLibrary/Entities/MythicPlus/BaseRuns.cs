using Newtonsoft.Json;
using System;

namespace DiscordBot.Server.Bot.RaiderIOLibrary.Entities.MythicPlus
{
    public class BaseRuns
    {
        public BaseRuns()
        {
        }

        [JsonProperty("dungeon")]
        public string DungeonName { get; set; }

        [JsonProperty("short_name")]
        public string DungeonShortName { get; set; }

        [JsonProperty("mythic_level")]
        public int Level { get; set; }

        [JsonProperty("completed_at")]
        public DateTime Date { get; set; }

        [JsonProperty("clear_time_ms")]
        public int ClearTime { get; set; }

        [JsonProperty("num_keystone_upgrades")]
        public int KeystoneUpgradeNum { get; set; }

        [JsonProperty("map_challenge_mode_id")]
        public int Id { get; set; }

        [JsonProperty("score")]
        public float Score { get; set; }

        [JsonProperty("affixes")]
        public Affix[] Affixes { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
