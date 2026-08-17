"use client";

import React, { useEffect, useState } from "react";
import Link from "next/link";
import { MainLayout } from "@/widgets/layout";
import { useAuth } from "@/features/auth";
import { addressService, type Address } from "@/entities/address";
import {
  CalendarDays,
  ChevronRight,
  Heart,
  LockKeyhole,
  LogOut,
  Mail,
  MapPin,
  PackageCheck,
  Pencil,
  Phone,
  Save,
  ShieldCheck,
  User,
  X,
  Plus,
} from "lucide-react";

const PROVINCES_API_V2 = "https://provinces.open-api.vn/api/v2";

interface WardApi {
  name: string;
  code: number;
}

interface ProvinceApi {
  name: string;
  code: number;
  wards?: WardApi[];
}

const quickLinks = [
  {
    label: "Lịch sử đơn hàng",
    description: "Theo dõi trạng thái và hóa đơn mua sắm",
    href: "/orders",
    icon: PackageCheck,
  },
  {
    label: "Sản phẩm yêu thích",
    description: "Danh sách vợt, giày và phụ kiện đã lưu",
    href: "/wishlist",
    icon: Heart,
  },
  {
    label: "Bảo mật tài khoản",
    description: "Mật khẩu, phiên đăng nhập và xác thực",
    href: "#security",
    icon: LockKeyhole,
  },
];

const initialAddressState = {
  recipientName: "",
  phone: "",
  province: "",
  ward: "",
  addressDetail: "",
  isDefault: false,
};

export default function ProfilePage() {
  const { user, loading, isAuthenticated, logout } = useAuth();

  const [isEditing, setIsEditing] = useState(false);
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [addressError, setAddressError] = useState<string | null>(null);

  const [profile, setProfile] = useState({
    fullName: "",
    email: "",
    phone: "",
    birthday: "",
  });

  // State điều khiển Modal địa chỉ
  const [showAddressForm, setShowAddressForm] = useState(false);
  const [editingAddressId, setEditingAddressId] = useState<number | null>(null);
  const [addressInput, setAddressInput] = useState(initialAddressState);

  // State lưu trữ dữ liệu hành chính v2: Tỉnh/Thành -> Phường/Xã.
  const [provinces, setProvinces] = useState<ProvinceApi[]>([]);
  const [wards, setWards] = useState<WardApi[]>([]);

  // Tải danh sách địa chỉ của user
  const loadAddresses = async () => {
    try {
      setAddressError(null);
      const data = await addressService.getAddresses();
      setAddresses(data);
    } catch (error) {
      setAddressError(error instanceof Error ? error.message : "Không thể tải danh sách địa chỉ");
    }
  };

  useEffect(() => {
    if (!isAuthenticated) return;
    let active = true;

    const loadInitialAddresses = async () => {
      try {
        const data = await addressService.getAddresses();
        if (active) {
          setAddressError(null);
          setAddresses(data);
        }
      } catch (error) {
        if (active) {
          setAddressError(error instanceof Error ? error.message : "Không thể tải danh sách địa chỉ");
        }
      }
    };

    void loadInitialAddresses();

    return () => {
      active = false;
    };
  }, [isAuthenticated]);

  // Fetch danh sách Tỉnh/Thành khi mở modal
  useEffect(() => {
    if (showAddressForm) {
      fetch(`${PROVINCES_API_V2}/`)
        .then((res) => res.json())
        .then((data) => setProvinces(data))
        .catch((err) => console.error("Lỗi tải danh sách Tỉnh/Thành:", err));
    }
  }, [showAddressForm]);

  // Hành động: Chọn Tỉnh/Thành
  const handleProvinceChange = async (provinceCode: string) => {
    const selected = provinces.find((p) => String(p.code) === provinceCode);
    setAddressInput((prev) => ({
      ...prev,
      province: selected ? selected.name : "",
      ward: "",
    }));
    setWards([]);

    if (provinceCode) {
      try {
        const res = await fetch(`${PROVINCES_API_V2}/p/${provinceCode}?depth=2`);
        const data = await res.json();
        setWards(data.wards || []);
      } catch (err) {
        setWards([]);
        console.error("Lỗi tải danh sách Phường/Xã:", err);
      }
    } else {
      setWards([]);
    }
  };

  // Mở form ở chế độ THÊM MỚI
  const handleOpenAddForm = () => {
    setEditingAddressId(null);
    setAddressInput(initialAddressState);
    setWards([]);
    setShowAddressForm(true);
  };

  // Mở form ở chế độ SỬA (Đổ ngược dữ liệu cũ đồng bộ API)
  const handleOpenEditForm = async (address: Address) => {
    setEditingAddressId(address.addressId);
    setAddressInput({
      recipientName: address.recipientName,
      phone: address.phone,
      province: address.province ?? "",
      ward: address.ward ?? "",
      addressDetail: address.addressDetail,
      isDefault: address.isDefault,
    });

    setShowAddressForm(true);

    try {
      // Tìm code của tỉnh hiện tại để fetch danh sách phường/xã theo API v2.
      const pRes = await fetch(`${PROVINCES_API_V2}/`);
      const pData = await pRes.json();
      setProvinces(pData);
      const currentProvince = pData.find((p: ProvinceApi) => p.name === address.province);

      if (currentProvince) {
        const wardRes = await fetch(`${PROVINCES_API_V2}/p/${currentProvince.code}?depth=2`);
        const wardData = await wardRes.json();
        setWards(wardData.wards || []);
      }
    } catch (error) {
      console.error("Lỗi đồng bộ cấu trúc dữ liệu sửa địa chỉ:", error);
    }
  };

  // Nhấn nút LƯU trên Modal
  const handleSaveAddress = async () => {
    try {
      setAddressError(null);
      if (editingAddressId) {
        if (addressService.updateAddress) {
          await addressService.updateAddress(editingAddressId, addressInput);
        } else {
          console.warn("addressService chưa có method updateAddress.");
        }
      } else {
        await addressService.createAddress(addressInput);
      }

      await loadAddresses();
      setShowAddressForm(false);
      setAddressInput(initialAddressState);
      setEditingAddressId(null);
    } catch (error) {
      setAddressError(error instanceof Error ? error.message : "Không thể lưu địa chỉ");
    }
  };

  const handleToggleProfileEdit = () => {
    if (!isEditing) {
      setProfile({
        fullName: user?.fullName ?? "",
        email: user?.email ?? "",
        phone: user?.phone ?? "",
        birthday: user?.birthday ?? "",
      });
    }

    setIsEditing((value) => !value);
  };

  if (loading) {
    return (
      <MainLayout>
        <div className="py-10">
          <div className="h-56 animate-pulse rounded-2xl bg-white shadow-sm" />
        </div>
      </MainLayout>
    );
  }

  if (!isAuthenticated) {
    return (
      <MainLayout>
        <section className="py-14">
          <div className="mx-auto max-w-7xl rounded-2xl border border-gray-200 bg-white p-8 text-center shadow-sm">
            <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl bg-emerald-50 text-emerald-700">
              <User className="h-8 w-8" />
            </div>
            <h1 className="mt-5 text-3xl font-bold text-gray-900">Đăng nhập để xem hồ sơ</h1>
            <p className="mt-3 text-gray-600">
              Quản lý thông tin cá nhân, địa chỉ giao hàng và lịch sử mua sắm FlyShot của bạn.
            </p>
            <div className="mt-7 flex flex-col justify-center gap-3 sm:flex-row">
              <Link href="/login" className="inline-flex h-12 items-center justify-center rounded-lg bg-emerald-600 px-6 font-semibold text-white transition-colors hover:bg-emerald-700">
                Đăng nhập
              </Link>
              <Link href="/register" className="inline-flex h-12 items-center justify-center rounded-lg border border-gray-300 bg-white px-6 font-semibold text-gray-700 transition-colors hover:border-emerald-500 hover:text-emerald-700">
                Tạo tài khoản
              </Link>
            </div>
          </div>
        </section>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <div className="py-8 text-gray-900 lg:py-10">
        <div className="grid gap-6 max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-6 lg:py-8 lg:grid-cols-[minmax(0,1fr)_360px]">
          <div className="space-y-6">
            {/* THÔNG TIN CÁ NHÂN */}
            <section className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
              <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                <div>
                  <h2 className="text-2xl font-bold text-gray-900">Thông tin cá nhân</h2>
                  <p className="mt-1 text-sm text-gray-500">Cập nhật thông tin liên hệ dùng cho đơn hàng và hỗ trợ.</p>
                </div>
                <button
                  onClick={handleToggleProfileEdit}
                  className="inline-flex h-10 items-center justify-center gap-2 rounded-lg border border-gray-300 bg-white px-4 text-sm font-semibold text-gray-700 transition hover:border-emerald-500 hover:text-emerald-700"
                >
                  {isEditing ? <X className="h-4 w-4" /> : <Pencil className="h-4 w-4" />}
                  {isEditing ? "Hủy" : "Chỉnh sửa"}
                </button>
              </div>

              <div className="grid gap-5 sm:grid-cols-2">
                <label className="space-y-2">
                  <span className="text-sm font-semibold text-gray-700">Họ và tên</span>
                  <div className="flex h-12 items-center gap-3 rounded-lg border border-gray-200 bg-gray-50 px-4">
                    <User className="h-5 w-5 text-gray-400" />
                    <input
                      value={isEditing ? profile.fullName : user?.fullName ?? ""}
                      disabled={!isEditing}
                      onChange={(event) => setProfile({ ...profile, fullName: event.target.value })}
                      className="min-w-0 flex-1 bg-transparent text-sm font-medium text-gray-900 outline-none disabled:text-gray-600"
                    />
                  </div>
                </label>

                <label className="space-y-2">
                  <span className="text-sm font-semibold text-gray-700">Email</span>
                  <div className="flex h-12 items-center gap-3 rounded-lg border border-gray-200 bg-gray-50 px-4">
                    <Mail className="h-5 w-5 text-gray-400" />
                    <input
                      value={isEditing ? profile.email : user?.email ?? ""}
                      disabled={!isEditing}
                      onChange={(event) => setProfile({ ...profile, email: event.target.value })}
                      className="min-w-0 flex-1 bg-transparent text-sm font-medium text-gray-900 outline-none disabled:text-gray-600"
                    />
                  </div>
                </label>

                <label className="space-y-2">
                  <span className="text-sm font-semibold text-gray-700">Số điện thoại</span>
                  <div className="flex h-12 items-center gap-3 rounded-lg border border-gray-200 bg-gray-50 px-4">
                    <Phone className="h-5 w-5 text-gray-400" />
                    <input
                      value={isEditing ? profile.phone : user?.phone ?? ""}
                      disabled={!isEditing}
                      placeholder="Thêm số điện thoại"
                      onChange={(event) => setProfile({ ...profile, phone: event.target.value })}
                      className="min-w-0 flex-1 bg-transparent text-sm font-medium text-gray-900 outline-none placeholder:text-gray-400 disabled:text-gray-600"
                    />
                  </div>
                </label>

                <label className="space-y-2">
                  <span className="text-sm font-semibold text-gray-700">Ngày sinh</span>
                  <div className="flex h-12 items-center gap-3 rounded-lg border border-gray-200 bg-gray-50 px-4">
                    <CalendarDays className="h-5 w-5 text-gray-400" />
                    <input
                      type="date"
                      value={isEditing ? profile.birthday : user?.birthday ?? ""}
                      disabled={!isEditing}
                      onChange={(event) => setProfile({ ...profile, birthday: event.target.value })}
                      className="min-w-0 flex-1 bg-transparent text-sm font-medium text-gray-900 outline-none disabled:text-gray-600"
                    />
                  </div>
                </label>
              </div>

              {isEditing && (
                <div className="mt-6 flex justify-end">
                  <button
                    onClick={() => setIsEditing(false)}
                    className="inline-flex h-11 items-center justify-center gap-2 rounded-lg bg-emerald-600 px-5 text-sm font-bold text-white transition hover:bg-emerald-700"
                  >
                    <Save className="h-4 w-4" />
                    Lưu thay đổi
                  </button>
                </div>
              )}
            </section>

            {/* ĐỊA CHỈ GIAO HÀNG */}
            <section className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
              <div className="mb-5 flex items-center justify-between gap-4">
                <div>
                  <h2 className="text-2xl font-bold text-gray-900">Địa chỉ giao hàng</h2>
                  <p className="mt-1 text-sm text-gray-500">Quản lý nơi nhận hàng thường dùng.</p>
                </div>
                <button
                  onClick={handleOpenAddForm}
                  className="inline-flex items-center gap-1.5 rounded-lg bg-emerald-50 px-4 py-2 text-sm font-bold text-emerald-700 transition hover:bg-emerald-100"
                >
                  <Plus className="h-4 w-4" />
                  Thêm mới
                </button>
              </div>

              <div className="space-y-3">
                {addressError ? (
                  <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm font-semibold text-red-700">
                    {addressError}
                  </div>
                ) : addresses.length === 0 ? (
                  <div className="rounded-xl border border-dashed border-gray-300 p-6 text-center text-sm text-gray-500">
                    Chưa có địa chỉ giao hàng nào
                  </div>
                ) : (
                  addresses.map((item) => (
                    <div key={item.addressId} className="rounded-2xl border border-gray-100 bg-gray-50 p-4 animate-fadeIn">
                      <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                        <div className="min-w-0">
                          <div className="flex flex-wrap items-center gap-2">
                            <h3 className="font-bold text-gray-900">{item.recipientName}</h3>
                            {item.isDefault && (
                              <span className="rounded-full bg-emerald-100 px-2.5 py-1 text-xs font-bold text-emerald-700">
                                Mặc định
                              </span>
                            )}
                          </div>
                          <p className="mt-2 text-sm font-semibold text-gray-700">{item.phone}</p>
                          <p className="mt-1 flex gap-2 text-sm leading-6 text-gray-500">
                            <MapPin className="mt-0.5 h-4 w-4 shrink-0 text-emerald-600" />
                            {[item.addressDetail, item.ward, item.province].filter(Boolean).join(", ")}
                          </p>
                        </div>
                        <div className="flex items-center gap-3"> 
                          <button
                            onClick={() => handleOpenEditForm(item)}
                            className="shrink-0 text-sm font-bold text-amber-800 bg-amber-50 border border-amber-300 px-3 py-1.5 rounded-md hover:bg-amber-100 transition"
                          >
                            Chỉnh sửa
                          </button>

                          <button
                            onClick={() => {
                              if (confirm("Bạn có chắc muốn xóa địa chỉ này?")) {
                                addressService.deleteAddress(item.addressId).then(loadAddresses);
                              }
                            }}
                            className="shrink-0 text-sm font-bold text-red-700 bg-red-50 border border-red-200 px-3 py-1.5 rounded-md hover:bg-red-100 transition"
                          >
                            Xóa
                          </button>
                        </div>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </section>
          </div>

          {/* SIDEBAR BÊN PHẢI */}
          <aside className="space-y-6">
            <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
              <h2 className="text-lg font-bold text-gray-900">Lối tắt tài khoản</h2>
              <div className="mt-4 space-y-3">
                {quickLinks.map((item) => {
                  const Icon = item.icon;
                  return (
                    <Link
                      key={item.label}
                      href={item.href}
                      className="group flex items-center justify-between gap-3 rounded-2xl border border-gray-100 bg-gray-50 p-4 transition hover:border-emerald-200 hover:bg-emerald-50"
                    >
                      <div className="flex min-w-0 items-center gap-3">
                        <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-white text-emerald-700 shadow-sm">
                          <Icon className="h-5 w-5" />
                        </span>
                        <span className="min-w-0">
                          <span className="block truncate text-sm font-bold text-gray-900">{item.label}</span>
                          <span className="mt-1 block truncate text-xs text-gray-500">{item.description}</span>
                        </span>
                      </div>
                      <ChevronRight className="h-5 w-5 shrink-0 text-gray-400 transition group-hover:translate-x-0.5 group-hover:text-emerald-600" />
                    </Link>
                  );
                })}
              </div>
            </section>

            <section id="security" className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
              <div className="flex items-start gap-4">
                <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-emerald-50 text-emerald-700">
                  <ShieldCheck className="h-6 w-6" />
                </div>
                <div>
                  <h2 className="text-lg font-bold text-gray-900">Bảo mật</h2>
                  <p className="mt-1 text-sm leading-6 text-gray-500">
                    Tài khoản đang được bảo vệ bằng phiên đăng nhập cá nhân. Hãy đổi mật khẩu định kỳ để giữ an toàn.
                  </p>
                </div>
              </div>
              <button className="mt-5 w-full rounded-lg border border-gray-300 bg-white px-4 py-3 text-sm font-bold text-gray-700 transition hover:border-emerald-500 hover:text-emerald-700">
                Đổi mật khẩu
              </button>
            </section>

            <button
              onClick={logout}
              className="flex w-full items-center justify-center gap-2 rounded-2xl border border-red-200 bg-red-50 px-5 py-4 text-sm font-bold text-red-700 transition hover:bg-red-100"
            >
              <LogOut className="h-5 w-5" />
              Đăng xuất
            </button>
          </aside>
        </div>
      </div>

      {/* MODAL DÙNG CHUNG TÍCH HỢP API CẤP HÀNH CHÍNH */}
      {showAddressForm && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6">
          <div className="fixed inset-0 bg-slate-900/60 backdrop-blur-sm transition-opacity" onClick={() => setShowAddressForm(false)} />

          <div className="relative w-full max-w-xl transform overflow-hidden rounded-2xl bg-white p-6 text-left shadow-xl transition-all sm:my-8 animate-scaleUp">
            <div className="flex items-center justify-between border-b border-gray-100 pb-4">
              <h3 className="text-xl font-bold text-gray-900">
                {editingAddressId ? "Cập nhật địa chỉ giao hàng" : "Thêm địa chỉ giao hàng mới"}
              </h3>
              <button onClick={() => setShowAddressForm(false)} className="rounded-lg p-1.5 text-gray-400 hover:bg-gray-100 hover:text-gray-700 transition">
                <X className="h-5 w-5" />
              </button>
            </div>

            <div className="mt-5 space-y-4">
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-1.5">
                  <label className="text-xs font-bold text-gray-700 uppercase tracking-wider">Tên người nhận</label>
                  <input
                    type="text"
                    placeholder="VD: Nguyễn Văn A"
                    value={addressInput.recipientName}
                    onChange={(e) => setAddressInput({ ...addressInput, recipientName: e.target.value })}
                    className="w-full rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 text-sm font-medium text-gray-900 outline-none transition focus:border-emerald-500 focus:bg-white focus:ring-2 focus:ring-emerald-500/10"
                  />
                </div>
                <div className="space-y-1.5">
                  <label className="text-xs font-bold text-gray-700 uppercase tracking-wider">Số điện thoại</label>
                  <input
                    type="tel"
                    placeholder="VD: 0912345678"
                    value={addressInput.phone}
                    onChange={(e) => setAddressInput({ ...addressInput, phone: e.target.value })}
                    className="w-full rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 text-sm font-medium text-gray-900 outline-none transition focus:border-emerald-500 focus:bg-white focus:ring-2 focus:ring-emerald-500/10"
                  />
                </div>
              </div>

              {/* DROPDOWN SELECT CẤP HÀNH CHÍNH TỰ ĐỘNG */}
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-1.5">
                  <label className="text-xs font-bold text-gray-700 uppercase tracking-wider">Tỉnh / Thành</label>
                  <select
                    value={provinces.find((p) => p.name === addressInput.province)?.code || ""}
                    onChange={(e) => handleProvinceChange(e.target.value)}
                    className="w-full rounded-xl border border-gray-200 bg-gray-50 px-3 py-3 text-sm font-medium text-gray-900 outline-none transition focus:border-emerald-500 focus:bg-white"
                  >
                    <option value="">-- Chọn Tỉnh/Thành --</option>
                    {provinces.map((p) => (
                      <option key={p.code} value={p.code}>{p.name}</option>
                    ))}
                  </select>
                </div>

                <div className="space-y-1.5">
                  <label className="text-xs font-bold text-gray-700 uppercase tracking-wider">Phường / Xã</label>
                  <select
                    disabled={!addressInput.province}
                    value={addressInput.ward}
                    onChange={(e) => setAddressInput({ ...addressInput, ward: e.target.value })}
                    className="w-full rounded-xl border border-gray-200 bg-gray-50 px-3 py-3 text-sm font-medium text-gray-900 outline-none transition focus:border-emerald-500 focus:bg-white disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    <option value="">-- Chọn Phường/Xã --</option>
                    {wards.map((w) => (
                      <option key={w.code} value={w.name}>{w.name}</option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="space-y-1.5">
                <label className="text-xs font-bold text-gray-700 uppercase tracking-wider">Địa chỉ chi tiết</label>
                <input
                  type="text"
                  placeholder="Số nhà, tên đường, tòa nhà..."
                  value={addressInput.addressDetail}
                  onChange={(e) => setAddressInput({ ...addressInput, addressDetail: e.target.value })}
                  className="w-full rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 text-sm font-medium text-gray-900 outline-none transition focus:border-emerald-500 focus:bg-white focus:ring-2 focus:ring-emerald-500/10"
                />
              </div>

              <label className="flex items-center gap-3 rounded-xl border border-gray-100 bg-gray-50 p-3 text-sm font-semibold text-gray-700 cursor-pointer transition hover:bg-gray-100 w-full select-none">
                <input
                  type="checkbox"
                  checked={addressInput.isDefault}
                  onChange={(e) => setAddressInput({ ...addressInput, isDefault: e.target.checked })}
                  className="h-4 w-4 rounded border-gray-300 text-emerald-600 focus:ring-emerald-500 accent-emerald-600"
                />
                <span>Đặt làm địa chỉ giao hàng mặc định</span>
              </label>
            </div>

            <div className="mt-6 flex items-center justify-end gap-3 border-t border-gray-100 pt-4">
              <button
                type="button"
                onClick={() => setShowAddressForm(false)}
                className="h-11 rounded-xl border border-gray-300 bg-white px-5 text-sm font-bold text-gray-700 transition hover:bg-gray-50"
              >
                Hủy bỏ
              </button>
              <button
                type="button"
                onClick={handleSaveAddress}
                className="h-11 rounded-xl bg-emerald-600 px-5 text-sm font-bold text-white shadow-sm transition hover:bg-emerald-700"
              >
                {editingAddressId ? "Cập nhật" : "Lưu địa chỉ"}
              </button>
            </div>
          </div>
        </div>
      )}
    </MainLayout>
  );
}
