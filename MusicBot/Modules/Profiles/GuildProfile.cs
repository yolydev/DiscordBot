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
                roleString += guilds.Mention + ", ";
            }

            var guildIcon = guild.IconUrl;

            var embed = new EmbedBuilder()
                .WithTitle("**ABOUT THIS SERVER**")
                .AddField("Name", guild.Name)
                .AddField("Server stats", guild.MemberCount + " members chatting in\n" + textChannel + " " + guild.TextChannels.Count + " Text Channel/s &\n" + voiceChannel + " " + guild.VoiceChannels.Count + " Voice Channel/s.", true)
                .AddField("Owner", guild.Owner.Username, true)
                .AddField("Roles", roleString)
                .AddField(creationDate + " Creation date", guild.CreatedAt.DayOfWeek + ", " + guild.CreatedAt.LocalDateTime, true)
                .WithColor(new Color(237, 61, 125))
                .WithThumbnailUrl(guildIcon);

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
