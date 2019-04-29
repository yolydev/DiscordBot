using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Modules.Profiles
{
    public class GuildProfile : ModuleBase<SocketCommandContext>
    {
        private Emoji textChannel = new Emoji("📝");
        private Emoji voiceChannel = new Emoji("📣");
        private Emoji creationDate = new Emoji("📆");

        [Command("guild")]
        public async Task GetGuildProfile()
        {
            var guild = Context.Guild;
            var roleString = "";
            foreach (var guilds in guild.Roles)
            {
                roleString += guilds.Name + ", ";
            }

            var embed = new EmbedBuilder();
            embed.WithTitle("**ABOUT THIS SERVER**");
            embed.AddField("Name", guild.Name);
            embed.AddField("Server stats", guild.MemberCount + " members chatting in\n" + textChannel + " " + guild.TextChannels.Count + " Text Channel/s &\n" + voiceChannel + " " + guild.VoiceChannels.Count + " Voice Channel/s.", true);
            embed.AddField("Owner", guild.Owner.Username, true);
            embed.AddField("Roles", roleString);
            embed.AddField(creationDate + " Creation date", guild.CreatedAt.DayOfWeek + ", " + guild.CreatedAt.LocalDateTime, true);
            embed.WithColor(new Color(237, 61, 125));

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
