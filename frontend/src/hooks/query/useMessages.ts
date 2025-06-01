import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";
import useSocket from "../useSocket";
import { getMessages, markMessagesAsRead } from "@/services/chat-service";
import useAuth from "../auth/useAuth";

const useMessages = (chatId: string | undefined) => {
  const queryClient = useQueryClient();
  const { auth } = useAuth();
  const { sendMessage, setChatMessages, messages } = useSocket();

  const {
    data: messagesData,
    isLoading: isMessagesLoading,
    isError: isMessagesError,
    error: messagesError,
  } = useQuery({
    queryKey: ["messages", chatId],
    queryFn: () => getMessages(chatId),
    enabled: !!chatId,
  });

  useEffect(() => {
    if (chatId && auth?.username) {
      markMessagesAsRead(chatId, auth.username).then(() => {
        queryClient.invalidateQueries({
          queryKey: ["chats"],
        });
      });
    }
  }, [chatId, auth?.username]);

  const { mutateAsync: sendMessageMutation } = useMutation({
    mutationFn: sendMessage,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["messages", chatId],
      });
    },
  });

  useEffect(() => {
    if (messagesData) {
      setChatMessages(messagesData);
    }
  }, [messagesData]);

  return {
    messages,
    isMessagesLoading,
    isMessagesError,
    messagesError,
    sendMessageMutation,
  };
};

export default useMessages;
