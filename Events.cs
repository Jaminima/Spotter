using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using SpotifyAPI.Web;

namespace Fume
{
    public static class Events
    {
        public static EventHandler<FullTrack> OnSkip;
        public static EventHandler<CurrentlyPlayingContext> OnResume, OnPause;

        public static void Start()
        {
            new Thread(async () => await CheckEvents()).Start();
        }

        private static async Task CheckEvents()
        {
            FullTrack lastTrack = null;
            CurrentlyPlayingContext last = null;

            while (true)
            {
                CurrentlyPlayingContext playing = await Spotify.spotify.Player.GetCurrentPlayback();

                if (playing!=null)
                {
                    if (lastTrack == null)
                    {
                        last = playing;
                        lastTrack = (FullTrack)playing.Item;
                    }
                    else if (playing.Item!=null)
                    {
                        FullTrack track = (FullTrack)playing.Item;

                        //Check if skipped
                        if (track.Id != lastTrack.Id && last.ProgressMs < lastTrack.DurationMs - 5000)
                        {
                            OnSkip(null, lastTrack);
                        }

                        lastTrack = track;
                    }

                    //Check if play state changed
                    if (playing.IsPlaying != last.IsPlaying)
                    {
                        if (playing.IsPlaying) OnResume(null, playing);
                        else OnPause(null, playing);
                    }

                    last = playing;
                }
                Thread.Sleep(1000);
            }

                
            }
        }
    }
