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
        SaveBool("CometenChatPip_Enabled", "enabled", true);
        SaveBoolCompat("CometenChatPip_NewChatterEnabled", "newChatterEnabled", "firstMessage", true);
        SaveBoolCompat("CometenChatPip_QuietChatEnabled", "quietChatEnabled", "afterSilence", true);
        SaveIntCompat("CometenChatPip_QuietMinutes", "quietMinutes", "silenceMinutes", 15, 1, 1440);
        SaveBoolCompat("CometenChatPip_ReturningUserEnabled", "returningUserEnabled", "userReturn", false);
        SaveIntCompat("CometenChatPip_ReturnMinutes", "returnMinutes", "userReturnMinutes", 60, 1, 10080);
        SaveSoundFile();
        SaveInt("CometenChatPip_Volume", "volume", 45, 0, 100);
        SaveString("CometenChatPip_IgnoreUsers", "ignoreUsers", "nightbot,streamelements,streamlabs,moobot,wizebot", true);
        SaveBool("CometenChatPip_IgnoreBroadcaster", "ignoreBroadcaster", true);
        SaveBool("CometenChatPip_ResetOnStreamStart", "resetOnStreamStart", true);
        CPH.SetGlobalVar("CometenChatPip_InstalledVersion", "1.1.1", true);

        int trackedUsers = GetTrackedUsers();
        CPH.SetGlobalVar("CometenChatPip_TrackedUsers", trackedUsers.ToString(CultureInfo.InvariantCulture), true);
        BroadcastSettings(trackedUsers);

        CPH.LogInfo(
            "[Cometen WebAdmin] Chat Pip settings saved. " +
            "FirstMessage=" + GetBool("CometenChatPip_NewChatterEnabled", true) +
            ", AfterSilence=" + GetBool("CometenChatPip_QuietChatEnabled", true) +
            ", SilenceMinutes=" + GetInt("CometenChatPip_QuietMinutes", 15) +
            ", UserReturn=" + GetBool("CometenChatPip_ReturningUserEnabled", false) +
            ", UserReturnMinutes=" + GetInt("CometenChatPip_ReturnMinutes", 60)
        );
        return true;
    }

    private void SaveSoundFile()
    {
        string current = GetGlobal("CometenChatPip_SoundFile", "");
        string value = GetArg("soundFile", "").Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            CPH.LogWarn("[Chat Pip Save] No soundFile value received. Existing global kept: " + current);
            return;
        }

        CPH.SetGlobalVar("CometenChatPip_SoundFile", value, true);

        string stored = CPH.GetGlobalVar<string>("CometenChatPip_SoundFile", true) ?? "";

        if (stored == value)
        {
            CPH.LogInfo("[Chat Pip Save] Sound path saved: " + stored);
        }
        else
        {
            CPH.LogError("[Chat Pip Save] Sound path verification failed. Sent: " + value + " | Stored: " + stored);
        }
    }

    private void SaveString(string globalName, string argName, string fallback, bool allowEmpty)
    {
        string value = GetArg(argName, fallback);
        if (value == null) value = fallback;
        if (!allowEmpty && string.IsNullOrWhiteSpace(value)) value = fallback;
        CPH.SetGlobalVar(globalName, value, true);
    }

    private void SaveBoolCompat(string globalName, string primaryArgName, string legacyArgName, bool fallback)
    {
        string raw = GetArgCompat(primaryArgName, legacyArgName, fallback ? "true" : "false")
            .Trim()
            .ToLowerInvariant();

        bool value =
            raw == "true" ||
            raw == "yes" ||
            raw == "1" ||
            raw == "on";

        CPH.SetGlobalVar(globalName, value ? "True" : "False", true);
    }

    private void SaveIntCompat(
        string globalName,
        string primaryArgName,
        string legacyArgName,
        int fallback,
        int minimum,
        int maximum)
    {
        int value;
        string raw = GetArgCompat(primaryArgName, legacyArgName, "");

        if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            value = fallback;

        if (value < minimum) value = minimum;
        if (value > maximum) value = maximum;

        CPH.SetGlobalVar(globalName, value.ToString(CultureInfo.InvariantCulture), true);
    }

    private string GetArgCompat(string primaryKey, string legacyKey, string fallback)
    {
        try
        {
            if (args.ContainsKey(primaryKey) && args[primaryKey] != null)
                return args[primaryKey].ToString();

            if (args.ContainsKey(legacyKey) && args[legacyKey] != null)
                return args[legacyKey].ToString();
        }
        catch { }

        return fallback;
    }

    private void SaveBool(string globalName, string argName, bool fallback)
    {
        string raw = GetArg(argName, fallback ? "true" : "false").Trim().ToLowerInvariant();
        string value = (raw == "true" || raw == "yes" || raw == "1") ? "True" : "False";
        CPH.SetGlobalVar(globalName, value, true);
    }

    private void SaveInt(string globalName, string argName, int fallback, int minimum, int maximum)
    {
        int value;
        if (!int.TryParse(GetArg(argName, ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) value = fallback;
        if (value < minimum) value = minimum;
        if (value > maximum) value = maximum;
        CPH.SetGlobalVar(globalName, value.ToString(CultureInfo.InvariantCulture), true);
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
            installedVersion = GetGlobal("CometenChatPip_InstalledVersion", "1.1.1")
        });

        CPH.WebsocketBroadcastJson(json);
    }

    private string GetArg(string key, string fallback)
    {
        try
        {
            if (args.ContainsKey(key) && args[key] != null) return args[key].ToString();
        }
        catch { }
        return fallback;
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
        return int.TryParse(GetGlobal(key, ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out result)
            ? result
            : fallback;
    }

    private bool GetBool(string key, bool fallback)
    {
        string value = GetGlobal(key, "").Trim().ToLowerInvariant();
        if (value == "true" || value == "yes" || value == "1") return true;
        if (value == "false" || value == "no" || value == "0") return false;
        return fallback;
    }
}
