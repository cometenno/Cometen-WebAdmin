using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        SetIfArg("topTitle", "CometenCredits_TopTitle");
        SetIfArg("topText", "CometenCredits_TopText");
        SetIfArg("bottomTitle", "CometenCredits_BottomTitle");
        SetIfArg("bottomText", "CometenCredits_BottomText");

        SetIfArg("colorGold", "CometenCredits_ColorGold");
        SetIfArg("colorGoldLight", "CometenCredits_ColorGoldLight");
        SetIfArg("colorText", "CometenCredits_ColorText");
        SetIfArg("panelBg", "CometenCredits_PanelBg");

        SetNumberIfArg("titleSize", "CometenCredits_TitleSize", 1);
        SetNumberIfArg("topTextSize", "CometenCredits_TopTextSize", 1);
        SetNumberIfArg("sectionTitleSize", "CometenCredits_SectionTitleSize", 1);
        SetNumberIfArg("nameSize", "CometenCredits_NameSize", 1);
        SetNumberIfArg("smallNameSize", "CometenCredits_SmallNameSize", 1);
        SetNumberIfArg("bottomTitleSize", "CometenCredits_BottomTitleSize", 1);
        SetNumberIfArg("bottomTextSize", "CometenCredits_BottomTextSize", 1);

        SetNumberIfArg("scrollSpeed", "CometenCredits_ScrollSpeed", 1);
        SetNumberIfArg("frameLeft", "CometenCredits_FrameLeft", -99999);
        SetNumberIfArg("frameTop", "CometenCredits_FrameTop", -99999);
        SetNumberIfArg("frameWidth", "CometenCredits_FrameWidth", 1);
        SetNumberIfArg("frameHeight", "CometenCredits_FrameHeight", 1);
        SetNumberIfArg("viewportLeft", "CometenCredits_ViewportLeft", -99999);
        SetNumberIfArg("viewportWidth", "CometenCredits_ViewportWidth", 1);
        SetNumberIfArg("sectionGap", "CometenCredits_SectionGap", 0);

        BroadcastCreditsSettings("save");
        CPH.LogInfo("[CWA Credits] Settings saved from WebAdmin.");
        return true;
    }

    private void SetIfArg(string argName, string globalName)
    {
        string value;
        if (CPH.TryGetArg(argName, out value) && value != null)
        {
            CPH.SetGlobalVar(globalName, value, true);
            CPH.LogInfo("[CWA Credits] Set " + globalName + " = " + value);
        }
    }

    private void SetNumberIfArg(string argName, string globalName, int minValue)
    {
        string raw;
        if (!CPH.TryGetArg(argName, out raw) || raw == null)
            return;

        int value;
        if (!int.TryParse(raw, out value))
            return;

        if (value < minValue)
            value = minValue;

        CPH.SetGlobalVar(globalName, value.ToString(), true);
        CPH.LogInfo("[CWA Credits] Set " + globalName + " = " + value);
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

    private void BroadcastCreditsSettings(string action)
    {
        var data = new
        {
            source = "CometenWebAdmin",
            type = "CREDITS_SETTINGS",
            action = action,
            topTitle = GetGlobal("CometenCredits_TopTitle", "END OF STREAM CREDITS"),
            topText = GetGlobal("CometenCredits_TopText", "Thank you for hanging out in the Cometen universe"),
            bottomTitle = GetGlobal("CometenCredits_BottomTitle", "SEE YOU NEXT STREAM!"),
            bottomText = GetGlobal("CometenCredits_BottomText", "Twitch: <TWITCH_CHANNEL> - X: <X_HANDLE> - YouTube: <YOUTUBE_HANDLE>"),
            colorGold = GetGlobal("CometenCredits_ColorGold", "#f0a900"),
            colorGoldLight = GetGlobal("CometenCredits_ColorGoldLight", "#ffd56a"),
            colorText = GetGlobal("CometenCredits_ColorText", "#f5e7bd"),
            panelBg = GetGlobal("CometenCredits_PanelBg", "rgba(0,0,0,0.52)"),
            titleSize = GetGlobal("CometenCredits_TitleSize", "90"),
            topTextSize = GetGlobal("CometenCredits_TopTextSize", "28"),
            sectionTitleSize = GetGlobal("CometenCredits_SectionTitleSize", "46"),
            nameSize = GetGlobal("CometenCredits_NameSize", "38"),
            smallNameSize = GetGlobal("CometenCredits_SmallNameSize", "30"),
            bottomTitleSize = GetGlobal("CometenCredits_BottomTitleSize", "52"),
            bottomTextSize = GetGlobal("CometenCredits_BottomTextSize", "26"),
            scrollSpeed = GetGlobal("CometenCredits_ScrollSpeed", "120"),
            frameLeft = GetGlobal("CometenCredits_FrameLeft", "250"),
            frameTop = GetGlobal("CometenCredits_FrameTop", "70"),
            frameWidth = GetGlobal("CometenCredits_FrameWidth", "1420"),
            frameHeight = GetGlobal("CometenCredits_FrameHeight", "940"),
            viewportLeft = GetGlobal("CometenCredits_ViewportLeft", "300"),
            viewportWidth = GetGlobal("CometenCredits_ViewportWidth", "1320"),
            sectionGap = GetGlobal("CometenCredits_SectionGap", "74")
        };

        string json = JsonConvert.SerializeObject(data);
        string markerPayload = "CWA_CREDITS_SETTINGS|" + Uri.EscapeDataString(json);

        CPH.WebsocketBroadcastJson(json);
        CPH.WebsocketBroadcastString(markerPayload);
    }
}
