using Newtonsoft.Json;

namespace DiscordBot.Core.RaiderIOLibrary.Entities.MythicPlus
{
    public class MythicPlusBestRuns
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("active_spec_name")]
        public string Spec { get; set; }

        [JsonProperty("mythic_plus_best_runs")]
        public BaseRuns[] BestRuns { get; set; }
    }
}
