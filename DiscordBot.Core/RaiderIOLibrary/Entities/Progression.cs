using Newtonsoft.Json;

namespace DiscordBot.Core.RaiderIOLibrary.Entities
{
    public class Progression
    {
        [JsonProperty("antorus-the-burning-throne")]
        public Raid Antorus { get; set; }

        [JsonProperty("the-emerald-nightmare")]
        public Raid EmeraldNightmare { get; set; }

        [JsonProperty("the-nighthold")]
        public Raid Nighthold { get; set; }

        [JsonProperty("tomb-of-sargeras")]
        public Raid TombOfSargeras { get; set; }

        [JsonProperty("trial-of-valor")]
        public Raid TrialOfValor { get; set; }

        [JsonProperty("uldir")]
        public Raid Uldir { get; set; }

        [JsonProperty("battle-of-dazaralor")]
        public Raid BattleOfDazaralor { get; set; }

        [JsonProperty("crucible-of-storms")]
        public Raid CrucibleOfStorms { get; set; }

        [JsonProperty("the-eternal-palace")]
        public Raid TheEternalPalace { get; set; }
    }
}
