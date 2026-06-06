import type {
  ProductCardModel,
  ProductCategory,
  ProductCategoryDto,
  ProductDetailDto,
  ProductDetailModel,
  ProductSummaryDto,
  ProductVariantDto,
  ProductVariantModel,
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
    const message = await readErrorMessage(response);
    throw new Error(message || "Không thể tải dữ liệu sản phẩm");
  }

  return response.json() as Promise<T>;
}

async function readErrorMessage(response: Response): Promise<string> {
  const contentType = response.headers.get("content-type") ?? "";

  if (contentType.includes("application/json")) {
    const errorData = await response.json().catch(() => null);
    return errorData?.message || errorData?.detail || JSON.stringify(errorData ?? {});
  }

  return response.text();
}

function normalizeImage(imageUrl?: string | null): string {
  if (!imageUrl) {
    return FALLBACK_IMAGE;
  }

  if (imageUrl.startsWith("http://") || imageUrl.startsWith("https://") || imageUrl.startsWith("/")) {
    return imageUrl;
  }

  return `${API_BASE_URL.replace(/\/api$/, "")}/${imageUrl.replace(/^\/+/, "")}`;
}

function mapProductCard(product: ProductSummaryDto): ProductCardModel {
  return {
    id: product.productId,
    name: product.productName,
    price: product.price,
    image: normalizeImage(product.imageUrl),
    category: product.categoryName,
    brand: product.brandName ?? null,
    stock: product.stock,
  };
}

function mapVariant(variant: ProductVariantDto): ProductVariantModel {
  return {
    id: variant.variantId,
    sku: variant.sku,
    weight: variant.weight ?? null,
    gripSize: variant.gripSize ?? null,
    color: variant.color ?? null,
    price: variant.price,
    stock: variant.stockQuantity,
    image: variant.imageUrl ? normalizeImage(variant.imageUrl) : null,
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
    const [product, variants] = await Promise.all([
      requestJson<ProductDetailDto>(`/products/${id}`),
      requestJson<ProductVariantDto[]>(`/products/${id}/variants`).catch(() => []),
    ]);

    return {
      ...mapProductCard(product),
      brand: product.brandName ?? null,
      description: product.description ?? null,
      createdAt: product.createdAt,
      updatedAt: product.updatedAt ?? null,
      variants: variants.map(mapVariant),
    };
  },

  async getCategories(): Promise<ProductCategory[]> {
    const categories = await requestJson<ProductCategoryDto[]>("/categories");
    return categories.map((item) => ({ id: item.categoryId, name: item.categoryName }));
  },
};

export const fetchProducts = productService.getProducts;
export const fetchProduct = productService.getProduct;
export const fetchCategories = productService.getCategories;
export { API_BASE_URL, FALLBACK_IMAGE };
