# Configuration Reference

This page summarizes the main environment-specific settings used by Cometen WebAdmin.

## WebSocket

Default/recommended port:

```text
8081
```

Host resolution in public browser pages:

1. `?host=` URL parameter
2. saved `cwa_ws_host`
3. current HTTP hostname
4. `127.0.0.1`

Port resolution:

1. `?port=` URL parameter
2. saved `cwa_ws_port`
3. `8081`

## HTTP Server

Recommended example port:

```text
7474
```

Example LAN URL:

```text
http://<STREAMERBOT_PC_IP>:7474/webadmin/Cometen_WebAdmin.html
```

## Browser localStorage

Common browser-local values include:

```text
cwa_ws_host
cwa_ws_port
```

The WebAdmin language preference is also stored browser-side.

## Chat Pip

The current public release persists Chat Pip settings through Streamer.bot backend actions. Important settings include:

- enabled
- sound file
- volume
- first-message behavior
- after-silence behavior
- silence minutes
- returning-user behavior
- returning-user minutes
- broadcaster ignore
- ignored users

## URL Guard

Authoritative persistent configuration:

```text
CometenUrlGuard_SettingsJson
```

The JSON state covers:

- enabled
- allow broadcaster
- allow moderators
- allow VIPs
- delete ordinary URLs
- delete URL commands
- announce blocked
- log deleted URLs
- skip YouTube logging
- skip Spotify logging
- block message
- extra allowed users

Legacy `CometenUrlGuard_*` globals are mirrored for compatibility.

Deleted URL log:

```text
<STREAMERBOT_DIR>\logs\CWA_URL_Guard_Deleted_URLs.log
```

## Alerts

The main alert overlay is:

```text
alerts\alerts.html
```

Optional IRL forwarding bridge:

```text
alerts\irl-forward.js
```

## Credits

Credits overlay:

```text
cometen_credits_CWA.html
```

## Chat Overlay

Chat overlay:

```text
Cometen Chat Overlay.html
```

## Private values

Never commit production values such as:

- Discord webhook URLs
- OAuth/API tokens
- private LAN addresses
- Windows usernames
- absolute production paths
- private role/channel IDs
- private IRL endpoints/tokens

Use the public placeholders documented in `SECURITY.md`.