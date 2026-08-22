# Getting Started

This page covers the shortest path from download to a working Cometen WebAdmin installation.

## 1. Download

Download the latest release asset:

```text
CometenWebAdmin_1.0_Public.zip
```

## 2. Extract to Streamer.bot

Open the Streamer.bot installation directory.

If this folder does not exist, create it:

```text
<STREAMERBOT_DIR>\plugins
```

Extract the ZIP inside `plugins`.

Correct result:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\
```

## 3. Import Streamer.bot actions

Import:

```text
Cometen_WebAdmin_1.0.sb
```

When updating an existing installation, review the import list and keep overwrite enabled for the CWA actions being updated.

## 4. Enable WebSocket

Enable Streamer.bot WebSocket Server.

Recommended default port:

```text
8081
```

## 5. Open WebAdmin

Main page:

```text
Cometen_WebAdmin.html
```

For local use, the page can be opened directly.

For LAN administration, serving the page through Streamer.bot HTTP Server is recommended.

## 6. Configure local values

The public release is sanitized. Configure your own environment-specific values after import, including channel names, webhook URLs, platform IDs, local media paths, and optional IRL endpoints.

## 7. Add OBS Browser Sources

See [OBS Setup](OBS-Setup) for the alert, credits and chat overlay paths.

## Next steps

- [Installation](Installation)
- [Network and LAN](Network-and-LAN)
- [WebAdmin Modules](WebAdmin-Modules)
- [Troubleshooting](Troubleshooting)