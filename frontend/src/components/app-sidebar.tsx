import { useState } from "react";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupAction,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuAction,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarSeparator,
} from "@/components/shadcn/sidebar";
import {
  Home,
  Music2,
  Library,
  ListMusic,
  PlusCircle,
  Settings,
  LogOut,
  User,
  Heart,
  Search,
  ChevronRight,
  Headphones,
  MoreHorizontal,
} from "lucide-react";
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
} from "./shadcn/dropdown-menu";

export function AppSidebar() {
  const [activeItem, setActiveItem] = useState("home");

  const handleNavigation = (item: string) => {
    setActiveItem(item);
  };

  return (
    <Sidebar collapsible="icon" className="border-r">
      <SidebarHeader className="pb-2">
        <div className="flex items-center justify-center md:justify-start gap-2 px-2">
          <Avatar className="h-9 w-9 bg-gradient-to-br from-blue-500 to-purple-600">
            <AvatarImage src="/logo.png" alt="TuneSpace" />
            <AvatarFallback>TS</AvatarFallback>
          </Avatar>
          <span className="text-lg font-bold bg-clip-text text-transparent bg-gradient-to-r from-blue-500 to-purple-600">
            TuneSpace
          </span>
        </div>
      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Navigation</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              <SidebarMenuItem>
                <SidebarMenuButton
                  tooltip="Home"
                  isActive={activeItem === "home"}
                  onClick={() => handleNavigation("home")}
                >
                  <Home className="h-4 w-4" />
                  <span>Home</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
              <SidebarMenuItem>
                <SidebarMenuButton
                  tooltip="Search"
                  isActive={activeItem === "search"}
                  onClick={() => handleNavigation("search")}
                >
                  <Search className="h-4 w-4" />
                  <span>Search</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
              <SidebarMenuItem>
                <SidebarMenuButton
                  tooltip="Discover"
                  isActive={activeItem === "discover"}
                  onClick={() => handleNavigation("discover")}
                >
                  <Music2 className="h-4 w-4" />
                  <span>Discover</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
              <SidebarMenuItem>
                <SidebarMenuButton
                  tooltip="Library"
                  isActive={activeItem === "library"}
                  onClick={() => handleNavigation("library")}
                >
                  <Library className="h-4 w-4" />
                  <span>Your Library</span>
                </SidebarMenuButton>
                <SidebarMenuAction>
                  <ChevronRight className="h-4 w-4" />
                </SidebarMenuAction>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>

        <SidebarSeparator />

        <SidebarGroup>
          <SidebarGroupLabel>Your Music</SidebarGroupLabel>
          <SidebarGroupAction>
            <PlusCircle className="h-4 w-4" />
          </SidebarGroupAction>
          <SidebarGroupContent>
            <SidebarMenu>
              <SidebarMenuItem>
                <SidebarMenuButton
                  tooltip="Liked Songs"
                  isActive={activeItem === "liked"}
                  onClick={() => handleNavigation("liked")}
                >
                  <Heart className="h-4 w-4" />
                  <span>Liked Songs</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
              <SidebarMenuItem>
                <SidebarMenuButton
                  tooltip="Playlists"
                  isActive={activeItem === "playlists"}
                  onClick={() => handleNavigation("playlists")}
                >
                  <ListMusic className="h-4 w-4" />
                  <span>Playlists</span>
                </SidebarMenuButton>
                <SidebarMenuAction>
                  <ChevronRight className="h-4 w-4" />
                </SidebarMenuAction>
              </SidebarMenuItem>
              <SidebarMenuItem>
                <SidebarMenuButton
                  tooltip="Recently Played"
                  isActive={activeItem === "recent"}
                  onClick={() => handleNavigation("recent")}
                >
                  <Headphones className="h-4 w-4" />
                  <span>Recently Played</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter>
        <SidebarGroup>
          <SidebarGroupLabel>Account</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              <SidebarMenuItem>
                <SidebarMenuButton
                  tooltip="Profile"
                  isActive={activeItem === "profile"}
                  onClick={() => handleNavigation("profile")}
                >
                  <User className="h-4 w-4" />
                  <span>Profile</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
              <SidebarMenuItem>
                <SidebarMenuButton
                  tooltip="Settings"
                  isActive={activeItem === "settings"}
                  onClick={() => handleNavigation("settings")}
                >
                  <Settings className="h-4 w-4" />
                  <span>Settings</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
              <SidebarMenuItem>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <SidebarMenuButton tooltip="More">
                      <MoreHorizontal className="h-4 w-4" />
                      <span>More</span>
                    </SidebarMenuButton>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end" className="w-48">
                    <DropdownMenuLabel>Account</DropdownMenuLabel>
                    <DropdownMenuItem>Help Center</DropdownMenuItem>
                    <DropdownMenuItem>Account Settings</DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem className="text-red-500">
                      <LogOut className="h-4 w-4 mr-2" />
                      <span>Log Out</span>
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarFooter>
    </Sidebar>
  );
}
