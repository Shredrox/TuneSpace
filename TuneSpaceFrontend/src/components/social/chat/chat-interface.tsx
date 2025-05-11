"use client";

import { useState, useEffect, useRef } from "react";
import { Input } from "@/components/shadcn/input";
import { Button } from "@/components/shadcn/button";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/shadcn/card";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/components/shadcn/avatar";
import { Send, Paperclip } from "lucide-react";
import ChatMessage from "./chat-message";
import ChatPreview from "@/interfaces/social/chat/ChatPreview";
import useMessages from "@/hooks/query/useMessages";
import useAuth from "@/hooks/useAuth";

interface ChatInterfaceProps {
  chat: ChatPreview | undefined;
}

const ChatInterface = ({ chat }: ChatInterfaceProps) => {
  const { messages, sendMessageMutation } = useMessages(chat?.id);

  const { auth } = useAuth();

  const [newMessage, setNewMessage] = useState("");
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const chatUser =
    auth.username == chat?.user1Name ? chat?.user2Name : chat?.user1Name!;
  const chatUserInitial = chatUser?.charAt(0).toUpperCase();

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const handleSendMessage = () => {
    if (
      newMessage.trim() === "" ||
      auth?.id === undefined ||
      auth?.username === undefined
    ) {
      return;
    }

    sendMessageMutation({
      sender: auth.username,
      message: newMessage,
      receiver: chatUser!,
    });
    setNewMessage("");
  };

  return (
    <Card className="flex flex-col h-[calc(100vh-14rem)] min-h-[500px] shadow-md">
      <CardHeader className="border-b py-3">
        <div className="flex items-center gap-3">
          <Avatar>
            <AvatarImage />
            <AvatarFallback>{chatUserInitial}</AvatarFallback>
          </Avatar>
          <CardTitle>{chatUser}</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="flex-1 overflow-auto p-4 flex flex-col gap-4">
        {messages.map((message) => (
          <ChatMessage key={message.id} message={message} />
        ))}
        <div ref={messagesEndRef} />
      </CardContent>
      <div className="border-t p-3 flex gap-2">
        <Button variant="ghost" size="icon" className="shrink-0">
          <Paperclip className="h-5 w-5" />
        </Button>
        <Input
          placeholder="Type a message..."
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSendMessage()}
          className="flex-1"
        />
        <Button onClick={handleSendMessage} disabled={newMessage.trim() === ""}>
          <Send className="h-5 w-5 mr-2" />
          Send
        </Button>
      </div>
    </Card>
  );
};

export default ChatInterface;
