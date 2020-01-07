using DiscordBot.Core.RaiderIOLibrary.Entities;
using DiscordBot.Core.RaiderIOLibrary.Entities.Enums;
using DiscordBot.Core.RaiderIOLibrary.Entities.MythicPlus;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordBot.Core.Services
{
    public class RaiderioService
    {
        // Make database model for this. I want this later to be configurable via web panel. 
        private const string url = "https://raider.io/api/v1/guilds/profile?region=eu&realm=Kazzak&name=Res%20Publica&fields=raid_progression";

        private string _name { get; set; }
        private string _realm { get; set; }
        private Region _region { get; set; }

        public async Task<GuildRaidProgression> GetGuildRaidProgAsync()
        {
            return await RetrieveData<GuildRaidProgression>(url);
        }

        public void CaptureData(string[] input)
        {
            _name = input[0];
            _realm = input[1];
            Enum.TryParse(input[2], out Region convertedRegion);
            _region = convertedRegion;
        }

        public async Task<Character> GetCharacterAsync(string[] input)
        {
            CaptureData(input);
            var output = await RetrieveData<Character>(GetUrl(DataType.Character));
            return output;
        }

        public async Task<MythicPlusRecentRuns> GetRecentRunsAsync(string[] input)
        {
            CaptureData(input);
            var output = await RetrieveData<MythicPlusRecentRuns>(GetUrl(DataType.MythicPlusRecent));
            return output;
        }

        public async Task<MythicPlusBestRuns> GetBestRunsAsync(string[] input, int count)
        {
            CaptureData(input);
            var output = await RetrieveData<MythicPlusBestRuns>($"{GetUrl(DataType.MythicPlusBest)}:{count}");
            return output;
        }

        private async Task<T> RetrieveData<T>(string baseUrl)
            => await Task.Run(async () => JsonConvert.DeserializeObject<T>(await GetRawData(baseUrl)));

        private async Task<string> GetRawData(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage message = await client.GetAsync(url);
                    message.EnsureSuccessStatusCode();
                    return await message.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        private string GetUrl(DataType type)
        {
            switch (type)
            {
                case DataType.Character:
                    return $"{GetBaseUrlRegion(_region)}&realm={_realm}&name={_name}&fields=gear%2Cguild%2Craid_progression%2Cmythic_plus_scores%2Cguild";
                case DataType.MythicPlusRecent:
                    return $"{GetBaseUrlRegion(_region)}&realm={_realm}&name={_name}&fields=mythic_plus_recent_runs";
                case DataType.MythicPlusBest:
                    return $"{GetBaseUrlRegion(_region)}&realm={_realm}&name={_name}&fields=mythic_plus_best_runs";
                default:
                    throw new Exception("Error in RaiderIOClient - GetUrl");
            }
        }

        private string GetBaseUrlRegion(Region region)
        {
            string baseUrl = null;
            switch (region)
            {
                case Region.US:
                    baseUrl = $"https://raider.io/api/v1/characters/profile?region=us";
                    break;
                case Region.EU:
                    baseUrl = $"https://raider.io/api/v1/characters/profile?region=eu";
                    break;
                default:
                    throw new NotSupportedException("The defined Region is not supported. Contact The Developer.");
            }
            return baseUrl;
        }
    }
}
