# Alerts

Cometen WebAdmin includes an alert administration module and an OBS/browser alert overlay.

## Alert overlay

Main browser source:

```text
<STREAMERBOT_DIR>\plugins\CometenWebAdmin\alerts\alerts.html
```

Recommended design resolution:

```text
1920 x 1080
```

Keep the full `alerts` directory intact so image and sound paths remain valid.

## Supported alert categories

The CWA alert system is designed to handle common stream events such as:

- Follow
- Sub
- Resub
- Gifted Sub
- Gift Bomb
- Bits
- Donation
- Raid
- YouTube Sub

Exact event behavior depends on the imported Streamer.bot actions and locally configured platform connections.

## IRL forwarding

Normal OBS alerts and IRL forwarding are separate concerns.

A normal alert can remain enabled while IRL forwarding for the same event is disabled.

See [IRL Integration](IRL-Integration).

## Media files

The public release package includes the alert media used by the supplied overlay.

Do not rename or move individual media files unless the corresponding overlay/configuration is also updated.