import { useMutation } from "@tanstack/react-query";
import { refreshToken } from "@/services/auth-service";
import { refreshSpotifyToken } from "@/services/spotify-service";
import useAuth from "./useAuth";
import { useRouter } from "next/navigation";
import { ROUTES } from "@/utils/constants";

const useRefreshToken = () => {
  const { updateAuth, clearAuth } = useAuth();
  const router = useRouter();

  const handleRefresh = useMutation({
    mutationFn: async () => {
      const userData = await refreshToken();
      return userData;
    },
    onSuccess: (userData) => {
      updateAuth({
        id: userData.id,
        username: userData.username,
        accessToken: userData.accessToken,
        role: userData.role,
      });
    },
    onError: (error) => {
      console.error("Failed to refresh access token:", error);
      clearAuth();
      router.push(ROUTES.LOGIN);
    },
  });

  const handleRefreshSpotify = useMutation({
    mutationFn: async () => {
      const data = await refreshSpotifyToken();
      return data;
    },
    onSuccess: (data) => {
      updateAuth({
        spotifyTokenExpiry: data.spotifyTokenExpiry,
      });
    },
    onError: (error) => {
      console.error("Failed to refresh Spotify token:", error);
      throw error;
    },
  });

  return {
    refresh: handleRefresh.mutateAsync,
    refreshSpotifyToken: handleRefreshSpotify.mutateAsync,
    isRefreshing: handleRefresh.isPending,
    isRefreshingSpotify: handleRefreshSpotify.isPending,
    refreshError: handleRefresh.error,
    spotifyRefreshError: handleRefreshSpotify.error,
  };
};

export default useRefreshToken;
