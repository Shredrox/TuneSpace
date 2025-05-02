import { getBand, updateBand } from "@/services/band-service";
import { getSpotifyArtist } from "@/services/spotify-service";
import { isNullOrEmpty } from "@/utils/helpers";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

const useBandData = (userId: string) => {
  const queryClient = useQueryClient();

  const {
    data: band,
    isLoading: isBandDataLoading,
    isError: isBandDataError,
    error: bandDataError,
  } = useQuery({
    queryKey: ["band", userId],
    queryFn: () => getBand(userId),
    enabled: !!userId,
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

  const isBandLoading = isBandDataLoading || isSpotifyProfileLoading;
  const isBandError = isBandDataError || isSpotifyProfileError;
  const bandError = bandDataError || spotifyProfileError;

  return {
    bandData: { band, spotifyProfile },
    mutations: { updateBandMutation },
    isBandLoading,
    isBandError,
    bandError,
  };
};

export default useBandData;
