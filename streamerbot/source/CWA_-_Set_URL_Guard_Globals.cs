using System;
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
        bool forceReset = GetArgBool("forceReset", false);

        UrlGuardSettings s;
        if (forceReset)
        {
            s = Defaults();
            SaveSettingsState(s);
            CPH.LogInfo("[Cometen WebAdmin] URL Guard JSON settings reset to defaults.");
        }
        else
        {
            s = LoadSettings();
            SaveSettingsState(s);
            CPH.LogInfo("[Cometen WebAdmin] URL Guard JSON settings initialized/preserved.");
        }

        try { CPH.RunAction("CWA - URL Guard Status", true); } catch { }
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

}
