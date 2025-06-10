import httpClient from "./http-client";
import { BASE_URL } from "@/utils/constants";

//TODO: Extract interfaces
export interface RecommendationFeedback {
  artistName: string;
  interactionType: "like" | "dislike" | "thumbs_up" | "thumbs_down" | "rating";
  rating?: number;
  genres?: string[];
}

export interface BatchRecommendationFeedback {
  interactions: RecommendationFeedback[];
}

export interface EnhancedRecommendation {
  id: string;
  name: string;
  description: string;
  genres: string[];
  location: string;
  relevanceScore: number;
  dataSource: string;
  externalUrls: Record<string, string>;
  similarToArtistName?: string;
  confidenceScore: number;
  isEnhancedAI: boolean;
  isCollaborativeFiltering: boolean;
  adaptiveLearningEnabled: boolean;
}

export interface EnhancedRecommendationsResponse {
  message: string;
  totalRecommendations: number;
  hasAdaptiveLearning: boolean;
  processedAt: string;
  recommendations: EnhancedRecommendation[];
}

export interface RecommendationFeedbackResponse {
  message: string;
  success: boolean;
  timestamp: string;
}

export interface BatchRecommendationFeedbackResponse {
  message: string;
  success: boolean;
  processedCount: number;
  totalCount: number;
  timestamp: string;
}

//TODO: Make not a class
class MusicDiscoveryService {
  private readonly baseUrl = `${BASE_URL}/MusicDiscovery`;

  /**
   * Get regular recommendations
   */
  async getRecommendations(
    genres?: string[],
    location?: string
  ): Promise<any[]> {
    let url = `${this.baseUrl}/recommendations`;
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
  }

  /**
   * Get enhanced AI recommendations with confidence scoring
   */
  async getEnhancedRecommendations(
    genres?: string[],
    location?: string
  ): Promise<EnhancedRecommendationsResponse> {
    let url = `${this.baseUrl}/recommendations/enhanced`;
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
  }

  /**
   * Track single recommendation feedback
   */
  async trackRecommendationFeedback(
    feedback: RecommendationFeedback
  ): Promise<RecommendationFeedbackResponse> {
    const response = await httpClient.post(
      `${this.baseUrl}/feedback`,
      feedback
    );
    return response.data;
  }

  /**
   * Track batch recommendation feedback
   */
  async trackBatchRecommendationFeedback(
    batchFeedback: BatchRecommendationFeedback
  ): Promise<BatchRecommendationFeedbackResponse> {
    const response = await httpClient.post(
      `${this.baseUrl}/feedback/batch`,
      batchFeedback
    );
    return response.data;
  }
}

export const musicDiscoveryService = new MusicDiscoveryService();
export default musicDiscoveryService;
