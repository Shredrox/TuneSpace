import { IoMusicalNote } from "react-icons/io5";
import { FaGuitar, FaSpotify, FaHistory } from "react-icons/fa";
import Loading from "@/components/fallback/loading";
import { SPOTIFY_ENDPOINTS } from "@/utils/constants";
import { cookies } from "next/headers";
import { getUserByName } from "@/services/user-service";
import httpClient from "@/services/http-client";
import { formatDate } from "@/utils/helpers";

interface RecentlyPlayedTrack {
  trackName: string;
  artistName: string;
  artistId: string;
  albumName: string;
  albumImageUrl: string;
  playedAt: string;
}

export default async function Profile({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;

  const user = await getUserByName(slug);

  const cookie = (await cookies()).get("SpotifyAccessToken");

  let spotifyProfileData = null;
  let recentlyPlayedTracks: RecentlyPlayedTrack[] = [];

  if (cookie) {
    try {
      spotifyProfileData = (
        await httpClient.get(SPOTIFY_ENDPOINTS.PROFILE, {
          withCredentials: true,
          headers: {
            Cookie: `SpotifyAccessToken=${cookie.value}`,
          },
        })
      ).data;

      recentlyPlayedTracks =
        (
          await httpClient.get(`${SPOTIFY_ENDPOINTS.RECENTLY_PLAYED}`, {
            withCredentials: true,
            headers: {
              Cookie: `SpotifyAccessToken=${cookie.value}`,
            },
          })
        ).data || [];
    } catch (error) {
      console.error("Error fetching Spotify data:", error);
    }
  }

  if (!spotifyProfileData) {
    return <Loading />;
  }

  return (
    <div className="text-foreground min-h-screen bg-gradient-to-b dark:from-gray-900 dark:to-black from-gray-100 to-white">
      <div className="relative">
        <div className="h-48 w-full overflow-hidden relative">
          <div className="absolute inset-0 bg-gradient-to-r dark:from-blue-900/40 dark:to-purple-900/40 from-blue-500/20 to-purple-500/20 z-10"></div>
          <img
            src="https://picsum.photos/1600/400"
            alt="Profile banner"
            className="w-full h-full object-cover"
          />
        </div>

        <div className="flex flex-col md:flex-row items-center md:items-end px-6 -mt-16 relative z-20">
          <img
            className="rounded-full w-32 h-32 border-4 border-background object-cover shadow-xl"
            src={
              user?.profilePicture ||
              spotifyProfileData?.profile?.profilePicture ||
              "https://via.placeholder.com/200"
            }
            alt={user?.userName || "Profile picture"}
          />
          <div className="md:ml-6 mt-4 md:mt-0 mb-4 text-center md:text-left">
            <h1 className="text-3xl font-bold">{user?.userName}</h1>
            <div className="flex items-center gap-2 text-muted-foreground mt-1 justify-center md:justify-start">
              <FaSpotify className="text-[#1ed760]" />
              <span>Premium: {spotifyProfileData?.profile?.spotifyPlan}</span>
              <span className="mx-2">•</span>
              <span>
                {spotifyProfileData?.profile?.followerCount} followers
              </span>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-6xl mx-auto px-4 py-8">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="bg-card bg-opacity-90 backdrop-filter backdrop-blur-lg rounded-xl overflow-hidden shadow-lg border border-border">
            <div className="p-5 border-b border-border">
              <h2 className="text-xl font-bold flex items-center gap-2 text-card-foreground">
                <FaGuitar className="text-primary" /> Top Artists
              </h2>
            </div>
            <div className="p-5">
              {spotifyProfileData?.topArtists?.map((artist, index) => (
                <div
                  key={index}
                  className="flex items-center gap-4 mb-4 hover:bg-accent p-2 rounded-lg transition-all"
                >
                  <img
                    className="rounded-full w-16 h-16 object-cover shadow-md"
                    src={artist.image}
                    alt={artist.name}
                  />
                  <div>
                    <p className="font-medium text-lg text-card-foreground">
                      {artist.name}
                    </p>
                    <p className="text-sm text-muted-foreground">Artist</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="bg-card bg-opacity-90 backdrop-filter backdrop-blur-lg rounded-xl overflow-hidden shadow-lg border border-border">
            <div className="p-5 border-b border-border">
              <h2 className="text-xl font-bold flex items-center gap-2 text-card-foreground">
                <IoMusicalNote className="text-primary" /> Top Songs
              </h2>
            </div>
            <div className="p-5">
              {spotifyProfileData?.topSongs?.map((song, index) => (
                <div
                  key={index}
                  className="flex items-center gap-4 mb-4 hover:bg-accent p-2 rounded-lg transition-all"
                >
                  <img
                    className="rounded-md w-16 h-16 object-cover shadow-md"
                    src={song.image}
                    alt={song.name}
                  />
                  <div className="flex flex-col">
                    <p className="font-medium text-lg text-card-foreground">
                      {song.name}
                    </p>
                    <p className="text-sm text-muted-foreground">
                      {song.artist}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        <div className="mt-6 bg-card bg-opacity-90 backdrop-filter backdrop-blur-lg rounded-xl overflow-hidden shadow-lg border border-border">
          <div className="p-5 border-b border-border">
            <h2 className="text-xl font-bold flex items-center gap-2 text-card-foreground">
              <FaHistory className="text-primary" /> Recently Played
            </h2>
          </div>

          <div className="p-5">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {recentlyPlayedTracks.length > 0 ? (
                recentlyPlayedTracks.map((track, index) => (
                  <div
                    key={`${track.trackName}-${index}`}
                    className="flex items-center gap-3 p-3 rounded-lg hover:bg-accent transition-all"
                  >
                    <img
                      src={track.albumImageUrl}
                      alt={track.albumName}
                      className="w-16 h-16 object-cover rounded shadow-md"
                    />
                    <div className="flex flex-col overflow-hidden">
                      <p className="font-medium text-card-foreground truncate">
                        {track.trackName}
                      </p>
                      <p className="text-sm text-muted-foreground truncate">
                        {track.artistName}
                      </p>
                      <p className="text-xs text-muted-foreground mt-1">
                        {formatDate(track.playedAt)}
                      </p>
                    </div>
                  </div>
                ))
              ) : (
                <div className="col-span-full text-center py-8 text-muted-foreground">
                  No recently played tracks found
                </div>
              )}
            </div>
          </div>
        </div>

        <div className="mt-6 bg-card bg-opacity-90 backdrop-filter backdrop-blur-lg rounded-xl p-6 shadow-lg border border-border">
          <h2 className="text-xl font-bold mb-4 flex items-center gap-2 text-card-foreground">
            <FaSpotify className="text-[#1ed760]" /> Spotify Profile
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="bg-accent p-4 rounded-lg text-center">
              <p className="text-muted-foreground text-sm">Followers</p>
              <p className="text-2xl font-bold text-accent-foreground">
                {spotifyProfileData?.profile?.followerCount || 0}
              </p>
            </div>
            <div className="bg-accent p-4 rounded-lg text-center">
              <p className="text-muted-foreground text-sm">Subscription</p>
              <p className="text-2xl font-bold text-accent-foreground">
                {spotifyProfileData?.profile?.spotifyPlan || "Free"}
              </p>
            </div>
            <div className="bg-accent p-4 rounded-lg text-center">
              <p className="text-muted-foreground text-sm">Member Since</p>
              <p className="text-2xl font-bold text-accent-foreground">2023</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
