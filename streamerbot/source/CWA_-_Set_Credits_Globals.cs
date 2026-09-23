using System;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        BroadcastAdmin();
        SendOverlaySettings();
        CPH.LogInfo("[CWA Set Credits Globals] Defaults applied by visible Set Global sub-actions and sent to WebAdmin/overlay.");
        return true;
    }

    private void BroadcastAdmin()
    {
        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin", type = "CREDITS_SETTINGS", action = "globals-reset",
            topTitle = Get("CometenCredits_TopTitle"), topText = Get("CometenCredits_TopText"), bottomTitle = Get("CometenCredits_BottomTitle"), bottomText = Get("CometenCredits_BottomText"),
            colorGold = Get("CometenCredits_ColorGold"), colorGoldLight = Get("CometenCredits_ColorGoldLight"), colorText = Get("CometenCredits_ColorText"), panelBg = Get("CometenCredits_PanelBg"),
            titleSize = Get("CometenCredits_TitleSize"), topTextSize = Get("CometenCredits_TopTextSize"), sectionTitleSize = Get("CometenCredits_SectionTitleSize"), nameSize = Get("CometenCredits_NameSize"), smallNameSize = Get("CometenCredits_SmallNameSize"), bottomTitleSize = Get("CometenCredits_BottomTitleSize"), bottomTextSize = Get("CometenCredits_BottomTextSize"),
            scrollSpeed = Get("CometenCredits_ScrollSpeed"), frameLeft = Get("CometenCredits_FrameLeft"), frameTop = Get("CometenCredits_FrameTop"), frameWidth = Get("CometenCredits_FrameWidth"), frameHeight = Get("CometenCredits_FrameHeight"), viewportLeft = Get("CometenCredits_ViewportLeft"), viewportWidth = Get("CometenCredits_ViewportWidth"), sectionGap = Get("CometenCredits_SectionGap")
        });
        CPH.WebsocketBroadcastJson(json);
        CPH.WebsocketBroadcastString("CWA_CREDITS_SETTINGS|" + Uri.EscapeDataString(json));
    }

    private void SendOverlaySettings()
    {
        SendText("CometenCredits_TopTitle", "title"); SendText("CometenCredits_TopText", "subtitle"); SendText("CometenCredits_BottomTitle", "footer"); SendText("CometenCredits_BottomText", "footerSmall");
        SendText("CometenCredits_ColorGold", "gold"); SendText("CometenCredits_ColorGoldLight", "goldLight"); SendText("CometenCredits_ColorText", "textColor"); SendText("CometenCredits_PanelBg", "panelBg");
        SendNumber("CometenCredits_TitleSize", "mainTitleSize"); SendNumber("CometenCredits_TopTextSize", "subtitleSize"); SendNumber("CometenCredits_SectionTitleSize", "sectionTitleSize"); SendNumber("CometenCredits_NameSize", "namesSize"); SendNumber("CometenCredits_SmallNameSize", "smallNamesSize"); SendNumber("CometenCredits_BottomTitleSize", "footerSize"); SendNumber("CometenCredits_BottomTextSize", "footerSmallSize");
        SendNumber("CometenCredits_ScrollSpeed", "speed"); SendNumber("CometenCredits_FrameLeft", "frameLeft"); SendNumber("CometenCredits_FrameTop", "frameTop"); SendNumber("CometenCredits_FrameWidth", "frameWidth"); SendNumber("CometenCredits_FrameHeight", "frameHeight"); SendNumber("CometenCredits_ViewportLeft", "viewportLeft"); SendNumber("CometenCredits_ViewportWidth", "viewportWidth"); SendNumber("CometenCredits_SectionGap", "sectionGap");
    }

    private void SendText(string key, string field) { CPH.WebsocketBroadcastString("{\"type\":\"COMETEN_CREDITS_SETTINGS\",\"field\":\"" + Escape(field) + "\",\"value\":\"" + Escape(Get(key)) + "\",\"save\":true}"); }
    private void SendNumber(string key, string field) { CPH.WebsocketBroadcastString("{\"type\":\"COMETEN_CREDITS_SETTINGS\",\"field\":\"" + Escape(field) + "\",\"value\":" + NumberOr(Get(key), "0") + ",\"save\":true}"); }
    private string Get(string key) { try { return CPH.GetGlobalVar<string>(key, true) ?? ""; } catch { return ""; } }
    private string NumberOr(string value, string fallback) { double n; return double.TryParse((value ?? "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out n) ? n.ToString(System.Globalization.CultureInfo.InvariantCulture) : fallback; }
    private string Escape(string value) { return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " "); }
}
