import { HttpErrorResponse } from '@angular/common/http';
import { computed, Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, finalize, map, Observable, of, shareReplay, tap, throwError } from 'rxjs';
import { AuthApiService } from './auth-api.service';
import { AuthResponse, LoginRequest, RegisterRequest } from './auth.models';
import { AuthStorageService } from './auth-storage.service';
import { decodeJwtUser } from './jwt.utils';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly api = inject(AuthApiService);
  private readonly storage = inject(AuthStorageService);
  private readonly router = inject(Router);

  private readonly accessTokenSignal = signal<string | null>(null);
  private readonly refreshTokenSignal = signal<string | null>(null);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly accessToken = this.accessTokenSignal.asReadonly();
  readonly refreshToken = this.refreshTokenSignal.asReadonly();
  readonly isAuthenticated = computed(() =>
    Boolean(this.accessTokenSignal() && this.refreshTokenSignal()),
  );
  readonly user = computed(() => decodeJwtUser(this.accessTokenSignal()));

  private refreshRequest$: Observable<string> | null = null;

  constructor() {
    const session = this.storage.readSession();
    if (session) {
      this.accessTokenSignal.set(session.accessToken);
      this.refreshTokenSignal.set(session.refreshToken);
    }
  }

  login(request: LoginRequest): Observable<void> {
    this.loading.set(true);
    this.error.set(null);

    return this.api.login(request).pipe(
      tap((response) => this.setSession(response)),
      tap(() => this.router.navigateByUrl('/app')),
      map(() => undefined),
      catchError((error) => this.handleAuthError(error)),
      finalize(() => this.loading.set(false)),
    );
  }

  register(request: RegisterRequest): Observable<void> {
    this.loading.set(true);
    this.error.set(null);

    return this.api.register(request).pipe(
      tap((response) => this.setSession(response)),
      tap(() => this.router.navigateByUrl('/app')),
      map(() => undefined),
      catchError((error) => this.handleAuthError(error)),
      finalize(() => this.loading.set(false)),
    );
  }

  logout(): Observable<void> {
    this.loading.set(true);

    return this.api.logout().pipe(
      catchError(() => of(undefined)),
      tap(() => this.clearSessionOnly()),
      tap(() => this.router.navigateByUrl('/auth/login')),
      map(() => undefined),
      finalize(() => this.loading.set(false)),
    );
  }

  refreshAccessToken(): Observable<string> {
    if (this.refreshRequest$) {
      return this.refreshRequest$;
    }

    const refreshToken = this.refreshTokenSignal();
    if (!refreshToken) {
      this.clearSessionOnly();
      return throwError(() => new Error('No refresh token available.'));
    }

    const request$ = this.api.refresh({ refreshToken }).pipe(
      tap((response) => this.setSession(response)),
      map((response) => response.accessToken),
      catchError((error) => {
        this.clearSessionOnly();
        return throwError(() => error);
      }),
      finalize(() => {
        this.refreshRequest$ = null;
      }),
      shareReplay(1),
    );

    this.refreshRequest$ = request$;
    return request$;
  }

  clearSessionAndRedirect(): void {
    this.clearSessionOnly();
    void this.router.navigateByUrl('/auth/login');
  }

  private setSession(response: AuthResponse): void {
    this.accessTokenSignal.set(response.accessToken);
    this.refreshTokenSignal.set(response.refreshToken);
    this.storage.writeSession({
      accessToken: response.accessToken,
      refreshToken: response.refreshToken,
    });
  }

  private clearSessionOnly(): void {
    this.accessTokenSignal.set(null);
    this.refreshTokenSignal.set(null);
    this.storage.clearSession();
    this.error.set(null);
  }

  private handleAuthError(error: unknown): Observable<never> {
    this.error.set(this.toErrorMessage(error));
    return throwError(() => error);
  }

  private toErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (typeof error.error?.message === 'string') {
        return error.error.message;
      }

      if (typeof error.error?.Message === 'string') {
        return error.error.Message;
      }

      if (error.status === 401) {
        return 'Invalid email or password.';
      }

      if (error.status === 409) {
        return 'Email already registered.';
      }
    }

    return 'Something went wrong. Please try again.';
  }
}
