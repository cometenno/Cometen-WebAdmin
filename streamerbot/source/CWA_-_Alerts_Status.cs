using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    private const string SettingsKey = "CometenAlerts_SettingsJson";
    private const string LastResultKey = "CometenAlerts_LastResult";
    private const string DefaultSettingsJson = @"{""version"":2,""profileName"":""Cometen Alerts"",""global"":{""enabled"":true,""soundEnabled"":true,""masterVolume"":1.0,""defaultDuration"":8000,""fadeOut"":650,""defaultVolume"":0.85,""maxQueue"":20,""queueDelay"":350,""fontFamily"":""Impact, \""Arial Black\"", Arial, sans-serif"",""userColor"":""#ffd56a"",""messageColor"":""#f0a900"",""userMaxSize"":82,""userMinSize"":46,""messageMaxSize"":44,""messageMinSize"":28,""offsetX"":0,""offsetY"":0},""alerts"":{""follow"":{""enabled"":true,""image"":""alert_follow.png"",""sound"":""sounds/follow.mp3"",""volume"":0.8,""duration"":8000,""animation"":""slide_up"",""box"":{""left"":560,""top"":390,""width"":1180,""height"":360},""offsetX"":0,""offsetY"":0,""userSize"":82,""messageSize"":44,""userColor"":"""",""messageColor"":"""",""defaultMessage"":""""},""sub"":{""enabled"":true,""image"":""alert_sub.png"",""sound"":""sounds/sub.mp3"",""volume"":0.85,""duration"":8000,""animation"":""pop"",""box"":{""left"":520,""top"":415,""width"":1220,""height"":320},""offsetX"":0,""offsetY"":0,""userSize"":82,""messageSize"":44,""userColor"":"""",""messageColor"":"""",""defaultMessage"":""""},""resub"":{""enabled"":true,""image"":""alert_resub.png"",""sound"":""sounds/resub.mp3"",""volume"":0.85,""duration"":8000,""animation"":""pop"",""box"":{""left"":570,""top"":370,""width"":1100,""height"":370},""offsetX"":0,""offsetY"":0,""userSize"":82,""messageSize"":44,""userColor"":"""",""messageColor"":"""",""defaultMessage"":""""},""gifted"":{""enabled"":true,""image"":""alert_gifted_subs.png"",""sound"":""sounds/gifted.mp3"",""volume"":0.85,""duration"":8000,""animation"":""drop"",""box"":{""left"":535,""top"":345,""width"":1130,""height"":390},""offsetX"":0,""offsetY"":0,""userSize"":82,""messageSize"":44,""userColor"":"""",""messageColor"":"""",""defaultMessage"":""""},""giftbomb"":{""enabled"":true,""image"":""alert_giftbomb.png"",""sound"":""sounds/giftbomb.mp3"",""volume"":0.9,""duration"":8000,""animation"":""drop"",""box"":{""left"":500,""top"":345,""width"":1230,""height"":390},""offsetX"":0,""offsetY"":0,""userSize"":82,""messageSize"":44,""userColor"":"""",""messageColor"":"""",""defaultMessage"":""""},""bits"":{""enabled"":true,""image"":""alert_bits.png"",""sound"":""sounds/bits.mp3"",""volume"":0.82,""duration"":8000,""animation"":""glitch"",""box"":{""left"":560,""top"":365,""width"":1120,""height"":350},""offsetX"":0,""offsetY"":0,""userSize"":82,""messageSize"":44,""userColor"":"""",""messageColor"":"""",""defaultMessage"":""""},""charity"":{""enabled"":true,""image"":""alert_donation.png"",""sound"":""sounds/donation.mp3"",""volume"":0.82,""duration"":8000,""animation"":""pop"",""box"":{""left"":585,""top"":400,""width"":1210,""height"":330},""offsetX"":0,""offsetY"":0,""userSize"":82,""messageSize"":44,""userColor"":"""",""messageColor"":"""",""defaultMessage"":""""},""raid"":{""enabled"":true,""image"":""alert_raid.png"",""sound"":""sounds/raid.mp3"",""volume"":0.9,""duration"":8000,""animation"":""shake"",""box"":{""left"":430,""top"":420,""width"":1150,""height"":330},""offsetX"":0,""offsetY"":0,""userSize"":82,""messageSize"":44,""userColor"":"""",""messageColor"":"""",""defaultMessage"":""""},""yt_sub"":{""enabled"":true,""image"":""alert_yt_sub.png"",""sound"":""sounds/sub.mp3"",""volume"":0.85,""duration"":8000,""animation"":""pop"",""box"":{""left"":535,""top"":380,""width"":1160,""height"":360},""offsetX"":0,""offsetY"":0,""userSize"":82,""messageSize"":44,""userColor"":"""",""messageColor"":"""",""defaultMessage"":""""}}}";

    public bool Execute()
    {
        string settingsJson = GetSettings();
        string client = GetArg("client", "");
        Broadcast(settingsJson, client == "overlay" ? "overlay-load" : "status");
        return true;
    }

    private string GetSettings()
    {
        string value = "";
        try { value = CPH.GetGlobalVar<string>(SettingsKey, true); } catch { }
        if (string.IsNullOrWhiteSpace(value) || !IsValidJson(value))
        {
            value = DefaultSettingsJson;
            CPH.SetGlobalVar(SettingsKey, value, true);
            CPH.SetGlobalVar(LastResultKey, "Default alert settings created.", true);
        }
        return value;
    }

    private bool IsValidJson(string value)
    {
        try { JToken.Parse(value); return true; } catch { return false; }
    }

    private string GetArg(string name, string fallback)
    {
        try { if (args.ContainsKey(name) && args[name] != null) return args[name].ToString(); } catch { }
        return fallback;
    }

    private void Broadcast(string settingsJson, string action)
    {
        JToken settings = JToken.Parse(settingsJson);
        string lastResult = "";
        try { lastResult = CPH.GetGlobalVar<string>(LastResultKey, true); } catch { }
        string json = JsonConvert.SerializeObject(new {
            source = "CometenWebAdmin",
            type = "ALERTS_SETTINGS",
            action = action,
            settings = settings,
            settingsJson = settingsJson,
            lastResult = lastResult
        });
        CPH.WebsocketBroadcastString(json);
    }
}
