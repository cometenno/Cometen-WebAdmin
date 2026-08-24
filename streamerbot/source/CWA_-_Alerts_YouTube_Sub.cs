using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        string user = GetArg("userName", GetArg("displayName", GetArg("user", GetArg("name", "YouTube Viewer"))));
        string message = GetArg("message", GetArg("text", ""));
        string json = JsonConvert.SerializeObject(new {
            source = "CometenAlerts",
            type = "COMETEN_ALERT",
            alert = "yt_sub",
            user = user,
            name = user,
            message = message
        });
        CPH.WebsocketBroadcastString(json);
        return true;
    }

    private string GetArg(string name, string fallback)
    {
        try { if (args.ContainsKey(name) && args[name] != null && !string.IsNullOrWhiteSpace(args[name].ToString())) return args[name].ToString(); } catch { }
        return fallback;
    }
}
