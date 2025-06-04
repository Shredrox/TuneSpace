import httpClient from "./http-client";

export interface BandChat {
  id: string;
  userId: string;
  userName: string;
  userAvatar: Uint8Array;
  bandId: string;
  bandName: string;
  bandAvatar: Uint8Array;
  lastMessage: string;
  lastMessageTime?: string;
  unreadCount: number;
  createdAt: string;
}

export interface BandMessage {
  id: string;
  bandChatId: string;
  content: string;
  timestamp: string;
  isRead: boolean;
  isFromBand: boolean;
  senderId?: string;
  senderName?: string;
  senderAvatar?: Uint8Array;
  bandId?: string;
  bandName?: string;
  bandAvatar?: Uint8Array;
}

export interface SendMessageRequest {
  content: string;
}

export const getBandChatById = async (
  chatId: string
): Promise<BandChat | null> => {
  try {
    const response = await httpClient.get(`/BandChat/${chatId}`);
    return response.data;
  } catch (error: any) {
    if (error.response?.status === 404) {
      return null;
    }
    throw error;
  }
};

export const checkBandChat = async (
  bandId: string
): Promise<BandChat | null> => {
  try {
    const response = await httpClient.get(`/BandChat/check/${bandId}`);
    return response.data;
  } catch (error: any) {
    if (error.response?.status === 404) {
      return null;
    }
    throw error;
  }
};

export const startBandChat = async (bandId: string): Promise<BandChat> => {
  const response = await httpClient.post(`/BandChat/start/${bandId}`);
  return response.data;
};

export const getUserBandChats = async (): Promise<BandChat[]> => {
  const response = await httpClient.get("/BandChat/user-chats");
  return response.data;
};

export const getBandChats = async (bandId: string): Promise<BandChat[]> => {
  const response = await httpClient.get(`/BandChat/band-chats/${bandId}`);
  return response.data;
};

export const getChatMessages = async (
  chatId: string,
  skip: number = 0,
  take: number = 50
): Promise<BandMessage[]> => {
  const response = await httpClient.get(
    `/BandChat/${chatId}/messages?skip=${skip}&take=${take}`
  );
  return response.data;
};

export const sendMessageToBand = async (
  bandId: string,
  content: string
): Promise<BandMessage> => {
  const response = await httpClient.post(`/BandChat/${bandId}/send-to-band`, {
    content,
  });
  return response.data;
};

export const sendMessageFromBand = async (
  bandId: string,
  userId: string,
  content: string
): Promise<BandMessage> => {
  const response = await httpClient.post(
    `/BandChat/${bandId}/send-from-band/${userId}`,
    { content }
  );
  return response.data;
};

export const markMessageAsRead = async (messageId: string): Promise<void> => {
  await httpClient.put(`/BandChat/messages/${messageId}/read`);
};

export const getUnreadCount = async (chatId: string): Promise<number> => {
  const response = await httpClient.get(`/BandChat/${chatId}/unread-count`);
  return response.data.count;
};

export const deleteMessage = async (messageId: string): Promise<void> => {
  await httpClient.delete(`/BandChat/messages/${messageId}`);
};

export const deleteBandChat = async (chatId: string): Promise<void> => {
  await httpClient.delete(`/BandChat/${chatId}`);
};
