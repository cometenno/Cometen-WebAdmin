# Security Policy

The public Cometen WebAdmin repository and release package must not contain production secrets or private environment data.

Do not publish:

- Discord webhook URLs
- OAuth tokens or API keys
- passwords or database credentials
- private relay/receiver tokens
- production LAN IP addresses
- Windows usernames
- private machine names
- production absolute paths
- Discord role/channel IDs
- other private platform identifiers

Use placeholders such as:

```text
<TWITCH_CHANNEL>
<DISCORD_WEBHOOK_URL>
<PLATFORM_ID>
<LOCAL_PATH>
<STREAMERBOT_PC_IP>
<IRL_RECEIVER_URL>
```

## Runtime logs

`CWA_URL_Guard_Deleted_URLs.log` can contain usernames and URLs removed from chat. Treat it as local operational data and never commit it.

## Reporting an issue

Do not post production credentials in public issues. If a credential is exposed, rotate or revoke it first.
