import httpClient from "./http-client";
import { BASE_URL } from "@/utils/constants";
import type {
  RecommendationFeedback,
  BatchRecommendationFeedback,
  EnhancedRecommendationsResponse,
  RecommendationFeedbackResponse,
  BatchRecommendationFeedbackResponse,
} from "@/interfaces/music-discovery";

const baseUrl = `${BASE_URL}/MusicDiscovery`;

export const getRecommendations = async (
  genres?: string[],
  location?: string
): Promise<any[]> => {
  let url = `${baseUrl}/recommendations`;
  const params = new URLSearchParams();

  if (genres && genres.length > 0) {
    params.append("genres", genres.join(","));
  }

  if (location) {
    params.append("location", location);
  }

  if (params.toString()) {
    url += `?${params.toString()}`;
  }

  const response = await httpClient.get(url);
  return response.data;
};

export const getEnhancedRecommendations = async (
  genres?: string[],
  location?: string
): Promise<EnhancedRecommendationsResponse> => {
  let url = `${baseUrl}/recommendations/enhanced`;
  const params = new URLSearchParams();

  if (genres && genres.length > 0) {
    params.append("genres", genres.join(","));
  }

  if (location) {
    params.append("location", location);
  }

  if (params.toString()) {
    url += `?${params.toString()}`;
  }

  const response = await httpClient.get(url);
  return response.data;
};

export const trackRecommendationFeedback = async (
  feedback: RecommendationFeedback
): Promise<RecommendationFeedbackResponse> => {
  const response = await httpClient.post(`${baseUrl}/feedback`, feedback);
  return response.data;
};

export const trackBatchRecommendationFeedback = async (
  batchFeedback: BatchRecommendationFeedback
): Promise<BatchRecommendationFeedbackResponse> => {
  const response = await httpClient.post(
    `${baseUrl}/feedback/batch`,
    batchFeedback
  );
  return response.data;
};
