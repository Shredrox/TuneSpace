"use client";

import LandingLayout from "@/layouts/landing-layout";
import { usePathname } from "next/navigation";

export default function AuthLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();

  const isAuthPage =
    pathname === "/login" ||
    pathname === "/signup" ||
    pathname === "/band/register";

  if (isAuthPage) {
    return <main>{children}</main>;
  }

  return <LandingLayout>{children}</LandingLayout>;
}
