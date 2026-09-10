"use client";

import { FormEvent, useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { organizationApi, organizationPayload } from "../features/organizations/api";
import { useAppStore } from "../store/useAppStore";
import "../organization-create.css";

const types = ["PhotographyStudio", "EventAgency", "WeddingPlanner", "Corporate", "Education", "BrandActivation", "Venue", "Other"];
const labelForType = (type: string) => type.replace(/([a-z])([A-Z])/g, "$1 $2");

export default function OrganizationCreate() {
  const navigate = useNavigate();
  const client = useQueryClient();
  const store = useAppStore();
  const [error, setError] = useState("");
  const mutation = useMutation({ mutationFn: (body: unknown) => organizationApi.create(body) });

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setError("");
    try {
      const organization = await mutation.mutateAsync(organizationPayload(new FormData(event.currentTarget)));
      const organizations = await organizationApi.list();
      store.setOrganizations(organizations); store.selectOrganization(organization.id);
      await client.invalidateQueries({ queryKey: ["organizations"] }); navigate("/organizations");
    } catch (reason) { setError(reason instanceof Error ? reason.message : "Creation failed."); }
  }

  return <AppShell title="Create organization" eyebrow="Organization core"><main className="organization-create-page"><section className="organization-create-intro" aria-labelledby="organization-create-heading"><span className="organization-create-icon" aria-hidden="true">✦</span><div><span className="organization-create-eyebrow">NEW WORKSPACE</span><h2 id="organization-create-heading">Set up your organization</h2><p>Create the workspace where your team, events, brand settings, and billing will live.</p></div></section><form className="organization-create-form" onSubmit={submit}><section className="organization-form-section"><div className="organization-section-heading"><span>01</span><div><h3>Organization details</h3><p>Use the name your team and clients will recognize.</p></div></div><label className="organization-field organization-field-wide"><span>Name <b>Required</b></span><input name="name" maxLength={200} autoFocus required placeholder="e.g. EventLens Branch" /></label><label className="organization-field organization-field-wide"><span>Organization type</span><select name="organizationType" defaultValue="WeddingPlanner">{types.map(type=><option key={type} value={type}>{labelForType(type)}</option>)}</select><small>This helps us tailor workspace defaults and terminology.</small></label><label className="organization-field organization-field-wide"><span>Description <em>Optional</em></span><textarea name="description" maxLength={4000} placeholder="Tell your team what this organization does." /><small>Visible to workspace administrators only.</small></label></section><section className="organization-form-section organization-brand-section"><div className="organization-section-heading"><span>02</span><div><h3>Branding</h3><p>You can update your brand kit any time.</p></div></div><label className="organization-field organization-field-wide"><span>Logo URL <em>Optional</em></span><input name="logo" type="url" placeholder="https://your-domain.com/logo.png" /><small>Use a publicly accessible HTTPS image link. You can upload or replace it later.</small></label></section>{error&&<div className="form-error" role="alert">{error}</div>}<footer className="organization-create-actions"><p>Your workspace will be ready immediately after creation.</p><div><button type="button" onClick={()=>navigate("/organizations")}>Cancel</button><button type="submit" disabled={mutation.isPending}>{mutation.isPending?"Creating...":"Create organization"}</button></div></footer></form></main></AppShell>;
}
