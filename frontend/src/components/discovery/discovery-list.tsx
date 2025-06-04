"use client";

import { useQuery } from "@tanstack/react-query";
import { ENDPOINTS } from "@/utils/constants";
import Loading from "../fallback/loading";
import { useState } from "react";
import httpClient from "@/services/http-client";
import { useRouter } from "next/navigation";
import {
  Music,
  MapPin,
  Users,
  Play,
  ExternalLink,
  Star,
  Headphones,
  TrendingUp,
  Volume2,
  Radio,
  Disc3,
  Waves,
} from "lucide-react";
import { Badge } from "../shadcn/badge";
import { Button } from "../shadcn/button";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "../shadcn/tooltip";
import ExternalArtistModal from "./external-artist-modal";

interface DiscoveryArtist {
  id?: string;
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
  const [isExternalModalOpen, setIsExternalModalOpen] = useState(false);
  const [selectedExternalArtist, setSelectedExternalArtist] =
    useState<string>("");
  const router = useRouter();

  const getGenreColor = (genres: string[]) => {
    const primaryGenre = genres[0]?.toLowerCase() || "";

    if (primaryGenre.includes("rock"))
      return "from-red-500/20 to-orange-500/20 border-red-200/30";
    if (primaryGenre.includes("metal"))
      return "from-gray-600/20 to-slate-800/20 border-gray-300/30";
    if (primaryGenre.includes("pop"))
      return "from-pink-500/20 to-purple-500/20 border-pink-200/30";
    if (primaryGenre.includes("jazz"))
      return "from-amber-500/20 to-yellow-500/20 border-amber-200/30";
    if (primaryGenre.includes("blues"))
      return "from-blue-500/20 to-indigo-500/20 border-blue-200/30";
    if (primaryGenre.includes("folk"))
      return "from-green-500/20 to-emerald-500/20 border-green-200/30";
    if (primaryGenre.includes("electronic"))
      return "from-cyan-500/20 to-teal-500/20 border-cyan-200/30";

    return "from-primary/10 to-primary/20 border-primary/20";
  };

  const getGenreIcon = (genres: string[]) => {
    const primaryGenre = genres[0]?.toLowerCase() || "";

    if (primaryGenre.includes("rock") || primaryGenre.includes("metal"))
      return <Volume2 className="w-5 h-5" />;
    if (primaryGenre.includes("jazz") || primaryGenre.includes("blues"))
      return <Music className="w-5 h-5" />;
    if (primaryGenre.includes("electronic"))
      return <Radio className="w-5 h-5" />;
    if (primaryGenre.includes("folk")) return <Disc3 className="w-5 h-5" />;

    return <Music className="w-5 h-5" />;
  };
  const handleExploreClick = (artist: DiscoveryArtist) => {
    if (artist.isRegistered && artist.id) {
      router.push(`/band/${artist.id}`);
    } else {
      setSelectedExternalArtist(artist.name);
      setIsExternalModalOpen(true);
    }
  };

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
      <div className="flex items-center justify-center p-12 min-h-[400px]">
        <div className="flex flex-col items-center gap-4">
          <div className="relative">
            <div className="p-4 bg-primary/10 rounded-2xl border border-primary/20">
              <Headphones className="w-8 h-8 text-primary animate-pulse" />
            </div>
            <div className="absolute -top-1 -right-1 w-3 h-3 bg-primary rounded-full animate-ping"></div>
          </div>
          <div className="text-center">
            <div className="text-xl font-semibold text-primary/90 mb-2">
              Discovering new artists
            </div>
            <div className="text-sm text-muted-foreground">
              Finding music that matches your taste...
            </div>
          </div>
          <Loading />
        </div>
      </div>
    );

  if (error)
    return (
      <div className="p-8 text-destructive bg-destructive/10 rounded-2xl text-center my-6 border border-destructive/20">
        <div className="flex flex-col items-center gap-3">
          <div className="p-3 bg-destructive/10 rounded-xl">
            <Volume2 className="w-6 h-6 text-destructive" />
          </div>
          <div>
            <div className="font-semibold mb-1">
              Unable to load recommendations
            </div>
            <div className="text-sm text-muted-foreground">
              Please try again later.
            </div>
          </div>
        </div>
      </div>
    );

  return (
    <div className="w-full py-8">
      <div className="relative mb-8">
        <div className="flex justify-between items-start mb-6">
          <div className="space-y-3">
            <div className="flex items-center gap-3">
              <div className="p-3 bg-primary/10 rounded-xl border border-primary/20">
                <Headphones className="w-6 h-6 text-primary" />
              </div>
              <div>
                <h2 className="text-3xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-primary via-primary/80 to-primary/60">
                  Discover New Artists
                </h2>
                <p className="text-muted-foreground mt-1">
                  Explore fresh sounds and hidden gems based on your musical
                  preferences
                </p>
              </div>
            </div>
          </div>{" "}
          <div className="flex items-center gap-3">
            {discovery && Array.isArray(discovery) && (
              <div className="hidden md:flex items-center gap-2 text-sm text-muted-foreground">
                <div className="flex items-center gap-1">
                  <Music className="w-4 h-4" />
                  <span>{discovery.length} artists</span>
                </div>
              </div>
            )}
            <Button
              onClick={() => refetch()}
              variant="outline"
              size="sm"
              className="flex items-center gap-2 bg-card hover:bg-accent transition-colors"
              disabled={isLoading}
            >
              <TrendingUp
                className={`w-4 h-4 ${isLoading ? "animate-spin" : ""}`}
              />
              <span className="hidden sm:inline">Refresh</span>
            </Button>

            <div className="flex items-center gap-2 bg-card p-2 rounded-xl shadow-sm border border-border/40">
              <span className="text-sm text-muted-foreground flex items-center gap-1.5">
                <MapPin className="w-3.5 h-3.5" />
                {useLocation ? "Location: On" : "Location: Off"}
              </span>
              <button
                onClick={handleLocationToggle}
                className="flex items-center"
                aria-label="Toggle location-based recommendations"
              >
                <div
                  className={`w-10 h-5 rounded-full p-0.5 transition-all duration-200 ${
                    useLocation ? "bg-primary shadow-md" : "bg-muted"
                  }`}
                >
                  <div
                    className={`w-4 h-4 rounded-full bg-white transition-transform duration-200 shadow-sm ${
                      useLocation ? "translate-x-5" : "translate-x-0"
                    }`}
                  />
                </div>
              </button>
            </div>
          </div>
        </div>

        <div className="flex items-center gap-4 mb-2">
          <div className="h-1 w-20 bg-gradient-to-r from-primary to-primary/50 rounded-full"></div>
          <div className="flex gap-1">
            <div className="w-2 h-2 bg-primary/60 rounded-full animate-pulse"></div>
            <div
              className="w-2 h-2 bg-primary/40 rounded-full animate-pulse"
              style={{ animationDelay: "0.2s" }}
            ></div>
            <div
              className="w-2 h-2 bg-primary/20 rounded-full animate-pulse"
              style={{ animationDelay: "0.4s" }}
            ></div>
          </div>
        </div>
      </div>
      {discovery && Array.isArray(discovery) ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {discovery.map((artist: DiscoveryArtist, index: number) => (
            <TooltipProvider key={index}>
              <div
                className={`group relative bg-gradient-to-br ${getGenreColor(
                  artist.genres
                )} backdrop-blur-sm rounded-2xl border transition-all duration-300 hover:shadow-xl hover:scale-[1.02] hover:shadow-primary/10 flex flex-col h-full`}
              >
                <div className="relative h-32 overflow-hidden rounded-t-2xl">
                  <div className="absolute inset-0 bg-gradient-to-br from-primary/10 via-transparent to-primary/5">
                    <div className="absolute inset-0 opacity-30">
                      <svg
                        className="w-full h-full"
                        viewBox="0 0 100 20"
                        preserveAspectRatio="none"
                      >
                        <defs>
                          <pattern
                            id={`waves-${index}`}
                            x="0"
                            y="0"
                            width="20"
                            height="20"
                            patternUnits="userSpaceOnUse"
                          >
                            <rect
                              width="1"
                              height="20"
                              x="1"
                              fill="currentColor"
                              opacity="0.1"
                            >
                              <animate
                                attributeName="height"
                                values="5;20;5"
                                dur="1.5s"
                                repeatCount="indefinite"
                                begin={`${index * 0.2}s`}
                              />
                            </rect>
                            <rect
                              width="1"
                              height="20"
                              x="3"
                              fill="currentColor"
                              opacity="0.1"
                            >
                              <animate
                                attributeName="height"
                                values="15;5;15"
                                dur="1.5s"
                                repeatCount="indefinite"
                                begin={`${index * 0.2 + 0.3}s`}
                              />
                            </rect>
                            <rect
                              width="1"
                              height="20"
                              x="5"
                              fill="currentColor"
                              opacity="0.1"
                            >
                              <animate
                                attributeName="height"
                                values="8;18;8"
                                dur="1.5s"
                                repeatCount="indefinite"
                                begin={`${index * 0.2 + 0.6}s`}
                              />
                            </rect>
                          </pattern>
                        </defs>
                        <rect
                          width="100%"
                          height="100%"
                          fill={`url(#waves-${index})`}
                          className="text-primary"
                        />
                      </svg>
                    </div>
                  </div>

                  <div className="absolute top-4 left-4 z-10">
                    <div className="p-2 bg-card/80 backdrop-blur-sm rounded-full border border-border/50">
                      {getGenreIcon(artist.genres)}
                    </div>
                  </div>

                  {artist.relevanceScore > 75 && (
                    <div className="absolute top-4 right-4 z-10">
                      <Badge
                        variant="default"
                        className="bg-primary/90 text-primary-foreground border-0 gap-1.5"
                      >
                        <Star className="w-3 h-3 fill-current" />
                        Rising
                      </Badge>
                    </div>
                  )}

                  <div className="absolute bottom-4 right-4 z-10">
                    <Tooltip>
                      <TooltipTrigger>
                        <div
                          className={`p-1.5 rounded-full ${
                            artist.isRegistered
                              ? "bg-green-500"
                              : "bg-orange-500"
                          } shadow-lg`}
                        >
                          {artist.isRegistered ? (
                            <Music className="w-3 h-3 text-white" />
                          ) : (
                            <ExternalLink className="w-3 h-3 text-white" />
                          )}
                        </div>
                      </TooltipTrigger>
                      <TooltipContent>
                        <p>
                          {artist.isRegistered
                            ? "TuneSpace Artist"
                            : "External Artist"}
                        </p>
                      </TooltipContent>
                    </Tooltip>
                  </div>
                </div>{" "}
                <div className="p-5 space-y-4 flex-1 flex flex-col">
                  <div className="flex-1 space-y-4">
                    <div className="space-y-2">
                      <div className="flex items-start justify-between gap-2">
                        <h3 className="text-lg font-bold text-foreground line-clamp-1 group-hover:text-primary transition-colors">
                          {artist.name}
                        </h3>
                        {artist.popularity > 0 && (
                          <div className="flex items-center gap-1 text-xs text-muted-foreground">
                            <TrendingUp className="w-3 h-3" />
                            {artist.popularity}%
                          </div>
                        )}
                      </div>

                      {artist.location && (
                        <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
                          <MapPin className="w-3.5 h-3.5 text-primary/70" />
                          <span>{artist.location}</span>
                        </div>
                      )}
                    </div>

                    <div className="flex items-center gap-4 text-xs text-muted-foreground">
                      {artist.listeners > 0 && (
                        <div className="flex items-center gap-1">
                          <Users className="w-3.5 h-3.5" />
                          <span>{artist.listeners.toLocaleString()}</span>
                        </div>
                      )}
                      {artist.playCount > 0 && (
                        <div className="flex items-center gap-1">
                          <Play className="w-3.5 h-3.5" />
                          <span>{artist.playCount.toLocaleString()}</span>
                        </div>
                      )}
                    </div>

                    <div className="flex flex-wrap gap-1.5">
                      {artist.genres.slice(0, 3).map((genre, idx) => (
                        <Badge
                          key={idx}
                          variant="secondary"
                          className="text-xs px-2 py-0.5 bg-secondary/50 hover:bg-secondary/70 transition-colors"
                        >
                          {genre}
                        </Badge>
                      ))}
                      {artist.genres.length > 3 && (
                        <Badge
                          variant="outline"
                          className="text-xs px-2 py-0.5"
                        >
                          +{artist.genres.length - 3}
                        </Badge>
                      )}
                    </div>

                    {artist.similarArtists &&
                      artist.similarArtists.length > 0 && (
                        <div className="p-3 bg-secondary/20 rounded-lg border border-border/30">
                          <div className="text-xs font-medium text-primary/80 mb-1 flex items-center gap-1">
                            <Waves className="w-3 h-3" />
                            Similar to:
                          </div>
                          <p className="text-xs text-muted-foreground">
                            {artist.similarArtists.slice(0, 2).join(", ")}
                            {artist.similarArtists.length > 2 && "..."}
                          </p>
                        </div>
                      )}
                  </div>

                  <div className="pt-2 mt-auto">
                    <Button
                      onClick={() => handleExploreClick(artist)}
                      className="w-full bg-primary hover:bg-primary/90 text-primary-foreground rounded-lg transition-all duration-200 group-hover:shadow-md"
                      size="sm"
                    >
                      <div className="flex items-center gap-2">
                        {artist.isRegistered ? (
                          <>
                            <Music className="w-4 h-4" />
                            <span>Explore Artist</span>
                          </>
                        ) : (
                          <>
                            <ExternalLink className="w-4 h-4" />
                            <span>View External</span>
                          </>
                        )}
                      </div>
                    </Button>
                  </div>
                </div>
                {artist.relevanceScore > 0 && (
                  <div className="absolute top-2 left-2">
                    <div
                      className="w-2 h-2 rounded-full bg-primary shadow-lg"
                      style={{
                        opacity: artist.relevanceScore / 100,
                        boxShadow: `0 0 8px hsla(var(--primary), ${
                          artist.relevanceScore / 100
                        })`,
                      }}
                    />
                  </div>
                )}
              </div>
            </TooltipProvider>
          ))}
        </div>
      ) : (
        <div className="flex flex-col items-center justify-center p-12 bg-secondary/20 rounded-2xl border border-border/50">
          <div className="flex flex-col items-center gap-4 text-center max-w-md">
            <div className="p-4 bg-muted/50 rounded-2xl">
              <Music className="w-8 h-8 text-muted-foreground" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-foreground mb-2">
                No recommendations found
              </h3>
              <p className="text-muted-foreground text-sm">
                We couldn't find any artist recommendations at the moment. Try
                adjusting your preferences or check back later.
              </p>
            </div>
            <Button
              onClick={() => refetch()}
              variant="outline"
              size="sm"
              className="mt-2"
            >
              <TrendingUp className="w-4 h-4 mr-2" />
              Try Again
            </Button>
          </div>{" "}
        </div>
      )}{" "}
      <ExternalArtistModal
        isOpen={isExternalModalOpen}
        onClose={() => setIsExternalModalOpen(false)}
        artistName={selectedExternalArtist}
      />
    </div>
  );
};

export default DiscoveryList;
