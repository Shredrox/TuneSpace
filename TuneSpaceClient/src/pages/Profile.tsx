import { useParams } from 'react-router-dom'
import { IoMusicalNote } from "react-icons/io5";
import { FaGuitar, FaSpotify } from "react-icons/fa";
import useSpotifyProfileData from '../hooks/query/useSpotifyProfileData';
import Loading from '@/components/fallback/Loading';

const Profile = () => {
  const {username} = useParams();

  const { 
    spotifyProfileData, 
    isSpotifyProfileLoading 
  } = useSpotifyProfileData();

  if(isSpotifyProfileLoading){
    return <Loading/>
  }

  return (
    <div className='text-white p-6 flex items-center gap-4 w-full flex-col'>
      <div className='flex flex-col justify-center items-center gap-4'>
        <img className='rounded-full w-[200px] h-[200px]' src={spotifyProfileData?.profile?.profilePicture} alt="" />
        <h2 className="text-2xl">{spotifyProfileData?.profile?.username} </h2>
      </div>
      <div className='flex flex-col gap-4 bg-gray-800 p-4 rounded-2xl'>
        <span className='flex items-center gap-2'>
          <FaSpotify className="w-8 h-8 text-[#1ed760]"/>Spotify Stats
        </span>
        <div className='flex gap-4'>
          <span>Followers: {spotifyProfileData?.profile?.followerCount}</span>
          <span>Plan: {spotifyProfileData?.profile?.spotifyPlan}</span>
        </div>
        <div className='flex gap-4'>
          <div className='flex flex-col gap-4'>
            <span className='flex items-center gap-2'>
              <FaGuitar /> Top Artists:
            </span>
            {spotifyProfileData?.topArtists?.map((artist, index) => (
              <div key={index} className='flex items-center gap-2'>
                <img className='rounded-full w-[50px] h-[50px] object-cover' src={artist.image} alt="" />
                {artist.name}
              </div>
            ))}
          </div>
          <div className='flex flex-col gap-4'>
            <span className='flex items-center gap-2'>
              <IoMusicalNote /> Top Songs:
            </span>
            {spotifyProfileData?.topSongs?.map((song, index) => (
              <div key={index} className='flex items-center gap-2'>
                <img className='rounded-full w-[50px] h-[50px] object-cover' src={song.image} alt="" />
                <div className='flex flex-col'>
                  {song.name}
                  <span className="text-xs text-gray-400">{song.artist}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  )
}

export default Profile
