using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

public class CPHInline
{

    private const string SettingsVariable = "CometenUrlGuard_SettingsJson";

    private class UrlGuardSettings
    {
        public bool Enabled { get; set; }
        public bool AllowBroadcaster { get; set; }
        public bool AllowMods { get; set; }
        public bool AllowVips { get; set; }
        public bool DeleteOtherUrls { get; set; }
        public bool DeleteUrlCommands { get; set; }
        public bool AnnounceBlocked { get; set; }
        public bool LogDeletedUrls { get; set; }
        public bool LogSkipYouTube { get; set; }
        public bool LogSkipSpotify { get; set; }
        public string BlockMessage { get; set; }
        public string AllowUsers { get; set; }
        public string InstalledVersion { get; set; }
    }

    private UrlGuardSettings Defaults()
    {
        return new UrlGuardSettings
        {
            Enabled = true,
            AllowBroadcaster = true,
            AllowMods = true,
            AllowVips = true,
            DeleteOtherUrls = true,
            DeleteUrlCommands = true,
            AnnounceBlocked = true,
            LogDeletedUrls = true,
            LogSkipYouTube = false,
            LogSkipSpotify = false,
            BlockMessage = "@{user} lenker er kun tillatt for VIP/mods.",
            AllowUsers = "",
            InstalledVersion = "1.2.0"
        };
    }


    public bool Execute()
    {
        UrlGuardSettings s = LoadSettings();

        s.Enabled = GetArgBool("enabled", s.Enabled);
        s.AllowBroadcaster = GetArgBool("allowBroadcaster", s.AllowBroadcaster);
        s.AllowMods = GetArgBool("allowMods", s.AllowMods);
        s.AllowVips = GetArgBool("allowVips", s.AllowVips);
        s.DeleteOtherUrls = GetArgBool("deleteOtherUrls", s.DeleteOtherUrls);
        s.DeleteUrlCommands = GetArgBool("deleteUrlCommands", s.DeleteUrlCommands);
        s.AnnounceBlocked = GetArgBool("announceBlocked", s.AnnounceBlocked);
        s.LogDeletedUrls = GetArgBool("logDeletedUrls", s.LogDeletedUrls);
        s.LogSkipYouTube = GetArgBool("logSkipYouTube", s.LogSkipYouTube);
        s.LogSkipSpotify = GetArgBool("logSkipSpotify", s.LogSkipSpotify);
        s.BlockMessage = GetArgString("blockMessage", s.BlockMessage);
        s.AllowUsers = GetArgString("allowUsers", s.AllowUsers);

        SaveSettingsState(s);
        Broadcast(s, "URL Guard settings saved.");

        CPH.LogInfo(
            "[Cometen WebAdmin] URL Guard JSON settings saved. " +
            "Logging=" + s.LogDeletedUrls +
            ", SkipYouTube=" + s.LogSkipYouTube +
            ", SkipSpotify=" + s.LogSkipSpotify
        );

        return true;
    }

    private bool GetArgBool(string key, bool fallback)
    {
        try
        {
            if (!args.ContainsKey(key) || args[key] == null)
                return fallback;

            object raw = args[key];
            if (raw is bool) return (bool)raw;

            string text = raw.ToString().Trim().ToLowerInvariant();
            if (text == "true" || text == "1" || text == "yes" || text == "on") return true;
            if (text == "false" || text == "0" || text == "no" || text == "off") return false;
        }
        catch { }
        return fallback;
    }

    private string GetArgString(string key, string fallback)
    {
        try
        {
            if (args.ContainsKey(key) && args[key] != null)
                return args[key].ToString();
        }
        catch { }
        return fallback ?? "";
    }


    private UrlGuardSettings LoadSettings()
    {
        try
        {
            string json = CPH.GetGlobalVar<string>(SettingsVariable, true);
            if (!string.IsNullOrWhiteSpace(json))
            {
                UrlGuardSettings parsed = JsonConvert.DeserializeObject<UrlGuardSettings>(json);
                if (parsed != null)
                {
                    parsed.BlockMessage = parsed.BlockMessage ?? "@{user} lenker er kun tillatt for VIP/mods.";
                    parsed.AllowUsers = parsed.AllowUsers ?? "";
                    parsed.InstalledVersion = "1.2.0";
                    return parsed;
                }
            }
        }
        catch { }

        UrlGuardSettings migrated = Defaults();
        migrated.Enabled = ReadLegacyBool("CometenUrlGuard_Enabled", migrated.Enabled);
        migrated.AllowBroadcaster = ReadLegacyBool("CometenUrlGuard_AllowBroadcaster", migrated.AllowBroadcaster);
        migrated.AllowMods = ReadLegacyBool("CometenUrlGuard_AllowMods", migrated.AllowMods);
        migrated.AllowVips = ReadLegacyBool("CometenUrlGuard_AllowVips", migrated.AllowVips);
        migrated.DeleteOtherUrls = ReadLegacyBool("CometenUrlGuard_DeleteOtherUrls", migrated.DeleteOtherUrls);
        migrated.DeleteUrlCommands = ReadLegacyBool("CometenUrlGuard_DeleteUrlCommands", migrated.DeleteUrlCommands);
        migrated.AnnounceBlocked = ReadLegacyBool("CometenUrlGuard_AnnounceBlocked", migrated.AnnounceBlocked);
        migrated.LogDeletedUrls = ReadLegacyBool("CometenUrlGuard_LogDeletedUrls", migrated.LogDeletedUrls);
        migrated.LogSkipYouTube = ReadLegacyBool("CometenUrlGuard_LogSkipYouTube", migrated.LogSkipYouTube);
        migrated.LogSkipSpotify = ReadLegacyBool("CometenUrlGuard_LogSkipSpotify", migrated.LogSkipSpotify);
        migrated.BlockMessage = ReadLegacyString("CometenUrlGuard_BlockMessage", migrated.BlockMessage);
        migrated.AllowUsers = ReadLegacyString("CometenUrlGuard_AllowUsers", migrated.AllowUsers);
        SaveSettingsState(migrated);
        return migrated;
    }

    private void SaveSettingsState(UrlGuardSettings settings)
    {
        settings.InstalledVersion = "1.2.0";
        string json = JsonConvert.SerializeObject(settings);
        CPH.SetGlobalVar(SettingsVariable, json, true);
        MirrorLegacyGlobals(settings);
    }

    private void MirrorLegacyGlobals(UrlGuardSettings settings)
    {
        CPH.SetGlobalVar("CometenUrlGuard_Enabled", settings.Enabled ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_AllowBroadcaster", settings.AllowBroadcaster ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_AllowMods", settings.AllowMods ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_AllowVips", settings.AllowVips ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_DeleteOtherUrls", settings.DeleteOtherUrls ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_DeleteUrlCommands", settings.DeleteUrlCommands ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_AnnounceBlocked", settings.AnnounceBlocked ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_LogDeletedUrls", settings.LogDeletedUrls ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_LogSkipYouTube", settings.LogSkipYouTube ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_LogSkipSpotify", settings.LogSkipSpotify ? "True" : "False", true);
        CPH.SetGlobalVar("CometenUrlGuard_BlockMessage", settings.BlockMessage ?? "", true);
        CPH.SetGlobalVar("CometenUrlGuard_AllowUsers", settings.AllowUsers ?? "", true);
        CPH.SetGlobalVar("CometenUrlGuard_InstalledVersion", "1.2.0", true);
    }

    private bool ReadLegacyBool(string name, bool fallback)
    {
        try
        {
            object raw = CPH.GetGlobalVar<object>(name, true);
            if (raw == null) return fallback;
            if (raw is bool) return (bool)raw;

            string text = raw.ToString().Trim().ToLowerInvariant();
            if (text == "true" || text == "1" || text == "yes" || text == "on") return true;
            if (text == "false" || text == "0" || text == "no" || text == "off") return false;
        }
        catch { }
        return fallback;
    }

    private string ReadLegacyString(string name, string fallback)
    {
        try
        {
            object raw = CPH.GetGlobalVar<object>(name, true);
            return raw == null ? fallback : raw.ToString();
        }
        catch { return fallback; }
    }


    private void Broadcast(UrlGuardSettings s, string result)
    {
        var payload = new
        {
            source = "CometenWebAdmin",
            type = "URL_GUARD_SETTINGS",
            enabled = s.Enabled,
            allowBroadcaster = s.AllowBroadcaster,
            allowMods = s.AllowMods,
            allowVips = s.AllowVips,
            deleteOtherUrls = s.DeleteOtherUrls,
            deleteUrlCommands = s.DeleteUrlCommands,
            announceBlocked = s.AnnounceBlocked,
            logDeletedUrls = s.LogDeletedUrls,
            logSkipYouTube = s.LogSkipYouTube,
            logSkipSpotify = s.LogSkipSpotify,
            logFilePath = GetLogPath(),
            blockMessage = s.BlockMessage ?? "",
            allowUsers = s.AllowUsers ?? "",
            blockedSession = GetSessionInt("CometenUrlGuard_BlockedSession", 0),
            lastBlockedUser = GetSessionString("CometenUrlGuard_LastBlockedUser", ""),
            lastBlockedReason = GetSessionString("CometenUrlGuard_LastBlockedReason", ""),
            lastBlockedTime = GetSessionString("CometenUrlGuard_LastBlockedTime", ""),
            installedVersion = "1.2.0",
            result = result
        };

        CPH.WebsocketBroadcastJson(JsonConvert.SerializeObject(payload));
    }

    private string GetLogPath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "CWA_URL_Guard_Deleted_URLs.log");
    }

    private int GetSessionInt(string name, int fallback)
    {
        try
        {
            object value = CPH.GetGlobalVar<object>(name, false);
            int parsed;
            if (value != null && int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
                return parsed;
        }
        catch { }
        return fallback;
    }

    private string GetSessionString(string name, string fallback)
    {
        try
        {
            object value = CPH.GetGlobalVar<object>(name, false);
            return value == null ? fallback : value.ToString();
        }
        catch { return fallback; }
    }

}
