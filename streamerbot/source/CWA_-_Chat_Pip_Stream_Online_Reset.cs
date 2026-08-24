using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        if (!GetBool("CometenChatPip_ResetOnStreamStart", true))
        {
            CPH.LogInfo("[Chat Pip] Stream Online received, but automatic session reset is disabled.");
            return true;
        }

        CPH.UnsetGlobalVar("CometenChatPip_State", false);
        CPH.SetGlobalVar("CometenChatPip_LastUser", "", true);
        CPH.SetGlobalVar("CometenChatPip_LastReason", "New stream - session reset", true);
        CPH.SetGlobalVar("CometenChatPip_LastTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true);
        CPH.SetGlobalVar("CometenChatPip_TrackedUsers", "0", true);
        BroadcastSettings();
        CPH.LogInfo("[Chat Pip] Runtime chatter session reset on Stream Online.");
        return true;
    }

    private void BroadcastSettings()
    {
        string soundFile = GetGlobal("CometenChatPip_SoundFile", "");
        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin",
            type = "CHAT_PIP_SETTINGS",
            enabled = GetBool("CometenChatPip_Enabled", true),
            newChatterEnabled = GetBool("CometenChatPip_NewChatterEnabled", true),
            quietChatEnabled = GetBool("CometenChatPip_QuietChatEnabled", true),
            quietMinutes = GetInt("CometenChatPip_QuietMinutes", 15),
            returningUserEnabled = GetBool("CometenChatPip_ReturningUserEnabled", false),
            returnMinutes = GetInt("CometenChatPip_ReturnMinutes", 60),
            soundFile = soundFile,
            volume = GetInt("CometenChatPip_Volume", 45),
            ignoreUsers = GetGlobal("CometenChatPip_IgnoreUsers", "nightbot,streamelements,streamlabs,moobot,wizebot"),
            ignoreBroadcaster = GetBool("CometenChatPip_IgnoreBroadcaster", true),
            resetOnStreamStart = GetBool("CometenChatPip_ResetOnStreamStart", true),
            lastUser = "",
            lastReason = "New stream - session reset",
            lastTime = GetGlobal("CometenChatPip_LastTime", ""),
            trackedUsers = 0,
            soundFileExists = !string.IsNullOrWhiteSpace(soundFile) && File.Exists(soundFile),
            installedVersion = GetGlobal("CometenChatPip_InstalledVersion", "1.1.0")
        });
        CPH.WebsocketBroadcastJson(json);
    }

    private string GetGlobal(string key, string fallback)
    {
        try
        {
            string value = CPH.GetGlobalVar<string>(key, true);
            return value ?? fallback;
        }
        catch (Exception ex)
        {
            CPH.LogError("[Chat Pip] Failed reading persisted global " + key + ": " + ex.Message);
            return fallback;
        }
    }

    private int GetInt(string key, int fallback)
    {
        int result;
        return int.TryParse(GetGlobal(key, ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : fallback;
    }

    private bool GetBool(string key, bool fallback)
    {
        string value = GetGlobal(key, "").Trim().ToLowerInvariant();
        if (value == "true" || value == "yes" || value == "1") return true;
        if (value == "false" || value == "no" || value == "0") return false;
        return fallback;
    }
}
