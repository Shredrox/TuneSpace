import SearchSong from "@/interfaces/spotify/SearchSong";
import SpotifyProfile from "../interfaces/spotify/SpotifyProfile";
import { ENDPOINTS, SPOTIFY_ENDPOINTS } from "@/utils/constants";
import SpotifyArtist from "@/interfaces/spotify/SpotifyArtist";
import httpClient from "./http-client";
import RecentlyPlayedStats from "@/interfaces/spotify/RecentlyPlayedStats";
import RecentlyPlayedTrack from "@/interfaces/spotify/RecentlyPlayedTrack";

export const getSpotifyProfile = async (): Promise<SpotifyProfile> => {
  const response = await httpClient.get(SPOTIFY_ENDPOINTS.PROFILE);
  return response.data;
};

export const getSpotifySongsBySearch = async (
  search: string
): Promise<SearchSong[]> => {
  const response = await httpClient.get(
    `${SPOTIFY_ENDPOINTS.SEARCH}/${search}`
  );
  return response.data;
};

export const getSpotifyArtist = async (
  artistId: string | undefined
): Promise<SpotifyArtist> => {
  return (await httpClient.get(`${SPOTIFY_ENDPOINTS.ARTIST}/${artistId}`)).data;
};

export const searchSpotifyArtists = async (
  searchTerm: string
): Promise<SpotifyArtist[]> => {
  const response = await httpClient.get(
    `${SPOTIFY_ENDPOINTS.SEARCH_ARTISTS}/${searchTerm}`
  );
  return response.data;
};

export const getTodayListeningStats =
  async (): Promise<RecentlyPlayedStats> => {
    const response = await httpClient.get(
      SPOTIFY_ENDPOINTS.LISTENING_STATS_TODAY
    );
    return response.data;
  };

export const getThisWeekListeningStats =
  async (): Promise<RecentlyPlayedStats> => {
    const response = await httpClient.get(
      SPOTIFY_ENDPOINTS.LISTENING_STATS_THIS_WEEK
    );
    return response.data;
  };

export const getRecentlyPlayedTracks = async (): Promise<
  RecentlyPlayedTrack[]
> => {
  const response = await httpClient.get(SPOTIFY_ENDPOINTS.RECENTLY_PLAYED);
  return response.data;
};

export const getFollowedArtists = async (): Promise<SpotifyArtist[]> => {
  const response = await httpClient.get(SPOTIFY_ENDPOINTS.FOLLOWED_ARTISTS);
  return response.data;
};

export const refreshSpotifyToken = async () => {
  const response = await httpClient.post(
    SPOTIFY_ENDPOINTS.REFRESH,
    {},
    {
      withCredentials: true,
    }
  );
  return response.data;
};

export const getSpotifyConnectionStatus = async () => {
  const response = await httpClient.get(SPOTIFY_ENDPOINTS.CONNECTION_STATUS);
  return response.data;
};

export const connectSpotifyAccount = async (code: string, state: string) => {
  const response = await httpClient.post(
    ENDPOINTS.CONNECT_SPOTIFY,
    { code, state },
    {
      withCredentials: true,
    }
  );
  return response.data;
};
