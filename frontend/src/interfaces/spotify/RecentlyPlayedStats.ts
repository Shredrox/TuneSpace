import RecentlyPlayedTrack from "./RecentlyPlayedTrack";

export default interface RecentlyPlayedStats {
  tracks: RecentlyPlayedTrack[];
  totalHoursPlayed: number;
  uniqueTracksCount: number;
  totalPlays: number;
  timePeriod: string;
}
