"use client";

import useAuth from "@/hooks/useAuth";
import AuthenticatedHomeView from "@/components/home/authenticated-home-view";
import UnauthenticatedHomeView from "@/components/home/unauthenticated-home-view";

export default function HomePage() {
  const { auth } = useAuth();

  if (auth.accessToken) {
    return <AuthenticatedHomeView />;
  }

  return <UnauthenticatedHomeView />;
}
