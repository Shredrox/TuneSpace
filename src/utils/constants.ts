export const BASE_URL = "http://localhost:5053/api";

export const ROUTES = {
  HOME: "/",
  NEWS: "/news",
  BAND_DASHBOARD: "/band/dashboard",
} as const;

export const ENDPOINTS = {
  LOGIN: "Auth/login",
  LOGOUT: "Auth/logout",
  REGISTER: "Auth/register",
  BANDREGISTER: "Band/register",
  BANDUPDATE: "Band/update",
  BAND: "Band",
  RECOMMENDATIONS: "MusicDiscovery/recommendations",
  DISCOVER: "MusicDiscovery/discover",
} as const;

export const SPOTIFY_ENDPOINTS = {
  LOGIN: "Spotify/login",
  SEARCH: "Spotify/search",
  PROFILE: "Spotify/profile",
  ARTIST: "Spotify/artist",
  ARTISTS: "Spotify/artists",
  RECENTLY_PLAYED: "Spotify/recently-played",
} as const;

export enum UserRole {
  Admin = "Admin",
  BandAdmin = "BandAdmin",
  BandMember = "BandMember",
  Listener = "Listener",
}
