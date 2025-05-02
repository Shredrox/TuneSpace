import { ENDPOINTS } from "@/utils/constants";
import httpClient from "./http-client";
import Event from "@/interfaces/Event";

export const getEvents = async (): Promise<Event[]> => {
  const response = await httpClient.get(`${ENDPOINTS.EVENTS}`);
  return response.data;
};

export const getEventsByBandId = async (bandId: string): Promise<Event[]> => {
  const response = await httpClient.get(`${ENDPOINTS.EVENTS}/band/${bandId}`);
  return response.data;
};

export const createEvent = async (eventData: FormData): Promise<Event> => {
  const response = await httpClient.post(`${ENDPOINTS.EVENTS}`, eventData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return response.data;
};

export const updateEvent = async (eventData: FormData): Promise<Event> => {
  const response = await httpClient.put(`${ENDPOINTS.EVENTS}`, eventData);
  return response.data;
};

export const deleteEvent = async (eventId: string): Promise<void> => {
  await httpClient.delete(`${ENDPOINTS.EVENTS}/${eventId}`);
};
