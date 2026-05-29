'use client'

import React from 'react'
import Link from 'next/link'
import MainLayout from '../components/layout/MainLayout'

export default function AboutPage() {
  return (
    <MainLayout>
      <div className="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 lg:py-8">
        <h1 className="text-3xl text-black font-bold mb-4">Về Chúng Tôi</h1>
        <p className="text-gray-700 mb-6">
          FlyShot là thương hiệu vợt cầu lông cao cấp được thành lập vào năm 2018 bởi một nhóm đam mê thể thao và công nghệ. Chúng tôi cam kết mang đến cho người chơi những trải nghiệm tốt nhất trên sân đấu thông qua sự kết hợp giữa thiết kế tinh tế và công nghệ tiên tiến.
        </p>
        {/* Editorial Header Hero */}
        <header className="border-b border-gray-300 py-16 md:py-24 ">
          <div className="max-w-5xl mx-auto px-4 text-center">
            <p className="text-xs tracking-[0.3em] uppercase text-gray-400 font-bold mb-4">
              The FlyShot Chronicles
            </p>
            <h1 className="text-5xl md:text-7xl font-black uppercase tracking-tight text-gray-900 mb-8 leading-[0.95]">
              FlyShot
            </h1>
            <p className="text-xl md:text-2xl font-serif italic text-gray-600 max-w-3xl mx-auto leading-relaxed">
              "Nơi giọt mồ hôi chuyển hóa thành quỹ đạo hoàn mỹ. Chúng tôi không chỉ chế tạo vợt; chúng tôi định hình lại cách bạn làm chủ khoảng không."
            </p>
          </div>
        </header>
        {/* Main Content Container */}
        <div className="w-full px-4 py-16">
          
          {/* Section 1: Khởi đầu */}
          <section className="mb-20 grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
            <div className="md:col-span-4">
              <span className="text-xs font-mono font-bold text-gray-400 block mb-2">01 / LỊCH SỬ</span>
              <h2 className="text-2xl font-black uppercase tracking-tight text-gray-950">
                Khởi Đầu Từ Một Ý Niệm Điên Rồ
              </h2>
            </div>
            <div className="md:col-span-8 text-gray-700 space-y-6 text-lg leading-relaxed">
              <h3 className="text-xl font-bold text-gray-900 font-sans italic">
                Đường cong hoàn hảo không sinh ra từ phòng thí nghiệm thông thường.
              </h3>
              <p>
                Mọi chuyện bắt đầu vào mùa hè năm 2018 tại một xưởng cơ khí nhỏ vùng ngoại ô. Khi đó, những người sáng lập của FlyShot – một kỹ sư hàng không vũ trụ và một cựu vận động viên chuyên nghiệp – cùng nhìn về một hướng: <span className="italic font-medium text-gray-950">Tại sao cây vợt cầu lông phải luôn tuân theo những giới hạn truyền thống?</span>
              </p>
              <p>
                Họ nhận ra rằng, phần lớn người chơi đang phải hy sinh tốc độ để đổi lấy sức mạnh, hoặc ngược lại. FlyShot ra đời với một sứ mệnh duy nhất: phá vỡ thế độc tôn của sự thỏa hiệp. Chúng tôi muốn tạo ra một thứ vũ khí mà ở đó, sợi carbon không chỉ là vật liệu, nó là phần mở rộng của cánh tay bạn.
              </p>
              
              {/* Editorial Blockquote */}
              <blockquote className="border-l-4 border-black pl-6 my-8 font-serif italic text-xl text-gray-900 bg-gray-50 py-4 pr-4">
                "FlyShot không sinh ra để nằm trong túi đựng. Nó được sinh ra để cắt đôi không khí và để lại những tiếng vang đầy kiêu hãnh trên mặt sân."
                <cite className="block text-xs uppercase tracking-widest font-sans font-bold text-gray-400 mt-3 not-italic">
                  — Minh Vũ, Founder & CEO FlyShot
                </cite>
              </blockquote>
            </div>
          </section>

          <hr className="border-gray-200 my-12" />

          {/* Section 2: Triết lý thiết kế */}
          <section className="mb-20 grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
            <div className="md:col-span-4">
              <span className="text-xs font-mono font-bold text-gray-400 block mb-2">02 / TRIẾT LÝ</span>
              <h2 className="text-2xl font-black uppercase tracking-tight text-gray-950">
                Bản Giao Hưởng Của Khí Động Học
              </h2>
            </div>
            <div className="md:col-span-8 text-gray-700 space-y-6 text-lg leading-relaxed">
              <h3 className="text-xl font-bold text-gray-900 font-sans italic">
                Khi công nghệ tối tân cúi đầu trước nghệ thuật chuyển động.
              </h3>
              <p>
                Tại FlyShot, chúng tôi tin rằng mỗi cú đập cầu (smash) là một tác phẩm nghệ thuật. Để tác phẩm đó đạt đến độ chín muồi, cây vợt phải đạt được sự cân bằng tuyệt đối giữa ba yếu tố: <span className="font-semibold text-gray-950">Trọng lượng siêu nhẹ</span>, <span className="font-semibold text-gray-950">Độ đàn hồi phản hồi tức thì</span>, và <span className="font-semibold text-gray-950">Khung vợt xé gió</span>.
              </p>
              <p>
                Chúng tôi áp dụng cấu trúc khung <span className="font-medium">Aero-Dynamic Grid</span> mô phỏng cánh máy bay chiến đấu, giúp giảm tối đa lực cản không khí. Kết hợp với công nghệ sợi carbon mật độ cao, mỗi cây vợt FlyShot cho phép bạn vung vợt nhanh hơn 0.15 giây so với tiêu chuẩn thông thường.
              </p>

              {/* Editorial Table */}
              <div className="pt-6 overflow-x-auto">
                <p className="text-xs uppercase tracking-widest font-bold text-gray-400 mb-3">Những Cột Mốc Định Hình Thương Hiệu</p>
                <table className="w-full text-left text-sm border-collapse">
                  <thead>
                    <tr className="border-b-2 border-gray-900">
                      <th className="py-2 font-mono font-bold text-gray-500 w-16">NĂM</th>
                      <th className="py-2 font-bold text-gray-900 pr-4">CỘT MỐC PHÁT TRIỂN</th>
                      <th className="py-2 font-bold text-gray-900">THÀNH TỰU ĐẠT ĐƯỢC</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-100">
                    <tr>
                      <td className="py-3 font-mono font-bold">2019</td>
                      <td className="py-3 font-medium pr-4">Thử nghiệm khung vợt Air-Slice</td>
                      <td className="py-3 text-gray-600">Giảm 12% lực cản không khí so với khung oval truyền thống.</td>
                    </tr>
                    <tr>
                      <td className="py-3 font-mono font-bold">2021</td>
                      <td className="py-3 font-medium pr-4">Ra mắt BST FlyShot Prime đầu tiên</td>
                      <td className="py-3 text-gray-600">Cháy hàng 5.000 bản giới hạn chỉ trong vòng 48 giờ ra mắt.</td>
                    </tr>
                    <tr>
                      <td className="py-3 font-mono font-bold">2024</td>
                      <td className="py-3 font-medium pr-4">Đồng hành cùng giải đấu Quốc tế</td>
                      <td className="py-3 text-gray-600">Trở thành nhà tài trợ chính thức cho 3 tay vợt nằm trong Top 50 thế giới.</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </section>

          <hr className="border-gray-200 my-12" />

          {/* Section 3: Đặc quyền dịch vụ */}
          <section className="mb-20 grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
            <div className="md:col-span-4">
              <span className="text-xs font-mono font-bold text-gray-400 block mb-2">03 / ĐẶC QUYỀN</span>
              <h2 className="text-2xl font-black uppercase tracking-tight text-gray-950">
                Dịch Vụ Chuẩn FlyShot
              </h2>
            </div>
            <div className="md:col-span-8 text-gray-700 space-y-8 text-lg leading-relaxed">
              <h3 className="text-xl font-bold text-gray-900 font-sans italic">
                Khi bạn chọn FlyShot, bạn đang tham gia vào một hệ sinh thái chăm sóc đẳng cấp VIP.
              </h3>
              
              <div className="space-y-6">
                <div>
                  <h4 className="font-bold text-gray-950 text-base uppercase tracking-wider mb-1">Cá Nhân Hóa Trải Nghiệm (Bespoke Stringing)</h4>
                  <p className="text-gray-600">Tại các Flagship Store, chúng tôi đo áp lực tay bằng cảm biến để tư vấn loại lưới và số kg căng vợt chính xác đến từng micro-vừa vặn với lối chơi của riêng bạn.</p>
                </div>
                <div>
                  <h4 className="font-bold text-gray-950 text-base uppercase tracking-wider mb-1">Thử Vợt Miễn Phí (The Test Drive)</h4>
                  <p className="text-gray-600">Chương trình cho phép bạn mượn vợt trải nghiệm ngay tại sân đấu trong vòng 3 ngày trước khi đưa ra quyết định mua hàng chính thức.</p>
                </div>
                <div>
                  <h4 className="font-bold text-gray-950 text-base uppercase tracking-wider mb-1">Bảo Dưỡng Trọn Đời (Lifetime Wellness)</h4>
                  <p className="text-gray-600">Thay gen vợt định kỳ miễn phí và kiểm tra độ cân bằng của khung định kỳ mỗi 6 tháng tại tất cả các trung tâm bảo hành.</p>
                </div>
              </div>
            </div>
          </section>

          <hr className="border-gray-200 my-12" />

          {/* Section 4: Chính sách bảo vệ */}
          <section className="mb-20 grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
            <div className="md:col-span-4">
              <span className="text-xs font-mono font-bold text-gray-400 block mb-2">04 / CHÍNH SÁCH</span>
              <h2 className="text-2xl font-black uppercase tracking-tight text-gray-950">
                Chính Sách Bảo Vệ Tuyệt Đối
              </h2>
            </div>
            <div className="md:col-span-8 text-gray-700 space-y-6 text-lg leading-relaxed">
              <h3 className="text-xl font-bold text-gray-900 font-sans italic">
                Niềm tin là thứ vật liệu đắt giá nhất cấu thành nên FlyShot.
              </h3>
              
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 pt-4">
                <div className="border border-gray-200 p-6 rounded-none hover:border-black transition-colors">
                  <span className="text-xs font-mono font-bold text-gray-400 block mb-3">01 . BẢO HÀNH</span>
                  <p className="font-bold text-gray-950 mb-2">1 Đổi 1 Trong 90 Ngày</p>
                  <p className="text-sm text-gray-500">Nếu xuất hiện vết nứt do lỗi kết cấu carbon từ nhà sản xuất.</p>
                </div>
                <div className="border border-gray-200 p-6 rounded-none hover:border-black transition-colors">
                  <span className="text-xs font-mono font-bold text-gray-400 block mb-3">02 . ĐỔI TRẢ</span>
                  <p className="font-bold text-gray-950 mb-2">7 Ngày Không Lý Do</p>
                  <p className="text-sm text-gray-500">Hoàn tiền 100% nếu sản phẩm còn nguyên vẹn tem mác, chưa bóc seal cán.</p>
                </div>
                <div className="border border-gray-200 p-6 rounded-none hover:border-black transition-colors">
                  <span className="text-xs font-mono font-bold text-gray-400 block mb-3">03 . GIAO HÀNG</span>
                  <p className="font-bold text-gray-950 mb-2">Hỏa Tốc Đặc Quyền</p>
                  <p className="text-sm text-gray-500">Giao nhanh 2H nội thành và bảo hiểm hư hại 100% toàn quốc.</p>
                </div>
              </div>
            </div>
          </section>

          <hr className="border-gray-200 my-12" />

          {/* Section 5: Đội ngũ & Con người */}
          <section className="mb-20 grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
            <div className="md:col-span-4 sticky top-6">
              <span className="text-xs font-mono font-bold text-gray-400 block mb-2">05 / CON NGƯỜI</span>
              <h2 className="text-2xl font-black uppercase tracking-tight text-gray-950">
                Phía Sau Những Đường Cầu
              </h2>
            </div>
            <div className="md:col-span-8 text-gray-700 space-y-6 text-lg leading-relaxed">
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-8">
                <div>
                  <p className="font-mono text-xs text-gray-400 font-bold">R&D DIRECTOR</p>
                  <h4 className="font-bold text-gray-950 text-lg mb-2">Trần Tiến Đạt</h4>
                  <p className="text-sm text-gray-600">Chuyên gia với 12 năm kinh nghiệm ngành vật liệu composite. Người ám ảnh bởi việc tối ưu hóa khung vợt mỏng thêm 0.1mm.</p>
                </div>
                <div>
                  <p className="font-mono text-xs text-gray-400 font-bold">CHIEF DESIGNER</p>
                  <h4 className="font-bold text-gray-950 text-lg mb-2">Elena Rostova</h4>
                  <p className="text-sm text-gray-600">Đến từ Milan, Elena mang tư duy thẩm mỹ nghệ thuật thời trang Ý vào các họa tiết góc cạnh, mạnh mẽ trên thân vợt FlyShot.</p>
                </div>
              </div>
            </div>
          </section>

          {/* Big Editorial Manifesto Section */}
          <section className="mb-20 bg-gray-950 text-white p-8 md:p-12 text-center rounded-none">
            <span className="text-xs font-mono tracking-widest text-gray-500 block mb-4">FLYSHOT ELITE MANIFESTO</span>
            <h2 className="text-3xl md:text-4xl font-serif italic mb-6 max-w-2xl mx-auto leading-snug">
              "Bạn đã sẵn sàng cùng FlyShot định nghĩa lại giới hạn của chính mình?"
            </h2>
            <p className="text-sm text-gray-400 max-w-xl mx-auto leading-relaxed mb-8">
              Hệ thống của chúng tôi được xây dựng dựa trên sự cam kết tuyệt đối: 100% sản phẩm cao cấp, tư vấn chuyên sâu theo cơ địa, hỗ trợ kỹ thuật trọn đời.
            </p>
            <div className="flex flex-wrap justify-center gap-12 border-t border-gray-800 pt-8">
              <div>
                <div className="text-3xl font-black tracking-tight font-mono">5K+</div>
                <div className="text-xs uppercase tracking-wider text-gray-500 mt-1">Hội Viên Thân Thiết</div>
              </div>
              <div>
                <div className="text-3xl font-black tracking-tight font-mono">0.15s</div>
                <div className="text-xs uppercase tracking-wider text-gray-500 mt-1">Tốc Độ Vung Vợt Nhanh Hơn</div>
              </div>
              <div>
                <div className="text-3xl font-black tracking-tight font-mono">42</div>
                <div className="text-xs uppercase tracking-wider text-gray-500 mt-1">Bài Kiểm Tra Áp Lực</div>
              </div>
            </div>
          </section>

          {/* Editorial Footer / Contact CTA */}
          <footer className="border border-gray-200 p-8 md:p-12 text-center rounded-none">
            <h3 className="text-xl font-bold uppercase tracking-wider text-gray-950 mb-8">
              KẾT NỐI VỚI CHÚNG TÔI
            </h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-6 mb-12 text-left border-b border-gray-100 pb-8">
              <div>
                <span className="text-xs font-mono text-gray-400 block mb-1">HOTLINE</span>
                <span className="font-bold text-gray-950">0123 456 789</span>
              </div>
              <div>
                <span className="text-xs font-mono text-gray-400 block mb-1">EMAIL</span>
                <span className="font-bold text-gray-950 text-sm break-all">contact@flyshot.vn</span>
              </div>
              <div>
                <span className="text-xs font-mono text-gray-400 block mb-1">FLAGSHIP STORE</span>
                <span className="font-bold text-gray-950">TP. Hồ Chí Minh</span>
              </div>
              <div>
                <span className="text-xs font-mono text-gray-400 block mb-1">GIỜ LÀM VIỆC</span>
                <span className="font-bold text-gray-950">08:00 — 21:00</span>
              </div>
            </div>
            
            <div className="flex flex-col sm:flex-row justify-center gap-4">
              <Link
                href="/shop"
                className="px-8 py-3 bg-gray-950 text-white font-bold uppercase tracking-wider text-xs hover:bg-gray-800 transition-colors"
              >
                Khám Phá Các Bộ Sưu Tập
              </Link>
              <Link
                href="/contact"
                className="px-8 py-3 bg-white text-gray-950 font-bold uppercase tracking-wider text-xs border border-gray-950 hover:bg-gray-50 transition-colors"
              >
                Yêu Cầu Đặt Lịch Tư Vấn
              </Link>
            </div>
          </footer>

        </div>
      </div>
    </MainLayout>
  )
}