"use client";

import { FormEvent, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { departmentApi, type Department } from "../features/organizations/structureApi";
import { teamApi } from "../features/organizations/teamApi";
import "../departments.css";

export default function OrganizationDepartments() {
  const { id = "" } = useParams();
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<Department | null | undefined>();
  const [selected, setSelected] = useState<Department | null>(null);
  const [error, setError] = useState("");
  const units = useQuery({
    queryKey: ["departments", id],
    queryFn: () => departmentApi.list(id),
    enabled: !!id,
  });
  const team = useQuery({
    queryKey: ["team", id],
    queryFn: () => teamApi.members(id),
    enabled: !!id,
  });
  const members = useQuery({
    queryKey: ["department-members", selected?.id],
    queryFn: () => departmentApi.members(selected!.id),
    enabled: !!selected,
  });
  const departments = units.data ?? [];
  const totalMembers = departments.reduce((sum, department) => sum + department.memberCount, 0);
  const refresh = () => queryClient.invalidateQueries({ queryKey: ["departments", id] });
  const save = useMutation({
    mutationFn: (form: FormData) => {
      const body = {
        name: String(form.get("name")),
        description: String(form.get("description")) || null,
        headUserId: String(form.get("headUserId")) || null,
        status: String(form.get("status") || "Active"),
      };
      return editing
        ? departmentApi.update(id, editing.id, body)
        : departmentApi.create(id, body);
    },
    onSuccess: async savedDepartment => {
      queryClient.setQueryData<Department[]>(["departments", id], current => {
        if (!current) return [savedDepartment];
        const exists = current.some(department => department.id === savedDepartment.id);
        return exists
          ? current.map(department => department.id === savedDepartment.id ? savedDepartment : department)
          : [...current, savedDepartment];
      });
      setEditing(undefined);
      await refresh();
    },
  });

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    try {
      await save.mutateAsync(new FormData(event.currentTarget));
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Unable to save department.");
    }
  }

  async function assign(userId: string) {
    if (!selected) return;
    try {
      await departmentApi.addMember(selected.id, userId);
      await queryClient.invalidateQueries({ queryKey: ["department-members", selected.id] });
      await refresh();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Assignment failed.");
    }
  }

  return (
    <AppShell title="Departments" eyebrow="Organization structure">
      <section className="dept-hero">
        <div>
          <span>Functional structure</span>
          <h2>Give every team a clear home.</h2>
          <p>Organize people by responsibility, assign department leads, and make ownership visible across your organization.</p>
        </div>
        <button onClick={() => { setError(""); setEditing(null); }}>
          <span>＋</span> Create department
        </button>
      </section>

      <section className="dept-stats" aria-label="Department overview">
        <article><span>Departments</span><strong>{departments.length}</strong><small>Functional groups</small></article>
        <article><span>Assigned members</span><strong>{totalMembers}</strong><small>Across all departments</small></article>
        <article><span>Department heads</span><strong>{departments.filter(department => department.headUserId).length}</strong><small>Leadership assigned</small></article>
      </section>

      {error && <div className="form-error dept-error">{error}</div>}

      {units.isLoading && <div className="dept-loading">Loading departments…</div>}
      {units.error && <div className="form-error">{units.error.message}</div>}

      {!units.isLoading && departments.length === 0 ? (
        <section className="dept-empty">
          <div className="dept-empty-art" aria-hidden="true">
            <span>D</span><i /><span>M</span><i /><span>C</span>
          </div>
          <span>Start with your structure</span>
          <h2>No departments yet</h2>
          <p>Create departments such as Creative, Operations, Marketing, or Client Services, then assign team members and a department head.</p>
          <button onClick={() => setEditing(null)}>Create your first department</button>
        </section>
      ) : (
        <section className="dept-grid" aria-label="Departments">
          {departments.map((department, index) => (
            <article
              className={`dept-card${selected?.id === department.id ? " selected" : ""}`}
              key={department.id}
              style={{ "--dept-index": index } as React.CSSProperties}
              onClick={() => setSelected(department)}
            >
              <header>
                <span className="dept-icon">{department.name.slice(0, 2).toUpperCase()}</span>
                <div><small>Department</small><h2>{department.name}</h2></div>
                <span className={`dept-status ${department.status.toLowerCase()}`}><i /> {department.status}</span>
              </header>
              <p>{department.description || "No description added yet."}</p>
              <dl>
                <div><dt>Department head</dt><dd>{department.headName || "Not assigned"}</dd></div>
                <div><dt>Members</dt><dd>{department.memberCount}</dd></div>
              </dl>
              <footer>
                <button type="button" onClick={event => { event.stopPropagation(); setSelected(department); }}>View members</button>
                <button type="button" onClick={event => { event.stopPropagation(); setEditing(department); }}>Edit</button>
                <button
                  type="button"
                  className="dept-archive"
                  onClick={async event => {
                    event.stopPropagation();
                    if (confirm(`Archive ${department.name}?`)) {
                      await departmentApi.archive(id, department.id);
                      setSelected(null);
                      await refresh();
                    }
                  }}
                >
                  Archive
                </button>
              </footer>
            </article>
          ))}
        </section>
      )}

      {selected && (
        <section className="dept-detail">
          <header>
            <div><span>Selected department</span><h2>{selected.name}</h2><p>{selected.description || "Department members and access."}</p></div>
            <select
              aria-label="Assign member"
              defaultValue=""
              onChange={event => { void assign(event.target.value); event.target.value = ""; }}
            >
              <option value="" disabled>＋ Assign member</option>
              {team.data
                ?.filter(person => !members.data?.some(member => member.userId === person.userId))
                .map(person => <option key={person.userId} value={person.userId}>{person.name}</option>)}
            </select>
          </header>
          <div className="dept-member-list">
            {members.data?.map(member => (
              <article key={member.userId}>
                <span>{member.name.split(/\s+/).slice(0, 2).map(part => part[0]).join("").toUpperCase()}</span>
                <div><strong>{member.name}</strong><small>{member.email} · {member.role}</small></div>
                <button onClick={async () => {
                  await departmentApi.removeMember(selected.id, member.userId);
                  await queryClient.invalidateQueries({ queryKey: ["department-members", selected.id] });
                  await refresh();
                }}>Remove</button>
              </article>
            ))}
            {!members.isLoading && !members.data?.length && (
              <div className="dept-members-empty"><span>◎</span><p>No members assigned yet. Use “Assign member” to build this department.</p></div>
            )}
          </div>
        </section>
      )}

      {editing !== undefined && (
        <div className="modal-backdrop dept-modal-backdrop" onClick={() => setEditing(undefined)}>
          <form className="event-modal dept-modal" onSubmit={submit} onClick={event => event.stopPropagation()}>
            <header><span>{editing ? "✎" : "＋"}</span><div><small>Organization structure</small><h2>{editing ? "Edit" : "Create"} department</h2></div></header>
            <p>Define the department’s purpose and optionally assign a team lead.</p>
            <label>Department name<input name="name" defaultValue={editing?.name ?? ""} placeholder="e.g. Creative Studio" autoFocus required /></label>
            <label>Description<textarea name="description" defaultValue={editing?.description ?? ""} placeholder="What does this department own?" /></label>
            <label>Department head<select name="headUserId" defaultValue={editing?.headUserId || ""}><option value="">Not assigned</option>{team.data?.map(person => <option key={person.userId} value={person.userId}>{person.name}</option>)}</select></label>
            {editing && <label>Status<select name="status" defaultValue={editing.status}><option>Active</option><option>Inactive</option></select></label>}
            {error && <div className="form-error">{error}</div>}
            <div><button type="button" onClick={() => setEditing(undefined)}>Cancel</button><button disabled={save.isPending}>{save.isPending ? "Saving…" : editing ? "Save changes" : "Create department"}</button></div>
          </form>
        </div>
      )}
    </AppShell>
  );
}
