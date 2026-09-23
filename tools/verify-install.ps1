param(
    [string]$StreamerBotHost = "127.0.0.1",
    [int]$WebSocketPort = 8081,
    [int]$HttpPort = 7474
)

$ErrorActionPreference = "Continue"
$Root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)

$Expected = @(
    "Cometen_WebAdmin.html",
    "Cometen_WebAdmin_1.0.sb",
    "Cometen Chat Overlay.html",
    "cometen_credits_CWA.html",
    "chat-pip.mp3",
    "alerts\alerts.html",
    "alerts\irl-forward.js"
)

Write-Host "Cometen WebAdmin 1.0 verification"
Write-Host "Root: $Root"

$Missing = @()
foreach ($Relative in $Expected) {
    $Path = Join-Path $Root $Relative
    if (Test-Path $Path) { Write-Host "[OK]   $Relative" }
    else { Write-Host "[MISS] $Relative"; $Missing += $Relative }
}

foreach ($Port in @($WebSocketPort, $HttpPort)) {
    try {
        $Result = Test-NetConnection -ComputerName $StreamerBotHost -Port $Port -WarningAction SilentlyContinue
        if ($Result.TcpTestSucceeded) { Write-Host "[OK]   TCP $StreamerBotHost`:$Port" }
        else { Write-Host "[WARN] TCP $StreamerBotHost`:$Port not reachable" }
    } catch {
        Write-Host "[WARN] Could not test TCP $StreamerBotHost`:$Port"
    }
}

if ($Missing.Count -eq 0) { exit 0 }
exit 1
