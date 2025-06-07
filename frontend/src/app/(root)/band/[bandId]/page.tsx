import BandShowcase from "@/components/band/band-showcase";
import { getBandById } from "@/services/band-service";
import httpClient from "@/services/http-client";
import { SPOTIFY_ENDPOINTS } from "@/utils/constants";
import { cookies } from "next/headers";
import { notFound } from "next/navigation";

interface BandShowcasePageProps {
  params: Promise<{ bandId: string }>;
}

export default async function BandShowcasePage({
  params,
}: BandShowcasePageProps) {
  const { bandId } = await params;

  const cookie = (await cookies()).get("SpotifyAccessToken");

  const band = await getBandById(bandId);

  if (!band) {
    notFound();
  }

  let spotifyData = null;
  if (band.spotifyId && cookie) {
    try {
      spotifyData = (
        await httpClient.get(`${SPOTIFY_ENDPOINTS.ARTIST}/${band.spotifyId}`, {
          withCredentials: true,
          headers: {
            Cookie: `SpotifyAccessToken=${cookie.value}`,
          },
        })
      ).data;
    } catch (error) {
      console.error("Failed to fetch Spotify data:", error);
    }
  }

  return (
    <div className="container mx-auto py-8 px-4">
      <BandShowcase band={band} spotifyData={spotifyData} />
    </div>
  );
}
