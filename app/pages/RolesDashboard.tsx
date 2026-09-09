"use client";

import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { teamApi } from "../features/organizations/teamApi";
import { useAppStore } from "../store/useAppStore";
import "../roles-dashboard.css";

type RoleDefinition = { name: string; label: string; scope: "Platform" | "Organization" | "Event"; summary: string; capabilities: string[]; assignable: boolean };

const roles: RoleDefinition[] = [
  { name: "SuperAdmin", label: "Super Admin", scope: "Platform", summary: "Full platform authority across every organization.", capabilities: ["Platform configuration", "All organizations", "Security oversight"], assignable: false },
  { name: "PlatformAdmin", label: "Platform Admin", scope: "Platform", summary: "Runs day-to-day platform administration without super-admin ownership.", capabilities: ["Platform operations", "Organization support", "Administrative reporting"], assignable: false },
  { name: "Owner", label: "Organization Owner", scope: "Organization", summary: "Owns an organization and its administrative controls.", capabilities: ["Billing and settings", "Team management", "Ownership transfer"], assignable: false },
  { name: "Manager", label: "Manager", scope: "Organization", summary: "Coordinates people, events, and operational access.", capabilities: ["Team invitations", "Event management", "Operational access"], assignable: true },
  { name: "Photographer", label: "Photographer", scope: "Organization", summary: "Captures and manages event photography workflows.", capabilities: ["Camera workflows", "Photo capture", "Assigned events"], assignable: true },
  { name: "BoothOperator", label: "Booth Operator", scope: "Organization", summary: "Operates booths and monitors live capture sessions.", capabilities: ["Booth setup", "Session operation", "Device checks"], assignable: true },
  { name: "Designer", label: "Designer", scope: "Organization", summary: "Creates templates and visual event experiences.", capabilities: ["Templates", "Brand assets", "Experience design"], assignable: true },
  { name: "MarketingManager", label: "Marketing Manager", scope: "Organization", summary: "Manages campaigns, guest engagement, and reporting.", capabilities: ["Guest CRM", "Campaign assets", "Analytics"], assignable: true },
  { name: "Guest", label: "Guest", scope: "Event", summary: "Uses public event and booth experiences without staff access.", capabilities: ["Public event access", "Booth experience", "Permitted sharing"], assignable: false },
  { name: "Viewer", label: "Viewer", scope: "Organization", summary: "Receives read-only access to permitted workspace content.", capabilities: ["View assigned content", "Read reports", "No administrative changes"], assignable: true },
];
const emptyRoles: string[] = [];

export default function RolesDashboard() {
  const organizationId = useAppStore(state => state.activeOrganizationId);
  const organizations = useAppStore(state => state.organizations);
  const userRoles = useAppStore(state => state.user?.roles ?? emptyRoles);
  const organization = organizations.find(item => item.id === organizationId);
  const members = useQuery({ queryKey: ["team", organizationId], queryFn: () => teamApi.members(organizationId), enabled: !!organizationId });
  const counts = new Map<string, number>();
  members.data?.forEach(member => counts.set(member.role, (counts.get(member.role) ?? 0) + 1));

  return <AppShell title="Roles dashboard" eyebrow="Platform · Organization · Event access">
    <section className="roles-hero"><div><span>Access model</span><h2>One view of every EventLens role.</h2><p>Review role scope, intended permissions, and live assignments in {organization?.name ?? "the selected organization"}.</p></div><Link to="/team">Manage team <span>→</span></Link></section>
    <section className="roles-stats" aria-label="Role overview">
      <article><span>Defined roles</span><strong>{roles.length}</strong><small>Across three access scopes</small></article>
      <article><span>Organization members</span><strong>{members.data?.length ?? 0}</strong><small>{members.isLoading ? "Loading assignments…" : organization?.name ?? "Select an organization"}</small></article>
      <article><span>Roles in use</span><strong>{counts.size}</strong><small>In the active organization</small></article>
      <article><span>Your access</span><strong>{userRoles.length}</strong><small>{userRoles.join(", ") || "No role found"}</small></article>
    </section>
    {members.error && <div className="form-error">Could not load organization role counts: {members.error.message}</div>}
    <section className="roles-panel"><header><div><span>Role directory</span><h2>Roles and responsibilities</h2></div><small>OrganizationOwner is stored as Owner</small></header><div className="roles-grid">
      {roles.map(role => {
        const isCurrent = userRoles.includes(role.name) || (role.name === "Owner" && userRoles.includes("OrganizationOwner"));
        const count = role.scope === "Organization" ? counts.get(role.name) ?? 0 : null;
        return <article className="role-card" key={role.name}><div className="role-card-head"><span className={`role-scope role-scope-${role.scope.toLowerCase()}`}>{role.scope}</span>{isCurrent && <span className="role-current">Your role</span>}</div><h3>{role.label}</h3><code>{role.name}</code><p>{role.summary}</p><ul>{role.capabilities.map(item => <li key={item}>{item}</li>)}</ul><footer><div><strong>{count ?? "—"}</strong><span>{count === null ? "Platform/event count restricted" : count === 1 ? "member assigned" : "members assigned"}</span></div><span className={role.assignable ? "role-assignable" : "role-controlled"}>{role.assignable ? "Team assignable" : "System controlled"}</span></footer></article>;
      })}
    </div></section>
  </AppShell>;
}
