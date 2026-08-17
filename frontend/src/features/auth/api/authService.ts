import { API_BASE_URL } from "@/shared/api/config";
import {
  getAccessToken,
  removeAccessToken,
  setAccessToken,
} from "@/shared/api/authSession";

const AUTH_API_URL = `${API_BASE_URL}/Auth`;
const ACCOUNT_API_URL = `${API_BASE_URL}/account`;

export interface RegisterData {
  fullName: string;
  email: string;
  password: string;
  phone?: string;
}

export interface LoginData {
  email: string;
  password: string;
}

export interface User {
  id: number | string;
  fullName: string;
  email: string;
  phone?: string;
  birthday?: string;
  role?: string;
}

export interface AuthResponse {
  access_token: string;
  token_type: string;
  user?: User;
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

type ApiObject = Record<string, unknown>;

function isObject(value: unknown): value is ApiObject {
  return typeof value === "object" && value !== null;
}

function readString(value: unknown): string | undefined {
  return typeof value === "string" ? value : undefined;
}

function decodeJwtPayload(token: string): ApiObject {
  try {
    const payload = token.split(".")[1];
    const normalizedPayload = payload.replace(/-/g, "+").replace(/_/g, "/");
    const json = atob(normalizedPayload);
    const parsed = JSON.parse(json);
    return isObject(parsed) ? parsed : {};
  } catch {
    return {};
  }
}

function readRoleFromToken(token: string): string | undefined {
  const payload = decodeJwtPayload(token);
  return (
    readString(payload.role) ??
    readString(payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"])
  );
}

function formatDateInput(value?: string) {
  if (!value) return "";
  return value.split("T")[0];
}

function mapUser(data: unknown): User {
  const dataObject = isObject(data) ? data : {};
  const nestedSource = dataObject.currentUser ?? dataObject.user ?? dataObject;
  const source = isObject(nestedSource) ? nestedSource : {};
  const id = source.id ?? source.userId ?? source.user_id ?? "";
  const email = readString(source.email) ?? "";
  const fullName = readString(source.fullName) ?? readString(source.full_name) ?? email ?? "Khách hàng";

  return {
    id: typeof id === "number" || typeof id === "string" ? id : "",
    fullName,
    email,
    phone: readString(source.phone),
    role: readString(source.role),
    birthday: formatDateInput(
      readString(source.birthdate) ??
      readString(source.birthDate) ??
      readString(source.birthday)
    ),
  };
}

class AuthService {
  async register(data: RegisterData): Promise<{ message: string }> {
    const response = await fetch(`${AUTH_API_URL}/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      throw new Error(await readErrorMessage(response, "Đăng ký thất bại"));
    }

    return response.json();
  }

  async login(data: LoginData): Promise<AuthResponse> {
    const response = await fetch(`${AUTH_API_URL}/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ email: data.email, password: data.password }),
    });

    if (!response.ok) {
      throw new Error(await readErrorMessage(response, "Đăng nhập thất bại"));
    }

    const json = await response.json();

    return {
      access_token: json.token,
      token_type: "Bearer",
      user: mapUser(json.user),
    };
  }

  async getCurrentUser(token: string): Promise<User> {
    const response = await fetch(`${ACCOUNT_API_URL}/me`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    });

    if (!response.ok) {
      throw new Error(await readErrorMessage(response, "Không thể lấy thông tin người dùng"));
    }

    const user = mapUser(await response.json());
    return {
      ...user,
      role: user.role ?? readRoleFromToken(token),
    };
  }

  setToken(token: string): void {
    setAccessToken(token);
  }

  getToken(): string | null {
    return getAccessToken();
  }

  removeToken(): void {
    removeAccessToken();
  }

  isAuthenticated(): boolean {
    return Boolean(this.getToken());
  }
}

export const authService = new AuthService();
