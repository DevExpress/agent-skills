<#
.SYNOPSIS
    Starts the built app once and reports whether it survives startup.

.DESCRIPTION
    DI mistakes that compile can still throw while the first window loads (a dependency read by a base
    constructor before it is set, a view model the container cannot create). This script finds the built
    executable, starts it, waits, and prints one result line:
      RUNNING       <window titles>      the app is up and shows a window; it is closed again
      CRASHED       <exit code>          the process ended; the exception text follows
      ERROR-DIALOG  <title>              the app shows an error message instead of its window
      NO-WINDOW                          the process is alive but shows no window
      EXTERNAL      <file>: <match>      not started: the code reaches a server, network service or
                                         remote database; ask the user, then rerun with -AllowExternal
      AMBIGUOUS     <projects>           not started: several WPF app projects; rerun with -Project <name>
      NO-EXE                             no built executable found; build first
    Exit code: 0 for RUNNING, 2 for EXTERNAL, 1 otherwise.

.PARAMETER Path
    Project or solution folder.
.PARAMETER Project
    The WPF app project to start (its .csproj name without extension), when the folder has several.
.PARAMETER Exe
    The executable itself (for tools that know it; the skill passes -Project).
.PARAMETER Seconds
    How long the app must stay up. Defaults to 20.
.PARAMETER AllowExternal
    Start the app even though it uses external resources (only after the user agreed).
#>
param([string] $Path = '.', [string] $Project, [string] $Exe, [int] $Seconds = 20, [switch] $AllowExternal)

$Path = (Resolve-Path $Path).Path
$sources = Get-ChildItem $Path -Recurse -File -Include *.cs, *.config, appsettings*.json |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj|\.git)[\\/]' }

# External resources: servers in connection strings, client-server database providers, HTTP clients.
# Local files (SQLite, LocalDB, .mdf), placeholders and localhost do not count.
if (-not $AllowExternal) {
    $patterns = @(
        '(?i)"[^"\r\n]*?\b(?:Server|Host|Address|Addr)\s*=\s*(?!\(localdb\)|\.\\|\.;|localhost\b|127\.0\.0\.1|\{)[^;"''\s]+',
        '(?i)"[^"\r\n]*?\bData Source\s*=\s*(?!\(localdb\)|\||\{|:memory:|[^;"'']*\.(?:db|sqlite3?|mdf|sdf)\b|[^;"'']*DataDirectory)[^;"''\s{]+',
        '\bUse(?:SqlServer|Npgsql|MySql|Oracle|Cosmos)\s*\(',
        '\bnew\s+(?:HttpClient|WebClient|TcpClient|UdpClient|SmtpClient|MongoClient|NpgsqlConnection|MySqlConnection|OracleConnection|GrpcChannel)\s*\(',
        '\b(?:WebRequest|HttpWebRequest)\s*\.\s*Create(?:Http)?\s*\(',
        '\.\s*(?:Get|Post|Put|Patch|Delete|Send|Download|Upload)\w*\s*\(\s*\$?@?"https?://(?!localhost\b|127\.0\.0\.1)',
        '\bIHttpClientFactory\b',
        '\bAddHttpClient\b',
        'new\s+Uri\s*\(\s*"https?://(?!localhost\b|127\.0\.0\.1)'
    )
    $external = foreach ($f in $sources) {
        $text = Get-Content $f.FullName -Raw
        foreach ($p in $patterns) {
            foreach ($m in [regex]::Matches($text, $p)) {
                "EXTERNAL  $([System.IO.Path]::GetRelativePath($Path, $f.FullName)): $($m.Value.Trim().TrimStart('"'))"
            }
        }
    }
    if ($external) {
        $external | Sort-Object -Unique | Select-Object -First 10
        'Not started. Ask the user whether starting the app against these resources is safe, then rerun with -AllowExternal.'
        exit 2
    }
}

if (-not $Exe) {
    # The app is the WPF WinExe project (or the one named with -Project). Never guess between several:
    # a wrong pick would start an unrelated tool and report the app as checked.
    $apps = @(foreach ($proj in Get-ChildItem $Path -Recurse -File -Filter *.csproj | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }) {
        $xml = Get-Content $proj.FullName -Raw
        # WPF: <UseWPF> (SDK style) or the WPF project type / PresentationFramework reference (.NET Framework style)
        if ($xml -notmatch '(?i)<OutputType>\s*WinExe\s*</OutputType>' -or
            $xml -notmatch '(?i)<UseWPF>\s*true\s*</UseWPF>|60dc8134-eba5-43b8-bcc9-bb4bc16c2548|Include="PresentationFramework"') { continue }
        if ($Project -and $proj.BaseName -ne $Project) { continue }
        $name = [regex]::Match($xml, '<AssemblyName>\s*([^<]+?)\s*</AssemblyName>').Groups[1].Value
        [pscustomobject]@{ Project = $proj.BaseName; Dir = $proj.DirectoryName; Name = $name ? $name : $proj.BaseName }
    })
    if ($apps.Count -gt 1) {
        "AMBIGUOUS  several WPF app projects: $(($apps | ForEach-Object Project) -join ', ') - rerun with -Project <name of the app you migrated>"
        exit 1
    }
    $Exe = @(foreach ($app in $apps) {
        Get-ChildItem (Join-Path $app.Dir 'bin') -Recurse -File -Filter "$($app.Name).exe" -ErrorAction SilentlyContinue
    }) | Sort-Object LastWriteTime -Descending | Select-Object -First 1 -ExpandProperty FullName
}
if (-not $Exe -or -not (Test-Path $Exe)) { 'NO-EXE  build the project first, or name it with -Project'; exit 1 }

Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$started = Get-Date
$info = [System.Diagnostics.ProcessStartInfo]::new($Exe)
$info.UseShellExecute = $false
$info.WorkingDirectory = Split-Path $Exe
$info.RedirectStandardError = $true
$info.RedirectStandardOutput = $true
$process = [System.Diagnostics.Process]::Start($info)
$stderr = $process.StandardError.ReadToEndAsync()
$null = $process.StandardOutput.ReadToEndAsync()

# .NET Framework writes an unhandled exception to the event log, not to stderr
function Get-RuntimeError {
    Get-WinEvent -FilterHashtable @{ LogName = 'Application'; ProviderName = '.NET Runtime'; StartTime = $started } -ErrorAction SilentlyContinue |
        Where-Object { $_.Message -match [regex]::Escape([System.IO.Path]::GetFileName($Exe)) } |
        Select-Object -First 1 -ExpandProperty Message
}
# Messages in full, at most three stack frames after each: the inner exception names the real cause
function Show-Exception([string] $text) {
    $frames = 0
    $lines = foreach ($line in $text -split "`r?`n" | Where-Object { $_.Trim() }) {
        if ($line -match '^\s*at\s') { if (++$frames -le 3) { $line } }
        else { $frames = 0; $line }
    }
    ($lines | Select-Object -First 30) -join "`n"
}

try {
    if ($process.WaitForExit($Seconds * 1000)) {
        "CRASHED  exit code $($process.ExitCode) after $([int]((Get-Date) - $started).TotalSeconds) s"
        $text = $stderr.Wait(2000) ? $stderr.Result : ''
        if (-not $text.Trim()) { $text = Get-RuntimeError }
        if ($text) { Show-Exception $text } else { '(no exception text; if the app is meant to exit on its own, e.g. a second instance, ignore this)' }
        exit 1
    }
    $runtimeError = Get-RuntimeError
    if ($runtimeError) { 'CRASHED  unhandled exception (the process is still up behind an error report)'; Show-Exception $runtimeError; exit 1 }

    $windows = [System.Windows.Automation.AutomationElement]::RootElement.FindAll([System.Windows.Automation.TreeScope]::Children,
        [System.Windows.Automation.PropertyCondition]::new([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $process.Id)) |
        Where-Object { $_.Current.Name -and -not $_.Current.IsOffscreen }
    $errorWindow = $windows | Where-Object { $_.Current.ClassName -eq '#32770' -or $_.Current.Name -match '(?i)error|exception|unhandled' } | Select-Object -First 1
    if ($errorWindow) {
        # the message is a label's name or an editor's value (some dialogs show the stack trace in a text box)
        $texts = foreach ($e in $errorWindow.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)) {
            $pattern = $null
            if ($e.TryGetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern, [ref] $pattern)) { $pattern.Current.Value }
            elseif ($e.TryGetCurrentPattern([System.Windows.Automation.TextPattern]::Pattern, [ref] $pattern)) { $pattern.DocumentRange.GetText(4000) }
            if ($e.Current.ControlType -eq [System.Windows.Automation.ControlType]::Text) { $e.Current.Name }
        }
        "ERROR-DIALOG  '$($errorWindow.Current.Name)'"
        Show-Exception (($texts | Where-Object { $_ -and $_ -ne $errorWindow.Current.Name } | Select-Object -Unique) -join "`n")
        exit 1
    }
    if (-not $windows) { 'NO-WINDOW  the process is running but shows no window'; exit 1 }
    "RUNNING  $(($windows | ForEach-Object { "'$($_.Current.Name)'" }) -join ', ')"
    exit 0
}
finally {
    if (-not $process.HasExited) { $process.Kill($true) }
}
