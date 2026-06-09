"use client";

import React, { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import {
  BadgePercent,
  BarChart3,
  Bell,
  Boxes,
  Building2,
  ChevronRight,
  ClipboardList,
  Gauge,
  LayoutDashboard,
  LogOut,
  Menu,
  Moon,
  Package,
  PackagePlus,
  Search,
  Settings,
  ShoppingBag,
  Sun,
  Tags,
  TicketPercent,
  Truck,
  UserPlus,
  Users,
  X,
} from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";
import {
  dashboardService,
  type DashboardData,
  type LowStockProduct,
  type OrderStatusStat,
  type RecentOrder,
  type RevenuePoint,
} from "@/services/dashboardService";

const statusConfig: Record<string, { label: string; badge: string; dot: string }> = {
  pending: {
    label: "Pending",
    badge: "bg-yellow-50 text-yellow-700 ring-yellow-200 dark:bg-yellow-500/10 dark:text-yellow-300 dark:ring-yellow-500/20",
    dot: "bg-yellow-400",
  },
  confirmed: {
    label: "Confirmed",
    badge: "bg-blue-50 text-blue-700 ring-blue-200 dark:bg-blue-500/10 dark:text-blue-300 dark:ring-blue-500/20",
    dot: "bg-blue-500",
  },
  shipping: {
    label: "Shipping",
    badge: "bg-purple-50 text-purple-700 ring-purple-200 dark:bg-purple-500/10 dark:text-purple-300 dark:ring-purple-500/20",
    dot: "bg-purple-500",
  },
  delivered: {
    label: "Delivered",
    badge: "bg-emerald-50 text-emerald-700 ring-emerald-200 dark:bg-emerald-500/10 dark:text-emerald-300 dark:ring-emerald-500/20",
    dot: "bg-emerald-500",
  },
  completed: {
    label: "Delivered",
    badge: "bg-emerald-50 text-emerald-700 ring-emerald-200 dark:bg-emerald-500/10 dark:text-emerald-300 dark:ring-emerald-500/20",
    dot: "bg-emerald-500",
  },
  cancelled: {
    label: "Cancelled",
    badge: "bg-red-50 text-red-700 ring-red-200 dark:bg-red-500/10 dark:text-red-300 dark:ring-red-500/20",
    dot: "bg-red-500",
  },
};

const navItems = [
  { label: "Dashboard", icon: LayoutDashboard, href: "/dashboard", active: true },
  { label: "Sản phẩm", icon: Package, href: "/admin/products" },
  { label: "Danh mục", icon: Tags, href: "/admin/categories" },
  { label: "Thương hiệu", icon: Building2, href: "/admin/brands" },
  { label: "Biến thể sản phẩm", icon: Boxes, href: "/admin/variants" },
  { label: "Đơn hàng", icon: ShoppingBag, href: "/admin/orders" },
  { label: "Khách hàng", icon: Users, href: "/admin/customers" },
  { label: "Coupon", icon: TicketPercent, href: "/admin/coupons" },
  { label: "Thống kê", icon: BarChart3, href: "/admin/statistics" },
  { label: "Cài đặt", icon: Settings, href: "/admin/settings" },
];

function formatCurrency(value: number): string {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    maximumFractionDigits: 0,
  }).format(value);
}

function formatNumber(value: number): string {
  return new Intl.NumberFormat("vi-VN").format(value);
}

function formatDate(value: string): string {
  return new Intl.DateTimeFormat("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  }).format(new Date(value));
}

function normalizeStatus(status: string): keyof typeof statusConfig {
  if (status === "completed") {
    return "delivered";
  }

  return status in statusConfig ? status : "pending";
}

function AdminSidebar({
  open,
  onClose,
  onLogout,
}: {
  open: boolean;
  onClose: () => void;
  onLogout: () => void;
}) {
  return (
    <>
      <div
        className={`fixed inset-0 z-40 bg-slate-950/50 transition-opacity lg:hidden ${
          open ? "opacity-100" : "pointer-events-none opacity-0"
        }`}
        onClick={onClose}
      />
      <aside
        className={`fixed inset-y-0 left-0 z-50 flex w-72 flex-col bg-[#0F172A] text-slate-200 shadow-2xl transition-transform duration-300 lg:sticky lg:top-0 lg:h-screen lg:translate-x-0 ${
          open ? "translate-x-0" : "-translate-x-full"
        }`}
      >
        <div className="flex h-20 items-center justify-between border-b border-white/10 px-6">
          <Link href="/dashboard" className="flex items-center gap-3">
            <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-emerald-500 text-white shadow-lg shadow-emerald-500/30">
              <Gauge className="h-6 w-6" />
            </div>
            <div>
              <p className="text-lg font-black tracking-tight text-white">FlyShot Admin</p>
              <p className="text-xs font-medium text-slate-400">Seller Operations</p>
            </div>
          </Link>
          <button
            onClick={onClose}
            className="rounded-xl p-2 text-slate-400 transition hover:bg-white/10 hover:text-white lg:hidden"
            aria-label="Đóng menu"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto px-4 py-5">
          {navItems.map((item) => {
            const Icon = item.icon;
            return (
              <Link
                key={item.label}
                href={item.href}
                className={`group flex h-12 items-center gap-3 rounded-2xl px-4 text-sm font-semibold transition-all ${
                  item.active
                    ? "bg-emerald-500 text-white shadow-lg shadow-emerald-500/20"
                    : "text-slate-300 hover:bg-white/10 hover:text-white"
                }`}
              >
                <Icon className="h-5 w-5 shrink-0" />
                <span className="truncate">{item.label}</span>
              </Link>
            );
          })}
        </nav>

        <div className="border-t border-white/10 p-4">
          <button
            onClick={onLogout}
            className="flex h-12 w-full items-center gap-3 rounded-2xl px-4 text-sm font-semibold text-slate-300 transition hover:bg-red-500/10 hover:text-red-300"
          >
            <LogOut className="h-5 w-5" />
            Đăng xuất
          </button>
        </div>
      </aside>
    </>
  );
}

function AdminHeader({
  adminName,
  isDark,
  onToggleDark,
  onOpenSidebar,
}: {
  adminName: string;
  isDark: boolean;
  onToggleDark: () => void;
  onOpenSidebar: () => void;
}) {
  return (
    <header className="sticky top-0 z-30 border-b border-slate-200/80 bg-white/85 backdrop-blur-xl dark:border-slate-800 dark:bg-slate-950/80">
      <div className="flex min-h-20 flex-col gap-4 px-4 py-4 sm:px-6 lg:px-8 xl:flex-row xl:items-center xl:justify-between">
        <div className="flex items-center gap-4">
          <button
            onClick={onOpenSidebar}
            className="rounded-2xl border border-slate-200 bg-white p-3 text-slate-700 shadow-sm transition hover:border-emerald-300 hover:text-emerald-600 dark:border-slate-800 dark:bg-slate-900 dark:text-slate-200 lg:hidden"
            aria-label="Mở menu"
          >
            <Menu className="h-5 w-5" />
          </button>
          <div>
            <div className="flex items-center gap-2 text-xs font-semibold text-slate-500 dark:text-slate-400">
              <span>Admin</span>
              <ChevronRight className="h-3.5 w-3.5" />
              <span className="text-emerald-600 dark:text-emerald-400">Dashboard</span>
            </div>
            <h1 className="mt-1 text-2xl font-black tracking-tight text-slate-950 dark:text-white sm:text-3xl">
              Bảng điều khiển bán hàng
            </h1>
          </div>
        </div>

        <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
          <label className="flex h-12 min-w-0 items-center gap-3 rounded-2xl border border-slate-200 bg-slate-50 px-4 text-sm shadow-sm transition focus-within:border-emerald-400 focus-within:bg-white dark:border-slate-800 dark:bg-slate-900 dark:focus-within:bg-slate-900 sm:w-80">
            <Search className="h-5 w-5 shrink-0 text-slate-400" />
            <input
              placeholder="Tìm sản phẩm, đơn hàng, khách hàng..."
              className="min-w-0 flex-1 bg-transparent text-slate-800 outline-none placeholder:text-slate-400 dark:text-slate-100"
            />
          </label>

          <div className="flex items-center gap-3">
            <button
              onClick={onToggleDark}
              className="flex h-12 w-12 items-center justify-center rounded-2xl border border-slate-200 bg-white text-slate-600 shadow-sm transition hover:border-emerald-300 hover:text-emerald-600 dark:border-slate-800 dark:bg-slate-900 dark:text-slate-300"
              aria-label="Đổi chế độ màu"
            >
              {isDark ? <Sun className="h-5 w-5" /> : <Moon className="h-5 w-5" />}
            </button>
            <button className="relative flex h-12 w-12 items-center justify-center rounded-2xl border border-slate-200 bg-white text-slate-600 shadow-sm transition hover:border-emerald-300 hover:text-emerald-600 dark:border-slate-800 dark:bg-slate-900 dark:text-slate-300">
              <Bell className="h-5 w-5" />
              <span className="absolute right-3 top-3 h-2.5 w-2.5 rounded-full border-2 border-white bg-emerald-500 dark:border-slate-900" />
            </button>
            <div className="flex h-12 items-center gap-3 rounded-2xl border border-slate-200 bg-white px-3 shadow-sm dark:border-slate-800 dark:bg-slate-900">
              <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-emerald-500 text-sm font-black text-white">
                {adminName.charAt(0).toUpperCase()}
              </div>
              <div className="hidden min-w-0 sm:block">
                <p className="truncate text-sm font-bold text-slate-900 dark:text-white">{adminName}</p>
                <p className="text-xs font-medium text-slate-500 dark:text-slate-400">Administrator</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
}

function KpiCard({
  title,
  value,
  change,
  icon: Icon,
  tone,
}: {
  title: string;
  value: string;
  change: string;
  icon: React.ElementType;
  tone: string;
}) {
  return (
    <div className="group rounded-3xl border border-slate-200 bg-white p-5 shadow-sm shadow-slate-200/60 transition duration-300 hover:-translate-y-1 hover:shadow-xl hover:shadow-emerald-500/10 dark:border-slate-800 dark:bg-slate-900 dark:shadow-none">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0">
          <p className="text-sm font-semibold text-slate-500 dark:text-slate-400">{title}</p>
          <p className="mt-3 truncate text-2xl font-black tracking-tight text-slate-950 dark:text-white">{value}</p>
          <p className="mt-3 text-sm font-bold text-emerald-600 dark:text-emerald-400">{change}</p>
        </div>
        <div className={`flex h-12 w-12 shrink-0 items-center justify-center rounded-2xl ${tone}`}>
          <Icon className="h-6 w-6" />
        </div>
      </div>
    </div>
  );
}

function RevenueChart({ data }: { data: RevenuePoint[] }) {
  const points = data.slice(-12);
  const maxRevenue = Math.max(...points.map((item) => item.revenue), 1);

  return (
    <div className="rounded-3xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="mb-6 flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-lg font-black text-slate-950 dark:text-white">Doanh thu theo tháng</h2>
          <p className="text-sm text-slate-500 dark:text-slate-400">Theo dõi tăng trưởng doanh thu đã hoàn tất.</p>
        </div>
        <span className="rounded-full bg-emerald-50 px-3 py-1 text-xs font-bold text-emerald-700 dark:bg-emerald-500/10 dark:text-emerald-300">
          Monthly Revenue
        </span>
      </div>

      <div className="flex h-72 items-end gap-3 overflow-hidden rounded-2xl bg-slate-50 p-4 dark:bg-slate-950/70">
        {points.length === 0 ? (
          <div className="flex h-full w-full items-center justify-center text-sm font-medium text-slate-500">
            Chưa có dữ liệu doanh thu.
          </div>
        ) : (
          points.map((item) => {
            const height = Math.max(8, (item.revenue / maxRevenue) * 100);

            return (
              <div key={item.period} className="flex min-w-10 flex-1 flex-col items-center justify-end gap-3">
                <div className="group/bar flex h-full w-full items-end">
                  <div
                    className="w-full rounded-t-2xl bg-linear-to-t from-emerald-600 to-emerald-300 shadow-lg shadow-emerald-500/20 transition duration-300 group-hover/bar:from-emerald-500 group-hover/bar:to-teal-300"
                    style={{ height: `${height}%` }}
                    title={`${item.period}: ${formatCurrency(item.revenue)}`}
                  />
                </div>
                <span className="max-w-14 truncate text-xs font-semibold text-slate-500 dark:text-slate-400">{item.period}</span>
              </div>
            );
          })
        )}
      </div>
    </div>
  );
}

function OrderStatusChart({ data }: { data: OrderStatusStat[] }) {
  const requiredStatuses = ["pending", "confirmed", "shipping", "delivered", "cancelled"];
  const normalized = requiredStatuses.map((status) => {
    const direct = data.find((item) => normalizeStatus(item.status) === status);
    return {
      status,
      count: direct?.count ?? 0,
    };
  });
  const total = normalized.reduce((sum, item) => sum + item.count, 0);
  const max = Math.max(...normalized.map((item) => item.count), 1);

  return (
    <div className="rounded-3xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="mb-6">
        <h2 className="text-lg font-black text-slate-950 dark:text-white">Đơn hàng theo trạng thái</h2>
        <p className="text-sm text-slate-500 dark:text-slate-400">Tổng {formatNumber(total)} đơn hàng đang được phân loại.</p>
      </div>
      <div className="space-y-5">
        {normalized.map((item) => {
          const config = statusConfig[item.status];
          const width = Math.max(4, (item.count / max) * 100);

          return (
            <div key={item.status} className="space-y-2">
              <div className="flex items-center justify-between text-sm">
                <div className="flex items-center gap-2 font-bold text-slate-700 dark:text-slate-200">
                  <span className={`h-2.5 w-2.5 rounded-full ${config.dot}`} />
                  {config.label}
                </div>
                <span className="font-black text-slate-950 dark:text-white">{formatNumber(item.count)}</span>
              </div>
              <div className="h-3 rounded-full bg-slate-100 dark:bg-slate-800">
                <div className={`h-3 rounded-full ${config.dot}`} style={{ width: `${width}%` }} />
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

function RecentOrdersTable({ orders }: { orders: RecentOrder[] }) {
  return (
    <div className="rounded-3xl border border-slate-200 bg-white shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="flex items-center justify-between border-b border-slate-100 px-5 py-4 dark:border-slate-800">
        <div>
          <h2 className="text-lg font-black text-slate-950 dark:text-white">Đơn hàng gần đây</h2>
          <p className="text-sm text-slate-500 dark:text-slate-400">Luồng xử lý đơn mới nhất của shop.</p>
        </div>
        <Link href="/admin/orders" className="hidden text-sm font-bold text-emerald-600 hover:text-emerald-700 sm:block">
          Xem tất cả
        </Link>
      </div>
      <div className="overflow-x-auto">
        <table className="w-full min-w-[760px] text-left text-sm">
          <thead className="bg-slate-50 text-xs uppercase tracking-wide text-slate-500 dark:bg-slate-950/60 dark:text-slate-400">
            <tr>
              <th className="px-5 py-4 font-bold">Mã đơn</th>
              <th className="px-5 py-4 font-bold">Khách hàng</th>
              <th className="px-5 py-4 text-right font-bold">Tổng tiền</th>
              <th className="px-5 py-4 font-bold">Trạng thái</th>
              <th className="px-5 py-4 font-bold">Ngày đặt</th>
              <th className="px-5 py-4 text-right font-bold">Thao tác</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
            {orders.length === 0 ? (
              <tr>
                <td colSpan={6} className="px-5 py-10 text-center font-medium text-slate-500">
                  Chưa có đơn hàng gần đây.
                </td>
              </tr>
            ) : (
              orders.map((order) => {
                const status = normalizeStatus(order.status);
                const config = statusConfig[status];

                return (
                  <tr key={order.orderId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                    <td className="px-5 py-4 font-black text-slate-950 dark:text-white">{order.orderCode}</td>
                    <td className="px-5 py-4 text-slate-600 dark:text-slate-300">Khách #{order.userId}</td>
                    <td className="px-5 py-4 text-right font-bold text-slate-950 dark:text-white">{formatCurrency(order.finalAmount)}</td>
                    <td className="px-5 py-4">
                      <span className={`inline-flex items-center rounded-full px-3 py-1 text-xs font-black ring-1 ${config.badge}`}>
                        {config.label}
                      </span>
                    </td>
                    <td className="px-5 py-4 text-slate-600 dark:text-slate-300">{formatDate(order.createdAt)}</td>
                    <td className="px-5 py-4 text-right">
                      <button className="rounded-xl border border-slate-200 px-3 py-2 text-xs font-bold text-slate-700 transition hover:border-emerald-400 hover:text-emerald-600 dark:border-slate-700 dark:text-slate-200">
                        Chi tiết
                      </button>
                    </td>
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

function LowStockList({ items }: { items: LowStockProduct[] }) {
  return (
    <div className="rounded-3xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="mb-5 flex items-center justify-between gap-4">
        <div>
          <h2 className="text-lg font-black text-slate-950 dark:text-white">Sản phẩm sắp hết hàng</h2>
          <p className="text-sm text-slate-500 dark:text-slate-400">Ưu tiên cập nhật tồn kho trước khi hết hàng.</p>
        </div>
        <Truck className="h-6 w-6 text-emerald-500" />
      </div>
      <div className="space-y-3">
        {items.length === 0 ? (
          <div className="rounded-2xl border border-dashed border-slate-200 p-6 text-center text-sm font-medium text-slate-500 dark:border-slate-800">
            Kho đang ổn, chưa có sản phẩm cần cảnh báo.
          </div>
        ) : (
          items.slice(0, 6).map((item) => (
            <div
              key={item.variantId}
              className="flex items-center justify-between gap-4 rounded-2xl border border-slate-100 bg-slate-50 p-4 transition hover:border-emerald-200 hover:bg-emerald-50/50 dark:border-slate-800 dark:bg-slate-950/60 dark:hover:border-emerald-500/30 dark:hover:bg-emerald-500/5"
            >
              <div className="min-w-0">
                <p className="truncate font-black text-slate-900 dark:text-white">{item.productName}</p>
                <p className="mt-1 text-xs font-semibold text-slate-500 dark:text-slate-400">
                  SKU: {item.sku || "N/A"} · {[item.weight, item.gripSize, item.color].filter(Boolean).join(" / ") || "Mặc định"}
                </p>
              </div>
              <div className="flex shrink-0 items-center gap-3">
                <span className="rounded-full bg-red-50 px-3 py-1 text-xs font-black text-red-700 ring-1 ring-red-100 dark:bg-red-500/10 dark:text-red-300 dark:ring-red-500/20">
                  {item.stockQuantity}
                </span>
                <button className="rounded-xl bg-emerald-500 px-3 py-2 text-xs font-black text-white shadow-sm transition hover:bg-emerald-600">
                  Cập nhật
                </button>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}

function QuickActions() {
  const actions = [
    { label: "Thêm sản phẩm", icon: PackagePlus, href: "/admin/products/new" },
    { label: "Thêm coupon", icon: BadgePercent, href: "/admin/coupons/new" },
    { label: "Tạo đơn hàng", icon: ClipboardList, href: "/admin/orders/new" },
    { label: "Thêm thương hiệu", icon: Building2, href: "/admin/brands/new" },
  ];

  return (
    <div className="rounded-3xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <h2 className="text-lg font-black text-slate-950 dark:text-white">Quick Actions</h2>
      <div className="mt-5 grid gap-3 sm:grid-cols-2 xl:grid-cols-1">
        {actions.map((action) => {
          const Icon = action.icon;
          return (
            <Link
              key={action.label}
              href={action.href}
              className="group flex items-center justify-between rounded-2xl border border-slate-100 bg-slate-50 p-4 transition hover:-translate-y-0.5 hover:border-emerald-300 hover:bg-emerald-50 dark:border-slate-800 dark:bg-slate-950/60 dark:hover:border-emerald-500/40 dark:hover:bg-emerald-500/5"
            >
              <div className="flex items-center gap-3">
                <span className="flex h-10 w-10 items-center justify-center rounded-xl bg-white text-emerald-600 shadow-sm dark:bg-slate-900">
                  <Icon className="h-5 w-5" />
                </span>
                <span className="font-black text-slate-900 dark:text-white">{action.label}</span>
              </div>
              <ChevronRight className="h-5 w-5 text-slate-400 transition group-hover:translate-x-0.5 group-hover:text-emerald-500" />
            </Link>
          );
        })}
      </div>
    </div>
  );
}

export default function DashboardPage() {
  const router = useRouter();
  const { user, loading: authLoading, logout } = useAuth();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [isDark, setIsDark] = useState(false);
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const canViewDashboard = user?.role === "admin" || user?.role === "staff";

  useEffect(() => {
    if (!authLoading && !canViewDashboard) {
      router.push("/login");
    }
  }, [authLoading, canViewDashboard, router]);

  useEffect(() => {
    if (!canViewDashboard) {
      return;
    }

    let active = true;

    const loadData = async () => {
      try {
        setLoading(true);
        setError(null);
        const dashboardData = await dashboardService.getDashboardData();

        if (active) {
          setData(dashboardData);
        }
      } catch (loadError) {
        if (active) {
          setError(loadError instanceof Error ? loadError.message : "Không thể tải dashboard.");
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadData();

    return () => {
      active = false;
    };
  }, [canViewDashboard]);

  const pendingOrders = useMemo(() => {
    return data?.orders.find((item) => item.status === "pending")?.count ?? 0;
  }, [data?.orders]);

  const kpis = data
    ? [
        {
          title: "Tổng doanh thu",
          value: formatCurrency(data.overview.totalRevenue),
          change: "+12.8% so với kỳ trước",
          icon: BarChart3,
          tone: "bg-emerald-50 text-emerald-600 dark:bg-emerald-500/10 dark:text-emerald-300",
        },
        {
          title: "Tổng đơn hàng",
          value: formatNumber(data.overview.totalOrders),
          change: "+8.4% tuần này",
          icon: ShoppingBag,
          tone: "bg-blue-50 text-blue-600 dark:bg-blue-500/10 dark:text-blue-300",
        },
        {
          title: "Đơn hàng chờ xử lý",
          value: formatNumber(pendingOrders),
          change: "Cần xử lý hôm nay",
          icon: ClipboardList,
          tone: "bg-yellow-50 text-yellow-600 dark:bg-yellow-500/10 dark:text-yellow-300",
        },
        {
          title: "Tổng khách hàng",
          value: formatNumber(data.overview.totalUsers),
          change: "+5.2% tăng trưởng",
          icon: UserPlus,
          tone: "bg-purple-50 text-purple-600 dark:bg-purple-500/10 dark:text-purple-300",
        },
        {
          title: "Tổng sản phẩm",
          value: formatNumber(data.overview.totalProducts),
          change: "Danh mục đang bán",
          icon: Package,
          tone: "bg-cyan-50 text-cyan-600 dark:bg-cyan-500/10 dark:text-cyan-300",
        },
        {
          title: "Sản phẩm sắp hết hàng",
          value: formatNumber(data.overview.lowStockProducts),
          change: "Ưu tiên nhập kho",
          icon: Boxes,
          tone: "bg-red-50 text-red-600 dark:bg-red-500/10 dark:text-red-300",
        },
      ]
    : [];

  const handleLogout = () => {
    logout();
    router.push("/login");
  };

  if (authLoading || !canViewDashboard) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-50 text-sm font-semibold text-slate-500">
        Đang kiểm tra quyền quản trị...
      </div>
    );
  }

  return (
    <div className={isDark ? "dark" : ""}>
      <div className="min-h-screen bg-[#F8FAFC] text-slate-950 dark:bg-slate-950 dark:text-white">
        <div className="lg:grid lg:grid-cols-[18rem_minmax(0,1fr)]">
          <AdminSidebar open={sidebarOpen} onClose={() => setSidebarOpen(false)} onLogout={handleLogout} />

          <div className="min-w-0">
            <AdminHeader
              adminName={user?.fullName || "Admin"}
              isDark={isDark}
              onToggleDark={() => setIsDark((value) => !value)}
              onOpenSidebar={() => setSidebarOpen(true)}
            />

            <main className="space-y-6 px-4 py-6 sm:px-6 lg:px-8">
              <section className="rounded-3xl bg-[#0F172A] p-5 text-white shadow-xl shadow-slate-900/10 sm:p-6">
                <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
                  <div>
                    <p className="text-sm font-bold uppercase tracking-[0.24em] text-emerald-300">Badminton Commerce Ops</p>
                    <h2 className="mt-3 text-3xl font-black tracking-tight sm:text-4xl">
                      Quản trị FlyShot chuyên nghiệp, gọn và rõ việc.
                    </h2>
                    <p className="mt-3 max-w-2xl text-sm leading-6 text-slate-300">
                      Theo dõi doanh thu, tồn kho, đơn hàng và khách hàng trong một màn hình vận hành duy nhất.
                    </p>
                  </div>
                  <div className="rounded-2xl border border-white/10 bg-white/10 px-4 py-3">
                    <p className="text-xs font-semibold text-slate-300">Doanh thu đang xử lý</p>
                    <p className="mt-1 text-2xl font-black">{data ? formatCurrency(data.overview.pendingRevenue) : "--"}</p>
                  </div>
                </div>
              </section>

              {loading ? (
                <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
                  {Array.from({ length: 6 }).map((_, index) => (
                    <div key={index} className="h-36 animate-pulse rounded-3xl bg-white dark:bg-slate-900" />
                  ))}
                </section>
              ) : error ? (
                <div className="rounded-3xl border border-red-200 bg-red-50 p-6 text-sm font-bold text-red-700 dark:border-red-500/20 dark:bg-red-500/10 dark:text-red-300">
                  {error}
                </div>
              ) : data ? (
                <>
                  <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
                    {kpis.map((item) => (
                      <KpiCard key={item.title} {...item} />
                    ))}
                  </section>

                  <section className="grid gap-6 2xl:grid-cols-[minmax(0,1.35fr)_minmax(360px,0.65fr)]">
                    <RevenueChart data={data.revenue} />
                    <OrderStatusChart data={data.orders} />
                  </section>

                  <section className="grid gap-6 2xl:grid-cols-[minmax(0,1.45fr)_minmax(360px,0.55fr)]">
                    <RecentOrdersTable orders={data.recentOrders} />
                    <QuickActions />
                  </section>

                  <section className="grid gap-6 xl:grid-cols-[minmax(0,0.95fr)_minmax(0,1.05fr)]">
                    <LowStockList items={data.lowStock} />
                    <div className="rounded-3xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
                      <div className="mb-5">
                        <h2 className="text-lg font-black text-slate-950 dark:text-white">Sản phẩm bán chạy</h2>
                        <p className="text-sm text-slate-500 dark:text-slate-400">Tối ưu tồn kho cho những mẫu đang kéo doanh thu.</p>
                      </div>
                      <div className="space-y-4">
                        {data.topProducts.length === 0 ? (
                          <div className="rounded-2xl border border-dashed border-slate-200 p-6 text-center text-sm font-medium text-slate-500 dark:border-slate-800">
                            Chưa có dữ liệu bán hàng.
                          </div>
                        ) : (
                          data.topProducts.slice(0, 6).map((item, index) => {
                            const max = Math.max(...data.topProducts.map((product) => product.soldQuantity), 1);
                            const width = Math.max(8, (item.soldQuantity / max) * 100);

                            return (
                              <div key={item.productId}>
                                <div className="mb-2 flex items-center justify-between gap-4 text-sm">
                                  <span className="truncate font-black text-slate-900 dark:text-white">
                                    {index + 1}. {item.productName}
                                  </span>
                                  <span className="shrink-0 font-bold text-slate-500 dark:text-slate-400">
                                    {formatNumber(item.soldQuantity)} bán
                                  </span>
                                </div>
                                <div className="h-3 rounded-full bg-slate-100 dark:bg-slate-800">
                                  <div className="h-3 rounded-full bg-emerald-500" style={{ width: `${width}%` }} />
                                </div>
                                <p className="mt-1 text-xs font-semibold text-slate-500 dark:text-slate-400">{formatCurrency(item.revenue)}</p>
                              </div>
                            );
                          })
                        )}
                      </div>
                    </div>
                  </section>
                </>
              ) : null}
            </main>
          </div>
        </div>
      </div>
    </div>
  );
}
