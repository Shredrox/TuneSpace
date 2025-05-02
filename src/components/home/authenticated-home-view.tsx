import { Tabs, TabsContent, TabsList, TabsTrigger } from "../shadcn/tabs";
import { ListMusic, Clock, Radio, Star, Play } from "lucide-react";
import { Card, CardHeader, CardTitle, CardContent } from "../shadcn/card";
import Link from "next/link";

const AuthenticatedHomeView = () => {
  return (
    <div className="flex flex-col gap-6 py-6">
      <h1 className="text-3xl font-bold px-6">Welcome back!</h1>

      <Tabs defaultValue="overview" className="px-6">
        <TabsList>
          <TabsTrigger value="overview">Overview</TabsTrigger>
          <TabsTrigger value="recent">Recently Played</TabsTrigger>
          <TabsTrigger value="recommended">Recommended</TabsTrigger>
        </TabsList>
        <TabsContent value="overview" className="space-y-6">
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 mt-6">
            <Card className="hover:shadow-md transition-shadow">
              <CardHeader className="pb-2">
                <CardTitle className="flex items-center gap-2">
                  <ListMusic className="h-5 w-5 text-blue-500" /> Your Playlists
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-bold">12</p>
                <p className="text-sm text-muted-foreground">3 new this week</p>
              </CardContent>
            </Card>

            <Card className="hover:shadow-md transition-shadow">
              <CardHeader className="pb-2">
                <CardTitle className="flex items-center gap-2">
                  <Clock className="h-5 w-5 text-purple-500" /> Hours Listened
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-bold">27</p>
                <p className="text-sm text-muted-foreground">This month</p>
              </CardContent>
            </Card>

            <Card className="hover:shadow-md transition-shadow">
              <CardHeader className="pb-2">
                <CardTitle className="flex items-center gap-2">
                  <Radio className="h-5 w-5 text-pink-500" /> Following
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-bold">43</p>
                <p className="text-sm text-muted-foreground">
                  Artists & friends
                </p>
              </CardContent>
            </Card>

            <Card className="hover:shadow-md transition-shadow">
              <CardHeader className="pb-2">
                <CardTitle className="flex items-center gap-2">
                  <Star className="h-5 w-5 text-amber-500" /> Favorites
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-bold">128</p>
                <p className="text-sm text-muted-foreground">Songs & albums</p>
              </CardContent>
            </Card>
          </div>

          <h2 className="text-2xl font-bold mt-8">Continue Listening</h2>
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
            {[1, 2, 3, 4, 5, 6].map((item) => (
              <Link key={item} href="#" className="group">
                <div className="relative aspect-square rounded-md overflow-hidden mb-2">
                  <img
                    src={`/placeholder-album-${item}.jpg`}
                    alt={`Album ${item}`}
                    className="object-cover w-full h-full group-hover:scale-105 transition-transform"
                  />
                  <div className="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                    <Play className="text-white h-10 w-10" />
                  </div>
                </div>
                <h3 className="font-medium truncate">Album Title {item}</h3>
                <p className="text-sm text-muted-foreground truncate">
                  Artist Name
                </p>
              </Link>
            ))}
          </div>
        </TabsContent>

        <TabsContent value="recent">
          <p className="text-muted-foreground py-4">
            Your recently played tracks will appear here.
          </p>
        </TabsContent>

        <TabsContent value="recommended">
          <p className="text-muted-foreground py-4">
            Your personalized recommendations will appear here.
          </p>
        </TabsContent>
      </Tabs>
    </div>
  );
};

export default AuthenticatedHomeView;
