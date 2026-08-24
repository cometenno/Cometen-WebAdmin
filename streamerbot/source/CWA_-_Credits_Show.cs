using System;
using System.Collections.Generic;
using System.Text;

public class CPHInline
{
    public bool Execute()
    {
        LoadSettings();

        List<string> sections = new List<string>();

        AddListSection(sections, "New Followers", Get("CometenCredits_Follows"));
        AddListSection(sections, "Cheers", Get("CometenCredits_Cheers"));
        AddListSection(sections, "Subs", Get("CometenCredits_Subs"));
        AddListSection(sections, "Resubs", Get("CometenCredits_Resubs"));
        AddListSection(sections, "Gift Subs", Get("CometenCredits_GiftSubs"));
        AddListSection(sections, "Gift Bombs", Get("CometenCredits_GiftBombs"));
        AddListSection(sections, "Raiders", Get("CometenCredits_Raiders"));
        AddListSection(sections, "Reward Redemptions", Get("CometenCredits_Rewards"));

        AddTopSection(sections, "Top Cheer", Get("CometenCredits_TopCheer"), " bits");
        AddTopSection(sections, "Top Channel Points", Get("CometenCredits_TopChannelPoints"), " points");

        AddListSection(sections, "Chat Crew", Get("CometenCredits_ChatCrew"));

        string title = GetOr("CometenCredits_TopTitle", "END OF STREAM CREDITS");
        string subtitle = GetOr("CometenCredits_TopText", "Thank you for hanging out in the Cometen universe");
        string footer = GetOr("CometenCredits_BottomTitle", "SEE YOU NEXT STREAM");
        string footerSmall = GetOr("CometenCredits_BottomText", "Twitch: <TWITCH_CHANNEL> - X: <X_HANDLE> - YouTube: <YOUTUBE_HANDLE>");
        string speed = GetOr("CometenCredits_ScrollSpeed", "62");

        string json =
            "{\"type\":\"COMETEN_CREDITS_ROLL\"," +
            "\"title\":\"" + Escape(title) + "\"," +
            "\"subtitle\":\"" + Escape(subtitle) + "\"," +
            "\"footer\":\"" + Escape(footer) + "\"," +
            "\"footerSmall\":\"" + Escape(footerSmall) + "\"," +
            "\"speed\":" + NumberOr(speed, "62") + "," +
            "\"sections\":[" + string.Join(",", sections.ToArray()) + "]}";

        CPH.WebsocketBroadcastString(json);
        return true;
    }

    private void LoadSettings()
    {
        SendText("CometenCredits_TopTitle", "title");
        SendText("CometenCredits_TopText", "subtitle");
        SendText("CometenCredits_BottomTitle", "footer");
        SendText("CometenCredits_BottomText", "footerSmall");

        SendText("CometenCredits_ColorGold", "gold");
        SendText("CometenCredits_ColorGoldLight", "goldLight");
        SendText("CometenCredits_ColorText", "textColor");
        SendText("CometenCredits_PanelBg", "panelBg");

        SendNumber("CometenCredits_TitleSize", "mainTitleSize");
        SendNumber("CometenCredits_TopTextSize", "subtitleSize");
        SendNumber("CometenCredits_SectionTitleSize", "sectionTitleSize");
        SendNumber("CometenCredits_NameSize", "namesSize");
        SendNumber("CometenCredits_SmallNameSize", "smallNamesSize");
        SendNumber("CometenCredits_BottomTitleSize", "footerSize");
        SendNumber("CometenCredits_BottomTextSize", "footerSmallSize");

        SendNumber("CometenCredits_ScrollSpeed", "speed");

        SendNumber("CometenCredits_FrameLeft", "frameLeft");
        SendNumber("CometenCredits_FrameTop", "frameTop");
        SendNumber("CometenCredits_FrameWidth", "frameWidth");
        SendNumber("CometenCredits_FrameHeight", "frameHeight");
        SendNumber("CometenCredits_ViewportLeft", "viewportLeft");
        SendNumber("CometenCredits_ViewportWidth", "viewportWidth");
        SendNumber("CometenCredits_SectionGap", "sectionGap");
    }

    private void SendText(string globalKey, string field)
    {
        string value = Get(globalKey);

        if (string.IsNullOrWhiteSpace(value))
            return;

        string json =
            "{\"type\":\"COMETEN_CREDITS_SETTINGS\",\"field\":\"" + Escape(field) +
            "\",\"value\":\"" + Escape(value) +
            "\",\"save\":true}";

        CPH.WebsocketBroadcastString(json);
    }

    private void SendNumber(string globalKey, string field)
    {
        string value = Get(globalKey);

        if (string.IsNullOrWhiteSpace(value))
            return;

        string json =
            "{\"type\":\"COMETEN_CREDITS_SETTINGS\",\"field\":\"" + Escape(field) +
            "\",\"value\":" + NumberOr(value, "0") +
            ",\"save\":true}";

        CPH.WebsocketBroadcastString(json);
    }

    private string Get(string key)
    {
        try
        {
            return CPH.GetGlobalVar<string>(key, true) ?? "";
        }
        catch
        {
            return "";
        }
    }

    private string GetOr(string key, string fallback)
    {
        string value = Get(key);

        if (string.IsNullOrWhiteSpace(value))
            return fallback;

        return value;
    }

    private string NumberOr(string value, string fallback)
    {
        double num;

        if (!double.TryParse(
            (value ?? "").Replace(",", "."),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out num))
        {
            return fallback;
        }

        return num.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private void AddListSection(List<string> sections, string title, string csv)
    {
        List<string> items = ParseCsv(csv);

        if (items.Count == 0)
            return;

        StringBuilder sb = new StringBuilder();

        sb.Append("{\"title\":\"").Append(Escape(title)).Append("\",\"items\":[");

        for (int i = 0; i < items.Count; i++)
        {
            if (i > 0)
                sb.Append(",");

            sb.Append("\"").Append(Escape(items[i])).Append("\"");
        }

        sb.Append("]}");
        sections.Add(sb.ToString());
    }

    private void AddTopSection(List<string> sections, string title, string data, string suffix)
    {
        if (string.IsNullOrWhiteSpace(data))
            return;

        string[] parts = data.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        List<TopItem> items = new List<TopItem>();

        foreach (string part in parts)
        {
            string[] kv = part.Split(new[] { '=' }, 2);

            if (kv.Length != 2)
                continue;

            int amount = 0;
            int.TryParse(kv[1], out amount);

            if (amount <= 0)
                continue;

            items.Add(new TopItem
            {
                Name = kv[0],
                Amount = amount
            });
        }

        items.Sort((a, b) => b.Amount.CompareTo(a.Amount));

        if (items.Count == 0)
            return;

        int take = Math.Min(items.Count, 5);

        StringBuilder sb = new StringBuilder();

        sb.Append("{\"title\":\"").Append(Escape(title)).Append("\",\"items\":[");

        for (int i = 0; i < take; i++)
        {
            if (i > 0)
                sb.Append(",");

            sb.Append("{\"name\":\"")
              .Append(Escape(items[i].Name))
              .Append("\",\"amount\":\"")
              .Append(Escape(items[i].Amount.ToString() + suffix))
              .Append("\"}");
        }

        sb.Append("]}");
        sections.Add(sb.ToString());
    }

    private List<string> ParseCsv(string csv)
    {
        List<string> output = new List<string>();

        if (string.IsNullOrWhiteSpace(csv))
            return output;

        string[] parts = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            string item = part.Trim();

            if (!string.IsNullOrWhiteSpace(item))
                output.Add(item);
        }

        return output;
    }

    private string Escape(string value)
    {
        return (value ?? "")
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", " ")
            .Replace("\n", " ");
    }

    private class TopItem
    {
        public string Name;
        public int Amount;
    }
}