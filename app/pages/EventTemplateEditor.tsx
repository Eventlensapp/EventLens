"use client";

import { FormEvent, useEffect, useState } from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { brandingApi } from "../features/organizations/brandingApi";
import { eventApi } from "../features/events/api";
import { templateApi } from "../features/events/templateApi";
import { useAppStore } from "../store/useAppStore";
import "../template-editor.css";

const features = [
  ["boothEnabled", "Photo booth", "Enable capture and booth workflows", "◎"],
  ["galleryEnabled", "Guest gallery", "Prepare online delivery for guests", "◇"],
  ["printingEnabled", "Print workflow", "Include on-site printing defaults", "▤"],
  ["aiEnabled", "AI studio", "Enable supported AI processing tools", "✦"],
  ["crmEnabled", "Guest CRM", "Prepare guest engagement and follow-up", "●"],
] as const;

export default function EventTemplateEditor() {
  const { id } = useParams();
  const organizationId = useAppStore((state) => state.activeOrganizationId);
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const [previewName, setPreviewName] = useState("New event template");
  const [previewType, setPreviewType] = useState("Choose an event type");
  const [previewDuration, setPreviewDuration] = useState(240);
  const [enabledFeatures, setEnabledFeatures] = useState<string[]>([]);
  const existing = useQuery({ queryKey: ["event-template", id], queryFn: () => templateApi.get(id!), enabled: !!id });
  const types = useQuery({ queryKey: ["event-types", organizationId], queryFn: () => eventApi.types(organizationId), enabled: !!organizationId });
  const themes = useQuery({ queryKey: ["brand-themes", organizationId], queryFn: () => brandingApi.themes(organizationId), enabled: !!organizationId });
  const save = useMutation({
    mutationFn: (body: unknown) => id ? templateApi.update(id, body) : templateApi.create(body),
    onSuccess: (template) => navigate(`/event-templates/${template.id}`),
  });
  const template = existing.data;

  useEffect(() => {
    if (!template) return;
    setPreviewName(template.name);
    setPreviewType(template.eventTypeName);
    setPreviewDuration(template.defaultDurationMinutes);
    setEnabledFeatures(features.filter(([key]) => template.configuration[key]).map(([key]) => key));
  }, [template]);

  if (id && existing.isLoading) return <AppShell title="Template editor"><div className="template-editor-loading">Loading template…</div></AppShell>;

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    setError("");
    try {
      await save.mutateAsync({
        organizationId,
        name: form.get("name"),
        description: form.get("description") || null,
        eventTypeId: form.get("eventTypeId"),
        defaultBrandProfileId: form.get("defaultBrandProfileId") || null,
        defaultDurationMinutes: Number(form.get("defaultDurationMinutes")),
        configuration: Object.fromEntries(features.map(([key]) => [key, form.get(key) === "on"])),
        isActive: true,
      });
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Unable to save template.");
    }
  }

  return (
    <AppShell title={id ? "Edit event template" : "Create event template"} eyebrow="Defaults · Features · Branding">
      <form className="template-builder" onSubmit={submit}>
        <div className="template-builder-head">
          <div><span>TEMPLATE BUILDER</span><h2>{id ? "Refine your reusable workflow." : "Build a reusable event foundation."}</h2><p>Set the defaults your team should inherit whenever they start from this template.</p></div>
          <Link to="/event-templates">← Back to templates</Link>
        </div>
        {error && <div className="form-error">{error}</div>}

        <div className="template-builder-layout">
          <div className="template-builder-main">
            <section className="template-builder-section">
              <div className="template-section-title"><span>01</span><div><h2>Template identity</h2><p>Name the workflow and choose its event foundation.</p></div></div>
              <div className="template-form-grid">
                <label className="wide">Template name<input name="name" defaultValue={template?.name} placeholder="e.g. Premium wedding experience" required onChange={(event) => setPreviewName(event.target.value || "New event template")} /></label>
                <label className="wide">Description<textarea name="description" defaultValue={template?.description} placeholder="Explain when your team should use this template." /></label>
                <label>Event type<select name="eventTypeId" defaultValue={template?.eventTypeId || ""} required onChange={(event) => setPreviewType(event.target.options[event.target.selectedIndex].text)}><option value="">Select event type</option>{types.data?.map((type) => <option key={type.id} value={type.id}>{type.name}</option>)}</select></label>
                <label>Brand profile<select name="defaultBrandProfileId" defaultValue={template?.defaultBrandProfileId || ""}><option value="">Organization default</option>{themes.data?.map((theme) => <option key={theme.id} value={theme.id}>{theme.name}</option>)}</select></label>
                <label>Default duration<div className="template-duration"><input name="defaultDurationMinutes" type="number" min="1" max="44640" defaultValue={template?.defaultDurationMinutes || 240} onChange={(event) => setPreviewDuration(Number(event.target.value) || 0)} /><span>minutes</span></div></label>
              </div>
            </section>

            <section className="template-builder-section">
              <div className="template-section-title"><span>02</span><div><h2>Feature defaults</h2><p>Choose which capabilities should be prepared for new events.</p></div></div>
              <div className="template-feature-options">
                {features.map(([key, title, description, icon]) => <label key={key}>
                  <input name={key} type="checkbox" defaultChecked={template?.configuration[key]} onChange={(event) => setEnabledFeatures((current) => event.target.checked ? [...new Set([...current, key])] : current.filter((item) => item !== key))} />
                  <i>{icon}</i><span><b>{title}</b><small>{description}</small></span><em />
                </label>)}
              </div>
            </section>
          </div>

          <aside className="template-preview-panel">
            <div className="template-preview-heading"><span>LIVE PREVIEW</span><h2>Template summary</h2><p>This is how the template will appear to your team.</p></div>
            <div className="template-live-card">
              <div className="template-live-cover"><span>{previewType}</span><i>◇</i></div>
              <div className="template-live-content"><small>EVENT TEMPLATE</small><h2>{previewName}</h2><p>Reusable organization configuration</p><div><span>◷ {previewDuration} minutes</span><span>✦ {enabledFeatures.length} features</span></div></div>
            </div>
            <div className="template-preview-features"><small>ENABLED BY DEFAULT</small>{enabledFeatures.length ? enabledFeatures.map((key) => <span key={key}>✓ {features.find(([feature]) => feature === key)?.[1]}</span>) : <p>No optional features selected.</p>}</div>
            <div className="template-builder-note"><span>ⓘ</span><p>Feature switches store configuration defaults. Availability still depends on your subscription.</p></div>
          </aside>
        </div>

        <footer className="template-builder-actions"><Link to="/event-templates">Cancel</Link><button disabled={save.isPending}>{save.isPending ? "Saving template…" : id ? "Save changes →" : "Create template →"}</button></footer>
      </form>
    </AppShell>
  );
}
