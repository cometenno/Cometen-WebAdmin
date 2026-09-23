using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        string alert = CleanAlert(GetArg("alertType", GetArg("alert", "follow")));
        string user = GetArg("user", GetArg("userName", "Cometen Test"));
        string amount = GetArg("amount", "25");
        string count = GetArg("count", amount);
        string viewers = GetArg("viewers", amount);
        string months = GetArg("months", amount);
        string bits = GetArg("bits", amount);
        string message = GetArg("message", "");

        string json = JsonConvert.SerializeObject(new {
            source = "CometenAlerts",
            type = "COMETEN_ALERT",
            alert = alert,
            user = user,
            name = user,
            amount = amount,
            count = count,
            viewers = viewers,
            viewerCount = viewers,
            months = months,
            bits = bits,
            message = message
        });
        CPH.WebsocketBroadcastString(json);

        string result = "Test alert sent: " + alert;
        CPH.SetGlobalVar("CometenAlerts_LastResult", result, true);
        CPH.WebsocketBroadcastString(JsonConvert.SerializeObject(new {
            source = "CometenWebAdmin",
            type = "ALERTS_RESULT",
            result = result
        }));
        return true;
    }

    private string GetArg(string name, string fallback)
    {
        try { if (args.ContainsKey(name) && args[name] != null) return args[name].ToString(); } catch { }
        return fallback;
    }

    private string CleanAlert(string value)
    {
        string v = (value ?? "follow").Trim().ToLowerInvariant().Replace("-", "_").Replace(" ", "_");
        if (v == "donation") return "charity";
        if (v == "youtube" || v == "youtube_sub" || v == "ytsub") return "yt_sub";
        return v;
    }
}
