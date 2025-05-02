"use client";

import useLogout from "../hooks/useLogout";
import { ModeToggle } from "@/components/shadcn/mode-toggle";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/components/shadcn/avatar";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/shadcn/dropdown-menu";
import { LogOut, User } from "lucide-react";
import SpotifySearchBar from "./spotify-search-bar";
import { useRouter } from "next/navigation";
import { LuMusic4 } from "react-icons/lu";
import Navigation from "./navigation";
import { SidebarTrigger } from "./shadcn/sidebar";
import useAuth from "@/hooks/useAuth";

const Header = () => {
  const { auth } = useAuth();

  const router = useRouter();
  const logout = useLogout();

  const handleLogout = async () => {
    await logout();
    router.push("/");
  };

  return (
    <header
      className="bg-[#18191b] backdrop-blur 
    border-b supports-[backdrop-filter]:bg-background/60 
    w-full flex justify-center items-center sticky h-16 p-4 top-0 z-50 shrink-0 gap-2 transition-[width,height] ease-linear group-has-[[data-collapsible=icon]]/sidebar-wrapper:h-16"
    >
      <div className="scroll-m-20 text-2xl font-semibold tracking-tight flex justify-between items-center gap-6 w-full">
        <div className="flex justify-center items-center gap-6">
          <SidebarTrigger />
          <div className="flex justify-center items-center gap-1">
            <LuMusic4 />
            <span>TuneSpace</span>
          </div>
          <Navigation />
        </div>
        <div className="flex gap-2">
          <SpotifySearchBar />
          <ModeToggle />
          <DropdownMenu>
            <DropdownMenuTrigger>
              <Avatar>
                <AvatarImage src="https://github.com/shadcn.png" />
                <AvatarFallback>CN</AvatarFallback>
              </Avatar>
            </DropdownMenuTrigger>
            <DropdownMenuContent onCloseAutoFocus={(e) => e.preventDefault()}>
              <DropdownMenuLabel>My Account</DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onClick={() => router.push(`/profile/${auth.username}`)}
              >
                <User className="mr-2 h-4 w-4" />
                <span>Profile</span>
              </DropdownMenuItem>
              <DropdownMenuItem onClick={handleLogout}>
                <LogOut className="mr-2 h-4 w-4" />
                <span>Log out</span>
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>
    </header>
  );
};

export default Header;
