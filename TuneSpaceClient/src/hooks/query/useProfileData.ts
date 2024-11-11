import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { checkFollowing, followUser, getUserProfile, unfollowUser } from "../../services/userService";

const useProfileData = (profileUser : string, loggedUser : string) =>{
  const queryClient = useQueryClient();

  const {data: profile, 
    isLoading: isUserProfileLoading, 
    isError: isUserProfileError, 
    error: userProfileError
  } = useQuery({ 
    queryKey: ['profile', profileUser],
    queryFn: () => getUserProfile(profileUser),
  });

  const loggedInUserProfile = loggedUser === profile?.username;

  const {data: following, 
    isLoading: isFollowingLoading, 
    isError: isFollowingError, 
    error: followingError
  } = useQuery({
    queryKey: ['following', loggedUser, profile?.username],
    queryFn: () => checkFollowing(loggedUser, profile?.username),
    enabled: !loggedInUserProfile,
  });

  const {mutateAsync: followUserMutation} = useMutation({
    mutationFn: followUser,
    onSuccess: () =>{
      queryClient.invalidateQueries({
        queryKey: ['following', loggedUser, profile?.username]
      });
    },
  });

  const {mutateAsync: unfollowUserMutation} = useMutation({
    mutationFn: unfollowUser,
    onSuccess: () =>{
      queryClient.invalidateQueries({
        queryKey: ['following', loggedUser, profile?.username]
      });
    },
  });

  const isProfileError =  isUserProfileError || isFollowingError;
  const profileError = userProfileError || followingError;
  const isProfileLoading = isUserProfileLoading || isFollowingLoading;

  return {
    profile, 
    following,
    isProfileLoading, 
    isProfileError, 
    profileError,
    followUserMutation,
    unfollowUserMutation
  }
}

export default useProfileData
