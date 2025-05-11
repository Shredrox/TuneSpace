"use client";

import ChatList from "@/components/social/chat/chat-list";
import { MessageSquare } from "lucide-react";

export default function MessagesPage() {
  return (
    <div className="container max-w-7xl mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">Messages</h1>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="md:col-span-1">
          <ChatList />
        </div>

        <div className="md:col-span-2 flex items-center justify-center h-[500px] bg-card rounded-lg border shadow-sm">
          <div className="text-center p-8">
            <div className="mx-auto bg-muted rounded-full w-16 h-16 flex items-center justify-center mb-4">
              <MessageSquare className="h-8 w-8 text-muted-foreground" />
            </div>
            <h2 className="text-xl font-medium mb-2">Select a Conversation</h2>
            <p className="text-muted-foreground max-w-sm">
              Choose from your existing conversations or start a new one to
              connect with bands and other music enthusiasts.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
