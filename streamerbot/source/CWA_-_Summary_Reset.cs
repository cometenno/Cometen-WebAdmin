using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        CPH.SetGlobalVar("ss_is_live", false, true);
        CPH.SetGlobalVar("ss_start_unix", "0", true);
        CPH.SetGlobalVar("ss_start_utc", "", true);
        CPH.SetGlobalVar("ss_chat_messages", 0, true);
        CPH.SetGlobalVar("ss_follows", 0, true);
        CPH.SetGlobalVar("ss_subs", 0, true);
        CPH.SetGlobalVar("ss_raids", 0, true);
        CPH.SetGlobalVar("ss_viewer_peak", 0, true);
        CPH.SetGlobalVar("ss_viewer_sum", 0, true);
        CPH.SetGlobalVar("ss_viewer_samples", 0, true);
        CPH.SetGlobalVar("ss_clip_keys_before", "", true);
        CPH.SetGlobalVar("ss_clip_baseline_ok", false, true);
        CPH.SetGlobalVar("ss_clip_baseline_count", 0, true);
        CPH.SetGlobalVar("CometenSummary_LastResult", "Active tracking reset.", true);

        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin", type = "SUMMARY_SETTINGS", action = "reset",
            isLive = false, startUnix = 0, startLocal = "", durationSeconds = 0, durationText = "0h 0m 0s",
            chatMessages = 0, follows = 0, subs = 0, raids = 0, peakViewers = 0, averageViewers = 0,
            viewerSamples = 0, currentStreamClipCount = 0, lastResult = "Active tracking reset."
        });
        CPH.WebsocketBroadcastJson(json);
        CPH.LogInfo("[CWA Summary] Active tracking reset.");
        return true;
    }
}
