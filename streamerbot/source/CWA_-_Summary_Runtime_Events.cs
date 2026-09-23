using System;
using System.Globalization;

// Cometen WebAdmin - Stream Summary Runtime Events v1.0
//
// This action records live Twitch runtime events in the ss_* globals that the
// existing Summary Status and Summary Send actions already read.
//
// Required triggers on THIS action:
// - Twitch -> Chat -> Message
// - Twitch -> Channel -> Follow
// - Twitch -> Subscriptions -> Subscription
// - Twitch -> Subscriptions -> Resubscription
// - Twitch -> Subscriptions -> Gift Subscription
// - Twitch -> Raid -> Raid
// - Twitch -> Channel -> Viewer Count Update (Any Value)
//
// Do NOT add Gift Bomb. Each recipient is already counted by Gift Subscription.

public class CPHInline
{
    public bool Execute()
    {
        string eventType = GetEventTypeName();

        // Only record events between Summary Stream Started and Summary Send/Reset.
        if (!GetBoolGlobal("ss_is_live", false))
        {
            return true;
        }

        string normalized = Normalize(eventType);

        if (normalized == "twitchchatmessage" || normalized == "twitchmessage")
        {
            // Do not count messages sent internally by Streamer.bot itself.
            if (!GetBoolArg("isInternal", false))
            {
                Increment("ss_chat_messages", 1);
                MarkLastEvent(eventType);
            }
            return true;
        }

        if (normalized == "twitchfollow")
        {
            Increment("ss_follows", 1);
            MarkLastEvent(eventType);
            CPH.LogInfo("[CWA Summary Runtime] Follow recorded.");
            return true;
        }

        if (normalized == "twitchsub" ||
            normalized == "twitchsubscription" ||
            normalized == "twitchresub" ||
            normalized == "twitchresubscription" ||
            normalized == "twitchgiftsub" ||
            normalized == "twitchgiftsubscription")
        {
            Increment("ss_subs", 1);
            MarkLastEvent(eventType);
            CPH.LogInfo("[CWA Summary Runtime] Subscription recorded: " + eventType + ".");
            return true;
        }

        // Gift Bomb must not increment here. Gift Subscription fires once per recipient.
        if (normalized == "twitchgiftbomb")
        {
            CPH.LogInfo("[CWA Summary Runtime] Gift Bomb ignored to prevent double counting.");
            return true;
        }

        if (normalized == "twitchraid")
        {
            Increment("ss_raids", 1);
            MarkLastEvent(eventType);
            CPH.LogInfo("[CWA Summary Runtime] Raid recorded.");
            return true;
        }

        if (normalized == "twitchviewercountupdate" || normalized == "twitchviewersupdate")
        {
            int viewerCount;
            if (!TryGetIntArg(out viewerCount, "viewerCount", "viewers", "count"))
            {
                CPH.LogWarn("[CWA Summary Runtime] Viewer Count Update had no readable viewerCount argument.");
                return true;
            }

            if (viewerCount < 0)
            {
                return true;
            }

            int currentPeak = GetIntGlobal("ss_viewer_peak", 0);
            if (viewerCount > currentPeak)
            {
                CPH.SetGlobalVar("ss_viewer_peak", viewerCount, true);
            }

            int currentSum = GetIntGlobal("ss_viewer_sum", 0);
            int currentSamples = GetIntGlobal("ss_viewer_samples", 0);

            long nextSum = (long)currentSum + viewerCount;
            if (nextSum > int.MaxValue) nextSum = int.MaxValue;

            CPH.SetGlobalVar("ss_viewer_sum", (int)nextSum, true);
            CPH.SetGlobalVar("ss_viewer_samples", currentSamples < int.MaxValue ? currentSamples + 1 : int.MaxValue, true);
            CPH.SetGlobalVar("ss_last_viewer_count", viewerCount, true);
            MarkLastEvent(eventType);
            return true;
        }

        CPH.LogWarn("[CWA Summary Runtime] Unhandled trigger type: " + eventType + ".");
        return true;
    }

    private string GetEventTypeName()
    {
        try
        {
            object eventType = CPH.GetEventType();
            if (eventType != null)
            {
                string value = eventType.ToString();
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }
        }
        catch { }

        return GetArgString("__source", GetArgString("eventType", "Unknown"));
    }

    private string Normalize(string value)
    {
        return (value ?? "")
            .Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("_", "")
            .ToLowerInvariant();
    }

    private void Increment(string key, int amount)
    {
        int current = GetIntGlobal(key, 0);
        long next = (long)current + amount;
        if (next < 0) next = 0;
        if (next > int.MaxValue) next = int.MaxValue;
        CPH.SetGlobalVar(key, (int)next, true);
    }

    private void MarkLastEvent(string eventType)
    {
        CPH.SetGlobalVar("ss_last_runtime_event", eventType ?? "", true);
        CPH.SetGlobalVar("ss_last_runtime_unix", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), true);
    }

    private bool GetBoolGlobal(string key, bool fallback)
    {
        try
        {
            return CPH.GetGlobalVar<bool>(key, true);
        }
        catch { }

        try
        {
            object raw = CPH.GetGlobalVar<object>(key, true);
            if (raw == null) return fallback;
            bool value;
            if (bool.TryParse(raw.ToString(), out value)) return value;
            int number;
            if (int.TryParse(raw.ToString(), out number)) return number != 0;
        }
        catch { }

        return fallback;
    }

    private int GetIntGlobal(string key, int fallback)
    {
        try
        {
            return CPH.GetGlobalVar<int>(key, true);
        }
        catch { }

        try
        {
            object raw = CPH.GetGlobalVar<object>(key, true);
            if (raw == null) return fallback;
            int value;
            if (int.TryParse(raw.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) return value;
        }
        catch { }

        return fallback;
    }

    private bool GetBoolArg(string key, bool fallback)
    {
        try
        {
            if (args.ContainsKey(key) && args[key] != null)
            {
                bool value;
                if (bool.TryParse(args[key].ToString(), out value)) return value;
                int number;
                if (int.TryParse(args[key].ToString(), out number)) return number != 0;
            }
        }
        catch { }

        return fallback;
    }

    private string GetArgString(string key, string fallback)
    {
        try
        {
            if (args.ContainsKey(key) && args[key] != null)
            {
                string value = args[key].ToString();
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }
        }
        catch { }

        return fallback;
    }

    private bool TryGetIntArg(out int value, params string[] names)
    {
        value = 0;

        foreach (string name in names)
        {
            try
            {
                if (!args.ContainsKey(name) || args[name] == null) continue;

                object raw = args[name];
                if (raw is int)
                {
                    value = (int)raw;
                    return true;
                }

                if (int.TryParse(raw.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                {
                    return true;
                }
            }
            catch { }
        }

        return false;
    }
}
