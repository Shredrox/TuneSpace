import Artist from "./Artist";
import Song from "./Song";
import SpotifyUserInfo from "./SpotifyUserInfo";

export default interface SpotifyProfile{
  profile: SpotifyUserInfo;
  topArtists: Artist[];
  topSongs: Song[];
}
