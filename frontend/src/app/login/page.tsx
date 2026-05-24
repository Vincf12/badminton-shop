"use client";

import React, { useState, useEffect } from "react";
import { Mail, Lock } from "lucide-react";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { useAuth } from "@/contexts/AuthContext";
import AuthLeftColumn from "@/app/components/auth/AuthLeftColumn";

export default function LoginPage() {
  const [showPass, setShowPass] = useState(false);
  const [contactInfo, setContactInfo] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const router = useRouter();
  const searchParams = useSearchParams();
  const { login, isAuthenticated } = useAuth();

  // Redirect nếu đã đăng nhập
  useEffect(() => {
    if (isAuthenticated) {
      router.push("/");
    }
  }, [isAuthenticated, router]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    
    try {
      await login(contactInfo, password);
      // Redirect sẽ được xử lý trong useEffect
      router.push("/");
    } catch (error: any) {
      setError(error.message || "Đăng nhập thất bại!");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="w-full min-h-screen bg-gray-50 flex items-center justify-center text-black">
      <div className="w-full h-screen flex bg-neutral-100 overflow-hidden">
        {/* Cột trái */}
        <AuthLeftColumn />
        {/* Cột phải */}
        <div className="flex w-full md:w-1/2 items-center justify-center p-8">
          <div className="w-full max-w-md">
            <h2 className="text-2xl font-bold text-center text-emerald-700 mb-6">
              Đăng nhập
            </h2>
            
            {/* Hiển thị thông báo từ URL params */}
            {searchParams.get('message') && (
              <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-4">
                {searchParams.get('message')}
              </div>
            )}
            
            {/* Hiển thị lỗi */}
            {error && (
              <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
                {error}
              </div>
            )}
            
            <form className="space-y-4" onSubmit={handleSubmit}>
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Email / Số điện thoại
                </label>
                <div className="relative mt-1 mb-5">
                  <input
                    type="text"
                    placeholder="you@example.com"
                    onChange={(e) => setContactInfo(e.target.value)}
                    className="w-full border rounded-lg px-10 py-2 focus:ring-emerald-500 focus:border-emerald-500"
                  />
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 h-5 w-5" />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Mật khẩu
                </label>
                <div className="relative mt-1 mb-5">
                  <input
                    type={showPass ? "text" : "password"}
                    placeholder="••••••••"
                    onChange={(e) => setPassword(e.target.value)}
                    className="w-full border rounded-lg px-10 py-2 focus:ring-emerald-500 focus:border-emerald-500"
                  />
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 h-5 w-5" />
                  <button
                    type="button"
                    onClick={() => setShowPass(!showPass)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-sm text-emerald-600 hover:underline"
                  >
                    {showPass ? "Ẩn" : "Hiện"}
                  </button>
                </div>
              </div>

              <div className="flex items-center justify-between text-sm">
                <label className="inline-flex items-center gap-2">
                  <input type="checkbox" className="h-4 w-4 text-emerald-600" />
                  Ghi nhớ tôi
                </label>
                <a href="#" className="text-emerald-600 hover:underline">
                  Quên mật khẩu?
                </a>
              </div>

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-emerald-600 hover:bg-emerald-700 text-white py-2 rounded-lg shadow-md transition"
              >
                {loading ? "Đang đăng nhập..." : "Đăng nhập"}
              </button>
            </form>

            <div className="my-6 flex items-center">
              <div className="grow border-t border-gray-300"></div>
              <span className="px-3 text-gray-500 text-sm">Hoặc</span>
              <div className="grow border-t border-gray-300"></div>
            </div>

            <div className="flex gap-4">
              <button className="flex-1 border py-2 rounded-lg hover:bg-gray-50">
                Google
              </button>
              <button className="flex-1 border py-2 rounded-lg hover:bg-gray-50">
                Facebook
              </button>
            </div>

            <p className="mt-6 text-sm text-center text-gray-600">
              Chưa có tài khoản?{" "}
              <Link
                href="/register"
                className="text-emerald-600 hover:underline"
              >
                Đăng ký ngay
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
