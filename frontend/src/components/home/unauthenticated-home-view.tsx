import { Button } from "../shadcn/button";
import {
  Headphones,
  Music,
  TrendingUp,
  Calendar,
  ArrowRight,
  PlayCircle,
  Badge,
  Heart,
  Users,
  Star,
} from "lucide-react";
import {
  Card,
  CardHeader,
  CardContent,
  CardTitle,
  CardDescription,
  CardFooter,
} from "../shadcn/card";
import { Avatar, AvatarFallback, AvatarImage } from "../shadcn/avatar";
import Link from "next/link";

const UnauthenticatedHomeView = () => {
  return (
    <div className="flex flex-col gap-12 pb-12">
      <section className="relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-br from-blue-900/40 via-purple-800/40 to-pink-800/40" />
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_right,theme(colors.blue.700/30),transparent_70%)]" />
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_bottom_left,theme(colors.purple.700/30),transparent_70%)]" />
        <div className="absolute inset-0 bg-black/10" />
        <div className="absolute inset-0 bg-grid-white/5 [mask-image:linear-gradient(0deg,white,rgba(255,255,255,0.5))]" />

        <div className="container relative mx-auto py-24 px-4 flex flex-col items-center text-center gap-6">
          <h1 className="text-4xl md:text-6xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-blue-300 via-purple-300 to-pink-300 drop-shadow-sm">
            Discover Your Sound Universe
          </h1>
          <p className="text-xl max-w-2xl text-gray-100 drop-shadow-sm">
            Connect with artists, discover new music, and join a community of
            music enthusiasts all in one place.
          </p>
          <div className="flex flex-wrap gap-4 justify-center">
            <Button
              size="lg"
              className="gap-2 bg-gradient-to-r from-blue-600 to-purple-700 hover:from-blue-700 hover:to-purple-800 text-white"
            >
              <Headphones className="h-4 w-4" /> Start Listening
            </Button>
            <Button
              size="lg"
              variant="outline"
              asChild
              className="backdrop-blur-sm bg-white/20 border-white/30 hover:bg-white/30 text-white"
            >
              <Link href="/discover" className="gap-2">
                <Music className="h-4 w-4" /> Explore Music
              </Link>
            </Button>
          </div>
        </div>
      </section>

      <section className="container mx-auto px-4">
        <h2 className="text-3xl font-bold mb-8 text-center">Why TuneSpace?</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div className="bg-card rounded-xl p-6 shadow-sm border border-blue-200/30 flex flex-col items-center text-center gap-3 hover:shadow-md transition-all hover:border-blue-300/50">
            <div className="h-12 w-12 rounded-full bg-gradient-to-br from-blue-500/20 to-blue-600/30 flex items-center justify-center">
              <Music className="h-6 w-6 text-blue-500" />
            </div>
            <h3 className="text-xl font-semibold text-blue-500">
              Discover Music
            </h3>
            <p className="text-muted-foreground">
              Find your next favorite track with our personalized
              recommendations.
            </p>
          </div>

          <div className="bg-card rounded-xl p-6 shadow-sm border border-purple-200/30 flex flex-col items-center text-center gap-3 hover:shadow-md transition-all hover:border-purple-300/50">
            <div className="h-12 w-12 rounded-full bg-gradient-to-br from-purple-500/20 to-purple-600/30 flex items-center justify-center">
              <TrendingUp className="h-6 w-6 text-purple-500" />
            </div>
            <h3 className="text-xl font-semibold text-purple-500">
              Follow Trends
            </h3>
            <p className="text-muted-foreground">
              Stay on top of what's hot in the music world with trending charts.
            </p>
          </div>

          <div className="bg-card rounded-xl p-6 shadow-sm border border-pink-200/30 flex flex-col items-center text-center gap-3 hover:shadow-md transition-all hover:border-pink-300/50">
            <div className="h-12 w-12 rounded-full bg-gradient-to-br from-pink-500/20 to-pink-600/30 flex items-center justify-center">
              <Calendar className="h-6 w-6 text-pink-500" />
            </div>
            <h3 className="text-xl font-semibold text-pink-500">Live Events</h3>
            <p className="text-muted-foreground">
              Find concerts and events from your favorite artists near you.
            </p>
          </div>
        </div>
      </section>

      <section className="container mx-auto px-4">
        <div className="flex justify-between items-center mb-8">
          <h2 className="text-3xl font-bold">Popular Artists</h2>
          <Button variant="ghost" className="gap-1 text-blue-500">
            View All <ArrowRight className="h-4 w-4" />
          </Button>
        </div>
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
          {Array.from({ length: 6 }).map((_, idx) => (
            <div
              key={idx}
              className="flex flex-col items-center gap-3 group cursor-pointer"
            >
              <div className="relative overflow-hidden rounded-full aspect-square w-full border-2 border-transparent group-hover:border-blue-500 transition-all">
                <div className="absolute inset-0 bg-gradient-to-br from-blue-500/20 to-purple-500/20 group-hover:opacity-0 transition-opacity" />
                <Avatar className="h-full w-full">
                  <AvatarImage
                    src={`/placeholder-artist-${idx + 1}.jpg`}
                    alt={`Artist ${idx + 1}`}
                    className="object-cover"
                  />
                  <AvatarFallback className="text-3xl">
                    {String.fromCharCode(65 + idx)}
                  </AvatarFallback>
                </Avatar>
                <div className="absolute inset-0 flex items-center justify-center bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity">
                  <PlayCircle className="h-12 w-12 text-white" />
                </div>
              </div>
              <div className="text-center">
                <p className="font-semibold truncate">Artist Name {idx + 1}</p>
                <p className="text-sm text-muted-foreground">
                  {40 + idx * 10}K followers
                </p>
              </div>
            </div>
          ))}
        </div>
      </section>

      <section className="container mx-auto px-4">
        <div className="flex justify-between items-center mb-8">
          <h2 className="text-3xl font-bold">New Releases</h2>
          <Button variant="outline" asChild>
            <Link href="/discover">View All</Link>
          </Button>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
          {Array.from({ length: 4 }).map((_, idx) => (
            <Card
              key={idx}
              className="overflow-hidden border-none bg-transparent group"
            >
              <div className="relative aspect-square overflow-hidden rounded-lg mb-3">
                <img
                  src={`/placeholder-album-${idx + 1}.jpg`}
                  alt={`Album ${idx + 1}`}
                  className="object-cover w-full h-full group-hover:scale-105 transition-transform duration-300"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent opacity-0 group-hover:opacity-100 transition-opacity flex items-end justify-center p-4">
                  <Button
                    size="icon"
                    variant="secondary"
                    className="rounded-full bg-white text-blue-600 hover:bg-white/90"
                  >
                    <PlayCircle className="h-6 w-6" />
                  </Button>
                </div>
              </div>
              <div>
                <h3 className="font-semibold line-clamp-1">
                  Album Title {idx + 1}
                </h3>
                <p className="text-sm text-muted-foreground">
                  Artist Name {idx + 1}
                </p>
              </div>
            </Card>
          ))}
        </div>
      </section>

      <section className="container mx-auto px-4">
        <div className="flex justify-between items-center mb-8">
          <h2 className="text-3xl font-bold">Discover New Music</h2>
          <Button variant="outline" asChild>
            <Link href="/discover">View All</Link>
          </Button>
        </div>
        {/* <DiscoveryList /> */}

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {Array.from({ length: 6 }).map((_, idx) => (
            <Card
              key={idx}
              className="overflow-hidden hover:shadow-md transition-shadow"
            >
              <CardHeader className="p-0">
                <div className="relative aspect-video">
                  <img
                    src={`/placeholder-discovery-${idx + 1}.jpg`}
                    alt={`Discovery ${idx + 1}`}
                    className="object-cover w-full h-full"
                  />
                  <Badge className="absolute top-2 right-2 bg-blue-600">
                    {
                      [
                        "Pop",
                        "Rock",
                        "Jazz",
                        "Hip-Hop",
                        "Electronic",
                        "Classical",
                      ][idx % 6]
                    }
                  </Badge>
                </div>
              </CardHeader>
              <CardContent className="pt-4">
                <CardTitle className="line-clamp-1">
                  Discovery Playlist {idx + 1}
                </CardTitle>
                <CardDescription className="line-clamp-2 mt-1">
                  Discover the best new{" "}
                  {
                    [
                      "Pop",
                      "Rock",
                      "Jazz",
                      "Hip-Hop",
                      "Electronic",
                      "Classical",
                    ][idx % 6]
                  }{" "}
                  music from emerging artists.
                </CardDescription>
              </CardContent>
              <CardFooter className="border-t pt-3">
                <div className="flex justify-between items-center w-full">
                  <div className="flex items-center gap-2">
                    <Avatar className="h-6 w-6">
                      <AvatarImage src={`/placeholder-artist-${idx + 1}.jpg`} />
                      <AvatarFallback>
                        {String.fromCharCode(65 + idx)}
                      </AvatarFallback>
                    </Avatar>
                    <span className="text-sm">by TuneSpace</span>
                  </div>
                  <div className="flex items-center gap-1 text-muted-foreground">
                    <Heart className="h-4 w-4" />
                    <span className="text-xs">{100 + idx * 23}</span>
                  </div>
                </div>
              </CardFooter>
            </Card>
          ))}
        </div>
      </section>

      <section className="container mx-auto px-4">
        <div className="bg-gradient-to-br from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 py-12 rounded-xl px-4 md:px-8">
          <div className="flex flex-col md:flex-row gap-8 items-center">
            <div className="flex-1">
              <h2 className="text-3xl font-bold mb-4">
                Join Our Music Community
              </h2>
              <p className="text-muted-foreground mb-6 max-w-lg">
                Connect with other music enthusiasts, share your favorite
                tracks, and discover new artists curated by people who share
                your taste.
              </p>
              <div className="flex flex-wrap gap-3">
                <div className="flex items-center gap-2 bg-white dark:bg-gray-800 px-3 py-2 rounded-lg shadow-sm">
                  <Users className="h-5 w-5 text-blue-500" />
                  <span className="font-medium">10K+ Members</span>
                </div>
                <div className="flex items-center gap-2 bg-white dark:bg-gray-800 px-3 py-2 rounded-lg shadow-sm">
                  <Star className="h-5 w-5 text-purple-500" />
                  <span className="font-medium">4.9 Rating</span>
                </div>
                <div className="flex items-center gap-2 bg-white dark:bg-gray-800 px-3 py-2 rounded-lg shadow-sm">
                  <Music className="h-5 w-5 text-pink-500" />
                  <span className="font-medium">1M+ Songs</span>
                </div>
              </div>
            </div>
            <div className="flex-1 relative">
              <div className="grid grid-cols-3 gap-2 md:gap-4">
                {Array.from({ length: 6 }).map((_, idx) => (
                  <div
                    key={idx}
                    className={`rounded-lg overflow-hidden ${
                      idx === 0 ? "col-span-2 row-span-2" : ""
                    }`}
                    style={{
                      transform: `rotate(${idx * 2 - 5}deg)`,
                      zIndex: 10 - idx,
                    }}
                  >
                    <img
                      src={`/placeholder-user-${idx + 1}.jpg`}
                      alt={`Community Member ${idx + 1}`}
                      className="object-cover w-full h-full"
                    />
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="container mx-auto px-4">
        <div className="relative rounded-2xl overflow-hidden">
          <div className="absolute inset-0 bg-gradient-to-r from-blue-600 to-purple-600" />
          <div className="absolute inset-0 bg-[url('/noise.png')] opacity-10" />
          <div className="relative p-8 md:p-12 flex flex-col md:flex-row items-center justify-between gap-6">
            <div>
              <h2 className="text-3xl font-bold text-white mb-2">
                Ready to elevate your music experience?
              </h2>
              <p className="text-white/80 max-w-xl">
                Join our community to create playlists, follow artists, and
                discover new music tailored to your taste.
              </p>
            </div>
            <Button
              size="lg"
              className="bg-white text-blue-600 hover:bg-white/90 hover:text-blue-700"
            >
              Get Started
            </Button>
          </div>
        </div>
      </section>
    </div>
  );
};

export default UnauthenticatedHomeView;
