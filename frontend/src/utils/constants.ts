export const BASE_URL = "http://localhost:5053/api";

export const SIGNALR_HUB_URL = "http://localhost:5053/socket-hub";

export const ROUTES = {
  ROOT: "/",
  HOME: "/home",
  LOGIN: "/login",
  SIGNUP: "/signup",
  TERMS: "/terms",
  PRIVACY: "/privacy",
  COPYRIGHT: "/copyright",
  ABOUT: "/about",
  DISCOVER: "/discover",
  NEWS: "/news",
  BAND_DASHBOARD: "/band/dashboard",
  EVENTS: "/events",
  PROFILE: "/profile",
  FORUMS: "/forums",
  MESSAGES: "/messages",
  BAND: "/band",
  BAND_REGISTER: "/band/register",
} as const;

export const ENDPOINTS = {
  LOGIN: "Auth/login",
  LOGOUT: "Auth/logout",
  REGISTER: "Auth/register",
  SPOTIFY_OAUTH: "Auth/spotify-oauth",
  BANDREGISTER: "Band/register",
  BANDUPDATE: "Band/update",
  BAND: "Band",
  RECOMMENDATIONS: "MusicDiscovery/recommendations",
  DISCOVER: "MusicDiscovery/discover",
  MUSIC_EVENTS: "MusicEvent",
  NOTIFICATION: "Notification",
  FORUM: "Forum",
} as const;

export const SPOTIFY_ENDPOINTS = {
  LOGIN: "Spotify/login",
  SEARCH: "Spotify/search",
  SEARCH_ARTISTS: "Spotify/search-artists",
  PROFILE: "Spotify/profile",
  ARTIST: "Spotify/artist",
  ARTISTS: "Spotify/artists",
  RECENTLY_PLAYED: "Spotify/recently-played",
  FOLLOWED_ARTISTS: "Spotify/followed-artists",
  LISTENING_STATS_TODAY: "Spotify/listening-stats/today",
  LISTENING_STATS_THIS_WEEK: "Spotify/listening-stats/this-week",
} as const;

export enum UserRole {
  Admin = "Admin",
  BandAdmin = "BandAdmin",
  BandMember = "BandMember",
  Listener = "Listener",
}

export enum NotificationTypes {
  Follow = "Follow",
  Like = "Like",
  Message = "Message",
  Event = "Event",
  Music = "Music",
}

export const PROTECTED_ROUTES = [
  ROUTES.HOME,
  ROUTES.DISCOVER,
  ROUTES.PROFILE,
  ROUTES.EVENTS,
  ROUTES.NEWS,
  ROUTES.FORUMS,
  ROUTES.MESSAGES,
  ROUTES.BAND_DASHBOARD,
];

export const PUBLIC_ROUTES = [
  ROUTES.ROOT,
  ROUTES.LOGIN,
  ROUTES.SIGNUP,
  ROUTES.BAND_REGISTER,
  ROUTES.TERMS,
  ROUTES.PRIVACY,
  ROUTES.COPYRIGHT,
  ROUTES.ABOUT,
];
