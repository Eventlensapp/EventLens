"use client";

import { useState } from "react";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { teamApi } from "../features/organizations/teamApi";
import { securityApi } from "../features/organizations/securityApi";
import { useAppStore } from "../store/useAppStore";
import "../security.css";
import "../security-dropdowns.css";

function readablePermission(key: string) {
  return key.replace(/[._-]+/g, " ").replace(/\b\w/g, letter => letter.toUpperCase());
}

export default function OrganizationSecurity() {
  const { id = "" } = useParams();
  const client = useQueryClient();
  const currentUser = useAppStore(state => state.user?.id);
  const [selected, setSelected] = useState("");
  const [error, setError] = useState("");
  const members = useQuery({
    queryKey: ["team", id],
    queryFn: () => teamApi.members(id),
    enabled: !!id,
  });
  const permissions = useQuery({
    queryKey: ["permissions", id, selected],
    queryFn: () => securityApi.permissions(id, selected),
    enabled: !!selected,
  });
  const transfer = useQuery({
    queryKey: ["ownership-transfer", id],
    queryFn: () => securityApi.pending(id),
    enabled: !!id,
  });
  const eligible = members.data?.filter(member => member.role !== "Owner") ?? [];
  const selectedMember = eligible.find(member => member.userId === selected);
  const allowedCount = permissions.data?.filter(permission => permission.allowed).length ?? 0;

  async function toggle(key: string, allowed: boolean) {
    setError("");
    try {
      await securityApi.set(id, selected, key, allowed);
      await client.invalidateQueries({ queryKey: ["permissions", id, selected] });
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Permission update failed.");
    }
  }

  async function start(userId: string) {
    const member = eligible.find(person => person.userId === userId);
    if (!confirm(`Transfer ownership to ${member?.name ?? "this member"}? They must accept within two days.`)) return;
    setError("");
    try {
      await securityApi.start(id, userId);
      await client.invalidateQueries({ queryKey: ["ownership-transfer", id] });
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Transfer failed.");
    }
  }

  return (
    <AppShell title="Access administration" eyebrow="Ownership · Permissions">
      <section className="security-hero">
        <div>
          <span>Security & governance</span>
          <h2>Control who can do what.</h2>
          <p>Manage sensitive organization permissions and transfer ownership with a clear, auditable workflow.</p>
        </div>
        <div className="security-score">
          <span>Access status</span><strong>Protected</strong><small><i /> Owner controls enabled</small>
        </div>
      </section>

      {error && <div className="form-error security-error">{error}</div>}

      <section className="security-layout">
        <article className="ownership-card">
          <header>
            <span className="security-icon">♛</span>
            <div><small>Highest privilege</small><h2>Organization ownership</h2></div>
            <span className="security-badge">Owner only</span>
          </header>

          {transfer.isLoading ? <p className="security-loading">Checking transfer status…</p> : transfer.data ? (
            <div className="transfer-pending">
              <div className="transfer-status"><i /><span><strong>Transfer pending</strong><small>Awaiting member acceptance</small></span></div>
              <dl>
                <div><dt>Expires</dt><dd>{new Date(transfer.data.expiresAt).toLocaleString()}</dd></div>
                <div><dt>Status</dt><dd>{transfer.data.status}</dd></div>
              </dl>
              <div className="ownership-actions">
                {transfer.data.toUserId === currentUser && (
                  <button className="security-primary" onClick={async () => {
                    await securityApi.accept(id, transfer.data!.id);
                    location.reload();
                  }}>Accept ownership</button>
                )}
                <button onClick={async () => {
                  await securityApi.cancel(id, transfer.data!.id);
                  await client.invalidateQueries({ queryKey: ["ownership-transfer", id] });
                }}>Cancel transfer</button>
              </div>
            </div>
          ) : (
            <div className="transfer-new">
              <p>The selected member must accept within 48 hours. Your Owner role changes to Manager only after acceptance.</p>
              <label>
                Transfer ownership to
                <select defaultValue="" onChange={event => {
                  if (event.target.value) void start(event.target.value);
                  event.target.value = "";
                }}>
                  <option value="" disabled>Choose an eligible member</option>
                  {eligible.map(member => <option value={member.userId} key={member.userId}>{member.name} · {member.role}</option>)}
                </select>
              </label>
              <div className="security-warning"><span>!</span><p><strong>This is a sensitive action.</strong> Ownership grants complete administrative and billing control.</p></div>
            </div>
          )}
        </article>

        <article className="security-guide">
          <span className="security-icon">⌾</span>
          <small>Security principles</small>
          <h2>Least privilege, always.</h2>
          <p>Give members only the access their responsibilities require. Custom overrides take precedence over role defaults.</p>
          <ul><li><i /> Review overrides regularly</li><li><i /> Keep ownership with a trusted admin</li><li><i /> Remove access when roles change</li></ul>
        </article>
      </section>

      <section className="permissions-panel">
        <header>
          <div><span>Fine-grained access</span><h2>Member permissions</h2><p>Review role defaults and apply individual overrides.</p></div>
          <label>
            <span>Select team member</span>
            <select value={selected} onChange={event => setSelected(event.target.value)}>
              <option value="">Choose a member</option>
              {eligible.map(member => <option value={member.userId} key={member.userId}>{member.name} · {member.role}</option>)}
            </select>
          </label>
        </header>

        {!selected ? (
          <div className="permissions-empty"><span>◇</span><div><strong>Select a member to review access</strong><p>Their effective permissions and custom overrides will appear here.</p></div></div>
        ) : permissions.isLoading ? (
          <div className="security-loading">Loading permissions…</div>
        ) : (
          <>
            <div className="permission-summary">
              <span className="permission-avatar">{selectedMember?.name.split(/\s+/).slice(0, 2).map(part => part[0]).join("").toUpperCase()}</span>
              <div><strong>{selectedMember?.name}</strong><small>{selectedMember?.role} · {allowedCount} of {permissions.data?.length ?? 0} permissions allowed</small></div>
              <span className="permission-count">{permissions.data?.filter(permission => permission.isOverride).length ?? 0} overrides</span>
            </div>
            <div className="permission-list">
              {permissions.data?.map(permission => (
                <article key={permission.key}>
                  <div className="permission-mark">{permission.allowed ? "✓" : "—"}</div>
                  <div><strong>{readablePermission(permission.key)}</strong><small>{permission.description}</small></div>
                  {permission.isOverride && <span className="override-badge">Custom override</span>}
                  <label className="permission-switch">
                    <input type="checkbox" checked={permission.allowed} onChange={event => void toggle(permission.key, event.target.checked)} />
                    <span><i /></span>
                    <b>{permission.allowed ? "Allowed" : "Denied"}</b>
                  </label>
                </article>
              ))}
              {!permissions.data?.length && <div className="permissions-empty"><span>✓</span><div><strong>No configurable permissions</strong><p>This member currently relies entirely on role defaults.</p></div></div>}
            </div>
          </>
        )}
      </section>
    </AppShell>
  );
}
