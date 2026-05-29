const API_BASE_URL = 'http://localhost:5211/api/Auth';

export interface RegisterData {
  username: string;
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
  user_id: number;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  role: string;
  created_at: string;
}

export interface AuthResponse {
  access_token: string;
  token_type: string;
}

class AuthService {
  async register(data: RegisterData): Promise<User> {
    try {
      const response = await fetch(`${API_BASE_URL}/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        let errMsg = 'Đăng ký thất bại';
        try {
          const ct = response.headers.get('content-type') || '';
          if (ct.includes('application/json')) {
            const errorData = await response.json();
            // Handle ASP.NET Core validation errors shape: { errors: { Field: ["..."] } }
            if (errorData?.errors && typeof errorData.errors === 'object') {
              const msgs = Object.values(errorData.errors).flat().filter(Boolean);
              errMsg = msgs.join('; ') || errMsg;
            } else {
              errMsg = errorData?.detail || errorData?.message || JSON.stringify(errorData) || errMsg;
            }
          } else {
            const text = await response.text();
            if (text) errMsg = text;
          }
        } catch (e) {
          // ignore parse errors and keep default message
        }
        throw new Error(errMsg);
      }

      return await response.json();
    } catch (error) {
      console.error('Register error:', error);
      throw error;
    }
  }

  async login(data: LoginData): Promise<AuthResponse> {
    try {
      // Backend expects JSON { email, password } and returns { message, token, user }
      const response = await fetch(`${API_BASE_URL}/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email: data.email, password: data.password }),
      });

      if (!response.ok) {
        let errMsg = 'Đăng nhập thất bại';
        try {
          const ct = response.headers.get('content-type') || '';
          if (ct.includes('application/json')) {
            const errorData = await response.json();
            if (errorData?.errors && typeof errorData.errors === 'object') {
              const msgs = Object.values(errorData.errors).flat().filter(Boolean);
              errMsg = msgs.join('; ') || errMsg;
            } else {
              errMsg = errorData?.detail || errorData?.message || JSON.stringify(errorData) || errMsg;
            }
          } else {
            const text = await response.text();
            if (text) errMsg = text;
          }
        } catch (e) {
          // ignore
        }
        throw new Error(errMsg);
      }

      // Normalize to { access_token, token_type } for frontend compatibility
      const json = await response.json();
      // backend returns token in `token` field
      return { access_token: json.token, token_type: 'Bearer' } as AuthResponse;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  }

  async getCurrentUser(token: string): Promise<User> {
    try {
      const response = await fetch(`${API_BASE_URL}/profile`, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error('Không thể lấy thông tin người dùng');
      }

      return await response.json();
    } catch (error) {
      console.error('Get current user error:', error);
      throw error;
    }
  }

  // Helper methods
  setToken(token: string): void {
    localStorage.setItem('access_token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }

  removeToken(): void {
    localStorage.removeItem('access_token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}

export const authService = new AuthService();
