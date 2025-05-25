import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  checkFollowing,
  followUser,
  getUserProfile,
  unfollowUser,
  uploadProfilePicture,
} from "../../services/user-service";

const useProfileData = (profileUser: string, loggedUser: string) => {
  const queryClient = useQueryClient();

  const {
    data: profile,
    isLoading: isUserProfileLoading,
    isError: isUserProfileError,
    error: userProfileError,
  } = useQuery({
    queryKey: ["profile", profileUser],
    queryFn: () => getUserProfile(profileUser),
  });

  const loggedInUserProfile = loggedUser === profile?.username;

  // const {
  //   data: following,
  //   isLoading: isFollowingLoading,
  //   isError: isFollowingError,
  //   error: followingError,
  // } = useQuery({
  //   queryKey: ["following", loggedUser, profile?.username],
  //   queryFn: () => checkFollowing(loggedUser, profile?.username),
  //   enabled: !loggedInUserProfile && !!profile?.username,
  // });

  // const { mutateAsync: followUserMutation } = useMutation({
  //   mutationFn: followUser,
  //   onSuccess: () => {
  //     queryClient.invalidateQueries({
  //       queryKey: ["following", loggedUser, profile?.username],
  //     });
  //     queryClient.invalidateQueries({
  //       queryKey: ["profile", profileUser],
  //     });
  //   },
  // });

  // const { mutateAsync: unfollowUserMutation } = useMutation({
  //   mutationFn: unfollowUser,
  //   onSuccess: () => {
  //     queryClient.invalidateQueries({
  //       queryKey: ["following", loggedUser, profile?.username],
  //     });
  //     queryClient.invalidateQueries({
  //       queryKey: ["profile", profileUser],
  //     });
  //   },
  // });

  const { mutateAsync: uploadProfilePictureMutation } = useMutation({
    mutationFn: (file: File) => uploadProfilePicture(file, loggedUser),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["profile", profileUser],
      });
    },
  });

  const isProfileError = isUserProfileError;
  const profileError = userProfileError;
  const isProfileLoading = isUserProfileLoading;

  return {
    profile,
    //following,
    isProfileLoading,
    isProfileError,
    profileError,
    //followUserMutation,
    //unfollowUserMutation,
    uploadProfilePictureMutation,
  };
};

export default useProfileData;
