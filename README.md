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

## Quick install

The release ZIP contains one top-level folder:

```text
CometenWebAdmin
```

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
- [Architecture](docs/ARCHITECTURE.md)
- [Network / LAN](docs/NETWORK_SETUP.md)
- [Streamer.bot](docs/STREAMERBOT_SETUP.md)
- [OBS](docs/OBS_SETUP.md)
- [URL Guard](docs/URL_GUARD.md)
- [IRL integration](docs/IRL_INTEGRATION.md)
- [Troubleshooting](docs/TROUBLESHOOTING.md)
- [Security](SECURITY.md)
- [Release checklist](docs/RELEASE_CHECKLIST.md)

## Public-release security

The public build contains no production Discord webhook URLs, private LAN addresses, Windows usernames, production absolute paths, private platform IDs or private IRL endpoints.

Private values must be configured locally after installation.
