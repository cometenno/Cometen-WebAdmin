# OBS Setup

Cometen WebAdmin includes browser pages intended for OBS Browser Sources.

## Alerts

Use:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\alerts\alerts.html
```

Recommended design resolution:

```text
1920 x 1080
```

Keep the complete `alerts` directory intact so relative image and sound paths continue to work.

## Credits

Use:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\cometen_credits_CWA.html
```

## Chat Overlay

Use:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\Cometen Chat Overlay.html
```

## Local files vs HTTP

Browser Sources can load the local HTML files directly.

For LAN-hosted or centralized setups, the pages can instead be served through Streamer.bot HTTP Server.

When loaded over HTTP, the public browser pages can use the page hostname as the WebSocket host unless overridden.

## Updating files

After replacing HTML or JavaScript files:

1. refresh the Browser Source cache, or
2. restart OBS if old browser code remains cached.