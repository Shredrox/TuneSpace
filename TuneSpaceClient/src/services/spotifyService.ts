import SearchSong from "@/interfaces/spotify/SearchSong";
import axios from "../axios/axios";
import SpotifyProfile from "../interfaces/spotify/SpotifyProfile";

export const getSpotifyProfile = async () : Promise<SpotifyProfile> =>{
  const response = await axios.get('/Spotify/profile');
  return response.data;
}

export const getSpotifySongsBySearch = async (search : string) : Promise<SearchSong[]> =>{
  const response = await axios.get(`/Spotify/search/${search}`);
  return response.data;
}
