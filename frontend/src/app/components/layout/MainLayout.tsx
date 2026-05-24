"use client";

import React, { useState, useEffect } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { ShoppingCart, User, Search, Menu, X, LogOut } from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";

interface MainLayoutProps {
  children: React.ReactNode;
}

const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
  const [menuOpen, setMenuOpen] = useState(false);
  const [scrolled, setScrolled] = useState(false);
  const [showUserMenu, setShowUserMenu] = useState(false);
  const pathname = usePathname();
  const { user, logout, isAuthenticated } = useAuth();

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 20);
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  // Đóng dropdown khi click outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (showUserMenu) {
        const target = event.target as HTMLElement;
        if (!target.closest('.user-dropdown')) {
          setShowUserMenu(false);
        }
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [showUserMenu]);

  const navItems = [
    { to: "/", label: "Trang chủ" },
    { to: "/shop", label: "Sản phẩm" },
    { to: "/about", label: "Về chúng tôi" },
    { to: "/contact", label: "Liên hệ" },
  ];

  // ✅ Kiểm tra nếu đang ở trang chủ
  const isHome = pathname === "/";

  return (
    <div className="flex flex-col min-h-screen bg-gray-50">
      {/* Header */}
      <header
        className={`fixed top-0 left-0 right-0 z-50 transition-all duration-500 ${
          isHome
            ? scrolled
              ? "bg-[#0D1B2A]/95 shadow-md backdrop-blur-md"
              : "bg-transparent"
            : "bg-[#0D1B2A]/95 shadow-md backdrop-blur-md"
        }`}
      >
        <div className="max-w-7xl mx-auto px-6">
          <div className="flex justify-between items-center h-20">
            {/* Logo */}
            <Link href="/" className="flex items-center gap-3 group">
              <div className="hidden sm:block">
                <span className="text-4xl font-bold bg-linear-to-r from-emerald-500 to-emerald-700 bg-clip-text text-transparent">
                  FlyShot
                </span>
                <p className="text-xs text-white -mt-1">Premium Badminton</p>
              </div>
            </Link>

            {/* Desktop Navigation */}
            <nav className="hidden lg:flex items-center gap-8">
              {navItems.map((item) => (
                <Link
                  key={item.to}
                  href={item.to}
                  className={`relative font-medium transition-all duration-200 ${
                    pathname === item.to
                      ? "text-emerald-400"
                      : "text-white hover:text-emerald-400"
                  } after:absolute after:-bottom-1 after:left-0 after:h-0.5 after:bg-emerald-400 after:transition-all after:duration-200 ${
                    pathname === item.to
                      ? "after:w-full"
                      : "after:w-0 hover:after:w-full"
                  }`}
                >
                  {item.label}
                </Link>
              ))}
            </nav>

            {/* Right Actions */}
            <div className="flex items-center gap-4">
              <button className="hidden md:flex items-center gap-2 px-6 py-3 min-w-60 rounded-lg bg-gray-100 hover:bg-gray-200 transition-colors duration-200">
                <Search className="w-4 h-4 text-gray-600" />
                <span className="text-sm text-gray-600">Tìm kiếm...</span>
              </button>

              {/* Cart */}
              <Link
                href="/cart"
                className="relative p-2 hover:bg-gray-100 rounded-lg transition-colors duration-200"
              >
                <ShoppingCart className="w-6 h-6 text-slate-400" />
                <span className="absolute -top-1 -right-1 bg-emerald-600 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
                  0
                </span>
              </Link>

              {/* User */}
              {isAuthenticated ? (
                <div className="relative user-dropdown">
                  <button
                    onClick={() => setShowUserMenu(!showUserMenu)}
                    className="hidden md:flex items-center gap-2 px-4 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg transition-colors duration-200 font-medium"
                  >
                    <User className="w-4 h-4" />
                    <span>{user?.full_name}</span>
                  </button>
                  
                  {/* User Dropdown Menu */}
                  {showUserMenu && (
                    <div className="absolute right-0 top-full mt-2 w-48 bg-white rounded-lg shadow-lg border border-gray-200 py-2 z-50">
                      <div className="px-4 py-2 border-b border-gray-100">
                        <p className="text-sm font-medium text-gray-900">{user?.full_name}</p>
                        <p className="text-xs text-gray-500">{user?.email}</p>
                      </div>
                      <Link
                        href="/dashboard"
                        className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                        onClick={() => setShowUserMenu(false)}
                      >
                        Dashboard
                      </Link>
                      <Link
                        href="/profile"
                        className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                        onClick={() => setShowUserMenu(false)}
                      >
                        Thông tin cá nhân
                      </Link>
                      <button
                        onClick={() => {
                          logout();
                          setShowUserMenu(false);
                        }}
                        className="w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 flex items-center gap-2"
                      >
                        <LogOut className="w-4 h-4" />
                        Đăng xuất
                      </button>
                    </div>
                  )}
                </div>
              ) : (
                <Link
                  href="/login"
                  className="hidden md:flex items-center gap-2 px-4 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg transition-colors duration-200 font-medium"
                >
                  <User className="w-4 h-4" />
                  <span>Đăng nhập</span>
                </Link>
              )}

              {/* Mobile Menu Button */}
              <button
                className="lg:hidden p-2 hover:bg-gray-100 rounded-lg transition-colors duration-200"
                onClick={() => setMenuOpen(!menuOpen)}
                aria-label="Toggle menu"
              >
                {menuOpen ? (
                  <X className="w-6 h-6 text-gray-700" />
                ) : (
                  <Menu className="w-6 h-6 text-gray-700" />
                )}
              </button>
            </div>
          </div>
        </div>

        {/* Mobile Menu */}
        <div
          className={`lg:hidden overflow-hidden transition-all duration-300 ${
            menuOpen ? "max-h-screen" : "max-h-0"
          }`}
        >
          <div className="bg-[#0D1B2A] border-t border-gray-800 shadow-lg">
            <div className="max-w-7xl mx-auto px-6 py-4 space-y-2">
              {navItems.map((item) => (
                <Link
                  key={item.to}
                  href={item.to}
                  onClick={() => setMenuOpen(false)}
                  className={`block px-4 py-3 rounded-lg transition-colors duration-200 ${
                    pathname === item.to
                      ? "bg-emerald-700 text-white font-semibold"
                      : "text-gray-200 hover:bg-gray-800"
                  }`}
                >
                  {item.label}
                </Link>
              ))}
              {isAuthenticated ? (
                <>
                  <div className="px-4 py-3 border-b border-gray-700">
                    <p className="text-sm font-medium text-white">{user?.full_name}</p>
                    <p className="text-xs text-gray-300">{user?.email}</p>
                  </div>
                  <Link
                    href="/dashboard"
                    onClick={() => setMenuOpen(false)}
                    className="block px-4 py-3 text-gray-200 hover:bg-gray-800 rounded-lg transition-colors duration-200"
                  >
                    Dashboard
                  </Link>
                  <Link
                    href="/profile"
                    onClick={() => setMenuOpen(false)}
                    className="block px-4 py-3 text-gray-200 hover:bg-gray-800 rounded-lg transition-colors duration-200"
                  >
                    Thông tin cá nhân
                  </Link>
                  <button
                    onClick={() => {
                      logout();
                      setMenuOpen(false);
                    }}
                    className="w-full text-left px-4 py-3 text-gray-200 hover:bg-gray-800 rounded-lg transition-colors duration-200 flex items-center gap-2"
                  >
                    <LogOut className="w-4 h-4" />
                    Đăng xuất
                  </button>
                </>
              ) : (
                <Link
                  href="/login"
                  onClick={() => setMenuOpen(false)}
                  className="block px-4 py-3 bg-emerald-600 text-white rounded-lg text-center font-medium hover:bg-emerald-700 transition-colors duration-200"
                >
                  Đăng nhập
                </Link>
              )}
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 max-w-7xl mx-auto w-full px-6 py-2 pt-24">
        {children}
      </main>

      {/* Footer */}
      <footer className="bg-[#0B1220] text-gray-400 font-sans border-t border-white/5">
      {/* Phần nội dung chính */}
      <div className="max-w-7xl mx-auto px-6 pt-16 pb-12">
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-8 md:gap-12">
          
          {/* Cột 1: Thương hiệu & Slogan */}
          <div className="space-y-4">
            <h3 className="text-2xl font-bold tracking-wider text-white">
              Fly<span className="text-emerald-400">Shot</span>
            </h3>
            <p className="text-sm leading-relaxed text-gray-400">
              Hệ thống cửa hàng cầu lông cao cấp. Nơi tiếp lửa đam mê và nâng tầm trình độ của các lông thủ bằng những sản phẩm chính hãng 100%.
            </p>
            {/* Mạng xã hội - Giúp tăng tương tác */}
            <div className="flex space-x-4 pt-2">
              <a href="#" className="p-2 bg-white/5 hover:bg-emerald-500 hover:text-white rounded-full transition-all text-gray-400">
                <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24"><path d="M22 12c0-5.523-4.477-10-10-10S2 6.477 2 12c0 4.991 3.657 9.128 8.438 9.878v-6.987h-2.54V12h2.54V9.797c0-2.506 1.492-3.89 3.777-3.89 1.094 0 2.238.195 2.238.195v2.46h-1.26c-1.243 0-1.63.771-1.63 1.562V12h2.773l-.443 2.89h-2.33v6.988C18.343 21.128 22 16.991 22 12z"/></svg>
              </a>
              <a href="#" className="p-2 bg-white/5 hover:bg-emerald-500 hover:text-white rounded-full transition-all text-gray-400">
                <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24"><path d="M12.315 2c2.43 0 2.784.013 3.808.06 1.064.049 1.791.218 2.427.465a4.902 4.902 0 011.772 1.153 4.902 4.902 0 011.153 1.772c.247.636.416 1.363.465 2.427.048 1.067.06 1.407.06 4.123v.08c0 2.643-.012 2.987-.06 4.043-.049 1.064-.218 1.791-.465 2.427a4.902 4.902 0 01-1.153 1.772 4.902 4.902 0 01-1.772 1.153c-.636.247-1.363.416-2.427.465-1.067.048-1.407.06-4.123.06h-.08c-2.643 0-2.987-.012-4.043-.06-1.064-.049-1.791-.218-2.427-.465a4.902 4.902 0 01-1.772-1.153 4.902 4.902 0 01-1.153-1.772c-.247-.636-.416-1.363-.465-2.427-.047-1.024-.06-1.379-.06-3.808v-.63c0-2.43.013-2.784.06-3.808.049-1.064.218-1.791.465-2.427a4.902 4.902 0 011.153-1.772A4.902 4.902 0 015.45 2.525c.636-.247 1.363-.416 2.427-.465C8.901 2.013 9.256 2 11.685 2h.63zm-.082 1.802h-.468c-2.456 0-2.784.011-3.807.058-.975.045-1.504.207-1.857.344-.467.182-.8.398-1.15.748-.35.35-.566.683-.748 1.15-.137.353-.3.882-.344 1.857-.047 1.023-.058 1.351-.058 3.807v.468c0 2.456.011 2.784.058 3.807.045.975.207 1.504.344 1.857.182.466.399.8.748 1.15.35.35.683.566 1.15.748.353.137.882.3 1.857.344 1.054.048 1.37.058 4.041.058h.08c2.597 0 2.917-.01 3.96-.058.976-.045 1.505-.207 1.858-.344.466-.182.8-.398 1.15-.748.35-.35.566-.683.748-1.15.137-.353.3-.882.344-1.857.048-1.055.058-1.37.058-4.041v-.08c0-2.597-.01-2.917-.058-3.96-.045-.976-.207-1.505-.344-1.858a3.097 3.097 0 00-.748-1.15 3.098 3.098 0 00-1.15-.748c-.353-.137-.882-.3-1.857-.344-1.023-.047-1.351-.058-3.807-.058zM12 6.865a5.135 5.135 0 110 10.27 5.135 5.135 0 010-10.27zm0 1.802a3.333 3.333 0 100 6.666 3.333 3.333 0 000-6.666zm5.338-3.205a1.2 1.2 0 110 2.4 1.2 1.2 0 010-2.4z"/></svg>
              </a>
            </div>
          </div>

          {/* Cột 2: Khám phá / Sản phẩm */}
          <div>
            <h4 className="text-white font-semibold text-base mb-4 tracking-wide uppercase text-xs opacity-90">Danh mục mua sắm</h4>
            <div className="space-y-2.5 text-sm">
              <Link href="/shop" className="block text-gray-400 hover:text-emerald-400 hover:translate-x-1 transition-all">Vợt cầu lông</Link>
              <Link href="/shoes" className="block text-gray-400 hover:text-emerald-400 hover:translate-x-1 transition-all">Giày cầu lông</Link>
              <Link href="/apparel" className="block text-gray-400 hover:text-emerald-400 hover:translate-x-1 transition-all">Quần áo & Phụ kiện</Link>
              <Link href="/sale" className="block text-rose-400 hover:text-rose-300 hover:translate-x-1 transition-all font-medium">Chương trình Sale %</Link>
            </div>
          </div>

          {/* Cột 3: Chính sách khách hàng (Quan trọng cho Web bán hàng) */}
          <div>
            <h4 className="text-white font-semibold text-base mb-4 tracking-wide uppercase text-xs opacity-90">Chính sách chung</h4>
            <div className="space-y-2.5 text-sm">
              <Link href="/shipping" className="block text-gray-400 hover:text-emerald-400 hover:translate-x-1 transition-all">Chính sách vận chuyển</Link>
              <Link href="/returns" className="block text-gray-400 hover:text-emerald-400 hover:translate-x-1 transition-all">Đổi trả trong 30 ngày</Link>
              <Link href="/warranty" className="block text-gray-400 hover:text-emerald-400 hover:translate-x-1 transition-all">Chính sách bảo hành</Link>
              <Link href="/privacy" className="block text-gray-400 hover:text-emerald-400 hover:translate-x-1 transition-all">Bảo mật thông tin</Link>
            </div>
          </div>

          {/* Cột 4: Liên hệ & Hỗ trợ */}
          <div>
            <h4 className="text-white font-semibold text-base mb-4 tracking-wide uppercase text-xs opacity-90">Thông tin hỗ trợ</h4>
            <div className="space-y-3 text-sm text-gray-400">
              <div className="flex items-start space-x-2">
                <span className="text-emerald-400 mt-0.5">📍</span>
                <p>123 Đường Số 1, Phường Bến Nghé, Quận 1, TP. HCM</p>
              </div>
              <div className="flex items-center space-x-2">
                <span className="text-emerald-400">📞</span>
                <p>Hotline: <span className="text-white font-medium">0900 000 123</span></p>
              </div>
              <div className="flex items-center space-x-2">
                <span className="text-emerald-400">✉️</span>
                <p>support@flyshot.vn</p>
              </div>
              <div className="flex items-center space-x-2">
                <span className="text-emerald-400">🕒</span>
                <p>Giờ làm việc: 8:00 - 22:00</p>
              </div>
            </div>
          </div>

        </div>
      </div>

      {/* Phần Bản quyền & Thanh toán */}
      <div className="border-t border-white/5 bg-[#080d18]">
        <div className="max-w-7xl mx-auto px-6 py-6 flex flex-col sm:flex-row items-center justify-between gap-4 text-xs text-gray-500">
          <div>
            © {new Date().getFullYear()} FlyShot. All rights reserved. Thiết kế bởi FlyShot Team.
          </div>
          {/* Giả lập các icon thanh toán giúp tăng uy tín cho web */}
          <div className="flex space-x-3 text-lg opacity-40 grayscale hover:grayscale-0 transition-all">
            <span>💳</span> {/* Visa */}
            <span>💵</span> {/* COD */}
            <span>🏦</span> {/* Chuyển khoản */}
          </div>
        </div>
      </div>
    </footer>
    </div>
  );
};

export default MainLayout;