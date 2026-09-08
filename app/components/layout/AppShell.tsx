"use client";

import { useEffect, useRef, useState, type ReactNode } from "react";
import { NavLink, useLocation, useNavigate } from "react-router-dom";
import { apiRequest, storageKeys } from "../../lib/api";
import { useAppStore } from "../../store/useAppStore";
import { CameraIcon, GalleryIcon, SparkleIcon } from "../ui/Icons";
import "../../sidebar-navigation.css";

const navGroups = [
  { label: "Events & planning", icon: "E", items: [
    { to: "/events", label: "Events", icon: "◇" },
    { to: "/event-templates", label: "Templates", icon: "▤" },
    { to: "/event-types", label: "Event types", icon: "T" },
  ]},
  { label: "Create & deliver", icon: "✦", items: [
    { to: "/booth", label: "Booth platform", icon: "◎" },
    { to: "/booth/camera", label: "Camera foundation", icon: "◉" },
    { to: "/booth/camera-controls", label: "Camera controls", icon: "◌" },
    { to: "/booth/professional-cameras", label: "Professional cameras", icon: "◉" },
    { to: "/booth/session", label: "Booth sessions", icon: "◇" },
    { to: "/booth/capture", label: "Photo capture", icon: "◎" },
    { to: "/booth/capture-modes", label: "Capture modes", icon: "◫" },
    { to: "/ai", label: "AI Studio", icon: "✦" },
    { to: "/editor", label: "Photo editor", icon: "◐" },
    { to: "/gallery", label: "Guest gallery", icon: "▦" },
  ]},
  { label: "People & access", icon: "◎", items: [
    { to: "/team", label: "Team members", icon: "◎" },
    { to: "/departments", label: "Departments", icon: "D" },
    { to: "/branches", label: "Branches", icon: "B" },
    { to: "/access", label: "Roles & access", icon: "◆" },
    { to: "/crm", label: "Guest CRM", icon: "C" },
  ]},
  { label: "Brand & files", icon: "▣", items: [
    { to: "/branding", label: "Brand Kit", icon: "◐" },
    { to: "/storage", label: "Storage", icon: "▣" },
  ]},
  { label: "Reports & billing", icon: "◒", items: [
    { to: "/analytics", label: "Analytics", icon: "◒" },
    { to: "/organization/usage", label: "Usage", icon: "◔" },
    { to: "/organization/subscription", label: "Plan & limits", icon: "◇" },
    { to: "/subscription", label: "Billing", icon: "$" },
  ]},
];

const mainPages = new Set([
  "/dashboard", "/organizations", "/events", "/event-types", "/event-templates",
  "/booth", "/camera", "/ai", "/editor", "/gallery", "/team", "/departments", "/branches",
  "/access", "/crm", "/branding", "/storage", "/analytics",
  "/organization/usage", "/organization/subscription", "/subscription",
]);

export function Brand() {
  return <NavLink to="/" className="brand"><span className="brand-mark"><SparkleIcon /></span><span>EventLens<span className="brand-ai">AI</span></span></NavLink>;
}

export function AppShell({ children, title, eyebrow }: { children: ReactNode; title: string; eyebrow?: string }) {
  const state = useAppStore();
  const location = useLocation();
  const navigate = useNavigate();
  const [organizationMenuOpen, setOrganizationMenuOpen] = useState(false);
  const [accountMenuOpen, setAccountMenuOpen] = useState(false);
  const organizationMenuRef = useRef<HTMLDivElement>(null);
  const accountMenuRef = useRef<HTMLDivElement>(null);
  const initials = state.userName.split(" ").map((part) => part[0]).join("").slice(0, 2).toUpperCase();
  const activeOrganization = state.organizations.find((organization) => organization.id === state.activeOrganizationId);
  const showBackButton = !mainPages.has(location.pathname);

  useEffect(() => {
    const closeMenu = (event: MouseEvent) => {
      if (!organizationMenuRef.current?.contains(event.target as Node)) setOrganizationMenuOpen(false);
      if (!accountMenuRef.current?.contains(event.target as Node)) setAccountMenuOpen(false);
    };
    document.addEventListener("mousedown", closeMenu);
    return () => document.removeEventListener("mousedown", closeMenu);
  }, []);

  async function logout() {
    const refreshToken = localStorage.getItem(storageKeys.refreshToken);
    try {
      await apiRequest("/api/auth/logout", { method: "POST", body: JSON.stringify({ refreshToken }) });
    } catch {}
    state.signOut();
  }

  return <div className={state.darkMode ? "app dark" : "app light"}>
    <aside className={`sidebar ${state.mobileNavOpen ? "open" : ""}`}>
      <div className="sidebar-top"><Brand /><button className="mobile-close" onClick={state.closeMobileNav}>×</button></div>
      <nav className="sidebar-navigation" aria-label="Main navigation">
        <div className="sidebar-primary">
          <NavLink to="/dashboard" onClick={state.closeMobileNav} className={({ isActive }) => isActive ? "active" : ""}><span aria-hidden="true">⌂</span><b>Dashboard</b></NavLink>
          <NavLink to="/organizations" onClick={state.closeMobileNav} className={({ isActive }) => isActive ? "active" : ""}><span aria-hidden="true">▦</span><b>Organizations</b></NavLink>
        </div>
        {navGroups.map((group) => {
          const containsCurrentPage = group.items.some((item) => location.pathname === item.to || location.pathname.startsWith(`${item.to}/`));
          return <details className="sidebar-nav-group" key={group.label} open={containsCurrentPage || undefined}>
            <summary><span className="sidebar-group-icon" aria-hidden="true">{group.icon}</span><b>{group.label}</b><i aria-hidden="true">⌄</i></summary>
            <div>{group.items.map((item) => <NavLink key={item.to} to={item.to} onClick={state.closeMobileNav} className={({ isActive }) => isActive ? "active" : ""}><span aria-hidden="true">{item.icon}</span><b>{item.label}</b></NavLink>)}</div>
          </details>;
        })}
      </nav>
      <div className="sidebar-foot">
        <div className="credit-card"><div><SparkleIcon /><span><strong>{state.credits}</strong> credits left</span></div><div className="credit-bar"><span /></div><NavLink to="/billing/upgrade">Upgrade plan <span>→</span></NavLink></div>
        <div className="profile"><span className="avatar">{initials}</span><div><strong>{state.userName}</strong><span>{state.user?.roles.join(", ") || "Member"}</span></div><button onClick={logout} aria-label="Sign out">↪</button></div>
      </div>
    </aside>
    {state.mobileNavOpen && <button className="scrim" onClick={state.closeMobileNav} aria-label="Close navigation" />}
    <main className="main"><header className="topbar"><button className="menu" onClick={state.toggleMobileNav}>☰</button><div className="page-heading">
      {showBackButton && <button className="page-back" type="button" aria-label="Go back to previous page" onClick={() => window.history.length > 1 ? navigate(-1) : navigate("/dashboard")}><span>←</span></button>}
      <div><span>{eyebrow}</span><h1>{title}</h1></div>
    </div><div className="top-actions">
      {state.organizations.length > 0 && <div className="organization-switcher" ref={organizationMenuRef}>
        <button className="organization-switcher-trigger" type="button" aria-haspopup="listbox" aria-expanded={organizationMenuOpen} onClick={() => { setOrganizationMenuOpen((open) => !open); setAccountMenuOpen(false); }}>
          <span className="organization-switcher-mark">{activeOrganization?.name.slice(0, 1).toUpperCase()}</span>
          <span className="organization-switcher-copy"><small>WORKSPACE</small><b>{activeOrganization?.name}</b></span>
          <i aria-hidden="true">⌄</i>
        </button>
        {organizationMenuOpen && <div className="organization-switcher-menu" role="listbox" aria-label="Choose workspace">
          <header><span>SWITCH WORKSPACE</span><small>{state.organizations.length} available</small></header>
          {state.organizations.map((organization) => {
            const selected = organization.id === state.activeOrganizationId;
            return <button key={organization.id} role="option" aria-selected={selected} type="button" onClick={() => {
              setOrganizationMenuOpen(false);
              if (!selected) {
                state.selectOrganization(organization.id);
                window.location.reload();
              }
            }}>
              <span>{organization.name.slice(0, 1).toUpperCase()}</span>
              <b>{organization.name}</b>
              {selected && <i>✓</i>}
            </button>;
          })}
          <NavLink to="/organizations" onClick={() => setOrganizationMenuOpen(false)}>Manage organizations <span>→</span></NavLink>
        </div>}
      </div>}
      <button className="theme" onClick={state.toggleDarkMode} aria-label={state.darkMode ? "Use light theme" : "Use dark theme"}>{state.darkMode ? "☀" : "☾"}</button>
      <div className="account-menu" ref={accountMenuRef}>
        <button className="top-avatar account-menu-trigger" type="button" aria-haspopup="menu" aria-expanded={accountMenuOpen} aria-label="Open account menu" onClick={() => { setAccountMenuOpen((open) => !open); setOrganizationMenuOpen(false); }}>{initials}</button>
        {accountMenuOpen && <div className="account-menu-popover" role="menu">
          <header><span className="account-menu-avatar">{initials}</span><div><strong>{state.userName}</strong><small>{state.user?.email || state.user?.roles.join(", ") || "Member"}</small></div></header>
          <div className="account-menu-context"><small>ACTIVE WORKSPACE</small><b>{activeOrganization?.name || "No organization selected"}</b></div>
          <NavLink role="menuitem" to="/organizations" onClick={() => setAccountMenuOpen(false)}><span>▦</span>Manage organizations<i>→</i></NavLink>
          <button role="menuitem" type="button" onClick={state.toggleDarkMode}><span>{state.darkMode ? "☀" : "☾"}</span>{state.darkMode ? "Switch to light mode" : "Switch to dark mode"}<i /></button>
          <button className="account-signout" role="menuitem" type="button" onClick={logout}><span>↪</span>Sign out<i /></button>
        </div>}
      </div>
    </div></header><div className="content">{children}</div></main>
  </div>;
}

export function PrimaryButton({ children, to = "/camera" }: { children: ReactNode; to?: string }) {
  return <NavLink to={to} className="primary-btn"><CameraIcon />{children}<span>→</span></NavLink>;
}

export { GalleryIcon };
