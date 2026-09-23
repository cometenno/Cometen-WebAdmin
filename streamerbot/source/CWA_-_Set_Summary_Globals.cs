using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        // Native Set Global sub-actions above this C# block apply the actual defaults.
        CPH.SetGlobalVar("ss_channel_login", Get("CometenSummary_ChannelLogin", "<TWITCH_CHANNEL>"), true);
        CPH.SetGlobalVar("ss_discord_webhook", Get("CometenSummary_DiscordWebhookUrl", ""), true);

        string summaryWebhook = Get("CometenSummary_DiscordWebhookUrl", "");
        string liveWebhook = Get("CometenLive_DiscordWebhookUrl", "");
        bool useLive = GetBool("CometenSummary_UseLiveWebhook", true);
        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin", type = "SUMMARY_SETTINGS", action = "globals-reset",
            enabled = GetBool("CometenSummary_Enabled", true), autoSend = GetBool("CometenSummary_AutoSend", true), useLiveWebhook = useLive,
            webhookSet = useLive ? !string.IsNullOrWhiteSpace(liveWebhook) : !string.IsNullOrWhiteSpace(summaryWebhook), liveWebhookSet = !string.IsNullOrWhiteSpace(liveWebhook), summaryWebhookSet = !string.IsNullOrWhiteSpace(summaryWebhook),
            channelLogin = Get("CometenSummary_ChannelLogin", "<TWITCH_CHANNEL>"), maxClips = GetInt("CometenSummary_MaxClips", 3), chatMessageOffset = GetInt("CometenSummary_ChatMessageOffset", 0), title = Get("CometenSummary_Title", "📊 Stream Summary"),
            currentStreamClipsOnly = GetBool("CometenSummary_CurrentStreamClipsOnly", true), showSeconds = GetBool("CometenSummary_ShowSeconds", true),
            includeClips = GetBool("CometenSummary_IncludeClips", true), includePeakViewers = GetBool("CometenSummary_IncludePeakViewers", true), includeAverageViewers = GetBool("CometenSummary_IncludeAverageViewers", true), includeChatMessages = GetBool("CometenSummary_IncludeChatMessages", true), includeFollowers = GetBool("CometenSummary_IncludeFollowers", true), includeSubs = GetBool("CometenSummary_IncludeSubs", true), includeRaids = GetBool("CometenSummary_IncludeRaids", true),
            isLive = false, startUnix = 0, startLocal = "", durationText = "0h 0m 0s", chatMessages = 0, follows = 0, subs = 0, raids = 0, peakViewers = 0, averageViewers = 0, viewerSamples = 0, currentStreamClipCount = 0,
            lastResult = Get("CometenSummary_LastResult", "Summary globals reset."), lastSummary = Get("ss_last_summary", "")
        });
        CPH.WebsocketBroadcastJson(json);
        CPH.LogInfo("[CWA Set Summary Globals] Defaults applied by visible Set Global sub-actions.");
        return true;
    }

    private string Get(string key, string fallback) { try { string v = CPH.GetGlobalVar<string>(key, true); return v == null ? fallback : v; } catch { return fallback; } }
    private int GetInt(string key, int fallback) { int n; return int.TryParse(Get(key, ""), out n) ? n : fallback; }
    private bool GetBool(string key, bool fallback) { string v = Get(key, "").Trim().ToLowerInvariant(); if (v == "true" || v == "1" || v == "yes" || v == "on") return true; if (v == "false" || v == "0" || v == "no" || v == "off") return false; return fallback; }
}
