import Image from "next/image";
import Link from "next/link";
import { Gauge, ShieldCheck, SlidersHorizontal, Sparkles } from "lucide-react";
import { MainLayout } from "@/widgets/layout";

const stats = [
  { value: "2018", label: "Thành lập tại TP. Hồ Chí Minh" },
  { value: "5K+", label: "Người chơi đã được FlyShot phục vụ" },
  { value: "42", label: "Bước kiểm tra khung và dây" },
];

const principles = [
  {
    title: "Tốc độ vung vợt nhanh hơn",
    description: "Khung khí động học và điểm cân bằng nhẹ giúp người chơi xử lý các pha cầu cuối trận với ít lực cản hơn.",
  },
  {
    title: "Cấu hình đúng lối chơi",
    description: "Mỗi tư vấn đều kết hợp trọng lượng vợt, lực căng dây, cỡ cán và phong cách di chuyển của bạn.",
  },
  {
    title: "Chăm sóc sẵn sàng ra sân",
    description: "Căng dây, kiểm tra gen vợt và bảo hành luôn bám sát cách người chơi thật sự tập luyện.",
  },
];

const services = [
  {
    icon: SlidersHorizontal,
    title: "Căng dây theo cá nhân",
    description: "Chúng tôi tinh chỉnh lực căng theo tốc độ vung vợt, điểm tiếp xúc và cảm giác cầu bạn mong muốn.",
  },
  {
    icon: Gauge,
    title: "Thử vợt hiệu năng",
    description: "Một số mẫu vợt có thể được trải nghiệm trước khi mua để lựa chọn cuối cùng thật chắc chắn trên sân.",
  },
  {
    icon: ShieldCheck,
    title: "Mua sắm được bảo vệ",
    description: "Chính sách đổi trả, bảo hành và kiểm định rõ ràng giúp thiết bị cao cấp dễ sở hữu hơn.",
  },
];

export default function AboutPage() {
  return (
    <MainLayout>
      <div className="mx-auto w-full max-w-7xl text-[#061017]">
        <section className="grid min-h-[calc(100dvh-120px)] grid-cols-1 items-center gap-8 py-10 lg:grid-cols-[1.02fr_0.98fr] lg:py-14">
          <div className="max-w-2xl">
            <p className="mb-5 text-xs font-bold uppercase tracking-[0.28em] text-emerald-700">
              Nhà hiệu năng FlyShot
            </p>
            <h1 className="text-5xl font-black leading-[0.92] tracking-[-0.06em] text-[#061017] sm:text-6xl lg:text-7xl">
              Dành cho người chơi cảm được nhịp cầu sớm hơn.
            </h1>
            <p className="mt-6 max-w-xl text-base leading-7 text-slate-600 sm:text-lg">
              FlyShot tuyển chọn thiết bị cầu lông cao cấp cho người chơi đề cao tốc độ, độ chính xác và cảm giác của một cú chạm cầu gọn gàng.
            </p>
            <div className="mt-8 flex flex-col gap-3 sm:flex-row">
              <Link
                href="/shop"
                className="inline-flex h-12 items-center justify-center rounded-full bg-[#061017] px-6 text-sm font-bold text-white transition-all duration-200 hover:bg-[#12202c] active:translate-y-px"
              >
                Mua thiết bị hiệu năng
              </Link>
              <Link
                href="/contact"
                className="inline-flex h-12 items-center justify-center rounded-full border border-slate-300 bg-white px-6 text-sm font-bold text-[#061017] transition-all duration-200 hover:border-[#061017] active:translate-y-px"
              >
                Đặt lịch tư vấn
              </Link>
            </div>
          </div>

          <div className="relative min-h-[420px] overflow-hidden rounded-[28px] border border-white/70 bg-[#061017] shadow-[0_28px_80px_rgba(6,16,23,0.22)]">
            <Image
              src="/assets/images/banner-flyshot01.png"
              alt="Người chơi cầu lông FlyShot và thiết bị cao cấp"
              fill
              sizes="(min-width: 1024px) 46vw, 100vw"
              className="object-cover"
              priority
            />
            <div className="absolute inset-0 bg-linear-to-t from-[#061017]/82 via-[#061017]/18 to-transparent" />
            <div className="absolute bottom-0 left-0 right-0 p-6 sm:p-8">
              <div className="grid grid-cols-3 gap-3 rounded-3xl border border-white/14 bg-white/10 p-3 text-white backdrop-blur-xl">
                {stats.map((stat) => (
                  <div key={stat.value} className="rounded-2xl bg-white/10 p-3">
                    <div className="text-2xl font-black tracking-[-0.04em]">{stat.value}</div>
                    <div className="mt-1 text-[11px] font-medium leading-4 text-white/68">{stat.label}</div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </section>

        <section className="border-y border-slate-200 py-10 lg:py-14">
          <div className="grid grid-cols-1 gap-8 lg:grid-cols-[0.7fr_1.3fr]">
            <div>
              <h2 className="text-3xl font-black tracking-[-0.04em] text-[#061017] sm:text-4xl">
                Chúng tôi thiết kế trải nghiệm mua sắm quanh từng pha cầu.
              </h2>
            </div>
            <div className="grid grid-cols-1 gap-5 md:grid-cols-3">
              {principles.map((principle) => (
                <div key={principle.title} className="border-l border-slate-200 pl-5">
                  <h3 className="text-base font-black text-[#061017]">{principle.title}</h3>
                  <p className="mt-3 text-sm leading-6 text-slate-600">{principle.description}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="grid grid-cols-1 gap-8 py-12 lg:grid-cols-[0.9fr_1.1fr] lg:py-16">
          <div className="relative min-h-[360px] overflow-hidden rounded-[28px] bg-slate-200">
            <Image
              src="/assets/images/badmintonimage1.png"
              alt="Chi tiết vợt cầu lông được FlyShot tuyển chọn"
              fill
              sizes="(min-width: 1024px) 42vw, 100vw"
              className="object-cover"
            />
          </div>

          <div className="flex flex-col justify-center">
            <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-2xl bg-emerald-100 text-emerald-800">
              <Sparkles className="h-5 w-5" strokeWidth={1.8} />
            </div>
            <h2 className="max-w-xl text-4xl font-black leading-[0.98] tracking-[-0.05em] text-[#061017] sm:text-5xl">
              Cao cấp không phải là xa cách. Cao cấp là chính xác.
            </h2>
            <p className="mt-6 max-w-2xl text-base leading-7 text-slate-600">
              Đội ngũ của chúng tôi kết hợp kiến thức sản phẩm với trải nghiệm trên sân. Chúng tôi tìm hiểu cách bạn phòng thủ, hồi vị và mất nhịp ở đâu, rồi thu hẹp lựa chọn vào những thiết bị cải thiện đúng các khoảnh khắc đó.
            </p>

            <div className="mt-8 space-y-4">
              {services.map((service) => {
                const Icon = service.icon;

                return (
                  <div key={service.title} className="flex gap-4 rounded-3xl border border-slate-200 bg-white p-5 shadow-[0_18px_45px_rgba(15,23,42,0.05)]">
                    <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-2xl bg-[#061017] text-white">
                      <Icon className="h-5 w-5" strokeWidth={1.8} />
                    </div>
                    <div>
                      <h3 className="font-black text-[#061017]">{service.title}</h3>
                      <p className="mt-1 text-sm leading-6 text-slate-600">{service.description}</p>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </section>

        <section className="overflow-hidden rounded-[28px] bg-[#061017] text-white">
          <div className="grid grid-cols-1 lg:grid-cols-[1.15fr_0.85fr]">
            <div className="p-7 sm:p-10 lg:p-12">
              <p className="text-xs font-bold uppercase tracking-[0.24em] text-emerald-300">
                Tiêu chuẩn FlyShot
              </p>
              <h2 className="mt-5 max-w-2xl text-4xl font-black leading-[0.98] tracking-[-0.05em] sm:text-5xl">
                Thiết bị được tuyển chọn với kỷ luật của một buổi chuẩn bị thi đấu.
              </h2>
              <p className="mt-6 max-w-2xl text-base leading-7 text-white/68">
                Từ tư vấn ban đầu đến hỗ trợ sau bán hàng, mỗi bước đều được xây dựng để giảm phỏng đoán và giúp người chơi tập trung vào di chuyển, thời điểm ra vợt và khả năng kiểm soát.
              </p>
            </div>
            <div className="grid grid-cols-1 border-t border-white/10 lg:border-l lg:border-t-0">
              {["Nguồn hàng chính hãng", "Tư vấn kỹ thuật theo lối chơi", "Hỗ trợ địa phương nhanh"].map((item) => (
                <div key={item} className="flex items-center border-b border-white/10 px-7 py-6 last:border-b-0 sm:px-10">
                  <span className="mr-4 h-2 w-2 rounded-full bg-emerald-300" />
                  <span className="text-sm font-bold text-white/86">{item}</span>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="py-12 text-center lg:py-16">
          <h2 className="mx-auto max-w-2xl text-4xl font-black leading-[1] tracking-[-0.05em] text-[#061017] sm:text-5xl">
            Tìm cấu hình khiến cú đánh tiếp theo của bạn trở nên chắc chắn hơn.
          </h2>
          <div className="mt-8 flex flex-col justify-center gap-3 sm:flex-row">
            <Link
              href="/shop"
              className="inline-flex h-12 items-center justify-center rounded-full bg-emerald-500 px-6 text-sm font-black text-[#061017] transition-all duration-200 hover:bg-emerald-400 active:translate-y-px"
            >
              Khám phá FlyShot
            </Link>
            <Link
              href="/contact"
              className="inline-flex h-12 items-center justify-center rounded-full border border-slate-300 bg-white px-6 text-sm font-black text-[#061017] transition-all duration-200 hover:border-[#061017] active:translate-y-px"
            >
              Trao đổi với chuyên viên
            </Link>
          </div>
        </section>
      </div>
    </MainLayout>
  );
}
