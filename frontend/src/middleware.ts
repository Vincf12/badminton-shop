import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

// ✅ Chỉ bảo vệ dashboard, bỏ cart ra
const protectedRoutes = ["/dashboard"];
const authRoutes = ["/login", "/register"];

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Check token from cookies
  const token = request.cookies.get("access_token")?.value;
  const isAuthenticated = !!token;

  // Chỉ redirect khi vào dashboard mà chưa login
  if (protectedRoutes.some((route) => pathname.startsWith(route))) {
    if (!isAuthenticated) {
      return NextResponse.redirect(new URL("/login", request.url));
    }
  }

  // Nếu đã login thì không cho vào trang login/register nữa
  if (authRoutes.some((route) => pathname.startsWith(route))) {
    if (isAuthenticated) {
      return NextResponse.redirect(new URL("/", request.url));
    }
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico|assets).*)"],
};
