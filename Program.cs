using SpotifyAPI.Web;
using System;

namespace Fume
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            User.Users.Add(new User("BQCPlsmLM47ujBwo4xrPbzYJ716VkuRp_oMiSz6gC2tXOTcK_YxZKkaxq8th_1onfsdy1cVFFdMN6MxNheKQxiKHRTihNA7NiZ05TXWOfvXShTvt8gcE4tQkOwQo2xwviNFNopfhWymzAuJS8Xi0HiG5URaSXReTeDGx68FTy8_EnaFba543pyhaGOLIVnqRl8SI3O_CKfznJF1TNEUJ6VO-ojtOvsmW_7zIO0Jozx6STSnjUaIxcsHvQTUL8Q"));

            Events.OnSkip = AutoSkipRemover.Skipped;
            Events.OnPause = Pause;
            Events.OnResume = Play;

            Events.Start();

            while (true) { Console.ReadLine(); }
        }

        private static void Pause(object sender, CurrentlyPlayingContext track)
        {
            Console.WriteLine($"Paused");
        }

        private static void Play(object sender, CurrentlyPlayingContext track)
        {
            Console.WriteLine($"Playing");
        }

        #endregion Methods
    }
}
