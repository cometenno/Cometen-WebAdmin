# Cometen WebAdmin

Cometen WebAdmin is a browser-based administration layer for Streamer.bot and OBS.

Public release: **1.0**

## Features

- Norwegian / English WebAdmin UI
- Streamer.bot WebSocket integration
- LAN-friendly setup
- Cheers controls
- Chat Overlay
- Chat Pip
- Live + Custom Message controls
- Stream Summary
- Credits overlay
- Alerts administration
- URL Guard
- Local deleted-URL logging
- Optional YouTube and Spotify log exclusions
- Optional IRL alert forwarding
- Full Cometen visual theme

## Download

Use the GitHub v1.0 release asset:

```text
CometenWebAdmin_1.0_Public.zip
```

SHA-256:

```text
7dbfa5f9b127e8d3ed362126f7af3e50e3ab7669885980c83a9d347f06469335
```

The release ZIP contains one top-level folder:

```text
CometenWebAdmin
```

## Quick install

Open your Streamer.bot installation and extract the ZIP inside:

```text
<STREAMERBOT_DIR>\plugins\
```

If `plugins` does not exist, create it first.

The final path must be:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\
```

Then import:

```text
Cometen_WebAdmin_1.0.sb
```

See [INSTALL.md](INSTALL.md) for the complete guide.

## Documentation

- [Installation](INSTALL.md)
- [WebAdmin module guide](docs/WEBADMIN.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Network / LAN](docs/NETWORK_SETUP.md)
- [Streamer.bot](docs/STREAMERBOT_SETUP.md)
- [OBS](docs/OBS_SETUP.md)
- [URL Guard](docs/URL_GUARD.md)
- [IRL integration](docs/IRL_INTEGRATION.md)
- [Troubleshooting](docs/TROUBLESHOOTING.md)
- [Security](SECURITY.md)
- [Release checklist](docs/RELEASE_CHECKLIST.md)
- [Checksums](CHECKSUMS.txt)

## Tested fixes included in 1.0

Public 1.0 contains the Chat Pip persistence fix tested on both machines and the URL Guard JSON-state persistence fix using:

```text
CometenUrlGuard_SettingsJson
```

## Security

The public build contains no production Discord webhook URLs, private LAN addresses, Windows usernames, production absolute paths, private platform IDs or private IRL endpoints.

Private values must be configured locally after installation.
