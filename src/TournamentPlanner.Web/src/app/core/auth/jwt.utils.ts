import { AuthUser } from './auth.models';

const NAME_ID_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
const GIVEN_NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname';
const SURNAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname';

const base64UrlDecode = (value: string): string => {
  const output = value.replace(/-/g, '+').replace(/_/g, '/');
  const padLength = output.length % 4;
  const padded = padLength === 0 ? output : output.padEnd(output.length + (4 - padLength), '=');
  return atob(padded);
};

const parsePayload = (token: string): Record<string, unknown> | null => {
  const parts = token.split('.');
  if (parts.length !== 3) {
    return null;
  }

  try {
    const decoded = base64UrlDecode(parts[1]);
    return JSON.parse(decoded) as Record<string, unknown>;
  } catch {
    return null;
  }
};

const toStringOrNull = (value: unknown): string | null => {
  return typeof value === 'string' ? value : null;
};

const toStringArray = (value: unknown): string[] => {
  if (Array.isArray(value)) {
    return value.filter((entry): entry is string => typeof entry === 'string');
  }

  if (typeof value === 'string') {
    return [value];
  }

  return [];
};

export const decodeJwtUser = (token: string | null): AuthUser | null => {
  if (!token) {
    return null;
  }

  const payload = parsePayload(token);
  if (!payload) {
    return null;
  }

  const permissions = [
    ...toStringArray(payload['permission']),
    ...toStringArray(payload['permissions']),
    ...toStringArray(payload['role']),
  ];

  return {
    id: toStringOrNull(payload['sub']) ?? toStringOrNull(payload[NAME_ID_CLAIM]),
    email: toStringOrNull(payload['email']) ?? toStringOrNull(payload[EMAIL_CLAIM]),
    firstName: toStringOrNull(payload['given_name']) ?? toStringOrNull(payload[GIVEN_NAME_CLAIM]),
    lastName: toStringOrNull(payload['family_name']) ?? toStringOrNull(payload[SURNAME_CLAIM]),
    permissions: [...new Set(permissions)],
  };
};
