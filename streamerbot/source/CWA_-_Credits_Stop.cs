using System;

public class CPHInline
{
    public bool Execute()
    {
        CPH.WebsocketBroadcastString("{\"type\":\"COMETEN_CREDITS_STOP\"}");
        return true;
    }
}
