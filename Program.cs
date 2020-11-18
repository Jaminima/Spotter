using SpotifyAPI.Web;
using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;

namespace Fume
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            //User u = AuthFlow.FromCode("AQCa_yD5tuo-cy39szWuf8lZiqk7XuHU7siChPB-0zgNkXb0eii2JpaYDgAAs_LDQKefPmPo7tykRegzvveUx9YhLOY5nCOzZX9xiDuQA-NcON0amdZbJImHTMOfj0arfzr8nuwH2vq2g7WKFq0FC_ishVQypXJa2R6pgeOkulnHc2r9Er-qgBR-Wd1vrqKPd65zDYAqpGPWyFFLXLKjt1GImOFLPgL4lzdQid-shr2HBHk2Qrkf5XIu52XdeZKSZXBe5OuyFIyjtDQMVeo8KMmx7fFHFuoZtGcnwFGDc3RXUYURTboWzdiV1XI6XmUA9Ocy_N7BmhRUlHzHtA7jrlC3CAwgmiLCetTmZM9fDUxCY5_Ry-QelHSjv16PGmqpcuLLzdasWf0izXimiKWsG8rvKUZtpoyOWg");

            //User.Users.Add(u);

            User.Users = JToken.Parse(File.ReadAllText("Users.json")).ToObject<List<User>>();

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
