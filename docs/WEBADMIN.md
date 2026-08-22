# WebAdmin Guide

`Cometen_WebAdmin.html` is the central browser interface for Cometen WebAdmin.

## Language

The interface supports Norwegian and English through the `NO | EN` switch. The selected language is stored in browser local storage. Language switching changes visible UI text only. Streamer.bot action names and backend message names remain unchanged.

English is the default language for a fresh public installation.

## Setup

The Setup area controls the Streamer.bot WebSocket connection, including host, port, automatic host selection and reconnect. The recommended default WebSocket port is `8081`.

See `NETWORK_SETUP.md` for LAN configuration.

## Cheers

The Cheers module contains the CWA controls associated with cheer and bit handling.

## Chat Overlay

The Chat Overlay module controls the supplied browser overlay:

```text
Cometen Chat Overlay.html
```

It is intended for use as an OBS Browser Source.

## Chat Pip

Chat Pip can play a short notification sound for selected chat activity. Settings include enable/disable, sound file, volume, silence interval, first-message behavior, returning-user behavior, broadcaster exclusion and ignored usernames.

Public 1.0 contains the tested persistence fix that aligns the WebAdmin fields with the Streamer.bot backend.

## Live + Custom Message

The Live module contains controls used by the CWA live-announcement workflow and one-stream custom live-message feature. Environment-specific destination settings are configured locally after installation.

## Stream Summary

The Stream Summary module works with the corresponding Streamer.bot summary actions. The statistics available depend on the runtime event triggers enabled in Streamer.bot.

## Credits

The Credits module configures the supplied credits overlay:

```text
cometen_credits_CWA.html
```

Use the WebAdmin preview to verify text and layout before using it as an OBS Browser Source.

## Alerts

The Alerts module configures:

```text
alerts/alerts.html
```

The release package includes its required relative media files. Keep the `alerts` folder structure intact.

IRL forwarding is optional and is documented in `IRL_INTEGRATION.md`.

## URL Guard

Public 1.0 stores the authoritative URL Guard configuration in:

```text
CometenUrlGuard_SettingsJson
```

Optional YouTube and Spotify exclusions affect local URL logging only. They do not whitelist those URLs from deletion rules.

See `URL_GUARD.md` for details.

## Save and Refresh

For configuration modules, change the desired settings, press Save, then use Refresh when you want to request the persisted state from Streamer.bot.

If settings do not remain after Save and Refresh, verify that the matching CWA actions from `Cometen_WebAdmin_1.0.sb` were imported with overwrite enabled during an update.
