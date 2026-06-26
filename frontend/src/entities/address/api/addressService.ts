import { API_BASE_URL } from "@/shared/api/config";
import { getAccessToken } from "@/shared/api/authSession";

const ADDRESS_API_URL = `${API_BASE_URL}/addresses`;

export interface Address {
  addressId: number;
  recipientName: string;
  phone: string;
  province?: string | null;
  ward?: string | null;
  addressDetail: string;
  isDefault: boolean;
}

export interface AddressUpsertData {
  recipientName: string;
  phone: string;
  province?: string;
  ward?: string;
  addressDetail: string;
  isDefault: boolean;
}

class AddressService {
  private getAuthHeaders(): HeadersInit | null {
    const token = getAccessToken();

    if (!token) {
      return null;
    }

    return {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    };
  }

  private async readErrorMessage(response: Response, fallback: string): Promise<string> {
    const contentType = response.headers.get("content-type") ?? "";

    try {
      if (contentType.includes("application/json")) {
        const errorData = await response.json();
        return errorData?.message || errorData?.detail || fallback;
      }

      return (await response.text()) || fallback;
    } catch {
      return fallback;
    }
  }

  async getAddresses(): Promise<Address[]> {
    const headers = this.getAuthHeaders();

    if (!headers) {
      return [];
    }

    const response = await fetch(ADDRESS_API_URL, {
      method: "GET",
      headers,
      cache: "no-store",
    });

    if (!response.ok) {
      throw new Error(await this.readErrorMessage(response, "Không thể tải danh sách địa chỉ"));
    }

    return response.json();
  }

  async createAddress(data: AddressUpsertData): Promise<{ message: string; addressId: number }> {
    const headers = this.getAuthHeaders();

    if (!headers) {
      throw new Error("Bạn cần đăng nhập để thêm địa chỉ");
    }

    const response = await fetch(ADDRESS_API_URL, {
      method: "POST",
      headers,
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      throw new Error(await this.readErrorMessage(response, "Không thể thêm địa chỉ"));
    }

    return response.json();
  }

  async updateAddress(id: number, data: AddressUpsertData): Promise<{ message: string }> {
    const headers = this.getAuthHeaders();

    if (!headers) {
      throw new Error("Bạn cần đăng nhập để cập nhật địa chỉ");
    }

    const response = await fetch(`${ADDRESS_API_URL}/${id}`, {
      method: "PUT",
      headers,
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      throw new Error(await this.readErrorMessage(response, "Không thể cập nhật địa chỉ"));
    }

    return response.json();
  }

  async deleteAddress(id: number): Promise<{ message: string }> {
    const headers = this.getAuthHeaders();

    if (!headers) {
      throw new Error("Bạn cần đăng nhập để xóa địa chỉ");
    }

    const response = await fetch(`${ADDRESS_API_URL}/${id}`, {
      method: "DELETE",
      headers,
    });

    if (!response.ok) {
      throw new Error(await this.readErrorMessage(response, "Không thể xóa địa chỉ"));
    }

    return response.json();
  }

  async setDefaultAddress(id: number): Promise<{ message: string }> {
    const headers = this.getAuthHeaders();

    if (!headers) {
      throw new Error("Bạn cần đăng nhập để đặt địa chỉ mặc định");
    }

    const response = await fetch(`${ADDRESS_API_URL}/${id}/default`, {
      method: "PUT",
      headers,
    });

    if (!response.ok) {
      throw new Error(await this.readErrorMessage(response, "Không thể đặt địa chỉ mặc định"));
    }

    return response.json();
  }
}

export const addressService = new AddressService();
