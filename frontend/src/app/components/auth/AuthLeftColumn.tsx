"use client";

import React, { useEffect } from "react";
import { motion, useMotionValue, useTransform, animate } from "framer-motion";
import Image from "next/image";

const VisitorCounter: React.FC<{ target: number }> = ({ target }) => {
  const count = useMotionValue(0);
  const rounded = useTransform(count, (latest) => Math.floor(latest));
  const display = useTransform(rounded, (val) => `${val.toLocaleString()}+`);

  useEffect(() => {
    const controls = animate(count, target, {
      duration: 3,
      ease: "easeOut",
    });
    return () => controls.stop();
  }, [count, target]);

  return (
    <motion.span className="text-4xl font-bold text-yellow-300 drop-shadow">
      {display}
    </motion.span>
  );
};

const AuthLeftColumn: React.FC = () => (
  <motion.div
    initial={{ opacity: 0, x: -120 }}
    animate={{ opacity: 1, x: 0 }}
    transition={{ duration: 0.8, ease: "easeOut" }}
    className="hidden md:flex w-3/5 flex-col justify-center items-center bg-emerald-700 text-white p-10 relative"
  >
    <div className="text-center">
      <h1 className="text-3xl font-bold mb-2">FlyShot</h1>
      <p className="text-base text-emerald-100">
        FlyShot – Where Passion Meets Performance
      </p>
    </div>
    <div className="mt-10 text-center">
      <p className="text-sm text-emerald-100 mb-1">Đã có hơn</p>
      <VisitorCounter target={10000} />
      <p className="text-sm text-emerald-100 mt-1">thành viên tham gia</p>
    </div>
    <Image
      src="/assets/images/banner-netro.png"
      alt="Badminton Banner"
      width={300}
      height={200}
      className="mt-10 max-w-xs drop-shadow-lg rounded-lg"
    />
    <p className="mt-6 text-emerald-100 text-center text-sm">
      Gia nhập cộng đồng FlyShot để nhận nhiều đặc quyền hơn!
    </p>
  </motion.div>
);

export default AuthLeftColumn;