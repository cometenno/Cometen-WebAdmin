using System;
public class CPHInline
{
    public bool Execute()
    {
        string[] keys = { "CometenCredits_Follows", "CometenCredits_Cheers", "CometenCredits_Subs", "CometenCredits_Resubs", "CometenCredits_GiftSubs", "CometenCredits_GiftBombs", "CometenCredits_Raiders", "CometenCredits_Rewards", "CometenCredits_TopCheer", "CometenCredits_TopChannelPoints", "CometenCredits_ChatCrew" };
        foreach (string key in keys) CPH.SetGlobalVar(key, "", true);
        CPH.SendMessage("Credits have been reset for this stream.");
        return true;
    }
}
