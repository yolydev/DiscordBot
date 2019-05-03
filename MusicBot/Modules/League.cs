using System;
using System.Collections.Generic;
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
                Console.WriteLine(summonerIconURL);

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

                var tier_flex = "";
                var rank_flex = "";
                var leagueName_flex = "";
                var leaguePoints_flex = 0;

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
                        tier_flex = summonerElo.Tier;
                        rank_flex = summonerElo.Rank;
                        leagueName_flex = summonerElo.LeagueName;
                        leaguePoints_flex = summonerElo.LeaguePoints;
                    }
                }
                //if (rank_flex is null || tier_flex is null)
                //    tier_flex = "Unranked ";

                if (tier == "")
                    tier = "Unranked";

                if (tier_flex == "")
                    tier_flex = "Unranked ";

                var masteries = api.ChampionMasteryV4.GetAllChampionMasteries(Region.EUW, summoner.Id);
                var result = "";
                for (var i = 0; i < 5; i++)
                {
                    var mastery = masteries[i];
                    var champ = (Champion)mastery.ChampionId;
                    result += $"**{champ.Name()}** {mastery.ChampionPoints} ({mastery.ChampionLevel})\n";
                }

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

                total = wins + losses;
                ratio = Convert.ToDouble(String.Format("{0:.##}", ((double)wins / (double)total) * 100));

                embed.WithTitle("**" + username + "'s League Information**");
                embed.WithThumbnailUrl(summonerIconURL);
                embed.AddField("Profile", $"**Solo/Duo: **{tier} {rank} **with** {leaguePoints}LP\n**Flex5v5: **{tier_flex} {rank_flex} **with** {leaguePoints_flex}LP \n**Win/Loss: **{wins}W / {losses}L\n **Win Ratio: **{ratio}%\n **League: **{leagueName}\n", true);
                embed.AddField("Top Champions", result, true);
                embed.AddField("** **", "** **");
                if (gameID is 0)
                {
                    embed.AddField(summonerName + " currently not in game", "-");
                }
                else
                {
                    if (mapID is 11)
                        embed.AddField(summonerName + " currently in game.", "Summoner's Rift");

                    if (mapID is 10)
                        embed.AddField(summonerName + " currently in game.", "Twisted Treeline");

                    if (mapID is 12)
                        embed.AddField(summonerName + " currently in game.", "Howling Abyss");
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
