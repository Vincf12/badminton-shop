import Image from "next/image";
import Link from "next/link";

export default function HeroBanner() {
  return (
    <div className="relative w-screen h-[600px]">
      <Image
        src="/assets/images/banner01.png"
        alt="Badminton Banner"
        fill
        sizes="100vw"
        className="object-cover"
        priority
      />

      {/* Lớp mờ */}
      <div className="absolute inset-0 bg-gradient-to-b from-black/40 via-black/20 to-transparent" />

      {/* Nội dung banner */}
      <div className="absolute inset-0 flex items-center justify-start">
        <div className="container max-w-7xl mx-auto px-6 text-left">
          <h1 className="text-white text-5xl md:text-6xl font-extrabold mb-6 leading-tight">
            Đập mạnh <span className="text-emerald-300">Chiến thắng nhanh</span>
          </h1>
          <p className="text-gray-200 text-lg md:text-xl mb-8 leading-relaxed">
            Trải nghiệm hiệu năng đỉnh cao cùng vợt cầu lông cao cấp — lựa chọn
            của những tay vợt hàng đầu.
          </p>
          <div className="flex flex-wrap justify-start gap-4">
            <Link
              href="/shop"
              className="px-8 py-4 bg-emerald-600 hover:bg-emerald-700 text-white font-semibold rounded-xl shadow-2xl hover:shadow-emerald-500/50 transition-all duration-300 transform hover:-translate-y-1"
            >
              Khám phá ngay
            </Link>
            <Link
              href="/about"
              className="px-8 py-4 bg-white/10 backdrop-blur-sm hover:bg-white/20 text-white font-semibold rounded-xl border-2 border-white/30 transition-all duration-300"
            >
              Tìm hiểu thêm
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}