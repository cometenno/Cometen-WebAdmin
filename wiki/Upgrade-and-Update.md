# Upgrade / Update

## Before updating

Back up your Streamer.bot configuration and your current `CometenWebAdmin` plugin directory.

## Standard update

1. Download the latest release ZIP.
2. Extract it into:

```text
<STREAMERBOT_DIR>\plugins\
```

3. Allow the release files to replace the previous Cometen WebAdmin files.
4. Import the included Streamer.bot `.sb` file.
5. Review the import list and keep overwrite enabled for CWA actions being updated.
6. Reload WebAdmin with `Ctrl+F5`.
7. Refresh OBS Browser Sources if cached code remains.

## Preserve local/private values

Verify your locally configured values after an update:

- Twitch channel/login
- Discord webhook URLs
- role/channel IDs
- local media paths
- optional IRL endpoints

Public releases are sanitized and do not contain production secrets.

## URL Guard migration

Public 1.0 uses:

```text
CometenUrlGuard_SettingsJson
```

as the authoritative persisted URL Guard configuration.

Legacy `CometenUrlGuard_*` globals are migrated/mirrored for compatibility.

## Chat Pip migration

Public 1.0 includes the tested Chat Pip persistence fix. If settings do not persist after an update, re-import the current release `.sb` with overwrite enabled for CWA Chat Pip actions and reload WebAdmin.

## Rollback

Restore the previous plugin directory and, if required, restore/import your previous Streamer.bot configuration.