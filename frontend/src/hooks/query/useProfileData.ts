import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getUserProfile,
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
    isProfileLoading,
    isProfileError,
    profileError,
    uploadProfilePictureMutation,
  };
};

export default useProfileData;
