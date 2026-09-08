"use client";

import { useMutation, useQuery } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { eventApi } from "../features/events/api";
import "../event-dashboard.css";
import "../event-launch-actions.css";

const metricMeta: Record<string, { icon: string; note: string }> = {
  PHOTOS: { icon: "▣", note: "Captured at this event" },
  GUESTS: { icon: "●", note: "Registered guests" },
  GALLERY: { icon: "◇", note: "Guest delivery status" },
  BOOTH: { icon: "◎", note: "Capture experience" },
  "AI JOBS": { icon: "✦", note: "Processing activity" },
};

export default function EventDashboard() {
  const { id = "" } = useParams();
  const navigate = useNavigate();
  const dashboard = useQuery({ queryKey: ["event-dashboard", id], queryFn: () => eventApi.dashboard(id), enabled: !!id });
  const clone = useMutation({ mutationFn: (body: unknown) => eventApi.clone(id, body), onSuccess: (event) => navigate(`/events/${event.id}`) });
  if (dashboard.isLoading) return <AppShell title="Event"><div className="event-dashboard-loading">Preparing event workspace…</div></AppShell>;
  if (dashboard.error || !dashboard.data) return <AppShell title="Event"><div className="form-error">{dashboard.error?.message || "Event not found."}</div></AppShell>;

  const data = dashboard.data;
  const event = data.event;
  const start = new Date(event.startDate);
  const end = new Date(event.endDate);
  const metrics: Array<[string, string | number]> = [
    ["PHOTOS", data.photoCount], ["GUESTS", data.guestCount], ["GALLERY", data.galleryStatus],
    ["BOOTH", data.boothStatus], ["AI JOBS", data.aiJobs],
  ];

  return (
    <AppShell title={event.name} eyebrow={`${data.organizationName} · ${data.eventTypeName || "Event"}`}>
      <div className="event-dashboard-page">
        <section className="event-dashboard-hero">
          <div className="event-date-tile"><span>{start.toLocaleDateString(undefined, { month: "short" }).toUpperCase()}</span><strong>{start.getDate()}</strong><small>{start.getFullYear()}</small></div>
          <div className="event-hero-copy">
            <div><span className={`event-status ${String(event.status).toLowerCase()}`}>{String(event.status)}</span><small>{data.eventTypeName || "Event"}</small></div>
            <h2>{event.name}</h2>
            <p>{event.description || "Your event command center is ready."}</p>
            <div className="event-hero-meta">
              <span><b>⌖</b>{event.venue || "Venue not set"}{event.city ? `, ${event.city}` : ""}</span>
              <span><b>◷</b>{start.toLocaleString(undefined, { weekday: "short", hour: "2-digit", minute: "2-digit" })} – {end.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}</span>
            </div>
          </div>
          <div className="event-hero-actions">
            <Link to={`/events/${id}/settings`}>Settings</Link>
            <Link className="primary" to={`/events/${id}/operations`}>Open operations →</Link>
          </div>
        </section>

        <nav className="event-quick-nav">
          {[
            ["operations", "⌁", "Operations"], ["schedule", "▦", "Schedule"], ["team", "●", "Team"],
            ["branding", "◐", "Branding"], ["assets", "▣", "Assets"], ["qr", "⌗", "QR & access"],
          ].map(([path, icon, label]) => <Link key={path} to={`/events/${id}/${path}`}><span>{icon}</span>{label}<b>→</b></Link>)}
        </nav>

        <section className="event-launch-actions" aria-label="Event launch actions">
          <header><div><span>LAUNCH</span><h2>Guest access and booth testing</h2></div><p>The event is selected automatically.</p></header>
          <div>
            <Link to={`/events/${id}/qr`}><span>QR</span><strong>Generate QR codes</strong><small>Create secure guest entry links</small><b>→</b></Link>
            <Link to={`/events/${id}/access`}><span>AC</span><strong>Public-access settings</strong><small>Control guest access and expiry</small><b>→</b></Link>
            <Link to="/booth/session"><span>BS</span><strong>Start booth session</strong><small>Launch the operator workflow</small><b>→</b></Link>
            <Link to="/booth/capture"><span>TC</span><strong>Test photo capture</strong><small>Verify the capture experience</small><b>→</b></Link>
          </div>
        </section>

        <div className="event-dashboard-metrics">
          {metrics.map(([label, value]) => <article key={label}>
            <div><span>{metricMeta[label].icon}</span><small>{label}</small></div>
            <strong>{value}</strong><p>{metricMeta[label].note}</p>
          </article>)}
        </div>

        <div className="event-dashboard-lower">
          <section className="event-team-panel">
            <div className="event-panel-title"><div><span>PEOPLE</span><h2>Assigned team</h2></div><Link to={`/events/${id}/team`}>Manage team →</Link></div>
            {data.assignedTeam.length
              ? <div className="event-team-list">{data.assignedTeam.map((member) => <article key={member.userId}><i>{member.firstName[0]}{member.lastName[0]}</i><div><strong>{member.firstName} {member.lastName}</strong><small>{String(member.role).replace(/([A-Z])/g, " $1")}</small></div><b>Active</b></article>)}</div>
              : <div className="event-panel-empty"><span>●</span><div><strong>No team assigned</strong><p>Add photographers, coordinators and operators to this event.</p></div><Link to={`/events/${id}/team`}>Assign team</Link></div>}
          </section>

          <aside className="event-readiness-panel">
            <div className="event-panel-title"><div><span>GET READY</span><h2>Workspace setup</h2></div></div>
            <ul>
              <li className={event.venue ? "done" : ""}><span>{event.venue ? "✓" : "1"}</span><div><strong>Venue details</strong><small>{event.venue ? "Configured" : "Add venue and address"}</small></div></li>
              <li className={data.assignedTeam.length ? "done" : ""}><span>{data.assignedTeam.length ? "✓" : "2"}</span><div><strong>Event team</strong><small>{data.assignedTeam.length ? `${data.assignedTeam.length} assigned` : "Assign your operators"}</small></div></li>
              <li className={data.galleryStatus !== "Not configured" ? "done" : ""}><span>{data.galleryStatus !== "Not configured" ? "✓" : "3"}</span><div><strong>Guest gallery</strong><small>{data.galleryStatus}</small></div></li>
              <li className={data.boothStatus !== "Not configured" ? "done" : ""}><span>{data.boothStatus !== "Not configured" ? "✓" : "4"}</span><div><strong>Booth experience</strong><small>{data.boothStatus}</small></div></li>
            </ul>
          </aside>
        </div>

        <div className="event-dashboard-footer">
          <p>Need a similar event?</p>
          <button disabled={clone.isPending} onClick={() => {
            const name = window.prompt("Name for cloned event", `${event.name} Copy`);
            if (name) clone.mutate({ name, startDate: event.startDate, endDate: event.endDate });
          }}>{clone.isPending ? "Cloning…" : "Clone this event"}</button>
        </div>
      </div>
    </AppShell>
  );
}
