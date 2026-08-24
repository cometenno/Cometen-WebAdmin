using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        LogIncomingArgs();

        int currentCount = GetCheersCount();
        int newCount = currentCount;

        string countText = GetFirstArgString("cheersCount", "count", "newCount", "value");
        if (!string.IsNullOrWhiteSpace(countText))
        {
            int parsed;
            if (int.TryParse(countText, out parsed))
                newCount = parsed;
            else
                CPH.LogWarn("CWA could not parse cheers count: " + countText);
        }
        else
        {
            CPH.LogWarn("CWA did not receive a cheers count arg. Keeping current count: " + currentCount);
        }

        if (newCount < 0) newCount = 0;

        string cheersMessage = GetArgString("cheersMessage", GetGlobalString("CheersMessage", "🍻 {user} raised a glass! This is cheers number {count}!"));
        string resetMessage = GetArgString("resetMessage", GetGlobalString("CheersResetMessage", "🍻 The cheers counter has been reset!"));
        string resetAnnounce = GetArgString("resetAnnounce", GetGlobalString("CheersResetAnnounce", "false"));

        CPH.SetGlobalVar("CheersCount", newCount, true);
        CPH.SetGlobalVar("CheersMessage", cheersMessage, true);
        CPH.SetGlobalVar("CheersResetMessage", resetMessage, true);
        CPH.SetGlobalVar("CheersResetAnnounce", resetAnnounce, true);

        int savedCount = GetCheersCount();
        BroadcastCheersSettings(savedCount, "save");

        CPH.LogInfo("CWA cheers settings saved. CheersCount=" + savedCount);
        return true;
    }

    private void LogIncomingArgs()
    {
        try
        {
            foreach (var item in args)
            {
                string value = item.Value == null ? "null" : item.Value.ToString();
                CPH.LogInfo("CWA Save Settings arg: " + item.Key + " = " + value);
            }
        }
        catch (Exception ex)
        {
            CPH.LogWarn("CWA could not log args: " + ex.Message);
        }
    }

    private string GetFirstArgString(params string[] names)
    {
        foreach (string name in names)
        {
            if (args.ContainsKey(name) && args[name] != null)
                return args[name].ToString();
        }
        return "";
    }

    private string GetArgString(string name, string fallback)
    {
        if (args.ContainsKey(name) && args[name] != null)
            return args[name].ToString();

        return fallback;
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
