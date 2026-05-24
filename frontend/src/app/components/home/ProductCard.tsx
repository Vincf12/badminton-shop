"use client";

import React from "react";
import Image from "next/image";
import Link from "next/link";

interface Product {
  id: number;
  name: string;
  price: string;
  image: string;
}

interface ProductCardProps {
  product: Product;
  showBadge?: boolean;
  badgeText?: string;
}

export default function ProductCard({
  product,
  showBadge = true,
  badgeText = "Mới",
}: ProductCardProps) {
  return (
    <Link href={`/product/${product.id}`}>
      <div className="bg-white w-full max-w-[240px] mx-auto rounded-2xl overflow-hidden shadow-md hover:shadow-2xl transition-all duration-300 hover:-translate-y-1 flex flex-col h-[420px] cursor-pointer">
        {/* Ảnh sản phẩm */}
        <div className="relative h-full overflow-hidden group m-4">
          <Image
            src={product.image}
            alt={product.name}
            fill
            sizes="(max-width: 768px) 100vw, 20vw"
            className="object-cover"
          />
          {showBadge && (
            <div className="absolute top-3 right-3 bg-emerald-600 text-white px-2.5 py-0.5 rounded-full text-xs font-semibold">
              {badgeText}
            </div>
          )}
        </div>

        {/* Nội dung sản phẩm */}
        <div className="p-3 flex flex-col flex-1 justify-between">
          <h3 className="text-sm font-semibold text-gray-800 line-clamp-2">
            {product.name}
          </h3>
          <p className="text-base font-bold text-emerald-600 mt-2">
            {product.price}
          </p>
        </div>
      </div>
    </Link>
  );
}