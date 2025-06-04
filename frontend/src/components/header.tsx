"use client";

import { ModeToggle } from "@/components/shadcn/mode-toggle";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/components/shadcn/avatar";
import { Bell } from "lucide-react";
import { SidebarTrigger } from "./shadcn/sidebar";
import Link from "next/link";
import { Button } from "./shadcn/button";
import { Badge } from "./shadcn/badge";
import { useEffect } from "react";
import useNotifications from "@/hooks/query/useNotifications";
import useSocket from "@/hooks/useSocket";
import HeaderSearchBar from "./search/header-search-bar";

const Header = () => {
  const { unreadCount, refetch } = useNotifications();
  const { notifications: socketNotifications } = useSocket();

  useEffect(() => {
    if (socketNotifications.length > 0) {
      refetch();
    }
  }, [socketNotifications, refetch]);

  useEffect(() => {
    const interval = setInterval(() => {
      refetch();
    }, 60000);

    return () => clearInterval(interval);
  }, [refetch]);

  return (
    <header
      className="bg-[#18191b] backdrop-blur 
    border-b supports-[backdrop-filter]:bg-background/60 
    w-full flex justify-center items-center sticky h-16 p-4 top-0 z-50 shrink-0 gap-2 transition-[width,height] ease-linear group-has-[[data-collapsible=icon]]/sidebar-wrapper:h-16"
    >
      <div className="scroll-m-20 text-2xl font-semibold tracking-tight flex items-center gap-6 w-full">
        <div className="flex flex-1 justify-start items-center gap-6">
          <div className="flex justify-center items-center gap-2">
            <Avatar className="h-9 w-9 bg-gradient-to-br from-blue-500 to-purple-600">
              <AvatarImage src="/logo.png" alt="TuneSpace" />
              <AvatarFallback>TS</AvatarFallback>
            </Avatar>
            <span>TuneSpace</span>
          </div>
          <HeaderSearchBar />
        </div>

        <div className="flex gap-2">
          <div className="relative">
            <Button variant="ghost" size="icon" asChild className="h-9 w-9">
              <Link href="/notifications">
                <Bell className="h-4 w-4" />
              </Link>
            </Button>
            {unreadCount > 0 && (
              <div className="absolute -top-1 -right-1">
                <Badge
                  className="h-5 w-5 flex items-center justify-center p-0 text-[10px] animate-in fade-in zoom-in duration-300"
                  variant="destructive"
                >
                  {unreadCount > 9 ? "9+" : unreadCount}
                </Badge>
              </div>
            )}
          </div>
          <ModeToggle />
        </div>
      </div>
    </header>
  );
};

export default Header;
