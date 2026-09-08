# Guest Booth Experience Engine

## Guest journey

The kiosk route is `/booth/experience?eventId={eventId}`. An active managed booth session may also supply the event automatically.

```text
Attract → Welcome → Mode selection → Preparation → Countdown → Capture
        → Preview → Template selection → Rendering → Completed → Attract
```

Preview may return to Preparation for a retake or Attract for cancellation. Any active state may enter Error. Error may retry Preparation or restart Attract. All transitions are enforced by `GuestBoothSession`, persisted, and recorded in `BoothExperienceLogs`.

## Kiosk behavior

- Wedding, Corporate, Birthday, and Festival themes use the event experience configuration.
- Controls meet the 44 CSS pixel touch-target baseline.
- Fullscreen uses the browser Fullscreen API and exposes a deliberate exit control.
- Camera streams, object URLs, timers, and listeners are released during recovery or teardown.
- Reduced-motion preferences disable ornamental motion.
- Photo captures are committed to the existing local IndexedDB capture store.
- GIF, boomerang, and video are selectable through the established capture-mode vocabulary. Their durable media processing remains owned by the Phase 5 capture engine.

## Recovery strategy

The browser stores only the opaque guest session ID, recovery token, and event ID in `sessionStorage`. On refresh it calls the recovery endpoint. The server compares the 256-bit token in fixed time and returns only that session's public journey state. Invalid tokens reveal no session data.

Camera permission, device loss, busy camera, refresh, and unexpected workflow errors produce guest-safe recovery messages. Idle sessions return to Attract using the configured timeout. Unexpected navigation shows the browser's leave confirmation while a journey is active.

## Deployment notes

Use a current Chromium, Edge, Safari, or Firefox release with HTTPS camera permission. Configure the event before kiosk launch, validate the selected camera, disable operating-system sleep, and test fullscreen exit procedures. Browser autoplay, fullscreen, camera permission, and storage quota policies vary by device. This phase does not provide offline synchronization, galleries, QR download, social sharing, payments, printing, or AI.
