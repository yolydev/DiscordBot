using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Modules
{
    public class Images : ModuleBase<SocketCommandContext>
    {
        private const string imageFolder = "Resources/Images/gifs";
        private const string gif = ".gif";

        [Command("gif")]
        public async Task GetImage()
        {
            Random random = new Random();
            int rng = random.Next(1, 150);

            await Context.Channel.SendFileAsync(imageFolder + "/" + rng + "" + gif);
            Console.WriteLine("gif sent -> " + rng);
        }
    }
}
