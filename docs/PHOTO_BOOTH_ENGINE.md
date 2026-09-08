# EventLens AI Photo Booth Engine

## Platform foundation (Module 4, Phase 0)

The booth platform foundation is the control plane for future capture
experiences. It does not start a camera stream, capture media, process photos,
or implement GIF, boomerang, video, DSLR, or AI workflows.

Protected workspace routes:

- `/booth` — readiness overview and browser support summary
- `/booth/setup` — permissions, safe device discovery, and offline preparation
- `/booth/settings` — tenant-scoped operational defaults
- `/booth/diagnostics` — capability, device, permission, storage, and network checks

The browser owns capability and permission detection. Device discovery uses
`enumerateDevices()` without opening media. Only safe metadata is shown; raw
device identifiers are not persisted by the browser foundation. Permission
requests require an explicit user action and blocked permissions include
recovery guidance.

The backend follows Clean Architecture and exposes tenant-authorized endpoints
under `/api/booth` for configuration, capability and device snapshots,
permissions, diagnostics, runtime state, and offline readiness. The
`AddBoothPlatformFoundation` migration creates:

- `booth_configurations`
- `booth_capabilities`
- `booth_devices`
- `booth_health_checks`

The runtime state machine is:

`Initializing → CheckingPermissions → DetectingHardware → Ready ↔ Busy`

`Offline`, `Maintenance`, and `Error` are explicit recovery states. Offline
Phase 0 initializes IndexedDB and local-storage availability only; background
synchronization remains a later phase.

Support is detected rather than assumed. Chromium browsers provide the widest
set of APIs. Safari and Firefox may return `Unknown` for permission state or
omit File System Access, Wake Lock, USB, or Bluetooth APIs. The UI treats those
results as unavailable capabilities, not application failures.

## Camera foundation (Module 4, Phase 1)

The authenticated `/booth/camera` workspace provides live, client-side camera
preview management. Its `ICameraManager` boundary owns initialization, preview
start/stop/pause/resume, switching, release, restart, lifecycle recovery, and
normalized errors. `BrowserCameraManager` is the first adapter; a future DSLR
or mirrorless bridge can implement the same lifecycle without changing the
workspace.

The camera state model is:

`Initializing → Ready → Streaming ↔ Paused → Stopped`

`Disconnected` and `Error` provide explicit recovery paths. The manager watches
`devicechange`, track-ended events, and document visibility. It pauses when a
tab becomes inactive, resumes when possible, detects unplugged devices, and
offers restart or camera reselection. Every stream track is stopped on release
or page unmount.

Supported resolution profiles are 640×480, 1280×720, 1920×1080, 2560×1440,
and 3840×2160. The browser receives ideal constraints and the manager falls
back through lower profiles when a camera rejects the requested resolution.
Actual width, height, and frame rate are read from the active track.

Camera capabilities come from `MediaStreamTrack.getCapabilities()` where the
browser supports it. Width, height, frame rate, zoom, focus, torch, exposure,
white balance, brightness, and contrast are shown honestly as supported or not
reported. Chrome and Edge generally expose the richest constraint metadata;
Safari and Firefox expose a smaller, device-dependent set. Mobile browsers
also require HTTPS outside localhost and always retain control of permission
prompts.

The backend stores organization-scoped `CameraPreference` metadata only:
preferred camera, resolution, mirror setting, and aspect ratio. Camera frames
never leave the browser, and Phase 1 contains no capture, recording, Canvas
processing, GIF, boomerang, strip, AI, DSLR communication, or upload behavior.

## Booth session engine (Module 4, Phase 2)

The authenticated `/booth/session` workspace manages guest, operator, and
administrator test sessions. It provides large touch controls, event selection,
an attract-mode idle screen, pause/resume/complete actions, runtime health, and
operator reset without opening a camera or handling media.

Managed session lifecycle:

`Created → Initializing → Active ↔ Paused → Completed`

Active, paused, or initializing sessions can also become `Expired`,
`Cancelled`, or `Error`. Invalid transitions are rejected by domain invariants.
Every significant transition writes a `BoothSessionActivity` record with a
timestamp and bounded metadata.

Runtime lifecycle:

`Idle → Preparing → Ready → Active ↔ Paused → Completing → Ready`

`Maintenance`, `Offline`, and `Error` are explicit operational states. An
operator reset clears the active workflow and returns the runtime to `Ready`.
The application never permits `Idle → Active`.

Each managed session receives a 256-bit opaque restore token. The browser keeps
that token locally and offers recovery after refresh, sleep, or interruption
only while the server session remains active and within its timeout. Recovery
validates the event organization before returning session data.

The kiosk enters attract mode after 60 seconds of local inactivity, clears its
temporary workflow, cancels the active session, and resets the runtime. A
server-side expiry worker scans every 30 seconds and expires sessions inactive
for five minutes, providing cleanup even after a tab closes or crashes.
Session creation is rate-limited and organization/event ownership is validated
server-side.

Phase 2 intentionally contains no photo capture, countdown, processing,
gallery, AI, or printing implementation.

## Scope

This module adds the browser photo booth only. It does not implement automatic
uploads, AI processing, templates, or the photo editor.

## Frontend structure

```text
app/
  booth/
    api.ts
    types.ts
    camera/
      CameraAdapter.ts
      BrowserCameraAdapter.ts
      useCamera.ts
    processing/
      stripGenerator.ts
    store/
      useBoothStore.ts
  pages/
    BoothLanding.tsx
    BoothSession.tsx
    BoothPreview.tsx
    BoothResult.tsx
```

The route flow is `/booth/:eventSlug` → `/session` → `/preview` → `/result`.
React Query loads event booth configuration. Zustand owns device-local session,
camera, countdown, capture, and result state.

## State flow

1. The event route loads public booth configuration.
2. Starting creates a backend session when authenticated and online. If the API
   is unavailable, a local unsynced session is created.
3. The session route requests camera permission and starts a live preview.
4. Countdown and burst capture place full-resolution blobs and thumbnails in
   browser memory.
5. Review supports retake and strip layout settings.
6. Accept generates a high-resolution strip with Canvas or OffscreenCanvas.
7. Result supports download. Nothing uploads automatically.

Blob URLs are revoked when a session is cleared.

## Camera lifecycle

`BrowserCameraAdapter` owns `MediaStream` tracks. It negotiates HD or Full HD,
supports device IDs and user/environment facing modes, disconnects old streams
before switching, and stops tracks on unmount. Track-ended events trigger a
bounded reconnect attempt. Permission, missing-device, disconnected, and
unexpected errors are represented separately in booth state.

The preview can be mirrored without changing the source stream. Capture applies
the mirror transform to the Canvas output so preview and result match.

## Backend

The Clean Architecture backend adds `BoothSession`, application DTOs and
validators, a session service, REST endpoints under `/api/booth`, EF
configuration, and the `PhotoBoothEngine` migration. Capture endpoints record
capture counts only; image bytes remain local until a future explicit upload
workflow is invoked.

## Future DSLR integration

`CameraAdapter` is the hardware boundary. A future `DslrCameraAdapter` can
implement the same connect, disconnect, device enumeration, and capture
contract while communicating with a signed desktop bridge over localhost
WebSocket or WebUSB where supported. UI and session orchestration remain
unchanged. The bridge should expose capability negotiation, health, capture
acknowledgements, and file-transfer progress, and must never be silently
selected over the browser camera.
# Phase 3 — Photo Capture Engine

Phase 3 adds a browser capture workflow at `/booth/capture`: configurable countdowns, sequential multi-photo and burst capture, cancellation, shutter feedback, local previews, retake, and navigation recovery.

JPEG blobs remain on the capture device in IndexedDB (`eventlens-captures`). SQL stores tenant-scoped metadata and an opaque local key only. Cloud upload, filters, editing, printing, AI processing, strips, and the customer gallery product remain outside Phase 3.

Browsers may revoke permission, suspend background tabs, disconnect devices, reject Canvas encoding under memory pressure, or clear IndexedDB. The UI reports these cases and preserves already committed local captures where the browser allows it.
# Phase 4 — Camera Controls & Professional Features

`/booth/camera-controls` layers professional operator controls over the existing browser camera without changing session or capture workflows. `BrowserCameraControlAdapter` discovers the active `MediaStreamTrack` capabilities and applies supported constraints. Unsupported controls remain visible but disabled.

Camera profiles are organization-scoped SQL records. Applying, changing, or resetting settings creates an audit record containing the operator, optional profile, previous settings, new settings, and UTC timestamp. No image, video, serial number, or sensitive device identifier is stored.

Future DSLR and mirrorless support implements the same adapter and application service contracts. Vendor SDK objects remain inside infrastructure adapters; booth sessions and photo capture continue to depend only on stable abstractions.

Browser support varies. Zoom, manual focus, exposure compensation, brightness, contrast, white balance, and torch are enabled only when `getCapabilities()` advertises them. Torch commonly requires a rear mobile camera and active track. Browser white-balance presets are best-effort because Media Capture usually exposes modes rather than photographic color temperatures.
# Phase 5 — Advanced Capture Modes

`/booth/capture-modes` adds GIF, boomerang, video, improved burst, time-lapse, and live-photo-style capture without entering photo editing, AI, gallery, template, or printing concerns.

Browser media bytes remain device-local in IndexedDB (`eventlens-media`). SQL stores tenant-scoped `CapturedMedia`, `MediaCaptureSettings`, and `MediaProcessingJob` lifecycle data plus an opaque local key. API DTOs never expose the stored key or a server filesystem path.

GIF and boomerang use Canvas frame sampling and local GIF encoding. Video and live-photo motion use `MediaRecorder`; MP4 is selected only when the browser reports support, otherwise WebM is used. Burst samples a rapid sequence, while time lapse samples periodic frames and renders a local video through `canvas.captureStream()`.

Browser limitations include memory pressure from high-resolution frame sequences, background-tab timer throttling, IndexedDB quota/eviction, MediaRecorder codec differences, missing `canvas.captureStream()` on older browsers, and camera permission/device loss. Long time lapses should keep the tab and device awake. Live photo is stored as a local motion asset with a key still, not an Apple/Google proprietary container.

The queue contract separates capture from future durable processing adapters. Later AI, gallery publication, printing, cloud storage, DSLR capture, and plugins consume stable media IDs rather than changing capture workflows.
# Phase 6 — Professional Camera Adapter Architecture

`/booth/professional-cameras` manages browser cameras and future DSLR/mirrorless adapter slots without changing booth session, capture, or advanced-media workflows.

The application depends on `ICameraProvider`, `ICameraAdapter`, and `ICameraAdapterFactory`. `BrowserCameraAdapter` delegates capture to the existing browser workflow. `DSLRAdapterBase` and `MirrorlessAdapterBase` define vendor-neutral connection, capability, capture, settings, and health contracts. Canon, Nikon, Sony, generic DSLR, and generic mirrorless adapters are deliberately unavailable until a separately deployed signed bridge implements the interface.

Professional camera records, adapter registrations, connection logs, capability profiles, and camera events are organization-scoped. Serial numbers are accepted only through protected registration, are omitted from DTOs, and must be encrypted or tokenized by a production secrets/storage policy before real hardware rollout. SDK credentials and hardware secrets must never enter these records.

Future SDK integration requires:

1. A workstation-side bridge isolated from the web/API process.
2. A vendor adapter implementing `ICameraAdapter`.
3. Signed installation and least-privilege device access.
4. Capability translation into EventLens DTOs.
5. Idempotent connect/disconnect/capture operations and heartbeat reporting.
6. Vendor SDK licensing, supported OS/architecture, driver, firmware, deployment, and rollback documentation.
7. Contract, reconnect, device-loss, and capture-integrity testing.

The web application cannot directly use most vendor USB SDKs. The bridge will require a supported operator workstation, vendor drivers, local service lifecycle management, secure API authentication, network/firewall configuration, and observability.

# Phase 7 — Photo Strip & Template Rendering Engine

`/templates` provides an organization-scoped template library and live Canvas preview. It reads only the current booth session's locally committed captures from IndexedDB, crops photographs into ordered slots, and composes shape, text, date, logo/sticker-ready, and QR layers. The final browser preview can be downloaded as JPEG. The capture engine remains the owner of capture lifecycle and local photo bytes.

SQL stores `PhotoTemplates`, `TemplateLayouts`, `TemplateElements`, `Stickers`, and `TemplateAssignments`. Layout coordinates are explicit print-canvas pixels with resolution and aspect metadata. Tenant checks apply before list, mutation, preview, or render; global/public assets are read-only to tenant users. Sticker DTOs and template DTOs do not reveal server filesystem paths.

The backend ImageSharp renderer independently creates bounded JPEG compositions and performs center-crop photo placement, layer ordering, shapes, and QRCoder-generated QR images. Render requests accept at most 12 bounded JPEG/PNG data URLs because browser IndexedDB is not server-addressable. Invalid image data is ignored safely and cancellation is observed. Browser Canvas supplies the immediate text/date preview; durable font and private asset resolution can be expanded behind `IPhotoComposerService` without changing API contracts.

Professional rendering endpoints are under `/api/templates/photo` for CRUD, duplicate, and activation, with `/api/templates/{id}/preview` and `/api/templates/{id}/render` for composition. The original `/api/templates` contract remains intact for backward compatibility.
