"use client";

import { useState } from "react";
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
import { Search, MessageSquarePlus } from "lucide-react";
import { Button } from "@/components/shadcn/button";
import { Badge } from "@/components/shadcn/badge";
import Link from "next/link";
import { format } from "date-fns";
import { toast } from "sonner";
import useChats from "@/hooks/query/useChats";
import useAuth from "@/hooks/useAuth";
import UserType from "@/interfaces/user/User";
import UserSearchDialog from "../user-search-dialog";

const ChatList = () => {
  const [searchQuery, setSearchQuery] = useState("");

  const { auth } = useAuth();

  const { chats, addChatMutation } = useChats(auth?.username);

  const filteredChats = chats.filter((chat) =>
    chat.user2Name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const startConversation = async (user: UserType) => {
    try {
      if (!auth?.username) {
        toast("You need to be logged in to start a conversation.");
        return;
      }

      addChatMutation({ user1: auth.username, user2: user.name });
      toast(`Started a conversation with ${user.name}`);
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
          <UserSearchDialog
            trigger={
              <Button size="sm" variant="ghost">
                <MessageSquarePlus className="h-5 w-5" />
              </Button>
            }
            title="New Conversation"
            onSelectUser={startConversation}
            buttonText="Start Chat"
          />
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
