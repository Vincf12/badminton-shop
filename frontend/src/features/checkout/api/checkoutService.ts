import { API_BASE_URL } from "@/shared/api/config";
import { getAccessToken } from "@/shared/api/authSession";

export interface CreateOrderPayload {
  addressId: number;
  couponCode?: string;
  paymentMethod: "COD";
}

export interface CreateOrderResponse {
  message: string;
  orderId: number;
  orderCode: string;
  totalAmount: number;
  shippingFee: number;
  discountAmount: number;
  finalAmount: number;
}

export interface CodPaymentResponse {
  message: string;
  payment: {
    paymentId: number;
    orderId: number;
    paymentMethod: string;
    paymentStatus: string;
    amount: number;
    transactionCode?: string | null;
    paidAt?: string | null;
    createdAt: string;
  };
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
    throw new Error("Vui lòng đăng nhập để thanh toán.");
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
    throw new Error(await readErrorMessage(response, "Không thể xử lý thanh toán."));
  }

  return response.json() as Promise<T>;
}

export const checkoutService = {
  createOrder(payload: CreateOrderPayload): Promise<CreateOrderResponse> {
    return requestJson<CreateOrderResponse>("/orders", {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  createCodPayment(orderId: number): Promise<CodPaymentResponse> {
    return requestJson<CodPaymentResponse>("/payments/cod/create", {
      method: "POST",
      body: JSON.stringify({ orderId }),
    });
  },
};
