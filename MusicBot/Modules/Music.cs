using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MusicBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;
using Console = Colorful.Console;
using Color = System.Drawing.Color;


namespace MusicBot.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {

        //private LavaRestClient _lavaRestClient;
        //private LavaSocketClient _lavaSocketClient;
        //private DiscordSocketClient _client;
        //private LavaPlayer player;

        //public Music(LavaRestClient lavaRestClient, LavaSocketClient lavaSocketClient, DiscordSocketClient client)
        //{
        //    _client = client;
        //    _lavaRestClient = lavaRestClient;
        //    _lavaSocketClient = lavaSocketClient;
        //}

        //public Task InitializeAsync()
        //{
        //    _client.Ready += ClientReadyAsync;
        //    _lavaSocketClient.Log += LogAsync;
        //    _lavaSocketClient.OnTrackFinished += TrackFinished;
        //    return Task.CompletedTask;
        //}

        //public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
        //    => await _lavaSocketClient.ConnectAsync(voiceChannel, textChannel);

        //public async Task DisconnectAsync(SocketVoiceChannel voiceChannel)
        //    => await _lavaSocketClient.DisconnectAsync(voiceChannel);


        //[Command("Join")]
        //public async Task JoinAsync()
        //{
        //    var user = Context.User as SocketGuildUser;
        //    await _lavaSocketClient.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
        //    await ReplyAsync("Connected");
        //}

        //[Command("Leave")]
        //public async Task LeaveAsync()
        //{
        //    var user = Context.User as SocketGuildUser;
        //    await _lavaSocketClient.DisconnectAsync(user.VoiceChannel);
        //    await ReplyAsync("Disconnected");
        //}

        //[Command("Move")]
        //public async Task MoveAsync()
        //{
        //    var user = Context.User as SocketGuildUser;
        //    await _lavaSocketClient.DisconnectAsync(user.VoiceChannel);
        //    await _lavaSocketClient.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
        //    await ReplyAsync($"Moved to {user.VoiceChannel.Name}!");
        //}

        //[Command("Play")]
        //public async Task PlayAsync([Remainder]string query)
        //{
        //    var results = await _lavaRestClient.SearchYouTubeAsync(query);

        //    if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
        //    {
        //        await ReplyAsync("Nothing found.");
        //        return;
        //    }

        //    var track = results.Tracks.FirstOrDefault();

        //    if (player.IsPlaying)
        //    {
        //        player.Queue.Enqueue(track);
        //        await ReplyAsync($"You added » **{track.Title}** to the queue.");
        //    }
        //    else
        //    {
        //        await player.PlayAsync(track);
        //        await ReplyAsync($"Bot's now playing » **{track.Title}**");
        //    }
        //}

        //[Command("Skip")]
        //public async Task SkipAsync()
        //{
        //    try
        //    {
        //        var skipped = await player.SkipAsync();
        //        await ReplyAsync($"Skipped: {skipped.Title}\nNow Playing: {player.CurrentTrack.Title}");
        //    }
        //    catch
        //    {
        //        await ReplyAsync("Can't skip, queue is empty.");
        //    }
        //}

        ////Supporter 
        //private async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        //{
        //    if (!reason.ShouldPlayNext())
        //        return;

        //    if (!player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
        //    {
        //        await player.TextChannel.SendMessageAsync("There are no more songs in the queue.");
        //        return;
        //    }

        //    await player.PlayAsync(nextTrack);
        //    await player.TextChannel.SendMessageAsync($"Bot's now playing » **{nextTrack.Title}**");
        //}

        //private async Task ClientReadyAsync()
        //{
        //    await _lavaSocketClient.StartAsync(_client, new Configuration
        //    {
        //        LogSeverity = LogSeverity.Info
        //    });
        //}

        //private async Task LogAsync(LogMessage logMessage)
        //{
        //    var date = $"[{DateTimeOffset.Now:MMM d - hh:mm:ss}]";

        //    Console.Write("{0,-20}", $"{date}: ", Color.DarkGray);
        //    Console.Write("{0,-12}", $"[{logMessage.Severity}] ", await SeverityColor(logMessage.Severity));
        //    Console.Write("{0,-14}", $"{ConvertSource(logMessage.Source)} ", Color.DarkGray);
        //    Console.Write("{0,-15}", $"{logMessage.Message} ", Color.White);
        //    Console.Write("\n");
        //}

        //private string ConvertSource(string source)
        //{
        //    switch (source.ToLower())
        //    {
        //        case "discord":
        //            return "Discord";
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
        //            return Task.FromResult(Color.DarkSeaGreen);
        //        default:
        //            return Task.FromResult(Color.LawnGreen);
        //    }
        //}


            //NEWW

        //private MusicService _musicService;

        //public Music(MusicService musicService)
        //{
        //    _musicService = musicService;
        //}

        //[Command("join")]
        //public async Task Join()
        //{
        //    var user = Context.User as SocketGuildUser;
        //    if (user.VoiceChannel is null)
        //    {
        //        await ReplyAsync("You must be connected to a voice channel.");
        //        return;
        //    }
        //    else
        //    {
        //        await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
        //        await ReplyAsync($"Bot now connected to **{user.VoiceChannel.Name}** voice channel.");
        //    }
        //}

        //[Command("move")]
        //public async Task Move()
        //{
        //    var user = Context.User as SocketGuildUser;
        //    if (user.VoiceChannel is null)
        //    {
        //        await ReplyAsync("You must be connected to a voice channel.");
        //        return;
        //    }
        //    else
        //    {
        //        await _musicService.DisconnectAsync(user.VoiceChannel);
        //        await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
        //        await ReplyAsync($"Bot moved to **{user.VoiceChannel.Name}** voice channel.");
        //    }
        //}

        //[Command("leave")]
        //public async Task Leave()
        //{
        //    var user = Context.User as SocketGuildUser;
        //    if (user.VoiceChannel is null)
        //    {
        //        await ReplyAsync("You need to be in the Bot's voice channel.");
        //    }
        //    else
        //    {
        //        await _musicService.DisconnectAsync(user.VoiceChannel);
        //        await ReplyAsync($"Bot left **{user.VoiceChannel.Name}** voice channel.");
        //    }
        //}

        //[Command("play")]
        //public async Task Play([Remainder]string query)
        //    => await ReplyAsync(await _musicService.PlayAsync(query, Context.Guild.Id));

        //[Command("pause")]
        //public async Task Pause()
        //    => await ReplyAsync(await _musicService.PauseAsync());

        //[Command("resume")]
        //public async Task Resume()
        //    => await ReplyAsync(await _musicService.ResumeAsync());

        //[Command("stop")]
        //public async Task Stop()
        //{
        //    await _musicService.StopAsync();
        //    await ReplyAsync("Bot's stopped playing music.");
        //}

        //[Command("skip")]
        //public async Task Skip()
        //    => await ReplyAsync(await _musicService.SkipAsync());

        //[Command("volume"), Alias("vol")]
        //public async Task Volume(int vol)
        //    => await ReplyAsync(await _musicService.SetVolumeAsync(vol));

        //[Command("songinfo"), Alias("current")]
        //public async Task Info()
        //    => await ReplyAsync(_musicService.Info());

        //[Command("queue")]
        //public async Task ShowQueue()
        //    => await ReplyAsync(_musicService.ShowQueue());

        //[Command("clearqueue"), Alias("cq")]
        //public async Task ClearQueue()
        //    => await ReplyAsync(_musicService.ClearQueue());
    }
}
