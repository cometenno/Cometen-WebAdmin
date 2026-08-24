using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {
        // Nullstill kun den midlertidige chatterhistorikken.
        CPH.UnsetGlobalVar("CometenChatPip_State", false);

        string soundFile = ReadString("CometenChatPip_SoundFile");

        // Alle verdiene leses direkte fra Persisted Globals.
        // Ingen innstillingsverdier er hardkodet her.
        var payload = new
        {
            source = "CometenWebAdmin",
            type = "CHAT_PIP_SETTINGS",

            enabled = ReadBool("CometenChatPip_Enabled"),
            newChatterEnabled = ReadBool("CometenChatPip_NewChatterEnabled"),

            quietChatEnabled = ReadBool("CometenChatPip_QuietChatEnabled"),
            quietMinutes = ReadInt("CometenChatPip_QuietMinutes"),

            returningUserEnabled = ReadBool("CometenChatPip_ReturningUserEnabled"),
            returnMinutes = ReadInt("CometenChatPip_ReturnMinutes"),

            soundFile = soundFile,
            volume = ReadInt("CometenChatPip_Volume"),

            ignoreUsers = ReadString("CometenChatPip_IgnoreUsers"),
            ignoreBroadcaster = ReadBool("CometenChatPip_IgnoreBroadcaster"),
            resetOnStreamStart = ReadBool("CometenChatPip_ResetOnStreamStart"),

            lastUser = ReadString("CometenChatPip_LastUser"),
            lastReason = ReadString("CometenChatPip_LastReason"),
            lastTime = ReadString("CometenChatPip_LastTime"),

            trackedUsers = ReadInt("CometenChatPip_TrackedUsers"),
            installedVersion = ReadString("CometenChatPip_InstalledVersion"),

            soundFileExists =
                !string.IsNullOrWhiteSpace(soundFile) &&
                File.Exists(soundFile)
        };

        string json = JsonConvert.SerializeObject(payload);
        CPH.WebsocketBroadcastJson(json);

        CPH.LogInfo(
            "[Chat Pip] Globals read and sent to WebAdmin. SoundFile=" +
            soundFile
        );

        return true;
    }

    private string ReadString(string name)
    {
        object value = CPH.GetGlobalVar<object>(name, true);

        if (value == null)
        {
            return "";
        }

        return Convert.ToString(
            value,
            CultureInfo.InvariantCulture
        ) ?? "";
    }

    private int ReadInt(string name)
    {
        string value = ReadString(name);
        int result;

        if (!int.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out result))
        {
            throw new InvalidOperationException(
                "Chat Pip global is missing or invalid: " +
                name +
                " = '" +
                value +
                "'"
            );
        }

        return result;
    }

    private bool ReadBool(string name)
    {
        string value = ReadString(name).Trim();
        bool result;

        if (bool.TryParse(value, out result))
        {
            return result;
        }

        if (value == "1" || value.Equals("yes", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (value == "0" || value.Equals("no", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        throw new InvalidOperationException(
            "Chat Pip global is missing or invalid: " +
            name +
            " = '" +
            value +
            "'"
        );
    }
}