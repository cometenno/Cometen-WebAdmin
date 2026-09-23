using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

public class CPHInline
{
    private const string StateVariable = "CometenChatPip_State";

    private class ChatPipState
    {
        public long LastChatUnix { get; set; }
        public Dictionary<string, long> Users { get; set; }
    }

    public bool Execute()
    {
        string soundFile = GetGlobal("CometenChatPip_SoundFile", "");
        int volumePercent = Clamp(GetInt("CometenChatPip_Volume", 45), 0, 100);
        int trackedUsers = GetTrackedUsers();

        if (string.IsNullOrWhiteSpace(soundFile) || !File.Exists(soundFile))
        {
            SetStatus("WebAdmin test", "Test failed - sound file not found: " + soundFile, trackedUsers);
            BroadcastSettings(trackedUsers);
            CPH.LogError("[Chat Pip] Test failed. Sound file not found: " + soundFile);
            return true;
        }

        CPH.PlaySound(soundFile, volumePercent / 100.0f, false);
        SetStatus("WebAdmin test", "Test pip played", trackedUsers);
        BroadcastSettings(trackedUsers);
        CPH.LogInfo("[Chat Pip] Test pip played.");
        return true;
    }

    private void SetStatus(string user, string reason, int trackedUsers)
    {
        CPH.SetGlobalVar("CometenChatPip_LastUser", user, true);
        CPH.SetGlobalVar("CometenChatPip_LastReason", reason, true);
        CPH.SetGlobalVar("CometenChatPip_LastTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true);
        CPH.SetGlobalVar("CometenChatPip_TrackedUsers", trackedUsers.ToString(CultureInfo.InvariantCulture), true);
    }

    private int GetTrackedUsers()
    {
        try
        {
            string json = CPH.GetGlobalVar<string>(StateVariable, false);
            if (!string.IsNullOrWhiteSpace(json))
            {
                ChatPipState state = JsonConvert.DeserializeObject<ChatPipState>(json);
                if (state != null && state.Users != null) return state.Users.Count;
            }
        }
        catch { }
        return GetInt("CometenChatPip_TrackedUsers", 0);
    }

    private void BroadcastSettings(int trackedUsers)
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
            lastUser = GetGlobal("CometenChatPip_LastUser", ""),
            lastReason = GetGlobal("CometenChatPip_LastReason", ""),
            lastTime = GetGlobal("CometenChatPip_LastTime", ""),
            trackedUsers = trackedUsers,
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

    private int Clamp(int value, int minimum, int maximum)
    {
        if (value < minimum) return minimum;
        if (value > maximum) return maximum;
        return value;
    }
}
