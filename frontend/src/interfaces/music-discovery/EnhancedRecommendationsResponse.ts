import EnhancedRecommendation from "./EnhancedRecommendation";

export default interface EnhancedRecommendationsResponse {
  message: string;
  totalRecommendations: number;
  hasAdaptiveLearning: boolean;
  processedAt: string;
  recommendations: EnhancedRecommendation[];
}
