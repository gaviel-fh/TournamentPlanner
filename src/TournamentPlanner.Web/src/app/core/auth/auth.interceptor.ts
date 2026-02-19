import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthStore } from './auth.store';

const AUTH_ENDPOINTS_TO_SKIP = ['/api/auth/login', '/api/auth/register', '/api/auth/refresh'];

const shouldSkipAuthHeader = (url: string): boolean => {
  return AUTH_ENDPOINTS_TO_SKIP.some((path) => url.includes(path));
};

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authStore = inject(AuthStore);
  const accessToken = authStore.accessToken();

  const requestWithAuth =
    accessToken && !shouldSkipAuthHeader(request.url)
      ? request.clone({
          setHeaders: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      : request;

  return next(requestWithAuth).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse)) {
        return throwError(() => error);
      }

      if (error.status !== 401 || shouldSkipAuthHeader(request.url)) {
        return throwError(() => error);
      }

      return authStore.refreshAccessToken().pipe(
        switchMap((newAccessToken) => {
          const retryRequest = request.clone({
            setHeaders: {
              Authorization: `Bearer ${newAccessToken}`,
            },
          });

          return next(retryRequest);
        }),
        catchError((refreshError) => {
          authStore.clearSessionAndRedirect();
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
