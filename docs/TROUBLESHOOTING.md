# Troubleshooting

## WebSocket shows disconnected

Check that Streamer.bot is running, WebSocket Server is enabled, the configured port matches WebAdmin, and the firewall allows the connection.

## Works locally but not over LAN

Confirm Streamer.bot is not bound only to `127.0.0.1`. Use the Streamer.bot PC LAN address or serve WebAdmin through Streamer.bot HTTP Server.

## Chat Pip settings do not persist

Public 1.0 includes the tested Chat Pip field-name persistence fix. Re-import `Cometen_WebAdmin_1.0.sb` with overwrite enabled for CWA actions and reload WebAdmin with `Ctrl+F5`.

## URL Guard settings do not persist

Public 1.0 uses `CometenUrlGuard_SettingsJson` as the authoritative persistent state. In Streamer.bot Persisted Globals, verify that this variable exists after pressing Save.

## URL log file does not exist

The log is created after the first loggable URL deletion while local URL logging is enabled.

## YouTube or Spotify still gets deleted

Expected if the normal URL Guard deletion rules apply. The YouTube/Spotify options suppress logging only.

## OBS still shows old code

Refresh the Browser Source cache or restart OBS.

## Duplicate alerts or announcements

Check whether two Streamer.bot instances have the same production event triggers enabled.
