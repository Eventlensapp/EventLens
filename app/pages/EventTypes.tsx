"use client";

import { FormEvent, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AppShell } from "../components/layout/AppShell";
import { eventApi, type EventType } from "../features/events/api";
import { useAppStore } from "../store/useAppStore";
import "../event-types.css";

const descriptions: Record<string, string> = {
  Birthday: "Celebrate birthdays with galleries, booths and guest sharing.",
  "Brand Promotion": "Launch branded campaigns and audience activations.",
  Conference: "Coordinate sessions, speakers, guests and event media.",
  Corporate: "Plan professional company events and team experiences.",
  Exhibition: "Manage exhibitors, displays, visitors and media capture.",
  Festival: "Run multi-zone experiences, staff and live guest engagement.",
  Graduation: "Capture ceremonies, graduates and family memories.",
  Other: "A flexible starting point for any unique event.",
  "Product Launch": "Deliver product reveals, press assets and branded moments.",
  School: "Organize school programs, functions and student experiences.",
  Wedding: "Plan the celebration, team, galleries and guest experience.",
};

export default function EventTypes() {
  const organizationId = useAppStore((state) => state.activeOrganizationId);
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<EventType | null | undefined>();
  const [error, setError] = useState("");
  const types = useQuery({ queryKey: ["event-types", organizationId], queryFn: () => eventApi.types(organizationId), enabled: !!organizationId });
  const save = useMutation({
    mutationFn: (body: unknown) => editing ? eventApi.updateType(editing.id, body) : eventApi.createType(body),
    onSuccess: async () => {
      setEditing(undefined);
      await queryClient.invalidateQueries({ queryKey: ["event-types", organizationId] });
    },
  });

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    setError("");
    try {
      await save.mutateAsync({
        organizationId,
        name: form.get("name"),
        description: form.get("description") || null,
        icon: form.get("icon") || null,
        color: form.get("color"),
        isActive: true,
      });
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Unable to save event type.");
    }
  }

  const systemCount = types.data?.filter((type) => type.isSystemType).length ?? 0;
  const customCount = types.data?.filter((type) => !type.isSystemType).length ?? 0;

  return (
    <AppShell title="Event types" eyebrow="System · Organization custom">
      <div className="event-types-page">
        <section className="event-types-intro">
          <div><span>EVENT CATALOG</span><h2>Choose the right foundation.</h2><p>Event types shape how teams organize and prepare each experience.</p></div>
          <div className="event-type-summary"><span><b>{systemCount}</b>System types</span><span><b>{customCount}</b>Custom types</span></div>
          <button onClick={() => setEditing(null)}>＋ Create custom type</button>
        </section>
        {error && <div className="form-error">{error}</div>}
        <div className="event-types-heading"><div><span>AVAILABLE TYPES</span><h2>Event foundations</h2></div><p>System types are maintained by EventLens. Custom types belong only to this organization.</p></div>
        <div className="event-type-catalog">
          {types.isLoading && <div className="event-types-state">Loading event types…</div>}
          {types.data?.map((type) => (
            <article key={type.id} style={{ "--type-color": type.color } as React.CSSProperties}>
              <div className="event-type-card-top"><i>{type.icon || "◇"}</i><span className={type.isSystemType ? "system" : "custom"}>{type.isSystemType ? "SYSTEM" : "CUSTOM"}</span></div>
              <h2>{type.name}</h2>
              <p>{type.description || descriptions[type.name] || "A reusable event structure for your organization."}</p>
              <footer>
                <span><i />{type.isActive ? "Available" : "Inactive"}</span>
                <div><button onClick={() => setEditing(type)}>Edit</button><button className="danger" onClick={async () => {
                  if (!window.confirm(`Archive ${type.name}?`)) return;
                  await eventApi.archiveType(type.id);
                  await queryClient.invalidateQueries({ queryKey: ["event-types", organizationId] });
                }}>Delete</button></div>
              </footer>
            </article>
          ))}
        </div>
      </div>

      {editing !== undefined && <div className="modal-backdrop" onClick={() => setEditing(undefined)}>
        <form className="event-modal event-type-modal" onSubmit={submit} onClick={(event) => event.stopPropagation()}>
          <div className="event-type-modal-title"><span>{editing ? "EDIT EVENT TYPE" : "NEW CUSTOM TYPE"}</span><h2>{editing ? `Edit ${editing.name}` : "Create event type"}</h2><p>Define a reusable foundation tailored to your organization.</p></div>
          <label>Name<input name="name" defaultValue={editing?.name} placeholder="e.g. Awards ceremony" required /></label>
          <label>Description<textarea name="description" defaultValue={editing?.description} placeholder="How will your team use this event type?" /></label>
          <div className="event-type-style-fields">
            <label>Icon<input name="icon" defaultValue={editing?.icon} placeholder="e.g. ★" maxLength={10} /></label>
            <label>Accent colour<input name="color" type="color" defaultValue={editing?.color || "#745CE0"} /></label>
          </div>
          <div className="event-type-modal-actions"><button type="button" onClick={() => setEditing(undefined)}>Cancel</button><button disabled={save.isPending}>{save.isPending ? "Saving…" : "Save event type"}</button></div>
        </form>
      </div>}
    </AppShell>
  );
}
