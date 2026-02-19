export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  nickname: string | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RefreshRequest {
  refreshToken: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

export interface AuthSession {
  accessToken: string;
  refreshToken: string;
}

export interface AuthUser {
  id: string | null;
  email: string | null;
  firstName: string | null;
  lastName: string | null;
  permissions: string[];
}
