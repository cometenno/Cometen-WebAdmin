using System;

public class CPHInline
{
    public bool Execute()
    {
        string user = args.ContainsKey("userName") ? args["userName"].ToString() : "Cometen Viewer";
        string count = "";

        if (args.ContainsKey("giftCount")) count = args["giftCount"].ToString();
        else if (args.ContainsKey("count")) count = args["count"].ToString();
        else if (args.ContainsKey("amount")) count = args["amount"].ToString();
        else if (args.ContainsKey("totalGiftedSubs")) count = args["totalGiftedSubs"].ToString();
        else if (args.ContainsKey("communitySubGiftCount")) count = args["communitySubGiftCount"].ToString();

        CPH.WebsocketBroadcastString(
            "{\"type\":\"COMETEN_ALERT\",\"alert\":\"giftbomb\",\"user\":\"" + Escape(user) + "\",\"count\":\"" + Escape(count) + "\"}"
        );

        return true;
    }

    private string Escape(string value)
    {
        return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}