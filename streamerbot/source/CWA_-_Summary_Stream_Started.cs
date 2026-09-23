using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        DateTimeOffset nowUtc = DateTimeOffset.UtcNow;
        long startUnix = nowUtc.ToUnixTimeSeconds();

        CPH.SetGlobalVar("ss_is_live", true, true);
        CPH.SetGlobalVar("ss_start_unix", startUnix.ToString(), true);
        CPH.SetGlobalVar("ss_start_utc", nowUtc.ToString("o"), true);

        CPH.SetGlobalVar("ss_chat_messages", 0, true);
        CPH.SetGlobalVar("ss_follows", 0, true);
        CPH.SetGlobalVar("ss_subs", 0, true);
        CPH.SetGlobalVar("ss_raids", 0, true);
        CPH.SetGlobalVar("ss_viewer_peak", 0, true);
        CPH.SetGlobalVar("ss_viewer_sum", 0, true);
        CPH.SetGlobalVar("ss_viewer_samples", 0, true);

        string channelLogin = FirstNonEmpty(
            GetGlobal("CometenSummary_ChannelLogin", ""),
            GetGlobal("ss_channel_login", ""),
            "<TWITCH_CHANNEL>"
        );

        bool baselineOk;
        int baselineCount;
        string baselineKeys = CaptureClipKeys(channelLogin, out baselineOk, out baselineCount);

        CPH.SetGlobalVar("ss_clip_keys_before", baselineKeys, true);
        CPH.SetGlobalVar("ss_clip_baseline_ok", baselineOk, true);
        CPH.SetGlobalVar("ss_clip_baseline_count", baselineCount, true);
        CPH.SetGlobalVar("CometenSummary_LastResult", "Tracking started.", true);

        BroadcastStatus("tracking-started");
        CPH.LogInfo("[CWA Summary] Tracking started UTC " + nowUtc.ToString("o") + ". Clip baseline: " + baselineCount + ".");
        return true;
    }

    private string CaptureClipKeys(string channelLogin, out bool success, out int count)
    {
        success = false;
        count = 0;

        try
        {
            var clips = CPH.GetClipsForUser(channelLogin);
            success = true;

            if (clips == null || clips.Count == 0)
                return "";

            List<string> unique = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (var clip in clips)
            {
                string key = GetClipKey(clip);
                if (string.IsNullOrWhiteSpace(key) || ContainsIgnoreCase(unique, key))
                    continue;

                unique.Add(key);

                if (sb.Length > 0) sb.Append("\n");
                sb.Append(key.Replace("\r", "").Replace("\n", "").Trim());
                count++;
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            CPH.LogWarn("[CWA Summary] Clip baseline failed: " + ex.Message);
            return "";
        }
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
        return FirstNonEmpty(
            SafeGet(clip, "Id"), SafeGet(clip, "ID"),
            SafeGet(clip, "ClipId"), SafeGet(clip, "ClipID"),
            SafeGet(clip, "Url"), SafeGet(clip, "URL")
        );
    }

    private string SafeGet(object obj, string propertyName)
    {
        if (obj == null) return "";
        try
        {
            PropertyInfo property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            object value = property == null ? null : property.GetValue(obj, null);
            return value == null ? "" : value.ToString();
        }
        catch { return ""; }
    }

    private string FirstNonEmpty(params string[] values)
    {
        foreach (string value in values)
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
        return "";
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

    private void BroadcastStatus(string action)
    {
        long startUnix;
        long.TryParse(GetGlobal("ss_start_unix", "0"), out startUnix);
        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin",
            type = "SUMMARY_SETTINGS",
            action = action,
            isLive = true,
            startUnix = startUnix,
            startLocal = startUnix > 0 ? DateTimeOffset.FromUnixTimeSeconds(startUnix).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
            durationSeconds = 0,
            durationText = "0h 0m 0s",
            chatMessages = 0,
            follows = 0,
            subs = 0,
            raids = 0,
            peakViewers = 0,
            averageViewers = 0,
            viewerSamples = 0,
            currentStreamClipCount = 0,
            lastResult = "Tracking started."
        });
        CPH.WebsocketBroadcastJson(json);
    }
}
