"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/shadcn/dialog";
import { Button } from "@/components/shadcn/button";
import { Badge } from "@/components/shadcn/badge";
import { ExternalLink, Users, Star } from "lucide-react";
import { searchSpotifyArtists } from "@/services/spotify-service";
import Loading from "@/components/fallback/loading";
import Image from "next/image";
import SpotifyArtist from "@/interfaces/spotify/SpotifyArtist";

interface ExternalArtistModalProps {
  isOpen: boolean;
  onClose: () => void;
  artistName: string;
}

const ExternalArtistModal = ({
  isOpen,
  onClose,
  artistName,
}: ExternalArtistModalProps) => {
  const [selectedArtist, setSelectedArtist] = useState<SpotifyArtist | null>(
    null
  );

  const {
    data: searchResults,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["searchArtists", artistName],
    queryFn: () => searchSpotifyArtists(artistName),
    enabled: isOpen && !!artistName,
    retry: 1,
  });

  const handleArtistSelect = (artist: SpotifyArtist) => {
    setSelectedArtist(artist);
  };

  const handleOpenSpotify = (artist: SpotifyArtist) => {
    const spotifyUrl = `https://open.spotify.com/artist/${artist.id}`;
    window.open(spotifyUrl, "_blank", "noopener,noreferrer");
  };

  const formatFollowerCount = (count: number) => {
    if (count >= 1000000) {
      return `${(count / 1000000).toFixed(1)}M`;
    } else if (count >= 1000) {
      return `${(count / 1000).toFixed(1)}K`;
    }
    return count.toString();
  };

  const renderContent = () => {
    if (isLoading) {
      return (
        <div className="flex justify-center items-center h-48">
          <Loading />
        </div>
      );
    }

    if (error) {
      return (
        <div className="text-center py-8">
          <p className="text-red-500 mb-4">Failed to search for artists</p>
          <Button variant="outline" onClick={onClose}>
            Close
          </Button>
        </div>
      );
    }

    if (!searchResults?.length) {
      return (
        <div className="text-center py-8">
          <p className="text-gray-500 mb-4">
            No artists found for "{artistName}"
          </p>
          <Button variant="outline" onClick={onClose}>
            Close
          </Button>
        </div>
      );
    }

    if (selectedArtist) {
      return (
        <div className="space-y-6">
          <div className="flex items-start gap-4">
            {selectedArtist.images?.[0] && (
              <Image
                src={selectedArtist.images[0].url}
                alt={selectedArtist.name}
                width={120}
                height={120}
                className="rounded-lg object-cover"
              />
            )}
            <div className="flex-1">
              <h3 className="text-2xl font-bold mb-2">{selectedArtist.name}</h3>
              <div className="flex items-center gap-4 mb-3">
                <div className="flex items-center gap-1">
                  <Users className="w-4 h-4 text-gray-500" />
                  <span className="text-sm text-gray-600">
                    {formatFollowerCount(selectedArtist.followers.total)}{" "}
                    followers
                  </span>
                </div>
                <div className="flex items-center gap-1">
                  <Star className="w-4 h-4 text-yellow-500" />
                  <span className="text-sm text-gray-600">
                    {selectedArtist.popularity}/100 popularity
                  </span>
                </div>
              </div>
              {selectedArtist.genres?.length > 0 && (
                <div className="flex flex-wrap gap-1 mb-4">
                  {selectedArtist.genres.slice(0, 4).map((genre) => (
                    <Badge key={genre} variant="secondary" className="text-xs">
                      {genre}
                    </Badge>
                  ))}
                  {selectedArtist.genres.length > 4 && (
                    <Badge variant="secondary" className="text-xs">
                      +{selectedArtist.genres.length - 4} more
                    </Badge>
                  )}
                </div>
              )}
            </div>
          </div>

          <div className="flex gap-3">
            <Button
              onClick={() => handleOpenSpotify(selectedArtist)}
              className="flex-1"
            >
              <ExternalLink className="w-4 h-4 mr-2" />
              Open in Spotify
            </Button>
            <Button variant="outline" onClick={() => setSelectedArtist(null)}>
              Back to Results
            </Button>
          </div>
        </div>
      );
    }

    return (
      <div className="space-y-4">
        <p className="text-sm text-gray-600 mb-4">
          Select an artist to view more details:
        </p>
        <div className="space-y-3 max-h-96 overflow-y-auto">
          {searchResults.map((artist) => (
            <div
              key={artist.id}
              className="flex items-center gap-3 p-3 border rounded-lg hover:bg-gray-50 cursor-pointer transition-colors"
              onClick={() => handleArtistSelect(artist)}
            >
              {artist.images?.[0] && (
                <Image
                  src={artist.images[0].url}
                  alt={artist.name}
                  width={48}
                  height={48}
                  className="rounded object-cover"
                />
              )}
              <div className="flex-1">
                <h4 className="font-medium">{artist.name}</h4>
                <div className="flex items-center gap-2 text-sm text-gray-500">
                  <span>
                    {formatFollowerCount(artist.followers.total)} followers
                  </span>
                  {artist.genres?.[0] && (
                    <>
                      <span>•</span>
                      <span>{artist.genres[0]}</span>
                    </>
                  )}
                </div>
              </div>
              <Button size="sm" variant="ghost">
                View Details
              </Button>
            </div>
          ))}
        </div>
      </div>
    );
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>
            {selectedArtist ? selectedArtist.name : `Explore "${artistName}"`}
          </DialogTitle>
        </DialogHeader>
        {renderContent()}
      </DialogContent>
    </Dialog>
  );
};

export default ExternalArtistModal;
