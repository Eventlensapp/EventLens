"use client";

import { FormEvent, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { teamApi, teamRoles } from "../features/organizations/teamApi";
import "../team.css";

function initials(name: string) {
  return name.split(/\s+/).filter(Boolean).slice(0, 2).map(part => part[0]).join("").toUpperCase();
}

function formatActivity(value?: string) {
  if (!value) return "Never";
  const date = new Date(value);
  const days = Math.floor((Date.now() - date.getTime()) / 86_400_000);
  if (days === 0) return `Today, ${date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}`;
  if (days === 1) return "Yesterday";
  if (days < 7) return `${days} days ago`;
  return date.toLocaleDateString([], { month: "short", day: "numeric", year: "numeric" });
}

export default function OrganizationTeam() {
  const { id = "" } = useParams();
  const client = useQueryClient();
  const [open, setOpen] = useState(false);
  const [error, setError] = useState("");
  const members = useQuery({
    queryKey: ["team", id],
    queryFn: () => teamApi.members(id),
    enabled: !!id,
  });
  const invites = useQuery({
    queryKey: ["invitations", id],
    queryFn: () => teamApi.invitations(id),
    enabled: !!id,
  });
  const pending = invites.data?.filter(invitation => invitation.status === "Pending") ?? [];
  const refresh = () => Promise.all([
    client.invalidateQueries({ queryKey: ["team", id] }),
    client.invalidateQueries({ queryKey: ["invitations", id] }),
  ]);
  const invite = useMutation({
    mutationFn: ({ email, role }: { email: string; role: string }) => teamApi.invite(id, email, role),
    onSuccess: async () => {
      setOpen(false);
      await refresh();
    },
  });

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    const form = new FormData(event.currentTarget);
    try {
      await invite.mutateAsync({
        email: String(form.get("email")),
        role: String(form.get("role")),
      });
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Invitation failed.");
    }
  }

  async function changeRole(userId: string, role: string) {
    setError("");
    try {
      await teamApi.role(id, userId, role);
      await refresh();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Role update failed.");
    }
  }

  async function remove(userId: string) {
    if (!confirm("Remove this member from the organization?")) return;
    setError("");
    try {
      await teamApi.remove(id, userId);
      await refresh();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Remove failed.");
    }
  }

  return (
    <AppShell title="Team management" eyebrow="Members · Roles · Invitations">
      <section className="team-hero">
        <div>
          <span>Organization access</span>
          <h2>Build the team behind every moment.</h2>
          <p>Invite collaborators, assign the right level of access, and keep your workspace secure.</p>
        </div>
        <button className="team-invite-primary" onClick={() => { setError(""); setOpen(true); }}>
          <span>＋</span> Invite member
        </button>
      </section>

      <section className="team-stats" aria-label="Team overview">
        <article><span>Members</span><strong>{members.data?.length ?? 0}</strong><small>Active collaborators</small></article>
        <article><span>Pending</span><strong>{pending.length}</strong><small>Open invitations</small></article>
        <article><span>Roles</span><strong>{new Set(members.data?.map(member => member.role)).size}</strong><small>Access levels in use</small></article>
      </section>

      {error && <div className="form-error team-page-error">{error}</div>}

      <section className="team-panel">
        <header className="team-panel-heading">
          <div><span>People</span><h2>Organization members</h2></div>
          <small>{members.data?.length ?? 0} total</small>
        </header>

        {members.isLoading && <div className="team-loading">Loading team…</div>}
        {members.error && <div className="form-error">{members.error.message}</div>}
        <div className="team-member-list">
          {members.data?.map(member => {
            const isOwner = member.role === "Owner";
            return (
              <article className="team-member" key={member.id}>
                <div className="team-person">
                  <span className="team-avatar">{initials(member.name)}</span>
                  <div><strong>{member.name}</strong><small>{member.email}</small></div>
                </div>
                <div className="team-role">
                  <label htmlFor={`role-${member.id}`}>Role</label>
                  <select
                    id={`role-${member.id}`}
                    value={member.role}
                    disabled={isOwner}
                    onChange={event => void changeRole(member.userId, event.target.value)}
                  >
                    <option>{member.role}</option>
                    {teamRoles.filter(role => role !== member.role).map(role => <option key={role}>{role}</option>)}
                  </select>
                </div>
                <div className="team-state">
                  <label>Status</label>
                  <span><i /> {member.status}</span>
                </div>
                <div className="team-activity">
                  <label>Last activity</label>
                  <span>{formatActivity(member.lastActivity)}</span>
                </div>
                <button
                  className="team-remove"
                  disabled={isOwner}
                  onClick={() => void remove(member.userId)}
                  aria-label={`Remove ${member.name}`}
                  title={isOwner ? "The organization owner cannot be removed" : `Remove ${member.name}`}
                >
                  ⋯
                </button>
              </article>
            );
          })}
        </div>
      </section>

      <section className="team-panel team-invitations">
        <header className="team-panel-heading">
          <div><span>Onboarding</span><h2>Pending invitations</h2></div>
          <small>{pending.length} pending</small>
        </header>
        {invites.isLoading && <div className="team-loading">Loading invitations…</div>}
        {pending.map(invitation => (
          <article className="team-invite-row" key={invitation.id}>
            <span className="team-mail">✉</span>
            <div><strong>{invitation.email}</strong><small>Expires {new Date(invitation.expiresAt).toLocaleDateString()}</small></div>
            <span className="team-role-pill">{invitation.role}</span>
            <button onClick={async () => { await teamApi.cancel(invitation.id); await refresh(); }}>Cancel invite</button>
          </article>
        ))}
        {!invites.isLoading && pending.length === 0 && (
          <div className="team-empty">
            <span>✓</span>
            <div><strong>Everyone is accounted for</strong><p>There are no pending invitations right now.</p></div>
            <button onClick={() => setOpen(true)}>Invite someone</button>
          </div>
        )}
      </section>

      {open && (
        <div className="modal-backdrop team-modal-backdrop" onClick={() => setOpen(false)}>
          <form className="event-modal team-modal" onSubmit={submit} onClick={event => event.stopPropagation()}>
            <header><span>＋</span><div><small>New teammate</small><h2>Invite team member</h2></div></header>
            <p>They’ll receive an invitation to join this organization with the role you choose.</p>
            <label>Email address<input name="email" type="email" placeholder="name@company.com" autoFocus required /></label>
            <label>Organization role<select name="role">{teamRoles.map(role => <option key={role}>{role}</option>)}</select></label>
            {error && <div className="form-error">{error}</div>}
            <div className="team-modal-actions">
              <button type="button" onClick={() => setOpen(false)}>Cancel</button>
              <button disabled={invite.isPending}>{invite.isPending ? "Sending…" : "Send invitation"}</button>
            </div>
          </form>
        </div>
      )}
    </AppShell>
  );
}
