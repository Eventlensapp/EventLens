"use client";

import { FormEvent, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { organizationApi, organizationPayload } from "../features/organizations/api";
import { useAppStore } from "../store/useAppStore";

const organizationTypes = [
  "PhotographyStudio", "EventAgency", "WeddingPlanner", "Corporate",
  "Education", "BrandActivation", "Venue", "Other",
];

export default function OrganizationSettings() {
  const { id = "" } = useParams();
  const navigate = useNavigate();
  const client = useQueryClient();
  const store = useAppStore();
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [isArchiving, setIsArchiving] = useState(false);
  const query = useQuery({
    queryKey: ["organization", id],
    queryFn: () => organizationApi.get(id),
    enabled: !!id,
  });
  const update = useMutation({
    mutationFn: (body: unknown) => organizationApi.update(id, body),
  });

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    setSuccess("");
    try {
      await update.mutateAsync(organizationPayload(new FormData(event.currentTarget)));
      await client.invalidateQueries({ queryKey: ["organization", id] });
      store.setOrganizations(await organizationApi.list());
      setSuccess("Organization settings saved.");
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Update failed.");
    }
  }

  async function archive() {
    if (!confirm("Archive this organization?")) return;
    setError("");
    setSuccess("");
    setIsArchiving(true);
    try {
      await organizationApi.archive(id);
      store.setOrganizations(await organizationApi.list());
      navigate("/organizations");
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Archive failed.");
      setIsArchiving(false);
    }
  }

  if (query.isLoading) {
    return <AppShell title="Organization settings"><p>Loading…</p></AppShell>;
  }
  if (query.error || !query.data) {
    return <AppShell title="Organization settings"><div className="form-error">{query.error?.message || "Not found."}</div></AppShell>;
  }

  const organization = query.data;
  const isBusy = update.isPending || isArchiving;
  return (
    <AppShell title="Organization settings" eyebrow="Owner access">
      <form className="event-modal" onSubmit={submit}>
        <label>Name<input name="name" defaultValue={organization.name} required /></label>
        <label>Type<select name="organizationType" defaultValue={organization.organizationType}>
          {organizationTypes.map(type => <option key={type}>{type}</option>)}
        </select></label>
        <label>Description<textarea name="description" defaultValue={organization.description ?? ""} /></label>
        <label>Logo URL placeholder<input name="logo" type="url" defaultValue={organization.logo ?? ""} /></label>
        {error && <div className="form-error">{error}</div>}
        {success && <div className="form-success" role="status">{success}</div>}
        <div>
          <button type="button" disabled={isBusy} onClick={() => void archive()}>
            {isArchiving ? "Archiving…" : "Archive"}
          </button>
          <button type="submit" disabled={isBusy}>
            {update.isPending ? "Saving…" : "Save changes"}
          </button>
        </div>
      </form>
    </AppShell>
  );
}
