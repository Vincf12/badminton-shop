"use client";

import { useState } from "react";
import { Mail } from "lucide-react";

export default function Newsletter() {
  const [email, setEmail] = useState("");

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    setEmail("");
  };

  return (
    <section className="overflow-hidden rounded-2xl border border-emerald-100 bg-emerald-50 px-5 py-8 sm:px-8 lg:px-10">
      <div className="grid gap-6 lg:grid-cols-[1fr_420px] lg:items-center">
        <div>
          <p className="text-sm font-bold uppercase tracking-[0.18em] text-emerald-700">FlyShot Club</p>
          <h2 className="mt-3 max-w-2xl text-3xl font-black tracking-[-0.04em] text-slate-950 sm:text-4xl">
            Nhận ưu đãi và gợi ý trang bị theo lối chơi.
          </h2>
        </div>
        <form onSubmit={handleSubmit} className="flex flex-col gap-3 sm:flex-row">
          <label className="flex min-h-12 flex-1 items-center gap-3 rounded-full border border-white bg-white px-4 text-slate-500 shadow-[0_1px_2px_rgba(15,23,42,0.05)] focus-within:border-emerald-400 focus-within:ring-4 focus-within:ring-emerald-100">
            <Mail className="h-4 w-4" strokeWidth={1.8} />
            <input
              type="email"
              placeholder="Email của bạn"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
              className="w-full bg-transparent text-sm text-slate-950 outline-none placeholder:text-slate-400"
            />
          </label>
          <button
            type="submit"
            className="h-12 shrink-0 rounded-full bg-slate-950 px-7 text-sm font-black text-white transition hover:bg-slate-800 active:translate-y-px"
          >
            Đăng ký
          </button>
        </form>
      </div>
    </section>
  );
}
