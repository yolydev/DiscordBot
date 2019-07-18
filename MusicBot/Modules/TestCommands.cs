using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Modules
{
    public class TestCommands : ModuleBase<SocketCommandContext>
    {
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("welcome"), Alias("w")]
        public async Task WelcomeMessage()
        {
            var user = Context.User as SocketGuildUser;

            var embed = new EmbedBuilder()
               .WithTitle($"Welcome **{Context.User.Username}#{Context.User.DiscriminatorValue}** to the server **{Context.Guild.Name}**!")
               .AddField("**Name**", $"{Context.User.Username}", true)
               .AddField("**Mutual Guilds**", $"{Context.User.MutualGuilds.Count}", true)
               .AddField("**Joined Date**", $"{user.JoinedAt.Value.DayOfWeek} {user.JoinedAt.Value.LocalDateTime}")
               .WithFooter($"There are now currently **{Context.Guild.MemberCount} User** on the server!")
               .WithColor(new Color(237, 61, 125))
               .WithThumbnailUrl(Context.User.GetAvatarUrl());
            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
