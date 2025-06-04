import { useAuthStore } from "@/stores/auth-store";
import { BASE_URL } from "@/utils/constants";
import axios, { AxiosError, InternalAxiosRequestConfig } from "axios";
import { refreshToken } from "./auth-service";

interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

interface QueueItem {
  resolve: (value?: any) => void;
  reject: (error?: any) => void;
}

const httpClient = axios.create({
  baseURL: BASE_URL,
  withCredentials: true,
});

let isRefreshing = false;
let failedQueue: QueueItem[] = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });

  failedQueue = [];
};

const attachTokenToConfig = (
  config: InternalAxiosRequestConfig,
  token?: string
) => {
  const authToken = token || useAuthStore.getState().auth.accessToken;

  if (authToken && config.headers) {
    config.headers["Authorization"] = `Bearer ${authToken}`;
  }

  return config;
};

httpClient.interceptors.request.use(
  (config) => {
    return attachTokenToConfig(config);
  },
  (error) => {
    return Promise.reject(error);
  }
);

httpClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest: CustomAxiosRequestConfig | undefined = error.config;

    const updateAuth = useAuthStore.getState().updateAuth;
    const clearAuth = useAuthStore.getState().clearAuth;

    if (
      error.response?.status === 401 &&
      originalRequest &&
      !originalRequest._retry
    ) {
      originalRequest._retry = true;

      if (isRefreshing) {
        return new Promise(function (resolve, reject) {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            attachTokenToConfig(originalRequest, token as string);
            return httpClient(originalRequest);
          })
          .catch((err) => {
            return Promise.reject(err);
          });
      }

      isRefreshing = true;

      try {
        const response = await refreshToken();

        updateAuth({ accessToken: response.accessToken });

        processQueue(null, response.accessToken);

        attachTokenToConfig(originalRequest, response.accessToken);

        return httpClient(originalRequest);
      } catch (err) {
        processQueue(err, null);
        clearAuth();
        return Promise.reject(err);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

export default httpClient;
