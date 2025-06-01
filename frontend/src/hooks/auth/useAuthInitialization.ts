"use client";

import { useEffect, useState } from "react";
import useAuth from "@/hooks/auth/useAuth";
import useRefreshToken from "@/hooks/auth/useRefreshToken";
import { isTokenExpired } from "@/lib/auth";

/**
 * Custom hook to handle authentication initialization
 * Attempts to restore auth state from HTTP-only cookies on app load
 */
const useAuthInitialization = () => {
  const { auth, updateAuth } = useAuth();
  const { refresh, refreshSpotifyToken, isRefreshing, isRefreshingSpotify } =
    useRefreshToken();
  const [isInitialized, setIsInitialized] = useState(false);

  useEffect(() => {
    let isMounted = true;

    const initializeAuth = async () => {
      try {
        if (!auth?.accessToken || isTokenExpired(auth.accessToken)) {
          await refresh();
        }

        if (
          auth?.accessToken &&
          (!auth?.spotifyTokenExpiry ||
            new Date(auth.spotifyTokenExpiry) <= new Date())
        ) {
          await refreshSpotifyToken();
        }
      } catch (error) {
        console.error("Auth initialization failed:", error);
      } finally {
        if (isMounted) {
          setIsInitialized(true);
        }
      }
    };

    if (!isInitialized) {
      initializeAuth();
    }

    return () => {
      isMounted = false;
    };
  }, [
    auth?.accessToken,
    auth?.spotifyTokenExpiry,
    refresh,
    refreshSpotifyToken,
    isInitialized,
    updateAuth,
  ]);

  return {
    isInitialized,
    isInitializing: !isInitialized || isRefreshing || isRefreshingSpotify,
    hasValidAuth: auth?.accessToken && !isTokenExpired(auth.accessToken),
  };
};

export default useAuthInitialization;
