"use client";

import { useEffect, useState } from "react";
import { Avatar, AvatarFallback, AvatarImage } from "../shadcn/avatar";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "../shadcn/card";
import { Separator } from "../shadcn/separator";
import { FaSpotify, FaYoutube } from "react-icons/fa";
import { SiTidal, SiApplemusic } from "react-icons/si";
import YouTubeEmbedDialog from "./youtube-embed-dialog";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "../shadcn/carousel";
import useAuth from "@/hooks/useAuth";
import useBandData from "@/hooks/query/useBandData";
import Loading from "../fallback/loading";
import useToast from "@/hooks/useToast";
import ConnectSpotifyDialog from "./connect-spotify-dialog";
import EditBandDialog from "./edit-band-dialog";

const BandDashboard = () => {
  const { auth } = useAuth();

  const { bandData, mutations, isBandLoading, isBandError, bandError } =
    useBandData(auth?.id || "");

  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  useEffect(() => {
    if (mounted) {
      useToast(
        "Welcome to the Band Dashboard! Here you can manage your band activities as well as add additional info for your band."
      );
    }
  }, [mounted]);

  const handleSpotifyIdUpdate = async (spotifyId: string) => {
    try {
      const formData = new FormData();
      formData.append("id", bandData.band?.id || "");
      formData.append("spotifyId", spotifyId);

      await mutations.updateBandMutation(formData);

      useToast("Spotify connection successful", 5000);
    } catch (error) {
      useToast(
        `Failed to connect Spotify: ${
          error instanceof Error ? error.message : "Unknown error"
        }`,
        5000
      );
      console.error("Spotify connection error:", error);
    }
  };

  const handleYouTubeEmbedIdUpdate = async (youTubeEmbedId: string) => {
    try {
      const formData = new FormData();
      formData.append("id", bandData.band?.id || "");
      formData.append("youTubeEmbedId", youTubeEmbedId);

      await mutations.updateBandMutation(formData);

      useToast("YouTube video successfully embedded", 5000);
    } catch (error) {
      useToast(
        `Failed to update YouTube embed: ${
          error instanceof Error ? error.message : "Unknown error"
        }`,
        5000
      );
      console.error("YouTube embed update error:", error);
    }
  };

  const handleBandUpdate = async (updatedBand: any) => {
    try {
      const formData = new FormData();
      formData.append("id", bandData.band?.id || "");
      formData.append("name", updatedBand.name || "");
      formData.append("description", updatedBand.description || "");
      formData.append("genre", updatedBand.genre || "");
      formData.append("spotifyId", updatedBand.spotifyId || "");
      formData.append("youTubeEmbedId", updatedBand.youTubeEmbedId || "");

      if (updatedBand.picture) {
        formData.append("picture", updatedBand.picture);
      }

      await mutations.updateBandMutation(formData);

      useToast("Band information updated successfully", 5000);
    } catch (error) {
      useToast(
        `Failed to update band: ${
          error instanceof Error ? error.message : "Unknown error"
        }`,
        5000
      );
      console.error("Band update error:", error);
    }
  };

  if (isBandLoading) {
    return <Loading />;
  }

  return (
    <div className="flex flex-col items-start gap-4 px-4 pb-4">
      <h1 className="text-4xl font-medium ml-4">Dashboard</h1>
      <Card className="w-full p-4 flex flex-col gap-4">
        <CardHeader className="flex flex-row justify-left items-center h-[300px] gap-4">
          <Avatar className="w-[200px] h-[200px]">
            <AvatarImage
              src={`data:image/jpeg;base64,${bandData.band?.coverImage}`}
            />
            <AvatarFallback>BandPicture</AvatarFallback>
          </Avatar>
          <Separator orientation="vertical" />
          <div className="h-full flex flex-col gap-4 justify-left">
            <CardTitle></CardTitle>
            <CardDescription className="flex gap-6 items-start justify-center">
              <div className="flex flex-col gap-2">
                <span
                  className="text-2xl
                  font-semibold
                  leading-none
                  tracking-tight text-white"
                >
                  {bandData.band?.name}
                </span>
                <span className="text-lg">{bandData.band?.description}</span>
                <span className="text-lg">{bandData.band?.genre}</span>
                <span className="text-lg">
                  {bandData.band?.country}, {bandData.band?.city}
                </span>
                <EditBandDialog
                  band={bandData?.band}
                  handleBandUpdate={handleBandUpdate}
                />
              </div>
              {bandData.band?.youTubeEmbedId ? (
                <iframe
                  className="w-[420px] aspect-video rounded-lg"
                  src={`https://www.youtube.com/embed/${bandData.band?.youTubeEmbedId}`}
                  title="YouTube video"
                  allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                  referrerPolicy="strict-origin-when-cross-origin"
                  allowFullScreen
                ></iframe>
              ) : (
                <Card className="p-2 flex-1 flex flex-col items-center justify-center gap-2">
                  <FaYoutube className="text-[#FF0000]" size={50} />
                  <YouTubeEmbedDialog
                    handleYouTubeEmbedIdUpdate={handleYouTubeEmbedIdUpdate}
                  />
                </Card>
              )}
              <Carousel className="ml-11 w-full max-w-sm">
                <CarouselContent className="-ml-1">
                  {Array.from({ length: 5 }).map((_, index) => (
                    <CarouselItem
                      key={index}
                      className="pl-1 md:basis-1/2 lg:basis-1/3"
                    >
                      <div className="p-1 flex flex-col">
                        <Card>
                          <CardContent className="flex aspect-square items-center justify-center p-6">
                            Image
                          </CardContent>
                        </Card>
                      </div>
                    </CarouselItem>
                  ))}
                </CarouselContent>
                <CarouselPrevious />
                <CarouselNext />
              </Carousel>
            </CardDescription>
          </div>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          <div className="flex w-full gap-4">
            <Card className="p-2 flex-1 flex flex-col items-center justify-center">
              <CardHeader className="w-full flex justify-left items-center gap-4">
                <div className="flex gap-4 items-center justify-center w-full">
                  <FaSpotify className="text-[#1DB954]" size={50} />
                  <h3>Spotify Stats</h3>
                </div>
                {bandData.spotifyProfile ? (
                  <>
                    <Avatar className="w-[200px] h-[200px]">
                      <AvatarImage
                        src={bandData.spotifyProfile?.images[0].url}
                      />
                      <AvatarFallback>BandPicture</AvatarFallback>
                    </Avatar>
                    <span>
                      Followers: {bandData.spotifyProfile?.followers.total}
                    </span>
                    <span>
                      Popularity: {bandData.spotifyProfile?.popularity}
                    </span>
                    <iframe
                      className="rounded-3xl"
                      src={`https://open.spotify.com/embed/artist/${bandData.band?.spotifyId}?utm_source=generator&theme=0`}
                      width="100%"
                      height="152"
                      allowFullScreen
                      allow="autoplay; clipboard-write; encrypted-media; fullscreen; picture-in-picture"
                      loading="lazy"
                    ></iframe>
                  </>
                ) : (
                  <ConnectSpotifyDialog
                    handleSpotifyIdUpdate={handleSpotifyIdUpdate}
                  />
                )}
              </CardHeader>
            </Card>
            <Card className="p-2 flex-1 flex flex-col items-center justify-center">
              <CardHeader className="w-full flex justify-left items-center gap-4">
                <div className="flex gap-4 items-center justify-center w-full">
                  <SiApplemusic size={50} /> <h3>Apple Music Stats</h3>
                </div>
              </CardHeader>
            </Card>
            <Card className="p-2 flex-1 flex flex-col items-center justify-center">
              <CardHeader className="w-full flex justify-left items-center gap-4">
                <div className="flex gap-4 items-center justify-center w-full">
                  <SiTidal size={50} /> <h3>Tidal Stats</h3>
                </div>
              </CardHeader>
            </Card>
          </div>
          <Carousel></Carousel>
        </CardContent>
        <Card>
          <CardHeader>
            <CardTitle>Band Members</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col gap-4">
              {/* {bandData.band?.members?.map((member: any, index: number) => (
                <Card key={index} className="flex flex-row items-center gap-4">
                  <Avatar className="w-[100px] h-[100px]">
                    <AvatarImage
                      src={`data:image/jpeg;base64,${member.picture}`}
                    />
                    <AvatarFallback>BandMember</AvatarFallback>
                  </Avatar>
                  <div className="flex flex-col gap-2">
                    <span className="text-lg font-semibold">{member.name}</span>
                    <span className="text-lg">{member.role}</span>
                  </div>
                </Card>
              ))} */}
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Upcoming Concerts</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col gap-4">
              {/* {bandData.band?.members?.map((member: any, index: number) => (
                <Card key={index} className="flex flex-row items-center gap-4">
                  <Avatar className="w-[100px] h-[100px]">
                    <AvatarImage
                      src={`data:image/jpeg;base64,${member.picture}`}
                    />
                    <AvatarFallback>BandMember</AvatarFallback>
                  </Avatar>
                  <div className="flex flex-col gap-2">
                    <span className="text-lg font-semibold">{member.name}</span>
                    <span className="text-lg">{member.role}</span>
                  </div>
                </Card>
              ))} */}
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Merchandise</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col gap-4">
              {/* {bandData.band?.members?.map((member: any, index: number) => (
                <Card key={index} className="flex flex-row items-center gap-4">
                  <Avatar className="w-[100px] h-[100px]">
                    <AvatarImage
                      src={`data:image/jpeg;base64,${member.picture}`}
                    />
                    <AvatarFallback>BandMember</AvatarFallback>
                  </Avatar>
                  <div className="flex flex-col gap-2">
                    <span className="text-lg font-semibold">{member.name}</span>
                    <span className="text-lg">{member.role}</span>
                  </div>
                </Card>
              ))} */}
            </div>
          </CardContent>
        </Card>
      </Card>
    </div>
  );
};

export default BandDashboard;
