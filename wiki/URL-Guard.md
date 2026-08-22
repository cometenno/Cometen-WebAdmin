# URL Guard

URL Guard monitors chat messages and can remove messages containing URLs according to the configured rules.

## Controls

The WebAdmin URL Guard module can control:

- enable/disable URL Guard
- broadcaster allowance
- moderator allowance
- VIP allowance
- ordinary URL deletion
- URL-command deletion
- blocked-message announcements
- additional allowed users
- local deleted-URL logging
- YouTube logging exclusion
- Spotify logging exclusion

## Persistent state

Public 1.0 uses one authoritative persistent Streamer.bot global:

```text
CometenUrlGuard_SettingsJson
```

Existing legacy `CometenUrlGuard_*` globals are migrated automatically and remain mirrored for compatibility.

## Deleted URL log

When enabled, URL Guard writes handled URLs to:

```text
<STREAMERBOT_DIR>\logs\CWA_URL_Guard_Deleted_URLs.log
```

The file is created automatically when the first loggable deletion occurs.

Each URL is stored on its own line with timestamp, user, reason and URL. The complete chat message is not stored.

## YouTube and Spotify exclusions

The settings:

- Do not log YouTube URLs
- Do not log Spotify URLs

apply to logging only.

They do **not** whitelist those URLs from the normal URL Guard delete rules.

## Persistence test

1. Change URL Guard settings.
2. Press **Save**.
3. Press **Refresh**.
4. Confirm values remain.
5. Close and reopen WebAdmin.
6. Refresh again.

If persistence fails, verify that `CometenUrlGuard_SettingsJson` exists in Streamer.bot persisted globals.