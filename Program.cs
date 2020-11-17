using SpotifyAPI.Web;
using System;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Fume
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            User u = AuthFlow.FromCode("AQDib_RlxnXNB9dLTUYtEbVNInsx7oY_yQ6V5doJjZzpnZLkwA3S-KWv2Z6oWZxlAZuRj6NQy4eVQ8YsZcvIndSVLDbvJstYFQyL4_T5JdG4e0p8qr34_vGCj9OajkQPl_qpoNgrSefrxUBuwhsqMMer72xUP5O5A9vEGXJGnx2XL3GVYchKTX-CGCDf0m3GysHrs64j8SMQsyDwunJ5uQC70UhWTZ98aN0m7ANNoQCnElg4q3WCBhDasvmju6MULcbVeMiBkTZq1IZZdrJV_G5TrVwtA-Lx4VzmcauHwri23ItiMg65zIAs6u3cpXkast0TS98xRa7jpt2QBOv28wxEpju_cebVNEUHHVPh6k4yV1nTq2ZKuLhSXNsA9igO8q878qFNJw-P4qnVhR-b2tPMehZf59BBCQ");

            User.Users.Add(u);

            Events.OnSkip = AutoSkipRemover.Skipped;
            Events.OnPause = Pause;
            Events.OnResume = Play;

            Events.Start();

            while (true) { Console.ReadLine(); File.WriteAllText("Users.json", JToken.FromObject(User.Users).ToString()); }
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
