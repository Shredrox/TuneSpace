import { useQuery } from "@tanstack/react-query";
import {
  getSpotifyProfile,
  getTodayListeningStats,
  getThisWeekListeningStats,
  getRecentlyPlayedTracks,
  getFollowedArtists,
} from "../../services/spotify-service";

const useSpotify = () => {
  const {
    data: spotifyProfileData,
    isLoading: isSpotifyProfileLoading,
    isError: isSpotifyProfileError,
    error: spotifyProfileError,
  } = useQuery({
    queryKey: ["spotify"],
    queryFn: getSpotifyProfile,
  });

  const {
    data: todayStats,
    isLoading: isTodayStatsLoading,
    isError: isTodayStatsError,
  } = useQuery({
    queryKey: ["spotify-today-stats"],
    queryFn: getTodayListeningStats,
    enabled: !!spotifyProfileData,
  });

  const {
    data: thisWeekStats,
    isLoading: isThisWeekStatsLoading,
    isError: isThisWeekStatsError,
  } = useQuery({
    queryKey: ["spotify-week-stats"],
    queryFn: getThisWeekListeningStats,
    enabled: !!spotifyProfileData,
  });

  const {
    data: recentlyPlayedTracks,
    isLoading: isRecentlyPlayedLoading,
    isError: isRecentlyPlayedError,
  } = useQuery({
    queryKey: ["spotify-recently-played"],
    queryFn: getRecentlyPlayedTracks,
    enabled: !!spotifyProfileData,
  });

  const {
    data: followedArtists,
    isLoading: isFollowedArtistsLoading,
    isError: isFollowedArtistsError,
  } = useQuery({
    queryKey: ["spotify-followed-artists"],
    queryFn: getFollowedArtists,
    enabled: !!spotifyProfileData,
  });

  return {
    spotifyProfileData,
    isSpotifyProfileLoading,
    isSpotifyProfileError,
    spotifyProfileError,
    todayStats,
    isTodayStatsLoading,
    isTodayStatsError,
    thisWeekStats,
    isThisWeekStatsLoading,
    isThisWeekStatsError,
    recentlyPlayedTracks,
    isRecentlyPlayedLoading,
    isRecentlyPlayedError,
    followedArtists,
    isFollowedArtistsLoading,
    isFollowedArtistsError,
  };
};

export default useSpotify;
