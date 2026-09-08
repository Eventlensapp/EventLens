"use client";

import { useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { templateApi } from "../features/events/templateApi";
import { useAppStore } from "../store/useAppStore";
import "../event-templates.css";

const featureLabel = (key: string) => key.replace("Enabled", "").replace(/([A-Z])/g, " $1").trim();

export default function EventTemplates() {
  const organizationId = useAppStore((state) => state.activeOrganizationId);
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const templates = useQuery({
    queryKey: ["event-templates", organizationId],
    queryFn: () => templateApi.list(organizationId),
    enabled: !!organizationId,
  });

  return (
    <AppShell title="Event templates" eyebrow="Reusable event configuration">
      <div className="event-templates-page">
        <section className="event-templates-intro">
          <div><span>REUSABLE WORKFLOWS</span><h2>Launch events with confidence.</h2><p>Save your preferred event structure, branding, duration and features for faster setup.</p></div>
          <div className="template-count"><strong>{templates.data?.length ?? 0}</strong><span>Saved templates</span></div>
          <Link to="/event-templates/create">＋ Create template</Link>
        </section>

        {templates.error && <div className="form-error">{templates.error.message}</div>}
        {templates.isLoading && <div className="templates-loading">Loading templates…</div>}

        {!!templates.data?.length && <>
          <div className="template-library-heading"><div><span>TEMPLATE LIBRARY</span><h2>Reusable foundations</h2></div><p>Choose a template when creating an event to prefill its configuration.</p></div>
          <div className="event-template-cards">
            {templates.data.map((template) => {
              const enabled = Object.entries(template.configuration).filter(([, active]) => active);
              return <article key={template.id}>
                <div className="template-card-cover">
                  <span>{template.eventTypeName}</span><i>◇</i>
                  <div><small>DEFAULT DURATION</small><strong>{template.defaultDurationMinutes}<b>min</b></strong></div>
                </div>
                <div className="template-card-content">
                  <h2>{template.name}</h2>
                  <p>{template.description || "Reusable event configuration for your organization."}</p>
                  <div className="template-feature-list">{enabled.slice(0, 5).map(([key]) => <span key={key}>✓ {featureLabel(key)}</span>)}{!enabled.length && <span>No optional features</span>}</div>
                </div>
                <footer>
                  <Link to={`/event-templates/${template.id}`}>Edit template</Link>
                  <button onClick={async () => {
                    const name = window.prompt("Duplicate name", `${template.name} Copy`);
                    if (name) {
                      const copy = await templateApi.clone(template.id, name);
                      navigate(`/event-templates/${copy.id}`);
                    }
                  }}>Duplicate</button>
                  <button className="danger" onClick={async () => {
                    if (!window.confirm(`Archive ${template.name}?`)) return;
                    await templateApi.archive(template.id);
                    await queryClient.invalidateQueries({ queryKey: ["event-templates", organizationId] });
                  }}>Archive</button>
                </footer>
              </article>;
            })}
          </div>
        </>}

        {!templates.isLoading && !templates.data?.length && <section className="templates-empty">
          <div className="templates-empty-visual"><span>◇</span><i>＋</i></div>
          <span>BUILD ONCE · REUSE EVERYWHERE</span>
          <h2>Create your first event template.</h2>
          <p>Templates save your event type, duration, branding and feature defaults—giving every new event a consistent starting point.</p>
          <div className="template-benefits"><span>✓ Faster event setup</span><span>✓ Consistent branding</span><span>✓ Reliable feature defaults</span></div>
          <Link to="/event-templates/create">＋ Create your first template</Link>
        </section>}
      </div>
    </AppShell>
  );
}
