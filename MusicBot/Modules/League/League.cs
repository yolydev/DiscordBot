using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using MusicBot.Services;

namespace MusicBot.Modules.League
{
    public class League : ModuleBase<SocketCommandContext>
    {
        [Command("elo")]
        public async Task GetEloAsync([Remainder]string username)
        {
            SocketGuildUser user = (SocketGuildUser)Context.User;
            var userAvatar = user.GetAvatarUrl();

            try
            {
                #region Basic Info
                var riotApi = RiotApi.NewInstance(Config.bot.riotToken);

                var summoner = riotApi.SummonerV4.GetBySummonerName(Region.EUW, username);
                var summonerIconURL = $"http://ddragon.leagueoflegends.com/cdn/9.15.1/img/profileicon/{summoner.ProfileIconId}.png";

                //var xx = riotApi.LeagueV4.GetLeagueEntries(Region.EUW, "RANKED_TFT", "MASTER", "I");
                //foreach(var x in xx) 
                //{
                //    Console.WriteLine($"Name: {x.SummonerName} {x.Tier} {x.Rank} {x.LeaguePoints}LP {x.Wins}W {x.Losses}L");
                //}
                //Console.WriteLine(summoner.Id);
                #endregion

                #region Summoner Rankings
                var ranks = riotApi.LeagueV4.GetLeagueEntriesForSummoner(Region.EUW, summoner.Id);
                var soloDuo = ranks.FirstOrDefault(x => x.QueueType == "RANKED_SOLO_5x5");

                var winRatio = Convert.ToDouble(String.Format("{0:.##}", ((double)soloDuo.Wins / (double)(soloDuo.Wins+soloDuo.Losses)) * 100));
                #endregion

                #region Summoner Champions
                var masteries = riotApi.ChampionMasteryV4.GetAllChampionMasteries(Region.EUW, summoner.Id);
                var result = "";
                for (var i = 0; i < 3; i++)
                {
                    var mastery = masteries[i];
                    var champ = (Champion)mastery.ChampionId;
                    result += $"**{champ.Name()}** {mastery.ChampionPoints} pts - lvl {mastery.ChampionLevel}\n";
                }
                #endregion

                #region Summoner Recent Games
                var matchList = await riotApi.MatchV4.GetMatchlistAsync(Region.EUW, summoner.AccountId, queue: new[] { 420 }, endIndex: 3);
                var matchDataTasks = matchList.Matches.Select(matchMetaData => riotApi.MatchV4.GetMatchAsync(Region.EUW, matchMetaData.GameId)).ToArray();
                var matchDatas = await Task.WhenAll(matchDataTasks);
                var history = "";
                for (var i = 0; i < matchDatas.Count(); i++)
                {
                    var matchData = matchDatas[i];
                    var participantIdData = matchData.ParticipantIdentities
                        .First(pi => summoner.Id.Equals(pi.Player.SummonerId));
                    var participant = matchData.Participants.First(p => p.ParticipantId == participantIdData.ParticipantId);

                    var win = participant.Stats.Win;
                    var champ = (Champion)participant.ChampionId;
                    var level = participant.Stats.ChampLevel;
                    var kills = participant.Stats.Kills;
                    var deaths = participant.Stats.Deaths;
                    var assists = participant.Stats.Assists;
                    var visionScore = participant.Stats.VisionScore;
                    var cs = participant.Stats.TotalMinionsKilled + participant.Stats.NeutralMinionsKilled;
                    var kda = String.Format("{0:0.##}", (kills + assists) / (float)deaths);
                    var duration = matchData.GameDuration;
                    history += win ? $"Win - lvl **{level}** - {champ.Name()} **{kills}**/{deaths}/**{assists}** » ({kda}KDA) - [{cs}**CS** / {visionScore}**VS**]\n": 
                                     $"Loss - lvl **{level}** - {champ.Name()} **{kills}**/{deaths}/**{assists}** » ({kda}KDA) - [{cs}**CS** / {visionScore}**VS**]\n";
                }
                #endregion 

                #region Summoner Status
                /*
                 * Assets: https://github.com/CommunityDragon/Docs/blob/master/assets.md
                 * 
                 * Raw Assets: http://raw.communitydragon.org/latest/
                 * 
                 * Rune Infos: http://ddragon.leagueoflegends.com/cdn/9.13.1/data/en_US/runesReforged.json
                 * 
                 * Rune Example Image: https://ddragon.leagueoflegends.com/cdn/img/perk-images/Styles/Domination/Electrocute/Electrocute.png
                */
                //var currentGame = riotApi.SpectatorV4.GetCurrentGameInfoBySummoner(Region.EUW, summoner.Id);

                //var c = currentGame.Participants;

                //foreach(var a in c)
                //{
                //    Console.WriteLine(RunesIdIntoString(a.Perks.PerkStyle) + " - " + RunesIdIntoString(a.Perks.PerkIds[0]) + " - " + RunesIdIntoString(a.Perks.PerkSubStyle));
                //}
                //var status = "";
                //try
                //{
                //    var currentGame = await riotApi.SpectatorV4.GetCurrentGameInfoBySummonerAsync(Region.EUW, summoner.Id);
                //    status = $"In game {currentGame.GameId}";
                //}
                //catch
                //{
                //    status = "Not in game";
                //}
                #endregion

                #region League Status
                //var elo = riotApi.LolStatusV3.GetShardData(Region.EUW);

                //Console.WriteLine($"Hostname: {elo.Hostname}\n" +
                //    $"Name: {elo.Name}\n" +
                //    $"");
                #endregion

                #region Leaderboards
                //WORKING
                //List<Summoner> list = new List<Summoner>();
                //var leader = riotApi.LeagueV4.GetChallengerLeague(Region.EUW, Queue.RANKED_SOLO_5x5).Entries;
                //var rank = 1;
                //foreach (var a in leader)
                //{

                //    list.Add(new Summoner(a.SummonerName, a.LeaguePoints, a.Wins, a.Losses));
                //    list = list.OrderBy(x => x.LeaguePoints).ToList();
                //    list.Reverse();
                //}
                //list.ForEach(y => Console.WriteLine("{0,3}) {1,-16} {2,10}LP {3,10}W {4,10}L {5,-10}%", rank++, y.SummonerName, y.LeaguePoints, y.Wins, y.Losses,
                //    Convert.ToDouble(String.Format("{0:.##}", ((double)y.Wins / (double)(y.Wins + y.Losses)) * 100))));
                #endregion

                #region embed
                var embed = new EmbedBuilder()
                    .WithTitle($"*{username}'s league profile*")
                    .WithColor(new Color(237, 61, 125))
                    .WithThumbnailUrl(summonerIconURL)
                    .AddField("Elo",
                        $"**Solo/Duo** {soloDuo.Tier} {soloDuo.Rank} **|** {soloDuo.LeaguePoints} LP\n\n" +
                        $"**W/L** {soloDuo.Wins}W / {soloDuo.Losses}L\n" +
                        $"**Win Ratio** {winRatio}%", true)
                    .AddField("Champions", $"{result}", true)
                    //.AddField("Status", status)
                    .AddField("Recent Games", history, true)
                    .WithFooter("Reviewed by " + Context.User.Username, Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync(embed: embed.Build());
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Context.Channel.SendMessageAsync("Error, please contact an administrator.");
            }
        }


        [Command("tft")]
        public async Task GetTFTAsync([Remainder] string username)
        {
            try
            {
                var riotApi = RiotApi.NewInstance(Config.bot.riotToken);

                var summoner = riotApi.SummonerV4.GetBySummonerName(Region.EUW, username);
                var summonerIconURL = $"http://ddragon.leagueoflegends.com/cdn/9.15.1/img/profileicon/{summoner.ProfileIconId}.png";

                var ranks = riotApi.LeagueV4.GetLeagueEntriesForSummoner(Region.EUW, summoner.Id);
                var tft = ranks.FirstOrDefault(xx => xx.QueueType == "RANKED_TFT");

                var winRatio = Convert.ToDouble(String.Format("{0:.##}", ((double)tft.Wins / (double)(tft.Wins + tft.Losses)) * 100));

                #region embed
                var embed = new EmbedBuilder()
                    .WithTitle($"{username}'s league profile")
                    .WithColor(new Color(237, 61, 125))
                    .WithThumbnailUrl(summonerIconURL)
                    .AddField("Elo",
                        $"**TFT** {tft.Tier} {tft.Rank} **|** {tft.LeaguePoints} LP\n\n" +
                        $"**W/L** {tft.Wins}W / {tft.Losses}L\n" +
                        $"**Win Ratio** {winRatio}%", true)
                    .WithFooter("Reviewed by " + Context.User.Username, Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync(embed: embed.Build());
                #endregion
            }
            catch
            {
                await ReplyAsync("User unranked or not found.");
            }
        }

        //public string RunesIdIntoString(long Id)
        //{
        //    string rune = "";
        //    switch(Id)
        //    {
        //        /*
        //         * To-Do:
        //         * 
        //         * Check numbers again since new api gave 5 digits on runes reforged (All)
        //         * Also check perks (vars) 0-6
        //        */
        //        #region Precision
        //        case 8000: rune = "Precision"; break;

        //        //Keystones
        //        case 8005: rune = "Press the Attack"; break;
        //        case 8008: rune = "Lethal Tempo"; break;
        //        case 8010: rune = "Conquerer"; break;
        //        case 8021: rune = "Fleet Footwork"; break;

        //        //Extra Perks
        //        case 9109: rune = "Overheal"; break;
        //        case 9111: rune = "Triumph"; break;
        //        case 8009: rune = "Presence of Mind"; break;

        //        case 9104: rune = "Legend: Alacrity"; break;
        //        case 9105: rune = "Legend: Tenacity"; break;
        //        case 9103: rune = "Legend: Bloodline"; break;

        //        case 8014: rune = "Coup de Grace"; break;
        //        case 8017: rune = "Cut Down"; break;
        //        case 8299: rune = "Last Stand"; break;

        //        #endregion

        //        #region Domination
        //        case 8100: rune = "Domination"; break;

        //        //Keystones
        //        case 8112: rune = "Electrocute"; break;
        //        case 8124: rune = "Predator"; break;
        //        case 8128: rune = "Dark Harvest"; break;
        //        case 9923: rune = "Hail of Blades"; break;

        //        //Extra Perks
        //        case 8126: rune = "Cheap Shot"; break;
        //        case 8139: rune = "Taste of Blood"; break;
        //        case 8143: rune = "Sudden Impact"; break;

        //        case 8136: rune = "Zombie Ward"; break;
        //        case 8120: rune = "Ghost Poro"; break;
        //        case 8138: rune = "Eyeball Collection"; break;

        //        case 8135: rune = "Ravenous Hunter"; break;
        //        case 8134: rune = "Ingenious Hunter"; break;
        //        case 8105: rune = "Relentless Hunter"; break;
        //        case 8106: rune = "Ultimate Hunter"; break;
        //        #endregion

        //        #region Sorcery
        //        case 8200: rune = "Sorcery"; break;

        //        //Keystones
        //        case 8214: rune = "Summon Aery"; break;
        //        case 8229: rune = "Arcane Comet"; break;
        //        case 8230: rune = "Phase Rush"; break;

        //        //Extra Perks
        //        case 8224: rune = "Nullifying Orb"; break;
        //        case 8226: rune = "Manaflow Band"; break;
        //        case 8275: rune = "Nimbus Cloak"; break;

        //        case 8210: rune = "Transcndence"; break;
        //        case 8234: rune = "Celerity"; break;
        //        case 8233: rune = "Absolute Focus"; break;

        //        case 8237: rune = "Scorch"; break;
        //        case 8232: rune = "Waterwalking"; break;
        //        case 8236: rune = "Gathering Storm"; break;
        //        #endregion

        //        #region Resolve
        //        case 8400: rune = "Resolve"; break;

        //        //Keystones
        //        case 8437: rune = "Grasp of the Undying"; break;
        //        case 8439: rune = "Aftershock"; break;
        //        case 8465: rune = "Guardian"; break;

        //        //Extra Perks
        //        case 8446: rune = "Demolish"; break;
        //        case 8462: rune = "Font of Life"; break;
        //        case 8401: rune = "Shield Bash"; break;

        //        case 8429: rune = "Conditioning"; break;
        //        case 8444: rune = "Second Wind"; break;
        //        case 8473: rune = "Bone Plating"; break;

        //        case 8451: rune = "Overgrowth"; break;
        //        case 8453: rune = "Revitalize"; break;
        //        case 8242: rune = "Unflinching"; break;
        //        #endregion

        //        #region Inspiration
        //        case 8300: rune = "Inspiration"; break;

        //        //Keystones
        //        case 8351: rune = "Glacial Augment"; break;
        //        case 8359: rune = "Kleptomancy"; break;
        //        case 8360: rune = "Unsealed Spellbook"; break;

        //        //Extra Perks
        //        case 8306: rune = "Hextech Flashtraption"; break;
        //        case 8304: rune = "Magical Footwear"; break;
        //        case 8313: rune = "Perfect Timing/"; break;

        //        case 8321: rune = "Future's Market"; break;
        //        case 8316: rune = "Minion Dematerializier"; break;
        //        case 8345: rune = "Biscuit Delivery"; break;

        //        case 8347: rune = "Cosmic Insight"; break;
        //        case 8410: rune = "Approach Velocity"; break;
        //        case 8352: rune = "Time Warp Tonic"; break;
        //            #endregion
        //    }
        //    return rune;
        //}
    }
}