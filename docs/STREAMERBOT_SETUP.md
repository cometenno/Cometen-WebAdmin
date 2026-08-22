# Streamer.bot Setup

## Import

Import the file included in the release package:

```text
Cometen_WebAdmin_1.0.sb
```

The public import contains the CWA action set, including the tested Chat Pip persistence fix and URL Guard JSON-state persistence.

## WebSocket server

Enable Streamer.bot WebSocket Server. Recommended default port:

```text
8081
```

## HTTP server

Streamer.bot HTTP Server can be used to serve WebAdmin to other devices on the LAN. A common default is port `7474`.

## Private values

The public import is sanitized. Configure locally:

- Discord webhook URLs
- channel/login values
- local media paths
- role/channel IDs
- optional IRL endpoints

## Updating

When updating an existing installation, review the Streamer.bot import list and keep overwrite enabled for the CWA actions being updated.

Back up the Streamer.bot configuration before major updates.

## Multiple Streamer.bot instances

If the same CWA actions exist on multiple PCs, avoid leaving identical production event triggers enabled on both unless duplicate execution is intended.
