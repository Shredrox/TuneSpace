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
  Newspaper,
} from "lucide-react";
import { ROUTES } from "@/utils/constants";

const Navigation = () => {
  const location = usePathname();

  const isActive = (path: string) => {
    return location === path;
  };

  const linksData = [
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
    {
      id: 2,
      to: ROUTES.BAND_DASHBOARD,
      text: "Dashboard",
      icon: <DiscAlbum className="w-6 h-6" />,
    },
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
  ];

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
