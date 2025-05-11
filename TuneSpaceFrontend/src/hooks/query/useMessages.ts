import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";
import useSocket from "../useSocket";
import { getMessages } from "@/services/chat-service";

const useMessages = (chatId: string | undefined) => {
  const queryClient = useQueryClient();

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
