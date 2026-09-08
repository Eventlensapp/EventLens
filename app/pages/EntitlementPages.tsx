"use client";

import { useQuery, useQueryClient } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { entitlementApi } from "../features/entitlements/api";
import PlanEditor from "../features/entitlements/PlanEditor";
import { useAppStore } from "../store/useAppStore";
import "../subscription.css";

export function AdminPlans() {
  const query = useQuery({ queryKey: ["admin-plans"], queryFn: () => entitlementApi.plans(true) });
  const queryClient = useQueryClient();
  return (
    <AppShell title="Subscription plans" eyebrow="Platform administration">
      <div className="event-head">
        <p>Feature entitlements and usage ceilings. No payment provider is connected.</p>
        <Link className="event-create" to="/admin/plans/create">+ Create plan</Link>
      </div>
      <div className="admin-plan-grid">
        {query.data?.map((plan) => (
          <article key={plan.id}>
            <strong>{plan.name}</strong><span>{plan.code}</span>
            <small>${plan.monthlyPrice}/month · {Object.values(plan.features).filter(Boolean).length} features</small>
            <div>
              <Link to={`/admin/plans/${plan.id}`}>Edit</Link>
              {plan.isActive && <button onClick={async () => {
                await entitlementApi.archive(plan.id);
                await queryClient.invalidateQueries({ queryKey: ["admin-plans"] });
              }}>Archive</button>}
            </div>
          </article>
        ))}
      </div>
    </AppShell>
  );
}

export const AdminPlanCreate = () => <PlanEditor create />;
export const AdminPlanDetails = () => <PlanEditor />;

const featureName = (value: string) => ({
  "ai.studio": "AI creative studio", "qr.gallery": "QR galleries", "booth.gif": "GIF booth",
  "booth.boomerang": "Boomerang booth", "unlimited.events": "Unlimited events",
  "unlimited.storage": "Unlimited storage", crm: "Guest CRM", gallery: "Online galleries",
  printing: "Print workflows", marketing: "Marketing tools",
}[value] || value.replace(/[._]/g, " ").replace(/\b\w/g, (letter) => letter.toUpperCase()));

export function OrganizationSubscriptionPage() {
  const organizationId = useAppStore((state) => state.activeOrganizationId);
  const subscription = useQuery({
    queryKey: ["org-entitlement", organizationId],
    queryFn: () => entitlementApi.subscription(organizationId),
    enabled: !!organizationId,
  });
  const plans = useQuery({ queryKey: ["entitlement-plans"], queryFn: () => entitlementApi.plans() });
  const queryClient = useQueryClient();
  if (!organizationId) return <AppShell title="Subscription"><p>Select an organization first.</p></AppShell>;
  const currentPlan = plans.data?.find((plan) => plan.id === subscription.data?.planId);

  return (
    <AppShell title="Subscription" eyebrow="Plan · Trial · Entitlements">
      <div className="subscription-page">
        <section className="subscription-hero">
          <div>
            <span className="subscription-label">CURRENT PLAN</span>
            <h2>{subscription.data?.planName || "Loading…"}</h2>
            <p>{currentPlan?.description || "Your active EventLens workspace plan."}</p>
          </div>
          <div className="subscription-status">
            <b>{subscription.data?.status || "Loading"}</b>
            <span>{subscription.data?.trialEnd
              ? `Trial ends ${new Date(subscription.data.trialEnd).toLocaleDateString()}`
              : subscription.data?.renewalDate
                ? `Renews ${new Date(subscription.data.renewalDate).toLocaleDateString()}`
                : "No renewal scheduled"}</span>
          </div>
        </section>

        <div className="subscription-heading">
          <div><span>FIND YOUR FIT</span><h2>Available plans</h2></div>
          <p>Choose the capacity and tools that match your organization.</p>
        </div>

        <div className="subscription-plan-grid">
          {plans.data?.map((plan, index) => {
            const active = plan.id === subscription.data?.planId;
            const enterprise = plan.code === "CustomEnterprise";
            const features = Object.entries(plan.features).filter(([, enabled]) => enabled).slice(0, 6);
            return (
              <article className={active ? "active" : ""} key={plan.id}>
                <div className="plan-top"><span>{active ? "CURRENT PLAN" : index > 2 ? "SCALE" : "PLAN"}</span>{active && <i>Active</i>}</div>
                <h3>{plan.name}</h3>
                <div className="plan-price">{enterprise
                  ? <><strong>Custom</strong><small>Talk to our team</small></>
                  : <><strong>${plan.monthlyPrice}</strong><small>/ month</small></>}</div>
                <p>{plan.description}</p>
                <div className="plan-divider" />
                <ul>{features.map(([key]) => <li key={key}><span>✓</span>{featureName(key)}</li>)}</ul>
                <button disabled={active} onClick={async () => {
                  if (!window.confirm(`Switch your organization to the ${plan.name} plan?`)) return;
                  await entitlementApi.change(organizationId, plan.id);
                  await queryClient.invalidateQueries({ queryKey: ["org-entitlement", organizationId] });
                }}>{active ? "Your current plan" : enterprise ? "Contact sales" : "Switch to this plan"}</button>
              </article>
            );
          })}
        </div>
        <div className="subscription-note"><span>ⓘ</span><p><strong>Plan changes are immediate.</strong> Entitlements update now; no payment is processed in this development phase.</p></div>
      </div>
    </AppShell>
  );
}

export function OrganizationUsagePage() {
  const organizationId = useAppStore((state) => state.activeOrganizationId);
  const query = useQuery({
    queryKey: ["entitlement-usage", organizationId],
    queryFn: () => entitlementApi.usage(organizationId),
    enabled: !!organizationId,
  });
  const metricDetails: Record<string, { label: string; note: string; icon: string }> = {
    EventsCreated: { label: "Events", note: "Events created", icon: "◇" },
    PhotosCaptured: { label: "Photo captures", note: "Photos captured this period", icon: "▣" },
    AIGenerations: { label: "AI generations", note: "AI-powered creations", icon: "✦" },
    StorageConsumed: { label: "Storage", note: "Files and brand assets", icon: "▤" },
    GalleryViews: { label: "Gallery views", note: "Guest gallery visits", icon: "◉" },
    Downloads: { label: "Downloads", note: "Asset downloads", icon: "↓" },
    Users: { label: "Team members", note: "Organization seats", icon: "●" },
    ApiRequests: { label: "API requests", note: "Requests this period", icon: "⌁" },
    Exports: { label: "Exports", note: "Completed exports", icon: "↗" },
    QrScans: { label: "QR scans", note: "Guest QR interactions", icon: "⌗" },
    Templates: { label: "Templates", note: "Reusable templates", icon: "▧" },
    Uploads: { label: "Uploads", note: "Files uploaded", icon: "↑" },
    BoothSessions: { label: "Booth sessions", note: "Guest booth sessions", icon: "◎" },
  };
  const entries = Object.entries(query.data?.usage || {});
  const period = query.data ? `${new Date(query.data.from).toLocaleDateString(undefined, { month: "short", day: "numeric" })} – ${new Date(query.data.to).toLocaleDateString(undefined, { month: "short", day: "numeric", year: "numeric" })}` : "Current billing period";
  const displayValue = (key: string, value: number) => key === "StorageConsumed"
    ? value >= 1073741824 ? `${(value / 1073741824).toFixed(1)} GB` : value >= 1048576 ? `${(value / 1048576).toFixed(1)} MB` : `${(value / 1024).toFixed(1)} KB`
    : value.toLocaleString();
  const displayLimit = (key: string, value: number) => key === "StorageConsumed"
    ? displayValue(key, value)
    : value.toLocaleString();
  return <AppShell title="Usage" eyebrow="Current entitlement period"><div className="entitlement-usage">
    <section className="usage-summary">
      <div><span>PLAN UTILIZATION</span><h2>Organization usage</h2><p>Monitor your plan capacity and activity in one place.</p></div>
      <div className="usage-period"><small>Current period</small><strong>{period}</strong></div>
    </section>
    {query.isLoading ? <div className="usage-loading">Loading usage…</div> : <div className="entitlement-usage-grid">
      {entries.map(([key, value]) => {
        const limit = query.data?.limits[key] ?? -1;
        const unlimited = limit < 0;
        const percentage = unlimited ? 0 : Math.min(100, value / Math.max(1, limit) * 100);
        const detail = metricDetails[key] ?? { label: featureName(key), note: "Current period usage", icon: "•" };
        const nearingLimit = !unlimited && percentage >= 80;
        return <article className={nearingLimit ? "warning" : ""} key={key}>
          <div className="usage-card-head"><span>{detail.icon}</span><small>{unlimited ? "UNLIMITED" : `${Math.round(percentage)}% USED`}</small></div>
          <h3>{detail.label}</h3><p>{detail.note}</p>
          <div className="usage-value"><strong>{displayValue(key, value)}</strong><small>{unlimited ? "No plan limit" : `of ${displayLimit(key, limit)}`}</small></div>
          <div className={`usage-progress ${unlimited ? "unlimited" : ""}`}><i style={{ width: unlimited ? "100%" : `${percentage}%` }} /></div>
        </article>;
      })}
    </div>}
  </div></AppShell>;
}
