"use client";
import { FormEvent, useState } from "react";
import { Navigate, NavLink, useNavigate } from "react-router-dom";
import { Brand } from "../components/layout/AppShell";
import { apiRequest, type AuthSession } from "../lib/api";
import { useAppStore, type Organization } from "../store/useAppStore";

export default function Login() {
  const { authenticated, setSession, setOrganizations } = useAppStore();
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
  if (authenticated) return <Navigate to="/dashboard" replace />;
  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setError(""); setBusy(true);
    const data = new FormData(event.currentTarget);
    try {
      const session = await apiRequest<AuthSession>("/api/auth/login", {
        method: "POST", body: JSON.stringify({ email: data.get("email"), password: data.get("password") }),
      });
      setSession(session);
      if (session.user.mustChangePassword) { navigate("/change-password"); return; }
      setOrganizations(await apiRequest<Organization[]>("/api/organizations"));
      navigate("/dashboard");
    } catch (reason) { setError(reason instanceof Error ? reason.message : "Sign in failed."); }
    finally { setBusy(false); }
  }
  return <main className="auth-page dark"><div className="auth-brand"><Brand /></div><section className="auth-card"><span className="pill">EVENTLENS AI</span><h1>Welcome back.</h1><p>Sign in to manage events, capture sessions, and galleries.</p><form onSubmit={submit}><label>Email<input name="email" type="email" autoComplete="email" required /></label><label>Password<input name="password" type="password" minLength={8} autoComplete="current-password" required /></label>{error && <div className="form-error" role="alert">{error}</div>}<button type="submit" disabled={busy}>{busy ? "Signing in…" : "Enter studio →"}</button></form><small>Use the account registered with your EventLens API.</small><NavLink to="/register">Create an account</NavLink><NavLink to="/">← Back to home</NavLink></section></main>;
}
