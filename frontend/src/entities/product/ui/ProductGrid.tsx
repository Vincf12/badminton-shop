import type { ProductCardModel } from "@/entities/product";
import { ProductCard } from "@/shared/ui";

interface ProductGridProps {
  products: ProductCardModel[];
  showBadge?: boolean;
}

export default function ProductGrid({
  products,
  showBadge = true,
}: ProductGridProps) {
  if (products.length === 0) {
    return (
      <div className="rounded-2xl border border-dashed border-slate-300 bg-white px-6 py-12 text-center">
        <p className="text-base font-bold text-slate-950">Chưa có sản phẩm phù hợp</p>
        <p className="mt-2 text-sm text-slate-500">Hãy thử đổi bộ lọc hoặc quay lại danh mục chính.</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-2 gap-4 md:grid-cols-3 lg:grid-cols-4">
      {products.map((product) => (
        <ProductCard
          key={product.id}
          id={product.id}
          name={product.name}
          image={product.image}
          price={product.price}
          stock={product.stock}
          brand={product.brand ?? undefined}
          category={product.category}
          showBadge={showBadge}
        />
      ))}
    </div>
  );
}
