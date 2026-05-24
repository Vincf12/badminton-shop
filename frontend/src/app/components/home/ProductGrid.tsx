"use client";

import ProductCard from "./ProductCard";

interface Product {
  id: number;
  name: string;
  price: string;
  image: string;
}

interface ProductGridProps {
  products: Product[];
  showBadge?: boolean;
}

export default function ProductGrid({
  products,
  showBadge = true,
}: ProductGridProps) {
  return (
    <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6 mb-10">
      {products.map((product) => (
        <ProductCard key={product.id} product={product} showBadge={showBadge} />
      ))}
    </div>
  );
}