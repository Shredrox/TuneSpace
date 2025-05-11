import { ENDPOINTS } from "@/utils/constants";
import httpClient from "./http-client";
import MusicEvent from "@/interfaces/MusicEvent";

export const getEvents = async (): Promise<MusicEvent[]> => {
  const response = await httpClient.get(`${ENDPOINTS.MUSIC_EVENTS}`);
  return response.data;
};

export const getEventsByBandId = async (
  bandId: string
): Promise<MusicEvent[]> => {
  const response = await httpClient.get(
    `${ENDPOINTS.MUSIC_EVENTS}/band/${bandId}`
  );
  return response.data;
};

export const createEvent = async (eventData: FormData): Promise<MusicEvent> => {
  const response = await httpClient.post(
    `${ENDPOINTS.MUSIC_EVENTS}`,
    eventData,
    {
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
  return response.data;
};

export const updateEvent = async (eventData: FormData): Promise<MusicEvent> => {
  const response = await httpClient.put(`${ENDPOINTS.MUSIC_EVENTS}`, eventData);
  return response.data;
};

export const deleteEvent = async (eventId: string): Promise<void> => {
  await httpClient.delete(`${ENDPOINTS.MUSIC_EVENTS}/${eventId}`);
};
