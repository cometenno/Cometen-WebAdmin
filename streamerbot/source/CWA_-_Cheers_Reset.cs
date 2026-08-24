using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        CPH.SetGlobalVar("CheersCount", 0, true);

        string resetMessage = GetGlobalString("CheersResetMessage", "🍻 The cheers counter has been reset!");
        string announce = GetGlobalString("CheersResetAnnounce", "false");

        if (IsTrue(announce))
            CPH.SendMessage(resetMessage);

        BroadcastCheersSettings(0, "reset");
        CPH.LogInfo("CWA cheers counter reset to 0.");
        return true;
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

    private bool IsTrue(string value)
    {
        value = (value ?? "").Trim().ToLowerInvariant();
        return value == "true" || value == "yes" || value == "1";
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
