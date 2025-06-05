import { addMemberToBand, getBand, updateBand } from "@/services/band-service";
import { fetchBandThreads } from "@/services/forum-service";
import { getSpotifyArtist } from "@/services/spotify-service";
import { isNullOrEmpty } from "@/utils/helpers";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import useAuth from "@/hooks/auth/useAuth";

const useBandData = (userId: string) => {
  const queryClient = useQueryClient();
  const { isAuthenticated } = useAuth();

  const {
    data: band,
    isLoading: isBandDataLoading,
    isError: isBandDataError,
    error: bandDataError,
  } = useQuery({
    queryKey: ["band", userId],
    queryFn: () => getBand(userId),
    enabled: isAuthenticated && !!userId,
  });

  const {
    data: forumThreads,
    isLoading: isBandForumThreadsLoading,
    isError: isBandForumThreadsError,
    error: bandForumThreadsError,
  } = useQuery({
    queryKey: ["bandForumThreads", band?.id],
    queryFn: () => fetchBandThreads(band?.id!),
    enabled: !!band && !!band?.id,
  });

  const {
    data: spotifyProfile,
    isLoading: isSpotifyProfileLoading,
    isError: isSpotifyProfileError,
    error: spotifyProfileError,
  } = useQuery({
    queryKey: ["bandSpotify", band?.id],
    queryFn: () => getSpotifyArtist(band?.spotifyId),
    enabled: !!band && !isNullOrEmpty(band.spotifyId),
  });

  const { mutateAsync: updateBandMutation } = useMutation({
    mutationFn: updateBand,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["band", userId],
      });
      queryClient.invalidateQueries({
        queryKey: ["bandSpotify", band?.id],
      });
    },
  });

  const { mutateAsync: addMemberMutation } = useMutation({
    mutationFn: ({ bandId, userId }: { bandId: string; userId: string }) =>
      addMemberToBand(bandId, userId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["band", userId],
      });
    },
  });

  const isBandLoading =
    isBandDataLoading || isSpotifyProfileLoading || isBandForumThreadsLoading;
  const isBandError =
    isBandDataError || isSpotifyProfileError || isBandForumThreadsError;
  const bandError =
    bandDataError || spotifyProfileError || bandForumThreadsError;

  return {
    bandData: { band, spotifyProfile, forumThreads },
    mutations: { updateBandMutation, addMemberMutation },
    isBandLoading,
    isBandError,
    bandError,
  };
};

export default useBandData;
