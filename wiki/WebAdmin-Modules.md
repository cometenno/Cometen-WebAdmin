# WebAdmin Modules

Cometen WebAdmin groups the system into browser-based administration modules backed by Streamer.bot actions and persistent settings.

## Cheers

Controls cheer/bits-related behavior and related settings exposed by the CWA backend.

## Chat Overlay

Controls the browser chat overlay used in OBS. The public release keeps the overlay LAN-aware so it can connect to the Streamer.bot WebSocket server from another PC.

## Chat Pip

Controls the notification sound used for selected chat activity. Public 1.0 includes the tested persistence fix for Chat Pip behavior checkboxes and timing values.

See [Chat Pip](Chat-Pip).

## Live + Custom Message

Controls live announcement settings and one-stream custom message behavior. Environment-specific webhook and platform values must be configured locally.

## Stream Summary

Provides controls for stream summary state and summary-related runtime data handled by Streamer.bot.

## Credits

Controls the credits browser overlay and its visible text/settings.

## Alerts

Controls alert behavior for supported events and optional IRL forwarding switches.

See [Alerts](Alerts).

## URL Guard

Controls chat URL handling, allow rules, blocked-message behavior and deleted-URL logging.

Public 1.0 uses one authoritative persistent JSON state:

```text
CometenUrlGuard_SettingsJson
```

See [URL Guard](URL-Guard).

## Setup / connection settings

WebAdmin supports local and LAN WebSocket targets. Host/port values can be stored in browser localStorage or supplied through URL parameters.

See [Network and LAN](Network-and-LAN).