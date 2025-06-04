import BandShowcase from "@/components/band/band-showcase";
import { getBandById } from "@/services/band-service";
import { getSpotifyArtist } from "@/services/spotify-service";
import { notFound } from "next/navigation";

interface BandShowcasePageProps {
  params: Promise<{ bandId: string }>;
}

export default async function BandShowcasePage({
  params,
}: BandShowcasePageProps) {
  const { bandId } = await params;

  const band = await getBandById(bandId);

  if (!band) {
    notFound();
  }

  let spotifyData = null;
  if (band.spotifyId) {
    try {
      spotifyData = await getSpotifyArtist(band.spotifyId);
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
