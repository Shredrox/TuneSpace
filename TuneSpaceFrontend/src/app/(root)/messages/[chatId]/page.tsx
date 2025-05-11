"use client";

import { useParams } from "next/navigation";
import ChatList from "@/components/social/chat/chat-list";
import ChatInterface from "@/components/social/chat/chat-interface";
import useChat from "@/hooks/query/useChat";

export default function MessagesPage() {
  const params = useParams();
  const chatId = params.chatId as string;

  const { chat, isLoading } = useChat(chatId);

  return (
    <div className="container max-w-7xl mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">Messages</h1>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="md:col-span-1">
          <ChatList />
        </div>

        <div className="md:col-span-2">
          {isLoading ? (
            <div className="flex items-center justify-center h-[500px]">
              <p>Loading conversation...</p>
            </div>
          ) : chat ? (
            <ChatInterface chat={chat} />
          ) : (
            <div className="flex items-center justify-center h-[500px]">
              <p>Conversation not found</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
