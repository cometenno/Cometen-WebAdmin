using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        // Webhook field is special: blank means keep current saved webhook.
        string webhook = GetArgString("discordWebhookUrl", "");
        if (!string.IsNullOrWhiteSpace(webhook))
            CPH.SetGlobalVar("CometenLive_DiscordWebhookUrl", webhook.Trim(), true);

        SetFromArg("roleMention", "CometenLive_DiscordRoleMention");
        SetFromArg("messageTemplate", "CometenLive_MessageTemplate");
        SetFromArg("customMessage", "CometenLive_CustomMessage");
        SetFromArg("twitchUrl", "CometenLive_TwitchUrl");
        SetFromArg("botName", "CometenLive_BotName");
        SetFromArg("cooldownMinutes", "CometenLive_CooldownMinutes");
        SetFromArg("testMode", "CometenLive_TestMode");
        SetFromArg("testGame", "CometenLive_TestGame");
        SetFromArg("testTitle", "CometenLive_TestTitle");
        SetFromArg("testIgnoreCooldown", "CometenLive_TestIgnoreCooldown");

        BroadcastStatus("saved");
        CPH.LogInfo("[CWA Live] Live announcement settings saved from WebAdmin.");
        return true;
    }

    private void SetFromArg(string argName, string globalName)
    {
        string value;
        if (TryGetArgString(argName, out value))
            CPH.SetGlobalVar(globalName, value, true);
    }

    private string GetArgString(string name, string fallback)
    {
        string value;
        if (TryGetArgString(name, out value))
            return value;
        return fallback;
    }

    private bool TryGetArgString(string name, out string value)
    {
        value = null;
        try
        {
            if (args != null && args.ContainsKey(name) && args[name] != null)
            {
                value = args[name].ToString();
                return true;
            }
        }
        catch { }

        try
        {
            string temp;
            if (CPH.TryGetArg(name, out temp))
            {
                value = temp;
                return true;
            }
        }
        catch { }

        return false;
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
            try { lastSentLocal = DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"); } catch { }
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
