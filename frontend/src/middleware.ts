import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

const adminRoutes = ["/admin", "/dashboard"];
const authRoutes = ["/login", "/register"];

function decodeBase64Url(value: string): string {
  const base64 = value.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "=");
  return atob(padded);
}

function readRoleFromToken(token?: string): string | null {
  if (!token) {
    return null;
  }

  try {
    const payload = JSON.parse(decodeBase64Url(token.split(".")[1] ?? "")) as Record<string, unknown>;
    const role =
      payload.role ??
      payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    return typeof role === "string" ? role.toLowerCase() : null;
  } catch {
    return null;
  }
}

function isAdminRole(role: string | null): boolean {
  return role === "admin" || role === "staff";
}

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const token = request.cookies.get("access_token")?.value;
  const isAuthenticated = Boolean(token);
  const role = readRoleFromToken(token);

  if (adminRoutes.some((route) => pathname.startsWith(route))) {
    if (!isAuthenticated) {
      return NextResponse.redirect(new URL("/login", request.url));
    }

    if (!isAdminRole(role)) {
      return NextResponse.redirect(new URL("/", request.url));
    }
  }

  if (authRoutes.some((route) => pathname.startsWith(route)) && isAuthenticated) {
    return NextResponse.redirect(new URL(isAdminRole(role) ? "/admin" : "/", request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico|assets).*)"],
};
