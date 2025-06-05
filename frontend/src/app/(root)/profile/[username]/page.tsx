import Loading from "@/components/fallback/loading";
import { SPOTIFY_ENDPOINTS } from "@/utils/constants";
import { cookies } from "next/headers";
import httpClient from "@/services/http-client";
import Profile from "@/components/user/profile";
import RecentlyPlayedTrack from "@/interfaces/spotify/RecentlyPlayedTrack";

export default async function ProfilePage({
  params,
}: {
  params: Promise<{ username: string }>;
}) {
  const { username } = await params;

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

  return (
    <>
      <Profile
        username={username}
        spotifyProfileData={spotifyProfileData}
        recentlyPlayedTracks={recentlyPlayedTracks}
      />
    </>
  );
}
