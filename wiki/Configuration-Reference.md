# Configuration Reference

## WebSocket

Recommended/default port:

```text
8081
```

Host resolution:

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

## URL Guard

Authoritative persistent state:

```text
CometenUrlGuard_SettingsJson
```

Deleted URL log:

```text
<STREAMERBOT_DIR>\logs\CWA_URL_Guard_Deleted_URLs.log
```

## Browser files

Main WebAdmin:

```text
Cometen_WebAdmin.html
```

Chat overlay:

```text
Cometen Chat Overlay.html
```

Credits overlay:

```text
cometen_credits_CWA.html
```

Alert overlay:

```text
alerts\alerts.html
```

Optional IRL bridge:

```text
alerts\irl-forward.js
```

## Private values

Never publish production webhook URLs, tokens, private IPs, Windows usernames, absolute paths, role/channel IDs or private IRL endpoints.