"use client";

import { useQuery } from "@tanstack/react-query";
import { useEffect, useState, useRef } from "react";
import { Input } from "@/components/shadcn/input";
import { Card } from "@/components/shadcn/card";
import Loading from "./fallback/loading";
import Image from "next/image";
import { getSpotifySongsBySearch } from "@/services/spotify-service";
import { Button } from "@/components/shadcn/button";
import { X, Search } from "lucide-react";

const SpotifySearchBar = () => {
  const [search, setSearch] = useState("");
  const [selectedIndex, setSelectedIndex] = useState(-1);
  const resultsRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  const {
    data: searchSongs,
    isError,
    error,
    refetch,
    isLoading,
  } = useQuery({
    queryKey: ["searchUsers", search],
    queryFn: () => getSpotifySongsBySearch(search),
    enabled: false,
    retry: 1,
  });

  useEffect(() => {
    const queryDelay = setTimeout(() => {
      if (search !== "") {
        refetch();
        setSelectedIndex(-1);
      }
    }, 500);

    return () => clearTimeout(queryDelay);
  }, [search, refetch]);

  const handleClearSearch = () => {
    setSearch("");
    inputRef.current?.focus();
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (!searchSongs?.length) return;

    if (e.key === "ArrowDown") {
      e.preventDefault();
      setSelectedIndex((prev) =>
        prev < searchSongs.length - 1 ? prev + 1 : prev
      );
    } else if (e.key === "ArrowUp") {
      e.preventDefault();
      setSelectedIndex((prev) => (prev > 0 ? prev - 1 : -1));
    } else if (e.key === "Enter" && selectedIndex >= 0) {
      setSearch("");
    } else if (e.key === "Escape") {
      setSearch("");
    }
  };

  useEffect(() => {
    if (selectedIndex >= 0 && resultsRef.current) {
      const selectedElement = resultsRef.current.children[
        selectedIndex
      ] as HTMLElement;
      if (selectedElement) {
        selectedElement.scrollIntoView({
          block: "nearest",
          behavior: "smooth",
        });
      }
    }
  }, [selectedIndex]);

  return (
    <div className="w-[26rem] relative">
      <div className="relative">
        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
        <Input
          ref={inputRef}
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          onKeyDown={handleKeyDown}
          className="pl-10 pr-10 focus-visible:ring-0 focus-visible:ring-offset-0"
          placeholder="Search songs..."
        />
        {search && (
          <Button
            variant="ghost"
            size="sm"
            className="absolute right-2 top-1/2 transform -translate-y-1/2 h-6 w-6 p-0"
            onClick={handleClearSearch}
          >
            <X className="h-4 w-4" />
          </Button>
        )}
      </div>

      {search !== "" && (
        <Card
          className="w-full flex flex-col justify-center gap-2 
          mt-1 max-h-[600px] absolute 
          z-10 p-3 overflow-y-auto rounded-lg shadow-lg"
        >
          <div ref={resultsRef} className="space-y-2">
            {isLoading ? (
              <div className="h-24 flex justify-center items-center">
                <Loading />
              </div>
            ) : searchSongs?.length! > 0 ? (
              searchSongs?.map((song, index) => (
                <Card
                  key={index}
                  onClick={() => setSearch("")}
                  className={`p-3 flex items-center gap-3 cursor-pointer hover:bg-gray-100 transition-colors ${
                    selectedIndex === index ? "bg-gray-100 border-blue-400" : ""
                  }`}
                >
                  <Image
                    className="rounded shadow-sm"
                    src={song.albumArt}
                    alt={`${song.name} album art`}
                    width={50}
                    height={50}
                  />
                  <div className="flex flex-col">
                    <span className="font-medium truncate">{song.name}</span>
                    <span className="text-sm text-gray-500 truncate">
                      {song.artist}
                    </span>
                  </div>
                </Card>
              ))
            ) : searchSongs?.length === 0 ? (
              <div className="p-4 text-center text-gray-500">
                No songs found for "{search}"
              </div>
            ) : isError ? (
              <div className="p-4 text-center text-red-500">
                Error: {error.message}
              </div>
            ) : null}
          </div>
        </Card>
      )}
    </div>
  );
};

export default SpotifySearchBar;
