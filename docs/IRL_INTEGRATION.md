# IRL Integration

IRL forwarding is optional.

The standard alert overlay can forward supported events through:

```text
alerts/irl-forward.js
```

to a Streamer.bot action named:

```text
Cometen IRL Notifications - Send
```

Core flow:

```text
WebAdmin -> Streamer.bot -> alert overlay
```

Optional IRL flow:

```text
alert overlay
-> irl-forward.js
-> IRL notification action
-> locally configured relay/receiver
```

The public Cometen WebAdmin package does not contain private relay credentials, receiver tokens or private receiver endpoints.

When multiple Streamer.bot instances are online, review production triggers carefully to avoid duplicate alerts or announcements.
