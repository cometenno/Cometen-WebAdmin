# Installation

## Requirements

- Streamer.bot
- OBS Studio for overlay use
- A modern browser
- Windows for the included verification helper

## 1. Create the plugin directory

Open your Streamer.bot installation directory.

If this directory does not exist, create it:

```text
<STREAMERBOT_DIR>\plugins
```

## 2. Extract the release ZIP

Download the `CometenWebAdmin_1.0_Public.zip` release asset and extract it **inside** the `plugins` directory.

Correct result:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\
```

Do not create an extra nested directory such as:

```text
plugins\CometenWebAdmin\CometenWebAdmin\
```

## 3. Import Streamer.bot actions

Import:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\Cometen_WebAdmin_1.0.sb
```

When updating an existing CWA installation, review the import list and keep overwrite enabled for the CWA actions being updated.

## 4. Enable Streamer.bot WebSocket

Enable the Streamer.bot WebSocket server.

Recommended default port:

```text
8081
```

For local use, binding to `127.0.0.1` is sufficient.

For LAN use, bind Streamer.bot to an appropriate LAN interface or `0.0.0.0` and create a matching firewall rule.

## 5. Open WebAdmin

Main page:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\Cometen_WebAdmin.html
```

The page can be opened directly or served through Streamer.bot HTTP Server. HTTP serving is recommended for LAN administration.

## 6. Configure private values

The public package is sanitized. Configure your own local values after import, including channel/login information, webhook URLs, platform role/channel IDs, media paths and optional IRL endpoints.

Never commit production secrets to a public repository.

## 7. OBS

Add the supplied Chat Overlay, Alerts and Credits HTML pages as Browser Sources where required.

See `docs/OBS_SETUP.md` for the recommended paths.
