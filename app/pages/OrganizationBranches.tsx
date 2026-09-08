"use client";

import { FormEvent, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { branchApi, type Branch } from "../features/organizations/structureApi";
import { teamApi } from "../features/organizations/teamApi";
import "../branches.css";
import "../branch-modal-fixes.css";

const location = (branch: Branch) => [branch.city, branch.country].filter(Boolean).join(", ") || "Location not set";

export default function OrganizationBranches() {
  const { id = "" } = useParams();
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<Branch | null | undefined>();
  const [selected, setSelected] = useState<Branch | null>(null);
  const [error, setError] = useState("");
  const units = useQuery({ queryKey: ["branches", id], queryFn: () => branchApi.list(id), enabled: !!id });
  const team = useQuery({ queryKey: ["team", id], queryFn: () => teamApi.members(id), enabled: !!id });
  const members = useQuery({
    queryKey: ["branch-members", selected?.id],
    queryFn: () => branchApi.members(selected!.id),
    enabled: !!selected,
  });
  const branches = units.data ?? [];
  const refresh = () => queryClient.invalidateQueries({ queryKey: ["branches", id] });
  const save = useMutation({
    mutationFn: (form: FormData) => {
      const body = {
        name: String(form.get("name")),
        code: String(form.get("code")),
        address: String(form.get("address")) || null,
        city: String(form.get("city")) || null,
        country: String(form.get("country")) || null,
        timezone: String(form.get("timezone") || "UTC"),
        phone: String(form.get("phone")) || null,
        email: String(form.get("email")) || null,
        managerUserId: String(form.get("managerUserId")) || null,
        status: String(form.get("status") || "Active"),
      };
      return editing ? branchApi.update(id, editing.id, body) : branchApi.create(id, body);
    },
    onSuccess: async savedBranch => {
      queryClient.setQueryData<Branch[]>(["branches", id], current => {
        if (!current) return [savedBranch];
        const exists = current.some(branch => branch.id === savedBranch.id);
        return exists
          ? current.map(branch => branch.id === savedBranch.id ? savedBranch : branch)
          : [...current, savedBranch];
      });
      setEditing(undefined);
      await refresh();
    },
  });

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    try { await save.mutateAsync(new FormData(event.currentTarget)); }
    catch (reason) { setError(reason instanceof Error ? reason.message : "Unable to save branch."); }
  }

  async function assign(userId: string) {
    if (!selected) return;
    try {
      await branchApi.addMember(selected.id, userId);
      await queryClient.invalidateQueries({ queryKey: ["branch-members", selected.id] });
      await refresh();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Assignment failed.");
    }
  }

  return (
    <AppShell title="Branches" eyebrow="Locations · Organization structure">
      <section className="branch-hero">
        <div><span>Location management</span><h2>Bring every office into view.</h2><p>Manage physical locations, local leadership, contact details, and the teams assigned to each branch.</p></div>
        <button onClick={() => { setError(""); setEditing(null); }}><span>＋</span> Create branch</button>
      </section>

      <section className="branch-stats" aria-label="Branch overview">
        <article><span>Locations</span><strong>{branches.length}</strong><small>Organization branches</small></article>
        <article><span>Team members</span><strong>{branches.reduce((sum, branch) => sum + branch.memberCount, 0)}</strong><small>Assigned across locations</small></article>
        <article><span>Branch managers</span><strong>{branches.filter(branch => branch.managerUserId).length}</strong><small>Local leads assigned</small></article>
      </section>

      {error && <div className="form-error branch-error">{error}</div>}
      {units.isLoading && <div className="branch-loading">Loading branches…</div>}
      {units.error && <div className="form-error">{units.error.message}</div>}

      {!units.isLoading && branches.length === 0 ? (
        <section className="branch-empty">
          <div aria-hidden="true"><span>⌖</span><i /><span>⌂</span><i /><span>◎</span></div>
          <small>Build your location network</small>
          <h2>No branches yet</h2>
          <p>Add your head office, studios, venues, or regional teams. Each branch can have its own manager, contact details, timezone, and assigned members.</p>
          <button onClick={() => setEditing(null)}>Create your first branch</button>
        </section>
      ) : (
        <section className="branch-grid">
          {branches.map((branch, index) => (
            <article className={`branch-card${selected?.id === branch.id ? " selected" : ""}`} key={branch.id} style={{ "--branch-index": index } as React.CSSProperties} onClick={() => setSelected(branch)}>
              <header><span className="branch-pin">⌖</span><div><small>{branch.code}</small><h2>{branch.name}</h2></div><span className={`branch-status ${branch.status.toLowerCase()}`}><i /> {branch.status}</span></header>
              <div className="branch-address"><strong>{location(branch)}</strong><span>{branch.address || "Address not added"}</span></div>
              <dl><div><dt>Manager</dt><dd>{branch.managerName || "Not assigned"}</dd></div><div><dt>Team</dt><dd>{branch.memberCount}</dd></div><div><dt>Timezone</dt><dd>{branch.timezone}</dd></div></dl>
              <footer>
                <button onClick={event => { event.stopPropagation(); setSelected(branch); }}>View team</button>
                <button onClick={event => { event.stopPropagation(); setEditing(branch); }}>Edit</button>
                <button className="branch-archive" onClick={async event => {
                  event.stopPropagation();
                  if (confirm(`Archive ${branch.name}?`)) { await branchApi.archive(id, branch.id); setSelected(null); await refresh(); }
                }}>Archive</button>
              </footer>
            </article>
          ))}
        </section>
      )}

      {selected && (
        <section className="branch-detail">
          <header><div><span>Selected branch</span><h2>{selected.name}</h2><p>{selected.address || location(selected)}</p></div><select aria-label="Assign member" defaultValue="" onChange={event => { void assign(event.target.value); event.target.value = ""; }}><option value="" disabled>＋ Assign member</option>{team.data?.filter(person => !members.data?.some(member => member.userId === person.userId)).map(person => <option key={person.userId} value={person.userId}>{person.name}</option>)}</select></header>
          {members.data?.map(member => <article className="branch-member" key={member.userId}><span>{member.name.split(/\s+/).slice(0, 2).map(part => part[0]).join("").toUpperCase()}</span><div><strong>{member.name}</strong><small>{member.email} · {member.role}</small></div><button onClick={async () => { await branchApi.removeMember(selected.id, member.userId); await queryClient.invalidateQueries({ queryKey: ["branch-members", selected.id] }); await refresh(); }}>Remove</button></article>)}
          {!members.isLoading && !members.data?.length && <div className="branch-members-empty"><span>◎</span><p>No members assigned. Use “Assign member” to build this location’s team.</p></div>}
        </section>
      )}

      {editing !== undefined && (
        <div className="modal-backdrop branch-modal-backdrop" onClick={() => setEditing(undefined)}>
          <form className="event-modal branch-modal" onSubmit={submit} onClick={event => event.stopPropagation()}>
            <header><span>{editing ? "✎" : "⌖"}</span><div><small>Organization location</small><h2>{editing ? "Edit" : "Create"} branch</h2></div></header>
            <p>Add the branch identity, location, contact information, and local leadership.</p>
            <div className="branch-form-grid">
              <label>Branch name<input name="name" defaultValue={editing?.name ?? ""} placeholder="e.g. Kathmandu Studio" autoFocus required /></label>
              <label>Branch code<input name="code" defaultValue={editing?.code ?? ""} placeholder="KTM-01" pattern="[A-Za-z0-9_-]+" required /></label>
              <label className="branch-wide">Street address<input name="address" defaultValue={editing?.address ?? ""} placeholder="Street, building, floor" /></label>
              <label>City<input name="city" defaultValue={editing?.city ?? ""} /></label>
              <label>Country<input name="country" defaultValue={editing?.country ?? ""} /></label>
              <label>Timezone<input name="timezone" defaultValue={editing?.timezone || Intl.DateTimeFormat().resolvedOptions().timeZone} required /></label>
              <label>Phone<input name="phone" defaultValue={editing?.phone ?? ""} /></label>
              <label>Email<input name="email" type="email" defaultValue={editing?.email ?? ""} /></label>
              <label>Branch manager<select name="managerUserId" defaultValue={editing?.managerUserId || ""}><option value="">Not assigned</option>{team.data?.map(person => <option key={person.userId} value={person.userId}>{person.name}</option>)}</select></label>
              {editing && <label>Status<select name="status" defaultValue={editing.status}><option>Active</option><option>Inactive</option></select></label>}
            </div>
            {error && <div className="form-error">{error}</div>}
            <div className="branch-modal-actions"><button type="button" onClick={() => setEditing(undefined)}>Cancel</button><button disabled={save.isPending}>{save.isPending ? "Saving…" : editing ? "Save changes" : "Create branch"}</button></div>
          </form>
        </div>
      )}
    </AppShell>
  );
}
