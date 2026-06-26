export {
  API_BASE_URL,
  FALLBACK_IMAGE,
  fetchCategories,
  fetchProduct,
  fetchProducts,
  productService,
} from "./api/productService";
export { default as ProductCard } from "./ui/ProductCard";
export { default as ProductGrid } from "./ui/ProductGrid";
export type {
  ProductCardModel,
  ProductCategory,
  ProductCategoryDto,
  ProductDetailDto,
  ProductDetailModel,
  ProductSummaryDto,
  ProductVariantDto,
  ProductVariantModel,
} from "./model/types";
