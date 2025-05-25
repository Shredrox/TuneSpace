import axios from "axios";
import { NewsApiResponse } from "@/interfaces/news/NewsArticle";

const NEWS_API_KEY = process.env.NEXT_PUBLIC_NEWS_API_KEY || "YOUR_API_KEY";

const newsApiClient = axios.create({
  baseURL: "https://newsapi.org/v2",
});

export const getMusicNews = async (
  page: number = 1,
  pageSize: number = 10
): Promise<NewsApiResponse> => {
  const response = await newsApiClient.get("/everything", {
    params: {
      q: '"music industry" OR "new album" OR "music release" OR "band announce" OR "concert tour" OR "music festival" OR "billboard chart" OR "grammy" OR "music award" OR "artist" OR "songwriter" OR "record label" OR "musician" OR "singer" -politics -crime -weather',
      language: "en",
      domains:
        "rollingstone.com,billboard.com,pitchfork.com,nme.com,consequence.net,stereogum.com,spin.com,loudwire.com,musicradar.com,thefader.com",
      sortBy: "publishedAt",
      pageSize,
      page,
      apiKey: NEWS_API_KEY,
    },
  });
  return response.data;
};
