using Newtonsoft.Json;

namespace DiscordBot.Core.RaiderIOLibrary.Entities
{
    public class Gear
    {
        [JsonProperty("item_level_equipped")]
        public int ItemLevelEquiped { get; set; }
        [JsonProperty("item_level_total")]
        public int ItemLevelAverage { get; set; }
        [JsonProperty("artifact_traits")]
        public int AzeritePower { get; set; }
    }
}
