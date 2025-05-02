import { BASE_URL } from "@/utils/constants";
import axios from "axios";

const httpClient = axios.create({
  baseURL: BASE_URL,
  withCredentials: true,
});

export default httpClient;
