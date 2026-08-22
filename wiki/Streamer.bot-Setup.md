# Streamer.bot Setup

## Import

Import:

```text
Cometen_WebAdmin_1.0.sb
```

The public import is sanitized and is intended to be configured locally after import.

## WebSocket server

Enable Streamer.bot WebSocket Server.

Recommended port:

```text
8081
```

For local-only use, binding to `127.0.0.1` is sufficient.

For LAN use, bind to an appropriate LAN interface or `0.0.0.0` and configure the firewall accordingly.

## HTTP server

Using Streamer.bot HTTP Server is recommended for LAN administration.

A common default port is:

```text
7474
```

## Private/local configuration

Configure your own:

- channel/login values
- Discord webhook URLs
- role/channel IDs
- local media paths
- optional IRL endpoints
- other environment-specific identifiers

## Multiple Streamer.bot PCs

If the same CWA import exists on multiple PCs, avoid leaving identical production event triggers enabled on both unless duplicate execution is intended.

It is valid to keep action definitions on more than one PC while only one instance owns the relevant production triggers.