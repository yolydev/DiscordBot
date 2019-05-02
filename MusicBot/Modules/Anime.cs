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
    public class MyAnimeList : ModuleBase<SocketCommandContext>
    {
        [Command("anime")]
        public async Task SearchAnime([Remainder] string anime)
        {
            IJikan jikan = new Jikan(true);

            AnimeSearchResult animeSearch = await jikan.SearchAnime(anime);
            
            var animeTitle = animeSearch.Results.First().Title;
            var animeURL = animeSearch.Results.First().URL;
            //var animeMALId = a.Results.First().MalId;
            var animeDesc = animeSearch.Results.First().Description;
            var animeImageURL = animeSearch.Results.First().ImageURL;


            var embed = new EmbedBuilder()
                .WithTitle($"{animeTitle}")
                .AddField("Description", animeDesc)
                .AddField($"** **", $"My Anime List: [{animeTitle}]({animeURL})")
                .WithColor(new Color(237, 61, 125))
                .WithThumbnailUrl(animeImageURL);

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
