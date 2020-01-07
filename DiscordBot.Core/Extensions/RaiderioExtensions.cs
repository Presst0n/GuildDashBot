using Discord;
using DiscordBot.Core.RaiderIOLibrary.Entities;
using DiscordBot.Core.RaiderIOLibrary.Entities.MythicPlus;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.Extensions
{
    public static class RaiderioExtensions
    {
        public static Embed GetEmbededGuildProgressMsg(this GuildRaidProgression raidProgression)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"{raidProgression.Name} raider.io");
            builder.AddField("Serwer:", $"{raidProgression.Realm}", true);
            builder.AddField("Region:", $"{raidProgression.Region}", true);
            builder.AddField("Frakcja:", $"{raidProgression.Faction}", true);

            builder.AddField("Uldir:",
                $"Normal: {raidProgression.RaidInfo.Uldir.NormalBossesKilled}/8\n" +
                $"Heroic: {raidProgression.RaidInfo.Uldir.HeroicBossesKilled}/8\n" +
                $"Mythic: {raidProgression.RaidInfo.Uldir.MythicBossesKilled}/8\n", true);

            builder.AddField("Battle of Dazar'alor:",
                $"Normal: {raidProgression.RaidInfo.BattleOfDazaralor.NormalBossesKilled}/9\n" +
                $"Heroic: {raidProgression.RaidInfo.BattleOfDazaralor.HeroicBossesKilled}/9\n" +
                $"Mythic: {raidProgression.RaidInfo.BattleOfDazaralor.MythicBossesKilled}/9\n", true);

            builder.AddField("The Eternal Palace:",
                $"Normal: {raidProgression.RaidInfo.TheEternalPalace.NormalBossesKilled}/8\n" +
                $"Heroic: {raidProgression.RaidInfo.TheEternalPalace.HeroicBossesKilled}/8\n" +
                $"Mythic: {raidProgression.RaidInfo.TheEternalPalace.MythicBossesKilled}/8\n", true);

            builder.WithUrl($"{raidProgression.URL}");
            builder.WithColor(Color.DarkTeal);
            var output = builder.Build();

            return output;
        }

        public static Embed GetEmbededRecentMpRunsMsg(this MythicPlusRecentRuns recentMplus)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.ThumbnailUrl = recentMplus.ThumbnailURL;
            builder.AddField("Nazwa postaci:", $"{recentMplus.Name}", false);
            builder.AddField("Rasa:", $"{recentMplus.Race}", true);
            builder.AddField("Klasa:", $"{recentMplus.Class}", true);
            foreach (var run in recentMplus.RecentRuns)
            {
                builder.AddField($"Dungeon:", $"{run.DungeonName} +{run.Level}", false);
                builder.AddField($"Czas:", $"{run.ClearTime.ConvertTime()} minut", true);
                if (run.KeystoneUpgradeNum > 0)
                {
                    builder.AddField($"Upgrade klucza:", $"+{recentMplus.RecentRuns[0].KeystoneUpgradeNum}", true);
                }
                else
                {
                    builder.AddField($"Upgrade klucza:", $"Zdepletowany xD", true);
                }

                if (run.Level < 4)
                {
                    builder.AddField($"Affixy:",
                        $"{run.Affixes[0].Name}", true);
                }
                if (run.Level >= 4 && recentMplus.RecentRuns[0].Level < 7)
                {
                    builder.AddField($"Affixy:",
                        $"{run.Affixes[0].Name}, " +
                        $"{run.Affixes[1].Name}", true);
                }
                if (run.Level >= 7 && recentMplus.RecentRuns[0].Level < 10)
                {
                    builder.AddField($"Affixy:",
                        $"{run.Affixes[0].Name}, " +
                        $"{run.Affixes[1].Name}, " +
                        $"{run.Affixes[2].Name}", true);
                }
                if (run.Level >= 10)
                {
                    builder.AddField($"Affixy:",
                        $"{run.Affixes[0].Name}, " +
                        $"{run.Affixes[1].Name}, " +
                        $"{run.Affixes[2].Name}, " +
                        $"{run.Affixes[3].Name}", true);
                }
            }
            builder.WithColor(Color.DarkBlue);
            var output = builder.Build();

            return output;
        }

        public static Embed GetEmbededBestMpRunsMsg(this MythicPlusBestRuns bestRuns)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.AddField("Nazwa postaci:", $"{bestRuns.Name}", true);
            builder.AddField("Klasa:", $"{bestRuns.Class}", true);
            foreach (var run in bestRuns.BestRuns)
            {
                builder.AddField($"Dungeon:", $"{run.DungeonShortName} +{run.Level}", false);
                builder.AddField($"Czas:", $"{run.ClearTime.ConvertTime()} minut", true);
                builder.AddField($"Score:", $"{run.Score}", true);
                if (run.KeystoneUpgradeNum > 0)
                {

                    builder.AddField($"Upgrade klucza:", $"+{run.KeystoneUpgradeNum}", true);
                }
                else
                {
                    builder.AddField($"Upgrade klucza:", $"Zdepletowany xD", true);
                }

                if (run.Level < 4)
                {
                    builder.AddField($"Affixy:",
                        $"{run.Affixes[0].Name}", true);
                }
                if (run.Level >= 4 && bestRuns.BestRuns[0].Level < 7)
                {
                    builder.AddField($"Affixy:",
                        $"{run.Affixes[0].Name}, " +
                        $"{run.Affixes[1].Name}", true);
                }
                if (run.Level >= 7 && bestRuns.BestRuns[0].Level < 10)
                {
                    builder.AddField($"Affixy:",
                        $"{run.Affixes[0].Name}, " +
                        $"{run.Affixes[1].Name}, " +
                        $"{run.Affixes[2].Name}", true);
                }
                if (run.Level >= 10)
                {
                    builder.AddField($"Affixy:",
                        $"{run.Affixes[0].Name}, " +
                        $"{run.Affixes[1].Name}, " +
                        $"{run.Affixes[2].Name}, " +
                        $"{run.Affixes[3].Name}", true);
                }
            }
            builder.WithColor(Color.DarkBlue);
            var output = builder.Build();

            return output;
        }

        public static Embed GetEmbededCharacterMsg(this Character character)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("Raider.io link");
            builder.WithUrl($"{character.ProfileURL}");
            builder.AddField("Nazwa postaci:", $"{character.Name}", false);
            builder.AddField("Klasa:", $"{character.Class}", true);
            builder.ThumbnailUrl = character.ThumbnailURL;
            builder.AddField("Spec:", $"{character.Spec}", true);
            builder.AddField("M+ Score:",
                $"Overall: {character.CurrentScore.Overall}\n" +
                $"DPS: {character.CurrentScore.DPS}\n" +
                $"Healer: {character.CurrentScore.Healer}\n" +
                $"Tank: {character.CurrentScore.Tank}", true);
            if (character.Guild == null)
            {
                builder.AddField("Gildia:", $"Brak", true);
            }
            else
            {
                builder.AddField("Gildia:", $"{character.Guild.Name} ({character.Guild.Realm})", true);
            }
            builder.AddField("Item Lvl:", $"Avg: {character.Gear.ItemLevelAverage}\n" +
                $"Equipped: {character.Gear.ItemLevelEquiped}", true);
            builder.AddField("The Eternal Palace:", $"M: {character.GetRaidProgression.TheEternalPalace.MythicBossesKilled}/8\n" +
                $"H: {character.GetRaidProgression.TheEternalPalace.HeroicBossesKilled}/8", true);
            builder.AddField("Battle of Dazar'alor:", $"M: {character.GetRaidProgression.BattleOfDazaralor.MythicBossesKilled}/9\n" +
                $"H: {character.GetRaidProgression.BattleOfDazaralor.HeroicBossesKilled}/9", true);
            builder.WithColor(Color.DarkBlue);
            var output = builder.Build();

            return output;
        }

        public static int ConvertTime(this int input)
        {
            return TimeSpan.FromMilliseconds(input).Minutes;
        }
    }
}
