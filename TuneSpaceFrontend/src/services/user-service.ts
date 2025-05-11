import UserProfile from "../interfaces/user/UserProfile";
import httpClient from "./http-client";
import UserType from "@/interfaces/user/User";

interface FollowData {
  loggedInUser: string;
  profileUser: string;
}

export const getUserProfile = async (
  username: string
): Promise<UserProfile> => {
  const response = await httpClient.get(`/User/${username}/profile`);
  return response.data;
};

export const getUserByName = async (username: string) => {
  const response = await httpClient.get(`User/${username}`);
  return response.data;
};

export const getUsersByNameSearch = async (
  username: string
): Promise<UserType[]> => {
  const response = await httpClient.get(`/User/search/${username}`);
  return response.data;
};

export const checkFollowing = async (
  followerUsername?: string,
  followedUsername?: string
): Promise<boolean> => {
  const response = await httpClient.get("/Follow/check-following", {
    params: {
      followerUsername,
      followedUsername,
    },
  });

  return response.data;
};

export const followUser = async ({ loggedInUser, profileUser }: FollowData) => {
  const response = await httpClient.post(`/Follow/create`, {
    loggedInUser,
    profileUser,
  });

  return response.data;
};

export const unfollowUser = async ({
  loggedInUser,
  profileUser,
}: FollowData) => {
  const response = await httpClient.post(`/Follow/delete`, {
    loggedInUser,
    profileUser,
  });

  return response.data;
};
