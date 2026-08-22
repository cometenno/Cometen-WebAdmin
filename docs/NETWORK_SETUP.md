# Network and LAN Setup

## Recommended defaults

```text
Streamer.bot WebSocket: 8081
Streamer.bot HTTP Server: 7474
```

These ports are examples and can be changed.

## Local use

When WebAdmin and Streamer.bot run on the same PC, `127.0.0.1` is sufficient.

## LAN use

For a second PC to access WebAdmin, the Streamer.bot PC must accept LAN connections. Bind Streamer.bot to the appropriate LAN interface or `0.0.0.0` where supported, and allow the configured TCP ports in Windows Firewall.

Example placeholders:

```text
http://<STREAMERBOT_PC_IP>:7474/webadmin/Cometen_WebAdmin.html
ws://<STREAMERBOT_PC_IP>:8081/
```

## Automatic WebSocket host selection

The public browser pages resolve WebSocket host in this order:

1. `?host=` URL parameter
2. saved `cwa_ws_host` browser setting
3. current HTTP page hostname
4. `127.0.0.1` fallback

WebSocket port resolves from `?port=`, saved `cwa_ws_port`, then `8081`.

## HTTPS warning

A page served over HTTPS can be blocked from connecting to insecure `ws://` because of browser mixed-content rules. On a trusted private LAN, HTTP is the simplest setup unless secure WebSockets are configured.
