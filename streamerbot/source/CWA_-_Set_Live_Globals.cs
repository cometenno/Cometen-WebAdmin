using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        string webhook = Get("CometenLive_DiscordWebhookUrl", "");
        string lastSentUnix = Get("CometenLive_LastSentUnix", "0");
        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin", type = "LIVE_SETTINGS", action = "globals-reset",
            webhookSet = !string.IsNullOrWhiteSpace(webhook),
            roleMention = Get("CometenLive_DiscordRoleMention", "@Twitch"),
            messageTemplate = Get("CometenLive_MessageTemplate", "🔴 {role} Cometen is LIVE!\n\n{title}\n\nPlaying: {game}\n\nCome hang out:\n{url}"),
            twitchUrl = Get("CometenLive_TwitchUrl", "https://twitch.tv/<TWITCH_CHANNEL>"), botName = Get("CometenLive_BotName", "Cometen Live"), cooldownMinutes = Get("CometenLive_CooldownMinutes", "180"),
            testMode = Get("CometenLive_TestMode", "False"), testGame = Get("CometenLive_TestGame", "The Division 4"), testTitle = Get("CometenLive_TestTitle", "Test live announcement"), testIgnoreCooldown = Get("CometenLive_TestIgnoreCooldown", "False"),
            lastSentUnix = lastSentUnix, lastSentLocal = UnixToLocal(lastSentUnix), lastResult = Get("CometenLive_LastResult", "")
        });
        CPH.WebsocketBroadcastJson(json);
        CPH.LogInfo("[CWA Set Live Globals] Defaults applied by visible Set Global sub-actions.");
        return true;
    }

    private string Get(string key, string fallback) { try { string v = CPH.GetGlobalVar<string>(key, true); return v == null ? fallback : v; } catch { return fallback; } }
    private string UnixToLocal(string raw) { long u; if (!long.TryParse(raw, out u) || u <= 0) return ""; try { return DateTimeOffset.FromUnixTimeSeconds(u).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"); } catch { return ""; } }
}
