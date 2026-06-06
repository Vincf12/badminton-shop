import { API_BASE_URL, FALLBACK_IMAGE } from "./productService";
import { authService } from "./authService";
import type { CartDto, CartItemDto, CartItemModel, CartModel } from "@/types/cart";

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
    throw new Error("Vui lòng đăng nhập để sử dụng giỏ hàng");
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

  return `${API_BASE_URL.replace(/\/api$/, "")}/${imageUrl.replace(/^\/+/, "")}`;
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
    throw new Error(await readErrorMessage(response, "Không thể cập nhật giỏ hàng"));
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export const cartService = {
  async getCart(): Promise<CartModel> {
    const cart = await requestJson<CartDto>("/cart");
    return mapCart(cart);
  },

  async getCartCount(): Promise<number> {
    const cart = await this.getCart();
    return cart.totalQuantity;
  },

  async addItem(variantId: number, quantity: number): Promise<void> {
    await requestJson("/cart/items", {
      method: "POST",
      body: JSON.stringify({ variantId, quantity }),
    });

    window.dispatchEvent(new Event("cart-updated"));
  },

  async updateItemQuantity(cartItemId: number, quantity: number): Promise<void> {
    await requestJson(`/cart/items/${cartItemId}`, {
      method: "PUT",
      body: JSON.stringify({ quantity }),
    });

    window.dispatchEvent(new Event("cart-updated"));
  },

  async deleteItem(cartItemId: number): Promise<void> {
    await requestJson(`/cart/items/${cartItemId}`, {
      method: "DELETE",
    });
    window.dispatchEvent(new Event("cartUpdated"));
  },

  async clearCart(): Promise<void> {
    await requestJson("/cart/clear", {
      method: "DELETE",
    });
    window.dispatchEvent(new Event("cartUpdated"));
  },
};