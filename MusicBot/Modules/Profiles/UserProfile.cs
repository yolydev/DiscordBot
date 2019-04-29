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
        public async Task GetUserProfile([Remainder]SocketGuildUser userMention = null)
        {
            userMention = userMention ?? (SocketGuildUser)Context.User;

            var userAccount = UserAccounts.GetAccount(userMention);

            var userRoles = userMention.Roles;
            var allUserRoles = "";
            foreach (var roles in userRoles)
            {
                if (userMention.Roles.Count == 1)
                    allUserRoles = "None";
                else
                    allUserRoles += roles.Name + "\n";
            }

            var userAvatar = userMention.GetAvatarUrl();

            var embed = new EmbedBuilder();
            embed.WithTitle("**YOUR AWESOME PROFILE**");
            embed.AddField(userName + " Name", userMention.Username);
            embed.AddField(this.userRoles + " Roles", allUserRoles.Replace("@everyone", string.Empty), true);
            embed.AddField(userRep + " Reputation", userAccount.Reputation, true);
            embed.AddField(accountCreated + " Account created", userMention.CreatedAt.DayOfWeek + ", " + userMention.CreatedAt.LocalDateTime);
            embed.AddField(userJoinedDate + " Joined At", userMention.JoinedAt.Value.DayOfWeek + ", " + userMention.JoinedAt.Value.LocalDateTime);
            embed.WithColor(new Color(237, 61, 125));
            embed.WithThumbnailUrl(userAvatar);

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
