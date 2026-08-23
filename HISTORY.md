# Project History

Cometen WebAdmin was developed and tested privately before the public 1.0 release.

The original development repository remains private as a legacy archive because it contains internal development history and environment-specific values that are not suitable for publication. Public development continues in this repository from version 1.0 onward.

## Pre-1.0 development

Before the public release, the project went through an internal 19.9.x development series. That work included repeated testing across the production and secondary Streamer.bot machines, WebAdmin UI refinement, alert integration work, state persistence fixes and release sanitization.

Notable pre-release milestones included:

- WebAdmin administration UI and bilingual NO/EN interface
- Streamer.bot WebSocket and HTTP integration
- Chat Overlay and Chat Pip modules
- Credits and alert administration
- URL Guard with local deleted-URL logging
- Optional IRL alert forwarding
- LAN-friendly multi-machine operation
- Chat Pip settings persistence fixes verified on both test machines
- URL Guard settings migration to the authoritative `CometenUrlGuard_SettingsJson` JSON state
- URL Guard compatibility handling for earlier saved globals
- dynamic WebSocket host and port handling for browser overlays
- public-release sanitization of private IP addresses, usernames, paths, IDs, endpoints and credentials

## Public 1.0

The clean public repository was established in August 2026 with a new Git history so that legacy private development data would not be exposed.

Public 1.0 introduced the supported public package layout:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\
```

and the public Streamer.bot import:

```text
Cometen_WebAdmin_1.0.sb
```

The public release also established separate maintained projects for the WebAdmin and IRL runtime layers:

- `Cometen-WebAdmin` - public WebAdmin releases, documentation and Wiki source
- `CometenIRLSystem` - public IRL/BELABOX integration, receiver, relay, watchdog and control system

## Development policy from 1.0 onward

All new public WebAdmin development continues in `Cometen-WebAdmin`.

The legacy private repository is retained only as an archive/reference and is not the active development target.

Future releases should keep public branches free of production secrets and environment-specific private values.