using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        SaveBool("CometenSummary_Enabled", "enabled", true);
        SaveBool("CometenSummary_AutoSend", "autoSend", true);
        SaveBool("CometenSummary_UseLiveWebhook", "useLiveWebhook", true);
        SaveStringIfProvided("CometenSummary_DiscordWebhookUrl", "discordWebhookUrl");
        SaveString("CometenSummary_ChannelLogin", "channelLogin", "<TWITCH_CHANNEL>");
        SaveInt("CometenSummary_MaxClips", "maxClips", 3, 0, 10);
        SaveInt("CometenSummary_ChatMessageOffset", "chatMessageOffset", 0, -100, 100);
        SaveString("CometenSummary_Title", "title", "📊 Stream Summary");
        SaveBool("CometenSummary_CurrentStreamClipsOnly", "currentStreamClipsOnly", true);
        SaveBool("CometenSummary_ShowSeconds", "showSeconds", true);
        SaveBool("CometenSummary_IncludeClips", "includeClips", true);
        SaveBool("CometenSummary_IncludePeakViewers", "includePeakViewers", true);
        SaveBool("CometenSummary_IncludeAverageViewers", "includeAverageViewers", true);
        SaveBool("CometenSummary_IncludeChatMessages", "includeChatMessages", true);
        SaveBool("CometenSummary_IncludeFollowers", "includeFollowers", true);
        SaveBool("CometenSummary_IncludeSubs", "includeSubs", true);
        SaveBool("CometenSummary_IncludeRaids", "includeRaids", true);

        // Keep the original Stream Summary globals compatible with older counter/actions.
        CPH.SetGlobalVar("ss_channel_login", GetGlobal("CometenSummary_ChannelLogin", "<TWITCH_CHANNEL>"), true);
        string summaryWebhook = GetGlobal("CometenSummary_DiscordWebhookUrl", "");
        if (!string.IsNullOrWhiteSpace(summaryWebhook))
            CPH.SetGlobalVar("ss_discord_webhook", summaryWebhook, true);

        CPH.SetGlobalVar("CometenSummary_LastResult", "Settings saved.", true);
        BroadcastStatus("settings-saved");
        CPH.LogInfo("[CWA Summary] Settings saved from WebAdmin.");
        return true;
    }

    private void SaveString(string key, string argName, string fallback)
    {
        string value = GetArg(argName, fallback);
        CPH.SetGlobalVar(key, value ?? fallback, true);
    }

    private void SaveStringIfProvided(string key, string argName)
    {
        string value = GetArg(argName, "");
        if (!string.IsNullOrWhiteSpace(value))
            CPH.SetGlobalVar(key, value.Trim(), true);
    }

    private void SaveInt(string key, string argName, int fallback, int min, int max)
    {
        int value;
        if (!int.TryParse(GetArg(argName, ""), out value)) value = fallback;
        value = Math.Max(min, Math.Min(max, value));
        CPH.SetGlobalVar(key, value.ToString(), true);
    }

    private void SaveBool(string key, string argName, bool fallback)
    {
        CPH.SetGlobalVar(key, ParseBool(GetArg(argName, ""), fallback) ? "True" : "False", true);
    }

    private string GetArg(string key, string fallback)
    {
        try
        {
            if (args != null && args.ContainsKey(key) && args[key] != null)
                return args[key].ToString();
        }
        catch { }
        try
        {
            string value;
            if (CPH.TryGetArg(key, out value) && value != null) return value;
        }
        catch { }
        return fallback;
    }

    private bool ParseBool(string value, bool fallback)
    {
        string v = (value ?? "").Trim().ToLowerInvariant();
        if (v == "true" || v == "1" || v == "yes" || v == "on") return true;
        if (v == "false" || v == "0" || v == "no" || v == "off") return false;
        return fallback;
    }

    private string GetGlobal(string key, string fallback)
    {
        try
        {
            string value = CPH.GetGlobalVar<string>(key, true);
            return value == null ? fallback : value;
        }
        catch { return fallback; }
    }

    private bool GetBool(string key, bool fallback) { return ParseBool(GetGlobal(key, ""), fallback); }
    private int GetInt(string key, int fallback) { int n; return int.TryParse(GetGlobal(key, ""), out n) ? n : fallback; }

    private void BroadcastStatus(string action)
    {
        string summaryWebhook = GetGlobal("CometenSummary_DiscordWebhookUrl", "");
        string liveWebhook = GetGlobal("CometenLive_DiscordWebhookUrl", "");
        bool useLive = GetBool("CometenSummary_UseLiveWebhook", true);
        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin", type = "SUMMARY_SETTINGS", action = action,
            enabled = GetBool("CometenSummary_Enabled", true),
            autoSend = GetBool("CometenSummary_AutoSend", true),
            useLiveWebhook = useLive,
            webhookSet = useLive ? !string.IsNullOrWhiteSpace(liveWebhook) : !string.IsNullOrWhiteSpace(summaryWebhook),
            liveWebhookSet = !string.IsNullOrWhiteSpace(liveWebhook),
            summaryWebhookSet = !string.IsNullOrWhiteSpace(summaryWebhook),
            channelLogin = GetGlobal("CometenSummary_ChannelLogin", "<TWITCH_CHANNEL>"),
            maxClips = GetInt("CometenSummary_MaxClips", 3),
            chatMessageOffset = GetInt("CometenSummary_ChatMessageOffset", 0),
            title = GetGlobal("CometenSummary_Title", "📊 Stream Summary"),
            currentStreamClipsOnly = GetBool("CometenSummary_CurrentStreamClipsOnly", true),
            showSeconds = GetBool("CometenSummary_ShowSeconds", true),
            includeClips = GetBool("CometenSummary_IncludeClips", true),
            includePeakViewers = GetBool("CometenSummary_IncludePeakViewers", true),
            includeAverageViewers = GetBool("CometenSummary_IncludeAverageViewers", true),
            includeChatMessages = GetBool("CometenSummary_IncludeChatMessages", true),
            includeFollowers = GetBool("CometenSummary_IncludeFollowers", true),
            includeSubs = GetBool("CometenSummary_IncludeSubs", true),
            includeRaids = GetBool("CometenSummary_IncludeRaids", true),
            lastResult = GetGlobal("CometenSummary_LastResult", "Settings saved.")
        });
        CPH.WebsocketBroadcastJson(json);
    }
}
