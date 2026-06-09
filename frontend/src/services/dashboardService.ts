import { authService } from "./authService";
import { API_BASE_URL } from "./productService";

export interface DashboardOverview {
  totalRevenue: number;
  pendingRevenue: number;
  totalOrders: number;
  totalUsers: number;
  totalProducts: number;
  lowStockProducts: number;
}

export interface RevenuePoint {
  period: string;
  revenue: number;
  orderCount: number;
}

export interface OrderStatusStat {
  status: string;
  count: number;
  totalAmount: number;
}

export interface TopProductStat {
  productId: number;
  productName: string;
  soldQuantity: number;
  revenue: number;
}

export interface LowStockProduct {
  productId: number;
  productName: string;
  variantId: number;
  sku: string;
  weight?: string | null;
  gripSize?: string | null;
  color?: string | null;
  stockQuantity: number;
}

export interface NewUserStat {
  userId: number;
  email: string;
  fullName: string;
  phone?: string | null;
  role?: string | null;
  isActive: boolean;
  createdAt: string;
}

export interface RecentOrder {
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

export interface DashboardData {
  overview: DashboardOverview;
  revenue: RevenuePoint[];
  orders: OrderStatusStat[];
  topProducts: TopProductStat[];
  lowStock: LowStockProduct[];
  newUsers: NewUserStat[];
  recentOrders: RecentOrder[];
}

async function readErrorMessage(response: Response, fallback: string): Promise<string> {
  const contentType = response.headers.get("content-type") ?? "";

  try {
    if (contentType.includes("application/json")) {
      const errorData = await response.json();
      return errorData?.message || errorData?.detail || JSON.stringify(errorData) || fallback;
    }

    return (await response.text()) || fallback;
  } catch {
    return fallback;
  }
}

function getAuthHeaders(): HeadersInit {
  const token = authService.getToken();

  if (!token) {
    throw new Error("Vui lòng đăng nhập bằng tài khoản admin hoặc staff.");
  }

  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
}

async function requestJson<T>(path: string): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: getAuthHeaders(),
    cache: "no-store",
  });

  if (!response.ok) {
    if (response.status === 401 || response.status === 403) {
      throw new Error("Tài khoản hiện tại không có quyền xem dashboard quản trị.");
    }

    throw new Error(await readErrorMessage(response, "Không thể tải dữ liệu dashboard."));
  }

  return response.json() as Promise<T>;
}

export const dashboardService = {
  async getDashboardData(): Promise<DashboardData> {
    const [overview, revenue, orders, topProducts, lowStock, newUsers, recentOrders] = await Promise.all([
      requestJson<DashboardOverview>("/dashboard/overview"),
      requestJson<RevenuePoint[]>("/dashboard/revenue?type=month"),
      requestJson<OrderStatusStat[]>("/dashboard/orders"),
      requestJson<TopProductStat[]>("/dashboard/top-products?limit=8"),
      requestJson<LowStockProduct[]>("/dashboard/low-stock?threshold=5"),
      requestJson<NewUserStat[]>("/dashboard/new-users?limit=8"),
      requestJson<RecentOrder[]>("/orders"),
    ]);

    return {
      overview,
      revenue,
      orders,
      topProducts,
      lowStock,
      newUsers,
      recentOrders: recentOrders.slice(0, 8),
    };
  },
};
