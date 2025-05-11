import Notification from "../Notification";
import Chat from "../social/chat/Chat";
import ChatPreview from "../social/chat/ChatPreview";
import Message from "../social/chat/Message";

export default interface SocketContextType {
  notifications: Notification[];
  messages: Message[];
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
  createHubConnection: () => void;
  disconnectFromHub: () => void;
  setChatMessages: (newMessages: Message[]) => void;
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
}
