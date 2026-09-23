using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        EnsureDefaults();
        BroadcastSettings();
        CPH.LogInfo("[Cometen WebAdmin] Chat settings/status sent.");
        return true;
    }

    private void BroadcastSettings()
    {
        string ignoreUsersRaw = GetGlobal("CometenChat_IgnoreUsers", "nightbot,streamelements,streamlabs,moobot,wizebot");

        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin",
            type = "CHAT_SETTINGS",
            displayTimeMs = GetInt("CometenChat_DisplayTimeMs", 300000),
            maxMessages = GetInt("CometenChat_MaxMessages", 7),
            fadeTimeMs = GetInt("CometenChat_FadeTimeMs", 450),
            nameSize = GetInt("CometenChat_NameSize", 50),
            textSize = GetInt("CometenChat_TextSize", 56),
            badgeSize = GetInt("CometenChat_BadgeSize", 28),
            cardRadius = GetInt("CometenChat_CardRadius", 14),
            cardPaddingY = GetInt("CometenChat_CardPaddingY", 14),
            cardPaddingX = GetInt("CometenChat_CardPaddingX", 16),
            messageGap = GetInt("CometenChat_MessageGap", 12),
            background = GetGlobal("CometenChat_Background", "rgba(10,10,12,0.30)"),
            cardBackground = GetGlobal("CometenChat_CardBackground", "rgba(18,18,22,0.86)"),
            borderColor = GetGlobal("CometenChat_BorderColor", "rgba(255,176,0,0.50)"),
            nameColor = GetGlobal("CometenChat_NameColor", "#ffb000"),
            textColor = GetGlobal("CometenChat_TextColor", "#f4f4f4"),
            broadcasterColor = GetGlobal("CometenChat_BroadcasterColor", "#ffb000"),
            modColor = GetGlobal("CometenChat_ModColor", "#33d17a"),
            vipColor = GetGlobal("CometenChat_VipColor", "#d783ff"),
            subColor = GetGlobal("CometenChat_SubColor", "#66c2ff"),
            shadowColor = GetGlobal("CometenChat_ShadowColor", "rgba(0,0,0,0.55)"),
            ignoreCommands = GetBool("CometenChat_IgnoreCommands", true),
            commandPrefix = GetGlobal("CometenChat_CommandPrefix", "!"),
            ignoreUsers = ignoreUsersRaw,
            ignoreUsersText = ignoreUsersRaw
        });

        CPH.WebsocketBroadcastJson(json);
    }

    private void EnsureDefaults()
    {
        SetIfMissing("CometenChat_DisplayTimeMs", "300000");
        SetIfMissing("CometenChat_MaxMessages", "7");
        SetIfMissing("CometenChat_FadeTimeMs", "450");
        SetIfMissing("CometenChat_NameSize", "50");
        SetIfMissing("CometenChat_TextSize", "56");
        SetIfMissing("CometenChat_BadgeSize", "28");
        SetIfMissing("CometenChat_CardRadius", "14");
        SetIfMissing("CometenChat_CardPaddingY", "14");
        SetIfMissing("CometenChat_CardPaddingX", "16");
        SetIfMissing("CometenChat_MessageGap", "12");
        SetIfMissing("CometenChat_Background", "rgba(10,10,12,0.30)");
        SetIfMissing("CometenChat_CardBackground", "rgba(18,18,22,0.86)");
        SetIfMissing("CometenChat_BorderColor", "rgba(255,176,0,0.50)");
        SetIfMissing("CometenChat_NameColor", "#ffb000");
        SetIfMissing("CometenChat_TextColor", "#f4f4f4");
        SetIfMissing("CometenChat_BroadcasterColor", "#ffb000");
        SetIfMissing("CometenChat_ModColor", "#33d17a");
        SetIfMissing("CometenChat_VipColor", "#d783ff");
        SetIfMissing("CometenChat_SubColor", "#66c2ff");
        SetIfMissing("CometenChat_ShadowColor", "rgba(0,0,0,0.55)");
        SetIfMissing("CometenChat_IgnoreCommands", "True");
        SetIfMissing("CometenChat_CommandPrefix", "!");
        SetIfMissing("CometenChat_IgnoreUsers", "nightbot,streamelements,streamlabs,moobot,wizebot");
    }

    private void SetIfMissing(string key, string value)
    {
        string current = GetGlobal(key, "");
        if (string.IsNullOrWhiteSpace(current)) CPH.SetGlobalVar(key, value, true);
    }

    private string GetGlobal(string key, string fallback)
    {
        try
        {
            string value = CPH.GetGlobalVar<string>(key, true);
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }
        catch { }
        return fallback;
    }

    private int GetInt(string key, int fallback)
    {
        int result;
        if (int.TryParse(GetGlobal(key, ""), out result)) return result;
        return fallback;
    }

    private bool GetBool(string key, bool fallback)
    {
        string value = GetGlobal(key, "").Trim().ToLowerInvariant();
        if (value == "true" || value == "yes" || value == "1") return true;
        if (value == "false" || value == "no" || value == "0") return false;
        return fallback;
    }
}
