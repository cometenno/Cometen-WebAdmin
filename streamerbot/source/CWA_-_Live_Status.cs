using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        BroadcastStatus("status");
        CPH.LogInfo("[CWA Live] Status sent to WebAdmin.");
        return true;
    }

    private string GetGlobal(string key, string fallback)
    {
        try
        {
            string value = CPH.GetGlobalVar<string>(key, true);
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }
        catch { }

        return fallback;
    }

    private void BroadcastStatus(string action)
    {
        string webhook = GetGlobal("CometenLive_DiscordWebhookUrl", "");
        string lastSentUnix = GetGlobal("CometenLive_LastSentUnix", "0");
        string lastSentLocal = "";

        long unix;
        if (long.TryParse(lastSentUnix, out unix) && unix > 0)
        {
            try
            {
                lastSentLocal = DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch { }
        }

        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin",
            type = "LIVE_SETTINGS",
            action = action,
            webhookSet = !string.IsNullOrWhiteSpace(webhook),
            roleMention = GetGlobal("CometenLive_DiscordRoleMention", "@Twitch"),
            messageTemplate = GetGlobal("CometenLive_MessageTemplate", "🔴 {role} Cometen is LIVE!\n\n{custom}{title}\n\nPlaying: {game}\n\nCome hang out:\n{url}"),
            customMessage = GetGlobal("CometenLive_CustomMessage", ""),
            twitchUrl = GetGlobal("CometenLive_TwitchUrl", "https://twitch.tv/<TWITCH_CHANNEL>"),
            botName = GetGlobal("CometenLive_BotName", "Cometen Live"),
            cooldownMinutes = GetGlobal("CometenLive_CooldownMinutes", "180"),
            testMode = GetGlobal("CometenLive_TestMode", "False"),
            testGame = GetGlobal("CometenLive_TestGame", "The Division 4"),
            testTitle = GetGlobal("CometenLive_TestTitle", ""),
            testIgnoreCooldown = GetGlobal("CometenLive_TestIgnoreCooldown", "False"),
            lastSentUnix = lastSentUnix,
            lastSentLocal = lastSentLocal,
            lastResult = GetGlobal("CometenLive_LastResult", "")
        });

        CPH.WebsocketBroadcastJson(json);
    }
}
