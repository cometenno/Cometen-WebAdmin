# Troubleshooting

## WebSocket shows disconnected

Check:

1. Streamer.bot is running.
2. WebSocket Server is enabled.
3. the configured port matches WebAdmin.
4. Windows Firewall allows the connection.
5. for LAN use, the server is not bound only to `127.0.0.1`.

## Works locally but not from another PC

Use the LAN address of the Streamer.bot PC or serve WebAdmin through Streamer.bot HTTP Server.

Confirm TCP connectivity to the configured WebSocket and HTTP ports.

## HTTPS page cannot connect to WebSocket

A browser may block `ws://` from an HTTPS page as mixed content.

Use HTTP on a trusted private LAN or configure secure WebSockets.

## OBS still shows old code

Refresh the Browser Source cache or restart OBS.

## Alerts have no media

Confirm the complete `alerts` directory was extracted and relative paths were preserved.

## Chat Pip settings do not persist

Public 1.0 includes the tested persistence fix.

Re-import `Cometen_WebAdmin_1.0.sb` with overwrite enabled for the CWA actions being updated, reload WebAdmin with `Ctrl+F5`, save again and refresh.

## URL Guard settings do not persist

Verify this persisted global exists after Save:

```text
CometenUrlGuard_SettingsJson
```

## URL Guard log file does not exist

The file is created only after the first loggable URL deletion while logging is enabled.

Expected location:

```text
<STREAMERBOT_DIR>\logs\CWA_URL_Guard_Deleted_URLs.log
```

## YouTube or Spotify is still deleted

Expected if normal URL Guard delete rules apply.

The YouTube and Spotify options suppress logging only. They do not whitelist those URLs.

## Duplicate alerts or announcements

Check whether more than one Streamer.bot instance has the same production event triggers enabled.