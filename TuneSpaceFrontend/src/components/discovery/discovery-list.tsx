"use client";

import { useQuery } from "@tanstack/react-query";
import { ENDPOINTS } from "@/utils/constants";
import Loading from "../fallback/loading";
import { useState } from "react";
import httpClient from "@/services/http-client";

interface DiscoveryArtist {
  name: string;
  location: string;
  genres: string[];
  listeners: number;
  playCount: number;
  imageUrl: string;
  popularity: number;
  relevanceScore: number;
  similarArtists: string[];
  isRegistered: boolean;
}

const fetchDiscover = async (
  genreSet: Set<string>,
  useLocation: boolean,
  location: string = "Bulgaria"
) => {
  const genresParam = Array.from(genreSet).join(",");
  let url = `${ENDPOINTS.RECOMMENDATIONS}?genres=${encodeURIComponent(
    genresParam
  )}`;

  if (useLocation) {
    url += `&location=${encodeURIComponent(location)}`;
  }

  const response = await httpClient.get(url);
  return response.data;
};

const DiscoveryList = () => {
  const [useLocation, setUseLocation] = useState(true);

  const {
    data: discovery,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ["discoveryBands", useLocation],
    queryFn: () => {
      const genreSet = new Set(["metal", "rock"]);
      return fetchDiscover(genreSet, useLocation);
    },
    refetchOnWindowFocus: false,
  });

  const handleLocationToggle = () => {
    setUseLocation(!useLocation);
  };

  if (isLoading)
    return (
      <div className="flex items-center justify-center p-8 min-h-[300px]">
        <div className="flex flex-col items-center gap-3">
          <div className="text-xl font-semibold text-primary/90">
            Discovering new artists
          </div>
          <Loading />
        </div>
      </div>
    );
  if (error)
    return (
      <div className="p-6 text-destructive bg-destructive/10 rounded-lg text-center my-4">
        Unable to load recommendations. Please try again later.
      </div>
    );

  return (
    <div className="w-full py-8">
      <div className="relative mb-8">
        <div className="flex justify-between items-start mb-4">
          <div>
            <h2 className="text-3xl font-bold mb-2 bg-clip-text text-transparent bg-gradient-to-r from-primary to-primary/70">
              Discover New Artists
            </h2>
            <p className="text-muted-foreground max-w-2xl">
              Explore fresh sounds and hidden gems based on your musical
              preferences
            </p>
          </div>

          <div className="flex items-center gap-2 bg-card p-2 rounded-lg shadow-sm border border-border/40">
            <span className="text-sm text-muted-foreground">
              {useLocation ? "Location: On" : "Location: Off"}
            </span>
            <button
              onClick={handleLocationToggle}
              className="flex items-center"
              aria-label="Toggle location-based recommendations"
            >
              <div
                className={`w-10 h-5 rounded-full p-0.5 transition-colors ${
                  useLocation ? "bg-primary" : "bg-muted"
                }`}
              >
                <div
                  className={`w-4 h-4 rounded-full bg-white transition-transform transform ${
                    useLocation ? "translate-x-5" : "translate-x-0"
                  }`}
                />
              </div>
            </button>
          </div>
        </div>
        <div className="absolute -bottom-2 left-0 h-1 w-16 bg-primary/60 rounded-full"></div>
      </div>

      {discovery && Array.isArray(discovery) ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
          {discovery.map((artist: DiscoveryArtist, index: number) => (
            <div
              key={index}
              className="group bg-card hover:bg-card/90 rounded-xl overflow-hidden shadow-sm transition-all duration-300 hover:shadow-md hover:translate-y-[-3px] border border-border/40 hover:border-primary/30"
            >
              <div className="relative h-44 overflow-hidden">
                <img
                  src={`https://picsum.photos/400/300?random=${index}`}
                  alt={artist.name}
                  className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-background/90 to-transparent"></div>

                {artist.relevanceScore > 75 && (
                  <div className="absolute top-3 right-3 flex items-center gap-1 bg-primary/80 backdrop-blur-sm px-2.5 py-1 rounded-full">
                    <span className="inline-block w-1.5 h-1.5 bg-white rounded-full animate-pulse"></span>
                    <span className="text-xs font-medium text-white">
                      Rising Star
                    </span>
                  </div>
                )}
              </div>

              <div className="p-4">
                <div className="flex justify-between items-start mb-2">
                  <h3 className="text-xl font-bold line-clamp-1 group-hover:text-primary transition-colors">
                    {artist.name}
                  </h3>
                  {artist.listeners > 0 && (
                    <span className="text-xs bg-secondary px-2 py-0.5 rounded-md text-secondary-foreground font-medium">
                      {artist.listeners.toLocaleString()} listeners
                    </span>
                  )}
                </div>

                {artist.location && (
                  <div className="flex items-center text-sm text-muted-foreground mb-3">
                    <span className="inline-block w-1 h-1 bg-primary/70 rounded-full mr-1.5"></span>
                    {artist.location}
                  </div>
                )}

                <div className="flex flex-wrap gap-1.5 my-2">
                  {artist.genres.slice(0, 3).map((genre, idx) => (
                    <span
                      key={idx}
                      className="px-2.5 py-1 bg-accent hover:bg-accent/80 rounded-full text-xs text-accent-foreground transition-colors"
                    >
                      {genre}
                    </span>
                  ))}
                  {artist.genres.length > 3 && (
                    <span className="px-2.5 py-1 bg-accent hover:bg-accent/80 rounded-full text-xs text-accent-foreground transition-colors">
                      +{artist.genres.length - 3}
                    </span>
                  )}
                </div>

                {artist.similarArtists && artist.similarArtists.length > 0 && (
                  <div className="text-sm mt-3 bg-secondary/40 p-2 rounded-md">
                    <span className="text-primary/80 font-medium">
                      Similar to:{" "}
                    </span>
                    <span className="text-secondary-foreground">
                      {artist.similarArtists.slice(0, 2).join(", ")}
                      {artist.similarArtists.length > 2 && "..."}
                    </span>
                  </div>
                )}

                <div className="mt-4 pt-3 border-t border-border/30 flex justify-between items-center">
                  <div className="flex items-center gap-1">
                    <div
                      className={`w-2 h-2 rounded-full ${
                        artist.isRegistered
                          ? "bg-primary"
                          : "bg-muted-foreground"
                      }`}
                    ></div>
                    <span className="text-xs text-muted-foreground">
                      {artist.isRegistered
                        ? "TuneSpace Artist"
                        : "External Artist"}
                    </span>
                  </div>
                  <button className="text-xs px-3 py-1.5 bg-primary/10 hover:bg-primary/20 text-primary rounded-md transition-colors">
                    Explore
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <div className="flex items-center justify-center p-6 bg-secondary/30 rounded-lg border border-border/50">
          <p className="text-muted-foreground">
            No artist recommendations available
          </p>
        </div>
      )}
    </div>
  );
};

export default DiscoveryList;
