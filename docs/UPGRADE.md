# Upgrade / Update Guide

This guide covers updates between Cometen WebAdmin releases.

## Before updating

Back up your current Streamer.bot configuration and your existing `CometenWebAdmin` plugin directory.

Recommended backup targets:

- Streamer.bot configuration/export
- local WebAdmin settings you want to preserve
- custom overlay edits
- custom sounds/images
- locally configured webhook URLs and platform IDs

## Standard update procedure

1. Download the latest release ZIP.
2. Stop OBS Browser Sources that actively use the plugin if practical.
3. Extract the ZIP into:

```text
<STREAMERBOT_DIR>\plugins\
```

4. Allow the release files to replace the previous Cometen WebAdmin files.
5. Import the included Streamer.bot `.sb` file.
6. Review the import list.
7. Keep overwrite enabled for CWA actions that are part of the update.
8. Reload WebAdmin with `Ctrl+F5`.
9. Refresh OBS Browser Sources or restart OBS if cached code remains.

## Preserving private/local values

Public releases are sanitized and do not contain production webhook URLs, platform IDs, LAN addresses or private machine paths.

After an update, verify your locally configured values, including:

- Twitch channel/login
- Discord webhook URLs
- role/channel IDs
- local media paths
- optional IRL endpoints

## URL Guard migration

Public 1.0 uses one authoritative persistent setting:

```text
CometenUrlGuard_SettingsJson
```

Legacy `CometenUrlGuard_*` globals are migrated/mirrored for compatibility.

If URL Guard settings appear wrong after an update:

1. Open WebAdmin.
2. Go to Setup / URL Guard.
3. Press Refresh.
4. Change one setting.
5. Press Save.
6. Verify `CometenUrlGuard_SettingsJson` exists in Streamer.bot Persisted Globals.

## Chat Pip migration

Public 1.0 includes the tested Chat Pip persistence fix. If Chat Pip settings do not persist after an update, re-import the current release `.sb` with overwrite enabled for CWA Chat Pip actions, then reload WebAdmin with `Ctrl+F5`.

## Rollback

To roll back:

1. restore the previous `CometenWebAdmin` plugin directory from backup;
2. restore/import your previous Streamer.bot configuration if required;
3. refresh browser sources.

Do not mix old WebAdmin HTML with newer backend actions unless you are intentionally testing compatibility.