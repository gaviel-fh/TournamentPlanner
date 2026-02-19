import { Injectable } from '@angular/core';
import { AuthSession } from './auth.models';

const ACCESS_TOKEN_KEY = 'tp.accessToken';
const REFRESH_TOKEN_KEY = 'tp.refreshToken';

@Injectable({ providedIn: 'root' })
export class AuthStorageService {
  readSession(): AuthSession | null {
    const accessToken = localStorage.getItem(ACCESS_TOKEN_KEY);
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);

    if (!accessToken || !refreshToken) {
      return null;
    }

    return { accessToken, refreshToken };
  }

  writeSession(session: AuthSession): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, session.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, session.refreshToken);
  }

  clearSession(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
  }
}
