# FAQ

## Where do I install Cometen WebAdmin?

Extract the release ZIP inside:

```text
<STREAMERBOT_DIR>\plugins\
```

The final directory must be:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\
```

If `plugins` does not exist, create it first.

## Which Streamer.bot file do I import?

Import:

```text
Cometen_WebAdmin_1.0.sb
```

## Can I use WebAdmin from another PC?

Yes. Enable Streamer.bot WebSocket access on the Streamer.bot PC and allow the required LAN traffic through the firewall.

## Why does WebAdmin work locally but not over LAN?

The Streamer.bot WebSocket server may be bound only to `127.0.0.1`, or the firewall may block the port.

## Why does HTTPS break the WebSocket connection?

A browser page served over HTTPS may block insecure `ws://` as mixed content. Use HTTP on a trusted private LAN or configure secure WebSockets.

## Why are my Chat Pip settings not saved?

Public 1.0 includes the tested Chat Pip persistence fix. Re-import the current `.sb` with overwrite enabled for the CWA Chat Pip actions, reload WebAdmin with `Ctrl+F5`, then Save and Refresh again.

## Why are URL Guard settings not saved?

Public 1.0 stores the authoritative state in:

```text
CometenUrlGuard_SettingsJson
```

After pressing Save, verify that this persisted global exists in Streamer.bot.

## Why are YouTube/Spotify links still deleted?

The YouTube and Spotify switches only exclude those URLs from the deleted-URL log. They do not whitelist them from URL Guard deletion rules.

## Where is the URL Guard log?

```text
<STREAMERBOT_DIR>\logs\CWA_URL_Guard_Deleted_URLs.log
```

The file is created on the first logged deletion.

## Can I use the system without IRL integration?

Yes. IRL forwarding is optional.

## Can I run CWA on two Streamer.bot PCs?

Yes, but avoid enabling the same production event triggers on both instances unless duplicate execution is intentional.

## Does the public release contain my private webhook or machine settings?

No. Public release files are sanitized. Configure private values locally after installation.

## Why does OBS still show the old overlay after an update?

Refresh the Browser Source cache or restart OBS.

## What license is the project under?

MIT License.