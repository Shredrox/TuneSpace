"use client";

import { AppSidebar } from "@/components/app-sidebar";
import Header from "@/components/header";
import Loading from "@/components/fallback/loading";
import { SidebarProvider } from "@/components/shadcn/sidebar";
import useAuth from "@/hooks/useAuth";
import useRefreshToken from "@/hooks/useRefreshToken";
import { useEffect, useState } from "react";
import LandingLayout from "@/layouts/landing-layout";
import { useSearchParams } from "next/navigation";

export default function MainClientLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { auth } = useAuth();
  const [isLoading, setIsLoading] = useState(true);
  const { refresh, refreshSpotifyToken } = useRefreshToken();
  const searchParams = useSearchParams();

  const showAuthForm = searchParams.get("auth") === "true";
  const authType = searchParams.get("type");

  useEffect(() => {
    const verifyRefreshToken = async () => {
      try {
        await refresh();
      } catch (error) {
        console.log(error);
      } finally {
        if (
          !auth?.spotifyTokenExpiry ||
          new Date(auth.spotifyTokenExpiry) <= new Date()
        ) {
          verifySpotifyToken();
        } else {
          setIsLoading(false);
        }
      }
    };

    const verifySpotifyToken = async () => {
      try {
        await refreshSpotifyToken();
      } catch (error) {
        console.log(error);
      } finally {
        setIsLoading(false);
      }
    };

    if (!auth?.accessToken) {
      verifyRefreshToken();
    } else if (
      !auth?.spotifyTokenExpiry ||
      new Date(auth.spotifyTokenExpiry) <= new Date()
    ) {
      verifySpotifyToken();
    } else {
      setIsLoading(false);
    }
  }, []);

  if (isLoading) {
    return <Loading />;
  }

  if (auth.accessToken) {
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
        <LandingLayout showAuthForm={showAuthForm} authType={authType}>
          {children}
        </LandingLayout>
      </>
    );
  }
}
