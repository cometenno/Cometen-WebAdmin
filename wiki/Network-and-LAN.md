# Network and LAN Setup

Cometen WebAdmin can run entirely on one PC or use a Streamer.bot PC as the central backend while another PC opens WebAdmin over the LAN.

## Recommended ports

```text
Streamer.bot WebSocket: 8081
Streamer.bot HTTP Server: 7474
```

These are defaults only and can be changed.

## Local use

For WebAdmin running on the same PC as Streamer.bot:

```text
ws://127.0.0.1:8081/
```

## LAN use

For a second PC, use the LAN address of the Streamer.bot PC.

Examples:

```text
http://<STREAMERBOT_PC_IP>:7474/webadmin/Cometen_WebAdmin.html
ws://<STREAMERBOT_PC_IP>:8081/
```

## Host resolution order

The public browser pages resolve WebSocket host in this order:

1. `?host=` URL parameter
2. saved `cwa_ws_host`
3. current HTTP page hostname
4. `127.0.0.1`

Port resolution order:

1. `?port=` URL parameter
2. saved `cwa_ws_port`
3. `8081`

Example:

```text
Cometen_WebAdmin.html?host=<STREAMERBOT_PC_IP>&port=8081
```

## Firewall

For LAN use, allow inbound TCP access to the configured WebSocket and HTTP ports on the Streamer.bot PC.

Restrict the firewall rule to the private/local network where practical.

## HTTP vs HTTPS

A page served over HTTPS may be blocked from opening an insecure `ws://` connection because of browser mixed-content policy.

For a trusted private LAN, HTTP is the simplest configuration unless secure WebSockets are configured.