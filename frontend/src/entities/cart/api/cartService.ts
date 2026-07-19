import type { CartDto, CartItemDto, CartItemModel, CartModel } from "../model/types";
import {
  API_BASE_URL,
  API_ORIGIN_URL,
  FALLBACK_PRODUCT_IMAGE,
} from "@/shared/api/config";
import { getAccessToken } from "@/shared/api/authSession";

const FALLBACK_IMAGE = FALLBACK_PRODUCT_IMAGE;
const GUEST_CART_KEY = "guest_cart_items";

interface VariantDto {
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

interface ProductDto {
  productId: number;
  productName: string;
  imageUrl?: string | null;
}

type GuestCartItem = CartItemModel;

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

function getAuthHeaders(): HeadersInit | null {
  const token = getAccessToken();

  if (!token) {
    return null;
  }

  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
}

function normalizeImage(imageUrl?: string | null): string {
  if (!imageUrl) {
    return FALLBACK_IMAGE;
  }

  if (
    imageUrl.startsWith("http://") ||
    imageUrl.startsWith("https://") ||
    imageUrl.startsWith("/")
  ) {
    return imageUrl;
  }

  return `${API_ORIGIN_URL}/${imageUrl.replace(/^\/+/, "")}`;
}

function dispatchCartUpdated() {
  window.dispatchEvent(new Event("cart-updated"));
}

function readGuestItems(): GuestCartItem[] {
  if (typeof window === "undefined") {
    return [];
  }

  try {
    const raw = sessionStorage.getItem(GUEST_CART_KEY);
    const parsed = raw ? JSON.parse(raw) : [];
    return Array.isArray(parsed) ? parsed : [];
  } catch {
    return [];
  }
}

function writeGuestItems(items: GuestCartItem[]) {
  sessionStorage.setItem(GUEST_CART_KEY, JSON.stringify(items));
  dispatchCartUpdated();
}

function mapCartItem(item: CartItemDto): CartItemModel {
  const variantParts = [item.weight, item.gripSize, item.color].filter(Boolean);

  return {
    id: item.cartItemId,
    variantId: item.variantId,
    productId: item.productId,
    name: item.productName,
    image: normalizeImage(item.imageUrl),
    sku: item.sku,
    variantLabel: variantParts.length > 0 ? variantParts.join(" / ") : item.sku,
    price: item.price,
    quantity: item.quantity,
    stock: item.stockQuantity,
    subTotal: item.subTotal,
  };
}

function mapCart(cart: CartDto): CartModel {
  const items = cart.items.map(mapCartItem);

  return {
    id: cart.cartId,
    items,
    totalAmount: cart.totalAmount,
    totalQuantity: items.reduce((sum, item) => sum + item.quantity, 0),
  };
}

function mapGuestCart(items: GuestCartItem[]): CartModel {
  return {
    id: 0,
    items,
    totalAmount: items.reduce((sum, item) => sum + item.subTotal, 0),
    totalQuantity: items.reduce((sum, item) => sum + item.quantity, 0),
  };
}

async function publicRequestJson<T>(path: string): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response, "Khong the tai du lieu san pham"));
  }

  return response.json() as Promise<T>;
}

async function requestJson<T>(path: string, init?: RequestInit): Promise<T> {
  const headers = getAuthHeaders();

  if (!headers) {
    throw new Error("Vui long dang nhap de su dung gio hang tren tai khoan");
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      ...headers,
      ...(init?.headers ?? {}),
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response, "Khong the cap nhat gio hang"));
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

async function createGuestItem(variantId: number, quantity: number): Promise<GuestCartItem> {
  const variant = await publicRequestJson<VariantDto>(`/product-variants/${variantId}`);
  const product = await publicRequestJson<ProductDto>(`/products/${variant.productId}`);
  const variantParts = [variant.weight, variant.gripSize, variant.color].filter(Boolean);
  const safeQuantity = Math.max(1, Math.min(quantity, variant.stockQuantity));

  return {
    id: variant.variantId,
    variantId: variant.variantId,
    productId: variant.productId,
    name: product.productName,
    image: normalizeImage(variant.imageUrl ?? product.imageUrl),
    sku: variant.sku,
    variantLabel: variantParts.length > 0 ? variantParts.join(" / ") : variant.sku,
    price: variant.price,
    quantity: safeQuantity,
    stock: variant.stockQuantity,
    subTotal: variant.price * safeQuantity,
  };
}

export const cartService = {
  async getCart(): Promise<CartModel> {
    if (!getAccessToken()) {
      return mapGuestCart(readGuestItems());
    }

    const cart = await requestJson<CartDto>("/cart");
    return mapCart(cart);
  },

  async getCartCount(): Promise<number> {
    const cart = await this.getCart();
    return cart.totalQuantity;
  },

  async addItem(variantId: number, quantity: number): Promise<void> {
    if (!getAccessToken()) {
      const items = readGuestItems();
      const existing = items.find((item) => item.variantId === variantId);

      if (existing) {
        const nextQuantity = Math.min(existing.stock, existing.quantity + quantity);
        writeGuestItems(items.map((item) =>
          item.variantId === variantId
            ? { ...item, quantity: nextQuantity, subTotal: nextQuantity * item.price }
            : item
        ));
        return;
      }

      writeGuestItems([...items, await createGuestItem(variantId, quantity)]);
      return;
    }

    await requestJson("/cart/items", {
      method: "POST",
      body: JSON.stringify({ variantId, quantity }),
    });

    dispatchCartUpdated();
  },

  async updateItemQuantity(cartItemId: number, quantity: number): Promise<void> {
    if (!getAccessToken()) {
      const items = readGuestItems();
      writeGuestItems(items.map((item) => {
        if (item.id !== cartItemId) return item;
        const nextQuantity = Math.max(1, Math.min(quantity, item.stock));
        return { ...item, quantity: nextQuantity, subTotal: nextQuantity * item.price };
      }));
      return;
    }

    await requestJson(`/cart/items/${cartItemId}`, {
      method: "PUT",
      body: JSON.stringify({ quantity }),
    });

    dispatchCartUpdated();
  },

  async deleteItem(cartItemId: number): Promise<void> {
    if (!getAccessToken()) {
      writeGuestItems(readGuestItems().filter((item) => item.id !== cartItemId));
      return;
    }

    await requestJson(`/cart/items/${cartItemId}`, {
      method: "DELETE",
    });
    dispatchCartUpdated();
  },

  async clearCart(): Promise<void> {
    if (!getAccessToken()) {
      writeGuestItems([]);
      return;
    }

    await requestJson("/cart/clear", {
      method: "DELETE",
    });
    dispatchCartUpdated();
  },

  async syncGuestCartToServer(): Promise<void> {
    if (!getAccessToken()) {
      return;
    }

    const items = readGuestItems();
    if (items.length === 0) {
      return;
    }

    for (const item of items) {
      await requestJson("/cart/items", {
        method: "POST",
        body: JSON.stringify({ variantId: item.variantId, quantity: item.quantity }),
      });
    }

    writeGuestItems([]);
  },
};
