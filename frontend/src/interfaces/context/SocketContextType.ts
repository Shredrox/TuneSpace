import Notification from "../Notification";
import Chat from "../social/chat/Chat";
import ChatPreview from "../social/chat/ChatPreview";
import Message from "../social/chat/Message";
import { BandChat, BandMessage } from "@/services/band-chat-service";

export default interface SocketContextType {
  notifications: Notification[];
  messages: Message[];
  bandMessages: BandMessage[];
  sendNotification: ({
    message,
    type,
    source,
    recipientName,
  }: {
    message: string;
    type: string;
    source: string;
    recipientName: string;
  }) => Promise<void>;
  setUserNotifications: (notifications: Notification[]) => void;
  setUserChats: (newUserChats: ChatPreview[]) => void;
  setBandChats: (newBandChats: BandChat[]) => void;
  createHubConnection: () => void;
  disconnectFromHub: () => void;
  setChatMessages: (newMessages: Message[]) => void;
  setBandChatMessages: (newMessages: BandMessage[]) => void;
  sendMessage: ({
    message,
    sender,
    receiver,
  }: {
    message: string;
    sender: string;
    receiver: string;
  }) => Promise<void>;
  createChat: (chat: Chat) => Promise<void>;
  chats: ChatPreview[];
  bandChats: BandChat[];
}
