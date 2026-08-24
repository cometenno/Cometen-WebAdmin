using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        BroadcastStatus("status");
        return true;
    }

    private void BroadcastStatus(string action)
    {
        bool isLive = GetBoolValue("ss_is_live", false);
        DateTimeOffset nowUtc = DateTimeOffset.UtcNow;
        DateTimeOffset startUtc = ReadStartUtc(nowUtc);
        TimeSpan duration = isLive && startUtc <= nowUtc ? nowUtc - startUtc : ReadLastDuration();
        if (duration.TotalSeconds < 0) duration = TimeSpan.Zero;

        int viewerSum = GetInt("ss_viewer_sum", 0);
        int viewerSamples = GetInt("ss_viewer_samples", 0);
        int average = viewerSamples > 0 ? (int)Math.Floor(viewerSum / (double)viewerSamples) : 0;
        int chatRaw = GetInt("ss_chat_messages", 0);
        int chatOffset = GetInt("CometenSummary_ChatMessageOffset", 0);
        int chatAdjusted = Math.Max(0, chatRaw + chatOffset);

        string summaryWebhook = GetGlobal("CometenSummary_DiscordWebhookUrl", "");
        string liveWebhook = GetGlobal("CometenLive_DiscordWebhookUrl", "");
        bool useLive = GetBool("CometenSummary_UseLiveWebhook", true);
        long startUnix = startUtc == nowUtc && !isLive ? 0 : startUtc.ToUnixTimeSeconds();

        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin",
            type = "SUMMARY_SETTINGS",
            action = action,
            enabled = GetBool("CometenSummary_Enabled", true),
            autoSend = GetBool("CometenSummary_AutoSend", true),
            useLiveWebhook = useLive,
            webhookSet = useLive ? !string.IsNullOrWhiteSpace(liveWebhook) : !string.IsNullOrWhiteSpace(summaryWebhook),
            liveWebhookSet = !string.IsNullOrWhiteSpace(liveWebhook),
            summaryWebhookSet = !string.IsNullOrWhiteSpace(summaryWebhook),
            channelLogin = FirstNonEmpty(GetGlobal("CometenSummary_ChannelLogin", ""), GetGlobal("ss_channel_login", ""), "<TWITCH_CHANNEL>"),
            maxClips = GetInt("CometenSummary_MaxClips", 3),
            chatMessageOffset = chatOffset,
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
            isLive = isLive,
            startUnix = startUnix,
            startLocal = startUnix > 0 ? startUtc.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
            durationSeconds = (long)duration.TotalSeconds,
            durationText = FormatDuration(duration, GetBool("CometenSummary_ShowSeconds", true)),
            chatMessagesRaw = chatRaw,
            chatMessages = chatAdjusted,
            follows = GetInt("ss_follows", 0),
            subs = GetInt("ss_subs", 0),
            raids = GetInt("ss_raids", 0),
            peakViewers = GetInt("ss_viewer_peak", 0),
            averageViewers = average,
            viewerSamples = viewerSamples,
            clipBaselineCount = GetInt("ss_clip_baseline_count", 0),
            currentStreamClipCount = CountCurrentStreamClips(),
            lastSentLocal = UnixToLocal(GetGlobal("CometenSummary_LastSentUnix", "0")),
            lastResult = GetGlobal("CometenSummary_LastResult", ""),
            lastSummary = GetGlobal("ss_last_summary", "")
        });

        CPH.WebsocketBroadcastJson(json);
    }

    private int CountCurrentStreamClips()
    {
        try
        {
            string channel = FirstNonEmpty(GetGlobal("CometenSummary_ChannelLogin", ""), GetGlobal("ss_channel_login", ""), "<TWITCH_CHANNEL>");
            var clips = CPH.GetClipsForUser(channel);
            if (clips == null || clips.Count == 0) return 0;

            List<string> before = ReadKeys(GetGlobal("ss_clip_keys_before", ""));
            bool baselineOk = GetBoolValue("ss_clip_baseline_ok", false);
            if (!baselineOk) return 0;

            List<string> current = new List<string>();
            foreach (var clip in clips)
            {
                string key = GetClipKey(clip);
                if (!string.IsNullOrWhiteSpace(key) && !ContainsIgnoreCase(before, key) && !ContainsIgnoreCase(current, key))
                    current.Add(key);
            }
            return current.Count;
        }
        catch { return 0; }
    }

    private List<string> ReadKeys(string raw)
    {
        List<string> result = new List<string>();

        foreach (string item in (raw ?? "").Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            string value = item.Trim();
            if (!string.IsNullOrWhiteSpace(value) && !ContainsIgnoreCase(result, value))
                result.Add(value);
        }

        return result;
    }

    private bool ContainsIgnoreCase(List<string> values, string value)
    {
        if (values == null || string.IsNullOrWhiteSpace(value))
            return false;

        for (int i = 0; i < values.Count; i++)
        {
            if (string.Equals(values[i], value, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private string GetClipKey(object clip)
    {
        return FirstNonEmpty(SafeGet(clip, "Id"), SafeGet(clip, "ID"), SafeGet(clip, "ClipId"), SafeGet(clip, "ClipID"), SafeGet(clip, "Url"), SafeGet(clip, "URL"));
    }

    private string SafeGet(object obj, string name)
    {
        if (obj == null) return "";
        try
        {
            PropertyInfo p = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            object value = p == null ? null : p.GetValue(obj, null);
            return value == null ? "" : value.ToString();
        }
        catch { return ""; }
    }

    private DateTimeOffset ReadStartUtc(DateTimeOffset fallback)
    {
        long unix;
        if (long.TryParse(GetGlobal("ss_start_unix", "0"), out unix) && unix > 0)
        {
            try { return DateTimeOffset.FromUnixTimeSeconds(unix).ToUniversalTime(); } catch { }
        }

        DateTimeOffset parsed;
        if (DateTimeOffset.TryParse(GetGlobal("ss_start_utc", ""), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsed))
            return parsed.ToUniversalTime();
        return fallback;
    }

    private TimeSpan ReadLastDuration()
    {
        long seconds;
        if (long.TryParse(GetGlobal("ss_last_duration_seconds", "0"), out seconds) && seconds > 0)
            return TimeSpan.FromSeconds(seconds);
        return TimeSpan.Zero;
    }

    private string FormatDuration(TimeSpan d, bool seconds)
    {
        string text = ((int)d.TotalHours) + "h " + d.Minutes + "m";
        if (seconds) text += " " + d.Seconds + "s";
        return text;
    }

    private string UnixToLocal(string raw)
    {
        long unix;
        if (!long.TryParse(raw, out unix) || unix <= 0) return "";
        try { return DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"); } catch { return ""; }
    }

    private string FirstNonEmpty(params string[] values)
    {
        foreach (string value in values) if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
        return "";
    }

    private string GetGlobal(string key, string fallback)
    {
        try { string value = CPH.GetGlobalVar<string>(key, true); return value == null ? fallback : value; }
        catch { return fallback; }
    }

    private int GetInt(string key, int fallback)
    {
        try { return CPH.GetGlobalVar<int>(key, true); } catch { }
        int n; return int.TryParse(GetGlobal(key, ""), out n) ? n : fallback;
    }

    private bool GetBoolValue(string key, bool fallback)
    {
        try { return CPH.GetGlobalVar<bool>(key, true); } catch { }
        return GetBool(key, fallback);
    }

    private bool GetBool(string key, bool fallback)
    {
        string v = GetGlobal(key, "").Trim().ToLowerInvariant();
        if (v == "true" || v == "1" || v == "yes" || v == "on") return true;
        if (v == "false" || v == "0" || v == "no" || v == "off") return false;
        return fallback;
    }
}
