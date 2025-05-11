"use client";

import { useState, useEffect } from "react";
import { Input } from "@/components/shadcn/input";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/components/shadcn/avatar";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/shadcn/card";
import { Search, MessageSquarePlus, User, Loader2 } from "lucide-react";
import { Button } from "@/components/shadcn/button";
import { Badge } from "@/components/shadcn/badge";
import Link from "next/link";
import { format } from "date-fns";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/shadcn/dialog";
import { ScrollArea } from "@/components/shadcn/scroll-area";
import { toast } from "sonner";
import useChats from "@/hooks/query/useChats";
import useAuth from "@/hooks/useAuth";
import { getUsersByNameSearch } from "@/services/user-service";
import UserType from "@/interfaces/user/User";

const ChatList = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [newChatDialogOpen, setNewChatDialogOpen] = useState(false);
  const [userSearchQuery, setUserSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState<UserType[]>([]);
  const [isSearching, setIsSearching] = useState(false);

  const { auth } = useAuth();

  const { chats, addChatMutation } = useChats(auth?.username);

  const filteredChats = chats.filter((chat) =>
    chat.user2Name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  useEffect(() => {
    const searchUsers = async () => {
      if (!userSearchQuery.trim()) {
        setSearchResults([]);
        return;
      }

      setIsSearching(true);
      try {
        const users = await getUsersByNameSearch(userSearchQuery);
        setSearchResults(users);
      } catch (error) {
        console.error("Error searching users:", error);
        toast("Failed to search for users. Please try again.");
        setSearchResults([]);
      } finally {
        setIsSearching(false);
      }
    };

    const timer = setTimeout(() => {
      if (userSearchQuery) searchUsers();
    }, 300);

    return () => clearTimeout(timer);
  }, [userSearchQuery]);

  const startConversation = async (username: string) => {
    try {
      if (!auth?.username) {
        toast("You need to be logged in to start a conversation.");
        return;
      }

      addChatMutation({ user1: auth.username, user2: username });
      setNewChatDialogOpen(false);
    } catch (error) {
      console.error("Error creating conversation:", error);
      toast("Failed to start conversation. Please try again.");
    }
  };

  return (
    <Card className="h-full shadow-md">
      <CardHeader className="pb-2">
        <div className="flex items-center justify-between">
          <CardTitle>Conversations</CardTitle>
          <Dialog open={newChatDialogOpen} onOpenChange={setNewChatDialogOpen}>
            <DialogTrigger asChild>
              <Button size="sm" variant="ghost">
                <MessageSquarePlus className="h-5 w-5" />
              </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-md">
              <DialogHeader>
                <DialogTitle>New Conversation</DialogTitle>
              </DialogHeader>
              <div className="relative mt-2">
                <Search className="absolute left-2 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Search for users..."
                  className="pl-8"
                  value={userSearchQuery}
                  onChange={(e) => setUserSearchQuery(e.target.value)}
                />
              </div>
              <ScrollArea className="mt-2 max-h-72">
                {isSearching ? (
                  <div className="flex justify-center py-4">
                    <Loader2 className="h-6 w-6 animate-spin text-primary" />
                  </div>
                ) : searchResults.length > 0 ? (
                  <div className="space-y-1">
                    {searchResults.map((user) => (
                      <div
                        key={user.id}
                        className="flex items-center gap-3 p-3 rounded-md hover:bg-accent cursor-pointer"
                        onClick={() => startConversation(user.name)}
                      >
                        <Avatar className="h-10 w-10">
                          <AvatarImage />
                          <AvatarFallback>
                            {user.name.charAt(0).toUpperCase()}
                          </AvatarFallback>
                        </Avatar>
                        <div>
                          <p className="font-medium">{user.name}</p>
                          <p className="text-sm text-muted-foreground">
                            @{user.name}
                          </p>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : userSearchQuery ? (
                  <div className="text-center py-4 text-muted-foreground">
                    No users found
                  </div>
                ) : (
                  <div className="text-center py-4 text-muted-foreground">
                    <User className="h-10 w-10 mx-auto mb-2 text-muted-foreground/50" />
                    <p>Search for users to start a conversation</p>
                  </div>
                )}
              </ScrollArea>
            </DialogContent>
          </Dialog>
        </div>
        <div className="relative mt-2">
          <Search className="absolute left-2 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search conversations..."
            className="pl-8"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
      </CardHeader>
      <CardContent className="px-2">
        {filteredChats.length > 0 ? (
          <div className="space-y-1">
            {filteredChats.map((chat) => (
              <Link
                key={chat.id}
                href={`/messages/${chat.id}`}
                className="block"
              >
                <div
                  className={`flex items-center gap-3 p-3 rounded-md hover:bg-accent ${
                    chat.unreadCount > 0 ? "bg-accent/30" : ""
                  }`}
                >
                  <Avatar className="h-12 w-12">
                    <AvatarImage />
                    <AvatarFallback>{chat.user2Name.charAt(0)}</AvatarFallback>
                  </Avatar>
                  <div className="flex-1 min-w-0">
                    <div className="flex justify-between items-center">
                      <h4 className="font-semibold truncate">
                        {chat.user2Name}
                      </h4>
                      <span className="text-xs text-muted-foreground">
                        {format(new Date(chat.lastMessageTime), "h:mm a")}
                      </span>
                    </div>
                    <p className="text-sm text-muted-foreground truncate">
                      {chat.lastMessage}
                    </p>
                  </div>
                  {chat.unreadCount > 0 && (
                    <Badge className="ml-2 bg-primary">
                      {chat.unreadCount}
                    </Badge>
                  )}
                </div>
              </Link>
            ))}
          </div>
        ) : (
          <div className="text-center py-8 text-muted-foreground">
            No conversations found
          </div>
        )}
      </CardContent>
    </Card>
  );
};

export default ChatList;
