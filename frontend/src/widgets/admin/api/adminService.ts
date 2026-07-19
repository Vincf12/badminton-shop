import { API_BASE_URL } from "@/shared/api/config";
import { getAccessToken } from "@/shared/api/authSession";

export type AdminSection =
  | "products"
  | "categories"
  | "brands"
  | "coupons"
  | "orders"
  | "customers"
  | "banners";

export interface ProductAdminItem {
  productId: number;
  categoryId: number;
  categoryName: string;
  productName: string;
  brandId: number;
  brandName: string;
  price: number;
  stock: number;
  imageUrl?: string | null;
  description?: string | null;
  createdAt: string;
}

export interface ProductPayload {
  categoryId: number;
  brandId: number;
  productName: string;
  slug?: string | null;
  shortDescription?: string | null;
  description?: string | null;
  status: string;
  imageUrl?: string | null;
  price: number;
  stock: number;
}

export interface CategoryAdminItem {
  categoryId: number;
  categoryName: string;
}

export interface BrandAdminItem {
  brandId: number;
  brandName: string;
}

export interface CouponAdminItem {
  couponId: number;
  code: string;
  couponName: string;
  discountType: string;
  discountValue: number;
  minimumOrderAmount: number;
  maximumDiscountAmount?: number | null;
  usageLimit?: number | null;
  usedCount: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export type CouponPayload = Omit<CouponAdminItem, "couponId" | "usedCount">;

export interface OrderAdminItem {
  orderId: number;
  orderCode: string;
  userId: number;
  totalAmount: number;
  shippingFee: number;
  discountAmount: number;
  finalAmount: number;
  status: string;
  createdAt: string;
}

export interface UserAdminItem {
  userId: number;
  email: string;
  fullName: string;
  phone?: string | null;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export interface BannerAdminItem {
  bannerId: number;
  title: string;
  imageUrl: string;
  targetType: string;
  targetId?: string | null;
  customUrl?: string | null;
  position: string;
  displayOrder: number;
  isActive: boolean;
  startDate: string;
  endDate?: string | null;
  createdAt: string;
  updatedAt?: string | null;
  version: number;
}

export type BannerPayload = Omit<
  BannerAdminItem,
  "bannerId" | "createdAt" | "updatedAt" | "version"
>;

export interface ProductVariantAdminItem {
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

export interface ShipmentAdminPayload {
  orderId: number;
  carrier: string;
  trackingNumber?: string | null;
  status: string;
  shippedAt?: string | null;
  deliveredAt?: string | null;
}

async function readErrorMessage(response: Response, fallback: string): Promise<string> {
  const contentType = response.headers.get("content-type") ?? "";

  try {
    if (contentType.includes("application/json")) {
      const errorData = await response.json();

      if (errorData?.errors && typeof errorData.errors === "object") {
        return Object.values(errorData.errors).flat().filter(Boolean).join("; ") || fallback;
      }

      return errorData?.message || errorData?.detail || JSON.stringify(errorData) || fallback;
    }

    return (await response.text()) || fallback;
  } catch {
    return fallback;
  }
}

function getAuthHeaders(): HeadersInit {
  const token = getAccessToken();

  if (!token) {
    throw new Error("Vui lòng đăng nhập bằng tài khoản quản trị.");
  }

  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
}

async function requestJson<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      ...getAuthHeaders(),
      ...(init?.headers ?? {}),
    },
    cache: "no-store",
  });

  if (!response.ok) {
    if (response.status === 401 || response.status === 403) {
      throw new Error("Tài khoản hiện tại không có quyền thực hiện thao tác này.");
    }

    throw new Error(await readErrorMessage(response, "Không thể kết nối API quản trị."));
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

function jsonBody(value: unknown): RequestInit {
  return {
    method: "POST",
    body: JSON.stringify(value),
  };
}

export const adminService = {
  getProducts: (search?: string) => {
    const query = search ? `?search=${encodeURIComponent(search)}` : "";
    return requestJson<ProductAdminItem[]>(`/products${query}`);
  },
  createProduct: (payload: ProductPayload) => requestJson<ProductAdminItem>("/products", jsonBody(payload)),
  updateProduct: (id: number, payload: ProductPayload) =>
    requestJson<{ message: string }>(`/products/${id}`, { ...jsonBody(payload), method: "PUT" }),
  deleteProduct: (id: number) => requestJson<void>(`/products/${id}`, { method: "DELETE" }),
  getProductVariants: (productId: number) => requestJson<ProductVariantAdminItem[]>(`/products/${productId}/variants`),
  updateVariantStock: (variantId: number, stockQuantity: number) =>
    requestJson<{ message: string }>(`/product-variants/${variantId}/stock`, {
      method: "PUT",
      body: JSON.stringify({ stockQuantity }),
    }),

  getCategories: () => requestJson<CategoryAdminItem[]>("/categories"),
  createCategory: (categoryName: string) => requestJson<CategoryAdminItem>("/categories", jsonBody({ categoryName })),
  updateCategory: (id: number, categoryName: string) =>
    requestJson<CategoryAdminItem>(`/categories/${id}`, { ...jsonBody({ categoryName }), method: "PUT" }),
  deleteCategory: (id: number) => requestJson<void>(`/categories/${id}`, { method: "DELETE" }),

  getBrands: () => requestJson<BrandAdminItem[]>("/brands"),
  createBrand: (brandName: string) => requestJson<BrandAdminItem>("/brands", jsonBody({ brandName })),
  updateBrand: (id: number, brandName: string) =>
    requestJson<BrandAdminItem>(`/brands/${id}`, { ...jsonBody({ brandName }), method: "PUT" }),
  deleteBrand: (id: number) => requestJson<void>(`/brands/${id}`, { method: "DELETE" }),

  getCoupons: () => requestJson<CouponAdminItem[]>("/coupons"),
  createCoupon: (payload: CouponPayload) => requestJson<CouponAdminItem>("/coupons", jsonBody(payload)),
  updateCoupon: (id: number, payload: CouponPayload) =>
    requestJson<CouponAdminItem>(`/coupons/${id}`, { ...jsonBody(payload), method: "PUT" }),
  deleteCoupon: (id: number) => requestJson<void>(`/coupons/${id}`, { method: "DELETE" }),

  getOrders: () => requestJson<OrderAdminItem[]>("/orders"),
  updateOrderStatus: (id: number, status: string, note?: string) =>
    requestJson<{ message: string }>(`/orders/${id}/status`, {
      method: "PUT",
      body: JSON.stringify({ status, note }),
    }),
  cancelOrder: (id: number) => requestJson<{ message: string }>(`/orders/${id}/cancel`, { method: "PUT" }),

  getUsers: () => requestJson<UserAdminItem[]>("/User"),
  updateUserRole: (id: number, role: string) =>
    requestJson<{ message: string }>(`/User/${id}/role`, {
      method: "PUT",
      body: JSON.stringify({ role }),
    }),
  updateUserStatus: (id: number, isActive: boolean) =>
    requestJson<{ message: string }>(`/User/${id}/status`, {
      method: "PUT",
      body: JSON.stringify({ isActive }),
    }),
  deleteUser: (id: number) => requestJson<void>(`/User/${id}`, { method: "DELETE" }),

  getBanners: () => requestJson<BannerAdminItem[]>("/banners"),
  createBanner: (payload: BannerPayload) => requestJson<BannerAdminItem>("/banners", jsonBody(payload)),
  updateBanner: (id: number, payload: BannerPayload) =>
    requestJson<BannerAdminItem>(`/banners/${id}`, { ...jsonBody(payload), method: "PUT" }),
  updateBannerStatus: (id: number, isActive: boolean) =>
    requestJson<{ message: string }>(`/banners/${id}/status`, {
      method: "PUT",
      body: JSON.stringify({ isActive }),
    }),
  deleteBanner: (id: number) => requestJson<void>(`/banners/${id}`, { method: "DELETE" }),

  getOrderPayment: (orderId: number) => requestJson<unknown>(`/payments/order/${orderId}`),
  updatePaymentStatus: (paymentId: number, paymentStatus: string) =>
    requestJson<{ message: string }>(`/payments/${paymentId}/status`, {
      method: "PUT",
      body: JSON.stringify({ paymentStatus }),
    }),
  getOrderShipment: (orderId: number) => requestJson<unknown>(`/shipments/order/${orderId}`),
  createShipment: (payload: ShipmentAdminPayload) => requestJson<unknown>("/shipments", jsonBody(payload)),
  updateShipmentStatus: (shipmentId: number, status: string) =>
    requestJson<{ message: string }>(`/shipments/${shipmentId}/status`, {
      method: "PUT",
      body: JSON.stringify({ status }),
    }),
};
