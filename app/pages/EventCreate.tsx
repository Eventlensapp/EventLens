"use client";

import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useMutation, useQuery } from "@tanstack/react-query";
import { AppShell } from "../components/layout/AppShell";
import { EventForm } from "../features/events/EventForm";
import { eventApi } from "../features/events/api";
import { templateApi } from "../features/events/templateApi";
import { useAppStore } from "../store/useAppStore";
import "../event-create.css";

const steps = [["1", "Starting point"], ["2", "Event details"], ["3", "Review & create"]];

export default function EventCreate() {
  const navigate = useNavigate();
  const organizationId = useAppStore((state) => state.activeOrganizationId);
  const [step, setStep] = useState(1);
  const [templateId, setTemplateId] = useState("");
  const [draft, setDraft] = useState<Record<string, unknown>>();
  const [error, setError] = useState("");
  const templates = useQuery({
    queryKey: ["event-templates", organizationId],
    queryFn: () => templateApi.list(organizationId),
    enabled: !!organizationId,
  });
  const selected = templates.data?.find((template) => template.id === templateId);
  const create = useMutation({ mutationFn: eventApi.create });
  const eyebrow = `Step ${step} of 3 · Event core`;

  return (
    <AppShell title="Create event" eyebrow={eyebrow}>
      <div className="event-create-page">
        <nav className="event-create-steps" aria-label="Event creation progress">
          {steps.map(([number, label], index) => {
            const current = index + 1 === step;
            const complete = index + 1 < step;
            return <button key={number} className={current ? "current" : complete ? "complete" : ""} disabled={index + 1 > step} onClick={() => complete && setStep(index + 1)}>
              <span>{complete ? "✓" : number}</span><b>{label}</b>
            </button>;
          })}
        </nav>

        {error && <div className="form-error">{error}</div>}

        {step === 1 && <section className="event-start">
          <div className="event-step-heading"><span>STEP 1 OF 3</span><h2>How would you like to start?</h2><p>Begin from scratch or use a reusable template to preconfigure your event.</p></div>
          <div className="event-template-grid">
            <button onClick={() => { setTemplateId(""); setStep(2); }}>
              <i>＋</i><span><b>Blank event</b><small>Build every detail with complete control.</small></span><em>Start blank →</em>
            </button>
            {templates.data?.map((template) => <button key={template.id} onClick={() => { setTemplateId(template.id); setStep(2); }}>
              <i>◇</i><span><b>{template.name}</b><small>{template.eventTypeName} · {template.defaultDurationMinutes} minutes</small></span><em>Use template →</em>
            </button>)}
          </div>
          {!templates.isLoading && !templates.data?.length && <p className="event-template-hint">Templates you create later will appear here.</p>}
        </section>}

        {step === 2 && <section className="event-details-step">
          <div className="event-step-heading"><span>STEP 2 OF 3</span><h2>Event details</h2><p>Set the identity, timing, location, team and branding for this event.</p></div>
          <EventForm template={selected} busy={false} onSubmit={async (body) => { setDraft(body as Record<string, unknown>); setStep(3); }} />
          <button className="event-step-back" onClick={() => setStep(1)}>← Back to starting point</button>
        </section>}

        {step === 3 && draft && <section className="event-review">
          <div className="event-step-heading"><span>STEP 3 OF 3</span><h2>Review generated configuration</h2><p>Confirm the essentials before EventLens prepares your event workspace.</p></div>
          <div className="event-review-card">
            <div className="event-review-title"><i>◇</i><div><small>{selected ? `BASED ON ${selected.name}` : "BLANK EVENT"}</small><h2>{String(draft.name || "Untitled event")}</h2><p>{String(draft.description || "No description added")}</p></div><b>{String(draft.status)}</b></div>
            <div className="event-review-grid">
              <div><small>STARTS</small><strong>{new Date(String(draft.startDate)).toLocaleString()}</strong></div>
              <div><small>ENDS</small><strong>{new Date(String(draft.endDate)).toLocaleString()}</strong></div>
              <div><small>VENUE</small><strong>{String(draft.venueName || "Not set")}</strong></div>
              <div><small>LOCATION</small><strong>{[draft.city, draft.country].filter(Boolean).join(", ") || "Not set"}</strong></div>
              <div><small>TIMEZONE</small><strong>{String(draft.timezone)}</strong></div>
              <div><small>BRANDING</small><strong>{draft.brandProfileId ? "Selected profile" : "Organization brand"}</strong></div>
            </div>
          </div>
          <div className="event-review-actions">
            <button onClick={() => setStep(2)}>← Edit details</button>
            <button disabled={create.isPending} onClick={async () => {
              try {
                const event = await create.mutateAsync(draft);
                navigate(`/events/${event.id}`);
              } catch (reason) {
                setError(reason instanceof Error ? reason.message : "Unable to create event.");
              }
            }}>{create.isPending ? "Creating workspace…" : "Create event workspace →"}</button>
          </div>
        </section>}
      </div>
    </AppShell>
  );
}
