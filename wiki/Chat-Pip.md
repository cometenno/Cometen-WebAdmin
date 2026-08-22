# Chat Pip

Chat Pip plays a configurable notification sound for selected chat activity.

## Main settings

The WebAdmin Chat Pip module exposes:

- enable/disable Chat Pip
- sound file path
- volume
- silence interval
- first message from each user
- pip after the whole chat has been silent
- pip when the same user returns after the selected time
- ignore broadcaster
- ignored usernames

## Public 1.0 persistence fix

Public 1.0 includes the tested fix that aligns WebAdmin field names with the Streamer.bot backend.

The fixed canonical settings include:

```text
newChatterEnabled
quietChatEnabled
quietMinutes
returningUserEnabled
returnMinutes
```

The backend also accepts the older field names for compatibility.

## Testing persistence

1. Change one or more Chat Pip behavior checkboxes.
2. Press **Save settings**.
3. Press **Refresh settings**.
4. Confirm the values remain.
5. Close and reopen WebAdmin.
6. Refresh again.

The values should remain persisted.

## Sound path

The release includes:

```text
chat-pip.mp3
```

Use a valid local path when configuring a local file directly. Public documentation intentionally uses placeholders instead of production paths.