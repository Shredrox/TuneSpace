import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  checkFollowing,
  followUser,
  unfollowUser,
  getUserFollowers,
  getUserFollowing,
  getFollowerCount,
  getFollowingCount,
  FollowData,
} from "@/services/follow-service";
import { toast } from "sonner";
import { NotificationTypes } from "@/utils/constants";
import useSocket from "../useSocket";

const useFollow = (profileUsername: string, loggedInUsername?: string) => {
  const queryClient = useQueryClient();
  const { sendNotification } = useSocket();

  const {
    data: isFollowing,
    isLoading: isFollowingStatusLoading,
    isError: isFollowingStatusError,
  } = useQuery({
    queryKey: ["following-status", loggedInUsername, profileUsername],
    queryFn: () => checkFollowing(profileUsername),
    enabled: !!loggedInUsername && loggedInUsername !== profileUsername,
  });

  const {
    data: followers,
    isLoading: isFollowersLoading,
    isError: isFollowersError,
  } = useQuery({
    queryKey: ["followers", profileUsername],
    queryFn: () => getUserFollowers(profileUsername),
  });

  const {
    data: following,
    isLoading: isFollowingLoading,
    isError: isFollowingError,
  } = useQuery({
    queryKey: ["following", profileUsername],
    queryFn: () => getUserFollowing(profileUsername),
  });

  const {
    data: followerCount,
    isLoading: isFollowerCountLoading,
    isError: isFollowerCountError,
  } = useQuery({
    queryKey: ["follower-count", profileUsername],
    queryFn: () => getFollowerCount(profileUsername),
  });

  const {
    data: followingCount,
    isLoading: isFollowingCountLoading,
    isError: isFollowingCountError,
  } = useQuery({
    queryKey: ["following-count", profileUsername],
    queryFn: () => getFollowingCount(profileUsername),
  });

  const invalidateFollowQueries = () => {
    queryClient.invalidateQueries({
      queryKey: ["following-status", loggedInUsername, profileUsername],
    });
    queryClient.invalidateQueries({
      queryKey: ["followers", profileUsername],
    });
    queryClient.invalidateQueries({
      queryKey: ["following", profileUsername],
    });
    queryClient.invalidateQueries({
      queryKey: ["follower-count", profileUsername],
    });
    queryClient.invalidateQueries({
      queryKey: ["following-count", profileUsername],
    });
  };

  const followMutation = useMutation({
    mutationFn: (data: FollowData) => followUser(data),
    onSuccess: () => {
      invalidateFollowQueries();
      toast.success(`You are now following ${profileUsername}`);

      if (sendNotification) {
        sendNotification({
          message: `${loggedInUsername} started following you`,
          type: NotificationTypes.Follow,
          source: loggedInUsername!,
          recipientName: profileUsername,
        });
      }
    },
    onError: () => {
      toast.error(`Failed to follow ${profileUsername}`);
    },
  });

  const unfollowMutation = useMutation({
    mutationFn: (data: FollowData) => unfollowUser(data),
    onSuccess: () => {
      invalidateFollowQueries();
      toast.success(`You have unfollowed ${profileUsername}`);
    },
    onError: () => {
      toast.error(`Failed to unfollow ${profileUsername}`);
    },
  });

  const handleFollow = () => {
    if (!loggedInUsername) {
      toast.error("You must be logged in to follow users");
      return;
    }

    followMutation.mutate({
      loggedInUser: loggedInUsername,
      profileUser: profileUsername,
    });
  };

  const handleUnfollow = () => {
    if (!loggedInUsername) {
      toast.error("You must be logged in to unfollow users");
      return;
    }

    unfollowMutation.mutate({
      loggedInUser: loggedInUsername,
      profileUser: profileUsername,
    });
  };

  const isLoading =
    isFollowingStatusLoading ||
    isFollowersLoading ||
    isFollowingLoading ||
    isFollowerCountLoading ||
    isFollowingCountLoading;

  const isError =
    isFollowingStatusError ||
    isFollowersError ||
    isFollowingError ||
    isFollowerCountError ||
    isFollowingCountError;

  return {
    isFollowing: isFollowing || false,
    followerCount: followerCount || 0,
    followingCount: followingCount || 0,
    followers: followers || [],
    following: following || [],
    isLoading,
    isError,
    handleFollow,
    handleUnfollow,
  };
};

export default useFollow;
