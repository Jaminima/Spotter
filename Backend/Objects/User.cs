using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotter
{
    public class User
    {
        #region Fields

        private string _authtoken;

        [JsonIgnore]
        private SpotifyClient _spotify;

        #endregion Fields

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

        private async void SetUser()
        {
            this.userid = (await spotify.UserProfile.Current()).Id;
        }

        #endregion Methods

        public DateTime authExpires = DateTime.MinValue;

        [JsonIgnore]
        public SimplePlaylist KickedPlaylist;

        [JsonIgnore]
        public List<PlaylistTrack<IPlayableItem>> KickedTracks;

        [JsonIgnore]
        public CurrentlyPlayingContext last = null;

        [JsonIgnore]
        public FullTrack lastTrack = null;

        public string refreshtoken;
        public List<Skip> SkipHistory = new List<Skip>();
        public int SkipThreshold = 3;
        public string userid;

        [JsonConstructor]
        public User(string authtoken, string refreshtoken, DateTime authExpires)
        {
            this._authtoken = authtoken;
            this.refreshtoken = refreshtoken;
            this.authExpires = authExpires;
            _spotify = new SpotifyClient(this.authtoken);
            SetUser();
            SetupKicked();
        }

        public string authtoken
        {
            get { if (DateTime.Now > authExpires || _authtoken == null) AuthFlow.Refresh(this); return _authtoken; }
            set { _authtoken = value; }
        }

        [JsonIgnore]
        public SpotifyClient spotify
        {
            get { if (DateTime.Now > authExpires || _spotify == null) _spotify = new SpotifyClient(authtoken); return _spotify; }
        }

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
