using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        BroadcastOverlayConfig();
        BroadcastAdminSettings();
        CPH.LogInfo("[CWA Set Chat Globals] Defaults applied by visible Set Global sub-actions and sent to WebAdmin/overlay.");
        return true;
    }

    private void BroadcastAdminSettings()
    {
        string ignoreUsersRaw = Get("CometenChat_IgnoreUsers", "nightbot,streamelements,streamlabs,moobot,wizebot");
        CPH.WebsocketBroadcastJson(JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin", type = "CHAT_SETTINGS", action = "globals-reset",
            displayTimeMs = GetInt("CometenChat_DisplayTimeMs", 300000), maxMessages = GetInt("CometenChat_MaxMessages", 7), fadeTimeMs = GetInt("CometenChat_FadeTimeMs", 450),
            nameSize = GetInt("CometenChat_NameSize", 50), textSize = GetInt("CometenChat_TextSize", 56), badgeSize = GetInt("CometenChat_BadgeSize", 28),
            cardRadius = GetInt("CometenChat_CardRadius", 14), cardPaddingY = GetInt("CometenChat_CardPaddingY", 14), cardPaddingX = GetInt("CometenChat_CardPaddingX", 16), messageGap = GetInt("CometenChat_MessageGap", 12),
            background = Get("CometenChat_Background", "rgba(10,10,12,0.30)"), cardBackground = Get("CometenChat_CardBackground", "rgba(18,18,22,0.86)"), borderColor = Get("CometenChat_BorderColor", "rgba(255,176,0,0.50)"),
            nameColor = Get("CometenChat_NameColor", "#ffb000"), textColor = Get("CometenChat_TextColor", "#f4f4f4"), broadcasterColor = Get("CometenChat_BroadcasterColor", "#ffb000"), modColor = Get("CometenChat_ModColor", "#33d17a"), vipColor = Get("CometenChat_VipColor", "#d783ff"), subColor = Get("CometenChat_SubColor", "#66c2ff"), shadowColor = Get("CometenChat_ShadowColor", "rgba(0,0,0,0.55)"),
            ignoreCommands = GetBool("CometenChat_IgnoreCommands", true), commandPrefix = Get("CometenChat_CommandPrefix", "!"), ignoreUsers = ignoreUsersRaw, ignoreUsersText = ignoreUsersRaw
        }));
    }

    private void BroadcastOverlayConfig()
    {
        string ignoreUsersRaw = Get("CometenChat_IgnoreUsers", "nightbot,streamelements,streamlabs,moobot,wizebot");
        string json = JsonConvert.SerializeObject(new
        {
            displayTimeMs = GetInt("CometenChat_DisplayTimeMs", 300000), maxMessages = GetInt("CometenChat_MaxMessages", 7), fadeTimeMs = GetInt("CometenChat_FadeTimeMs", 450),
            nameSize = GetInt("CometenChat_NameSize", 50), textSize = GetInt("CometenChat_TextSize", 56), badgeSize = GetInt("CometenChat_BadgeSize", 28), cardRadius = GetInt("CometenChat_CardRadius", 14), cardPaddingY = GetInt("CometenChat_CardPaddingY", 14), cardPaddingX = GetInt("CometenChat_CardPaddingX", 16), messageGap = GetInt("CometenChat_MessageGap", 12),
            background = Get("CometenChat_Background", "rgba(10,10,12,0.30)"), cardBackground = Get("CometenChat_CardBackground", "rgba(18,18,22,0.86)"), borderColor = Get("CometenChat_BorderColor", "rgba(255,176,0,0.50)"), nameColor = Get("CometenChat_NameColor", "#ffb000"), textColor = Get("CometenChat_TextColor", "#f4f4f4"), broadcasterColor = Get("CometenChat_BroadcasterColor", "#ffb000"), modColor = Get("CometenChat_ModColor", "#33d17a"), vipColor = Get("CometenChat_VipColor", "#d783ff"), subColor = Get("CometenChat_SubColor", "#66c2ff"), shadowColor = Get("CometenChat_ShadowColor", "rgba(0,0,0,0.55)"),
            ignoreCommands = GetBool("CometenChat_IgnoreCommands", true), commandPrefix = Get("CometenChat_CommandPrefix", "!"), ignoreUsers = SplitCsv(ignoreUsersRaw)
        });
        CPH.WebsocketBroadcastString("COMETEN_CHAT_CONFIG_V6|" + Uri.EscapeDataString(json));
    }

    private string Get(string key, string fallback) { try { string v = CPH.GetGlobalVar<string>(key, true); return string.IsNullOrWhiteSpace(v) ? fallback : v; } catch { return fallback; } }
    private int GetInt(string key, int fallback) { int n; return int.TryParse(Get(key, ""), out n) ? n : fallback; }
    private bool GetBool(string key, bool fallback) { string v = Get(key, "").ToLowerInvariant(); if (v == "true" || v == "1" || v == "yes") return true; if (v == "false" || v == "0" || v == "no") return false; return fallback; }
    private string[] SplitCsv(string text) { List<string> result = new List<string>(); foreach (string item in (text ?? "").Split(',')) { string clean = item.Trim().TrimStart('@').ToLowerInvariant(); if (clean != "") result.Add(clean); } return result.ToArray(); }
}
