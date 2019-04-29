using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;

namespace MusicBot.Services
{
    public class MusicService
    {
        private LavaRestClient _lavaRestClient;
        private LavaSocketClient _lavaSocketClient;
        private DiscordSocketClient _client;
        private LavaPlayer _player;

        public MusicService(LavaRestClient lavaRestClient, LavaSocketClient lavaSocketClient, DiscordSocketClient client)
        {
            _client = client;
            _lavaRestClient = lavaRestClient;
            _lavaSocketClient = lavaSocketClient;
        }

        public Task InitializeAsync()
        {
            _client.Ready += ClientReadyAsync;
            _lavaSocketClient.Log += LogAsync;
            _lavaSocketClient.OnTrackFinished += TrackFinished;
            return Task.CompletedTask; 
        }

        public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
            => await _lavaSocketClient.ConnectAsync(voiceChannel, textChannel);

        public async Task DisconnectAsync(SocketVoiceChannel voiceChannel)
            => await _lavaSocketClient.DisconnectAsync(voiceChannel);

        public async Task<string> PlayAsync(string query, ulong guildId)
        {
            _player = _lavaSocketClient.GetPlayer(guildId);
            var results = await _lavaRestClient.SearchYouTubeAsync(query);

            if(results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
            {
                return "No matches found.";
            }

            var track = results.Tracks.FirstOrDefault();

            if(_player.IsPlaying)
            {
                _player.Queue.Enqueue(track);
                return $"You added » **{track.Title}** to the queue.";
            }
            else
            {
                await _player.PlayAsync(track);
                return $"Bot's now playing » **{track.Title}**";
            }
        }

        public async Task<string> PauseAsync()
        {
            if (_player is null)
                return "No music is playing.";

            await _player.PauseAsync();
            return $"Paused current song » **{_player.CurrentTrack.Title}**";
        }

        public async Task<string> ResumeAsync()
        {
            if (_player is null)
                return "No music is playing.";

            await _player.ResumeAsync();
            return $"Resumed current song » **{_player.CurrentTrack.Title}**";
        }

        public async Task StopAsync()
        {
            if (_player is null)
                return;

            await _player.StopAsync();
        }

        public async Task<string> SkipAsync()
        {
            if (_player is null)
                return "No music is playing.";
            if (_player.Queue.Count is 0)
                return "Nothing in queue";

            var oldTrack = _player.CurrentTrack;
            await _player.SkipAsync();
            return $"Skipped » **{oldTrack.Title}** \nNow playing » **{_player.CurrentTrack.Title}**";
        }

        public async Task<string> SetVolumeAsync(int vol)
        {
            if (_player is null)
                return "No music is playing.";

            if (vol > 150 || vol < 1)
            {
                return $"Please use a number between **1** - **150**.";
            }

            var oldVol = _player.CurrentVolume;
            await _player.SetVolumeAsync(vol);
            return $"Bot's volume changed from **{oldVol}** » **{vol}**.";
        }

        public string Info()
        {
            if (_player is null)
                return $"No music is playing";

            return $"Currently playing » **{_player.CurrentTrack.Title}**";
        }

        public string ShowQueue()
        {
            var count = 0;
            for (var i = 0; i < _player.Queue.Count; i++)
            {
                count++;
            }
            return $"Currently there are **{count}** songs in queue.";
        }

        public string ClearQueue()
        {
            _player.Queue.Clear();
            return $"Bot's cleared current queue.";
        }

        private async Task ClientReadyAsync()
        {
            await _lavaSocketClient.StartAsync(_client, new Configuration
            {
                LogSeverity = LogSeverity.Info
            });
        }

        private async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        {
            if (!reason.ShouldPlayNext())
                return;
            
            if(!player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await player.TextChannel.SendMessageAsync("There are no more songs in the queue.");
                return;
            }

            await player.PlayAsync(nextTrack);
            await player.TextChannel.SendMessageAsync($"Bot's now playing » **{nextTrack.Title}**");
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
