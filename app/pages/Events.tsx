"use client";

import { useState } from "react";
import { Link } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AppShell } from "../components/layout/AppShell";
import { eventApi } from "../features/events/api";
import { useAppStore } from "../store/useAppStore";
import "../events.css";

const statuses = ["Draft", "Upcoming", "Active", "Completed", "Cancelled"];

export default function Events() {
  const organizationId = useAppStore((state) => state.activeOrganizationId);
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [statusOpen, setStatusOpen] = useState(false);
  const queryClient = useQueryClient();
  const query = new URLSearchParams({
    organizationId,
    pageSize: "100",
    search,
    ...(status ? { status } : {}),
  }).toString();
  const events = useQuery({
    queryKey: ["events", organizationId, search, status],
    queryFn: () => eventApi.list(query),
    enabled: !!organizationId,
  });
  const archive = useMutation({
    mutationFn: eventApi.archive,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["events"] }),
  });
  const hasFilters = Boolean(search || status);

  return (
    <AppShell title="Events" eyebrow="Plan · Operate · Deliver">
      <div className="events-page">
        <section className="events-intro">
          <div>
            <span>EVENT OPERATIONS</span>
            <h2>Every event, one workspace.</h2>
            <p>Plan experiences, assign your team, manage galleries and track delivery from one place.</p>
          </div>
          <Link to="/events/create"><b>＋</b><span>Create event<small>Start planning</small></span></Link>
        </section>

        <div className="events-toolbar">
          <label className="events-search">
            <span>⌕</span>
            <input aria-label="Search events" placeholder="Search by event name…" value={search} onChange={(event) => setSearch(event.target.value)} />
            {search && <button aria-label="Clear search" onClick={() => setSearch("")}>×</button>}
          </label>
          <div className={`events-status-filter ${statusOpen ? "open" : ""}`}>
            <button
              type="button"
              aria-label="Filter status"
              aria-haspopup="listbox"
              aria-expanded={statusOpen}
              onClick={() => setStatusOpen((open) => !open)}
            >
              <b aria-hidden="true">☷</b><span>{status || "All events"}</span><i>⌄</i>
            </button>
            {statusOpen && <div className="events-status-menu" role="listbox">
              {["", ...statuses].map((item) => (
                <button
                  type="button"
                  role="option"
                  aria-selected={status === item}
                  className={status === item ? "selected" : ""}
                  key={item || "all"}
                  onClick={() => { setStatus(item); setStatusOpen(false); }}
                >
                  <span>{item || "All events"}</span>{status === item && <b>✓</b>}
                </button>
              ))}
            </div>}
          </div>
        </div>

        {events.error && <div className="form-error">{events.error.message}</div>}
        {events.isLoading && <div className="events-loading">Loading your events…</div>}

        {!events.isLoading && !events.data?.items.length && (
          <section className="events-empty">
            <div className="events-empty-mark"><span>◇</span><i>＋</i></div>
            <span>{hasFilters ? "NO MATCHES" : "YOUR FIRST EVENT STARTS HERE"}</span>
            <h2>{hasFilters ? "No events match these filters" : "Create something memorable."}</h2>
            <p>{hasFilters ? "Try a different search term or reset the status filter." : "Set the date, venue and team. EventLens will prepare the workspace for everything that follows."}</p>
            {hasFilters
              ? <button onClick={() => { setSearch(""); setStatus(""); }}>Clear all filters</button>
              : <Link to="/events/create">＋ Create your first event</Link>}
          </section>
        )}

        <section className="events-card-grid">
          {events.data?.items.map((event) => (
            <article key={event.id}>
              <div className="events-card-top">
                <span className={`event-status ${String(event.status).toLowerCase()}`}>{String(event.status)}</span>
                <small>{String(event.eventType).replace(/([A-Z])/g, " $1")}</small>
              </div>
              <h2>{event.name}</h2>
              <p><span>⌖</span>{event.venue || "Venue not set"}</p>
              <div className="events-date">
                <span>{new Date(event.startDate).toLocaleDateString(undefined, { month: "short" }).toUpperCase()}</span>
                <strong>{new Date(event.startDate).getDate()}</strong>
                <p>{new Date(event.startDate).toLocaleDateString(undefined, { weekday: "long", year: "numeric" })}<small>{new Date(event.startDate).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}</small></p>
              </div>
              <footer>
                <Link to={`/events/${event.id}`}>Open workspace <span>→</span></Link>
                <button disabled={archive.isPending} onClick={() => window.confirm(`Archive ${event.name}?`) && archive.mutate(event.id)}>Archive</button>
              </footer>
            </article>
          ))}
        </section>
      </div>
    </AppShell>
  );
}
