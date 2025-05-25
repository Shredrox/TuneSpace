"use client";

import { Button } from "@/components/shadcn/button";
import { usePathname } from "next/navigation";
import Link from "next/link";
import {
  NavigationMenu,
  NavigationMenuList,
} from "@/components/shadcn/navigation-menu";
import {
  AudioLines,
  CalendarDays,
  DiscAlbum,
  House,
  MessageSquare,
  Newspaper,
  Users,
} from "lucide-react";
import { ROUTES, UserRole } from "@/utils/constants";
import useAuth from "@/hooks/useAuth";

const Navigation = () => {
  const location = usePathname();
  const { auth } = useAuth();

  const isBandAdmin =
    auth?.role?.toLowerCase() === UserRole.BandAdmin.toLowerCase();

  const isActive = (path: string) => {
    return location === path;
  };

  const baseLinks = [
    {
      id: 0,
      to: ROUTES.HOME,
      text: "Home",
      icon: <House className="w-6 h-6" />,
    },
    {
      id: 1,
      to: ROUTES.DISCOVER,
      text: "Discover",
      icon: <AudioLines className="w-6 h-6" />,
    },
  ];

  const bandDashboardLink = {
    id: 2,
    to: ROUTES.BAND_DASHBOARD,
    text: "Band Dashboard",
    icon: <DiscAlbum className="w-6 h-6" />,
  };

  const otherLinks = [
    {
      id: 3,
      to: ROUTES.NEWS,
      text: "News",
      icon: <Newspaper className="w-6 h-6" />,
    },
    {
      id: 4,
      to: ROUTES.EVENTS,
      text: "Events",
      icon: <CalendarDays className="w-6 h-6" />,
    },
    {
      id: 5,
      to: "/forums",
      text: "Forums",
      icon: <Users className="h-4 w-4 mr-1" />,
    },
    {
      id: 6,
      to: "/messages",
      text: "Messages",
      icon: <MessageSquare className="h-4 w-4 mr-1" />,
    },
  ];

  const linksData = isBandAdmin
    ? [...baseLinks, bandDashboardLink, ...otherLinks]
    : [...baseLinks, ...otherLinks];

  return (
    <div>
      <NavigationMenu>
        <NavigationMenuList className="flex gap-1 items-center">
          {linksData.map((link) => (
            <div key={link.id}>
              <Button
                key={link.id}
                asChild
                variant={isActive(link.to) ? "default" : "outline"}
                className="flex gap-1"
              >
                <Link key={link.id} href={link.to}>
                  {link.icon}
                  {link.text}
                </Link>
              </Button>
            </div>
          ))}
        </NavigationMenuList>
      </NavigationMenu>
    </div>
  );
};

export default Navigation;
