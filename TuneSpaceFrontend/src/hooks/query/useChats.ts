import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";
import useSocket from "../useSocket";
import { getChats } from "@/services/chat-service";

const useChats = (user: string | undefined) => {
  const { createChat, setUserChats, chats } = useSocket();

  const queryClient = useQueryClient();

  const {
    data: chatsData,
    isLoading: isChatsLoading,
    isError: isChatsError,
    error: chatsError,
  } = useQuery({
    queryKey: ["chats", user],
    queryFn: () => getChats(user),
    enabled: !!user,
  });

  const { mutateAsync: addChatMutation } = useMutation({
    mutationFn: createChat,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["chats", user],
      });
    },
  });

  useEffect(() => {
    if (chatsData) {
      setUserChats(chatsData);
    }
  }, [chatsData]);

  return {
    chats,
    isChatsLoading,
    isChatsError,
    chatsError,
    addChatMutation,
  };
};

export default useChats;
