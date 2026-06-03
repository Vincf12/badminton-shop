import type {
  ProductCardModel,
  ProductCategory,
  ProductCategoryDto,
  ProductDetailDto,
  ProductDetailModel,
  ProductSummaryDto,
} from "@/types/product";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5211/api";
const FALLBACK_IMAGE = "/assets/images/banner-netro.png";

async function requestJson<T>(path: string): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || "Không thể tải dữ liệu sản phẩm");
  }

  return response.json() as Promise<T>;
}

function mapProductCard(product: ProductSummaryDto): ProductCardModel {
  return {
    id: product.productId,
    name: product.productName,
    price: product.price,
    image: product.imageUrl || FALLBACK_IMAGE,
    category: product.categoryName,
    brand: product.brand ?? null,
    stock: product.stock,
  };
}

export const productService = {
  async getProducts(params?: { search?: string; categoryId?: number }): Promise<ProductCardModel[]> {
    const query = new URLSearchParams();

    if (params?.search) {
      query.set("search", params.search);
    }

    if (typeof params?.categoryId === "number") {
      query.set("categoryId", String(params.categoryId));
    }

    const path = query.toString() ? `/products?${query.toString()}` : "/products";
    const products = await requestJson<ProductSummaryDto[]>(path);
    return products.map(mapProductCard);
  },

  async getProduct(id: number): Promise<ProductDetailModel> {
    const product = await requestJson<ProductDetailDto>(`/products/${id}`);
    return {
      ...mapProductCard(product),
      brand: product.brand ?? null,
      description: product.description ?? null,
      createdAt: product.createdAt,
      updatedAt: product.updatedAt ?? null,
    };
  },

  async getCategories(): Promise<ProductCategory[]> {
    const categories = await requestJson<ProductCategoryDto[]>("/products/categories");
    return categories.map((item) => ({ id: item.categoryId, name: item.categoryName }));
  },
};

export { FALLBACK_IMAGE };