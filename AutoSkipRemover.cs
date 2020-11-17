using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fume
{
    public static class AutoSkipRemover
    {
        #region Fields

        private const int SkipThreshold = 3;

        #endregion Fields

        #region Methods

        public static async void Skipped(object sender, FullTrack track)
        {
            User user = (User)sender;

            if (user.SkipHistory.Count(x => x.trackId == track.Id) >= SkipThreshold - 1)
            {
                Console.WriteLine($"Removed {track.Name}");

                if (user.KickedTracks.Count(x => ((FullTrack)x.Track).Id == track.Id) == 0)
                {
                    await user.spotify.Playlists.AddItems(user.KickedPlaylist.Id, new PlaylistAddItemsRequest(new List<string>() { track.Uri }));

                    user.KickedTracks.Add(new PlaylistTrack<IPlayableItem>());
                    user.KickedTracks.Last().Track = track;
                }

                List<bool> Exists = await user.spotify.Library.CheckTracks(new LibraryCheckTracksRequest(new List<string>() { track.Id }));
                if (Exists[0])
                {
                    await user.spotify.Library.RemoveTracks(new LibraryRemoveTracksRequest(new List<string>() { track.Id }));
                }
            }
            else
            {
                user.SkipHistory.Add(new Skip(track.Id));
                Console.WriteLine($"Skipped {track.Name} For The {user.SkipHistory.Count(x => x.trackId == track.Id)} Time");
            }
        }

        #endregion Methods
    }
}
