using System;
using SpotifyAPI.Web;
using System.Threading;

namespace Fume
{
    class Program
    {
        static void Main(string[] args)
        {
            Ahh();

            while (true) { Console.ReadLine(); }
        }

        static async void Ahh()
        {
            SpotifyClient spotify = new SpotifyClient("BQAY8wl_z77A8C59rHzbt72IrAcHWX9DU9YL6y9To1hyKhxKL31mjslCvIe5TzwO9oL0z-PVoAFHPgdCMeUWnSeA1QVU_OR9KQ6PLiT8axN8l3s5Cvw8dZYx9lLt1mR6_MWIKHosyPWYZz-ZWCRg3RkxiQZ0V8eav45eMNXPaZlN8YEYKttnWxeqIIm0R4YthRKlig");

            FullTrack lastTrack = null;
            CurrentlyPlayingContext last = null;

            while (true)
            {
                CurrentlyPlayingContext playing = await spotify.Player.GetCurrentPlayback();


                if (lastTrack == null) {
                    last = playing;
                    lastTrack = (FullTrack)playing.Item;
                }
                else if (playing.IsPlaying)
                {
                    FullTrack track = (FullTrack)playing.Item;

                    //Check if skipped
                    if (track.Id != lastTrack.Id && last.ProgressMs < lastTrack.DurationMs)
                    {
                        Console.WriteLine($"You Skipped {lastTrack.Name}");
                    }

                    last = playing;
                    lastTrack = track;
                }

                Thread.Sleep(1000);
            }

            //await spotify.Player.ResumePlayback();

            //var track = await spotify.Tracks.Get("1s6ux0lNiTziSrd7iUAADH");

            //Console.WriteLine(track.Name);
        }
    }
}
