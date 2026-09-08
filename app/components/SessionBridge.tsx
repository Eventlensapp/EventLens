"use client";
import { useEffect } from "react";
import { apiRequest, type AuthSession } from "../lib/api";
import { useAppStore, type Organization } from "../store/useAppStore";

export default function SessionBridge() {
  const { authenticated, setSession, setOrganizations, signOut } = useAppStore();
  useEffect(() => {
    const expired = () => signOut();
    const refreshed = (event: Event) => setSession((event as CustomEvent<AuthSession>).detail);
    window.addEventListener("eventlens:session-expired", expired);
    window.addEventListener("eventlens:token-refreshed", refreshed);
    if (authenticated) apiRequest<Organization[]>("/api/organizations").then(setOrganizations).catch(() => {});
    return () => { window.removeEventListener("eventlens:session-expired", expired); window.removeEventListener("eventlens:token-refreshed", refreshed); };
  }, [authenticated, setOrganizations, setSession, signOut]);
  return null;
}
