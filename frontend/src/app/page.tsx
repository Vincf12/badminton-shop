"use client";

import React from "react";
import Link from "next/link";
import MainLayout from "@/app/components/layout/MainLayout";
import HeroBanner from "@/app/components/home/HeroBanner";
import { authService } from "@/services/authService";
import { usePathname, useRouter} from "next/navigation";
import FeatureCard from "@/app/components/home/FeatureCard";
import ProductGrid from "@/app/components/home/ProductGrid";
import Newsletter from "@/app/components/home/Newsletter";
import SectionTitle from "./components/home/SectionTitle";

export default function HomePage() {
  const features = [
    {
      icon: "🏆",
      title: "Chất lượng cao",
      description: "Sản phẩm chính hãng từ các thương hiệu hàng đầu",
    },
    {
      icon: "🚚",
      title: "Giao hàng nhanh",
      description: "Miễn phí vận chuyển cho đơn hàng trên 500k",
    },
    {
      icon: "🎗️",
      title: "Bảo hành uy tín",
      description: "Đổi trả trong 30 ngày nếu có lỗi",
    },
    {
      icon: "👨‍💼",
      title: "Tư vấn chuyên nghiệp",
      description: "Đội ngũ am hiểu cầu lông hỗ trợ 24/7",
    },
  ];

  const products = [
    {
      id: 1,
      name: "Vợt Yonex Astrox 99",
      price: "4.500.000đ",
      image:
        "https://cdn.shopvnb.com/img/300x300/uploads/san_pham/vot-cau-long-vnb-tc88c-1.webp",
    },
    {
      id: 2,
      name: "Vợt Victor Thruster K9900",
      price: "3.800.000đ",
      image:
        "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
    },
    {
      id: 3,
      name: "Giày Yonex 65Z3",
      price: "2.200.000đ",
      image:
        "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
    },
    {
      id: 4,
      name: "Giày Adidas Adizero Club",
      price: "1.900.000đ",
      image:
        "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
    },
    {
      id: 5,
      name: "Quần áo cầu lông Lining",
      price: "500.000đ",
      image:
        "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
    },
  ];

  return (
    <>
      {/* Banner full màn hình */}
      <HeroBanner />

      {/* Dashboard và các phần còn lại */}
      <MainLayout>
        <div className="home-page">
          {/* Features Section */}
          <section className="mb-5 mt-2">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              {features.map((feature, index) => (
                <FeatureCard key={index} feature={feature} />
              ))}
            </div>
          </section>

          {/* Featured Products */}
          <section className="mb-20">
            <SectionTitle title="Sản phẩm mới" />
            <ProductGrid products={products} />
            <SectionTitle title="Vợt cầu lông" />
            <ProductGrid products={products} />
            <SectionTitle title="Giày cầu lông" />
            <ProductGrid products={products} showBadge={false} />
            <SectionTitle title="Balo cầu lông" />
            <ProductGrid products={products} />
          </section>


          {/* CTA Section */}
          <Newsletter />
        </div>
      </MainLayout>
    </>
  );
}
