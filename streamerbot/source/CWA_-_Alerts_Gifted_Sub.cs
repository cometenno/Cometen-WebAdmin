// Bruk på Twitch Gift Sub trigger.

using System;

public class CPHInline
{
    public bool Execute()
    {
        // Bytt denne linjen for hver action:
        // follow, raid, sub, resub, gift_sub, community_gift, bits,
        // channel_points, hype_train_start, hype_train_level, hype_train_end,
        // shoutout, charity, goal, custom
        string alert = "gift_sub";

        string user = GetArg("userName", "displayName", "fromUser", "fromUserName", "targetUser", "name", "login");
        string amount = GetArg("viewerCount", "viewers", "raidViewerCount", "raiderViewerCount", "bits", "giftCount", "communitySubGiftCount", "amount", "count", "total");
        string months = GetArg("cumulativeMonths", "months", "tenure", "durationMonths");
        string tier = GetArg("tier", "subTier");
        string reward = GetArg("rewardName", "reward", "title");
        string message = GetArg("message", "userInput", "rawInput", "text");
        string level = GetArg("level", "hypeTrainLevel");
        string goal = GetArg("goal", "goalName", "title");

        if (string.IsNullOrWhiteSpace(user))
            user = "Unknown";

        string json =
            "{"
            + "\"source\":\"CometenAlerts\","
            + "\"type\":\"COMETEN_ALERT\","
            + "\"alert\":\"" + EscapeJson(alert) + "\","
            + "\"user\":\"" + EscapeJson(user) + "\","
            + "\"name\":\"" + EscapeJson(user) + "\","
            + "\"amount\":\"" + EscapeJson(amount) + "\","
            + "\"viewers\":\"" + EscapeJson(amount) + "\","
            + "\"viewerCount\":\"" + EscapeJson(amount) + "\","
            + "\"count\":\"" + EscapeJson(amount) + "\","
            + "\"months\":\"" + EscapeJson(months) + "\","
            + "\"tier\":\"" + EscapeJson(tier) + "\","
            + "\"reward\":\"" + EscapeJson(reward) + "\","
            + "\"message\":\"" + EscapeJson(message) + "\","
            + "\"level\":\"" + EscapeJson(level) + "\","
            + "\"goal\":\"" + EscapeJson(goal) + "\""
            + "}";

        CPH.LogInfo("COMETEN ALERT JSON: " + json);
        CPH.WebsocketBroadcastString(json);

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
