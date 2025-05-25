"use client";

import { ReactNode, createContext, useEffect, useState } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import useAuth from "@/hooks/useAuth";
import SocketContextType from "@/interfaces/context/SocketContextType";
import httpClient from "@/services/http-client";
import Message from "@/interfaces/social/chat/Message";
import { SIGNALR_HUB_URL } from "@/utils/constants";
import Notification from "@/interfaces/Notification";
import Chat from "@/interfaces/social/chat/Chat";
import ChatPreview from "@/interfaces/social/chat/ChatPreview";

interface SocketProviderProps {
  children: ReactNode;
}

export const SocketContext = createContext<SocketContextType | null>(null);

const SocketProvider = ({ children }: SocketProviderProps) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);

  const { auth } = useAuth();

  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [messages, setMessages] = useState<Message[]>([]);
  const [chats, setChats] = useState<ChatPreview[]>([]);

  const createHubConnection = () => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(SIGNALR_HUB_URL, { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  };

  const disconnectFromHub = () => {
    if (connection) {
      connection.stop();
      setConnection(null);
      console.log("Disconnected from SignalR hub");
    }
  };

  useEffect(() => {
    if (auth.username !== undefined && connection === null) {
      createHubConnection();
    }
  }, [auth.username]);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => console.log("SignalR Connected"))
        .catch((error) => console.log("SignalR Connection Error: ", error));

      connection.on("ReceiveMessage", onMessageReceived);
      connection.on("ChatCreated", onChatCreated);
      connection.on("ReceiveNotification", onNotificationReceived);
    }
  }, [connection]);

  // Chats
  const onMessageReceived = (message: Message) => {
    setMessages((prevMessages) => [...prevMessages, message]);
  };

  const sendMessage = async ({
    message,
    sender,
    receiver,
  }: {
    message: string;
    sender: string;
    receiver: string;
  }) => {
    try {
      const chatMessage = {
        sender: sender,
        content: message,
        receiver: receiver,
      };

      await httpClient.post("Chat/send-private-message", chatMessage);
    } catch (error) {
      console.error("Error sending message:", error);
    }
  };

  const setChatMessages = (newMessages: Message[]) => {
    setMessages(newMessages);
  };

  const onChatCreated = (chat: ChatPreview) => {
    setChats((prevChats) => [...prevChats, chat]);
  };

  const createChat = async (chat: Chat) => {
    try {
      const newChat = {
        user1Name: chat.user1,
        user2Name: chat.user2,
      };
      await httpClient.post("Chat/create-chat", newChat);
    } catch (error) {
      console.error("Error creating chat:", error);
    }
  };

  const setUserChats = (newUserChats: ChatPreview[]) => {
    setChats(newUserChats);
  };

  // Notifications
  const sendNotification = async ({
    message,
    type,
    source,
    recipientName,
  }: {
    message: string;
    type: string;
    source: string;
    recipientName: string;
  }) => {
    try {
      const notification = {
        message,
        type,
        source,
        recipientName,
      };

      await httpClient.post("Notification/send-notification", notification);
    } catch (error) {
      console.error("Error sending notification:", error);
    }
  };

  const onNotificationReceived = (notification: Notification) => {
    setNotifications((prevNotifications) => [
      ...prevNotifications,
      notification,
    ]);
  };

  const setUserNotifications = (newNotifications: Notification[]) => {
    setNotifications(newNotifications);
  };

  const contextValue: SocketContextType = {
    notifications,
    sendNotification,
    setUserNotifications,
    createHubConnection,
    disconnectFromHub,
    messages,
    sendMessage,
    setChatMessages,
    createChat,
    setUserChats,
    chats,
  };

  return (
    <SocketContext.Provider value={contextValue}>
      {children}
    </SocketContext.Provider>
  );
};

export default SocketProvider;
