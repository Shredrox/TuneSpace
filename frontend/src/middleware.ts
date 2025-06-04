import { NextRequest, NextResponse } from "next/server";
import { isAuthenticated } from "./lib/auth";
import { PROTECTED_ROUTES, ROUTES } from "./utils/constants";

export async function middleware(request: NextRequest) {
  const path = request.nextUrl.pathname;

  const isProtectedRoute = PROTECTED_ROUTES.some((route) =>
    path.startsWith(route)
  );

  const authenticated = await isAuthenticated(request);

  if (!authenticated && isProtectedRoute) {
    return NextResponse.redirect(new URL(ROUTES.LOGIN, request.nextUrl));
  }

  if (authenticated && path === ROUTES.ROOT) {
    return NextResponse.redirect(new URL(ROUTES.HOME, request.nextUrl));
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    /*
     * Match all request paths except for the ones starting with:
     * - api (API routes)
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico, sitemap.xml, robots.txt (metadata files)
     */
    "/((?!api|_next/static|_next/image|favicon.ico|sitemap.xml|robots.txt).*)",
  ],
};
