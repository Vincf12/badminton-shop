"use client";

import React, { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import {
  ArrowLeft,
  BadgePercent,
  Boxes,
  Building2,
  CheckCircle2,
  LayoutDashboard,
  Loader2,
  Package,
  Plus,
  RotateCw,
  Save,
  ShoppingBag,
  Tags,
  Trash2,
  UserCog,
  X,
} from "lucide-react";
import { useAuth } from "@/features/auth";
import {
  adminService,
  type AdminSection,
  type BannerAdminItem,
  type BannerPayload,
  type BrandAdminItem,
  type CategoryAdminItem,
  type CouponAdminItem,
  type CouponPayload,
  type OrderAdminItem,
  type ProductAdminItem,
  type ProductPayload,
  type UserAdminItem,
} from "@/widgets/admin";

type Row = ProductAdminItem | CategoryAdminItem | BrandAdminItem | CouponAdminItem | OrderAdminItem | UserAdminItem | BannerAdminItem;

const sectionConfig: Record<AdminSection, { title: string; description: string; icon: React.ElementType }> = {
  products: {
    title: "Quản lý sản phẩm",
    description: "CRUD sản phẩm, giá, tồn kho và ảnh đại diện qua API /products.",
    icon: Package,
  },
  categories: {
    title: "Quản lý danh mục",
    description: "Tạo, sửa, xóa danh mục sản phẩm qua API /categories.",
    icon: Tags,
  },
  brands: {
    title: "Quản lý thương hiệu",
    description: "Tạo, sửa, xóa thương hiệu qua API /brands.",
    icon: Building2,
  },
  coupons: {
    title: "Quản lý coupon",
    description: "CRUD mã giảm giá và trạng thái hiệu lực qua API /coupons.",
    icon: BadgePercent,
  },
  orders: {
    title: "Quản lý đơn hàng",
    description: "Xem đơn hàng, cập nhật trạng thái và hủy đơn qua API /orders.",
    icon: ShoppingBag,
  },
  customers: {
    title: "Quản lý khách hàng",
    description: "Xem người dùng, đổi role, khóa/mở và xóa qua API /User.",
    icon: UserCog,
  },
  banners: {
    title: "Quản lý banner",
    description: "CRUD banner trang chủ và bật/tắt hiển thị qua API /banners.",
    icon: Boxes,
  },
};

const orderStatuses = ["pending", "confirmed", "shipping", "completed", "cancelled"];
const roles = ["customer", "staff", "admin"];

function formatCurrency(value: number): string {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    maximumFractionDigits: 0,
  }).format(value);
}

function formatDate(value?: string | null): string {
  if (!value) return "--";
  return new Intl.DateTimeFormat("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  }).format(new Date(value));
}

function toInputDate(value?: string | null): string {
  if (!value) return "";
  return value.split("T")[0];
}

function isSection(value: string): value is AdminSection {
  return value in sectionConfig;
}

function getRowId(section: AdminSection, row: Row): number {
  switch (section) {
    case "products":
      return (row as ProductAdminItem).productId;
    case "categories":
      return (row as CategoryAdminItem).categoryId;
    case "brands":
      return (row as BrandAdminItem).brandId;
    case "coupons":
      return (row as CouponAdminItem).couponId;
    case "orders":
      return (row as OrderAdminItem).orderId;
    case "customers":
      return (row as UserAdminItem).userId;
    case "banners":
      return (row as BannerAdminItem).bannerId;
  }
}

function getRowLabel(section: AdminSection, row: Row): string {
  switch (section) {
    case "products":
      return (row as ProductAdminItem).productName;
    case "categories":
      return (row as CategoryAdminItem).categoryName;
    case "brands":
      return (row as BrandAdminItem).brandName;
    case "coupons":
      return (row as CouponAdminItem).code;
    case "orders":
      return (row as OrderAdminItem).orderCode;
    case "customers":
      return (row as UserAdminItem).email;
    case "banners":
      return (row as BannerAdminItem).title;
  }
}

function EmptyForm({ section }: { section: AdminSection }): Record<string, string | boolean> {
  if (section === "products") {
    return {
      productName: "",
      categoryId: "",
      brandId: "",
      price: "0",
      stock: "0",
      imageUrl: "",
      description: "",
      status: "active",
    };
  }

  if (section === "coupons") {
    const today = new Date().toISOString().split("T")[0];
    return {
      code: "",
      couponName: "",
      discountType: "percentage",
      discountValue: "10",
      minimumOrderAmount: "0",
      maximumDiscountAmount: "",
      usageLimit: "",
      startDate: today,
      endDate: today,
      isActive: true,
    };
  }

  if (section === "banners") {
    const today = new Date().toISOString().split("T")[0];
    return {
      title: "",
      imageUrl: "",
      targetType: "custom",
      targetId: "",
      customUrl: "",
      position: "HOME_TOP",
      displayOrder: "0",
      isActive: true,
      startDate: today,
      endDate: "",
    };
  }

  return { name: "" };
}

function createProductPayload(form: Record<string, string | boolean>): ProductPayload {
  return {
    categoryId: Number(form.categoryId),
    brandId: Number(form.brandId),
    productName: String(form.productName || "").trim(),
    slug: null,
    shortDescription: null,
    description: String(form.description || "").trim() || null,
    status: String(form.status || "active"),
    imageUrl: String(form.imageUrl || "").trim() || null,
    price: Number(form.price || 0),
    stock: Number(form.stock || 0),
  };
}

function createCouponPayload(form: Record<string, string | boolean>): CouponPayload {
  return {
    code: String(form.code || "").trim().toUpperCase(),
    couponName: String(form.couponName || "").trim(),
    discountType: String(form.discountType || "percentage"),
    discountValue: Number(form.discountValue || 0),
    minimumOrderAmount: Number(form.minimumOrderAmount || 0),
    maximumDiscountAmount: form.maximumDiscountAmount ? Number(form.maximumDiscountAmount) : null,
    usageLimit: form.usageLimit ? Number(form.usageLimit) : null,
    startDate: String(form.startDate || new Date().toISOString()),
    endDate: String(form.endDate || new Date().toISOString()),
    isActive: Boolean(form.isActive),
  };
}

function createBannerPayload(form: Record<string, string | boolean>): BannerPayload {
  return {
    title: String(form.title || "").trim(),
    imageUrl: String(form.imageUrl || "").trim(),
    targetType: String(form.targetType || "custom"),
    targetId: String(form.targetId || "").trim() || null,
    customUrl: String(form.customUrl || "").trim() || null,
    position: String(form.position || "HOME_TOP"),
    displayOrder: Number(form.displayOrder || 0),
    isActive: Boolean(form.isActive),
    startDate: String(form.startDate || new Date().toISOString()),
    endDate: form.endDate ? String(form.endDate) : null,
  };
}

export default function AdminSectionPage() {
  const params = useParams<{ section: string }>();
  const router = useRouter();
  const section = isSection(params.section) ? params.section : null;
  const { user, loading: authLoading, logout } = useAuth();
  const role = user?.role?.toLowerCase();
  const canOpenAdmin = role === "admin" || role === "staff";

  const [rows, setRows] = useState<Row[]>([]);
  const [categories, setCategories] = useState<CategoryAdminItem[]>([]);
  const [brands, setBrands] = useState<BrandAdminItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [form, setForm] = useState<Record<string, string | boolean>>(() => (section ? EmptyForm({ section }) : { name: "" }));

  const config = section ? sectionConfig[section] : null;
  const Icon = config?.icon ?? LayoutDashboard;
  const canWrite = role === "admin" || role === "staff";
  const canDelete = role === "admin";
  const formDisabled = section === "orders" || section === "customers";

  const loadData = async () => {
    if (!section) return;

    try {
      setLoading(true);
      setError(null);
      setMessage(null);

      if (section === "products") {
        const [products, nextCategories, nextBrands] = await Promise.all([
          adminService.getProducts(),
          adminService.getCategories(),
          adminService.getBrands(),
        ]);
        setRows(products);
        setCategories(nextCategories);
        setBrands(nextBrands);
      } else if (section === "categories") {
        setRows(await adminService.getCategories());
      } else if (section === "brands") {
        setRows(await adminService.getBrands());
      } else if (section === "coupons") {
        setRows(await adminService.getCoupons());
      } else if (section === "orders") {
        setRows(await adminService.getOrders());
      } else if (section === "customers") {
        setRows(await adminService.getUsers());
      } else if (section === "banners") {
        setRows(await adminService.getBanners());
      }
    } catch (loadError) {
      setError(loadError instanceof Error ? loadError.message : "Không thể tải dữ liệu quản trị.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!authLoading && !canOpenAdmin) {
      router.push("/login");
    }
  }, [authLoading, canOpenAdmin, router]);

  useEffect(() => {
    if (section && canOpenAdmin) {
      setForm(EmptyForm({ section }));
      setEditingId(null);
      void loadData();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [section, canOpenAdmin]);

  const stats = useMemo(() => {
    if (section === "orders") {
      const total = (rows as OrderAdminItem[]).reduce((sum, item) => sum + item.finalAmount, 0);
      return `${rows.length} đơn hàng · ${formatCurrency(total)}`;
    }

    if (section === "coupons") {
      const active = (rows as CouponAdminItem[]).filter((item) => item.isActive).length;
      return `${rows.length} coupon · ${active} đang bật`;
    }

    if (section === "customers") {
      const active = (rows as UserAdminItem[]).filter((item) => item.isActive).length;
      return `${rows.length} tài khoản · ${active} đang hoạt động`;
    }

    if (section === "banners") {
      const active = (rows as BannerAdminItem[]).filter((item) => item.isActive).length;
      return `${rows.length} banner · ${active} đang hiển thị`;
    }

    return `${rows.length} bản ghi`;
  }, [rows, section]);

  const updateForm = (key: string, value: string | boolean) => {
    setForm((current) => ({ ...current, [key]: value }));
  };

  const resetForm = () => {
    if (!section) return;
    setEditingId(null);
    setForm(EmptyForm({ section }));
  };

  const startEdit = (row: Row) => {
    if (!section) return;

    setEditingId(getRowId(section, row));

    if (section === "products") {
      const item = row as ProductAdminItem;
      setForm({
        productName: item.productName,
        categoryId: String(item.categoryId),
        brandId: String(item.brandId),
        price: String(item.price),
        stock: String(item.stock),
        imageUrl: item.imageUrl ?? "",
        description: item.description ?? "",
        status: "active",
      });
    } else if (section === "categories") {
      setForm({ name: (row as CategoryAdminItem).categoryName });
    } else if (section === "brands") {
      setForm({ name: (row as BrandAdminItem).brandName });
    } else if (section === "coupons") {
      const item = row as CouponAdminItem;
      setForm({
        code: item.code,
        couponName: item.couponName,
        discountType: item.discountType,
        discountValue: String(item.discountValue),
        minimumOrderAmount: String(item.minimumOrderAmount),
        maximumDiscountAmount: item.maximumDiscountAmount ? String(item.maximumDiscountAmount) : "",
        usageLimit: item.usageLimit ? String(item.usageLimit) : "",
        startDate: toInputDate(item.startDate),
        endDate: toInputDate(item.endDate),
        isActive: item.isActive,
      });
    } else if (section === "banners") {
      const item = row as BannerAdminItem;
      setForm({
        title: item.title,
        imageUrl: item.imageUrl,
        targetType: item.targetType,
        targetId: item.targetId ?? "",
        customUrl: item.customUrl ?? "",
        position: item.position,
        displayOrder: String(item.displayOrder),
        isActive: item.isActive,
        startDate: toInputDate(item.startDate),
        endDate: toInputDate(item.endDate),
      });
    }
  };

  const saveForm = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!section || formDisabled || !canWrite) return;

    try {
      setSaving(true);
      setError(null);
      setMessage(null);

      if (section === "products") {
        const payload = createProductPayload(form);
        if (editingId) {
          await adminService.updateProduct(editingId, payload);
        } else {
          await adminService.createProduct(payload);
        }
      } else if (section === "categories") {
        const name = String(form.name || "").trim();
        if (editingId) {
          await adminService.updateCategory(editingId, name);
        } else {
          await adminService.createCategory(name);
        }
      } else if (section === "brands") {
        const name = String(form.name || "").trim();
        if (editingId) {
          await adminService.updateBrand(editingId, name);
        } else {
          await adminService.createBrand(name);
        }
      } else if (section === "coupons") {
        const payload = createCouponPayload(form);
        if (editingId) {
          await adminService.updateCoupon(editingId, payload);
        } else {
          await adminService.createCoupon(payload);
        }
      } else if (section === "banners") {
        const payload = createBannerPayload(form);
        if (editingId) {
          await adminService.updateBanner(editingId, payload);
        } else {
          await adminService.createBanner(payload);
        }
      }

      setMessage(editingId ? "Đã cập nhật dữ liệu." : "Đã tạo dữ liệu mới.");
      resetForm();
      await loadData();
    } catch (saveError) {
      setError(saveError instanceof Error ? saveError.message : "Không thể lưu dữ liệu.");
    } finally {
      setSaving(false);
    }
  };

  const deleteRow = async (row: Row) => {
    if (!section || !canDelete) return;
    const id = getRowId(section, row);
    const label = getRowLabel(section, row);

    if (!window.confirm(`Xóa "${label}"? Thao tác này không thể hoàn tác.`)) {
      return;
    }

    try {
      setError(null);

      if (section === "products") await adminService.deleteProduct(id);
      if (section === "categories") await adminService.deleteCategory(id);
      if (section === "brands") await adminService.deleteBrand(id);
      if (section === "coupons") await adminService.deleteCoupon(id);
      if (section === "customers") await adminService.deleteUser(id);
      if (section === "banners") await adminService.deleteBanner(id);

      setMessage("Đã xóa dữ liệu.");
      await loadData();
    } catch (deleteError) {
      setError(deleteError instanceof Error ? deleteError.message : "Không thể xóa dữ liệu.");
    }
  };

  const updateOrderStatus = async (orderId: number, status: string) => {
    try {
      await adminService.updateOrderStatus(orderId, status, "Cập nhật từ dashboard admin.");
      setMessage("Đã cập nhật trạng thái đơn hàng.");
      await loadData();
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : "Không thể cập nhật đơn hàng.");
    }
  };

  const updateUserRole = async (userId: number, nextRole: string) => {
    try {
      await adminService.updateUserRole(userId, nextRole);
      setMessage("Đã cập nhật role người dùng.");
      await loadData();
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : "Không thể cập nhật role.");
    }
  };

  const updateUserStatus = async (userId: number, isActive: boolean) => {
    try {
      await adminService.updateUserStatus(userId, isActive);
      setMessage("Đã cập nhật trạng thái người dùng.");
      await loadData();
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : "Không thể cập nhật trạng thái.");
    }
  };

  const updateBannerStatus = async (bannerId: number, isActive: boolean) => {
    try {
      await adminService.updateBannerStatus(bannerId, isActive);
      setMessage("Đã cập nhật trạng thái banner.");
      await loadData();
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : "Không thể cập nhật banner.");
    }
  };

  if (!section || !config) {
    return (
      <div className="min-h-screen bg-slate-50 p-8 text-slate-900">
        <Link href="/admin" className="inline-flex items-center gap-2 text-sm font-bold text-emerald-700">
          <ArrowLeft className="h-4 w-4" />
          Về dashboard admin
        </Link>
        <h1 className="mt-6 text-3xl font-black">Trang admin không tồn tại</h1>
      </div>
    );
  }

  if (authLoading || !canOpenAdmin) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-50 text-sm font-semibold text-slate-500">
        Đang kiểm tra quyền quản trị...
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-950">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-7xl flex-col gap-4 px-4 py-5 sm:px-6 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex items-center gap-4">
            <Link
              href="/admin"
              className="flex h-11 w-11 items-center justify-center rounded-xl border border-slate-200 bg-white text-slate-700 shadow-sm hover:border-emerald-300 hover:text-emerald-700"
              aria-label="Về dashboard"
            >
              <ArrowLeft className="h-5 w-5" />
            </Link>
            <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-emerald-600 text-white">
              <Icon className="h-6 w-6" />
            </div>
            <div>
              <h1 className="text-2xl font-black tracking-tight sm:text-3xl">{config.title}</h1>
              <p className="mt-1 text-sm font-medium text-slate-500">{config.description}</p>
            </div>
          </div>
          <div className="flex flex-wrap items-center gap-3">
            <span className="rounded-full bg-emerald-50 px-4 py-2 text-sm font-black text-emerald-700 ring-1 ring-emerald-100">
              {stats}
            </span>
            <button
              onClick={loadData}
              className="inline-flex h-11 items-center gap-2 rounded-xl border border-slate-200 bg-white px-4 text-sm font-bold text-slate-700 hover:border-emerald-300 hover:text-emerald-700"
            >
              <RotateCw className="h-4 w-4" />
              Tải lại
            </button>
            <button
              onClick={() => {
                logout();
                router.push("/login");
              }}
              className="h-11 rounded-xl bg-slate-900 px-4 text-sm font-bold text-white hover:bg-slate-800"
            >
              Đăng xuất
            </button>
          </div>
        </div>
      </header>

      <main className="mx-auto grid max-w-7xl gap-6 px-4 py-6 sm:px-6 xl:grid-cols-[360px_minmax(0,1fr)]">
        <aside className="space-y-4">
          <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
            <div className="flex items-center gap-2">
              <Plus className="h-5 w-5 text-emerald-600" />
              <h2 className="text-lg font-black">{editingId ? "Cập nhật" : "Thêm mới"}</h2>
            </div>

            {formDisabled ? (
              <div className="mt-4 rounded-xl border border-slate-200 bg-slate-50 p-4 text-sm font-medium leading-6 text-slate-600">
                Trang này dùng thao tác nhanh trực tiếp trên bảng. Các API tạo mới riêng sẽ nằm ở luồng người dùng hoặc chi tiết vận hành.
              </div>
            ) : (
              <form onSubmit={saveForm} className="mt-5 space-y-4">
                {section === "products" && (
                  <>
                    <Input label="Tên sản phẩm" value={String(form.productName ?? "")} onChange={(value) => updateForm("productName", value)} required />
                    <Select label="Danh mục" value={String(form.categoryId ?? "")} onChange={(value) => updateForm("categoryId", value)} required>
                      <option value="">Chọn danh mục</option>
                      {categories.map((item) => (
                        <option key={item.categoryId} value={item.categoryId}>{item.categoryName}</option>
                      ))}
                    </Select>
                    <Select label="Thương hiệu" value={String(form.brandId ?? "")} onChange={(value) => updateForm("brandId", value)} required>
                      <option value="">Chọn thương hiệu</option>
                      {brands.map((item) => (
                        <option key={item.brandId} value={item.brandId}>{item.brandName}</option>
                      ))}
                    </Select>
                    <div className="grid grid-cols-2 gap-3">
                      <Input label="Giá" type="number" value={String(form.price ?? "")} onChange={(value) => updateForm("price", value)} />
                      <Input label="Tồn kho" type="number" value={String(form.stock ?? "")} onChange={(value) => updateForm("stock", value)} />
                    </div>
                    <Input label="Ảnh URL" value={String(form.imageUrl ?? "")} onChange={(value) => updateForm("imageUrl", value)} />
                    <Textarea label="Mô tả" value={String(form.description ?? "")} onChange={(value) => updateForm("description", value)} />
                  </>
                )}

                {(section === "categories" || section === "brands") && (
                  <Input label={section === "categories" ? "Tên danh mục" : "Tên thương hiệu"} value={String(form.name ?? "")} onChange={(value) => updateForm("name", value)} required />
                )}

                {section === "coupons" && (
                  <>
                    <Input label="Mã coupon" value={String(form.code ?? "")} onChange={(value) => updateForm("code", value)} required />
                    <Input label="Tên coupon" value={String(form.couponName ?? "")} onChange={(value) => updateForm("couponName", value)} required />
                    <Select label="Loại giảm" value={String(form.discountType ?? "percentage")} onChange={(value) => updateForm("discountType", value)}>
                      <option value="percentage">Phần trăm</option>
                      <option value="fixed">Số tiền cố định</option>
                    </Select>
                    <div className="grid grid-cols-2 gap-3">
                      <Input label="Giá trị" type="number" value={String(form.discountValue ?? "")} onChange={(value) => updateForm("discountValue", value)} />
                      <Input label="Đơn tối thiểu" type="number" value={String(form.minimumOrderAmount ?? "")} onChange={(value) => updateForm("minimumOrderAmount", value)} />
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <Input label="Giảm tối đa" type="number" value={String(form.maximumDiscountAmount ?? "")} onChange={(value) => updateForm("maximumDiscountAmount", value)} />
                      <Input label="Giới hạn lượt" type="number" value={String(form.usageLimit ?? "")} onChange={(value) => updateForm("usageLimit", value)} />
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <Input label="Ngày bắt đầu" type="date" value={String(form.startDate ?? "")} onChange={(value) => updateForm("startDate", value)} />
                      <Input label="Ngày kết thúc" type="date" value={String(form.endDate ?? "")} onChange={(value) => updateForm("endDate", value)} />
                    </div>
                    <Checkbox label="Đang hoạt động" checked={Boolean(form.isActive)} onChange={(value) => updateForm("isActive", value)} />
                  </>
                )}

                {section === "banners" && (
                  <>
                    <Input label="Tiêu đề" value={String(form.title ?? "")} onChange={(value) => updateForm("title", value)} required />
                    <Input label="Ảnh URL" value={String(form.imageUrl ?? "")} onChange={(value) => updateForm("imageUrl", value)} required />
                    <div className="grid grid-cols-2 gap-3">
                      <Input label="Vị trí" value={String(form.position ?? "")} onChange={(value) => updateForm("position", value)} />
                      <Input label="Thứ tự" type="number" value={String(form.displayOrder ?? "")} onChange={(value) => updateForm("displayOrder", value)} />
                    </div>
                    <Select label="Target" value={String(form.targetType ?? "custom")} onChange={(value) => updateForm("targetType", value)}>
                      <option value="custom">Custom URL</option>
                      <option value="product">Sản phẩm</option>
                      <option value="category">Danh mục</option>
                    </Select>
                    <Input label="Target ID" value={String(form.targetId ?? "")} onChange={(value) => updateForm("targetId", value)} />
                    <Input label="Custom URL" value={String(form.customUrl ?? "")} onChange={(value) => updateForm("customUrl", value)} />
                    <div className="grid grid-cols-2 gap-3">
                      <Input label="Ngày bắt đầu" type="date" value={String(form.startDate ?? "")} onChange={(value) => updateForm("startDate", value)} />
                      <Input label="Ngày kết thúc" type="date" value={String(form.endDate ?? "")} onChange={(value) => updateForm("endDate", value)} />
                    </div>
                    <Checkbox label="Đang hiển thị" checked={Boolean(form.isActive)} onChange={(value) => updateForm("isActive", value)} />
                  </>
                )}

                <div className="flex gap-3">
                  <button
                    type="submit"
                    disabled={saving || !canWrite}
                    className="inline-flex h-11 flex-1 items-center justify-center gap-2 rounded-xl bg-emerald-600 px-4 text-sm font-black text-white hover:bg-emerald-700 disabled:cursor-not-allowed disabled:opacity-60"
                  >
                    {saving ? <Loader2 className="h-4 w-4 animate-spin" /> : <Save className="h-4 w-4" />}
                    Lưu
                  </button>
                  {editingId && (
                    <button
                      type="button"
                      onClick={resetForm}
                      className="flex h-11 w-11 items-center justify-center rounded-xl border border-slate-200 text-slate-600 hover:border-red-300 hover:text-red-600"
                      aria-label="Hủy sửa"
                    >
                      <X className="h-4 w-4" />
                    </button>
                  )}
                </div>
              </form>
            )}
          </section>

          <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
            <h2 className="text-sm font-black uppercase text-slate-500">API liên quan</h2>
            <div className="mt-4 space-y-2 text-sm font-semibold text-slate-600">
              {getApiList(section).map((item) => (
                <div key={item} className="rounded-xl bg-slate-50 px-3 py-2">{item}</div>
              ))}
            </div>
          </section>
        </aside>

        <section className="min-w-0 rounded-2xl border border-slate-200 bg-white shadow-sm">
          <div className="border-b border-slate-100 p-5">
            {error && <div className="rounded-xl border border-red-200 bg-red-50 p-3 text-sm font-bold text-red-700">{error}</div>}
            {message && (
              <div className="mt-3 flex items-center gap-2 rounded-xl border border-emerald-200 bg-emerald-50 p-3 text-sm font-bold text-emerald-700">
                <CheckCircle2 className="h-4 w-4" />
                {message}
              </div>
            )}
          </div>

          {loading ? (
            <div className="flex h-80 items-center justify-center text-sm font-bold text-slate-500">
              <Loader2 className="mr-2 h-5 w-5 animate-spin" />
              Đang tải dữ liệu...
            </div>
          ) : (
            <div className="overflow-x-auto">
              <AdminTable
                section={section}
                rows={rows}
                onEdit={startEdit}
                onDelete={deleteRow}
                canDelete={canDelete}
                onOrderStatus={updateOrderStatus}
                onUserRole={updateUserRole}
                onUserStatus={updateUserStatus}
                onBannerStatus={updateBannerStatus}
              />
            </div>
          )}
        </section>
      </main>
    </div>
  );
}

function Input({
  label,
  value,
  onChange,
  type = "text",
  required,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  type?: string;
  required?: boolean;
}) {
  return (
    <label className="block space-y-1.5">
      <span className="text-xs font-black uppercase tracking-wide text-slate-500">{label}</span>
      <input
        type={type}
        value={value}
        required={required}
        onChange={(event) => onChange(event.target.value)}
        className="h-11 w-full rounded-xl border border-slate-200 bg-white px-3 text-sm font-semibold outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-100"
      />
    </label>
  );
}

function Textarea({ label, value, onChange }: { label: string; value: string; onChange: (value: string) => void }) {
  return (
    <label className="block space-y-1.5">
      <span className="text-xs font-black uppercase tracking-wide text-slate-500">{label}</span>
      <textarea
        value={value}
        onChange={(event) => onChange(event.target.value)}
        rows={4}
        className="w-full resize-none rounded-xl border border-slate-200 bg-white px-3 py-3 text-sm font-semibold outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-100"
      />
    </label>
  );
}

function Select({
  label,
  value,
  onChange,
  children,
  required,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  children: React.ReactNode;
  required?: boolean;
}) {
  return (
    <label className="block space-y-1.5">
      <span className="text-xs font-black uppercase tracking-wide text-slate-500">{label}</span>
      <select
        value={value}
        required={required}
        onChange={(event) => onChange(event.target.value)}
        className="h-11 w-full rounded-xl border border-slate-200 bg-white px-3 text-sm font-semibold outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-100"
      >
        {children}
      </select>
    </label>
  );
}

function Checkbox({ label, checked, onChange }: { label: string; checked: boolean; onChange: (value: boolean) => void }) {
  return (
    <label className="flex items-center gap-3 rounded-xl border border-slate-200 px-3 py-3 text-sm font-bold text-slate-700">
      <input type="checkbox" checked={checked} onChange={(event) => onChange(event.target.checked)} className="h-4 w-4 accent-emerald-600" />
      {label}
    </label>
  );
}

function AdminTable({
  section,
  rows,
  onEdit,
  onDelete,
  canDelete,
  onOrderStatus,
  onUserRole,
  onUserStatus,
  onBannerStatus,
}: {
  section: AdminSection;
  rows: Row[];
  onEdit: (row: Row) => void;
  onDelete: (row: Row) => void;
  canDelete: boolean;
  onOrderStatus: (orderId: number, status: string) => void;
  onUserRole: (userId: number, role: string) => void;
  onUserStatus: (userId: number, isActive: boolean) => void;
  onBannerStatus: (bannerId: number, isActive: boolean) => void;
}) {
  if (rows.length === 0) {
    return <div className="p-10 text-center text-sm font-bold text-slate-500">Chưa có dữ liệu.</div>;
  }

  if (section === "products") {
    return (
      <Table headers={["Sản phẩm", "Phân loại", "Giá", "Tồn kho", "Ngày tạo", ""]}>
        {(rows as ProductAdminItem[]).map((item) => (
          <tr key={item.productId} className="border-b border-slate-100">
            <Td><span className="font-black">{item.productName}</span></Td>
            <Td>{item.categoryName} · {item.brandName}</Td>
            <Td>{formatCurrency(item.price)}</Td>
            <Td>{item.stock}</Td>
            <Td>{formatDate(item.createdAt)}</Td>
            <ActionTd row={item} onEdit={onEdit} onDelete={onDelete} canDelete={canDelete} />
          </tr>
        ))}
      </Table>
    );
  }

  if (section === "categories") {
    return (
      <Table headers={["ID", "Danh mục", ""]}>
        {(rows as CategoryAdminItem[]).map((item) => (
          <tr key={item.categoryId} className="border-b border-slate-100">
            <Td>#{item.categoryId}</Td>
            <Td><span className="font-black">{item.categoryName}</span></Td>
            <ActionTd row={item} onEdit={onEdit} onDelete={onDelete} canDelete={canDelete} />
          </tr>
        ))}
      </Table>
    );
  }

  if (section === "brands") {
    return (
      <Table headers={["ID", "Thương hiệu", ""]}>
        {(rows as BrandAdminItem[]).map((item) => (
          <tr key={item.brandId} className="border-b border-slate-100">
            <Td>#{item.brandId}</Td>
            <Td><span className="font-black">{item.brandName}</span></Td>
            <ActionTd row={item} onEdit={onEdit} onDelete={onDelete} canDelete={canDelete} />
          </tr>
        ))}
      </Table>
    );
  }

  if (section === "coupons") {
    return (
      <Table headers={["Mã", "Giảm", "Giới hạn", "Hiệu lực", "Trạng thái", ""]}>
        {(rows as CouponAdminItem[]).map((item) => (
          <tr key={item.couponId} className="border-b border-slate-100">
            <Td><span className="font-black">{item.code}</span><p className="text-xs text-slate-500">{item.couponName}</p></Td>
            <Td>{item.discountType === "percentage" ? `${item.discountValue}%` : formatCurrency(item.discountValue)}</Td>
            <Td>{item.usedCount}/{item.usageLimit ?? "∞"}</Td>
            <Td>{formatDate(item.startDate)} - {formatDate(item.endDate)}</Td>
            <Td><StatusPill active={item.isActive} /></Td>
            <ActionTd row={item} onEdit={onEdit} onDelete={onDelete} canDelete={canDelete} />
          </tr>
        ))}
      </Table>
    );
  }

  if (section === "orders") {
    return (
      <Table headers={["Đơn hàng", "Khách", "Tổng tiền", "Ngày", "Trạng thái", ""]}>
        {(rows as OrderAdminItem[]).map((item) => (
          <tr key={item.orderId} className="border-b border-slate-100">
            <Td><span className="font-black">{item.orderCode}</span></Td>
            <Td>#{item.userId}</Td>
            <Td>{formatCurrency(item.finalAmount)}</Td>
            <Td>{formatDate(item.createdAt)}</Td>
            <Td>
              <select
                value={item.status}
                onChange={(event) => onOrderStatus(item.orderId, event.target.value)}
                className="h-9 rounded-lg border border-slate-200 px-2 text-sm font-bold"
              >
                {orderStatuses.map((status) => (
                  <option key={status} value={status}>{status}</option>
                ))}
              </select>
            </Td>
            <Td>
              <button onClick={() => onOrderStatus(item.orderId, "cancelled")} className="rounded-lg border border-red-200 px-3 py-2 text-xs font-black text-red-700 hover:bg-red-50">
                Hủy
              </button>
            </Td>
          </tr>
        ))}
      </Table>
    );
  }

  if (section === "customers") {
    return (
      <Table headers={["Người dùng", "Điện thoại", "Ngày tạo", "Role", "Trạng thái", ""]}>
        {(rows as UserAdminItem[]).map((item) => (
          <tr key={item.userId} className="border-b border-slate-100">
            <Td><span className="font-black">{item.fullName}</span><p className="text-xs text-slate-500">{item.email}</p></Td>
            <Td>{item.phone || "--"}</Td>
            <Td>{formatDate(item.createdAt)}</Td>
            <Td>
              <select value={item.role || "customer"} onChange={(event) => onUserRole(item.userId, event.target.value)} className="h-9 rounded-lg border border-slate-200 px-2 text-sm font-bold">
                {roles.map((role) => <option key={role} value={role}>{role}</option>)}
              </select>
            </Td>
            <Td>
              <button onClick={() => onUserStatus(item.userId, !item.isActive)}>
                <StatusPill active={item.isActive} />
              </button>
            </Td>
            <ActionTd row={item} onDelete={onDelete} canDelete={canDelete} />
          </tr>
        ))}
      </Table>
    );
  }

  return (
    <Table headers={["Banner", "Target", "Vị trí", "Thời gian", "Trạng thái", ""]}>
      {(rows as BannerAdminItem[]).map((item) => (
        <tr key={item.bannerId} className="border-b border-slate-100">
          <Td><span className="font-black">{item.title}</span><p className="max-w-xs truncate text-xs text-slate-500">{item.imageUrl}</p></Td>
          <Td>{item.targetType} {item.targetId ? `#${item.targetId}` : ""}</Td>
          <Td>{item.position} · {item.displayOrder}</Td>
          <Td>{formatDate(item.startDate)} - {formatDate(item.endDate)}</Td>
          <Td>
            <button onClick={() => onBannerStatus(item.bannerId, !item.isActive)}>
              <StatusPill active={item.isActive} />
            </button>
          </Td>
          <ActionTd row={item} onEdit={onEdit} onDelete={onDelete} canDelete={canDelete} />
        </tr>
      ))}
    </Table>
  );
}

function Table({ headers, children }: { headers: string[]; children: React.ReactNode }) {
  return (
    <table className="w-full min-w-[760px] text-left text-sm">
      <thead className="bg-slate-50 text-xs uppercase tracking-wide text-slate-500">
        <tr>
          {headers.map((header) => (
            <th key={header} className="px-5 py-4 font-black">{header}</th>
          ))}
        </tr>
      </thead>
      <tbody>{children}</tbody>
    </table>
  );
}

function Td({ children }: { children: React.ReactNode }) {
  return <td className="px-5 py-4 align-middle text-slate-700">{children}</td>;
}

function ActionTd({
  row,
  onEdit,
  onDelete,
  canDelete,
}: {
  row: Row;
  onEdit?: (row: Row) => void;
  onDelete: (row: Row) => void;
  canDelete: boolean;
}) {
  return (
    <Td>
      <div className="flex justify-end gap-2">
        {onEdit && (
          <button onClick={() => onEdit(row)} className="rounded-lg border border-slate-200 px-3 py-2 text-xs font-black text-slate-700 hover:border-emerald-300 hover:text-emerald-700">
            Sửa
          </button>
        )}
        {canDelete && (
          <button onClick={() => onDelete(row)} className="flex h-9 w-9 items-center justify-center rounded-lg border border-red-200 text-red-700 hover:bg-red-50" aria-label="Xóa">
            <Trash2 className="h-4 w-4" />
          </button>
        )}
      </div>
    </Td>
  );
}

function StatusPill({ active }: { active: boolean }) {
  return (
    <span className={`inline-flex rounded-full px-3 py-1 text-xs font-black ring-1 ${active ? "bg-emerald-50 text-emerald-700 ring-emerald-100" : "bg-slate-100 text-slate-600 ring-slate-200"}`}>
      {active ? "Đang bật" : "Đã tắt"}
    </span>
  );
}

function getApiList(section: AdminSection): string[] {
  if (section === "products") return ["GET /api/products", "POST /api/products", "PUT /api/products/{id}", "DELETE /api/products/{id}", "GET /api/products/{id}/variants", "PUT /api/product-variants/{id}/stock"];
  if (section === "categories") return ["GET /api/categories", "POST /api/categories", "PUT /api/categories/{id}", "DELETE /api/categories/{id}"];
  if (section === "brands") return ["GET /api/brands", "POST /api/brands", "PUT /api/brands/{id}", "DELETE /api/brands/{id}"];
  if (section === "coupons") return ["GET /api/coupons", "POST /api/coupons", "PUT /api/coupons/{id}", "DELETE /api/coupons/{id}", "POST /api/coupons/apply"];
  if (section === "orders") return ["GET /api/orders", "GET /api/orders/{id}", "PUT /api/orders/{id}/status", "PUT /api/orders/{id}/cancel", "GET /api/payments/order/{orderId}", "GET /api/shipments/order/{orderId}"];
  if (section === "customers") return ["GET /api/User", "GET /api/User/{id}", "PUT /api/User/{id}/role", "PUT /api/User/{id}/status", "DELETE /api/User/{id}"];
  return ["GET /api/banners", "POST /api/banners", "PUT /api/banners/{id}", "PUT /api/banners/{id}/status", "DELETE /api/banners/{id}"];
}
