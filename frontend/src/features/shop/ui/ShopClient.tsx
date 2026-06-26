"use client";

import { useMemo, useState } from "react";
import { Search, SlidersHorizontal } from "lucide-react";
import { ProductGrid } from "@/entities/product";
import { Badge, Button, Card, FilterGroup, TextInput } from "@/shared/ui";
import type { ProductCardModel, ProductCategory } from "@/entities/product";

interface ShopClientProps {
  products: ProductCardModel[];
  categories: ProductCategory[];
  error: string | null;
}

const priceFilters = ["Dưới 500k", "500k đến 1 triệu", "1 triệu đến 3 triệu", "Trên 3 triệu"];
const sizeFilters = ["4U", "5U", "6U", "7U", "8U"];
const racketLengths = ["665mm", "670mm", "675mm"];
const colors = ["Trắng", "Xanh", "Đen", "Hồng", "Vàng"];
const shoeSizes = ["37", "38", "39", "40", "41", "42"];
const sortOptions = [
  "Khuyến mãi tốt",
  "Sản phẩm mới",
  "Bán chạy",
  "Giá tăng dần",
  "Giá giảm dần",
];

export default function ShopClient({ products, categories, error }: ShopClientProps) {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);

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

  const brandNames = Array.from(
    new Set(products.map((product) => product.brand).filter(Boolean) as string[])
  );

  return (
    <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:py-12">
      <div className="mb-8 max-w-3xl">
        <p className="text-sm font-bold uppercase tracking-[0.18em] text-emerald-700">FlyShot Shop</p>
        <h1 className="mt-3 text-4xl font-black tracking-[-0.05em] text-slate-950 sm:text-5xl">
          Chọn trang bị theo cách bạn chơi.
        </h1>
        <p className="mt-4 text-base leading-7 text-slate-600">
          Lọc nhanh vợt, giày và phụ kiện theo nhu cầu thực tế trên sân.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-[280px_minmax(0,1fr)]">
        <aside className="space-y-4 lg:sticky lg:top-24 lg:self-start">
          <Card className="p-4">
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-base font-black text-slate-950">Bộ lọc</h2>
              <SlidersHorizontal className="h-4 w-4 text-slate-500" strokeWidth={1.8} />
            </div>

            <TextInput
              type="text"
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
              placeholder="Tìm nhanh"
              leftIcon={<Search className="h-4 w-4" strokeWidth={1.8} />}
            />

            <div className="mt-5 space-y-1">
              <FilterOptions title="Khoảng giá" options={priceFilters} />
              <FilterOptions title="Kích thước vợt" options={sizeFilters} />
              <FilterOptions title="Chiều dài vợt" options={racketLengths} />
              <FilterOptions title="Màu sắc" options={colors} />
              <FilterOptions title="Size giày" options={shoeSizes} />
            </div>
          </Card>
        </aside>

        <section className="space-y-6">
          <div className="rounded-2xl border border-slate-200 bg-white p-4">
            <div className="flex flex-wrap gap-2">
              <Button
                onClick={() => setSelectedCategoryId(null)}
                variant={selectedCategoryId === null ? "primary" : "outline"}
              >
                Tất cả
              </Button>

              {categories.map((item) => (
                <Button
                  key={item.id}
                  onClick={() => setSelectedCategoryId(item.id)}
                  variant={selectedCategoryId === item.id ? "primary" : "outline"}
                >
                  {item.name}
                </Button>
              ))}
            </div>

            <div className="mt-4 flex flex-wrap items-center gap-2 border-t border-slate-100 pt-4">
              {brandNames.length > 0 ? (
                brandNames.map((brand) => (
                  <Badge key={brand} variant="neutral" className="h-9 justify-center text-xs">
                    {brand}
                  </Badge>
                ))
              ) : (
                <span className="text-sm text-slate-500">Chưa có thương hiệu để hiển thị.</span>
              )}
            </div>
          </div>

          <div className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white px-4 py-3 sm:flex-row sm:items-center sm:justify-between">
            <p className="text-sm font-semibold text-slate-600">
              Hiển thị <span className="font-black text-slate-950">{visibleProducts.length}</span> sản phẩm
            </p>
            <div className="flex flex-wrap gap-2">
              {sortOptions.map((item, index) => (
                <Button
                  key={item}
                  variant={index === 0 ? "soft" : "ghost"}
                  size="sm"
                >
                  {item}
                </Button>
              ))}
            </div>
          </div>

          {error ? (
            <Card variant="danger" className="p-8 text-center text-sm font-semibold">
              {error}
            </Card>
          ) : (
            <ProductGrid products={visibleProducts} />
          )}
        </section>
      </div>
    </div>
  );
}

function FilterOptions({ title, options }: { title: string; options: string[] }) {
  return (
    <FilterGroup title={title}>
      {options.map((item) => (
        <label key={item} className="flex cursor-pointer items-center gap-2 text-slate-600 transition hover:text-emerald-700">
          <input
            type="checkbox"
            className="h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500"
          />
          <span>{item}</span>
        </label>
      ))}
    </FilterGroup>
  );
}
