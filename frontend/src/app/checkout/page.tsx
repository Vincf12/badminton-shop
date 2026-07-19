"use client";

import React, { useCallback, useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import {
  CheckCircle2,
  CreditCard,
  Loader2,
  MapPin,
  PackageCheck,
  Plus,
  ShoppingBag,
} from "lucide-react";
import { MainLayout } from "@/widgets/layout";
import { useAuth } from "@/features/auth";
import { cartService, type CartItemModel } from "@/features/cart";
import { addressService, type Address, type AddressUpsertData } from "@/entities/address";
import { checkoutService, type CreateOrderResponse } from "@/features/checkout";

const emptyAddressForm: AddressUpsertData = {
  recipientName: "",
  phone: "",
  province: "",
  ward: "",
  addressDetail: "",
  isDefault: true,
};

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

function formatCurrency(value: number): string {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    maximumFractionDigits: 0,
  }).format(value);
}

export default function CheckoutPage() {
  const router = useRouter();
  const { isAuthenticated, loading: authLoading } = useAuth();
  const [items, setItems] = useState<CartItemModel[]>([]);
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [selectedAddressId, setSelectedAddressId] = useState<number | null>(null);
  const [addressForm, setAddressForm] = useState<AddressUpsertData>(emptyAddressForm);
  const [provinces, setProvinces] = useState<ProvinceApi[]>([]);
  const [wards, setWards] = useState<WardApi[]>([]);
  const [couponCode, setCouponCode] = useState("");
  const [loading, setLoading] = useState(true);
  const [placingOrder, setPlacingOrder] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [order, setOrder] = useState<CreateOrderResponse | null>(null);

  const total = useMemo(() => {
    return items.reduce((sum, item) => sum + item.subTotal, 0);
  }, [items]);

  const loadCheckout = useCallback(async () => {
    if (!isAuthenticated) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const [cart, nextAddresses] = await Promise.all([
        cartService.getCart(),
        addressService.getAddresses(),
      ]);
      setItems(cart.items);
      setAddresses(nextAddresses);
      setSelectedAddressId(
        nextAddresses.find((item) => item.isDefault)?.addressId ??
        nextAddresses[0]?.addressId ??
        null
      );
    } catch (loadError) {
      setError(loadError instanceof Error ? loadError.message : "Không thể tải thông tin thanh toán.");
    } finally {
      setLoading(false);
    }
  }, [isAuthenticated]);

  useEffect(() => {
    if (!authLoading) {
      void loadCheckout();
    }
  }, [authLoading, loadCheckout]);

  useEffect(() => {
    if (!isAuthenticated) {
      return;
    }

    let active = true;

    const loadProvinces = async () => {
      try {
        const response = await fetch(`${PROVINCES_API_V2}/`);
        const data = await response.json();

        if (active) {
          setProvinces(Array.isArray(data) ? data : []);
        }
      } catch {
        if (active) {
          setProvinces([]);
        }
      }
    };

    void loadProvinces();

    return () => {
      active = false;
    };
  }, [isAuthenticated]);

  const updateAddressForm = (key: keyof AddressUpsertData, value: string | boolean) => {
    setAddressForm((current) => ({ ...current, [key]: value }));
  };

  const handleProvinceChange = async (provinceCode: string) => {
    const selectedProvince = provinces.find((province) => String(province.code) === provinceCode);

    setAddressForm((current) => ({
      ...current,
      province: selectedProvince?.name ?? "",
      ward: "",
    }));
    setWards([]);

    if (!provinceCode) {
      return;
    }

    try {
      const response = await fetch(`${PROVINCES_API_V2}/p/${provinceCode}?depth=2`);
      const data = await response.json();
      setWards(Array.isArray(data?.wards) ? data.wards : []);
    } catch {
      setWards([]);
    }
  };

  const createAddress = async () => {
    try {
      setError(null);
      const result = await addressService.createAddress(addressForm);
      setAddressForm(emptyAddressForm);
      setWards([]);
      const nextAddresses = await addressService.getAddresses();
      setAddresses(nextAddresses);
      setSelectedAddressId(result.addressId);
    } catch (addressError) {
      setError(addressError instanceof Error ? addressError.message : "Không thể tạo địa chỉ giao hàng.");
    }
  };

  const placeOrder = async () => {
    if (!selectedAddressId) {
      setError("Vui lòng chọn hoặc thêm địa chỉ giao hàng.");
      return;
    }

    if (items.length === 0) {
      setError("Giỏ hàng đang trống.");
      return;
    }

    try {
      setPlacingOrder(true);
      setError(null);
      const createdOrder = await checkoutService.createOrder({
        addressId: selectedAddressId,
        couponCode: couponCode.trim() || undefined,
        paymentMethod: "COD",
      });

      await checkoutService.createCodPayment(createdOrder.orderId);
      setOrder(createdOrder);
      setItems([]);
      window.dispatchEvent(new Event("cart-updated"));
    } catch (checkoutError) {
      setError(checkoutError instanceof Error ? checkoutError.message : "Không thể đặt hàng COD.");
    } finally {
      setPlacingOrder(false);
    }
  };

  if (authLoading || loading) {
    return (
      <MainLayout>
        <div className="flex min-h-[420px] items-center justify-center text-sm font-bold text-slate-500">
          <Loader2 className="mr-2 h-5 w-5 animate-spin" />
          Đang tải thanh toán...
        </div>
      </MainLayout>
    );
  }

  if (!isAuthenticated) {
    return (
      <MainLayout>
        <section className="mx-auto max-w-2xl px-4 py-16 text-center text-slate-900">
          <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl bg-emerald-50 text-emerald-700">
            <ShoppingBag className="h-8 w-8" />
          </div>
          <h1 className="mt-5 text-3xl font-black">Đăng nhập để thanh toán</h1>
          <p className="mt-3 text-slate-600">
            Giỏ hàng của bạn đang được lưu trong phiên hiện tại. Sau khi đăng nhập, hệ thống sẽ đồng bộ giỏ hàng vào tài khoản.
          </p>
          <Link
            href="/login"
            className="mt-7 inline-flex h-12 items-center justify-center rounded-xl bg-emerald-600 px-6 font-bold text-white hover:bg-emerald-700"
          >
            Đăng nhập
          </Link>
        </section>
      </MainLayout>
    );
  }

  if (order) {
    return (
      <MainLayout>
        <section className="mx-auto max-w-3xl px-4 py-16 text-center text-slate-900">
          <CheckCircle2 className="mx-auto h-16 w-16 text-emerald-600" />
          <h1 className="mt-5 text-3xl font-black">Đặt hàng COD thành công</h1>
          <p className="mt-3 text-slate-600">
            Mã đơn hàng <span className="font-black text-slate-950">{order.orderCode}</span>. Bạn sẽ thanh toán khi nhận hàng.
          </p>
          <p className="mt-2 text-xl font-black text-emerald-700">{formatCurrency(order.finalAmount)}</p>
          <div className="mt-8 flex flex-col justify-center gap-3 sm:flex-row">
            <Link href="/shop" className="inline-flex h-12 items-center justify-center rounded-xl bg-emerald-600 px-6 font-bold text-white hover:bg-emerald-700">
              Tiếp tục mua sắm
            </Link>
            <button onClick={() => router.push("/profile")} className="inline-flex h-12 items-center justify-center rounded-xl border border-slate-300 px-6 font-bold text-slate-700 hover:border-emerald-400 hover:text-emerald-700">
              Xem tài khoản
            </button>
          </div>
        </section>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <div className="mx-auto max-w-7xl px-4 py-8 text-slate-900 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="text-4xl font-black">Thanh toán</h1>
          <p className="mt-2 text-slate-600">Hoàn tất đơn hàng với phương thức thanh toán khi nhận hàng.</p>
        </div>

        {error && (
          <div className="mb-6 rounded-2xl border border-red-200 bg-red-50 p-4 text-sm font-bold text-red-700">
            {error}
          </div>
        )}

        {items.length === 0 ? (
          <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center">
            <PackageCheck className="mx-auto h-10 w-10 text-slate-400" />
            <h2 className="mt-4 text-2xl font-black">Giỏ hàng trống</h2>
            <Link href="/shop" className="mt-6 inline-flex h-12 items-center justify-center rounded-xl bg-emerald-600 px-6 font-bold text-white">
              Mua sản phẩm
            </Link>
          </div>
        ) : (
          <div className="grid gap-8 lg:grid-cols-[minmax(0,1fr)_380px]">
            <div className="space-y-6">
              <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
                <div className="mb-5 flex items-center gap-3">
                  <MapPin className="h-5 w-5 text-emerald-600" />
                  <h2 className="text-xl font-black">Địa chỉ giao hàng</h2>
                </div>

                <div className="space-y-3">
                  {addresses.map((address) => (
                    <label key={address.addressId} className="flex cursor-pointer gap-3 rounded-2xl border border-slate-200 p-4 hover:border-emerald-300">
                      <input
                        type="radio"
                        name="address"
                        checked={selectedAddressId === address.addressId}
                        onChange={() => setSelectedAddressId(address.addressId)}
                        className="mt-1 h-4 w-4 accent-emerald-600"
                      />
                      <span>
                        <span className="font-black">{address.recipientName}</span>
                        <span className="ml-2 text-sm font-semibold text-slate-500">{address.phone}</span>
                        <span className="mt-1 block text-sm text-slate-600">
                          {[address.addressDetail, address.ward, address.province].filter(Boolean).join(", ")}
                        </span>
                      </span>
                    </label>
                  ))}
                </div>

                <div className="mt-6 rounded-2xl bg-slate-50 p-4">
                  <div className="mb-4 flex items-center gap-2 font-black">
                    <Plus className="h-4 w-4 text-emerald-600" />
                    Thêm địa chỉ nhanh
                  </div>
                  <div className="grid gap-3 sm:grid-cols-2">
                    <Input label="Người nhận" value={addressForm.recipientName} onChange={(value) => updateAddressForm("recipientName", value)} />
                    <Input label="Số điện thoại" value={addressForm.phone} onChange={(value) => updateAddressForm("phone", value)} />
                    <Select
                      label="Tỉnh/Thành"
                      value={provinces.find((province) => province.name === addressForm.province)?.code.toString() ?? ""}
                      onChange={handleProvinceChange}
                    >
                      <option value="">Chọn Tỉnh/Thành</option>
                      {provinces.map((province) => (
                        <option key={province.code} value={province.code}>
                          {province.name}
                        </option>
                      ))}
                    </Select>
                    <Select
                      label="Phường/Xã"
                      value={addressForm.ward ?? ""}
                      onChange={(value) => updateAddressForm("ward", value)}
                      disabled={!addressForm.province}
                    >
                      <option value="">Chọn Phường/Xã</option>
                      {wards.map((ward) => (
                        <option key={ward.code} value={ward.name}>
                          {ward.name}
                        </option>
                      ))}
                    </Select>
                    <div className="sm:col-span-2">
                      <Input label="Địa chỉ chi tiết" value={addressForm.addressDetail} onChange={(value) => updateAddressForm("addressDetail", value)} />
                    </div>
                  </div>
                  <button
                    onClick={createAddress}
                    className="mt-4 h-11 rounded-xl bg-slate-900 px-5 text-sm font-bold text-white hover:bg-slate-800"
                  >
                    Lưu địa chỉ
                  </button>
                </div>
              </section>

              <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
                <div className="mb-5 flex items-center gap-3">
                  <CreditCard className="h-5 w-5 text-emerald-600" />
                  <h2 className="text-xl font-black">Phương thức thanh toán</h2>
                </div>
                <div className="rounded-2xl border-2 border-emerald-500 bg-emerald-50 p-4">
                  <p className="font-black text-emerald-800">COD - Thanh toán khi nhận hàng</p>
                  <p className="mt-1 text-sm text-emerald-700">Bạn kiểm tra hàng và thanh toán trực tiếp cho đơn vị vận chuyển.</p>
                </div>
              </section>
            </div>

            <aside className="h-fit rounded-2xl border border-slate-200 bg-white p-6 shadow-sm lg:sticky lg:top-24">
              <h2 className="text-xl font-black">Tóm tắt đơn hàng</h2>
              <div className="mt-5 space-y-4">
                {items.map((item) => (
                  <div key={item.id} className="flex justify-between gap-4 border-b border-slate-100 pb-3 text-sm">
                    <div>
                      <p className="font-bold">{item.name}</p>
                      <p className="text-slate-500">{item.variantLabel} · x{item.quantity}</p>
                    </div>
                    <span className="font-black">{formatCurrency(item.subTotal)}</span>
                  </div>
                ))}
              </div>

              <label className="mt-5 block">
                <span className="text-sm font-bold text-slate-600">Mã giảm giá</span>
                <input
                  value={couponCode}
                  onChange={(event) => setCouponCode(event.target.value)}
                  className="mt-2 h-11 w-full rounded-xl border border-slate-200 px-3 text-sm font-semibold outline-none focus:border-emerald-400"
                  placeholder="Nhập mã nếu có"
                />
              </label>

              <div className="mt-6 space-y-3 border-t border-slate-100 pt-4">
                <div className="flex justify-between text-sm">
                  <span>Tạm tính</span>
                  <span className="font-bold">{formatCurrency(total)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span>Phí vận chuyển</span>
                  <span className="font-bold text-emerald-600">Miễn phí</span>
                </div>
                <div className="flex justify-between text-xl font-black">
                  <span>Tổng cộng</span>
                  <span className="text-emerald-700">{formatCurrency(total)}</span>
                </div>
              </div>

              <button
                onClick={placeOrder}
                disabled={placingOrder}
                className="mt-6 inline-flex h-12 w-full items-center justify-center gap-2 rounded-xl bg-emerald-600 px-5 text-base font-black text-white hover:bg-emerald-700 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {placingOrder ? <Loader2 className="h-5 w-5 animate-spin" /> : <PackageCheck className="h-5 w-5" />}
                Đặt hàng COD
              </button>
            </aside>
          </div>
        )}
      </div>
    </MainLayout>
  );
}

function Input({ label, value, onChange }: { label: string; value: string; onChange: (value: string) => void }) {
  return (
    <label className="block">
      <span className="text-xs font-black uppercase tracking-wide text-slate-500">{label}</span>
      <input
        value={value}
        onChange={(event) => onChange(event.target.value)}
        className="mt-1 h-11 w-full rounded-xl border border-slate-200 bg-white px-3 text-sm font-semibold outline-none focus:border-emerald-400"
      />
    </label>
  );
}

function Select({
  label,
  value,
  onChange,
  children,
  disabled,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  children: React.ReactNode;
  disabled?: boolean;
}) {
  return (
    <label className="block">
      <span className="text-xs font-black uppercase tracking-wide text-slate-500">{label}</span>
      <select
        value={value}
        disabled={disabled}
        onChange={(event) => onChange(event.target.value)}
        className="mt-1 h-11 w-full rounded-xl border border-slate-200 bg-white px-3 text-sm font-semibold outline-none focus:border-emerald-400 disabled:cursor-not-allowed disabled:opacity-60"
      >
        {children}
      </select>
    </label>
  );
}
