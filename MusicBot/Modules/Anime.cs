using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JikanDotNet;

namespace MusicBot.Modules
{
    public class Anime : ModuleBase<SocketCommandContext>
    {
        [Command("anime")]
        public async Task SearchAnime([Remainder] string anime)
        {
            IJikan jikan = new Jikan(true);
            AnimeSearchResult animeSearch = await jikan.SearchAnime(anime);

            var result = animeSearch.Results.First();

            var embed = new EmbedBuilder()
                .WithTitle($"{result.Title}")
                .AddField("Description", result.Description)
                .AddField("Info", $"**{result.Episodes}** Episodes - {result.StartDate.Value.DayOfWeek}, {result.StartDate.Value.Day}.{result.StartDate.Value.Month}.{result.StartDate.Value.Year} **to** {result.EndDate.Value.DayOfWeek}, {result.EndDate.Value.Day}.{result.EndDate.Value.Month}.{result.EndDate.Value.Year}")
                .AddField($"** **", $"My Anime List: [{result.Title}]({result.URL})")
                .WithColor(new Color(237, 61, 125))
                .WithThumbnailUrl(result.ImageURL);

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
