using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        int cheersCount = GetCheersCount();
        cheersCount++;
        CPH.SetGlobalVar("CheersCount", cheersCount, true);

        string user = args.ContainsKey("userName") && args["userName"] != null
            ? args["userName"].ToString()
            : "Someone";

        string cheersMessage = GetGlobalString("CheersMessage", "🍻 {user} raised a glass! This is cheers number {count}!");
        CPH.SendMessage(FormatMessage(cheersMessage, user, cheersCount));

        BroadcastCheersSettings(cheersCount, "increment");
        return true;
    }

    private int GetCheersCount()
    {
        try { return CPH.GetGlobalVar<int>("CheersCount", true); } catch {}

        try
        {
            string text = CPH.GetGlobalVar<string>("CheersCount", true);
            int parsed;
            if (int.TryParse(text, out parsed)) return parsed;
        }
        catch {}

        CPH.SetGlobalVar("CheersCount", 0, true);
        return 0;
    }

    private string GetGlobalString(string name, string fallback)
    {
        try
        {
            string value = CPH.GetGlobalVar<string>(name, true);
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }
        catch {}
        return fallback;
    }

    private string FormatMessage(string message, string user, int count)
    {
        return message
            .Replace("{user}", user)
            .Replace("{count}", count.ToString());
    }

    private void BroadcastCheersSettings(int count, string action)
    {
        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin",
            type = "CHEERS_SETTINGS",
            action = action,
            cheersCount = count,
            cheersMessage = GetGlobalString("CheersMessage", "🍻 {user} raised a glass! This is cheers number {count}!"),
            resetMessage = GetGlobalString("CheersResetMessage", "🍻 The cheers counter has been reset!"),
            resetAnnounce = GetGlobalString("CheersResetAnnounce", "false"),
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });

        CPH.WebsocketBroadcastJson(json);
        CPH.WebsocketBroadcastString("CWA_CHEERS_SETTINGS|" + Uri.EscapeDataString(json));
    }
}
