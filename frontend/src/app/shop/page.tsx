"use client";

import React from "react";
import MainLayout from "@/app/components/layout/MainLayout";
import ProductGrid from "@/app/components/home/ProductGrid";
import FilterGroup from "@/app/components/product/FilterGroup"; // Đã sửa tên import cho đúng chuẩn
import { Search, SlidersHorizontal } from "lucide-react";
import { addUniqueItem } from "framer-motion";

export default function ShopPage() {
  const products = [
    {
      id: 1,
      name: "Vợt Yonex Astrox 99",
      price: "4.500.000đ",
      image: "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
      category: "Vợt",
    },
    {
      id: 2,
      name: "Vợt Victor Thruster K9900",
      price: "3.800.000đ",
      image: "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
      category: "Vợt",
    },
    {
      id: 3,
      name: "Giày Yonex 65Z3",
      price: "2.200.000đ",
      image: "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
      category: "Giày",
    },
    {
      id: 4,
      name: "Cầu lông Yonex AS-03",
      price: "450.000đ",
      image: "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
      category: "Cầu",
    },
    {
      id: 5,
      name: "Túi đựng vợt Yonex Pro",
      price: "1.200.000đ",
      image: "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
      category: "Phụ kiện",
    },
  ];

  const prices = [
    { label: "Dưới 500k", value: "under-500k" },
    { label: "500k - 1 triệu", value: "500k-1m" },
    { label: "1 triệu - 3 triệu", value: "1m-3m" },
    { label: "Trên 3 triệu", value: "above-3m" },
  ];

  const categories = [
    "Vợt Cầu Lông",
    "Giày Cầu Lông",
    "Áo Cầu Lông",
    "Quần Cầu Lông",
    "Túi/Balo Cầu Lông",
    "Phụ Kiện Cầu Lông",
    "Vợt Pickleball",
    "Giày Pickleball",
    "Phụ Kiện Pickleball",
    "Máy Đan",
  ];

  const brands = ["APAVI", "X", "ASHAWAY", "ASICS", "KADA SX", "DUNLOP", "FECOLOS", "FF"];

  const sizeracket = ["665mm", "670mm","675mm"];

  const sortOptions = [
    "Khuyến mãi tốt",
    "Sản phẩm mới nhất",
    "Bán chạy nhất",
    "Giá tăng dần",
    "Giá giảm dần",
  ];

  const sizeFilters = ["4U", "5U", "6U", "7U", "8U"];
  const shoeSizes = ["37", "38", "39", "40", "41", "42"];
  const colors = ["Trắng", "Xanh", "Đen", "Hồng", "Vàng"];

  return (
    <MainLayout>
      <div className="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 lg:py-8">
        <div className="mb-6">
          <h1 className="text-3xl lg:text-4xl font-bold text-gray-900 mb-2">Sản Phẩm</h1>
          <p className="text-gray-600">
            Khám phá bộ sưu tập cầu lông và pickleball được chọn lọc cho người chơi ở mọi cấp độ.
          </p>
        </div>

        <div className="grid grid-cols-1 gap-6 lg:grid-cols-[280px_minmax(0,1fr)]">
          
          {/* ASIDE: SIDEBAR BỘ LỌC */}
          <aside className="space-y-4 lg:top-28 lg:self-start">
            <div className="rounded-2xl border border-gray-200 bg-white p-4 shadow-sm">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-semibold text-gray-900">Bộ lọc</h2>
                <SlidersHorizontal className="h-4 w-4 text-gray-500" />
              </div>

              {/* Thanh tìm kiếm nhanh */}
              <div className="flex items-center gap-2 rounded-xl border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-400 focus-within:border-emerald-500 focus-within:bg-white transition-colors">
                <Search className="h-4 w-4 flex-shrink-0" />
                <input 
                  type="text" 
                  placeholder="Tìm nhanh..." 
                  className="w-full bg-transparent text-gray-700 outline-none placeholder-gray-400 text-sm"
                />
              </div>

              {/* Khu vực chứa các khối Accordion Filters */}
              <div className="mt-4 space-y-1">
                {/* 0. Khoảng giá */}
                <FilterGroup title="Khoảng giá">
                  {prices.map((price) => (
                    <label key={price.value} className="flex items-center gap-2 cursor-pointer hover:text-emerald-600 transition-colors">
                      <input 
                        type="checkbox" 
                        className="h-4 w-4 rounded border-gray-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer" 
                      />
                      <span>{price.label}</span>
                    </label>
                  ))}
                </FilterGroup>

                {/* 1. Kích thước vợt */}
                <FilterGroup title="Kích thước vợt">
                  {sizeFilters.map((item) => (
                    <label key={item} className="flex items-center gap-2 cursor-pointer hover:text-emerald-600 transition-colors">
                      <input 
                        type="checkbox" 
                        className="h-4 w-4 rounded border-gray-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer" 
                      />
                      <span>{item}</span>
                    </label>
                  ))}
                </FilterGroup>

                {/* 2. Chiều dài vợt */}
                <FilterGroup title="Chiều dài vợt">
                  {sizeracket.map((item) => (
                    <label key={item} className="flex items-center gap-2 cursor-pointer hover:text-emerald-600 transition-colors">
                      <input 
                        type="checkbox" 
                        className="h-4 w-4 rounded border-gray-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer" 
                      />
                      <span>{item}</span>
                    </label>
                  ))}
                </FilterGroup>

                {/* 3. Màu sắc */}
                <FilterGroup title="Màu sắc">
                  {colors.map((item) => (
                    <label key={item} className="flex items-center gap-2 cursor-pointer hover:text-emerald-600 transition-colors">
                      <input 
                        type="checkbox" 
                        className="h-4 w-4 rounded border-gray-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer" 
                      />
                      <span>{item}</span>
                    </label>
                  ))}
                </FilterGroup>

                {/* 4. Form giày */}
                <FilterGroup title="Form giày">
                  {shoeSizes.map((item) => (
                    <label key={item} className="flex items-center gap-2 cursor-pointer hover:text-emerald-600 transition-colors">
                      <input 
                        type="checkbox" 
                        className="h-4 w-4 rounded border-gray-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer" 
                      />
                      <span>{item}</span>
                    </label>
                  ))}
                </FilterGroup>

              </div>
            </div>
          </aside>

          {/* SECTION: KHU VỰC HIỂN THỊ SẢN PHẨM */}
          <section className="space-y-5">
            {/* Top Categories & Brand list */}
            <div className="rounded-2xl border border-gray-200 bg-white p-4 shadow-sm">
              <div className="flex flex-wrap gap-3">
                {categories.map((item, index) => (
                  <button
                    key={item}
                    className={`rounded-xl border px-4 py-2 text-sm font-medium transition-colors ${
                      index === 0
                        ? "border-emerald-600 bg-emerald-600 text-white"
                        : "border-gray-300 bg-white text-gray-800 hover:border-emerald-500 hover:text-emerald-700"
                    }`}
                  >
                    {item}
                  </button>
                ))}
              </div>

              <div className="mt-4 flex flex-wrap items-center gap-3 overflow-hidden rounded-2xl border border-gray-100 bg-gray-50 p-3">
                {brands.map((brand) => (
                  <div
                    key={brand}
                    className="flex h-10 min-w-24 items-center justify-center rounded-lg border border-gray-200 bg-white px-3 text-sm font-semibold text-gray-700"
                  >
                    {brand}
                  </div>
                ))}
              </div>
            </div>

            {/* Sắp xếp sản phẩm */}
            <div className="flex flex-wrap items-center gap-3 rounded-2xl border border-gray-200 bg-white px-4 py-3 shadow-sm">
              <span className="text-sm font-semibold text-gray-900">Sắp xếp theo</span>
              <div className="flex flex-wrap gap-3">
                {sortOptions.map((item, index) => (
                  <button
                    key={item}
                    className={`rounded-full border px-4 py-2 text-sm font-medium transition-colors ${
                      index === 0
                        ? "border-emerald-600 bg-emerald-50 text-emerald-700"
                        : "border-gray-300 bg-white text-gray-600 hover:border-emerald-500 hover:bg-emerald-50"
                    }`}
                  >
                    {item}
                  </button>
                ))}
              </div>
            </div>

            {/* Grid kết quả sản phẩm */}
            <ProductGrid products={products} />
          </section>

        </div>
      </div>
    </MainLayout>
  );
}