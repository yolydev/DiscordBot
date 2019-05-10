using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace MusicBot
{
    public class Status : ModuleBase<SocketCommandContext>
    {

        [Command("status")]
        public async Task GetStatus()
        {
            await ReplyAsync($"Stable ..\nSending log ..");
        }
    }
}
