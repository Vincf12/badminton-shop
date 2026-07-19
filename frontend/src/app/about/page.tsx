import Image from "next/image";
import Link from "next/link";
import { Be_Vietnam_Pro, Inter } from "next/font/google";
import {
  Gauge,
  MapPin,
  Clock,
  ShieldCheck,
  SlidersHorizontal,
  Quote,
  ArrowRight,
} from "lucide-react";
import { MainLayout } from "@/widgets/layout";

const display = Be_Vietnam_Pro({
  subsets: ["vietnamese", "latin"],
  weight: ["600", "700", "800"],
  display: "swap",
});

const body = Inter({
  subsets: ["vietnamese", "latin"],
  weight: ["400", "500", "600"],
  display: "swap",
});

const stats = [
  { value: "2018", label: "Năm thành lập" },
  { value: "5.000+", label: "Người chơi đã phục vụ" },
  { value: "42", label: "Bước kiểm tra vợt & dây" },
];

const timeline = [
  {
    year: "2018",
    title: "Khởi đầu từ một sân tập nhỏ",
    description:
      "FlyShot ra đời tại TP. Hồ Chí Minh từ nhu cầu rất thật: người chơi phong trào khó tìm được vợt đúng lối chơi và nơi căng dây đáng tin cậy.",
  },
  {
    year: "2020",
    title: "Mở phòng kỹ thuật riêng",
    description:
      "Đầu tư máy căng dây điện tử và quy trình kiểm tra khung vợt 42 bước, giảm sai số căng dây xuống dưới 0.5kg.",
  },
  {
    year: "2022",
    title: "Hợp tác nhà phân phối chính hãng",
    description:
      "Trở thành điểm bán được ủy quyền của nhiều thương hiệu vợt lớn, đảm bảo nguồn hàng chính hãng và bảo hành đầy đủ.",
  },
  {
    year: "Hiện tại",
    title: "Đồng hành cùng hơn 5.000 người chơi",
    description:
      "Từ người mới bắt đầu đến vận động viên phong trào thi đấu, mỗi tư vấn vẫn giữ nguyên một nguyên tắc: đúng người, đúng vợt.",
  },
];

const principles = [
  {
    title: "Tư vấn theo lối chơi thật",
    description:
      "Không bán theo trào lưu. Vợt được chọn dựa trên tốc độ vung, điểm tiếp xúc và cách bạn di chuyển trên sân.",
  },
  {
    title: "Minh bạch về thông số",
    description:
      "Trọng lượng, điểm cân bằng, độ cứng trục — mọi thông số kỹ thuật đều được giải thích rõ trước khi bạn quyết định.",
  },
  {
    title: "Đồng hành sau khi mua",
    description:
      "Căng lại dây, kiểm tra định kỳ hoặc đổi cấu hình khi lối chơi của bạn thay đổi — chúng tôi vẫn ở đó.",
  },
];

const services = [
  {
    icon: SlidersHorizontal,
    title: "Căng dây theo cá nhân",
    description: "Lực căng được tinh chỉnh theo tốc độ vung vợt và cảm giác cầu bạn muốn có.",
  },
  {
    icon: Gauge,
    title: "Thử vợt trước khi mua",
    description: "Trải nghiệm thực tế trên sân trước khi đưa ra lựa chọn cuối cùng.",
  },
  {
    icon: ShieldCheck,
    title: "Bảo hành rõ ràng",
    description: "Chính sách đổi trả và kiểm định minh bạch cho từng sản phẩm.",
  },
];

const testimonials = [
  {
    quote:
      "Mình từng đổi 3 cây vợt vì không hợp tay. Sau khi được FlyShot tư vấn theo tốc độ vung, cây vợt hiện tại là cây mình gắn bó lâu nhất.",
    name: "Minh Anh",
    role: "Người chơi phong trào, 4 năm",
  },
  {
    quote:
      "Đội ngũ giải thích thông số rất dễ hiểu, không ép mua vợt đắt tiền. Căng dây đúng như yêu cầu, cảm giác cầu ổn định qua nhiều buổi tập.",
    name: "Quốc Bảo",
    role: "Thành viên CLB cầu lông quận 7",
  },
];

const faqs = [
  {
    question: "FlyShot có tư vấn cho người mới chơi không?",
    answer:
      "Có. Phần lớn khách hàng của FlyShot là người chơi phong trào. Chúng tôi bắt đầu từ mục tiêu và thể trạng của bạn trước khi nói đến thông số vợt.",
  },
  {
    question: "Có thể thử vợt trước khi mua không?",
    answer:
      "Một số mẫu vợt trong kho có thể mượn thử trên sân theo lịch hẹn trước, giúp bạn chắc chắn hơn về cảm giác cầm và tiếp cầu.",
  },
  {
    question: "Thời gian căng dây mất bao lâu?",
    answer:
      "Thông thường 30–45 phút với máy căng dây điện tử. Bạn có thể chờ lấy ngay hoặc gửi vợt và quay lại sau.",
  },
  {
    question: "Chính sách bảo hành áp dụng như thế nào?",
    answer:
      "Vợt chính hãng được bảo hành theo tiêu chuẩn của nhà sản xuất. FlyShot hỗ trợ toàn bộ thủ tục bảo hành và kiểm tra khung định kỳ miễn phí.",
  },
];

export default function AboutPage() {
  return (
    <MainLayout>
      <div className={`${body.className} mx-auto w-full max-w-6xl px-4 text-[#0B1220] sm:px-6`}>
        
        {/* 1. HERO SECTION */}
        <section className="grid grid-cols-1 items-center gap-12 lg:grid-cols-12 lg:gap-16">
          <div className="lg:col-span-7">
            <p className="text-xl font-black tracking-[0.2em] text-emerald-600">
              Về FlyShot
            </p>
            <h1
              className={`${display.className} mt-4 text-4xl font-extrabold leading-[1.1] tracking-tight text-[#0B1220] sm:text-5xl lg:text-[3.5rem]`}
            >
              Thiết bị cầu lông cho người chơi nghiêm túc.
            </h1>
            <p className="mt-6 max-w-xl text-base leading-7 text-slate-600 sm:text-lg">
              FlyShot tuyển chọn vợt, dây và phụ kiện cao cấp — ưu tiên tốc độ,
              độ chính xác và cảm giác chạm cầu thật gọn gàng. Mỗi cây vợt rời
              cửa hàng đều đã qua tư vấn theo đúng lối chơi của bạn, không phải
              theo trào lưu.
            </p>

            <div className="mt-10 flex flex-wrap gap-4">
              <Link
                href="/shop"
                className="inline-flex h-12 items-center justify-center rounded-full bg-[#0B1220] px-7 text-sm font-semibold tracking-wide text-white transition-all hover:bg-[#1a2b3a] shadow-sm"
              >
                Mua sắm ngay
              </Link>
              <Link
                href="/contact"
                className="inline-flex h-12 items-center justify-center rounded-full border border-slate-300 px-7 text-sm font-semibold tracking-wide text-[#0B1220] transition-all hover:border-[#0B1220]"
              >
                Đặt lịch tư vấn
              </Link>
            </div>
          </div>

          <div className="lg:col-span-5">
            <div className="relative aspect-[4/5] w-full overflow-hidden rounded-2xl bg-slate-100 shadow-md">
              <Image
                src="/assets/images/banner-flyshot01.png"
                alt="Người chơi cầu lông FlyShot và thiết bị cao cấp"
                fill
                sizes="(min-width: 1024px) 40vw, 100vw"
                className="object-cover"
                priority
              />
            </div>
          </div>
        </section>

        {/* 3. STORY & TIMELINE */}
        <section className="py-16 lg:py-24">
          <div className="grid grid-cols-1 gap-12 lg:grid-cols-12 lg:gap-16">
            <div className="lg:col-span-4 lg:sticky lg:top-8 lg:h-fit">
              <p className="text-xs font-semibold uppercase tracking-[0.2em] text-emerald-600">
                Hành trình
              </p>
              <h2 className={`${display.className} mt-4 text-3xl font-bold leading-[1.2] tracking-tight text-[#0B1220] sm:text-4xl`}>
                Từ một sân tập nhỏ đến điểm đến của người chơi.
              </h2>
              <p className="mt-4 text-sm leading-6 text-slate-600">
                Chúng tôi không ngừng cải tiến để mang lại giá trị kỹ thuật chính xác nhất cho từng cú đánh của bạn.
              </p>
            </div>

            <div className="space-y-10 lg:col-span-8 border-l border-slate-200 pl-6 lg:pl-10 ml-2">
              {timeline.map((item) => (
                <div key={item.year} className="relative group">
                  <div className="absolute -left-[31px] lg:-left-[47px] top-1.5 h-3 w-3 rounded-full border-2 border-emerald-600 bg-white transition-colors group-hover:bg-emerald-600" />
                  <div className={`${display.className} text-sm font-bold tracking-wide text-emerald-600`}>
                    {item.year}
                  </div>
                  <h3 className={`${display.className} mt-2 text-lg font-bold text-[#0B1220]`}>
                    {item.title}
                  </h3>
                  <p className="mt-2 text-base leading-7 text-slate-600">
                    {item.description}
                  </p>
                </div>
              ))}
            </div>
          </div>
        </section>

        {/* 4. PRINCIPLES & FEATURE IMAGE */}
        <section className="border-t border-slate-200 py-16 lg:py-24">
          <div className="grid grid-cols-1 gap-12 items-center lg:grid-cols-12 lg:gap-16">
            <div className="lg:col-span-5">
              <div className="relative aspect-square overflow-hidden rounded-2xl bg-slate-100 shadow-sm">
                <Image
                  src="/assets/images/badmintonimage1.png"
                  alt="Chi tiết vợt cầu lông được FlyShot tuyển chọn"
                  fill
                  sizes="(min-width: 1024px) 42vw, 100vw"
                  className="object-cover"
                />
              </div>
            </div>

            <div className="lg:col-span-7">
              <p className="text-xs font-semibold uppercase tracking-[0.2em] text-emerald-600">
                Triết lý vận hành
              </p>
              <h2 className={`${display.className} mt-3 text-3xl font-bold leading-tight text-[#0B1220] sm:text-4xl`}>
                Cao cấp không phải là xa cách. Cao cấp là chính xác.
              </h2>
              
              <div className="mt-10 space-y-6">
                {principles.map((principle, index) => (
                  <div key={principle.title} className="flex gap-4 p-4 rounded-xl bg-slate-50/70 border border-slate-100">
                    <div className={`${display.className} text-base font-bold text-emerald-600 shrink-0 mt-0.5`}>
                      0{index + 1}.
                    </div>
                    <div>
                      <h3 className={`${display.className} text-base font-bold text-[#0B1220]`}>
                        {principle.title}
                      </h3>
                      <p className="mt-1.5 text-sm leading-6 text-slate-600">
                        {principle.description}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </section>

        {/* 5. SERVICES */}
        <section className="border-t border-slate-200 py-16 lg:py-24">
          <div className="text-center max-w-xl mx-auto mb-12">
            <p className="text-xs font-semibold uppercase tracking-[0.2em] text-emerald-600">Giá trị gia tăng</p>
            <h2 className={`${display.className} mt-3 text-3xl font-bold tracking-tight text-[#0B1220]`}>
              Dịch vụ tiêu chuẩn cao
            </h2>
          </div>
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-3">
            {services.map((service) => {
              const Icon = service.icon;
              return (
                <div key={service.title} className="p-6 rounded-2xl border border-slate-300 bg-white transition-all">
                  <div className="p-3 bg-emerald-50 text-emerald-600 rounded-xl w-fit">
                    <Icon className="h-6 w-6" strokeWidth={1.8} />
                  </div>
                  <h3 className={`${display.className} mt-5 text-base font-bold text-[#0B1220]`}>
                    {service.title}
                  </h3>
                  <p className="mt-2.5 text-sm leading-6 text-slate-600">
                    {service.description}
                  </p>
                </div>
              );
            })}
          </div>
        </section>

        {/* 6. TESTIMONIALS */}
        <section className="border-t border-slate-300 py-16 lg:py-24 bg-slate-50/50 -mx-4 px-4 sm:-mx-6 sm:px-6 rounded-3xl">
          <div className="max-w-6xl mx-auto">
            <h2 className={`${display.className} text-center text-2xl font-bold tracking-tight text-[#0B1220] sm:text-3xl mb-12`}>
              Đồng hành cùng cộng đồng người chơi
            </h2>
            <div className="grid grid-cols-1 gap-8 sm:grid-cols-2">
              {testimonials.map((testimonial) => (
                <div key={testimonial.name} className="flex flex-col justify-between p-6 bg-white rounded-2xl border border-slate-100 shadow-sm">
                  <div>
                    <Quote className="h-6 w-6 text-emerald-600/30 transform rotate-180" strokeWidth={2} />
                    <p className="mt-4 text-base leading-7 text-slate-700 italic">
                      {testimonial.quote}
                    </p>
                  </div>
                  <div className="mt-6 pt-4 border-t border-slate-100 flex items-center justify-between">
                    <div>
                      <div className={`${display.className} text-sm font-bold text-[#0B1220]`}>
                        {testimonial.name}
                      </div>
                      <div className="text-xs text-slate-500 mt-0.5">{testimonial.role}</div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </section>

        {/* 7. FAQ */}
        <section className="border-t border-slate-200 py-16 lg:py-24 max-w-3xl mx-auto">
          <h2 className={`${display.className} text-center text-2xl font-bold tracking-tight text-[#0B1220] sm:text-3xl mb-8`}>
            Giải đáp thắc mắc
          </h2>
          <div className="divide-y divide-slate-200 border-y border-slate-200">
            {faqs.map((faq) => (
              <details key={faq.question} className="group py-5">
                <summary className={`${display.className} flex cursor-pointer list-none items-center justify-between text-base font-semibold text-[#0B1220] hover:text-emerald-600 transition-colors`}>
                  <span>{faq.question}</span>
                  <span className="ml-4 shrink-0 text-xl text-slate-400 transition-transform duration-200 group-open:rotate-45">
                    +
                  </span>
                </summary>
                <p className="mt-3 text-sm leading-6 text-slate-600 pl-1">
                  {faq.answer}
                </p>
              </details>
            ))}
          </div>
        </section>

        {/* 8. VISIT STORE CARD */}
        <section className="py-6">
          <div className="bg-gradient-to-br from-[#0B1220] via-[#0F172A] to-[#0A221D] rounded-2xl p-8 sm:p-12 shadow-xl grid grid-cols-1 gap-8 md:grid-cols-12 md:items-centerbg-gradient-to-br from-slate-50 via-slate-100 to-emerald-50/30 text-slate-900 rounded-2xl p-8 sm:p-12 shadow-md border border-slate-200/60 grid grid-cols-1 gap-8 md:grid-cols-12 md:items-center">
            <div className="md:col-span-7">
              <span className="text-xs font-bold uppercase tracking-widest text-emerald-400">Trải nghiệm thực tế</span>
              <h2 className={`${display.className} text-2xl font-bold tracking-tight sm:text-3xl mt-2`}>
                Ghé cửa hàng để thử vợt trực tiếp
              </h2>
              <p className="mt-3 max-w-md text-sm text-gray-600 leading-relaxed">
                Trải nghiệm cảm giác cầm vợt thực tế và nhận tư vấn đo thông số trực tiếp từ đội ngũ kỹ thuật lành nghề của FlyShot.
              </p>
              <Link
                href="/contact"
                className="mt-6 inline-flex h-11 items-center justify-center rounded-full bg-emerald-500 px-6 text-sm font-semibold tracking-wide text-[#0B1220] transition-colors hover:bg-emerald-400"
              >
                Xem chỉ đường <ArrowRight className="ml-2 h-4 w-4" />
              </Link>
            </div>
            
            <div className="space-y-4 md:col-span-5 border-t border-slate-700/60 pt-6 md:border-t-0 md:border-l md:border-slate-700/60 md:pt-0 md:pl-8">
              <div className="flex gap-4">
                <MapPin className="h-5 w-5 shrink-0 text-emerald-400 mt-0.5" strokeWidth={2} />
                <div>
                  <h4 className="text-xs font-bold text-slate-400 uppercase tracking-wider">Địa chỉ</h4>
                  <p className="text-sm text-gray-600 mt-1">
                    Số 12, Đường ABC, Quận 7, TP. Hồ Chí Minh
                  </p>
                </div>
              </div>
              <div className="flex gap-4">
                <Clock className="h-5 w-5 shrink-0 text-emerald-400 mt-0.5" strokeWidth={2} />
                <div>
                  <h4 className="text-xs font-bold text-slate-400 uppercase tracking-wider">Giờ hoạt động</h4>
                  <p className="text-sm text-gray-600 mt-1">
                    9:00 – 21:00, tất cả các ngày trong tuần
                  </p>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* 9. FINAL CTA */}
        <section className="py-16 text-center lg:py-24">
          <h2 className={`${display.className} mx-auto max-w-2xl text-2xl font-bold leading-tight text-[#0B1220] sm:text-4xl`}>
            Tìm cấu hình phù hợp cho cú đánh tiếp theo của bạn.
          </h2>
          <div className="mt-8 flex flex-wrap justify-center gap-4">
            <Link
              href="/shop"
              className="inline-flex h-12 items-center justify-center rounded-full bg-emerald-600 px-7 text-sm font-semibold tracking-wide text-white transition-all hover:bg-emerald-500 shadow-md"
            >
              Khám phá FlyShot
            </Link>
            <Link
              href="/contact"
              className="inline-flex h-12 items-center justify-center rounded-full border border-slate-300 px-7 text-sm font-semibold tracking-wide text-[#0B1220] transition-all hover:border-[#0B1220]"
            >
              Trao đổi với chuyên viên
            </Link>
          </div>
        </section>

      </div>
    </MainLayout>
  );
}