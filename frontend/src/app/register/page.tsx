"use client";

import React, { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import AuthLeftColumn from "@/app/components/auth/AuthLeftColumn";
import { authService, RegisterData } from "@/services/authService";

export default function RegisterPage() {
  const [showPass, setShowPass] = useState(false);
  const [formData, setFormData] = useState<RegisterData>({
    fullName: "",
    username: "",
    email: "",
    phone: "",
    password: "",
  });
  const [confirmPassword, setConfirmPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const router = useRouter();

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    if (name === "confirmPassword") {
      setConfirmPassword(value);
    } else {
      setFormData(prev => ({
        ...prev,
        [name]: value
      }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    // Validation
    if (formData.password !== confirmPassword) {
      setError("Mật khẩu xác nhận không khớp");
      setLoading(false);
      return;
    }

    if (formData.password.length < 6) {
      setError("Mật khẩu phải có ít nhất 6 ký tự");
      setLoading(false);
      return;
    }

    try {
      const user = await authService.register(formData);
      console.log("Đăng ký thành công:", user);
      // Redirect to login page
      router.push("/login?message=Đăng ký thành công! Vui lòng đăng nhập.");
    } catch (err: any) {
      setError(err.message || "Có lỗi xảy ra khi đăng ký");
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
              Đăng ký
            </h2>
            <form className="space-y-4" onSubmit={handleSubmit}>
              {error && (
                <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
                  {error}
                </div>
              )}
              
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Nhập Username
                </label>
                <div className="relative mt-1 mb-5">
                  <input
                    type="text"
                    name="username"
                    value={formData.username}
                    onChange={handleInputChange}
                    placeholder="Nhập username"
                    className="w-full border rounded-lg px-5 py-2 focus:ring-emerald-500 focus:border-emerald-500"
                    required
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Nhập Họ và Tên
                </label>
                <div className="relative mt-1 mb-5">
                  <input
                    type="text"
                    name="fullName"
                    value={formData.fullName}
                    onChange={handleInputChange}
                    placeholder="Nhập họ và tên"
                    className="w-full border rounded-lg px-5 py-2 focus:ring-emerald-500 focus:border-emerald-500"
                    required
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Email
                </label>
                <div className="relative mt-1 mb-5">
                  <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleInputChange}
                    placeholder="Nhập email"
                    className="w-full border rounded-lg px-5 py-2 focus:ring-emerald-500 focus:border-emerald-500"
                    required
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Số điện thoại
                </label>
                <div className="relative mt-1 mb-5">
                  <input
                    type="text"
                    name="phone"
                    value={formData.phone}
                    onChange={handleInputChange}
                    placeholder="Nhập số điện thoại"
                    className="w-full border rounded-lg px-5 py-2 focus:ring-emerald-500 focus:border-emerald-500"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Mật khẩu
                </label>
                <div className="relative mt-1 mb-5">
                  <input
                    type={showPass ? "text" : "password"}
                    name="password"
                    value={formData.password}
                    onChange={handleInputChange}
                    placeholder="••••••••"
                    className="w-full border rounded-lg px-5 py-2 focus:ring-emerald-500 focus:border-emerald-500"
                    required
                  />

                  <button
                    type="button"
                    onClick={() => setShowPass(!showPass)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-sm text-emerald-600 hover:underline"
                  >
                    {showPass ? "Ẩn" : "Hiện"}
                  </button>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Nhập lại mật khẩu
                </label>
                <div className="relative mt-1 mb-2">
                  <input
                    type={showPass ? "text" : "password"}
                    name="confirmPassword"
                    value={confirmPassword}
                    onChange={handleInputChange}
                    placeholder="••••••••"
                    className="w-full border rounded-lg px-5 py-2 focus:ring-emerald-500 focus:border-emerald-500"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPass(!showPass)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-sm text-emerald-600 hover:underline"
                  >
                    {showPass ? "Ẩn" : "Hiện"}
                  </button>
                </div>
                <label className="block text-sm font-medium text-gray-600 mb-7 mt-4">
                  Mật khẩu tối thiểu 6 ký tự, có ít nhất 1 chữ số và 1 số
                </label>
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
                className="w-full bg-emerald-600 hover:bg-emerald-700 disabled:bg-gray-400 text-white py-2 rounded-lg shadow-md transition"
              >
                {loading ? "Đang đăng ký..." : "Đăng ký"}
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
              Bạn đã có tài khoản{" "}
              <Link href="/login" className="text-emerald-600 hover:underline">
                Đăng nhập
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
