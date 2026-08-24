using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        CPH.WebsocketBroadcastString(JsonConvert.SerializeObject(new {
            source = "CometenWebAdmin",
            type = "COMETEN_ALERT_CONTROL",
            command = "clear"
        }));
        string result = "Active alert stopped and queue cleared.";
        CPH.SetGlobalVar("CometenAlerts_LastResult", result, true);
        CPH.WebsocketBroadcastString(JsonConvert.SerializeObject(new {
            source = "CometenWebAdmin",
            type = "ALERTS_RESULT",
            result = result
        }));
        return true;
    }
}
