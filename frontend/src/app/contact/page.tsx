'use client'

import React, { useState } from 'react'
import MainLayout from '../components/layout/MainLayout'

export default function ContactPage() {
  const [formData, setFormData] = useState({
    fullName: '',
    phone: '',
    address: '',
    email: '',
    message: ''
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    console.log('FlyShot Contact Data:', formData)
  }

  return (
    <MainLayout>
      <div className="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 lg:py-8">
        <h1 className="text-3xl lg:text-4xl font-bold text-gray-900 mb-2">Liên Hệ</h1>
        <p className="text-gray-700 text-lg ">Chúng tôi rất mong được nghe từ bạn! Hãy điền vào mẫu bên dưới để đăng ký trải nghiệm sản phẩm hoặc gửi phản hồi về dịch vụ của chúng tôi.</p>
        <p className="text-gray-700 text-lg">Địa chỉ cửa hàng và thông tin liên hệ của chúng tôi cũng được cung cấp bên dưới nếu bạn muốn đến thăm trực tiếp hoặc liên hệ qua điện thoại/email.</p>

        {/* Main Content: Split Layout (Map Left, Form Right) */}
        <main className="max-w-7xl mx-auto sm:px-6 lg:px-8 py-12 md:py-16">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 lg:gap-16 items-start">
            
            {/* LEFT COLUMN: Google Maps Embedded */}
            <div className="w-full aspect-[4/3] lg:aspect-square bg-gray-100 border border-gray-200 top-8">
              {/* Thay thế src bằng link iframe thực tế của bạn khi đưa vào vận hành */}
              <iframe 
                src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3723.8638558814504!2d105.7917861759187!3d21.038132787458117!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ab3b4be6e6d1%3A0x6b30f36746cfdf3a!2zMTEzIFRy4bqnbiDEkMSDbmcgTmluaCwgROG7Y2ggVuG7峮uLCBD4bqndSBHaeG6pXksIEjDoCBO4buZaSwgVmnhu4d0IE5hbQ!5e0!3m2!1svi!2s!4v1716900000000!5m2!1svi!2s" 
                className="w-full h-full opacity-90 contrast-115 transition-all duration-500"
                allowFullScreen={true}
                loading="lazy" 
                referrerPolicy="no-referrer-when-downgrade"
                title="FlyShot Flagship Store Map"
              ></iframe>
            </div>

            {/* RIGHT COLUMN: Contact Info & Form */}
            <div className="space-y-10">
              
              {/* Brand Meta Info */}
              <div className="space-y-3 border-b border-gray-100 pb-8 text-sm md:text-base">
                <div className="flex flex-col sm:flex-row sm:items-baseline gap-1">
                  <span className="font-mono text-xs uppercase tracking-wider text-gray-400 sm:w-28 block">Địa chỉ :</span>
                  <span className="font-medium text-gray-900">113 Trần Đăng Ninh - Dịch Vọng - Cầu Giấy - Hà Nội</span>
                </div>
                <div className="flex flex-col sm:flex-row sm:items-baseline gap-1">
                  <span className="font-mono text-xs uppercase tracking-wider text-gray-400 sm:w-28 block">Email :</span>
                  <span className="font-medium text-gray-900">contact@flyshot.vn</span>
                </div>
                <div className="flex flex-col sm:flex-row sm:items-baseline gap-1">
                  <span className="font-mono text-xs uppercase tracking-wider text-gray-400 sm:w-28 block">Bán Lẻ :</span>
                  <span className="font-bold text-gray-950 font-mono">0123.456.789</span>
                </div>
                <div className="flex flex-col sm:flex-row sm:items-baseline gap-1">
                  <span className="font-mono text-xs uppercase tracking-wider text-gray-400 sm:w-28 block">Bán Buôn :</span>
                  <span className="font-bold text-gray-950 font-mono">0987.654.321 — 0912.345.678</span>
                </div>
              </div>

              {/* Contact Form */}
              <div className="space-y-6">
                <div className="space-y-1">
                  <h2 className="text-xl font-black uppercase tracking-wider text-gray-950 font-mono">FlyShot</h2>
                  <p className="text-xs uppercase tracking-widest text-gray-400">Phòng đăng ký trải nghiệm & phản hồi</p>
                </div>

                <form onSubmit={handleSubmit} className="space-y-5">
                  <div>
                    <label className="block text-xs uppercase tracking-widest font-bold text-gray-500 mb-1.5">
                      Họ và tên
                    </label>
                    <input
                      type="text"
                      required
                      placeholder="Họ và tên"
                      className="w-full px-4 py-3 border border-gray-200 text-sm text-gray-950 placeholder-gray-300 bg-transparent rounded-none focus:outline-none focus:border-black focus:ring-1 focus:ring-black transition-all"
                      value={formData.fullName}
                      onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                    />
                  </div>

                  <div>
                    <label className="block text-xs uppercase tracking-widest font-bold text-gray-500 mb-1.5">
                      Số điện thoại
                    </label>
                    <input
                      type="tel"
                      required
                      placeholder="Số điện thoại"
                      className="w-full px-4 py-3 border border-gray-200 text-sm text-gray-950 placeholder-gray-300 bg-transparent rounded-none focus:outline-none focus:border-black focus:ring-1 focus:ring-black transition-all"
                      value={formData.phone}
                      onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                    />
                  </div>

                  <div>
                    <label className="block text-xs uppercase tracking-widest font-bold text-gray-500 mb-1.5">
                      Địa chỉ
                    </label>
                    <input
                      type="text"
                      placeholder="Địa chỉ"
                      className="w-full px-4 py-3 border border-gray-200 text-sm text-gray-950 placeholder-gray-300 bg-transparent rounded-none focus:outline-none focus:border-black focus:ring-1 focus:ring-black transition-all"
                      value={formData.address}
                      onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                    />
                  </div>

                  <div>
                    <label className="block text-xs uppercase tracking-widest font-bold text-gray-500 mb-1.5">
                      Email
                    </label>
                    <input
                      type="email"
                      required
                      placeholder="Email"
                      className="w-full px-4 py-3 border border-gray-200 text-sm text-gray-950 placeholder-gray-300 bg-transparent rounded-none focus:outline-none focus:border-black focus:ring-1 focus:ring-black transition-all"
                      value={formData.email}
                      onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    />
                  </div>

                  <div>
                    <label className="block text-xs uppercase tracking-widest font-bold text-gray-500 mb-1.5">
                      Nội dung
                    </label>
                    <textarea
                      rows={5}
                      placeholder="Nội dung tin nhắn"
                      className="w-full px-4 py-3 border border-gray-200 text-sm text-gray-950 placeholder-gray-300 bg-transparent rounded-none focus:outline-none focus:border-black focus:ring-1 focus:ring-black transition-all resize-none"
                      value={formData.message}
                      onChange={(e) => setFormData({ ...formData, message: e.target.value })}
                    />
                  </div>

                  <div className="pt-2">
                    <button
                      type="submit"
                      className="px-8 py-3.5 bg-gray-950 text-white font-bold uppercase tracking-widest text-xs rounded-none hover:bg-gray-800 active:bg-black transition-colors"
                    >
                      Gửi
                    </button>
                  </div>
                </form>
              </div>

            </div>

          </div>
        </main>

      </div>
    </MainLayout>
  )
}