using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace MusicBot.Modules
{
    public class Movie : ModuleBase<SocketCommandContext>
    {
        [Command("movie")]
        public async Task SearchMovie([Remainder] string movie)
        {
            TMDbClient client = new TMDbClient(Config.bot.TMDbToken);
            SearchContainer<SearchMovie> movieSearch = client.SearchMovieAsync(movie).Result;

            var result = movieSearch.Results.First();

            var embed = new EmbedBuilder()
                .WithTitle($"{result.Title}")
                .AddField("Description", result.Overview)
                .AddField("Info", $"{result.ReleaseDate.Value.DayOfWeek}, {result.ReleaseDate.Value.Day}.{result.ReleaseDate.Value.Month}.{result.ReleaseDate.Value.Year}")
                .AddField($"** **", $"The Movie DB: [{result.Title}](https://www.themoviedb.org/movie/{result.Id})")
                .WithColor(new Color(237, 61, 125))
                .WithThumbnailUrl($"http://image.tmdb.org/t/p/w500{result.PosterPath}");

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
