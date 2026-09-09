"use client";

import { FormEvent, useState } from "react";
import { apiRequest } from "../lib/api";
import { useAppStore } from "../store/useAppStore";

export default function ChangeTemporaryPassword() {
  const signOut = useAppStore(state => state.signOut);
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
  const [complete, setComplete] = useState(false);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    setBusy(true);
    const form = new FormData(event.currentTarget);
    try {
      await apiRequest("/api/account/password/change", { method: "POST", body: JSON.stringify({
        currentPassword: form.get("currentPassword"), password: form.get("password"), confirmPassword: form.get("confirmPassword"),
      }) });
      signOut();
      setComplete(true);
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Password change failed.");
    } finally { setBusy(false); }
  }

  return <main className="auth-page dark"><section className="auth-card">
    <span className="pill">SECURITY REQUIRED</span><h1>Choose your password.</h1>
    {complete ? <><p>Your password was changed. Sign in again with your new password.</p><a href="/login">Return to sign in</a></> : <>
      <p>The administrator gave you a temporary password. Replace it before entering the workspace.</p>
      <form onSubmit={submit}>
        <label>Temporary password<input name="currentPassword" type="password" autoComplete="current-password" required /></label>
        <label>New password<input name="password" type="password" minLength={12} autoComplete="new-password" required /></label>
        <label>Confirm new password<input name="confirmPassword" type="password" minLength={12} autoComplete="new-password" required /></label>
        <small>Use at least 12 characters with uppercase, lowercase, a number, and a special character.</small>
        {error && <div className="form-error" role="alert">{error}</div>}
        <button disabled={busy}>{busy ? "Changing…" : "Change password →"}</button>
      </form>
    </>}
  </section></main>;
}
