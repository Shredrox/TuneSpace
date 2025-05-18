"use client";

import { useEffect, useState } from "react";
import { Avatar, AvatarFallback, AvatarImage } from "../shadcn/avatar";
import { Card, CardHeader, CardTitle, CardContent } from "../shadcn/card";
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
import { MessageSquare, Users, CalendarDays } from "lucide-react";
import Link from "next/link";
import CreateDiscussionDialog from "../social/create-discussion-dialog";
import AddEventDialog from "../events/add-event-dialog";
import useEvents from "@/hooks/query/useEvents";
import UserSearchDialog from "../social/user-search-dialog";
import UserType from "@/interfaces/user/User";
import { toast } from "sonner";
import AddMerchandiseDialog from "./add-merchandise-dialog";
import useMerchandise from "@/hooks/query/useMerchandise";

const BandDashboard = () => {
  const { auth } = useAuth();

  const { bandData, mutations, isBandLoading, isBandError, bandError } =
    useBandData(auth?.id || "");

  const { bandEvents } = useEvents(bandData?.band?.id || "");
  const {
    merchandise,
    isLoading: isMerchandiseLoading,
    refetch: refetchMerchandise,
  } = useMerchandise(bandData?.band?.id || "");

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

  const handleAddMember = async (user: UserType) => {
    try {
      await mutations.addMemberMutation({
        bandId: bandData.band?.id || "",
        userId: user.id,
      });
      toast(`Added ${user.name} as a band member!`);
    } catch (error) {
      console.error("Error adding band member:", error);
      toast("Failed to add member. Please try again.");
    }
  };

  if (isBandLoading) {
    return <Loading />;
  }

  // Mock fan engagement data for now
  const fanMessages = [
    {
      id: 1,
      name: "JazzLover42",
      message: "When are you coming to NYC?",
      time: "2 hours ago",
    },
    {
      id: 2,
      name: "MusicFan",
      message: "Loved your latest album!",
      time: "1 day ago",
    },
    {
      id: 3,
      name: "ConcertGoer",
      message: "Will there be VIP tickets?",
      time: "2 days ago",
    },
  ];

  return (
    <div className="flex flex-col items-start gap-6 px-6 py-8 max-w-[1400px] mx-auto w-full">
      <div className="flex flex-wrap justify-between items-center w-full mb-2">
        <h1 className="text-4xl font-bold">Band Dashboard</h1>
        <EditBandDialog
          band={bandData?.band}
          handleBandUpdate={handleBandUpdate}
        />
      </div>

      <Card className="w-full shadow-md border-2 border-muted/20">
        <CardHeader className="pb-0">
          <div className="flex flex-col lg:flex-row gap-8">
            <div className="flex flex-col items-center">
              <Avatar className="w-[220px] h-[220px] rounded-lg shadow-md border border-muted">
                <AvatarImage
                  src={`data:image/jpeg;base64,${bandData.band?.coverImage}`}
                  className="object-cover"
                />
                <AvatarFallback className="text-3xl font-semibold bg-secondary/30">
                  {bandData.band?.name?.substring(0, 2) || "BP"}
                </AvatarFallback>
              </Avatar>
            </div>

            <div className="flex flex-col gap-4 flex-1">
              <div>
                <h2 className="text-3xl font-bold mb-1">
                  {bandData.band?.name}
                </h2>
                <p className="text-xl font-medium text-muted-foreground">
                  {bandData.band?.genre}
                </p>
                <p className="text-muted-foreground mt-1">
                  {bandData.band?.country}, {bandData.band?.city}
                </p>
                <p className="mt-4 text-lg">{bandData.band?.description}</p>
              </div>

              <div className="mt-auto">
                {bandData.band?.youTubeEmbedId ? (
                  <div className="mt-4">
                    <h3 className="text-lg font-medium flex items-center gap-2 mb-2">
                      <FaYoutube className="text-red-600" />
                      Featured Video
                    </h3>
                    <iframe
                      className="w-full max-w-[560px] aspect-video rounded-md shadow-sm"
                      src={`https://www.youtube.com/embed/${bandData.band?.youTubeEmbedId}`}
                      title="YouTube video"
                      allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                      referrerPolicy="strict-origin-when-cross-origin"
                      allowFullScreen
                    ></iframe>
                  </div>
                ) : (
                  <div className="flex items-center gap-3 mt-4">
                    <FaYoutube className="text-red-600" size={24} />
                    <YouTubeEmbedDialog
                      handleYouTubeEmbedIdUpdate={handleYouTubeEmbedIdUpdate}
                    />
                  </div>
                )}
              </div>
            </div>

            <div className="lg:max-w-[320px] w-full">
              <h3 className="text-lg font-medium mb-3">Band Gallery</h3>
              <Carousel className="w-full">
                <CarouselContent>
                  {Array.from({ length: 5 }).map((_, index) => (
                    <CarouselItem key={index} className="basis-full">
                      <Card>
                        <CardContent className="flex aspect-square items-center justify-center p-2 bg-muted/20">
                          Image {index + 1}
                        </CardContent>
                      </Card>
                    </CarouselItem>
                  ))}
                </CarouselContent>
                <div className="flex justify-center gap-2 mt-2">
                  <CarouselPrevious className="relative transform-none w-8 h-8" />
                  <CarouselNext className="relative transform-none w-8 h-8" />
                </div>
              </Carousel>
            </div>
          </div>
        </CardHeader>

        <CardContent className="pt-8">
          {/* Music Stats Section */}
          <div className="mb-10">
            <h2 className="text-2xl font-bold mb-6">Music Stats</h2>
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
              <Card className="overflow-hidden h-full">
                <CardHeader className="bg-gradient-to-r from-green-900/20 to-transparent">
                  <div className="flex items-center gap-3">
                    <FaSpotify className="text-[#1DB954]" size={28} />
                    <h3 className="font-semibold text-2xl">Spotify Stats</h3>
                  </div>
                </CardHeader>
                <CardContent className="p-4">
                  {bandData.spotifyProfile ? (
                    <div className="flex flex-col gap-4">
                      <div className="flex gap-4 items-center">
                        <Avatar className="w-[100px] h-[100px]">
                          <AvatarImage
                            src={bandData.spotifyProfile?.images[0].url}
                          />
                          <AvatarFallback>SP</AvatarFallback>
                        </Avatar>
                        <div>
                          <p className="font-medium">
                            Followers:{" "}
                            <span className="font-bold">
                              {bandData.spotifyProfile?.followers.total.toLocaleString()}
                            </span>
                          </p>
                          <p className="font-medium">
                            Popularity:{" "}
                            <span className="font-bold">
                              {bandData.spotifyProfile?.popularity}%
                            </span>
                          </p>
                        </div>
                      </div>
                      <iframe
                        className="rounded-xl mt-2"
                        src={`https://open.spotify.com/embed/artist/${bandData.band?.spotifyId}?utm_source=generator&theme=0`}
                        width="100%"
                        height="152"
                        allowFullScreen
                        allow="autoplay; clipboard-write; encrypted-media; fullscreen; picture-in-picture"
                        loading="lazy"
                      ></iframe>
                    </div>
                  ) : (
                    <div className="flex flex-col items-center justify-center p-6 text-center h-[200px]">
                      <p className="mb-4 text-muted-foreground">
                        Connect your band's Spotify profile
                      </p>
                      <ConnectSpotifyDialog
                        handleSpotifyIdUpdate={handleSpotifyIdUpdate}
                      />
                    </div>
                  )}
                </CardContent>
              </Card>
              <Card className="overflow-hidden h-full">
                <CardHeader className="bg-gradient-to-r from-pink-900/20 to-transparent">
                  <div className="flex items-center gap-3">
                    <SiApplemusic size={28} />
                    <h3 className="font-semibold text-2xl">
                      Apple Music Stats
                    </h3>
                  </div>
                </CardHeader>
                <CardContent className="p-4">
                  <div className="flex flex-col items-center justify-center p-6 text-center h-[200px]">
                    <p className="mb-4 text-muted-foreground">
                      Connect your Apple Music account
                    </p>
                    <button className="bg-black text-white px-4 py-2 rounded-full hover:bg-black/80">
                      Connect Apple Music
                    </button>
                  </div>
                </CardContent>
              </Card>
              <Card className="overflow-hidden h-full">
                <CardHeader className="bg-gradient-to-r from-blue-900/20 to-transparent">
                  <div className="flex items-center gap-3">
                    <SiTidal size={28} />
                    <h3 className="font-semibold text-2xl">Tidal Stats</h3>
                  </div>
                </CardHeader>
                <CardContent className="p-4">
                  <div className="flex flex-col items-center justify-center p-6 text-center h-[200px]">
                    <p className="mb-4 text-muted-foreground">
                      Connect your Tidal account
                    </p>
                    <button className="bg-black text-white px-4 py-2 rounded-full hover:bg-black/80">
                      Connect Tidal
                    </button>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>

          <div className="mb-10">
            <h2 className="text-2xl font-bold mb-6">Fan Community</h2>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <Card className="overflow-hidden">
                <CardHeader className="bg-gradient-to-r from-blue-900/20 to-transparent">
                  <div className="flex items-center justify-between">
                    <CardTitle className="flex items-center gap-2">
                      <MessageSquare className="h-5 w-5" />
                      <span>Fan Messages</span>
                    </CardTitle>
                    <Link href="/messages">
                      <span className="text-sm text-primary hover:text-primary/80">
                        View All
                      </span>
                    </Link>
                  </div>
                </CardHeader>
                <CardContent className="p-4">
                  {fanMessages.map((message) => (
                    <div
                      key={message.id}
                      className="flex items-center gap-3 p-3 border-b last:border-0"
                    >
                      <Avatar className="h-10 w-10">
                        <AvatarFallback>
                          {message.name.charAt(0)}
                        </AvatarFallback>
                      </Avatar>
                      <div className="flex-1 min-w-0">
                        <div className="flex justify-between">
                          <h4 className="font-medium truncate">
                            {message.name}
                          </h4>
                          <span className="text-xs text-muted-foreground">
                            {message.time}
                          </span>
                        </div>
                        <p className="text-sm text-muted-foreground truncate">
                          {message.message}
                        </p>
                      </div>
                    </div>
                  ))}
                  <div className="mt-4 flex justify-center">
                    <Link href="/messages">
                      <span className="text-primary hover:text-primary/80">
                        Respond to Messages
                      </span>
                    </Link>
                  </div>
                </CardContent>
              </Card>

              <Card className="overflow-hidden">
                <CardHeader className="bg-gradient-to-r from-purple-900/20 to-transparent">
                  <div className="flex items-center justify-between">
                    <CardTitle className="flex items-center gap-2">
                      <Users className="h-5 w-5" />
                      <span>Band Forum</span>
                    </CardTitle>
                    <CreateDiscussionDialog
                      buttonText="New Discussion"
                      buttonVariant="outline"
                    />
                  </div>
                </CardHeader>
                <CardContent className="p-4">
                  {bandData?.forumThreads?.map((thread) => (
                    <Link
                      key={thread.id}
                      href={`/forums/${thread.categoryName}/${thread.id}`}
                      className="block"
                    >
                      <div className="flex items-center p-3 border-b last:border-0 hover:bg-accent/40 rounded-md">
                        <div className="flex-1 min-w-0">
                          <h4 className="font-medium">{thread.title}</h4>
                          <div className="flex justify-between">
                            <span className="text-xs text-muted-foreground">
                              {thread.repliesCount} replies
                            </span>
                            <span className="text-xs text-muted-foreground">
                              {new Date(thread.createdAt).toLocaleString(
                                "en-US",
                                {
                                  year: "numeric",
                                  month: "long",
                                  day: "numeric",
                                  hour: "2-digit",
                                  minute: "2-digit",
                                  hour12: false,
                                }
                              )}
                            </span>
                          </div>
                        </div>
                      </div>
                    </Link>
                  ))}
                  <div className="mt-4 flex justify-center">
                    <Link href="/forums">
                      <span className="text-primary hover:text-primary/80">
                        View All Forums
                      </span>
                    </Link>
                  </div>
                </CardContent>
              </Card>
            </div>

            <div className="mt-6">
              <Card className="overflow-hidden">
                <CardHeader className="bg-gradient-to-r from-amber-900/20 to-transparent">
                  <CardTitle className="flex items-center gap-2">
                    <Users className="h-5 w-5" />
                    <span>Fan Engagement</span>
                  </CardTitle>
                </CardHeader>
                <CardContent className="p-4">
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <Card className="bg-accent/30">
                      <CardContent className="p-4 text-center">
                        <h4 className="text-2xl font-bold">2.4K</h4>
                        <p className="text-sm text-muted-foreground">
                          Total Followers
                        </p>
                      </CardContent>
                    </Card>
                    <Card className="bg-accent/30">
                      <CardContent className="p-4 text-center">
                        <h4 className="text-2xl font-bold">156</h4>
                        <p className="text-sm text-muted-foreground">
                          New Messages
                        </p>
                      </CardContent>
                    </Card>
                    <Card className="bg-accent/30">
                      <CardContent className="p-4 text-center">
                        <h4 className="text-2xl font-bold">87%</h4>
                        <p className="text-sm text-muted-foreground">
                          Response Rate
                        </p>
                      </CardContent>
                    </Card>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>

          <div className="mb-10">
            <h2 className="text-2xl font-bold mb-6">Events</h2>
            <Card className="mb-6 overflow-hidden border-muted/20">
              <CardHeader className="bg-muted/10 pb-2">
                <CardTitle className="flex items-center gap-2">
                  <CalendarDays className="h-5 w-5" />
                  <span>Upcoming Concerts</span>
                  <AddEventDialog bandId={bandData.band?.id} />
                </CardTitle>
              </CardHeader>
              <CardContent className="p-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {bandEvents && bandEvents.length > 0 ? (
                    bandEvents.map((event, index) => (
                      <Card key={index} className="flex overflow-hidden">
                        <div className="bg-muted/20 w-[100px] flex items-center justify-center">
                          <div className="text-center">
                            {event.date ? (
                              <>
                                <div className="font-bold text-xl">
                                  {new Date(event.date).getDate()}
                                </div>
                                <div className="text-sm">
                                  {new Date(event.date)
                                    .toLocaleString("default", {
                                      month: "short",
                                    })
                                    .toUpperCase()}
                                </div>
                              </>
                            ) : (
                              <div className="text-sm text-muted-foreground">
                                No date
                              </div>
                            )}
                          </div>
                        </div>
                        <div className="p-3 flex-1">
                          <h4 className="font-semibold">{event.venue}</h4>
                          <p className="text-sm">
                            {event.city}, {event.country}
                          </p>
                          <div className="flex justify-between items-center mt-2">
                            <span className="text-sm text-muted-foreground">
                              {event.date
                                ? new Date(event.date).toLocaleTimeString([], {
                                    hour: "2-digit",
                                    minute: "2-digit",
                                    hour12: false,
                                  })
                                : "Time TBA"}
                            </span>
                            <button className="text-xs px-2 py-1 bg-primary/80 text-primary-foreground rounded">
                              Details
                            </button>
                          </div>
                        </div>
                      </Card>
                    ))
                  ) : (
                    <div className="col-span-full flex flex-col items-center justify-center p-8 text-center">
                      <p className="text-muted-foreground mb-4">
                        No events scheduled yet
                      </p>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>

            <Card className="mb-6 overflow-hidden border-muted/20">
              <CardHeader className="bg-muted/10 pb-2">
                <CardTitle className="flex items-center gap-2">
                  <span>Band Members</span>
                  <UserSearchDialog
                    trigger={
                      <button className="ml-auto text-sm px-3 py-1 bg-primary hover:bg-primary/80 text-primary-foreground rounded-md">
                        Add Member
                      </button>
                    }
                    title="Add Band Member"
                    onSelectUser={handleAddMember}
                    buttonText="Add to Band"
                  />
                </CardTitle>
              </CardHeader>
              <CardContent className="p-4">
                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-4">
                  {bandData.band?.members?.map((member, index) => (
                    <Card key={index} className="flex flex-col overflow-hidden">
                      <div className="bg-muted/20 aspect-square flex items-center justify-center">
                        <Avatar className="w-[220px] h-[220px] rounded-lg shadow-md border border-muted">
                          <AvatarImage
                            src={`data:image/jpeg;base64,${member.profilePicture}`}
                            className="object-cover"
                          />
                          <AvatarFallback className="text-3xl font-semibold bg-secondary/30">
                            {member.name?.charAt(0)}
                          </AvatarFallback>
                        </Avatar>
                      </div>
                      <div className="p-3">
                        <h4 className="font-semibold">{member.name}</h4>
                        <p className="text-sm text-muted-foreground">
                          Role/Instrument
                        </p>
                      </div>
                    </Card>
                  ))}
                </div>
              </CardContent>
            </Card>
            <Card className="overflow-hidden border-muted/20">
              {" "}
              <CardHeader className="bg-muted/10 pb-2">
                <CardTitle className="flex items-center gap-2">
                  <span>Merchandise</span>
                  <AddMerchandiseDialog
                    bandId={bandData?.band?.id || ""}
                    onMerchandiseAdded={refetchMerchandise}
                    trigger={
                      <button className="ml-auto text-sm px-3 py-1 bg-primary hover:bg-primary/80 text-primary-foreground rounded-md">
                        Add Item
                      </button>
                    }
                  />
                </CardTitle>
              </CardHeader>
              <CardContent className="p-4">
                {isMerchandiseLoading ? (
                  <div className="flex justify-center py-8">
                    <Loading />
                  </div>
                ) : merchandise && merchandise.length > 0 ? (
                  <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
                    {merchandise.map((item) => (
                      <Card key={item.id} className="overflow-hidden">
                        <div className="aspect-square bg-muted/20 flex items-center justify-center">
                          {item.imageUrl ? (
                            <img
                              src={item.imageUrl}
                              alt={item.name}
                              className="w-full h-full object-cover"
                            />
                          ) : (
                            "No Image"
                          )}
                        </div>
                        <div className="p-3">
                          <div className="flex justify-between">
                            <h4 className="font-semibold">{item.name}</h4>
                            <span className="font-bold">
                              ${item.price.toFixed(2)}
                            </span>
                          </div>
                          <p className="text-sm text-muted-foreground mt-1">
                            {item.description || "No description"}
                          </p>
                        </div>
                      </Card>
                    ))}
                  </div>
                ) : (
                  <div className="text-center py-8 text-muted-foreground">
                    No merchandise items yet. Add your first item!
                  </div>
                )}
              </CardContent>
            </Card>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default BandDashboard;
