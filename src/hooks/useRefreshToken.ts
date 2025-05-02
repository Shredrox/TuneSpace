import httpClient from "@/services/http-client";
import useAuth from "./useAuth";

const useRefreshToken = () => {
  const { setAuth } = useAuth();

  const refresh = async () => {
    const response = await httpClient.post(
      "Auth/refresh-token",
      {},
      {
        withCredentials: true,
      }
    );

    setAuth((prev) => {
      return {
        ...prev,
        id: response.data.id,
        username: response.data.username,
        accessToken: response.data.newAccessToken,
        role: response.data.role,
      };
    });
    return response.data.newAccessToken;
  };

  const refreshSpotifyToken = async () => {
    try {
      const response = await httpClient.post(
        "Spotify/refresh",
        {},
        {
          withCredentials: true,
        }
      );

      setAuth((prev) => {
        return {
          ...prev,
          spotifyTokenExpiry: response.data.spotifyTokenExpiry,
        };
      });

      console.log(
        "Spotify token refreshed successfully, expires:",
        new Date(response.data.spotifyTokenExpiry)
      );
    } catch (error) {
      console.error("Failed to refresh Spotify token:", error);
      throw error;
    }
  };

  return { refresh, refreshSpotifyToken };
};

export default useRefreshToken;
