export default interface SpotifyArtist {
  followers: {
    total: number;
  };
  images: {
    url: string;
  }[];
  popularity: number;
}
