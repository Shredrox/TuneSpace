"use client";

import ChatList from "@/components/social/chat/chat-list";
import { MessageSquare, Music2 } from "lucide-react";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/shadcn/card";
import { Button } from "@/components/shadcn/button";
import useBandChat from "@/hooks/query/useBandChat";
import useAuth from "@/hooks/auth/useAuth";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/components/shadcn/avatar";
import { useRouter } from "next/navigation";

export default function MessagesPage() {
  const { auth } = useAuth();
  const router = useRouter();

  const { chatData, isLoading: bandChatsLoading } = useBandChat({});

  const bandChats = chatData.userChats || [];

  const handleBandChatClick = (bandId: string) => {
    router.push(`/band/${bandId}/chat/new`);
  };

  return (
    <div className="container max-w-7xl mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">Messages</h1>

      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6 h-[calc(100vh-200px)]">
        <div className="lg:col-span-1 flex flex-col space-y-4">
          <Card className="flex-1 min-h-0">
            <CardHeader className="pb-3">
              <CardTitle className="flex items-center gap-2 text-lg">
                <MessageSquare className="h-5 w-5" />
                Direct Messages
              </CardTitle>
            </CardHeader>
            <CardContent className="px-0 flex-1 overflow-hidden">
              <div className="h-full">
                <ChatList />
              </div>
            </CardContent>
          </Card>
          {auth?.id && (
            <Card className="flex-1 min-h-0">
              <CardHeader className="pb-3">
                <CardTitle className="flex items-center gap-2 text-lg">
                  <Music2 className="h-5 w-5" />
                  Band Chats
                </CardTitle>
              </CardHeader>
              <CardContent className="flex-1 overflow-hidden">
                {bandChatsLoading ? (
                  <div className="flex items-center justify-center h-full">
                    <div className="text-center">
                      <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary mx-auto mb-3"></div>
                      <p className="text-sm text-muted-foreground">
                        Loading bands...
                      </p>
                    </div>
                  </div>
                ) : bandChats && bandChats.length > 0 ? (
                  <div className="space-y-2 h-full overflow-y-auto">
                    {bandChats.map((chat) => (
                      <div
                        key={chat.id}
                        className="flex items-center gap-3 p-3 rounded-lg hover:bg-muted/50 cursor-pointer transition-colors border border-transparent hover:border-border"
                        onClick={() => handleBandChatClick(chat.bandId)}
                      >
                        <Avatar className="w-12 h-12 ring-2 ring-primary/20">
                          <AvatarImage
                            src={
                              chat.bandAvatar
                                ? `data:image/jpeg;base64,${chat.bandAvatar}`
                                : undefined
                            }
                            className="object-cover"
                          />
                          <AvatarFallback className="bg-gradient-to-br from-primary/20 to-primary/10 text-primary font-semibold">
                            {chat.bandName.substring(0, 2) || "BA"}
                          </AvatarFallback>
                        </Avatar>
                        <div className="flex-1 min-w-0">
                          <p className="font-semibold text-sm truncate">
                            {chat.bandName || "Unknown Band"}
                          </p>
                          <p className="text-xs text-muted-foreground truncate">
                            {chat.lastMessageTime
                              ? `${new Date(
                                  chat.lastMessageTime
                                ).toLocaleDateString()}`
                              : "Start a conversation"}
                          </p>
                        </div>
                        <div className="flex items-center">
                          <Music2 className="h-4 w-4 text-primary/60" />
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <div className="flex items-center justify-center h-full">
                    <div className="text-center max-w-xs">
                      <div className="mx-auto bg-primary/10 rounded-full w-16 h-16 flex items-center justify-center mb-4">
                        <Music2 className="h-8 w-8 text-primary/60" />
                      </div>
                      <p className="text-sm text-muted-foreground mb-4">
                        No band conversations yet. Discover bands to start
                        chatting!
                      </p>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => router.push("/discover")}
                        className="border-primary/20 hover:bg-primary/5"
                      >
                        <Music2 className="h-4 w-4 mr-2" />
                        Discover Bands
                      </Button>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          )}
        </div>
        <div className="lg:col-span-3">
          <Card className="h-full shadow-lg">
            <div className="h-full flex items-center justify-center">
              <div className="text-center p-8 max-w-md">
                <div className="mx-auto bg-gradient-to-br from-primary/10 to-accent/10 rounded-full w-20 h-20 flex items-center justify-center mb-6">
                  <MessageSquare className="h-10 w-10 text-primary/70" />
                </div>
                <h2 className="text-2xl font-semibold mb-3 text-foreground">
                  Select a Conversation
                </h2>
                <p className="text-muted-foreground leading-relaxed">
                  Choose from your direct messages or band conversations to
                  start chatting. Connect with fellow music lovers and discover
                  new sounds together.
                </p>
                <div className="flex items-center justify-center gap-4 mt-6 text-sm text-muted-foreground">
                  <div className="flex items-center gap-2">
                    <MessageSquare className="h-4 w-4" />
                    <span>Direct Messages</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Music2 className="h-4 w-4" />
                    <span>Band Chats</span>
                  </div>
                </div>
              </div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}
