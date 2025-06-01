"use client";

import { AppSidebar } from "@/components/app-sidebar";
import Header from "@/components/header";
import Loading from "@/components/fallback/loading";
import { SidebarProvider } from "@/components/shadcn/sidebar";
import useLogout from "@/hooks/auth/useLogout";
import useAuthInitialization from "@/hooks/auth/useAuthInitialization";

export default function MainClientLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { isLoading: isLoggingOut } = useLogout();
  const { isInitializing } = useAuthInitialization();

  const isLoading = isInitializing || isLoggingOut;

  if (isLoading) {
    return <Loading />;
  }

  return (
    <SidebarProvider defaultOpen={true}>
      <AppSidebar />
      <main className="relative flex w-full flex-1 flex-col bg-background md:peer-data-[variant=inset]:m-2 md:peer-data-[state=collapsed]:peer-data-[variant=inset]:ml-2 md:peer-data-[variant=inset]:ml-0 md:peer-data-[variant=inset]:rounded-xl md:peer-data-[variant=inset]:shadow">
        <Header />
        <div>{children}</div>
      </main>
    </SidebarProvider>
  );
}
