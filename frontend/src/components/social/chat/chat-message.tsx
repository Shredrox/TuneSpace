import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/components/shadcn/avatar";
import useAuth from "@/hooks/useAuth";
import Message from "@/interfaces/social/chat/Message";
import { format } from "date-fns";

interface ChatMessageProps {
  message: Message;
}

const ChatMessage = ({ message }: ChatMessageProps) => {
  const { senderName, senderId, content, timestamp } = message;
  const { auth } = useAuth();

  const isCurrentUser = auth?.id === senderId;

  return (
    <div className={`flex ${isCurrentUser ? "justify-end" : "justify-start"}`}>
      <div
        className={`flex gap-3 max-w-[80%] ${
          isCurrentUser ? "flex-row-reverse" : ""
        }`}
      >
        <Avatar className="h-8 w-8">
          <AvatarImage src={`data:image/png;base64,${message.senderAvatar}`} />
          <AvatarFallback>{senderName.charAt(0)}</AvatarFallback>
        </Avatar>
        <div>
          <div
            className={`px-4 py-2 rounded-lg text-sm ${
              isCurrentUser
                ? "bg-primary text-primary-foreground rounded-tr-none"
                : "bg-accent text-accent-foreground rounded-tl-none"
            }`}
          >
            {content}
          </div>
          <div className="text-xs text-muted-foreground mt-1">
            {format(new Date(timestamp), "HH:mm")}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ChatMessage;
