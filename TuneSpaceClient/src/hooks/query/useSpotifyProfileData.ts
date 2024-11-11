import { useQuery } from "@tanstack/react-query"
import { getSpotifyProfile } from "../../services/spotifyService";

const useSpotifyProfileData = () =>{
  const { 
    data: spotifyProfileData, 
    isLoading: isSpotifyProfileLoading, 
    isError: isSpotifyProfileError,
    error: spotifyProfileError 
  } = useQuery({
    queryKey: ['spotify'],
    queryFn: getSpotifyProfile
  });

  return {
    spotifyProfileData, 
    isSpotifyProfileLoading, 
    isSpotifyProfileError, 
    spotifyProfileError
  }
}

export default useSpotifyProfileData
