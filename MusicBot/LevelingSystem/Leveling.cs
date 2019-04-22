using Discord.WebSocket;
using MusicBot.Core.UserAccounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicBot.LevelingSystem
{
    internal static class Leveling
    {
        internal static async void MessageCounter(SocketGuildUser user, SocketTextChannel channel, SocketUserMessage message)
        {
            Random rngXp = new Random();
            var userAccount = UserAccounts.GetAccount(user);

            uint oldLevel = userAccount.Level;
            uint xpGain = (uint)rngXp.Next(0, 101);

            userAccount.XP += xpGain;
            UserAccounts.SaveAccounts();

            uint newLevel = userAccount.Level;

            if (oldLevel != newLevel)
            {
                if (channel.Id == 513494298801733637)
                    await channel.SendMessageAsync(user.Mention + " just leveled up to level " + newLevel + "!");
            }
            Console.WriteLine(user.Username + " -> (" + xpGain + "xp) :" + message + " <" + channel.Name + ">");
        }
    }
}
