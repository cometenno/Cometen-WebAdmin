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

        public ChatPipState()
        {
            Users = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public bool Execute()
    {
        if (!GetBool("CometenChatPip_Enabled", true))
        {
            return true;
        }

        string userName = GetArg("userName", "");
        string displayName = GetArg("user", userName);

        if (string.IsNullOrWhiteSpace(userName))
        {
            userName = displayName;
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            CPH.LogWarn("[Chat Pip] Twitch Chat Message did not contain a user name.");
            return true;
        }

        userName = userName.Trim().TrimStart('@');
        displayName = string.IsNullOrWhiteSpace(displayName) ? userName : displayName.Trim().TrimStart('@');
        string normalizedUser = userName.ToLowerInvariant();

        if (IsIgnoredUser(normalizedUser))
        {
            return true;
        }

        if (GetBool("CometenChatPip_IgnoreBroadcaster", true))
        {
            string broadcaster = GetArg("broadcastUserName", GetArg("broadcastUser", ""));
            broadcaster = (broadcaster ?? "").Trim().TrimStart('@').ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(broadcaster) && normalizedUser == broadcaster)
            {
                return true;
            }
        }

        ChatPipState state = LoadState();
        long now = UnixNow();
        long previousUserMessage = 0;
        bool hasPreviousUserMessage = state.Users.TryGetValue(normalizedUser, out previousUserMessage);

        bool newChatter = !hasPreviousUserMessage;
        bool chatWasQuiet =
            state.LastChatUnix > 0 &&
            now - state.LastChatUnix >= GetInt("CometenChatPip_QuietMinutes", 15) * 60L;

        bool returningUser =
            hasPreviousUserMessage &&
            now - previousUserMessage >= GetInt("CometenChatPip_ReturnMinutes", 60) * 60L;

        state.Users[normalizedUser] = now;
        state.LastChatUnix = now;
        SaveState(state);

        if (newChatter)
        {
            CPH.SetGlobalVar("CometenChatPip_TrackedUsers", state.Users.Count.ToString(CultureInfo.InvariantCulture), true);
        }

        bool shouldPlay =
            (newChatter && GetBool("CometenChatPip_NewChatterEnabled", true)) ||
            (chatWasQuiet && GetBool("CometenChatPip_QuietChatEnabled", true)) ||
            (returningUser && GetBool("CometenChatPip_ReturningUserEnabled", false));

        if (!shouldPlay)
        {
            return true;
        }

        string reason;

        if (newChatter && GetBool("CometenChatPip_NewChatterEnabled", true))
        {
            reason = "First message this stream";
        }
        else if (chatWasQuiet && GetBool("CometenChatPip_QuietChatEnabled", true))
        {
            reason = "Chat was quiet for at least " + GetInt("CometenChatPip_QuietMinutes", 15) + " minutes";
        }
        else
        {
            reason = "User returned after at least " + GetInt("CometenChatPip_ReturnMinutes", 60) + " minutes";
        }

        string soundFile = GetGlobal("CometenChatPip_SoundFile", "");
        int volumePercent = Clamp(GetInt("CometenChatPip_Volume", 45), 0, 100);

        if (string.IsNullOrWhiteSpace(soundFile) || !File.Exists(soundFile))
        {
            UpdateLastStatus(displayName, "Sound file not found: " + soundFile, state.Users.Count);
            BroadcastSettings(state.Users.Count);
            CPH.LogError("[Chat Pip] Sound file not found: " + soundFile);
            return true;
        }

        float volume = volumePercent / 100.0f;
        CPH.PlaySound(soundFile, volume, false);

        UpdateLastStatus(displayName, reason, state.Users.Count);
        BroadcastSettings(state.Users.Count);
        CPH.LogInfo("[Chat Pip] " + displayName + " - " + reason);

        return true;
    }

    private bool IsIgnoredUser(string normalizedUser)
    {
        string raw = GetGlobal(
            "CometenChatPip_IgnoreUsers",
            "nightbot,streamelements,streamlabs,moobot,wizebot"
        );

        string[] users = raw.Split(',');

        foreach (string item in users)
        {
            string clean = (item ?? "").Trim().TrimStart('@').ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(clean) && clean == normalizedUser)
            {
                return true;
            }
        }

        return false;
    }

    private ChatPipState LoadState()
    {
        try
        {
            string json = CPH.GetGlobalVar<string>(StateVariable, false);

            if (!string.IsNullOrWhiteSpace(json))
            {
                ChatPipState state = JsonConvert.DeserializeObject<ChatPipState>(json);

                if (state != null)
                {
                    if (state.Users == null)
                    {
                        state.Users = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
                    }

                    return state;
                }
            }
        }
        catch (Exception ex)
        {
            CPH.LogWarn("[Chat Pip] Could not read runtime state. A new state was created. " + ex.Message);
        }

        return new ChatPipState();
    }

    private void SaveState(ChatPipState state)
    {
        string json = JsonConvert.SerializeObject(state);
        CPH.SetGlobalVar(StateVariable, json, false);
    }

    private void UpdateLastStatus(string user, string reason, int trackedUsers)
    {
        CPH.SetGlobalVar("CometenChatPip_LastUser", user ?? "", true);
        CPH.SetGlobalVar("CometenChatPip_LastReason", reason ?? "", true);
        CPH.SetGlobalVar("CometenChatPip_LastTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true);
        CPH.SetGlobalVar("CometenChatPip_TrackedUsers", trackedUsers.ToString(CultureInfo.InvariantCulture), true);
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
            volume = Clamp(GetInt("CometenChatPip_Volume", 45), 0, 100),
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

    private string GetArg(string key, string fallback)
    {
        try
        {
            if (args.ContainsKey(key) && args[key] != null)
            {
                return args[key].ToString();
            }
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

    private int Clamp(int value, int minimum, int maximum)
    {
        if (value < minimum) return minimum;
        if (value > maximum) return maximum;
        return value;
    }

    private long UnixNow()
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
    }
}
