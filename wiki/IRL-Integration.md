# IRL Integration

IRL forwarding is optional and is intentionally separated from the core WebAdmin installation.

## Core flow

```text
WebAdmin -> Streamer.bot -> normal alert overlay
```

## Optional IRL flow

```text
alert overlay
-> alerts/irl-forward.js
-> Cometen IRL Notifications - Send
-> locally configured relay / receiver
```

## Forwarding bridge

The public release includes:

```text
alerts/irl-forward.js
```

The bridge can forward supported alert events to a Streamer.bot action named:

```text
Cometen IRL Notifications - Send
```

## Private IRL configuration

The public repository does not include:

- private receiver URLs
- relay credentials
- receiver tokens
- private machine addresses

Configure those locally in the IRL environment.

## Multiple PCs

An IRL workflow may intentionally keep an IRL-capable Streamer.bot PC running while another streaming PC is offline.

When more than one Streamer.bot instance is online, review production triggers carefully to prevent duplicate alerts, announcements or runtime actions.