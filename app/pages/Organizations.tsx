"use client";

import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import "../organizations.css";
import { AppShell } from "../components/layout/AppShell";
import { organizationApi } from "../features/organizations/api";
import { useAppStore } from "../store/useAppStore";

function initials(name: string) {
  return name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map(part => part[0])
    .join("")
    .toUpperCase();
}

function readableType(type: string) {
  return type.replace(/([a-z])([A-Z])/g, "$1 $2");
}

export default function Organizations() {
  const store = useAppStore();
  const query = useQuery({
    queryKey: ["organizations"],
    queryFn: organizationApi.list,
  });
  const organizations = query.data ?? [];
  const active = organizations.find(org => org.id === store.activeOrganizationId);

  return (
    <AppShell title="Organizations" eyebrow="Tenant workspace">
      <section className="org-hero">
        <div>
          <span className="org-kicker">Your workspaces</span>
          <h2>Choose where you want to create.</h2>
          <p>
            Every organization keeps its events, team, brand assets, and
            billing neatly separated.
          </p>
        </div>
        <div className="org-hero-actions">
          <div className="org-summary">
            <strong>{organizations.length}</strong>
            <span>{organizations.length === 1 ? "workspace" : "workspaces"}</span>
          </div>
          <Link className="org-create" to="/organizations/create">
            <span>＋</span>
            New organization
          </Link>
        </div>
      </section>

      {query.isLoading && (
        <div className="org-loading" role="status">
          <span />
          <p>Loading your workspaces…</p>
        </div>
      )}
      {query.error && <div className="form-error">{query.error.message}</div>}
      {!query.isLoading && organizations.length === 0 && (
        <section className="org-empty">
          <div>✦</div>
          <h2>Create your first workspace</h2>
          <p>Bring your events, team, and brand together in one place.</p>
          <Link to="/organizations/create">Create organization</Link>
        </section>
      )}

      <section className="org-grid" aria-label="Your organizations">
        {organizations.map((organization, index) => {
          const isCurrent = organization.id === store.activeOrganizationId;
          return (
            <article
              className={`org-card${isCurrent ? " is-current" : ""}`}
              key={organization.id}
              style={{ "--org-index": index } as React.CSSProperties}
            >
              <header>
                <div className="org-monogram" aria-hidden="true">
                  {initials(organization.name)}
                </div>
                <div className="org-card-title">
                  <span>{readableType(organization.organizationType)}</span>
                  <h2>{organization.name}</h2>
                </div>
                {isCurrent && <span className="org-current-badge">Active</span>}
              </header>

              <p className="org-description">
                {organization.description || "No description added yet."}
              </p>

              <div className="org-meta">
                <span><i /> {organization.status || "Active"}</span>
                <span>{organization.timeZone || "Local timezone"}</span>
              </div>

              <footer>
                <button
                  type="button"
                  className={isCurrent ? "org-selected" : "org-switch"}
                  disabled={isCurrent}
                  onClick={() => {
                    store.setOrganizations(organizations);
                    store.selectOrganization(organization.id);
                  }}
                >
                  {isCurrent ? "✓ Current workspace" : "Switch workspace"}
                </button>
                <Link
                  className="org-settings"
                  to={`/organizations/${organization.id}/settings`}
                  aria-label={`Open settings for ${organization.name}`}
                >
                  Settings <span>↗</span>
                </Link>
              </footer>
            </article>
          );
        })}
      </section>

      {active && (
        <p className="org-active-note">
          <span>●</span> Currently working in <strong>{active.name}</strong>
        </p>
      )}
    </AppShell>
  );
}
