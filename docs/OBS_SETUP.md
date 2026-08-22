# OBS Setup

## Alerts

Add an OBS Browser Source pointing to:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\alerts\alerts.html
```

Recommended design resolution:

```text
1920 x 1080
```

Keep the complete `alerts` directory intact because images and sounds use relative paths.

## Credits

Add:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\cometen_credits_CWA.html
```

## Chat Overlay

Add:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\Cometen Chat Overlay.html
```

## LAN-hosted Browser Sources

If OBS loads the pages over HTTP, the public pages can use the HTTP hostname for Streamer.bot WebSocket access unless an explicit host override is configured.

## Updating

After replacing HTML/JS files, refresh the Browser Source cache or restart OBS if old browser code remains cached.
