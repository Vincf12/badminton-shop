import { ProductDetailClient } from "@/features/product-detail";
import type { ProductDetailModel } from "@/entities/product";

interface ProductDetailPageProps {
  product: ProductDetailModel;
}

export function ProductDetailPage({ product }: ProductDetailPageProps) {
  return <ProductDetailClient product={product} />;
}
