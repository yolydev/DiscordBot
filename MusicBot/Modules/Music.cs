using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MusicBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace MusicBot.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Music(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("join")]
        public async Task Join()
        {
            var user = Context.User as SocketGuildUser;
            if(user.VoiceChannel is null)
            {
                await ReplyAsync("You must be connected to a voice channel.");
                return;
            }
            else
            {
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Bot now connected to **{user.VoiceChannel.Name}** voice channel.");
            }
        }

        [Command("move")]
        public async Task Move()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("You must be connected to a voice channel.");
                return;
            }
            else
            {
                await _musicService.DisconnectAsync(user.VoiceChannel);
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Bot moved to **{user.VoiceChannel.Name}** voice channel.");
            }
        }

        [Command("leave")]
        public async Task Leave()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("You need to be in the Bot's voice channel.");
            }
            else
            {
                await _musicService.DisconnectAsync(user.VoiceChannel);
                await ReplyAsync($"Bot left **{user.VoiceChannel.Name}** voice channel.");
            }
        }

        [Command("play")]
        public async Task Play([Remainder]string query)
            => await ReplyAsync(await _musicService.PlayAsync(query, Context.Guild.Id));
        
        [Command("pause")]
        public async Task Pause()
            => await ReplyAsync(await _musicService.PauseAsync());
        
        [Command("resume")]
        public async Task Resume()
            => await ReplyAsync(await _musicService.ResumeAsync());

        [Command("stop")]
        public async Task Stop()
        {
            await _musicService.StopAsync();
            await ReplyAsync("Bot's stopped playing music.");
        }

        [Command("skip")]
        public async Task Skip()
            => await ReplyAsync(await _musicService.SkipAsync());

        [Command("volume"), Alias("vol")]
        public async Task Volume(int vol)
            => await ReplyAsync(await _musicService.SetVolumeAsync(vol));

        [Command("songinfo"), Alias("current")]
        public async Task Info()
            => await ReplyAsync(_musicService.Info());

        [Command("queue")]
        public async Task ShowQueue()
            => await ReplyAsync(_musicService.ShowQueue());

        [Command("clearqueue"), Alias("cq")]
        public async Task ClearQueue()
            => await ReplyAsync(_musicService.ClearQueue());
    }
}
