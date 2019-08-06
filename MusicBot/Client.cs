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
using MusicBot.Modules;

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
            //await _services.GetRequiredService<MusicService>().InitializeAsync();
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

            Console.WriteLine(header, Color.Plum);
            Console.WriteLine(lineBreak, Color.LightCoral);
            Console.Write("     Runtime: ", Color.Plum);
            Console.Write($"{RuntimeInformation.FrameworkDescription}\n", Color.White);
            Console.Write("     Process: ", Color.Plum);
            Console.Write($"{process.Id} ID | {process.Threads.Count} Threads\n", Color.White);
            Console.Write("          OS: ", Color.Plum);
            Console.Write($"{RuntimeInformation.OSDescription} | {RuntimeInformation.ProcessArchitecture}\n", Color.White);
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
            //.AddSingleton<MusicService>()
            .BuildServiceProvider();

        private async Task LogAsync(LogMessage logMessage)
        {
            var date = $"[{DateTimeOffset.Now:MMM d - hh:mm:ss}]";

            Console.Write("{0,-20}",$"{date}: ", Color.DarkGray);
            Console.Write("{0,-12}",$"[{logMessage.Severity}] ", await SeverityColor(logMessage.Severity));
            Console.Write("{0,-14}",$"{ConvertSource(logMessage.Source)} ", Color.DarkGray);
            Console.Write("{0,-15}",$"{logMessage.Message} ", Color.White);
            Console.Write("\n");
            //Console.WriteLine(String.Format("{0,-20} {1,-10} {2,-10} {3,-10}", $"{date}", $"[{logMessage.Severity}]", $"{logMessage.Source}", $"{logMessage.Message}"));
            //return Task.CompletedTask;
        }

        private string ConvertSource(string source)
        {
            switch (source.ToLower())
            {
                case "discord":
                    return "Discord";
                case "gateway":
                    return "Gateway";
                case "command":
                    return "Command";
                case "rest":
                    return "RestSer";
                default:
                    return source;
            }
        }

        private Task<Color> SeverityColor(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical:
                    return Task.FromResult(Color.Red);
                case LogSeverity.Error:
                    return Task.FromResult(Color.DarkRed);
                case LogSeverity.Warning:
                    return Task.FromResult(Color.Yellow);
                case LogSeverity.Info:
                    return Task.FromResult(Color.DarkSeaGreen);
                default:
                    return Task.FromResult(Color.LawnGreen);
            }
        }
    }
}
