# EventLens AI Product Vision

> **Document type:** Product strategy  
> **Product:** EventLens AI  
> **Category:** AI-powered event experience platform  
> **Engineering authority:** This document complements, and does not supersede, [`PROJECT_RULES.md`](../PROJECT_RULES.md).  
> **Status language:** “Current” means represented in the repository today; “planned” means roadmap intent, not a delivery commitment.

## Executive Summary

EventLens AI exists to help event professionals create memorable guest experiences and convert those experiences into measurable business value. Traditional photo-booth products usually stop after capture, printing, or sharing. Event management suites often handle registration and schedules but do not own the creative moment. Marketing tools receive fragmented data after the event, if they receive it at all.

EventLens AI unifies these disconnected activities into one multi-tenant SaaS platform: event setup, browser-based capture, professional photo processing, AI-assisted creativity, galleries, printing, consent-aware CRM, automation, analytics, subscriptions, and extensibility.

Our guiding goal is:

> **Transform every event into an intelligent, interactive, measurable experience.**

The platform must work at two very different speeds. During an event it must be immediate, resilient, touch-friendly, and understandable without training. Before and after an event it must give owners, agencies, designers, marketers, and enterprise teams the controls, auditability, insights, and automation they need to operate at scale.

EventLens AI is therefore not merely photo-booth software. It is an event operating system with a creative capture experience at its center.

## Product Thesis

Every capture is more than an image. With permission, it can become:

- A branded guest interaction.
- A shareable memory.
- A consented customer relationship.
- A real-time signal about event engagement.
- A trigger for an automated workflow.
- An input to an event story, memory book, or highlight reel.
- Evidence that helps an organizer improve the next event.

The winning platform will make that lifecycle feel like one continuous experience rather than a collection of integrations. It will remain approachable for a single photographer while scaling to agencies, franchises, brands, and enterprise portfolios.

## Market Problem

### Fragmented tools

Event operators commonly assemble separate products for booth capture, templates, cloud galleries, forms, email delivery, analytics, billing, and client reporting. This creates repeated setup, inconsistent branding, duplicate guest records, weak attribution, and expensive support.

### Hardware dependency

Many professional solutions assume a Windows workstation, a specific DSLR configuration, wired peripherals, and specialist setup. That remains important for premium installations, but it excludes lightweight mobile activations and creates a single point of operational failure. A browser-first platform can serve phones, tablets, webcams, and kiosks while preserving a future adapter boundary for professional cameras.

### Expensive setup and operation

Licensing, dedicated hardware, manual template creation, local file transfer, and staff training raise the cost of every activation. Small teams need a credible entry point; agencies need reusable configurations, remote visibility, and predictable operations.

### Limited branding

Basic overlays are not enough for brand activations. Customers expect end-to-end control over booth screens, templates, galleries, communications, domains, exports, and client-facing portals. Branding must be reusable through a governed brand kit rather than recreated per screen.

### Missing CRM and consent context

Photo delivery often captures an email address without creating a useful, consent-aware guest profile. EventLens AI should preserve the purpose, source, version, and timing of consent, connect engagement to a guest timeline, and respect suppression and privacy requests.

### Weak analytics

Counts such as “photos taken” do not answer business questions. Customers need to understand guest conversion, booth utilization, sharing behavior, QR effectiveness, popular experiences, staffing pressure, campaign outcomes, and value across events.

### Limited AI creativity

AI features are frequently isolated effects without a reliable job lifecycle, entitlement controls, moderation, cost visibility, or reusable brand context. EventLens AI treats AI as a provider-neutral platform capability, not a collection of hard-coded model calls.

### Limited automation

Manual exporting, uploading, messaging, and reporting delays follow-up and causes errors. Event activity should safely trigger processing, gallery publication, communications, CRM updates, exports, and alerts under explicit tenant-owned rules.

## Target Customers

### 1. Wedding planners

Wedding planners need a polished guest journey that complements the event theme without creating another operational burden. They benefit from reusable wedding templates, guest-friendly booth modes, private galleries, QR access, print options, memory books, and post-event thank-you workflows. Privacy, family-friendly moderation, and simple handoff to the couple are essential.

### 2. Event agencies

Agencies coordinate multiple clients, brands, venues, crews, and concurrent events. They require organization-scoped access, reusable assets, team roles, multi-booth supervision, remote health visibility, client reporting, white-label delivery, predictable billing, and an integration layer. Their buying decision depends on operational scale and reduced setup time.

### 3. Photographers

Photographers need dependable capture, high-resolution output, color-conscious processing, template flexibility, printing, fast delivery, and portfolio-quality results. Solo creators need simplicity; studios need staff access, repeatable workflows, storage controls, and branded client galleries. A future DSLR adapter must preserve professional workflows without coupling the core platform to one camera vendor.

### 4. Corporate marketing teams

Corporate marketers need campaign consistency, lead capture, consent, branded AI experiences, segmentation, attribution, analytics, exports, and integrations with CRM and marketing systems. They also need review workflows, auditability, data retention controls, and enterprise identity.

### 5. Schools and universities

Education customers run graduations, reunions, fairs, sports events, and recruitment activations. They need affordable deployment, clear privacy settings, role separation, accessible guest flows, controlled sharing, and safe moderation. Large institutions may require SSO and department-level reporting.

### 6. Brand activation companies

Activation specialists optimize for attention, participation, sharing, and lead conversion. They need distinctive capture modes, fast branded output, AI transformations, QR flows, consented forms, real-time dashboards, integrations, offline resilience, and evidence of campaign performance.

### 7. Shopping malls

Malls need persistent or seasonal kiosks with high throughput, touch-friendly recovery, scheduled campaigns, multilingual experiences, coupon delivery, repeat-visitor insight, and centralized management across locations. Device health, queue behavior, and privacy notices must be operationally clear.

### 8. Entertainment venues

Theme parks, museums, clubs, theaters, arenas, and attractions need repeatable capture experiences tied to venues, sessions, shows, or admissions. They benefit from fast retrieval, printing, upsell opportunities, branded galleries, API access, and integrations with ticketing or loyalty platforms.

## User Personas

| Persona | Scope | Primary outcomes |
|---|---|---|
| Super Admin | Entire SaaS platform | Operate tenants, safety, providers, plans, and platform health with audited elevation |
| Platform Admin | Delegated platform operations | Support customers, inspect operational status, moderate where authorized, and manage configuration |
| Organization Owner | One or more organizations | Control subscription, security, teams, branding, events, storage, and business performance |
| Event Manager | Assigned organization/events | Configure schedules, venues, booths, guest flows, galleries, staff, and event reporting |
| Photographer | Assigned events | Operate capture, review output, manage quality, processing, print, and delivery |
| Booth Operator | Assigned booth/event | Start and recover sessions, switch camera, monitor capture, printing, and guest flow |
| Designer | Organization/event creative assets | Build templates, brand kits, overlays, animations, and reusable visual systems |
| Marketing Manager | Authorized audience and campaigns | Define forms, consent, segments, campaigns, automations, and attribution reports |
| Guest | Personal event interaction | Capture, review, consent, receive, download, share, and manage personal data choices |
| Viewer | Read-only authorized access | Review event status, galleries, or reports without operational mutation |

Roles express permission boundaries, not job titles. Organizations may compose narrower permissions as the authorization model matures.

## Competitive Positioning

Competitor observations describe market positioning and inspiration, not claims about every current competitor release.

| Product | Recognized strength | EventLens AI direction |
|---|---|---|
| DSLRBooth | Professional booth capture, DSLR and print workflows | Preserve professional reliability while adding browser/mobile deployment, multi-tenant operations, CRM, analytics, provider-neutral AI, and SaaS administration |
| FotoShare Cloud | Gallery delivery and sharing | Connect delivery to capture, consent, guest history, automation, analytics, and organization-owned workflows |
| Simple Booth | Accessible, polished guest activation | Combine ease of use with deeper event operations, creative tooling, enterprise controls, and measurable business outcomes |
| Snappic | Branded activations, lead capture, and engagement | Extend activation workflows into a complete lifecycle with reusable tenant data, reporting, billing, AI jobs, and developer capabilities |
| Breeze Booth | Configurable professional booth operation | Offer modern browser deployment and cloud coordination while retaining future specialist hardware adapters |
| LumaBooth | Mobile-oriented booth accessibility | Unify mobile capture with administration, AI, CRM, analytics, templates, and multi-location operations |
| TouchPix | Interactive effects and sharing experiences | Add governed brand systems, automation, enterprise controls, and cross-event intelligence |
| Cvent | Enterprise event planning, registration, and operational breadth | Connect enterprise-grade event operations to an owned creative capture, guest-content, CRM, and activation lifecycle |
| Bizzabo | Event engagement, attendee experience, and data-driven event management | Combine measurable attendee engagement with booth creation, branded media delivery, consent-aware follow-up, and extensible event workflows |

EventLens AI’s position rests on six commitments:

1. **AI-first, not AI-only.** AI enhances a dependable capture and delivery foundation.
2. **Complete lifecycle.** Planning, capture, creation, sharing, engagement, insight, and billing belong to one product.
3. **CRM included.** Consent-aware guest engagement is a core capability, not an export afterthought.
4. **Automation built around events.** Real event signals can trigger reliable, auditable actions.
5. **SaaS at every layer.** Tenant ownership, roles, usage, entitlements, observability, and billing are architectural concerns.
6. **White-label and extensible.** Agencies and enterprises can eventually deliver differentiated experiences without forking the platform.

## Product Pillars

### 1. Capture Experience

The capture layer must feel instant and dependable. It includes browser webcams and mobile cameras today, an offline-first booth workflow, live preview, camera switching, mirror mode, countdown, audio and flash cues, multi-shot progress, photo strips, and session tracking. Its architecture must accommodate GIF, boomerang, short video, advanced capture modes, kiosk hardening, printing, and a future DSLR bridge.

Success means guests can understand the booth without instruction and operators can recover from permission, device, network, or session interruption.

### 2. Creative Experience

Creative tools transform captures into branded assets. The platform direction includes non-destructive editing, crop and layout tools, filters, overlays, text, stickers, frames, watermarks, template layers, safe zones, brand kits, output presets, and batch processing.

Success means a designer can build reusable experiences while an operator cannot accidentally break approved brand output.

### 3. AI Experience

AI should make sophisticated creative outcomes accessible while remaining transparent, controllable, and cost-aware. Capabilities include background work, enhancement, upscale, styles, props, object removal, smart selection, template generation, storytelling, and business insight.

AI processing is asynchronous, provider-neutral, entitlement-aware, moderated, and auditable. A disabled provider must never masquerade as a successful operation.

### 4. Sharing Experience

Guests should move from capture to delivery with minimal friction through QR codes, public or private galleries, albums, favorites, downloads, social sharing, email, SMS, or WhatsApp where configured and consented. Owners need moderation, expiration, signed access, branding, and engagement attribution.

Success means sharing is fast for the guest and measurable, respectful, and controllable for the organization.

### 5. Business Experience

Organizations need event CRUD, teams, roles, CRM, lead capture, segments, campaigns, automation, analytics, reports, subscriptions, usage limits, invoices, and operational dashboards. The platform should turn event signals into clear actions rather than producing isolated vanity metrics.

Success means customers can operate and grow their event business from one workspace.

### 6. Enterprise Experience

Enterprise readiness includes SSO, advanced permissions, departments and branches, data governance, retention, audit search, regional controls, provider governance, integrations, SLAs, custom domains, and white-label portals.

Success means a large organization can delegate safely, demonstrate compliance, and integrate EventLens AI into its established operating environment.

## Unique Differentiators

### AI Event Assistant

A permission-aware natural-language assistant will help users explore authorized event data and initiate reviewable actions.

Example questions and commands:

- “Show me the most downloaded photos.”
- “Which hour had the longest booth queue?”
- “Generate a thank-you campaign for guests who consented to email.”
- “Create a wedding highlight video.”
- “Compare QR conversion across our last five activations.”

The assistant must respect tenant scope, role permissions, consent, feature entitlements, and action confirmation. It should cite the records and date range behind an answer and create drafts before consequential actions.

### AI Memory Book

The AI Memory Book turns an event collection into an editable narrative:

- Automatic quality- and diversity-aware photo selection.
- Event timeline assembled from capture metadata.
- Theme-appropriate story and caption generation.
- Layout suggestions using approved brand assets.
- Human review, replacement, reordering, and text editing.
- Print-ready and accessible PDF export.

It must preserve originals, identify AI-generated text, and avoid inferring sensitive facts about guests.

### AI Highlight Reel

The AI Highlight Reel creates short event videos with:

- Smart scene and photo selection.
- Configurable pacing and transitions.
- Licensed or customer-supplied music.
- Editable captions and brand end cards.
- Portrait, landscape, square, and social formats.
- Review, moderation, and regeneration controls.

Rendering should be a durable queued job with clear progress, cost, output provenance, and retry behavior.

### AI Template Generator

Users describe an event, audience, dimensions, and visual intent. The generator proposes:

- Prompt-based templates.
- Brand-aware colors, typography, and logo placement.
- Automatic layouts for strips, social assets, galleries, and print.
- Safe zones and output-specific constraints.
- Editable layer structures rather than flattened images.

Generated templates remain drafts until reviewed and must use licensed assets.

### Workflow Automation

Automation connects event signals to governed actions:

```text
Photo captured
      ↓
AI enhancement queued
      ↓
Approved result published to gallery
      ↓
Consented WhatsApp delivery queued
      ↓
Guest timeline and analytics updated
```

Workflows require triggers, conditions, actions, versioning, test mode, idempotency, retries, dead-letter handling, rate controls, audit history, and provider-aware consent checks.

### Marketplace

The marketplace creates an ecosystem for:

- Photo and print templates.
- Stickers, overlays, fonts, and animations.
- Curated AI styles and workflow recipes.
- Integration and device plugins.

Marketplace content needs ownership, licensing, compatibility, versioning, moderation, reviews, entitlements, revenue sharing, and safe installation boundaries.

### AI Business Insights

AI-assisted analysis will translate operational data into recommendations:

- Engagement and conversion analysis.
- Staffing and booth-capacity recommendations.
- Popular-template and capture-mode prediction.
- Event timing and campaign suggestions.
- Storage and AI-cost forecasting.
- Revenue and retention insights when billing and attribution data are available.

Recommendations must show supporting data, confidence, and limitations. Users retain control over decisions.

## Product Experience Principles

| Principle | Product implication |
|---|---|
| Event-day confidence | Critical actions are obvious, recoverable, and usable under time pressure |
| Creative without complexity | Advanced results begin with strong defaults and remain editable |
| Privacy by design | Consent, visibility, retention, and deletion are designed into workflows |
| Insight over volume | Dashboards explain outcomes and next actions, not just counts |
| Honest capability | Disabled providers and future features are clearly labeled |
| One tenant, one boundary | Organization ownership is enforced consistently across every module |
| Extensible by contract | Cameras, storage, AI, payments, messaging, and partners connect through stable abstractions |
| Accessible to every guest | Mobile, touch, keyboard, contrast, motion, and assistive technology are first-class concerns |

## Product Success Measures

The product should be evaluated through a balanced scorecard:

- Time from registration to first published event.
- Successful booth-session and capture completion rates.
- Median capture-to-preview and capture-to-delivery time.
- Operator recovery rate after camera or network interruption.
- Guest gallery visit, download, and consented-sharing conversion.
- Lead-to-campaign and campaign-to-engagement conversion.
- Repeat organization usage and events per active organization.
- Template, AI, storage, and feature utilization by plan.
- Support incidents per live event and mean time to recovery.
- Subscription retention, expansion, and entitlement integrity.
- Accessibility, security, privacy, and tenant-isolation defect rates.

## Current Product Boundary

The repository currently contains foundations for authentication, organization membership, event management, browser booth sessions, photo processing/template tooling, AI job abstraction, CRM, analytics/reporting, and billing/usage. Maturity varies by module. External AI, payment, messaging, object storage, professional camera, printer, SSO, white-label, and marketplace capabilities require configured or future adapters.

This distinction matters: roadmap inclusion does not authorize implementation, and a UI foundation does not imply production readiness until the definition of done in `PROJECT_RULES.md` is satisfied.

## Long-Term Vision

### AI Event Operating System

EventLens AI will evolve from a unified event-photo platform into an intelligent operating system for experiential events. It will help teams plan capacity, generate creative systems, operate booths, engage guests, identify risk, automate follow-up, and learn across an authorized portfolio.

### Mobile applications

Future operator and guest applications will support managed devices, offline synchronization, push notifications, rapid event setup, remote monitoring, personal galleries, and on-site operational tools. They will consume the same versioned platform APIs and tenant authorization rules.

### Partner ecosystem

Photographers, agencies, venues, hardware vendors, print labs, creative designers, AI providers, and marketing partners will be able to package specialized capabilities. Certification and compatibility programs will protect event-day reliability.

### Developer platform

A versioned REST API, webhooks, SDKs, sandbox environments, API keys, granular scopes, documentation, and usage governance will allow customers and partners to build on EventLens AI without bypassing tenant or entitlement controls.

### Marketplace

The marketplace will distribute templates, assets, AI styles, workflow recipes, integrations, and plugins. It can create new revenue for creators and partners while helping customers deploy high-quality experiences faster.

The long-term outcome is a platform where an event team can move from intent to measurable experience in hours, operate confidently at any scale, and continuously improve through trustworthy data and AI assistance.
