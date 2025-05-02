import Band from "@/interfaces/Band";
import { ENDPOINTS } from "@/utils/constants";
import httpClient from "./http-client";

export const getBand = async (userId: string): Promise<Band> => {
  return (await httpClient.get(`${ENDPOINTS.BAND}/user/${userId}`)).data;
};

export const updateBand = async (updateData: FormData) => {
  await httpClient.put(`${ENDPOINTS.BANDUPDATE}`, updateData);
};
