import httpClient from "./http-client";
import { ENDPOINTS } from "@/utils/constants";

export interface LoginData {
  email: string;
  password: string;
}

export interface RegisterData {
  name: string;
  email: string;
  password: string;
  role: string;
}

export interface AuthResponse {
  id: string;
  username: string;
  accessToken: string;
  role: string;
}

export const checkCurrentUser = async (): Promise<AuthResponse | null> => {
  try {
    const response = await httpClient.get(ENDPOINTS.CURRENT_USER, {
      withCredentials: true,
    });

    return {
      id: response.data.id,
      username: response.data.username,
      accessToken: "",
      role: response.data.role,
    };
  } catch (error) {
    return null;
  }
};

export const login = async (data: LoginData): Promise<AuthResponse> => {
  const response = await httpClient.post(
    ENDPOINTS.LOGIN,
    JSON.stringify(data),
    {
      headers: { "Content-Type": "application/json" },
    }
  );

  return {
    id: response?.data?.id,
    username: response?.data?.username,
    accessToken: response?.data?.accessToken,
    role: response?.data?.role,
  };
};

export const register = async (data: RegisterData): Promise<{ id: string }> => {
  const response = await httpClient.post(
    ENDPOINTS.REGISTER,
    JSON.stringify(data),
    {
      headers: { "Content-Type": "application/json" },
    }
  );

  return {
    id: response?.data?.id,
  };
};

export const refreshToken = async (): Promise<AuthResponse> => {
  const response = await httpClient.post(
    ENDPOINTS.REFRESH_TOKEN,
    {},
    {
      withCredentials: true,
    }
  );

  return {
    id: response.data.id,
    username: response.data.username,
    accessToken: response.data.newAccessToken,
    role: response.data.role,
  };
};

export const logout = async (): Promise<void> => {
  try {
    await httpClient.post(
      ENDPOINTS.LOGOUT,
      {},
      {
        withCredentials: true,
      }
    );
  } catch (error) {
    console.log(error);
  }
};
