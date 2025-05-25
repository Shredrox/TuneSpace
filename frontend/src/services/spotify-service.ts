import SearchSong from "@/interfaces/spotify/SearchSong";
import SpotifyProfile from "../interfaces/spotify/SpotifyProfile";
import { SPOTIFY_ENDPOINTS } from "@/utils/constants";
import SpotifyArtist from "@/interfaces/spotify/SpotifyArtist";
import httpClient from "./http-client";

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
