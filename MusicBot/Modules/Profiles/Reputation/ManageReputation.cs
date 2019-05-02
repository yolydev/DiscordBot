using Discord.Commands;
using Discord.WebSocket;
using MusicBot.Core.UserAccounts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Modules.Profiles.Reputation
{
    public class ManageReputation : ModuleBase<SocketCommandContext>
    {
        //Add emote to Reputation message and cooldown

        [Command("+rep")]
        public async Task GiveRep([Remainder] SocketGuildUser user)
        {
            var userAccount = UserAccounts.GetAccount(user);

            if (user.Username == Context.User.Username)
            {
                await Context.Channel.SendMessageAsync("**" + user.Username + "** did you just try to give yourself reputation? Get some friends ..");
                return;
            }

            userAccount.Reputation += 1;
            UserAccounts.SaveAccounts();
            await Context.Channel.SendMessageAsync("**" + user.Username + "** just got reputation from " + Context.User.Username + "!");
        }

        [Command("-rep")]
        public async Task TakeRep([Remainder] SocketGuildUser user)
        {
            var userAccount = UserAccounts.GetAccount(user);

            if (userAccount.Reputation == 0)
            {
                await Context.Channel.SendMessageAsync("**" + user.Username + "** has already 0 reputation, don't make him or her go even lower ..");
                return;
            }

            if (user.Username == Context.User.Username)
            {
                await Context.Channel.SendMessageAsync("**" + user.Username + "** why would you remove reputation from yourself? That's not how it works.");
                return;
            }

            userAccount.Reputation -= 1;
            UserAccounts.SaveAccounts();

            await Context.Channel.SendMessageAsync("**" + user.Username + "** just had reputation removed from " + Context.User.Username + "!");
        }
    }
}
