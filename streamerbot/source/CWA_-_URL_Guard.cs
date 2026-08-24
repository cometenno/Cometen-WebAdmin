using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

// Cometen WebAdmin - URL Guard runtime v1.2.0
// Trigger: Twitch -> Chat -> Chat Message
// Never logs URL text.

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

    private UrlGuardSettings _settings;

    private static readonly Regex UrlRegex = new Regex(
        @"(?ix)(?:https?://|www\.)\S+|\b[a-z0-9](?:[a-z0-9-]{0,62}\.)+[a-z]{2,24}(?:[/?#]\S*)?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    public bool Execute()
    {
        _settings = LoadSettings();

        if (!_settings.Enabled)
            return true;

        string message = FirstArg("message", "rawInput").Trim();
        if (string.IsNullOrWhiteSpace(message) || !UrlRegex.IsMatch(message))
            return true;

        string msgId = FirstArg("msgId", "messageId").Trim();
        string user = FirstArg("userName", "user", "displayName").Trim();
        bool isCommand = message.TrimStart().StartsWith("!", StringComparison.Ordinal);

        // URL commands are handled first. Streamer.bot already has the event copy,
        // so deleting the Twitch message does not remove the command arguments.
        if (isCommand)
        {
            if (!_settings.DeleteUrlCommands)
                return true;

            if (!DeleteMessage(msgId))
            {
                CPH.LogWarn("[URL Guard] Could not delete URL command message. Check msgId/mod permissions.");
                return true;
            }

            LogDeletedUrls(message, user, "command");
            IncrementBlocked(user, "command");
            return true;
        }

        if (AllowedToPostOrdinaryUrl(user))
            return true;

        if (!_settings.DeleteOtherUrls)
            return true;

        if (!DeleteMessage(msgId))
        {
            CPH.LogWarn("[URL Guard] Could not delete URL message. Check msgId/mod permissions.");
            return true;
        }

        LogDeletedUrls(message, user, "ordinary");
        IncrementBlocked(user, "ordinary");

        if (_settings.AnnounceBlocked)
        {
            string template = string.IsNullOrWhiteSpace(_settings.BlockMessage)
                ? "@{user} lenker er kun tillatt for VIP/mods."
                : _settings.BlockMessage;

            string response = template.Replace("{user}", string.IsNullOrWhiteSpace(user) ? "viewer" : user);
            if (!string.IsNullOrWhiteSpace(response))
                CPH.SendMessage(response, true, true);
        }

        return true;
    }

    private bool AllowedToPostOrdinaryUrl(string user)
    {
        if (_settings.AllowBroadcaster &&
            BoolArg("isBroadcaster", "broadcaster"))
            return true;

        if (_settings.AllowMods &&
            BoolArg("isModerator", "isMod", "moderator"))
            return true;

        if (_settings.AllowVips &&
            BoolArg("isVip", "isVIP", "vip"))
            return true;

        string role = FirstArg("role").Trim().ToLowerInvariant();

        if (_settings.AllowBroadcaster && role.Contains("broadcaster"))
            return true;

        if (_settings.AllowMods &&
            (role.Contains("moderator") || role == "mod"))
            return true;

        if (_settings.AllowVips && role.Contains("vip"))
            return true;

        return IsExtraAllowedUser(user);
    }

    private bool IsExtraAllowedUser(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
            return false;

        string raw = _settings.AllowUsers ?? "";
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        string wanted = user.Trim().TrimStart('@').ToLowerInvariant();
        string[] parts = Regex.Split(raw, @"[\s,;]+");

        foreach (string part in parts)
        {
            string candidate = (part ?? "").Trim().TrimStart('@').ToLowerInvariant();
            if (candidate != "" && candidate == wanted)
                return true;
        }

        return false;
    }

    private bool DeleteMessage(string msgId)
    {
        if (string.IsNullOrWhiteSpace(msgId))
            return false;

        try
        {
            if (CPH.TwitchDeleteChatMessage(msgId, true))
                return true;
        }
        catch { }

        try
        {
            return CPH.TwitchDeleteChatMessage(msgId, false);
        }
        catch
        {
            return false;
        }
    }

    private void LogDeletedUrls(string message, string user, string reason)
    {
        if (!_settings.LogDeletedUrls)
            return;

        try
        {
            MatchCollection matches = UrlRegex.Matches(message ?? "");
            if (matches == null || matches.Count == 0)
                return;

            string path = GetLogPath();
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            foreach (Match match in matches)
            {
                string url = match == null ? "" : (match.Value ?? "").Trim();
                if (string.IsNullOrWhiteSpace(url))
                    continue;

                if (ShouldSkipLog(url))
                    continue;

                string line =
                    DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture) +
                    "\t" + SanitizeLogField(user) +
                    "\t" + SanitizeLogField(reason) +
                    "\t" + SanitizeLogField(url);

                File.AppendAllText(path, line + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
            CPH.LogWarn("[URL Guard] Could not write deleted URL log: " + ex.Message);
        }
    }

    private bool ShouldSkipLog(string url)
    {
        string host = GetHost(url);
        if (string.IsNullOrWhiteSpace(host))
            return false;

        if (_settings.LogSkipYouTube && IsYouTubeHost(host))
            return true;

        if (_settings.LogSkipSpotify && IsSpotifyHost(host))
            return true;

        return false;
    }

    private string GetHost(string url)
    {
        try
        {
            string candidate = (url ?? "").Trim();
            if (!candidate.Contains("://"))
                candidate = "https://" + candidate;

            Uri uri;
            if (!Uri.TryCreate(candidate, UriKind.Absolute, out uri))
                return "";

            string host = (uri.Host ?? "").Trim().ToLowerInvariant();
            if (host.StartsWith("www.", StringComparison.Ordinal))
                host = host.Substring(4);

            return host;
        }
        catch { return ""; }
    }

    private bool IsYouTubeHost(string host)
    {
        return host == "youtube.com" ||
               host.EndsWith(".youtube.com", StringComparison.Ordinal) ||
               host == "youtu.be" ||
               host == "youtube-nocookie.com" ||
               host.EndsWith(".youtube-nocookie.com", StringComparison.Ordinal);
    }

    private bool IsSpotifyHost(string host)
    {
        return host == "spotify.com" ||
               host.EndsWith(".spotify.com", StringComparison.Ordinal) ||
               host == "spotify.link" ||
               host.EndsWith(".spotify.link", StringComparison.Ordinal) ||
               host == "spoti.fi";
    }

    private string GetLogPath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "CWA_URL_Guard_Deleted_URLs.log");
    }

    private string SanitizeLogField(string value)
    {
        return (value ?? "").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
    }

    private void IncrementBlocked(string user, string reason)
    {
        int count = GetSessionInt("CometenUrlGuard_BlockedSession", 0) + 1;
        CPH.SetGlobalVar("CometenUrlGuard_BlockedSession", count.ToString(CultureInfo.InvariantCulture), false);
        CPH.SetGlobalVar("CometenUrlGuard_LastBlockedUser", user ?? "", false);
        CPH.SetGlobalVar("CometenUrlGuard_LastBlockedReason", reason ?? "", false);
        CPH.SetGlobalVar(
            "CometenUrlGuard_LastBlockedTime",
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            false
        );
    }

    private bool BoolArg(params string[] names)
    {
        foreach (string name in names)
        {
            object value;
            if (args.TryGetValue(name, out value) && value != null)
            {
                bool parsed;
                if (bool.TryParse(value.ToString(), out parsed))
                    return parsed;

                string text = value.ToString().Trim().ToLowerInvariant();
                if (text == "1" || text == "yes" || text == "on")
                    return true;
            }
        }
        return false;
    }

    private string FirstArg(params string[] names)
    {
        foreach (string name in names)
        {
            object value;
            if (args.TryGetValue(name, out value) && value != null)
            {
                string text = value.ToString();
                if (!string.IsNullOrWhiteSpace(text))
                    return text.Trim();
            }
        }
        return "";
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

        UrlGuardSettings s = Defaults();
        s.Enabled = ReadLegacyBool("CometenUrlGuard_Enabled", s.Enabled);
        s.AllowBroadcaster = ReadLegacyBool("CometenUrlGuard_AllowBroadcaster", s.AllowBroadcaster);
        s.AllowMods = ReadLegacyBool("CometenUrlGuard_AllowMods", s.AllowMods);
        s.AllowVips = ReadLegacyBool("CometenUrlGuard_AllowVips", s.AllowVips);
        s.DeleteOtherUrls = ReadLegacyBool("CometenUrlGuard_DeleteOtherUrls", s.DeleteOtherUrls);
        s.DeleteUrlCommands = ReadLegacyBool("CometenUrlGuard_DeleteUrlCommands", s.DeleteUrlCommands);
        s.AnnounceBlocked = ReadLegacyBool("CometenUrlGuard_AnnounceBlocked", s.AnnounceBlocked);
        s.LogDeletedUrls = ReadLegacyBool("CometenUrlGuard_LogDeletedUrls", s.LogDeletedUrls);
        s.LogSkipYouTube = ReadLegacyBool("CometenUrlGuard_LogSkipYouTube", s.LogSkipYouTube);
        s.LogSkipSpotify = ReadLegacyBool("CometenUrlGuard_LogSkipSpotify", s.LogSkipSpotify);
        s.BlockMessage = ReadLegacyString("CometenUrlGuard_BlockMessage", s.BlockMessage);
        s.AllowUsers = ReadLegacyString("CometenUrlGuard_AllowUsers", s.AllowUsers);

        try
        {
            CPH.SetGlobalVar(SettingsVariable, JsonConvert.SerializeObject(s), true);
        }
        catch { }

        return s;
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

    private int GetSessionInt(string name, int fallback)
    {
        try
        {
            object value = CPH.GetGlobalVar<object>(name, false);
            int parsed;
            if (value != null && int.TryParse(
                value.ToString(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out parsed))
                return parsed;
        }
        catch { }

        return fallback;
    }
}
