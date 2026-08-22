# Release Notes

## Cometen WebAdmin 1.0

Initial public release.

### Included

- Norwegian / English WebAdmin UI
- Streamer.bot WebSocket integration
- LAN-friendly host resolution
- Cheers controls
- Chat Overlay
- Chat Pip
- Live + Custom Message controls
- Stream Summary
- Credits overlay
- Alerts administration
- URL Guard
- deleted-URL logging
- optional YouTube and Spotify log exclusions
- optional IRL alert forwarding
- MIT License

### Tested fixes

Public 1.0 includes the Chat Pip persistence fix tested on both production machines.

URL Guard uses one authoritative persistent JSON state:

```text
CometenUrlGuard_SettingsJson
```

Legacy URL Guard globals are mirrored for compatibility.

### Download

Use the GitHub release asset:

```text
CometenWebAdmin_1.0_Public.zip
```

See [Installation](Installation) for the required `plugins\CometenWebAdmin` layout.