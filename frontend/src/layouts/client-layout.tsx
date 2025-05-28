"use client";

import { AppSidebar } from "@/components/app-sidebar";
import Header from "@/components/header";
import Loading from "@/components/fallback/loading";
import { SidebarProvider } from "@/components/shadcn/sidebar";
import useAuth from "@/hooks/useAuth";
import useRefreshToken from "@/hooks/useRefreshToken";
import { useEffect, useState } from "react";
import LandingLayout from "@/layouts/landing-layout";

export default function MainClientLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { auth } = useAuth();
  const { refresh, refreshSpotifyToken } = useRefreshToken();

  const [isLoading, setIsLoading] = useState(true);
  const [isInitialized, setIsInitialized] = useState(false);

  useEffect(() => {
    let isMounted = true;

    const verifyRefreshToken = async () => {
      try {
        await refresh();
        if (!isMounted) {
          return;
        }

        if (
          !auth?.spotifyTokenExpiry ||
          new Date(auth.spotifyTokenExpiry) <= new Date()
        ) {
          await verifySpotifyToken();
        }
      } catch (error) {
        console.log(error);
      } finally {
        if (isMounted) {
          setIsLoading(false);
          setIsInitialized(true);
        }
      }
    };

    const verifySpotifyToken = async () => {
      try {
        await refreshSpotifyToken();
      } catch (error) {
        console.log(error);
      } finally {
        if (isMounted) {
          setIsLoading(false);
          setIsInitialized(true);
        }
      }
    };

    const initializeAuth = async () => {
      if (auth?.accessToken) {
        if (
          !auth?.spotifyTokenExpiry ||
          new Date(auth.spotifyTokenExpiry) <= new Date()
        ) {
          await verifySpotifyToken();
        } else {
          if (isMounted) {
            setIsLoading(false);
            setIsInitialized(true);
          }
        }
      } else {
        await verifyRefreshToken();
      }
    };

    if (!isInitialized) {
      initializeAuth();
    }

    return () => {
      isMounted = false;
    };
  }, [refresh, refreshSpotifyToken, isInitialized]);
  if (isLoading) {
    return <Loading />;
  }

  const hasValidAuth = auth.accessToken && auth.accessToken.length > 0;

  if (hasValidAuth) {
    return (
      <>
        <SidebarProvider defaultOpen={true}>
          <AppSidebar />
          <main className="relative flex w-full flex-1 flex-col bg-background md:peer-data-[variant=inset]:m-2 md:peer-data-[state=collapsed]:peer-data-[variant=inset]:ml-2 md:peer-data-[variant=inset]:ml-0 md:peer-data-[variant=inset]:rounded-xl md:peer-data-[variant=inset]:shadow">
            <Header />
            <div>{children}</div>
          </main>
        </SidebarProvider>
      </>
    );
  } else {
    return (
      <>
        <LandingLayout>{children}</LandingLayout>
      </>
    );
  }
}
