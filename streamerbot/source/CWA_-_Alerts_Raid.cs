using System;
using System.Threading;

public class CPHInline
{
    public bool Execute()
    {
        string user = GetArg("userName", "displayName", "fromUser", "fromUserName", "raider", "targetUser", "name", "login");
        string viewers = GetArg("viewerCount", "viewers", "raidViewerCount", "raiderViewerCount", "viewer_count", "amount", "count");

        if (string.IsNullOrWhiteSpace(user))
            user = "Unknown";

        if (string.IsNullOrWhiteSpace(viewers))
            viewers = "0";

        // 1. Raid alert til universal_twitch_alerts.html
        string raidJson =
            "{"
            + "\"source\":\"CometenAlerts\","
            + "\"type\":\"COMETEN_ALERT\","
            + "\"alert\":\"raid\","
            + "\"user\":\"" + EscapeJson(user) + "\","
            + "\"name\":\"" + EscapeJson(user) + "\","
            + "\"amount\":\"" + EscapeJson(viewers) + "\","
            + "\"viewers\":\"" + EscapeJson(viewers) + "\","
            + "\"viewerCount\":\"" + EscapeJson(viewers) + "\","
            + "\"count\":\"" + EscapeJson(viewers) + "\""
            + "}";

        CPH.WebsocketBroadcastString(raidJson);
        CPH.LogInfo("COMETEN RAID ALERT: " + raidJson);

        Thread.Sleep(1500);

        // 2. Twitch sin ekte shoutout
        try
        {
            bool soOk = CPH.TwitchSendShoutoutByLogin(user);
            CPH.LogInfo("COMETEN TWITCH SHOUTOUT til " + user + ": " + soOk);
        }
        catch (Exception ex)
        {
            CPH.LogWarn("COMETEN TWITCH SHOUTOUT FEIL: " + ex.Message);
        }

        Thread.Sleep(1500);

        // 3. Video shoutout til din video SO overlay
        string videoJson =
            "{"
            + "\"type\":\"VIDEO_SHOUTOUT\","
            + "\"user\":\"" + EscapeJson(user) + "\","
            + "\"targetUser\":\"" + EscapeJson(user) + "\","
            + "\"name\":\"" + EscapeJson(user) + "\""
            + "}";

        CPH.WebsocketBroadcastString(videoJson);
        CPH.LogInfo("COMETEN VIDEO SHOUTOUT: " + videoJson);

        return true;
    }

    private string GetArg(params string[] keys)
    {
        foreach (string key in keys)
        {
            if (args.ContainsKey(key) && args[key] != null)
            {
                string value = args[key].ToString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
        }

        return "";
    }

    private string EscapeJson(string value)
    {
        if (value == null)
            return "";

        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }
}
