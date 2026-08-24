using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        // Discord currently requires modern TLS.
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

        bool forceTest = GetBoolArg("forceTest", false);
        bool testMode = forceTest || GetBoolArg(
            "testMode",
            GetBoolGlobal("CometenLive_TestMode", false)
        );

        CPH.LogInfo("[CWA Live] Send action started. Test mode: " + testMode);

        string webhook = GetArgString("discordWebhookUrl", "");
        if (string.IsNullOrWhiteSpace(webhook))
            webhook = GetGlobal("CometenLive_DiscordWebhookUrl", "");

        if (string.IsNullOrWhiteSpace(webhook))
        {
            Finish("No Discord webhook saved.", "send-failed", true);
            return true;
        }

        bool ignoreCooldown = forceTest || GetBoolArg(
            "testIgnoreCooldown",
            GetBoolGlobal("CometenLive_TestIgnoreCooldown", false)
        );

        int cooldownMinutes = GetIntArg(
            "cooldownMinutes",
            GetIntGlobal("CometenLive_CooldownMinutes", 180)
        );

        if (cooldownMinutes < 0)
            cooldownMinutes = 0;

        if (!ignoreCooldown && cooldownMinutes > 0 && IsInCooldown(cooldownMinutes))
        {
            Finish("Skipped by cooldown.", "cooldown", false);
            return true;
        }

        string role = GetArgString(
            "roleMention",
            GetGlobal("CometenLive_DiscordRoleMention", "@Twitch")
        );

        string url = GetArgString(
            "twitchUrl",
            GetGlobal("CometenLive_TwitchUrl", "https://twitch.tv/<TWITCH_CHANNEL>")
        );

        string botName = GetArgString(
            "botName",
            GetGlobal("CometenLive_BotName", "Cometen Live")
        );

        string template = GetArgString(
            "messageTemplate",
            GetGlobal(
                "CometenLive_MessageTemplate",
                "🔴 {role} Cometen is LIVE!\n\n{custom}{title}\n\nPlaying: {game}\n\nCome hang out:\n{url}"
            )
        );

        string customMessage = GetArgString(
            "customMessage",
            GetGlobal("CometenLive_CustomMessage", "")
        );

        string title;
        string game;

        if (testMode)
        {
            title = GetArgString(
                "testTitle",
                GetGlobal("CometenLive_TestTitle", "")
            );

            game = GetArgString(
                "testGame",
                GetGlobal("CometenLive_TestGame", "The Division 4")
            );
        }
        else
        {
            // targetChannelTitle is populated by:
            // Twitch -> User -> Get User Info for Target
            title = FirstNonEmpty(
                GetArgString("targetChannelTitle", ""),
                GetArgString("title", ""),
                GetArgString("streamTitle", ""),
                GetArgString("broadcastTitle", ""),
                GetArgString("channelTitle", ""),
                GetArgString("status", ""),
                "Cometen is LIVE!"
            );

            game = FirstNonEmpty(
                GetArgString("game", ""),
                GetArgString("gameName", ""),
                GetArgString("category", ""),
                GetArgString("targetGame", ""),
                "Gaming"
            );
        }

        CPH.LogInfo("[CWA Live] Resolved title: " + title);
        CPH.LogInfo("[CWA Live] Resolved game: " + game);

        string renderedTemplate = template.Replace("\\n", "\n");
        string customBlock = string.IsNullOrWhiteSpace(customMessage)
            ? ""
            : customMessage.Trim() + "\n\n";

        // Keep older saved templates compatible. If {custom} is missing,
        // insert the optional one-stream text directly before {title}.
        if (!renderedTemplate.Contains("{custom}") && !string.IsNullOrWhiteSpace(customBlock))
        {
            int titleTokenIndex = renderedTemplate.IndexOf("{title}", StringComparison.Ordinal);
            if (titleTokenIndex >= 0)
                renderedTemplate = renderedTemplate.Insert(titleTokenIndex, "{custom}");
            else
                renderedTemplate += "\n\n{custom}";
        }

        string content = renderedTemplate
            .Replace("{custom}", customBlock)
            .Replace("{role}", role ?? "")
            .Replace("{title}", title ?? "")
            .Replace("{game}", game ?? "")
            .Replace("{url}", url ?? "");

        if (content.Length > 2000)
        {
            Finish(
                "Discord message is too long: " + content.Length + " characters.",
                "send-failed",
                true
            );
            return true;
        }

        try
        {
            string discordJson = JsonConvert.SerializeObject(new
            {
                content = content,
                username = botName
            });

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(20);

                using (StringContent body = new StringContent(
                    discordJson,
                    Encoding.UTF8,
                    "application/json"
                ))
                {
                    HttpResponseMessage response = httpClient
                        .PostAsync(webhook, body)
                        .GetAwaiter()
                        .GetResult();

                    string responseBody = response.Content
                        .ReadAsStringAsync()
                        .GetAwaiter()
                        .GetResult();

                    if (!response.IsSuccessStatusCode)
                    {
                        string details = CleanResponseBody(responseBody);
                        string result =
                            "Discord failed: " +
                            ((int)response.StatusCode) +
                            " " +
                            response.ReasonPhrase;

                        if (!string.IsNullOrWhiteSpace(details))
                            result += " - " + details;

                        Finish(result, "send-failed", true);
                        return true;
                    }
                }
            }

            // A test never consumes the prepared text. A real message clears it only after Discord accepted the post.
            if (!testMode)
                CPH.SetGlobalVar("CometenLive_CustomMessage", "", true);

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            CPH.SetGlobalVar(
                "CometenLive_LastSentUnix",
                now.ToString(),
                true
            );

            Finish(
                testMode
                    ? "Test announcement sent."
                    : "Announcement sent.",
                testMode ? "test-sent" : "sent",
                false
            );
        }
        catch (Exception ex)
        {
            Finish("Error: " + ex.Message, "send-error", true);
        }

        return true;
    }

    private void Finish(string result, string statusAction, bool isError)
    {
        CPH.SetGlobalVar("CometenLive_LastResult", result, true);
        BroadcastStatus(statusAction);

        if (isError)
            CPH.LogError("[CWA Live] " + result);
        else
            CPH.LogInfo("[CWA Live] " + result);
    }

    private string CleanResponseBody(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        value = value
            .Replace("\r", " ")
            .Replace("\n", " ")
            .Trim();

        if (value.Length > 500)
            value = value.Substring(0, 500);

        return value;
    }

    private bool IsInCooldown(int cooldownMinutes)
    {
        long last;

        if (!long.TryParse(
            GetGlobal("CometenLive_LastSentUnix", "0"),
            out last
        ))
            return false;

        if (last <= 0)
            return false;

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long cooldownSeconds = cooldownMinutes * 60L;

        return (now - last) < cooldownSeconds;
    }

    private string FirstNonEmpty(params string[] values)
    {
        foreach (string value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return "";
    }

    private string GetArgString(string name, string fallback)
    {
        try
        {
            string value;

            if (CPH.TryGetArg(name, out value) && value != null)
                return value;
        }
        catch { }

        return fallback;
    }

    private int GetIntArg(string name, int fallback)
    {
        int value;

        if (int.TryParse(GetArgString(name, ""), out value))
            return value;

        return fallback;
    }

    private bool GetBoolArg(string name, bool fallback)
    {
        return ParseBool(GetArgString(name, ""), fallback);
    }

    private string GetGlobal(string key, string fallback)
    {
        try
        {
            string value = CPH.GetGlobalVar<string>(key, true);

            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }
        catch { }

        return fallback;
    }

    private int GetIntGlobal(string key, int fallback)
    {
        int value;

        if (int.TryParse(GetGlobal(key, ""), out value))
            return value;

        return fallback;
    }

    private bool GetBoolGlobal(string key, bool fallback)
    {
        return ParseBool(GetGlobal(key, ""), fallback);
    }

    private bool ParseBool(string value, bool fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback;

        value = value.Trim().ToLowerInvariant();

        if (value == "true" || value == "yes" || value == "1")
            return true;

        if (value == "false" || value == "no" || value == "0")
            return false;

        return fallback;
    }

    private void BroadcastStatus(string action)
    {
        string webhook = GetGlobal("CometenLive_DiscordWebhookUrl", "");
        string lastSentUnix = GetGlobal("CometenLive_LastSentUnix", "0");
        string lastSentLocal = "";

        long unix;

        if (long.TryParse(lastSentUnix, out unix) && unix > 0)
        {
            try
            {
                lastSentLocal = DateTimeOffset
                    .FromUnixTimeSeconds(unix)
                    .LocalDateTime
                    .ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch { }
        }

        string json = JsonConvert.SerializeObject(new
        {
            source = "CometenWebAdmin",
            type = "LIVE_SETTINGS",
            action = action,
            webhookSet = !string.IsNullOrWhiteSpace(webhook),
            roleMention = GetGlobal(
                "CometenLive_DiscordRoleMention",
                "@Twitch"
            ),
            messageTemplate = GetGlobal(
                "CometenLive_MessageTemplate",
                "🔴 {role} Cometen is LIVE!\n\n{custom}{title}\n\nPlaying: {game}\n\nCome hang out:\n{url}"
            ),
            customMessage = GetGlobal("CometenLive_CustomMessage", ""),
            twitchUrl = GetGlobal(
                "CometenLive_TwitchUrl",
                "https://twitch.tv/<TWITCH_CHANNEL>"
            ),
            botName = GetGlobal(
                "CometenLive_BotName",
                "Cometen Live"
            ),
            cooldownMinutes = GetGlobal(
                "CometenLive_CooldownMinutes",
                "180"
            ),
            testMode = GetGlobal(
                "CometenLive_TestMode",
                "False"
            ),
            testGame = GetGlobal(
                "CometenLive_TestGame",
                "The Division 4"
            ),
            testTitle = GetGlobal(
                "CometenLive_TestTitle",
                ""
            ),
            testIgnoreCooldown = GetGlobal(
                "CometenLive_TestIgnoreCooldown",
                "False"
            ),
            lastSentUnix = lastSentUnix,
            lastSentLocal = lastSentLocal,
            lastResult = GetGlobal(
                "CometenLive_LastResult",
                ""
            )
        });

        CPH.WebsocketBroadcastJson(json);
    }
}