"use client";

import React, { useEffect, useState } from "react";
import Link from "next/link";
import Image from "next/image";
import { usePathname } from "next/navigation";
import {
  ChevronDown,
  Clock,
  LogOut,
  Mail,
  MapPin,
  Menu,
  Phone,
  Search,
  ShoppingBag,
  User,
  X,
} from "lucide-react";
import { useAuth } from "@/features/auth";
import { cartService } from "@/features/cart";

interface MainLayoutProps {
  children: React.ReactNode;
}

const navItems = [
  { to: "/", label: "Trang chủ" },
  { to: "/shop", label: "Sản phẩm" },
  { to: "/about", label: "Về chúng tôi" },
  { to: "/contact", label: "Liên hệ" },
];

const footerLinks = [
  {
    title: "Mua sắm",
    links: [
      { href: "/shop", label: "Vợt cầu lông" },
      { href: "/shop", label: "Giày cầu lông" },
      { href: "/shop", label: "Phụ kiện" },
      { href: "/shop", label: "Sản phẩm mới" },
    ],
  },
  {
    title: "Hỗ trợ",
    links: [
      { href: "/contact", label: "Tư vấn chọn vợt" },
      { href: "/contact", label: "Chính sách đổi trả" },
      { href: "/contact", label: "Bảo hành" },
      { href: "/contact", label: "Giao hàng" },
    ],
  },
];

export default function MainLayout({ children }: MainLayoutProps) {
  const [menuOpen, setMenuOpen] = useState(false);
  const [showUserMenu, setShowUserMenu] = useState(false);
  const [cartCount, setCartCount] = useState(0);
  const [scrolled, setScrolled] = useState(false);

  const pathname = usePathname();
  const isHome = pathname === "/";
    const currentYear = new Date().getFullYear();

  const { user, logout, isAuthenticated } = useAuth();
  const userRole = user?.role?.toLowerCase();
  const canOpenAdmin = userRole === "admin" || userRole === "staff";

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 20);
    };

    handleScroll();

    window.addEventListener("scroll", handleScroll);

    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, []);

  useEffect(() => {
    const loadCartCount = async () => {
      try {
        const count = await cartService.getCartCount();
        setCartCount(count);
      } catch {
        setCartCount(0);
      }
    };

    loadCartCount();

    window.addEventListener("cart-updated", loadCartCount);

    return () => {
      window.removeEventListener("cart-updated", loadCartCount);
    };
  }, [isAuthenticated]);

  useEffect(() => {
    const closeUserMenu = (event: MouseEvent) => {
      const target = event.target as HTMLElement;

      if (!target.closest(".user-dropdown")) {
        setShowUserMenu(false);
      }
    };

    document.addEventListener("mousedown", closeUserMenu);

    return () => {
      document.removeEventListener("mousedown", closeUserMenu);
    };
  }, []);

  return (
    <div className="flex min-h-screen flex-col bg-gray-50">
      <header
        className={`fixed left-0 right-0 top-0 z-50 transition-all duration-500 ${
          isHome && !scrolled
            ? "border-b border-white/15 bg-white/12 backdrop-blur-md shadow-none"
            : " border-b border-white/10 bg-[#061017]/90 backdrop-blur-xl shadow-[0_18px_50px_rgba(0,0,0,0.24)]"
        }`}
      >
        <div className="mx-auto max-w-7xl px-6 lg:px-8">
          <div className="flex h-[80px] lg:h-[84px] items-center">
            {/* Logo + Navigation */}
            <div className="flex items-center gap-16">
              <Link
                href="/"
                className="group flex shrink-0 items-center gap-3"
                aria-label="FlyShot home" >
                <div className="leading-none">
                  <span className="block text-2xl font-black tracking-[-0.05em] text-white">
                    FlyShot
                  </span>

                  <span className="mt-1 block text-[10px] font-semibold uppercase tracking-[0.28em] text-emerald-300">
                    Badminton
                  </span>
                </div>
              </Link>

              <nav
                className="hidden items-center gap-1 lg:flex"
                aria-label="Điều hướng chính"
              >
                {navItems.map((item) => {
                  const active = pathname === item.to;

                  return (
                    <Link
                      key={item.to}
                      href={item.to}
                      className={`rounded-full px-5 py-2.5 text-[15px] font-semibold transition-all duration-300 ${
                        active
                          ? "bg-emerald-300 text-[#07111a]"
                          : "text-white/80 hover:-translate-y-0.5 hover:bg-white/15 hover:backdrop-blur hover:text-white"
                      }`}
                    >
                      {item.label}
                    </Link>
                  );
                })}
              </nav>
            </div>

            {/* Right Actions */}
            <div className="ml-auto flex items-center gap-3">
              <Link
                href="/shop"
                className="hidden h-12 min-w-[180px] items-center gap-3 rounded-full border border-white/10 bg-white/10 px-5 text-sm font-medium text-white/80 backdrop-blur transition-all duration-300 hover:border-emerald-300/40 hover:bg-white/15 hover:text-white md:flex"
              >
                <Search className="h-4 w-4" strokeWidth={1.8} />
                <span>Tìm kiếm</span>
              </Link>

              <Link
                href="/cart"
                className="relative flex h-12 w-12 items-center justify-center rounded-full border border-white/10 bg-white/10 text-white transition-all duration-300 hover:border-emerald-300/40 hover:bg-white/15 hover:text-emerald-300"
                aria-label="Xem giỏ hàng"
              >
                <ShoppingBag className="h-5 w-5" strokeWidth={1.8} />

                <span className="absolute -right-0.5 -top-0.5 flex h-5 min-w-5 items-center justify-center rounded-full bg-emerald-500 px-1 text-[11px] font-black text-[#07111a] ring-2 ring-[#061017]">
                  {cartCount}
                </span>
              </Link>

              {isAuthenticated ? (
                <div className="user-dropdown relative hidden md:block">
                  <button
                    type="button"
                    onClick={() => setShowUserMenu((value) => !value)}
                    className="flex h-12 items-center gap-2 rounded-full bg-emerald-300 px-5 text-sm font-bold text-[#07111a] shadow-md transition-all duration-300 hover:-translate-y-0.5 hover:bg-emerald-200 hover:shadow-lg"
                  >
                    <User className="h-4 w-4" strokeWidth={1.8} />

                    <span className="max-w-32 truncate">
                      {user?.fullName || "Tài khoản"}
                    </span>

                    <ChevronDown
                      className="h-4 w-4 text-[#07111a]/70"
                      strokeWidth={1.8}
                    />
                  </button>

                  {showUserMenu && (
                    <div className="absolute right-0 top-full z-50 mt-3 w-56 overflow-hidden rounded-2xl border border-slate-200 bg-white py-2 shadow-2xl">
                      <div className="border-b border-slate-100 px-4 py-3">
                        <p className="truncate text-sm font-semibold text-slate-950">
                          {user?.fullName || "Tài khoản"}
                        </p>
                      </div>

                      {canOpenAdmin && (
                        <Link
                          href="/admin"
                          onClick={() => setShowUserMenu(false)}
                          className="block px-4 py-2.5 text-sm font-medium text-slate-600 hover:bg-slate-50 hover:text-slate-950"
                        >
                          Trang quản trị
                        </Link>
                      )}

                      <Link
                        href="/profile"
                        onClick={() => setShowUserMenu(false)}
                        className="block px-4 py-2.5 text-sm font-medium text-slate-600 hover:bg-slate-50 hover:text-slate-950"
                      >
                        Hồ sơ
                      </Link>

                      <button
                        type="button"
                        onClick={() => {
                          logout();
                          setShowUserMenu(false);
                        }}
                        className="flex w-full items-center gap-2 px-4 py-2.5 text-left text-sm font-medium text-slate-600 hover:bg-slate-50 hover:text-slate-950"
                      >
                        <LogOut className="h-4 w-4" strokeWidth={1.8} />
                        Đăng xuất
                      </button>
                    </div>
                  )}
                </div>
              ) : (
                <Link
                  href="/login"
                  className="hidden h-12 items-center gap-2 rounded-full bg-emerald-300 px-6 text-sm font-bold text-[#07111a] shadow-md transition-all duration-300 hover:-translate-y-0.5 hover:bg-emerald-200 hover:shadow-lg md:flex"
                >
                  <User className="h-4 w-4" strokeWidth={1.8} />
                  <span>Đăng nhập</span>
                </Link>
              )}

              <button
                type="button"
                className="flex h-12 w-12 items-center justify-center rounded-full border border-white/10 bg-white/10 text-white transition-all duration-300 hover:border-emerald-300/40 lg:hidden"
                onClick={() => setMenuOpen((value) => !value)}
                aria-label="Mở menu"
              >
                {menuOpen ? (
                  <X className="h-5 w-5" strokeWidth={1.8} />
                ) : (
                  <Menu className="h-5 w-5" strokeWidth={1.8} />
                )}
              </button>
            </div>
          </div>
        </div>

        <div
          className={`overflow-hidden border-t border-white/10 transition-all duration-300 lg:hidden ${
            menuOpen ? "max-h-screen" : "max-h-0"
          }`}
        >
          <div className="mx-auto max-w-7xl space-y-2 px-4 py-4 sm:px-6">
            {navItems.map((item) => (
              <Link
                key={item.to}
                href={item.to}
                onClick={() => setMenuOpen(false)}
                className={`block rounded-2xl px-4 py-3 text-sm font-semibold ${
                  pathname === item.to
                    ? "bg-emerald-300 text-[#07111a]"
                    : "text-white/75 hover:bg-white/10 hover:text-white"
                }`}
              >
                {item.label}
              </Link>
            ))}

            <Link
              href={isAuthenticated ? "/profile" : "/login"}
              onClick={() => setMenuOpen(false)}
              className="block rounded-full bg-emerald-500 px-4 py-3 text-center text-sm font-bold text-[#07111a]"
            >
              {isAuthenticated ? "Hồ sơ của tôi" : "Đăng nhập"}
            </Link>
          </div>
        </div>
      </header>

      <main className={`flex-1 ${isHome ? "" : "pt-[72px] lg:pt-20"}`}>{children}</main>

      <footer className="border-t border-slate-200 bg-slate-950 text-slate-300">
        <div className="mx-auto grid max-w-7xl gap-10 px-4 py-14 sm:px-6 lg:grid-cols-[1.2fr_1fr_1fr_1.1fr]">
          <div>
            <div className="flex items-center gap-3">
              <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-emerald-500 text-sm font-black text-slate-950">
                FS
              </div>

              <div>
                <p className="text-2xl font-black tracking-[-0.05em] text-white">
                  FlyShot
                </p>
                <p className="text-xs font-bold uppercase tracking-[0.24em] text-emerald-300">
                  Badminton
                </p>
              </div>
            </div>

            <p className="mt-5 max-w-sm text-sm leading-6 text-slate-400">
              Cửa hàng cầu lông dành cho người chơi muốn chọn đúng vợt, đúng
              giày và đúng cảm giác thi đấu.
            </p>
          </div>

          {footerLinks.map((group) => (
            <div key={group.title}>
              <h4 className="text-sm font-bold uppercase tracking-[0.18em] text-white">
                {group.title}
              </h4>

              <div className="mt-4 space-y-3 text-sm">
                {group.links.map((item) => (
                  <Link
                    key={item.label}
                    href={item.href}
                    className="block text-slate-400 transition hover:text-emerald-300"
                  >
                    {item.label}
                  </Link>
                ))}
              </div>
            </div>
          ))}

          <div>
            <h4 className="text-sm font-bold uppercase tracking-[0.18em] text-white">
              Liên hệ
            </h4>

            <div className="mt-4 space-y-3 text-sm text-slate-400">
              <p className="flex gap-3">
                <MapPin
                  className="mt-0.5 h-4 w-4 shrink-0 text-emerald-300"
                  strokeWidth={1.8}
                />
                123 Đường Số 1, Quận 1, TP. HCM
              </p>

              <p className="flex items-center gap-3">
                <Phone
                  className="h-4 w-4 text-emerald-300"
                  strokeWidth={1.8}
                />
                0900 000 123
              </p>

              <p className="flex items-center gap-3">
                <Mail
                  className="h-4 w-4 text-emerald-300"
                  strokeWidth={1.8}
                />
                support@flyshot.vn
              </p>

              <p className="flex items-center gap-3">
                <Clock
                  className="h-4 w-4 text-emerald-300"
                  strokeWidth={1.8}
                />
                8:00 đến 22:00
              </p>
            </div>
          </div>
        </div>

        <div className="border-t border-white/10">
          <div className="mx-auto flex max-w-7xl flex-col gap-2 px-4 py-5 text-xs text-slate-500 sm:flex-row sm:items-center sm:justify-between sm:px-6">
            <span>© {currentYear} FlyShot. All rights reserved.</span>
            <span>
              Thiết kế cho trải nghiệm mua sắm cầu lông rõ ràng và nhanh gọn.
            </span>
          </div>
        </div>
      </footer>
    </div>
  );
}
