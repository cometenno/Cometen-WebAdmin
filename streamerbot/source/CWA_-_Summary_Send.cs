using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

public class CPHInline
{
    private class ClipEntry
    {
        public string Key;
        public string Title;
        public string Url;
        public DateTimeOffset CreatedUtc;
        public bool HasCreatedTime;
    }

    public bool Execute()
    {
        bool testMode = GetBoolArg("testMode", false);
        // Stream Offline safety reset for the one-stream live text.
        if (!testMode)
            CPH.SetGlobalVar("CometenLive_CustomMessage", "", true);
        bool forceSend = testMode || GetBoolArg("forceSend", false);
        bool keepLive = testMode || GetBoolArg("keepLive", false);
        bool isLive = GetBoolValue("ss_is_live", false);

        if (!GetBool("CometenSummary_Enabled", true) && !forceSend)
        {
            SetResult("Summary module disabled.");
            BroadcastStatus("disabled");
            return true;
        }

        if (!isLive && !forceSend)
        {
            SetResult("Stream was not marked live - summary skipped.");
            BroadcastStatus("not-live");
            return true;
        }

        if (!keepLive) CPH.SetGlobalVar("ss_is_live", false, true);

        if (!testMode && !forceSend && !GetBool("CometenSummary_AutoSend", true))
        {
            SetResult("Automatic summary sending is disabled.");
            BroadcastStatus("auto-send-disabled");
            return true;
        }

        string webhook = ResolveWebhook();
        if (string.IsNullOrWhiteSpace(webhook))
        {
            SetResult("No Discord webhook saved for Stream Summary.");
            BroadcastStatus("missing-webhook");
            CPH.LogError("[CWA Summary] No usable Discord webhook saved.");
            return false;
        }

        DateTimeOffset endUtc = DateTimeOffset.UtcNow;
        DateTimeOffset startUtc = testMode ? endUtc.AddHours(-2).AddMinutes(-43) : ReadStartUtc(endUtc);
        TimeSpan duration = endUtc - startUtc;
        if (duration.TotalSeconds < 0) duration = TimeSpan.Zero;

        int chatMessages = testMode ? 210 : Math.Max(0, GetInt("ss_chat_messages", 0) + GetInt("CometenSummary_ChatMessageOffset", 0));
        int follows = testMode ? 0 : GetInt("ss_follows", 0);
        int subs = testMode ? 0 : GetInt("ss_subs", 0);
        int raids = testMode ? 0 : GetInt("ss_raids", 0);
        int peakViewers = testMode ? 9 : GetInt("ss_viewer_peak", 0);
        int viewerSum = testMode ? 128 : GetInt("ss_viewer_sum", 0);
        int viewerSamples = testMode ? 32 : GetInt("ss_viewer_samples", 0);
        int avgViewers = viewerSamples > 0 ? (int)Math.Floor(viewerSum / (double)viewerSamples) : 0;

        string channelLogin = FirstNonEmpty(GetGlobal("CometenSummary_ChannelLogin", ""), GetGlobal("ss_channel_login", ""), "<TWITCH_CHANNEL>");
        string clipsText = testMode ? "1. Test clip from current stream\nhttps://clips.twitch.tv/example" : GetCurrentStreamClips(channelLogin, startUtc, endUtc);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("**" + GetGlobal("CometenSummary_Title", "📊 Stream Summary") + "**");
        sb.AppendLine();
        sb.AppendLine("⏱️ **Stream length:** " + FormatDuration(duration, GetBool("CometenSummary_ShowSeconds", true)));
        if (GetBool("CometenSummary_IncludePeakViewers", true)) sb.AppendLine("👀 **Peak viewers:** " + (peakViewers > 0 ? peakViewers.ToString() : "Not logged"));
        if (GetBool("CometenSummary_IncludeAverageViewers", true)) sb.AppendLine("📈 **Average viewers (estimated):** " + (viewerSamples > 0 ? avgViewers.ToString() : "Not logged"));
        if (GetBool("CometenSummary_IncludeChatMessages", true)) sb.AppendLine("💬 **Chat messages:** " + chatMessages);
        if (GetBool("CometenSummary_IncludeFollowers", true)) sb.AppendLine("👤 **New followers:** " + follows);
        if (GetBool("CometenSummary_IncludeSubs", true)) sb.AppendLine("⭐ **Subs:** " + subs);
        if (GetBool("CometenSummary_IncludeRaids", true)) sb.AppendLine("🚀 **Raids received:** " + raids);

        if (GetBool("CometenSummary_IncludeClips", true))
        {
            sb.AppendLine();
            if (!string.IsNullOrWhiteSpace(clipsText))
            {
                sb.AppendLine("🎬 **Clips from this stream:**");
                sb.AppendLine(clipsText.TrimEnd());
            }
            else
            {
                sb.AppendLine("🎬 **Clips from this stream:** None found.");
            }
        }

        string summaryText = sb.ToString();

        try
        {
            SendDiscord(webhook, summaryText);
            long nowUnix = endUtc.ToUnixTimeSeconds();
            CPH.SetGlobalVar("ss_last_summary", summaryText, true);
            CPH.SetGlobalVar("ss_last_duration_seconds", ((long)duration.TotalSeconds).ToString(), true);
            CPH.SetGlobalVar("ss_last_end_unix", nowUnix.ToString(), true);
            CPH.SetGlobalVar("CometenSummary_LastSentUnix", nowUnix.ToString(), true);
            SetResult(testMode ? "Test summary sent." : "Summary sent.");
            BroadcastStatus(testMode ? "test-sent" : "sent");
            CPH.LogInfo("[CWA Summary] " + (testMode ? "Test summary" : "Summary") + " sent. Duration " + FormatDuration(duration, true) + ".");
            return true;
        }
        catch (Exception ex)
        {
            SetResult("Discord send failed: " + ex.Message);
            BroadcastStatus("send-error");
            CPH.LogError("[CWA Summary] Discord send failed: " + ex.Message);
            return false;
        }
    }

    private string ResolveWebhook()
    {
        if (GetBool("CometenSummary_UseLiveWebhook", true))
        {
            string live = GetGlobal("CometenLive_DiscordWebhookUrl", "");
            if (!string.IsNullOrWhiteSpace(live)) return live;
        }
        return FirstNonEmpty(GetGlobal("CometenSummary_DiscordWebhookUrl", ""), GetGlobal("ss_discord_webhook", ""));
    }

    private string GetCurrentStreamClips(string channelLogin, DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        try
        {
            var clips = CPH.GetClipsForUser(channelLogin, 100, null);
            if (clips == null || clips.Count == 0) return "";

            bool currentOnly = GetBool("CometenSummary_CurrentStreamClipsOnly", true);
            bool baselineOk = GetBoolValue("ss_clip_baseline_ok", false);
            List<string> before = ReadKeys(GetGlobal("ss_clip_keys_before", ""));
            List<string> added = new List<string>();
            List<ClipEntry> selected = new List<ClipEntry>();

            foreach (var clip in clips)
            {
                string key = GetClipKey(clip);
                if (string.IsNullOrWhiteSpace(key) || ContainsIgnoreCase(added, key)) continue;
                added.Add(key);

                DateTimeOffset createdUtc;
                bool hasCreated = TryGetCreatedUtc(clip, out createdUtc);
                bool include = true;

                if (currentOnly)
                {
                    if (baselineOk) include = !ContainsIgnoreCase(before, key);
                    else include = hasCreated && createdUtc >= startUtc.AddMinutes(-2) && createdUtc <= endUtc.AddMinutes(15);
                }

                if (!include) continue;
                selected.Add(new ClipEntry
                {
                    Key = key,
                    Title = FirstNonEmpty(SafeGet(clip, "Title"), "Clip"),
                    Url = FirstNonEmpty(SafeGet(clip, "Url"), SafeGet(clip, "URL")),
                    CreatedUtc = createdUtc,
                    HasCreatedTime = hasCreated
                });
            }

            selected.Sort(delegate(ClipEntry a, ClipEntry b)
            {
                if (a.HasCreatedTime && b.HasCreatedTime) return b.CreatedUtc.CompareTo(a.CreatedUtc);
                if (a.HasCreatedTime) return -1;
                if (b.HasCreatedTime) return 1;
                return 0;
            });

            int max = Math.Max(0, Math.Min(10, GetInt("CometenSummary_MaxClips", 3)));
            if (max == 0 || selected.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Math.Min(max, selected.Count); i++)
            {
                sb.AppendLine((i + 1) + ". " + selected[i].Title);
                if (!string.IsNullOrWhiteSpace(selected[i].Url)) sb.AppendLine(selected[i].Url);
            }
            return sb.ToString().TrimEnd();
        }
        catch (Exception ex)
        {
            CPH.LogError("[CWA Summary] Clip fetch failed: " + ex.Message);
            return "";
        }
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

    private bool TryGetCreatedUtc(object clip, out DateTimeOffset createdUtc)
    {
        createdUtc = DateTimeOffset.MinValue;
        string[] names = { "CreatedAt", "CreatedAtUtc", "Created", "CreationDate" };
        foreach (string name in names)
        {
            object value = GetPropertyValue(clip, name);
            if (value == null) continue;
            if (value is DateTimeOffset) { createdUtc = ((DateTimeOffset)value).ToUniversalTime(); return true; }
            if (value is DateTime)
            {
                DateTime d = (DateTime)value;
                if (d.Kind == DateTimeKind.Unspecified) d = DateTime.SpecifyKind(d, DateTimeKind.Utc);
                createdUtc = new DateTimeOffset(d.ToUniversalTime());
                return true;
            }
            DateTimeOffset parsed;
            if (DateTimeOffset.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsed))
            {
                createdUtc = parsed.ToUniversalTime();
                return true;
            }
        }
        return false;
    }

    private object GetPropertyValue(object obj, string name)
    {
        if (obj == null) return null;
        try
        {
            PropertyInfo p = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return p == null ? null : p.GetValue(obj, null);
        }
        catch { return null; }
    }

    private string SafeGet(object obj, string name)
    {
        object value = GetPropertyValue(obj, name);
        return value == null ? "" : value.ToString();
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

    private string FormatDuration(TimeSpan d, bool seconds)
    {
        string result = ((int)d.TotalHours) + "h " + d.Minutes + "m";
        if (seconds) result += " " + d.Seconds + "s";
        return result;
    }

    private void SendDiscord(string webhook, string content)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        if (content == null) content = "";
        if (content.Length > 1950)
            content = content.Substring(0, 1900) + "\n\n[Summary truncated to fit Discord.]";

        string json = JsonConvert.SerializeObject(new { content = content, username = "Cometen Stream Summary" });

        try
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                client.UploadString(webhook, "POST", json);
            }
        }
        catch (WebException ex)
        {
            string detail = ex.Message;
            try
            {
                if (ex.Response != null)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        string body = reader.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(body))
                        {
                            if (body.Length > 500) body = body.Substring(0, 500);
                            detail += " | Discord: " + body;
                        }
                    }
                }
            }
            catch { }

            throw new Exception(detail, ex);
        }
    }

    private void SetResult(string value) { CPH.SetGlobalVar("CometenSummary_LastResult", value, true); }

    private void BroadcastStatus(string action)
    {
        string summaryWebhook = GetGlobal("CometenSummary_DiscordWebhookUrl", "");
        string liveWebhook = GetGlobal("CometenLive_DiscordWebhookUrl", "");
        bool useLive = GetBool("CometenSummary_UseLiveWebhook", true);
        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin", type = "SUMMARY_SETTINGS", action = action,
            isLive = GetBoolValue("ss_is_live", false),
            useLiveWebhook = useLive,
            webhookSet = (useLive && !string.IsNullOrWhiteSpace(liveWebhook)) || !string.IsNullOrWhiteSpace(summaryWebhook),
            liveWebhookSet = !string.IsNullOrWhiteSpace(liveWebhook),
            summaryWebhookSet = !string.IsNullOrWhiteSpace(summaryWebhook),
            lastSentLocal = UnixToLocal(GetGlobal("CometenSummary_LastSentUnix", "0")),
            lastResult = GetGlobal("CometenSummary_LastResult", ""),
            lastSummary = GetGlobal("ss_last_summary", "")
        });
        CPH.WebsocketBroadcastJson(json);
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

    private string GetArg(string key, string fallback)
    {
        try { if (args != null && args.ContainsKey(key) && args[key] != null) return args[key].ToString(); } catch { }
        try { string value; if (CPH.TryGetArg(key, out value) && value != null) return value; } catch { }
        return fallback;
    }

    private bool GetBoolArg(string key, bool fallback) { return ParseBool(GetArg(key, ""), fallback); }

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

    private bool GetBool(string key, bool fallback) { return ParseBool(GetGlobal(key, ""), fallback); }

    private bool ParseBool(string value, bool fallback)
    {
        string v = (value ?? "").Trim().ToLowerInvariant();
        if (v == "true" || v == "1" || v == "yes" || v == "on") return true;
        if (v == "false" || v == "0" || v == "no" || v == "off") return false;
        return fallback;
    }
}
