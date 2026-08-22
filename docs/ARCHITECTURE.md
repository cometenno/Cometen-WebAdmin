# Architecture

Cometen WebAdmin uses Streamer.bot as the automation backend and browser pages as the administration and overlay frontend.

```text
WebAdmin / browser overlays
        |
        | WebSocket
        v
    Streamer.bot
        |
        +--> platform events/actions
        +--> local persisted globals
        +--> OBS-related actions
        +--> locally configured Discord webhooks
        +--> optional IRL forwarding
```

## Main components

`Cometen_WebAdmin.html` is the primary administration interface.

`Cometen_WebAdmin_1.0.sb` is the sanitized Streamer.bot import.

`Cometen Chat Overlay.html` is the chat Browser Source.

`cometen_credits_CWA.html` is the credits Browser Source.

`alerts/alerts.html` is the standard alert Browser Source.

`alerts/irl-forward.js` is the optional IRL forwarding bridge.

## State

Most settings are stored as Streamer.bot persistent globals.

URL Guard 1.0 uses one authoritative persistent JSON state:

```text
CometenUrlGuard_SettingsJson
```

Legacy `CometenUrlGuard_*` globals are mirrored for compatibility.

Browser `localStorage` stores browser-local preferences such as UI language and WebSocket host/port overrides.

## Multiple PCs

The same installation can be used locally or across a LAN. If multiple Streamer.bot instances contain the same production triggers, avoid enabling identical event triggers on both machines unless duplicate execution is intended.
