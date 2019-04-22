using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MusicBot.Core.UserAccounts;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Modules.Profiles
{
    public class UserProfile : ModuleBase<SocketCommandContext>
    {
        private Emoji userName = new Emoji("🏷");
        private Emoji userRoles = new Emoji("👪");
        private Emoji userJoinedDate = new Emoji("📆");
        private Emoji userRep = new Emoji("⚜");
        private Emoji accountCreated = new Emoji("🔞");

        [Command("profile")]
        public async Task GetUserProfile()
        {
            SocketGuildUser user = (SocketGuildUser)Context.User;
            var userAccount = UserAccounts.GetAccount(user);

            var b = user.Roles;
            var c = "";
            foreach (var roles in b)
            {
                if (user.Roles.Count == 1)
                    c = "None";
                else
                    c += roles.Name + "\n";
            }

            var userAvatar = user.GetAvatarUrl();

            var embed = new EmbedBuilder();
            embed.WithTitle("**YOUR AWESOME PROFILE**");
            embed.AddField(userName + " Name", user.Username);
            embed.AddField(userRoles + " Roles", c.Replace("@everyone", string.Empty), true);
            embed.AddField(userRep + " Reputation", userAccount.Reputation, true);
            embed.AddField(accountCreated + " Account created", user.CreatedAt.DayOfWeek + ", " + user.CreatedAt.LocalDateTime);
            embed.AddField(userJoinedDate + " Joined At", user.JoinedAt.Value.DayOfWeek + ", " +user.JoinedAt.Value.LocalDateTime);
            embed.WithColor(new Color(237, 61, 125));
            embed.WithThumbnailUrl(userAvatar);

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("profile")]
        public async Task GetUserProfile([Remainder]SocketGuildUser userMention)
        {
            var userAccount = UserAccounts.GetAccount(userMention);

            var b = userMention.Roles;
            var c = "";
            foreach (var roles in b)
            {
                if (userMention.Roles.Count == 1)
                    c = "None";
                else
                    c += roles.Name + "\n";
            }

            var userAvatar = userMention.GetAvatarUrl();

            var embed = new EmbedBuilder();
            embed.WithTitle("**YOUR AWESOME PROFILE**");
            embed.AddField(userName + " Name", userMention.Username);
            embed.AddField(userRoles + " Roles", c.Replace("@everyone", string.Empty), true);
            embed.AddField(userRep + " Reputation", userAccount.Reputation, true);
            embed.AddField(accountCreated + " Account created", userMention.CreatedAt.UtcDateTime + " UTC");
            embed.AddField(userJoinedDate + " Joined At", userMention.JoinedAt.ToString().Replace("+00:00", "UTC"));
            embed.WithColor(new Color(237, 61, 125));
            embed.WithThumbnailUrl(userAvatar);

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
