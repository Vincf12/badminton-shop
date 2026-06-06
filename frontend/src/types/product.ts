export interface ProductSummaryDto {
  productId: number;
  categoryId: number;
  categoryName: string;
  productName: string;
  brandId?: number;
  brandName?: string | null;
  price: number;
  stock: number;
  imageUrl?: string | null;
  description?: string | null;
  createdAt: string;
  isInStock: boolean;
}

export interface ProductDetailDto extends ProductSummaryDto {
  updatedAt?: string | null;
}

export interface ProductCategoryDto {
  categoryId: number;
  categoryName: string;
}

export interface ProductCardModel {
  id: number;
  name: string;
  price: number;
  image: string;
  category: string;
  brand?: string | null;
  stock: number;
}

export interface ProductDetailModel extends ProductCardModel {
  brand?: string | null;
  description?: string | null;
  createdAt: string;
  updatedAt?: string | null;
  variants: ProductVariantModel[];
}

export interface ProductCategory {
  id: number;
  name: string;
}

export interface ProductVariantDto {
  variantId: number;
  productId: number;
  sku: string;
  weight?: string | null;
  gripSize?: string | null;
  color?: string | null;
  price: number;
  stockQuantity: number;
  imageUrl?: string | null;
}

export interface ProductVariantModel {
  id: number;
  sku: string;
  weight?: string | null;
  gripSize?: string | null;
  color?: string | null;
  price: number;
  stock: number;
  image?: string | null;
}
