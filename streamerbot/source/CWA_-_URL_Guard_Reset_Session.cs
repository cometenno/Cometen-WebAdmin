using System;

public class CPHInline
{
    public bool Execute()
    {
        CPH.UnsetGlobalVar("CometenUrlGuard_BlockedSession", false);
        CPH.UnsetGlobalVar("CometenUrlGuard_LastBlockedUser", false);
        CPH.UnsetGlobalVar("CometenUrlGuard_LastBlockedReason", false);
        CPH.UnsetGlobalVar("CometenUrlGuard_LastBlockedTime", false);

        CPH.LogInfo("[Cometen WebAdmin] URL Guard session stats reset.");

        try { CPH.RunAction("CWA - URL Guard Status", true); } catch { }
        return true;
    }
}
