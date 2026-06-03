"use client";

import React, { useEffect, useState } from "react";
import MainLayout from "@/app/components/layout/MainLayout";
import HeroBanner from "@/app/components/home/HeroBanner";
import FeatureCard from "@/app/components/home/FeatureCard";
import ProductGrid from "@/app/components/home/ProductGrid";
import Newsletter from "@/app/components/home/Newsletter";
import SectionTitle from "./components/home/SectionTitle";
import { fetchProducts } from "@/services/productService";
import type { ProductCardModel } from "@/types/product";

export default function HomePage() {
  const [products, setProducts] = useState<ProductCardModel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    const loadProducts = async () => {
      try {
        const data = await fetchProducts();
        if (isActive) {
          setProducts(data);
        }
      } catch (loadError) {
        if (isActive) {
          setError(loadError instanceof Error ? loadError.message : "Không thể tải sản phẩm");
        }
      } finally {
        if (isActive) {
          setLoading(false);
        }
      }
    };

    loadProducts();

    return () => {
      isActive = false;
    };
  }, []);

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

  const getSectionProducts = (categoryName: string) => {
    const filtered = products.filter((product) => product.category === categoryName);
    return filtered.length > 0 ? filtered : products.slice(0, 5);
  };

  return (
    <>
      <HeroBanner />

      <MainLayout>
        <div className="home-page">
          <section className="mb-5 mt-2">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              {features.map((feature, index) => (
                <FeatureCard key={index} feature={feature} />
              ))}
            </div>
          </section>

          <section className="mb-20">
            <SectionTitle title="Sản phẩm mới" />
            {loading ? <p className="mb-10 text-gray-600">Đang tải sản phẩm...</p> : null}
            {error ? <p className="mb-10 text-red-600">{error}</p> : null}
            <ProductGrid products={products.slice(0, 5)} />

            <SectionTitle title="Vợt cầu lông" />
            <ProductGrid products={getSectionProducts("Vợt cầu lông")} />

            <SectionTitle title="Giày cầu lông" />
            <ProductGrid products={getSectionProducts("Giày cầu lông")} showBadge={false} />

            <SectionTitle title="Phụ kiện" />
            <ProductGrid products={getSectionProducts("Phụ kiện")} />
          </section>

          <Newsletter />
        </div>
      </MainLayout>
    </>
  );
}
