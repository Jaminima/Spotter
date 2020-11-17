using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public SimplePlaylist KickedPlaylist;

        public List<PlaylistTrack<IPlayableItem>> KickedTracks;

        public CurrentlyPlayingContext last = null;

        public FullTrack lastTrack = null;

        //https://accounts.spotify.com/authorize?client_id=1da271b7266f4723bd37afa0ff58fe83&response_type=code&redirect_uri=http://localhost&scope=user-read-playback-state%20playlist-modify-public%20playlist-modify-private%20user-read-currently-playing%20user-library-modify%20user-read-playback-position%20playlist-read-private%20user-library-read
        public string refreshtoken, authtoken;

        public List<Skip> SkipHistory = new List<Skip>();

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
    }
}
