using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MusicBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, CommandService cmdService, IServiceProvider services)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _cmdService.Log += LogAsync;
            _client.UserJoined += AnnounceUserJoin;
            _client.MessageReceived += HandleMessageAsync;
        }


        private async Task AnnounceUserJoin(SocketGuildUser user)
        {
            var channel = _client.GetChannel(494934565337694222) as SocketTextChannel;

            var embed = new EmbedBuilder()
               .WithTitle($"Welcome **{user.Username}#{user.DiscriminatorValue}** to the server **{user.Guild.Name}**!")
               .AddField("**Name**", $"{user.Username}", true)
               .AddField("**Mutual Guilds**", $"{user.MutualGuilds.Count}", true)
               .AddField("**Joined Date**", $"{user.JoinedAt.Value.DayOfWeek} {user.JoinedAt.Value.LocalDateTime}")
               .WithFooter($"There are now currently {user.Guild.MemberCount} User on the server!")
               .WithColor(new Color(237, 61, 125))
               .WithThumbnailUrl(user.GetAvatarUrl());
            await channel.SendMessageAsync(embed: embed.Build());
        }

        private async Task HandleMessageAsync(SocketMessage msg)
        {
            //Console.WriteLine(msg.Author + ": " + msg.Content);
            SaveLog(msg);

            var argPos = 0;
            if (msg.Author.IsBot) return;

            var userMessage = msg as SocketUserMessage;
            if (userMessage is null)
                return;

            if (!userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
                return;

            var context = new SocketCommandContext(_client, userMessage);
            var result = await _cmdService.ExecuteAsync(context, argPos, _services);
        }

        private void SaveLog(SocketMessage msg)
        {
            var dateTime = DateTime.Now;
            var guildChannel = msg.Channel as SocketGuildChannel;

            var file = $"logs/{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt";
            if (!File.Exists(file))
                File.Create(file);
            //if (!File.Exists("log.txt"))
            //    File.Create("log.txt");

            File.AppendAllText(file, $"[{dateTime}] ({guildChannel.Guild.Name}) #{msg.Channel} - {msg.Author}: {msg.Content}" + Environment.NewLine);
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
