using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

namespace SweazyHut.Modules
{
    public class MusicModule : SweazyHutModuleBase
    {
        private readonly LavaNode lavanode;
        public MusicModule(LavaNode lavanode)   
          =>  this.lavanode = lavanode;


        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayAsync( [Remainder]string query)
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            if (!lavanode.HasPlayer(Context.Guild))
            {
                try
                {
                    await lavanode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                    await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
                }
                catch (Exception exception)
                {
                    await ReplyAsync(exception.Message);
                }
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!lavanode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            var searchResponse = Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                   await lavanode.SearchAsync(query)
                   : await lavanode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for {query}.");
                return;
            }

            var player = lavanode.GetPlayer(Context.Guild);

            if(player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    foreach (var track in searchResponse.Tracks)
                    {
                        player.Queue.Enqueue(track);
                    }
                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    var track = searchResponse.Tracks[0];
                    player.Queue.Enqueue(track);
                    await ReplyAsync($"Enqueued: {track.Title}");
                }
            }
            else
            {
                var track = searchResponse.Tracks[0];
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    for (var i = 0; i < searchResponse.Tracks.Count; i++)
                    {
                        if (i == 0)
                        {
                            await player.PlayAsync(track);
                            await ReplyAsync($"Now playing: {track.Title}");
                        }
                        else
                        {
                            player.Queue.Enqueue(searchResponse.Tracks[i]);
                        }
                    }
                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    await player.PlayAsync(track);
                await ReplyAsync($"Now Playing: {track.Title}");
                }
            }
        }

        [Command("skip", RunMode = RunMode.Async)]
        [Alias("s")]
        public async Task Skip()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            if (!lavanode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }
            
            var player = lavanode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same voice channel as me!");
                return;
            }
            if(player.Queue.Count == 0)
            {
                await ReplyAsync("There are no more songs in the queue!");
                return;
            }

            await player.SkipAsync();
            await ReplyAsync($"Skipped! Now playing **{player.Track.Title}**!");
        }

        [Command("pause", RunMode = RunMode.Async)]
        public async Task Pause()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            if (!lavanode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            var player = lavanode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same voice channel as me!");
                return;
            }

            if(player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("The Music is already paused!");
                return;
            }

            await player.PauseAsync();
            await ReplyAsync("Paused the music!");
        }

        [Command("resume", RunMode = RunMode.Async)]
        public async Task Resume()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            if (!lavanode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            var player = lavanode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same voice channel as me!");
                return;
            }

            if (player.PlayerState == PlayerState.Playing)
            {
                await ReplyAsync("The Music is already playing!");
                return;
            }

            await player.ResumeAsync();
            await ReplyAsync("Resumed the music!");
        }
    }
}
