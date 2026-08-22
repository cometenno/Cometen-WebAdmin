# URL Guard

URL Guard monitors chat messages and can remove messages containing URLs according to the configured rules.

## Persistent configuration

Public 1.0 uses one authoritative persistent Streamer.bot global:

```text
CometenUrlGuard_SettingsJson
```

Existing legacy `CometenUrlGuard_*` values are migrated automatically where applicable and remain mirrored for compatibility.

## Deleted URL logging

When enabled, URL Guard writes URLs handled by its deletion branch to:

```text
<STREAMERBOT_DIR>\logs\CWA_URL_Guard_Deleted_URLs.log
```

The directory/file is created automatically on the first loggable deletion.

Each URL is written as a separate line with timestamp, user, reason and URL. The complete chat message is not stored.

## YouTube and Spotify exclusions

Optional controls can suppress local logging for common YouTube and Spotify URLs.

These controls affect **logging only**. They do not whitelist those URLs from URL Guard deletion rules.

## Privacy

The URL Guard log is local runtime data and must not be committed to GitHub.
