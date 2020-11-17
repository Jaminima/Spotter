using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using SpotifyAPI.Web;
using Newtonsoft.Json.Linq;

namespace Fume
{
    class Program
    {
        static void Main(string[] args)
        {
            InitKicked();

            Events.OnSkip = Skipped;
            Events.OnPause = Pause;
            Events.OnResume = Play;

            Events.Start();

            while (true) { Console.ReadLine(); }
        }

        static async void InitKicked()
        {
            if (File.Exists("./KickHistory.json")) { 
                StreamReader stream = new StreamReader("./KickHistory.json");
                SkipHistory = JToken.Parse(stream.ReadToEnd()).ToObject<Dictionary<string, int>>();
            }

            string KickedName = $"Kicked Out {DateTime.Now.Year}";

            Paging<SimplePlaylist> playlists = await Spotify.spotify.Playlists.CurrentUsers();

            Kicked = playlists.Items.Find(x => x.Name == KickedName);

            if (Kicked == null)
            {
                FullPlaylist playlist = await Spotify.spotify.Playlists.Create((await Spotify.spotify.UserProfile.Current()).Id, new PlaylistCreateRequest(KickedName));
                InitKicked();
            }
            else KickedTracks = (await Spotify.spotify.Playlists.GetItems(Kicked.Id)).Items;
        }

        static SimplePlaylist Kicked;
        static List<PlaylistTrack<IPlayableItem>> KickedTracks;

        static void Pause(object sender, CurrentlyPlayingContext track)
        {
            Console.WriteLine($"Paused");
        }

        static void Play(object sender, CurrentlyPlayingContext track)
        {
            Console.WriteLine($"Playing");
        }

        static Dictionary<string, int> SkipHistory = new Dictionary<string, int>();
        const int SkipThreshold = 3;

        static async void Skipped(object sender, FullTrack track)
        {
            if (SkipHistory.ContainsKey(track.Id))
            {
                if (SkipHistory[track.Id] >= SkipThreshold)
                {
                    Console.WriteLine($"Removed {track.Name}");

                    if (KickedTracks.Count(x => ((FullTrack)x.Track).Id == track.Id) == 0)
                    {
                        await Spotify.spotify.Playlists.AddItems(Kicked.Id, new PlaylistAddItemsRequest(new List<string>() { track.Uri }));

                        KickedTracks.Add(new PlaylistTrack<IPlayableItem>());
                        KickedTracks.Last().Track = track;
                    }

                    List<bool> Exists = await Spotify.spotify.Library.CheckTracks(new LibraryCheckTracksRequest(new List<string>() { track.Id }));
                    if (Exists[0])
                    {
                        await Spotify.spotify.Library.RemoveTracks(new LibraryRemoveTracksRequest(new List<string>() { track.Id }));
                    }
                }
                else
                {
                    SkipHistory[track.Id]++;
                    Console.WriteLine($"Skipped {track.Name} For The {SkipHistory[track.Id]} Time");
                }
            }
            else
            {
                SkipHistory.Add(track.Id, 1);
                Console.WriteLine($"Skipped {track.Name} For The {SkipHistory[track.Id]} Time");
            }

            StreamWriter stream = new StreamWriter("./SkipHistory.json");
            stream.Write(JToken.FromObject(SkipHistory));
            stream.Flush();
            stream.Close();
        }
    }
}
