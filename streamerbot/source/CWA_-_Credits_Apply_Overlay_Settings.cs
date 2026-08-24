using System;
using System.Globalization;

public class CPHInline
{
    public bool Execute()
    {
        int sent = 0;

        sent += SendText("CometenCredits_TopTitle", "title");
        sent += SendText("CometenCredits_TopText", "subtitle");
        sent += SendText("CometenCredits_BottomTitle", "footer");
        sent += SendText("CometenCredits_BottomText", "footerSmall");

        sent += SendText("CometenCredits_ColorGold", "gold");
        sent += SendText("CometenCredits_ColorGoldLight", "goldLight");
        sent += SendText("CometenCredits_ColorText", "textColor");
        sent += SendText("CometenCredits_PanelBg", "panelBg");

        sent += SendNumber("CometenCredits_TitleSize", "mainTitleSize");
        sent += SendNumber("CometenCredits_TopTextSize", "subtitleSize");
        sent += SendNumber("CometenCredits_SectionTitleSize", "sectionTitleSize");
        sent += SendNumber("CometenCredits_NameSize", "namesSize");
        sent += SendNumber("CometenCredits_SmallNameSize", "smallNamesSize");
        sent += SendNumber("CometenCredits_BottomTitleSize", "footerSize");
        sent += SendNumber("CometenCredits_BottomTextSize", "footerSmallSize");

        sent += SendNumber("CometenCredits_ScrollSpeed", "speed");

        sent += SendNumber("CometenCredits_FrameLeft", "frameLeft");
        sent += SendNumber("CometenCredits_FrameTop", "frameTop");
        sent += SendNumber("CometenCredits_FrameWidth", "frameWidth");
        sent += SendNumber("CometenCredits_FrameHeight", "frameHeight");
        sent += SendNumber("CometenCredits_ViewportLeft", "viewportLeft");
        sent += SendNumber("CometenCredits_ViewportWidth", "viewportWidth");
        sent += SendNumber("CometenCredits_SectionGap", "sectionGap");

        CPH.LogInfo("Credits easy variables loaded. Sent settings: " + sent);
        return true;
    }

    private int SendText(string globalKey, string field)
    {
        string value = Get(globalKey);
        if (string.IsNullOrWhiteSpace(value)) return 0;

        string json = "{\"type\":\"COMETEN_CREDITS_SETTINGS\",\"field\":\"" + Escape(field) +
                      "\",\"value\":\"" + Escape(value) + "\",\"save\":true}";

        CPH.WebsocketBroadcastString(json);
        return 1;
    }

    private int SendNumber(string globalKey, string field)
    {
        string value = Get(globalKey);
        if (string.IsNullOrWhiteSpace(value)) return 0;

        double num;
        if (!double.TryParse(value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num))
            return 0;

        string json = "{\"type\":\"COMETEN_CREDITS_SETTINGS\",\"field\":\"" + Escape(field) +
                      "\",\"value\":" + num.ToString(CultureInfo.InvariantCulture) + ",\"save\":true}";

        CPH.WebsocketBroadcastString(json);
        return 1;
    }

    private string Get(string key)
    {
        try { return CPH.GetGlobalVar<string>(key, true) ?? ""; }
        catch { return ""; }
    }

    private string Escape(string value)
    {
        return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ");
    }
}
