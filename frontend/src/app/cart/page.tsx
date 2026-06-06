"use client";

import React, { useCallback, useEffect, useState } from "react";
import MainLayout from "@/app/components/layout/MainLayout";
import Image from "next/image";
import Link from "next/link";
import { Trash2, Plus, Minus } from "lucide-react";
import { cartService } from "@/services/cartService";
import type { CartItemModel } from "@/types/cart";

export default function CartPage() {
  const [cartItems, setCartItems] = useState<CartItemModel[]>([]);
  const [loading, setLoading] = useState(true);
  const [updatingItemId, setUpdatingItemId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const loadCart = useCallback(async () => {
    try {
      setError(null);
      const cart = await cartService.getCart();
      setCartItems(cart.items);
    } catch (loadError) {
      setError(loadError instanceof Error ? loadError.message : "Không thể tải giỏ hàng");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadCart();
  }, [loadCart]);

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price);
  };

  const totalPrice = cartItems.reduce(
    (total, item) => total + item.price * item.quantity,
    0
  );

  const updateQuantity = async (item: CartItemModel, quantity: number) => {
    if (quantity < 1 || quantity > item.stock) {
      return;
    }

    setUpdatingItemId(item.id);

    try {
      await cartService.updateItemQuantity(item.id, quantity);
      setCartItems((items) =>
        items.map((cartItem) =>
          cartItem.id === item.id ? { ...cartItem, quantity, subTotal: quantity * cartItem.price } : cartItem
        )
      );
      window.dispatchEvent(new Event("cart-updated"));
    } catch (updateError) {
      setError(updateError instanceof Error ? updateError.message : "Không thể cập nhật giỏ hàng");
    } finally {
      setUpdatingItemId(null);
    }
  };

  const deleteItem = async (itemId: number) => {
    setUpdatingItemId(itemId);

    try {
      await cartService.deleteItem(itemId);
      setCartItems((items) => items.filter((item) => item.id !== itemId));
      window.dispatchEvent(new Event("cart-updated"));
    } catch (deleteError) {
      setError(deleteError instanceof Error ? deleteError.message : "Không thể xóa sản phẩm");
    } finally {
      setUpdatingItemId(null);
    }
  };

  return (
    <MainLayout>
      <div className="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 lg:py-8">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-800 mb-4">Giỏ hàng</h1>
          <p className="text-gray-600">
            Kiểm tra và thanh toán đơn hàng của bạn
          </p>
        </div>

        {loading ? (
          <div className="rounded-2xl border border-dashed border-gray-300 bg-white p-8 text-center text-gray-600">
            Đang tải giỏ hàng...
          </div>
        ) : error ? (
          <div className="rounded-2xl border border-red-200 bg-red-50 p-8 text-center text-red-700">
            <p>{error}</p>
            {error.includes("đăng nhập") ? (
              <Link
                href="/login"
                className="mt-4 inline-block bg-emerald-600 hover:bg-emerald-700 text-white px-8 py-3 rounded-lg font-semibold transition-colors duration-300"
              >
                Đăng nhập
              </Link>
            ) : null}
          </div>
        ) : cartItems.length === 0 ? (
          <div className="text-center py-16">
            <div className="text-6xl mb-4">🛒</div>
            <h2 className="text-2xl font-bold text-gray-800 mb-2">
              Giỏ hàng trống
            </h2>
            <p className="text-gray-600 mb-6">
              Hãy thêm sản phẩm vào giỏ hàng để tiếp tục mua sắm
            </p>
            <Link
              href="/shop"
              className="inline-block bg-emerald-600 hover:bg-emerald-700 text-white px-8 py-3 rounded-lg font-semibold transition-colors duration-300"
            >
              Khám phá sản phẩm
            </Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 text-black">
            <div className="lg:col-span-2">
              <div className="bg-white rounded-2xl shadow-md overflow-hidden">
                {cartItems.map((item) => (
                  <div
                    key={item.id}
                    className="p-6 border-b border-gray-100 last:border-b-0"
                  >
                    <div className="flex flex-col sm:flex-row sm:items-center gap-6">
                      <Link href={`/product/${item.productId}`} className="relative w-24 h-24 rounded-lg overflow-hidden bg-gray-50 shrink-0">
                        <Image
                          src={item.image}
                          alt={item.name}
                          fill
                          className="object-contain p-2"
                        />
                      </Link>
                      <div className="flex-1">
                        <h3 className="text-xl font-bold text-gray-800 mb-1">
                          {item.name}
                        </h3>
                        <p className="text-sm text-gray-500 mb-2">{item.variantLabel}</p>
                        <p className="text-2xl font-bold text-emerald-600 mb-4">
                          {formatPrice(item.price)}
                        </p>
                        <div className="flex items-center gap-4">
                          <div className="flex items-center gap-2">
                            <button
                              onClick={() => updateQuantity(item, item.quantity - 1)}
                              disabled={updatingItemId === item.id || item.quantity <= 1}
                              className="w-8 h-8 rounded-full bg-gray-100 hover:bg-gray-200 disabled:opacity-50 flex items-center justify-center"
                            >
                              <Minus className="w-4 h-4" />
                            </button>
                            <span className="w-8 text-center font-semibold">
                              {item.quantity}
                            </span>
                            <button
                              onClick={() => updateQuantity(item, item.quantity + 1)}
                              disabled={updatingItemId === item.id || item.quantity >= item.stock}
                              className="w-8 h-8 rounded-full bg-gray-100 hover:bg-gray-200 disabled:opacity-50 flex items-center justify-center"
                            >
                              <Plus className="w-4 h-4" />
                            </button>
                          </div>
                          <button
                            onClick={() => deleteItem(item.id)}
                            disabled={updatingItemId === item.id}
                            className="text-red-500 hover:text-red-700 disabled:opacity-50 p-2"
                          >
                            <Trash2 className="w-5 h-5" />
                          </button>
                        </div>
                      </div>
                      <div className="text-right font-semibold text-gray-800">
                        {formatPrice(item.subTotal)}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            <div className="lg:col-span-1">
              <div className="bg-white rounded-2xl shadow-md p-6 sticky top-24">
                <h2 className="text-2xl font-bold text-gray-800 mb-6">
                  Tóm tắt đơn hàng
                </h2>
                <div className="space-y-4 mb-6">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Tạm tính:</span>
                    <span className="font-semibold text-orange-950">
                      {formatPrice(totalPrice)}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Phí vận chuyển:</span>
                    <span className="font-semibold text-emerald-600">
                      Miễn phí
                    </span>
                  </div>
                  <div className="border-t pt-4">
                    <div className="flex justify-between text-xl font-bold text-blue-600">
                      <span>Tổng cộng:</span>
                      <span className="text-emerald-600">
                        {formatPrice(totalPrice)}
                      </span>
                    </div>
                  </div>
                </div>
                <button className="w-full bg-emerald-600 hover:bg-emerald-700 text-white py-4 rounded-xl font-bold text-lg transition-colors duration-300">
                  Thanh toán
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </MainLayout>
  );
}
