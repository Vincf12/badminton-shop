"use client";

import React, { useEffect } from "react";
import { motion, useMotionValue, useTransform, animate } from "framer-motion";
import Image from "next/image";
import Link from "next/link";

const VisitorCounter: React.FC<{ target: number }> = ({ target }) => {
  const count = useMotionValue(0);
  const rounded = useTransform(count, (latest) => Math.floor(latest));
  
  // Cách viết chuẩn để render MotionValue mượt mà trong Framer Motion
  const display = useTransform(rounded, (val) => `${val.toLocaleString()}+`);

  useEffect(() => {
    const controls = animate(count, target, {
      duration: 2.5, // Giảm nhẹ thời gian để hiệu ứng dứt khoát hơn
      ease: [0.16, 1, 0.3, 1], // Custom ease-out mượt mà hơn (Quintic)
    });
    return () => controls.stop();
  }, [count, target]);

  return (
    // Sử dụng motion.span và truyền display vào để framer-motion tự cập nhật DOM tối ưu
    <motion.span className="text-5xl font-extrabold text-yellow-300 drop-shadow-[0_4px_6px_rgba(0,0,0,0.15)] tracking-tight">
      {display}
    </motion.span>
  );
};

const AuthLeftColumn: React.FC = () => (
  <motion.div
    initial={{ opacity: 0, x: -60 }} // Giảm bớt khoảng cách x để tránh cảm giác bị giật mạnh khi load
    animate={{ opacity: 1, x: 0 }}
    transition={{ duration: 0.6, ease: "easeOut" }}
    className="hidden md:flex w-3/5 flex-col justify-between items-center bg-emerald-700 text-white p-12 relative overflow-hidden"
  >
    {/* Background Decorative - Thêm chút hiệu ứng góc vòng tròn ẩn cho chuyên nghiệp */}
    <div className="absolute top-[-20%] left-[-20%] w-96 h-96 bg-emerald-600/30 rounded-full blur-3xl pointer-events-none" />
    <div className="absolute bottom-[-20%] right-[-20%] w-96 h-96 bg-emerald-800/40 rounded-full blur-3xl pointer-events-none" />

    {/* Header Section */}
    <div className="text-center z-10 mt-6">
      <h1 className="text-4xl font-black mb-3 tracking-wide drop-shadow-md hover:scale-105 transition-transform duration-300">
        <Link href="/" className="hover:text-emerald-100 transition-colors">
          FlyShot
        </Link>
      </h1>
      <p className="text-sm font-medium text-emerald-100/80 max-w-sm mx-auto uppercase tracking-widest">
        Where Passion Meets Performance
      </p>
    </div>

    {/* Counter & Mascot Section */}
    <div className="flex flex-col items-center justify-center z-10 my-auto gap-8">
      <div className="text-center px-6 py-4 rounded-2xl backdrop-blur-sm border border-emerald-600/20 shadow-inner">
        <p className="text-xs uppercase tracking-widest text-emerald-200 mb-1">Đã có hơn</p>
        <VisitorCounter target={10000} />
        <p className="text-xs uppercase tracking-widest text-emerald-200 mt-1">thành viên tham gia</p>
      </div>

      {/* Hiệu ứng Floating (Bay nhẹ) cho chú mèo siêu đáng yêu */}
      <motion.div
        animate={{ y: [0, -12, 0] }}
        transition={{
          duration: 4,
          repeat: Infinity,
          ease: "easeInOut",
        }}
        className="drop-shadow-[0_15px_15px_rgba(0,0,0,0.3)] filter"
      >
        <Image
          src="/assets/images/banner-flyshot01.png"
          alt="Badminton Banner"
          width={280}
          height={280}
          priority // Load ảnh ngay lập tức tránh bị giật layout (LCP)
          className="object-contain transform hover:scale-105 transition-transform duration-500"
        />
      </motion.div>
    </div>

    {/* Footer Section */}
    <div className="text-center z-10 mb-4 max-w-xs">
      <p className="text-sm text-emerald-100/90 font-medium leading-relaxed">
        Gia nhập cộng đồng <span className="text-yellow-300 font-semibold">FlyShot</span> để nhận nhiều đặc quyền hơn!
      </p>
    </div>
  </motion.div>
);

export default AuthLeftColumn;
