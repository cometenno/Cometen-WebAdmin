using System;

public class CPHInline
{
    public bool Execute()
    {
        string user = GetArg("userName", "displayName", "fromUser", "fromUserName", "targetUser", "name");

        if (string.IsNullOrWhiteSpace(user))
            user = "Unknown";

        string json =
            "{"
            + "\"source\":\"CometenAlerts\","
            + "\"type\":\"COMETEN_ALERT\","
            + "\"alert\":\"follow\","
            + "\"user\":\"" + EscapeJson(user) + "\","
            + "\"name\":\"" + EscapeJson(user) + "\""
            + "}";

        CPH.LogInfo("COMETEN FOLLOW ALERT: " + json);
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