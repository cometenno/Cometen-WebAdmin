# Security

The public Cometen WebAdmin repository is intentionally sanitized.

## Do not commit

- Discord webhook URLs
- OAuth tokens
- API keys
- passwords
- database credentials
- private relay or receiver tokens
- production LAN IP addresses
- Windows usernames
- private machine names
- production absolute paths
- Discord role/channel IDs
- other private platform identifiers

## Recommended placeholders

```text
<TWITCH_CHANNEL>
<DISCORD_WEBHOOK_URL>
<PLATFORM_ID>
<LOCAL_PATH>
<STREAMERBOT_PC_IP>
<IRL_RECEIVER_URL>
```

## Runtime data

Private values belong in the local Streamer.bot installation or other local configuration.

Do not commit runtime logs.

## URL Guard log

The file:

```text
CWA_URL_Guard_Deleted_URLs.log
```

may contain usernames and URLs removed from chat. Treat it as local operational data.

## Exposed credentials

If a production credential is ever exposed publicly, rotate or revoke it immediately before discussing or documenting the implementation problem.

## License

Cometen WebAdmin is released under the MIT License.