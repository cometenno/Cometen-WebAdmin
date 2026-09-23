using System;

public class CPHInline
{
    public bool Execute()
    {
        string json =
            "{\"type\":\"COMETEN_CREDITS_ROLL\"," +
            "\"sections\":[" +
                "{\"title\":\"New Followers\",\"items\":[\"Kathrine87\",\"NordicViewer\",\"AgentPhoenix\"]}," +
                "{\"title\":\"Subs\",\"items\":[\"CometenFan\",\"SHDAgent\"]}," +
                "{\"title\":\"Gift Bombs\",\"items\":[{\"name\":\"BigSupporter\",\"amount\":\"5 gifted subs\"}]}," +
                "{\"title\":\"Cheers\",\"items\":[{\"name\":\"BitsBoss\",\"amount\":\"500 bits\"}]}," +
                "{\"title\":\"Raiders\",\"items\":[{\"name\":\"Mystic\",\"amount\":\"18 raiders\"},{\"name\":\"Reckenin\",\"amount\":\"12 raiders\"}]}" +
            "]}";

        CPH.WebsocketBroadcastString(json);
        return true;
    }
}
