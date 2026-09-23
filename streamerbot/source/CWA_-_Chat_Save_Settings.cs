using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        SaveInt("CometenChat_DisplayTimeMs", "displayTimeMs", 300000);
        SaveInt("CometenChat_MaxMessages", "maxMessages", 7);
        SaveInt("CometenChat_FadeTimeMs", "fadeTimeMs", 450);

        SaveInt("CometenChat_NameSize", "nameSize", 50);
        SaveInt("CometenChat_TextSize", "textSize", 56);
        SaveInt("CometenChat_BadgeSize", "badgeSize", 28);
        SaveInt("CometenChat_CardRadius", "cardRadius", 14);
        SaveInt("CometenChat_CardPaddingY", "cardPaddingY", 14);
        SaveInt("CometenChat_CardPaddingX", "cardPaddingX", 16);
        SaveInt("CometenChat_MessageGap", "messageGap", 12);

        SaveString("CometenChat_Background", "background", "rgba(10,10,12,0.30)");
        SaveString("CometenChat_CardBackground", "cardBackground", "rgba(18,18,22,0.86)");
        SaveString("CometenChat_BorderColor", "borderColor", "rgba(255,176,0,0.50)");
        SaveString("CometenChat_NameColor", "nameColor", "#ffb000");
        SaveString("CometenChat_TextColor", "textColor", "#f4f4f4");
        SaveString("CometenChat_BroadcasterColor", "broadcasterColor", "#ffb000");
        SaveString("CometenChat_ModColor", "modColor", "#33d17a");
        SaveString("CometenChat_VipColor", "vipColor", "#d783ff");
        SaveString("CometenChat_SubColor", "subColor", "#66c2ff");
        SaveString("CometenChat_ShadowColor", "shadowColor", "rgba(0,0,0,0.55)");

        SaveBoolString("CometenChat_IgnoreCommands", "ignoreCommands", true);
        SaveString("CometenChat_CommandPrefix", "commandPrefix", "!");
        SaveString("CometenChat_IgnoreUsers", "ignoreUsers", "nightbot,streamelements,streamlabs,moobot,wizebot");

        BroadcastOverlayConfig();
        BroadcastAdminSettings();

        CPH.LogInfo("[Cometen WebAdmin] Chat settings saved and config sent to overlay.");
        return true;
    }

    private void SaveInt(string globalName, string argName, int fallback)
    {
        string raw = GetArg(argName, "");
        int value;
        if (!int.TryParse(raw, out value)) value = fallback;
        if (value < 0) value = 0;
        CPH.SetGlobalVar(globalName, value.ToString(), true);
        CPH.LogInfo("Set global '" + globalName + "' to '" + value + "'");
    }

    private void SaveString(string globalName, string argName, string fallback)
    {
        string value = GetArg(argName, fallback);
        if (value == null) value = fallback;
        CPH.SetGlobalVar(globalName, value, true);
        CPH.LogInfo("Set global '" + globalName + "' to '" + value + "'");
    }

    private void SaveBoolString(string globalName, string argName, bool fallback)
    {
        string raw = GetArg(argName, fallback ? "true" : "false").Trim().ToLowerInvariant();
        string value = (raw == "true" || raw == "yes" || raw == "1") ? "True" : "False";
        CPH.SetGlobalVar(globalName, value, true);
        CPH.LogInfo("Set global '" + globalName + "' to '" + value + "'");
    }

    private string GetArg(string key, string fallback)
    {
        try
        {
            if (args.ContainsKey(key) && args[key] != null) return args[key].ToString();
        }
        catch { }
        return fallback;
    }

    private void BroadcastAdminSettings()
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

    private void BroadcastOverlayConfig()
    {
        string ignoreUsersRaw = GetGlobal("CometenChat_IgnoreUsers", "nightbot,streamelements,streamlabs,moobot,wizebot");
        string[] ignoreUsers = SplitCsv(ignoreUsersRaw);

        string json = JsonConvert.SerializeObject(new
        {
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
            ignoreUsers = ignoreUsers
        });

        string payload = "COMETEN_CHAT_CONFIG_V6|" + Uri.EscapeDataString(json);
        CPH.WebsocketBroadcastString(payload);
        CPH.LogInfo("[Cometen Chat V6] Config sent to overlay: " + json);
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

    private string[] SplitCsv(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new string[0];
        string[] raw = text.Split(',');
        List<string> result = new List<string>();
        foreach (string item in raw)
        {
            string clean = item.Trim().TrimStart('@').ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(clean)) result.Add(clean);
        }
        return result.ToArray();
    }
}
