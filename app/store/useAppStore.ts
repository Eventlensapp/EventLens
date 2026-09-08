"use client";

import { create } from "zustand";
import { clearTokens, persistTokens, storageKeys, type AuthSession, type AuthUser } from "../lib/api";

export type Organization = {
  id: string; name: string; slug: string; subscriptionPlan: string | number;
  storageUsed: number; storageLimit: number;
};

const stored = (key: string) => typeof window === "undefined" ? null : localStorage.getItem(key);
const storedJson = <T,>(key: string): T | null => {
  try { return JSON.parse(stored(key) || "null") as T | null; } catch { return null; }
};

type AppState = {
  darkMode: boolean;
  credits: number;
  mobileNavOpen: boolean;
  authenticated: boolean;
  userName: string;
  user: AuthUser | null;
  organizations: Organization[];
  activeOrganizationId: string;
  captures: string[];
  activePhoto: string | null;
  setSession: (session: AuthSession) => void;
  setOrganizations: (organizations: Organization[]) => void;
  selectOrganization: (id: string) => void;
  signOut: () => void;
  setCaptures: (captures: string[]) => void;
  setActivePhoto: (photo: string) => void;
  toggleDarkMode: () => void;
  toggleMobileNav: () => void;
  closeMobileNav: () => void;
};

export const useAppStore = create<AppState>((set) => ({
  darkMode: true,
  credits: 28,
  mobileNavOpen: false,
  authenticated: !!stored(storageKeys.accessToken),
  user: storedJson<AuthUser>("eventlens_user"),
  organizations: storedJson<Organization[]>("eventlens_organizations") ?? [],
  activeOrganizationId: stored(storageKeys.organizationId) ?? "",
  userName: storedJson<AuthUser>("eventlens_user")?.firstName ?? "Studio user",
  captures: [],
  activePhoto: null,
  setSession: (session) => {
    persistTokens(session);
    localStorage.setItem("eventlens_user", JSON.stringify(session.user));
    set({ authenticated: true, user: session.user, userName: `${session.user.firstName} ${session.user.lastName}`.trim() });
  },
  setOrganizations: (organizations) => {
    localStorage.setItem("eventlens_organizations", JSON.stringify(organizations));
    const current = stored(storageKeys.organizationId);
    const activeOrganizationId = current && organizations.some(x => x.id === current) ? current : organizations[0]?.id ?? "";
    if (activeOrganizationId) localStorage.setItem(storageKeys.organizationId, activeOrganizationId);
    set({ organizations, activeOrganizationId });
  },
  selectOrganization: (activeOrganizationId) => {
    localStorage.setItem(storageKeys.organizationId, activeOrganizationId);
    set({ activeOrganizationId });
  },
  signOut: () => {
    clearTokens();
    localStorage.removeItem("eventlens_user");
    localStorage.removeItem("eventlens_organizations");
    set({ authenticated: false, user: null, organizations: [], activeOrganizationId: "", userName: "Studio user" });
  },
  setCaptures: (captures) => set({ captures, activePhoto: captures.at(-1) ?? null }),
  setActivePhoto: (activePhoto) => set({ activePhoto }),
  toggleDarkMode: () => set((state) => ({ darkMode: !state.darkMode })),
  toggleMobileNav: () => set((state) => ({ mobileNavOpen: !state.mobileNavOpen })),
  closeMobileNav: () => set({ mobileNavOpen: false }),
}));
