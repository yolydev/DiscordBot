using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MusicBot.Services;
using Victoria;
using Console = Colorful.Console;
using Color = System.Drawing.Color;

namespace MusicBot
{
    public class Client
    {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private IServiceProvider _services;
                
        public Client(DiscordSocketClient client = null, CommandService cmdService = null)
        {
            _client = client ?? new DiscordSocketClient(new DiscordSocketConfig {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Debug
            });

            _cmdService = cmdService ?? new CommandService(new CommandServiceConfig {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            });
        }

        public async Task InitializeAsync()
        {
            await _client.LoginAsync(TokenType.Bot, Config.bot.discordToken); 
            await _client.StartAsync();

            InitializeConsole();
            HookEvents();
            _services = SetupServices();

            var cmdHandler = new CommandHandler(_client, _cmdService, _services);

            await cmdHandler.InitializeAsync();
            await _services.GetRequiredService<MusicService>().InitializeAsync();
            await Task.Delay(-1);
        }

        private void InitializeConsole()
        {
            const string header = @"
            █▀▀█ █▀▄▀█   █▀▀▄ █░░█ █▀▀▄ █▀▀ █▀▀
            █░░█ █░▀░█   █░░█ █░░█ █░░█ █▀▀ ▀▀█
            █▀▀▀ ▀░░░▀   ▀░░▀ ░▀▀▀ ▀▀▀░ ▀▀▀ ▀▀▀";
            var lineBreak = $"\n{new string('-', 90)}\n";
            var process = Process.GetCurrentProcess();

            Console.WriteLine(header, Color.Teal);
            Console.WriteLine(lineBreak, Color.LightCoral);
            Console.Write("     Runtime: ", Color.Plum);
            Console.Write($"{RuntimeInformation.FrameworkDescription}\n");
            Console.Write("     Process: ", Color.Plum);
            Console.Write($"{process.Id} ID | {process.Threads.Count} Threads\n");
            Console.Write("          OS: ", Color.Plum);
            Console.Write($"{RuntimeInformation.OSDescription} | {RuntimeInformation.ProcessArchitecture}\n");
            Console.WriteLine(lineBreak, Color.LightCoral);
        }

        private void HookEvents()
        {
            _client.Ready += OnReady;
            _client.Log += LogAsync;
        }

        private async Task OnReady()
        {
            await _client.SetGameAsync("ur mom nigga", "https://www.twitch.tv/ehasywhin", ActivityType.Streaming);
        }

        private IServiceProvider SetupServices()
            => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_cmdService)
            .AddSingleton<LavaRestClient>()
            .AddSingleton<LavaSocketClient>()
            .AddSingleton<MusicService>()
            .BuildServiceProvider();

        //Logging

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine($" {logMessage.Message}");
            return Task.CompletedTask;
        }

        //private string ConvertSource(string source)
        //{
        //    switch (source.ToLower())
        //    {
        //        case "discord":
        //            return $"Discord";
        //        case "gateway":
        //            return "Gateway";
        //        case "command":
        //            return "Command";
        //        case "rest":
        //            return "RestSer";
        //        default:
        //            return source;
        //    }
        //}

        //private Task<Color> SeverityColor(LogSeverity severity)
        //{
        //    switch (severity)
        //    {
        //        case LogSeverity.Critical:
        //            return Task.FromResult(Color.Red);
        //        case LogSeverity.Error:
        //            return Task.FromResult(Color.DarkRed);
        //        case LogSeverity.Warning:
        //            return Task.FromResult(Color.Yellow);
        //        case LogSeverity.Info:
        //            return Task.FromResult(Color.LightGreen);
        //        default:
        //            return Task.FromResult(Color.Lime);
        //    }
        //}
    }
}
