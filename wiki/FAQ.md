# FAQ

## Where do I install Cometen WebAdmin?

Extract the release ZIP into:

```text
<STREAMERBOT_DIR>\plugins\
```

Final path:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\
```

If `plugins` does not exist, create it first.

## Which Streamer.bot file do I import?

```text
Cometen_WebAdmin_1.0.sb
```

## Can I use WebAdmin from another PC?

Yes. Enable Streamer.bot WebSocket access on the Streamer.bot PC and allow the required LAN traffic through the firewall.

## Why does WebAdmin work locally but not over LAN?

The WebSocket server may be bound only to `127.0.0.1`, or the firewall may block the configured port.

## Why are my Chat Pip settings not saved?

Public 1.0 includes the tested Chat Pip persistence fix. Re-import the current `.sb` with overwrite enabled for CWA Chat Pip actions and reload WebAdmin with `Ctrl+F5`.

## Why are URL Guard settings not saved?

Public 1.0 uses:

```text
CometenUrlGuard_SettingsJson
```

as the authoritative persisted state.

## Why are YouTube/Spotify links still deleted?

Those switches control logging only. They do not whitelist those URLs from deletion.

## Where is the URL Guard log?

```text
<STREAMERBOT_DIR>\logs\CWA_URL_Guard_Deleted_URLs.log
```

## Can I use CWA without IRL integration?

Yes. IRL forwarding is optional.

## Why does OBS still show the old overlay after an update?

Refresh the Browser Source cache or restart OBS.

## License

MIT License.