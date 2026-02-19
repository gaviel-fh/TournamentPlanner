import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import {
  AuthResponse,
  AuthUserLookup,
  LoginRequest,
  RefreshRequest,
  RegisterRequest,
} from './auth.models';

type AuthApiResponse =
  | AuthResponse
  | {
      accessToken?: string;
      refreshToken?: string;
      AccessToken?: string;
      RefreshToken?: string;
    };

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly authBaseUrl = '/api/auth';

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthApiResponse>(`${this.authBaseUrl}/register`, request)
      .pipe(map((response) => this.normalizeAuthResponse(response)));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthApiResponse>(`${this.authBaseUrl}/login`, request)
      .pipe(map((response) => this.normalizeAuthResponse(response)));
  }

  refresh(request: RefreshRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthApiResponse>(`${this.authBaseUrl}/refresh`, request)
      .pipe(map((response) => this.normalizeAuthResponse(response)));
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.authBaseUrl}/logout`, {});
  }

  getUsers(): Observable<AuthUserLookup[]> {
    return this.http.get<AuthUserLookup[]>(`${this.authBaseUrl}/users`);
  }

  private normalizeAuthResponse(response: AuthApiResponse): AuthResponse {
    const responseWithLegacyCasing = response as {
      AccessToken?: string;
      RefreshToken?: string;
    };

    const accessToken = response.accessToken ?? responseWithLegacyCasing.AccessToken;
    const refreshToken = response.refreshToken ?? responseWithLegacyCasing.RefreshToken;

    if (!accessToken || !refreshToken) {
      throw new Error('Authentication response is missing tokens.');
    }

    return { accessToken, refreshToken };
  }
}
