"use client";

export type ApiEnvelope<T> = { success: boolean; message?: string; data: T; errors?: unknown };

export const storageKeys = {
  accessToken: "eventlens_access_token",
  refreshToken: "eventlens_refresh_token",
  expiresAt: "eventlens_token_expires_at",
  organizationId: "eventlens_organization_id",
  apiOrigin: "eventlens_api_origin",
} as const;

export const apiOrigin = () =>
  (typeof window !== "undefined" && localStorage.getItem(storageKeys.apiOrigin)) ||
  "https://localhost:7180";

const read = (key: string) =>
  typeof window === "undefined" ? null : localStorage.getItem(key);

let refreshPromise: Promise<boolean> | null = null;

async function refreshSession(): Promise<boolean> {
  if (refreshPromise) return refreshPromise;
  refreshPromise = (async () => {
    const refreshToken = read(storageKeys.refreshToken);
    if (!refreshToken) return false;
    const response = await fetch(`${apiOrigin()}/api/auth/refresh`, {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ refreshToken }),
    });
    if (!response.ok) return false;
    const envelope = (await response.json()) as ApiEnvelope<AuthSession>;
    persistTokens(envelope.data);
    window.dispatchEvent(new CustomEvent("eventlens:token-refreshed", { detail: envelope.data }));
    return true;
  })().catch(() => false).finally(() => { refreshPromise = null; });
  return refreshPromise;
}

export type AuthUser = {
  id: string; firstName: string; lastName: string; email: string; roles: string[];
};
export type AuthSession = {
  accessToken: string; refreshToken: string; expiresAt: string; user: AuthUser;
};

export function persistTokens(session: AuthSession) {
  localStorage.setItem(storageKeys.accessToken, session.accessToken);
  localStorage.setItem(storageKeys.refreshToken, session.refreshToken);
  localStorage.setItem(storageKeys.expiresAt, session.expiresAt);
}

export function clearTokens() {
  localStorage.removeItem(storageKeys.accessToken);
  localStorage.removeItem(storageKeys.refreshToken);
  localStorage.removeItem(storageKeys.expiresAt);
  localStorage.removeItem(storageKeys.organizationId);
}

export class ApiError extends Error {
  constructor(message: string, public status: number, public details?: unknown) { super(message); }
}

function validationMessage(payload: ApiEnvelope<unknown> | null) {
  const container = payload?.errors ?? (payload?.data as { errors?: unknown } | null)?.errors;
  if (!container || typeof container !== "object") return payload?.message;
  const messages = Object.values(container as Record<string, unknown>)
    .flatMap(value => Array.isArray(value) ? value : [value])
    .filter((value): value is string => typeof value === "string");
  return messages.length ? messages.join(" ") : payload?.message;
}

export async function apiRequest<T>(path: string, init: RequestInit = {}, retry = true): Promise<T> {
  const token = read(storageKeys.accessToken);
  const headers = new Headers(init.headers);
  if (token) headers.set("Authorization", `Bearer ${token}`);
  if (init.body && !(init.body instanceof FormData)) headers.set("Content-Type", "application/json");
  const response = await fetch(`${apiOrigin()}${path}`, { ...init, headers, credentials: "include" });
  if (response.status === 401 && retry && !path.includes("/api/auth/")) {
    if (await refreshSession()) return apiRequest<T>(path, init, false);
    clearTokens();
    window.dispatchEvent(new Event("eventlens:session-expired"));
  }
  const payload = await response.json().catch(() => null) as ApiEnvelope<T> | null;
  if (!response.ok) throw new ApiError(
    validationMessage(payload as ApiEnvelope<unknown> | null) || `Request failed (${response.status}).`,
    response.status,
    payload?.errors ?? (payload?.data as { errors?: unknown } | null)?.errors,
  );
  return payload?.data as T;
}

export async function apiDownload(path: string, init: RequestInit, fileName: string) {
  const token = read(storageKeys.accessToken);
  const headers = new Headers(init.headers);
  if (token) headers.set("Authorization", `Bearer ${token}`);
  if (init.body) headers.set("Content-Type", "application/json");
  const response = await fetch(`${apiOrigin()}${path}`, { ...init, headers, credentials: "include" });
  if (!response.ok) throw new ApiError("Download failed.", response.status);
  const url = URL.createObjectURL(await response.blob());
  const anchor = document.createElement("a");
  anchor.href = url; anchor.download = fileName; anchor.click();
  URL.revokeObjectURL(url);
}
