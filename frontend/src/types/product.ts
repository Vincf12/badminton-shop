export interface ProductSummaryDto {
  productId: number;
  categoryId: number;
  categoryName: string;
  productName: string;
  brand?: string | null;
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
}

export interface ProductCategory {
  id: number;
  name: string;
}