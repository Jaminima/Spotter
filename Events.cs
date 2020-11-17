using SpotifyAPI.Web;
using System;
using System.Threading;

namespace Fume
{
    public static class Events
    {
        #region Methods

        private static void CheckEvents()
        {
            while (true)
            {
                foreach (User user in User.Users)
                {
                    CheckUserEvent(user);
                }
                Thread.Sleep(1000);
            }
        }

        private static async void CheckUserEvent(User user)
        {
            CurrentlyPlayingContext playing = await user.spotify.Player.GetCurrentPlayback();

            if (playing != null)
            {
                if (user.lastTrack == null)
                {
                    user.last = playing;
                    user.lastTrack = (FullTrack)playing.Item;
                }
                else if (playing.Item != null)
                {
                    FullTrack track = (FullTrack)playing.Item;

                    //Check if skipped
                    if (track.Id != user.lastTrack.Id && user.last.ProgressMs < user.lastTrack.DurationMs - 5000)
                    {
                        OnSkip(user, user.lastTrack);
                    }

                    user.lastTrack = track;
                }

                //Check if play state changed
                if (playing.IsPlaying != user.last.IsPlaying)
                {
                    if (playing.IsPlaying) OnResume(user, playing);
                    else OnPause(user, playing);
                }

                user.last = playing;
            }
        }

        #endregion Methods

        #region Fields

        public static EventHandler<CurrentlyPlayingContext> OnResume, OnPause;
        public static EventHandler<FullTrack> OnSkip;

        #endregion Fields

        public static void Start()
        {
            new Thread(() => CheckEvents()).Start();
        }
    }
}
