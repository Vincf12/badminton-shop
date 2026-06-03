"use client";

import React from "react";
import Image from "next/image";
import Link from "next/link";
import type { ProductCardModel } from "@/types/product";

interface ProductCardProps {
  product: ProductCardModel;
  showBadge?: boolean;
  badgeText?: string;
}

function formatPrice(price: number): string {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(price);
}

export default function ProductCard({
  product,
  showBadge = true,
  badgeText,
}: ProductCardProps) {
  return (
    <Link href={`/product/${product.id}`}>
      <div className="bg-white w-full max-w-[240px] mx-auto rounded-2xl overflow-hidden shadow-md hover:shadow-2xl transition-all duration-300 hover:-translate-y-1 flex flex-col h-[420px] cursor-pointer">
        {/* Ảnh sản phẩm */}
        <div className="relative h-[260px] overflow-hidden group m-4 rounded-xl bg-gray-50">
          <Image
            src={product.image}
            alt={product.name}
            fill
            sizes="(max-width: 768px) 100vw, 20vw"
            className="object-contain p-3"
          />
          {showBadge && (
            <div className="absolute top-3 right-3 bg-emerald-600 text-white px-2.5 py-0.5 rounded-full text-xs font-semibold">
              {badgeText ?? product.category}
            </div>
          )}
        </div>

        {/* Nội dung sản phẩm */}
        <div className="p-3 flex flex-col flex-1 justify-between">
          <h3 className="text-sm font-semibold text-gray-800 line-clamp-2">
            {product.name}
          </h3>
          <p className="text-base font-bold text-emerald-600 mt-2">
            {formatPrice(product.price)}
          </p>
          <p className="mt-1 text-xs text-gray-500">
            {product.stock > 0 ? `Còn ${product.stock} sản phẩm` : "Hết hàng"}
          </p>
        </div>
      </div>
    </Link>
  );
}