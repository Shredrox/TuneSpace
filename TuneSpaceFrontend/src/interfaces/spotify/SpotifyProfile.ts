import SpotifyArtist from "./SpotifyArtist";
import Song from "./Song";
import SpotifyUserInfo from "./SpotifyUserInfo";

export default interface SpotifyProfile {
  profile: SpotifyUserInfo;
  topArtists: SpotifyArtist[];
  topSongs: Song[];
}
