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

namespace MusicBot.Modules
{
    public class League : ModuleBase<SocketCommandContext>
    {
        readonly RiotApi api = RiotApi.NewInstance(Config.bot.riotToken);

        [Command("elo")]
        public async Task GetElo([Remainder]string username)
        {
            SocketGuildUser user = (SocketGuildUser)Context.User;
            var userAvatar = user.GetAvatarUrl();

            var embed = new EmbedBuilder();

            try
            {
                var summoner = api.SummonerV4.GetBySummonerName(Region.EUW, username);

                //Summoner Basic Inforamtion

                var summonerName = summoner.Name;
                var summonerId = summoner.Id;
                var summonerLevel = summoner.SummonerLevel;
                var summonerIconURL = "http://avatar.leagueoflegends.com/euw/" + summonerName.Replace(' ', '+') + ".png";

                //Summoner League Information

                var summonerLeague = api.LeagueV4.GetAllLeaguePositionsForSummoner(Region.EUW, summonerId);
                var tier = "";
                var rank = "";
                var leagueName = "";
                var leaguePoints = 0;
                int wins = 0;
                int losses = 0;
                int total = 0;
                double ratio = 0;

                var tier_flex5v5 = "";
                var rank_flex5v5 = "";
                var leagueName_flex5v5 = "";
                var leaguePoints_flex5v5 = 0;

                var tier_flex3v3 = "";
                var rank_flex3v3 = "";
                var leagueName_flex3v3 = "";
                var leaguePoints_flex3v3 = 0;

                foreach (var summonerElo in summonerLeague)
                {
                    if (summonerElo.QueueType == "RANKED_SOLO_5x5")
                    {
                        tier = summonerElo.Tier;
                        rank = summonerElo.Rank;
                        leagueName = summonerElo.LeagueName;
                        leaguePoints = summonerElo.LeaguePoints;
                        wins = summonerElo.Wins;
                        losses = summonerElo.Losses;
                    }

                    if (summonerElo.QueueType == "RANKED_FLEX_5x5")
                    {
                        tier_flex5v5 = summonerElo.Tier;
                        rank_flex5v5 = summonerElo.Rank;
                        leagueName_flex5v5 = summonerElo.LeagueName;
                        leaguePoints_flex5v5 = summonerElo.LeaguePoints;
                    }

                    if (summonerElo.QueueType == "RANKED_FLEX_TT")
                    {
                        tier_flex3v3 = summonerElo.Tier;
                        rank_flex3v3 = summonerElo.Rank;
                        leagueName_flex3v3 = summonerElo.LeagueName;
                        leaguePoints_flex3v3 = summonerElo.LeaguePoints;
                    }
                }

                if (tier == "")
                    tier = "Unranked";

                if (tier_flex5v5 == "")
                    tier_flex5v5 = "Unranked ";

                if(tier_flex3v3 == "")
                    tier_flex3v3 = "Unranked";

                var masteries = api.ChampionMasteryV4.GetAllChampionMasteries(Region.EUW, summoner.Id);
                var result = "";
                for (var i = 0; i < 5; i++)
                {
                    var mastery = masteries[i];
                    var champ = (Champion)mastery.ChampionId;
                    result += $"**{champ.Name()}** {mastery.ChampionPoints} ({mastery.ChampionLevel})\n";
                }

                total = wins + losses;
                ratio = Convert.ToDouble(String.Format("{0:.##}", ((double)wins / (double)total) * 100));

                embed.WithTitle("**" + username + "'s League Information**");
                embed.WithThumbnailUrl(summonerIconURL);
                embed.AddField("Profile", $"**Solo/Duo: **{tier} {rank} **with** {leaguePoints}LP\n" +
                    $"**Flex5v5: **{tier_flex5v5} {rank_flex5v5} **with** {leaguePoints_flex5v5}LP\n" +
                    $"**Flex3v3: **{tier_flex3v3} {rank_flex3v3} **with** {leaguePoints_flex3v3}LP\n" +
                    $"**Win/Loss: **{wins}W / {losses}L\n **Win Ratio: **{ratio}%\n **League: **{leagueName}\n", true);
                embed.AddField("Top Champions", result, true);
                embed.AddField("** **", "** **");
                
                var matchList = await api.MatchV4.GetMatchlistAsync(Region.EUW, summoner.AccountId, queue: new[] { 420 }, endIndex: 5);
                var matchDataTasks = matchList.Matches.Select(matchMetaData => api.MatchV4.GetMatchAsync(Region.EUW, matchMetaData.GameId)).ToArray();
                var matchDatas = await Task.WhenAll(matchDataTasks);
                var history = "";
                for(var i = 0; i < matchDatas.Count(); i++)
                {
                    var matchData = matchDatas[i];
                    var participantIdData = matchData.ParticipantIdentities
                        .First(pi => summoner.Id.Equals(pi.Player.SummonerId));
                    var participant = matchData.Participants.First(p => p.ParticipantId == participantIdData.ParticipantId);

                    var win = participant.Stats.Win;
                    var champ = (Champion)participant.ChampionId;
                    var k = participant.Stats.Kills;
                    var d = participant.Stats.Deaths;
                    var a = participant.Stats.Assists;
                    var kda = (k + a) / (float)d;
                    history += win ? $"Win - {champ.Name()}: {k}/{d}/{a}\n" : $"Loss - {champ.Name()}: {k}/{d}/{a}\n";
                }
                embed.AddField("Recent Games", history, true);

                long gameID = 0;
                long mapID = 0;
                try
                {
                    var currentGame = api.SpectatorV4.GetCurrentGameInfoBySummoner(Region.EUW, summonerId);
                    gameID = currentGame.GameId;
                    mapID = currentGame.MapId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (gameID is 0)
                {
                    embed.AddField(summonerName + " currently not in game", "-", true);
                }
                else
                {
                    if (mapID is 11)
                        embed.AddField(summonerName + " currently in game.", "Summoner's Rift", true);

                    if (mapID is 10)
                        embed.AddField(summonerName + " currently in game.", "Twisted Treeline", true);

                    if (mapID is 12)
                        embed.AddField(summonerName + " currently in game.", "Howling Abyss", true);
                }

                embed.WithFooter("Reviewed by " + Context.User.Username, Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
