using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        int count = GetIntValue("CheersCount", 0);
        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin", type = "CHEERS_SETTINGS", action = "globals-reset",
            cheersCount = count,
            cheersMessage = Get("CheersMessage", "🍻 {user} raised a glass! This is cheers number {count}!"),
            resetMessage = Get("CheersResetMessage", "🍻 The cheers counter has been reset!"),
            resetAnnounce = Get("CheersResetAnnounce", "false"),
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });
        CPH.WebsocketBroadcastJson(json);
        CPH.WebsocketBroadcastString("CWA_CHEERS_SETTINGS|" + Uri.EscapeDataString(json));
        CPH.LogInfo("[CWA Set Cheers Globals] Defaults applied by visible Set Global sub-actions.");
        return true;
    }

    private string Get(string key, string fallback) { try { string v = CPH.GetGlobalVar<string>(key, true); return string.IsNullOrWhiteSpace(v) ? fallback : v; } catch { return fallback; } }
    private int GetIntValue(string key, int fallback) { try { return CPH.GetGlobalVar<int>(key, true); } catch { } int n; return int.TryParse(Get(key, ""), out n) ? n : fallback; }
}
