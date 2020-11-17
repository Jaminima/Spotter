using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Fume
{
    public class User
    {
        #region Methods

        private async void SetupKicked()
        {
            string KickedName = $"Kicked Out {DateTime.Now.Year}";

            Paging<SimplePlaylist> playlists = await spotify.Playlists.CurrentUsers();

            KickedPlaylist = playlists.Items.Find(x => x.Name == KickedName);

            if (KickedPlaylist == null)
            {
                FullPlaylist playlist = await spotify.Playlists.Create((await spotify.UserProfile.Current()).Id, new PlaylistCreateRequest(KickedName));
                SetupKicked();
            }
            else KickedTracks = (await spotify.Playlists.GetItems(KickedPlaylist.Id)).Items;
        }

        #endregion Methods

        #region Fields

        public static List<User> Users = new List<User>();

        [JsonIgnore]
        public SimplePlaylist KickedPlaylist;

        [JsonIgnore]
        public List<PlaylistTrack<IPlayableItem>> KickedTracks;

        [JsonIgnore]
        public CurrentlyPlayingContext last = null;

        [JsonIgnore]
        public FullTrack lastTrack = null;

        public string refreshtoken, authtoken;

        public List<Skip> SkipHistory = new List<Skip>();

        public int SkipThreshold = 3;

        [JsonIgnore]
        public SpotifyClient spotify;

        #endregion Fields

        #region Constructors

        public User()
        {
            spotify = new SpotifyClient(authtoken);
            SetupKicked();
        }

        public User(string authtoken)
        {
            this.authtoken = authtoken;
            spotify = new SpotifyClient(authtoken);
            SetupKicked();
        }

        #endregion Constructors

        public async Task<PrivateUser> GetUser()
        {
            return await spotify.UserProfile.Current();
        }

        public int RecentSkips(string trackid)
        {
            DateTime After = DateTime.Now.AddDays(-7);
            return SkipHistory.Count(x => x.trackId == trackid && x.when > After);
        }
    }
}
