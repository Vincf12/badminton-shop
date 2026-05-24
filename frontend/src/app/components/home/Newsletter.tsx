"use client";

import { useState } from "react";

export default function Newsletter() {
  const [email, setEmail] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Xử lý đăng ký newsletter
    console.log("Email:", email);
  };

  return (
    <section className="bg-gradient-to-r from-emerald-600 to-emerald-700 rounded-3xl p-12 text-center text-white mb-5 shadow-xl">
      <h2 className="text-4xl font-bold mb-4">Sẵn sàng nâng tầm trình độ?</h2>
      <p className="text-xl mb-8 opacity-90">
        Nhận ưu đãi 15% cho đơn hàng đầu tiên khi đăng ký ngay hôm nay
      </p>
      <form
        onSubmit={handleSubmit}
        className="flex flex-col sm:flex-row gap-4 justify-center items-center max-w-md mx-auto"
      >
        <input
          type="email"
          placeholder="Email của bạn"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          className="flex-1 px-4 py-4 rounded-xl w-full sm:w-auto border-2 text-white bg-transparent border border-white placeholder-white/60 focus:outline-none"
        />
        <button
          type="submit"
          className="bg-white text-emerald-600 px-8 py-4 rounded-xl font-bold hover:bg-gray-100 transition-colors duration-300 whitespace-nowrap"
        >
          Đăng ký
        </button>
      </form>
    </section>
  );
}