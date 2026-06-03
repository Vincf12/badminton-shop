"use client";

import React, { useEffect, useMemo, useState } from "react";
import MainLayout from "@/app/components/layout/MainLayout";
import ProductGrid from "@/app/components/home/ProductGrid";
import FilterGroup from "@/app/components/product/FilterGroup";
import { Search, SlidersHorizontal } from "lucide-react";
import { fetchCategories, fetchProducts } from "@/services/productService";
import type { ProductCardModel, ProductCategory } from "@/types/product";

export default function ShopPage() {
  const [products, setProducts] = useState<ProductCardModel[]>([]);
  const [categories, setCategories] = useState<ProductCategory[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    const loadData = async () => {
      try {
        const [productData, categoryData] = await Promise.all([
          fetchProducts(),
          fetchCategories(),
        ]);

        if (isActive) {
          setProducts(productData);
          setCategories(categoryData);
        }
      } catch (loadError) {
        if (isActive) {
          setError(loadError instanceof Error ? loadError.message : "Không thể tải dữ liệu sản phẩm");
        }
      } finally {
        if (isActive) {
          setLoading(false);
        }
      }
    };

    loadData();

    return () => {
      isActive = false;
    };
  }, []);

  const visibleProducts = useMemo(() => {
    const normalizedSearch = searchTerm.trim().toLowerCase();

    return products.filter((product) => {
      const matchesSearch =
        !normalizedSearch ||
        product.name.toLowerCase().includes(normalizedSearch) ||
        product.category.toLowerCase().includes(normalizedSearch) ||
        (product.brand ? product.brand.toLowerCase().includes(normalizedSearch) : false);

      const matchesCategory =
        selectedCategoryId === null ||
        categories.find((category) => category.id === selectedCategoryId)?.name === product.category;

      return matchesSearch && matchesCategory;
    });
  }, [categories, products, searchTerm, selectedCategoryId]);

  const priceFilters = [
    { label: "Dưới 500k", value: "under-500k" },
    { label: "500k - 1 triệu", value: "500k-1m" },
    { label: "1 triệu - 3 triệu", value: "1m-3m" },
    { label: "Trên 3 triệu", value: "above-3m" },
  ];

  const sizeFilters = ["4U", "5U", "6U", "7U", "8U"];
  const racketLengths = ["665mm", "670mm", "675mm"];
  const colors = ["Trắng", "Xanh", "Đen", "Hồng", "Vàng"];
  const shoeSizes = ["37", "38", "39", "40", "41", "42"];

  const brandNames = Array.from(
    new Set(products.map((product) => product.brand).filter(Boolean) as string[])
  );

  return (
    <MainLayout>
      <div className="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 lg:py-8">
        <div className="mb-6">
          <h1 className="text-3xl lg:text-4xl font-bold text-gray-900 mb-2">Sản Phẩm</h1>
          <p className="text-gray-600">
            Khám phá bộ sưu tập cầu lông được đồng bộ trực tiếp từ database.
          </p>
        </div>

        <div className="grid grid-cols-1 gap-6 lg:grid-cols-[280px_minmax(0,1fr)]">
          <aside className="space-y-4 lg:top-28 lg:self-start">
            <div className="rounded-2xl border border-gray-200 bg-white p-4 shadow-sm">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-semibold text-gray-900">Bộ lọc</h2>
                <SlidersHorizontal className="h-4 w-4 text-gray-500" />
              </div>

              <div className="flex items-center gap-2 rounded-xl border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-400 focus-within:border-emerald-500 focus-within:bg-white transition-colors">
                <Search className="h-4 w-4 shrink-0" />
                <input
                  type="text"
                  value={searchTerm}
                  onChange={(event) => setSearchTerm(event.target.value)}
                  placeholder="Tìm nhanh..."
                  className="w-full bg-transparent text-gray-700 outline-none placeholder-gray-400 text-sm"
                />
              </div>

              <div className="mt-4 space-y-1">
                <FilterGroup title="Khoảng giá">
                  {priceFilters.map((price) => (
                    <label key={price.value} className="flex items-center gap-2 cursor-pointer hover:text-emerald-600 transition-colors">
                      <input
                        type="checkbox"
                        className="h-4 w-4 rounded border-gray-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer"
                      />
                      <span>{price.label}</span>
                    </label>
                  ))}
                </FilterGroup>

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

                <FilterGroup title="Chiều dài vợt">
                  {racketLengths.map((item) => (
                    <label key={item} className="flex items-center gap-2 cursor-pointer hover:text-emerald-600 transition-colors">
                      <input
                        type="checkbox"
                        className="h-4 w-4 rounded border-gray-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer"
                      />
                      <span>{item}</span>
                    </label>
                  ))}
                </FilterGroup>

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

          <section className="space-y-5">
            <div className="rounded-2xl border border-gray-200 bg-white p-4 shadow-sm">
              <div className="flex flex-wrap gap-3">
                <button
                  onClick={() => setSelectedCategoryId(null)}
                  className={`rounded-xl border px-4 py-2 text-sm font-medium transition-colors ${
                    selectedCategoryId === null
                      ? "border-emerald-600 bg-emerald-600 text-white"
                      : "border-gray-300 bg-white text-gray-800 hover:border-emerald-500 hover:text-emerald-700"
                  }`}
                >
                  Tất cả
                </button>

                {categories.map((item) => (
                  <button
                    key={item.id}
                    onClick={() => setSelectedCategoryId(item.id)}
                    className={`rounded-xl border px-4 py-2 text-sm font-medium transition-colors ${
                      selectedCategoryId === item.id
                        ? "border-emerald-600 bg-emerald-600 text-white"
                        : "border-gray-300 bg-white text-gray-800 hover:border-emerald-500 hover:text-emerald-700"
                    }`}
                  >
                    {item.name}
                  </button>
                ))}
              </div>

              <div className="mt-4 flex flex-wrap items-center gap-3 overflow-hidden rounded-2xl border border-gray-100 bg-gray-50 p-3">
                {brandNames.length > 0 ? (
                  brandNames.map((brand) => (
                    <div
                      key={brand}
                      className="flex h-10 min-w-24 items-center justify-center rounded-lg border border-gray-200 bg-white px-3 text-sm font-semibold text-gray-700"
                    >
                      {brand}
                    </div>
                  ))
                ) : (
                  <span className="text-sm text-gray-500">Chưa có thương hiệu để hiển thị.</span>
                )}
              </div>
            </div>

            <div className="flex flex-wrap items-center gap-3 rounded-2xl border border-gray-200 bg-white px-4 py-3 shadow-sm">
              <span className="text-sm font-semibold text-gray-900">Sắp xếp theo</span>
              <div className="flex flex-wrap gap-3">
                {[
                  "Khuyến mãi tốt",
                  "Sản phẩm mới nhất",
                  "Bán chạy nhất",
                  "Giá tăng dần",
                  "Giá giảm dần",
                ].map((item, index) => (
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

            {loading ? (
              <p className="rounded-2xl border border-dashed border-gray-300 bg-white p-8 text-center text-gray-600">
                Đang tải sản phẩm...
              </p>
            ) : error ? (
              <p className="rounded-2xl border border-red-200 bg-red-50 p-8 text-center text-red-700">
                {error}
              </p>
            ) : (
              <>
                <p className="text-sm text-gray-600">Hiển thị {visibleProducts.length} sản phẩm</p>
                <ProductGrid products={visibleProducts} />
              </>
            )}
          </section>
        </div>
      </div>
    </MainLayout>
  );
}
