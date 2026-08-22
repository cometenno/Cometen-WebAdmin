# Installation

## Requirements

- Streamer.bot
- OBS Studio for overlay use
- modern web browser
- Windows for the supplied helper script

## Plugin directory

Open the Streamer.bot installation directory.

If `plugins` does not exist, create it:

```text
<STREAMERBOT_DIR>\plugins
```

Extract the release ZIP inside that folder.

The final layout must be:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\
```

Do not create an extra nested folder such as:

```text
plugins\CometenWebAdmin\CometenWebAdmin\
```

## Streamer.bot import

Import:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\Cometen_WebAdmin_1.0.sb
```

For updates, review the import list and allow overwrite for the CWA actions being updated.

## WebSocket

Enable Streamer.bot WebSocket Server.

Recommended port:

```text
8081
```

For LAN use, bind Streamer.bot to an appropriate LAN interface or `0.0.0.0` and configure the Windows Firewall rule accordingly.

## WebAdmin

Open:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\Cometen_WebAdmin.html
```

## Private values

Configure private values locally after installation. The public package intentionally does not include production webhook URLs, private LAN addresses, Windows usernames, local production paths, platform IDs or private IRL endpoints.

## Verification

From the `CometenWebAdmin` directory, run:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\verify-install.ps1
```

Continue with [Network and LAN](Network-and-LAN) and [OBS Setup](OBS-Setup).