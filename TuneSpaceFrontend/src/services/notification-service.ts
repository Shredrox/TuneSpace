import { ENDPOINTS } from "@/utils/constants";
import httpClient from "./http-client";
import Notification from "@/interfaces/Notification";

export const getUserNotifications = async (
  username: string
): Promise<Notification[]> => {
  const response = await httpClient.get(
    `${ENDPOINTS.NOTIFICATION}/${username}`
  );
  return response.data;
};

export const readUserNotifications = async (username: string) => {
  const response = await httpClient.post(
    `${ENDPOINTS.NOTIFICATION}/${username}/set-read`
  );
  return response.data;
};
