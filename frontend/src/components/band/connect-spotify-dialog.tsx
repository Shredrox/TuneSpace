"use client";

import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/shadcn/dialog";
import { Button } from "@/components/shadcn/button";
import { Input } from "@/components/shadcn/input";
import { useState } from "react";
import { extractArtistIdFromSpotifyLink } from "@/utils/helpers";

interface ConnectSpotifyDialogProps {
  handleSpotifyIdUpdate: (spotifyId: string) => Promise<void>;
}

const ConnectSpotifyDialog = ({
  handleSpotifyIdUpdate,
}: ConnectSpotifyDialogProps) => {
  const [spotifyArtistLink, setSpotifyArtistLink] = useState("");

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSpotifyArtistLink(e.target.value);
  };

  const handleSpotifyLinkSubmit = async () => {
    const spotifyId = extractArtistIdFromSpotifyLink(spotifyArtistLink);
    if (spotifyId) {
      await handleSpotifyIdUpdate(spotifyId);
    }
  };

  return (
    <Dialog onOpenChange={() => setSpotifyArtistLink("")}>
      <DialogTrigger asChild>
        <Button variant="outline">Connect</Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Connect Spotify Artist</DialogTitle>
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <Input
            type="text"
            placeholder="Enter Spotify Artist Link..."
            value={spotifyArtistLink}
            onChange={handleInputChange}
            className="border p-2 w-full max-w-md rounded"
          />
        </div>
        <DialogFooter>
          <DialogClose asChild>
            <Button type="submit" onClick={handleSpotifyLinkSubmit}>
              Confirm
            </Button>
          </DialogClose>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default ConnectSpotifyDialog;
