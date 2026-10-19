#Requires -Version 7.0

<#
.SYNOPSIS
  Phase 4 verification for a rendered DevExpress ASP.NET Core reporting page — Document Viewer,
  End-User Report Designer, or standalone Report Parameters Panel.

.DESCRIPTION
  Fetches the rendered page and mechanically checks the things a human would check in browser
  DevTools, plus things DevTools does not make obvious:

    1. HTTP status of every <script src> and <link rel="stylesheet"> the page references.
    2. That every client library the chosen component requires is actually present.
    3. That the libraries satisfy every real load-order dependency (SKILL.md Constraint 2).
       Genuinely independent pairs (Knockout vs. Bootstrap) are not order-checked.
    4. That no library is loaded twice (e.g. viewer assets also loaded on the designer page ->
       'Component dx-filtereditor-plain is already registered').
    5. That the component's init script actually rendered.
    6. That no literal, un-evaluated Razor fluent-call text (`.Bind(...)`, `.Height(...)`) leaked
       into the page body — the signature of a multi-line Html.DevExpress() chain that was not
       wrapped in `@(...)`.
    7. That every asset a stylesheet references INTERNALLY (`url(...)` — icon fonts, images)
       actually resolves. CSS resolves those against the STYLESHEET's own URL, not the page's, so
       a theme CSS served from /css/devextreme/ asks for /css/devextreme/icons/dxicons.woff2, and
       a font copied to /css/icons/ 404s even though every <link>/<script> tag is 200 and the file
       exists on disk.

  BUNDLE-AWARE. Libraries are identified by scanning the *content* of each fetched resource for
  that library's banner signature, not by guessing from the file name, so this works whether the
  page uses direct <script> references or bundled files. Ordering uses a composite key of
  (position of the script tag in the document, byte offset of the library inside that file), so
  mis-ordering *within* a single bundle is caught too.

  LIMITATION — minified bundles. Banner comments survive concatenation but a minifier that strips
  `/** ... */` comments removes them. When a resource cannot be identified AND a required library
  appears missing, the script exits INCONCLUSIVE (exit code 2) rather than claiming success or
  failure — verify in a real browser in that case.

  LAUNCH MODE (-ProjectDir). Prefer this over -Url. The script starts the app itself, checks the
  port is free before launching and that the launched process stays alive on every poll, and
  stops only the process it started — so a dead launch reports INCONCLUSIVE instead of silently
  attaching to whatever else answers on that port. The RESULT block carries the provenance line
  ("Launched by script: pid N, bound after N s on attempt N"), so a verbatim paste always states which
  server was verified.

  WHAT THIS SCRIPT CANNOT DO. It fetches and parses page text; it does NOT execute JavaScript. It
  reads the init script's "callbacks" map, so it DOES fail a handler that is declared below the
  init script (a guaranteed ReferenceError at init), and WARNS when a handler has no declaration
  anywhere in the page HTML — only a warning, because the handler may legitimately live in an
  external .js file this script never opens. A handler that runs and throws, or that runs and does
  the WRONG thing (wrong ActionId / ExportFormatID), produces the same PASS as a correct one.
  Whenever handlers are present the output says the browser check is required and, without
  -KeepAlive, that it was NOT done — 'handler behaviour' is named in the VERIFIED stamp's
  NOT-verified list. Use -KeepAlive and read the real console with a browser-automation tool.
  Full URL of an ALREADY-RUNNING page, e.g. http://localhost:5276/Reports/Viewer.
  Mutually exclusive with -ProjectDir. The RESULT block is stamped to say the server was not
  launched by this script.

.PARAMETER ProjectDir
  Project DIRECTORY (not the .csproj) to `dotnet run` -- which builds and executes it, so point
  it only at a project you trust. Passed via -WorkingDirectory, so a path containing spaces is
  safe. Requires -Route.

.PARAMETER Route
  Page route to verify in launch mode, e.g. /Reports/Viewer

.PARAMETER Port
  Port for launch mode. Default 0 = pick a free one automatically.

.PARAMETER Component
  Which control the page hosts: Viewer, Designer, ParametersPanel, or Auto (default).
  Auto detects it from the control constructor in the page's inline script.

.PARAMETER Strict
  Promote warnings (missing Bootstrap/ace, a handler with no declaration in the page) to
  failures.

.PARAMETER KeepAlive
  Launch mode only. Leave the launched server RUNNING after the static checks, and print its URL
  and PID, so a browser-automation MCP (Chrome DevTools MCP, Playwright MCP, BrowserTools MCP, or
  similar) can navigate to it and read the console -- the only way to catch a client-side handler
  that is undefined or throwing. THE CALLER MUST STOP THE PRINTED PID when finished; the script
  prints the exact command. Early-exit paths (launch failed, never bound) always tear down, so a
  dead launch is never left running.

.PARAMETER TimeoutSec
  Per-request timeout. Default 100.

.PARAMETER StartupTimeoutSec
  Wall-clock seconds to wait for the app to bind in launch mode. Default 60.

.EXAMPLE
  ./Verify-ReportingPage.ps1 -ProjectDir 'C:\src\My App\Web' -Route /Reports/Viewer

.EXAMPLE
  ./Verify-ReportingPage.ps1 -ProjectDir 'C:\src\My App\Web' -Route /Reports/Designer -Component Designer

.EXAMPLE
  ./Verify-ReportingPage.ps1 -Url http://localhost:5276/Reports/Viewer

.OUTPUTS
  Exit code 0 = PASS, 1 = FAIL, 2 = INCONCLUSIVE (unidentifiable resources, the app never
  started / the port was already taken, or the route did not render (401/403 or redirect) -- neither of which is success).
#>
[CmdletBinding(DefaultParameterSetName = 'Url')]
param(
    [Parameter(Mandatory = $true, ParameterSetName = 'Url')][string]$Url,
    [Parameter(Mandatory = $true, ParameterSetName = 'Launch')][string]$ProjectDir,
    [Parameter(Mandatory = $true, ParameterSetName = 'Launch')][string]$Route,
    [Parameter(ParameterSetName = 'Launch')][int]$Port = 0,
    [ValidateSet('Auto', 'Viewer', 'Designer', 'ParametersPanel')]
    [string]$Component = 'Auto',
    [switch]$Strict,
    [switch]$KeepAlive,
    [int]$TimeoutSec = 100,
    [int]$StartupTimeoutSec = 60
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$failures = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()
$notes    = [System.Collections.Generic.List[string]]::new()

# ---------------------------------------------------------------------------------------------
# Library signature tables.
#
# Banner = the file's own header comment. Code = a fallback identifier that survives minification.
# Only signatures verified unique to one library are listed; public API names such as
# `bindingHandlers` and `dxDataGrid` are intentionally absent — they also appear inside the
# DevExpress reporting files and would produce phantom duplicates.
# ---------------------------------------------------------------------------------------------
$JsLibraries = [ordered]@{
    jquery       = @{ Name = 'jQuery';               Banner = @('jquery.org/license');                    Code = @('jquery:') }
    knockout     = @{ Name = 'Knockout';             Banner = @('Knockout JavaScript library');            Code = @('__ko__', 'knockoutjs.com') }
    bootstrap    = @{ Name = 'Bootstrap';            Banner = @('getbootstrap.com');                       Code = @('bs.modal') }
    ace          = @{ Name = 'ace-builds';           Banner = @('ACE_NAMESPACE');                          Code = @('ACE_NAMESPACE') }
    # ace module ids ('ace/ext/language_tools', etc.) also appear as string literals inside
    # dx-analytics-core.min.js, so those obvious ids are not usable as detectors here.
    aceLangTools = @{ Name = 'ace ext-language_tools'; Banner = @('snippetCompleter');                     Code = @('snippetCompleter') }
    aceThemeDw   = @{ Name = 'ace theme-dreamweaver'; Banner = @('ace-dreamweaver .ace_gutter');           Code = @('ace-dreamweaver .ace_gutter') }
    aceThemeAmb  = @{ Name = 'ace theme-ambiance';    Banner = @('ace-ambiance .ace_gutter');              Code = @('ace-ambiance .ace_gutter') }
    aceSnippets  = @{ Name = 'ace snippets/text';     Banner = @('ace/snippets/text');                     Code = @('ace/snippets/text') }
    devextreme   = @{ Name = 'DevExtreme (dx.all)';  Banner = @('DevExtreme (dx.all.js)');                 Code = @() }
    analytics    = @{ Name = 'dx-analytics-core';    Banner = @('dx-analytics-core.min.js', 'dx-analytics-core.js'); Code = @('dx-filtereditor-plain') }
    viewer       = @{ Name = 'dx-webdocumentviewer'; Banner = @('dx-webdocumentviewer.min.js', 'dx-webdocumentviewer.js'); Code = @('DxReportViewer') }
    querybuilder = @{ Name = 'dx-querybuilder';      Banner = @('dx-querybuilder.min.js', 'dx-querybuilder.js'); Code = @('DxQueryBuilder') }
    designer     = @{ Name = 'dx-reportdesigner';    Banner = @('dx-reportdesigner.min.js', 'dx-reportdesigner.js'); Code = @('DxReportDesigner') }
}

$CssLibraries = [ordered]@{
    bootstrapCss   = @{ Name = 'bootstrap.css';            Banner = @('getbootstrap.com') }
    dxTheme        = @{ Name = 'dx.<theme>.css';           Banner = @('DevExtreme (dx.') }
    analyticsCommon= @{ Name = 'dx-analytics.common.css';  Banner = @('dx-analytics.common.css') }
    analyticsTheme = @{ Name = 'dx-analytics.<theme>.css'
                        Banner = @('dx-analytics.light.', 'dx-analytics.dark.', 'dx-analytics.contrast.',
                                   'dx-analytics.carmine.', 'dx-analytics.darkmoon.', 'dx-analytics.softblue.',
                                   'dx-analytics.darkviolet.', 'dx-analytics.greenmist.', 'dx-analytics.material.') }
    viewerCss      = @{ Name = 'dx-webdocumentviewer.css'; Banner = @('dx-webdocumentviewer.css') }
    querybuilderCss= @{ Name = 'dx-querybuilder.css';      Banner = @('dx-querybuilder.css') }
    designerCss    = @{ Name = 'dx-reportdesigner.css';    Banner = @('dx-reportdesigner.css') }
    # '.ace_editor' is NOT usable as a marker — it also appears in dx-analytics.common.css,
    # dx-analytics.light.css and dx-webdocumentviewer.css.
    aceCss         = @{ Name = 'ace.css';                  Banner = @('ace_br1') }
    aceCssDw       = @{ Name = 'ace dreamweaver.css';      Banner = @('ace-dreamweaver') }
    aceCssAmb      = @{ Name = 'ace ambiance.css';         Banner = @('ace-ambiance') }
}

# Load-order constraints as real dependency PAIRS rather than one total order — mirroring
# SKILL.md Constraint 2. Not every adjacent pair in the templates' chain is an actual dependency
# (Bootstrap and Knockout are independent), so only genuine pairs are listed. Each maps to a
# concrete runtime failure documented in the skill.
$LoadDependencies = @(
    @{ Before = 'jquery';       After = 'knockout';     Because = "Knockout binds to jQuery at load time if it is present" }
    @{ Before = 'jquery';       After = 'bootstrap';    Because = "Bootstrap 4 requires jQuery" }
    @{ Before = 'jquery';       After = 'devextreme';   Because = "DevExtreme's jQuery integration registers widgets at load time" }
    @{ Before = 'knockout';     After = 'devextreme';   Because = "otherwise: 'ko is not defined' / 'DevExtreme library is included before Knockout'" }
    @{ Before = 'ace';          After = 'devextreme';   Because = "otherwise the Designer's code editors fail to initialize" }
    @{ Before = 'devextreme';   After = 'analytics';    Because = "otherwise: 'DevExpress is not defined'" }
    @{ Before = 'analytics';    After = 'viewer';       Because = "otherwise: 'Unable to process binding'" }
    @{ Before = 'analytics';    After = 'querybuilder'; Because = "otherwise: 'Unable to process binding'" }
    @{ Before = 'viewer';       After = 'designer';     Because = "the Designer's Preview reuses the Viewer's components" }
    @{ Before = 'querybuilder'; After = 'designer';     Because = "the Designer's Data Source Wizard embeds the Query Builder" }
)

# Per-component requirements.
$ComponentSpecs = @{
    Viewer          = @{ Ctor = 'DxReportViewer';           RequiredJs = @('jquery','knockout','devextreme','analytics','viewer');
                         ExpectedJs = @('bootstrap'); RequiredCss = @('dxTheme','analyticsCommon','analyticsTheme','viewerCss') }
    Designer        = @{ Ctor = 'DxReportDesigner';         RequiredJs = @('jquery','knockout','devextreme','analytics','viewer','querybuilder','designer');
                         ExpectedJs = @('bootstrap','ace'); RequiredCss = @('dxTheme','analyticsCommon','analyticsTheme','viewerCss','querybuilderCss','designerCss') }
    ParametersPanel = @{ Ctor = 'DxReportParametersPanel';  RequiredJs = @('jquery','knockout','devextreme','analytics','viewer');
                         ExpectedJs = @('bootstrap'); RequiredCss = @('dxTheme','analyticsCommon','analyticsTheme','viewerCss') }
}

# ---------------------------------------------------------------------------------------------
# 0. Launch mode
# ---------------------------------------------------------------------------------------------
# Owns the app's lifetime so the page verified is provably the one this run built:
#   a) the target port must be FREE before launching, or a foreign server would answer instead.
#   b) the launched process must still be ALIVE on every poll; a dead launch reports INCONCLUSIVE.
$script:launched   = $null   # process we started; $null in -Url mode
$script:launchStamp = $null  # provenance line, printed inside the RESULT block
$script:launchLog  = $null
$script:launchLogDir = $null # per-run temp directory holding the redirected stdout/stderr
$script:launchPort = 0       # port we bound; used to identify the apphost at teardown
$script:launchTime = $null   # taken BEFORE Start-Process; anything older cannot be ours
$script:keepRunning = $false # set only by -KeepAlive, so the teardown guard skips this run

function Stop-LaunchedApp {
    if ($null -eq $script:launched) { return }
    $procId = $script:launched.Id
    $script:launched = $null

    # `dotnet run` spawns an apphost named after the project, which survives a parent-only kill
    # and keeps bin/ locked and the port bound if not also terminated.
    $tree = [System.Collections.Generic.List[int]]::new()
    $tree.Add($procId)
    try {
        foreach ($kid in (Get-CimInstance Win32_Process -Filter "ParentProcessId=$procId" -ErrorAction SilentlyContinue)) {
            if (-not $tree.Contains([int]$kid.ProcessId)) { $tree.Add([int]$kid.ProcessId) }
        }
    } catch { }

    # Port ownership widens the set, but only for a process that started at or after our launch
    # time -- an unrelated process that later reused a recycled ephemeral port must not be killed.
    $foreign = [System.Collections.Generic.List[int]]::new()
    try {
        foreach ($conn in (Get-NetTCPConnection -LocalPort $script:launchPort -State Listen -ErrorAction SilentlyContinue)) {
            $ownerPid = [int]$conn.OwningProcess
            if ($ownerPid -le 4 -or $tree.Contains($ownerPid)) { continue }
            $isOurs = $false
            try {
                $owner = Get-Process -Id $ownerPid -ErrorAction SilentlyContinue
                $isOurs = ($null -ne $owner -and $null -ne $script:launchTime -and $owner.StartTime -ge $script:launchTime)
            } catch { }
            if ($isOurs) { $tree.Add($ownerPid) }
            elseif (-not $foreign.Contains($ownerPid)) { $foreign.Add($ownerPid) }
        }
    } catch { }
    if ($foreign.Count -gt 0) {
        Write-Host ("  Port {0} is also held by pid(s) {1}, which predate this launch. Left running." -f $script:launchPort, ($foreign -join ', ')) -ForegroundColor DarkYellow
    }

    try {
        if ($IsWindows) { & taskkill.exe /PID $procId /T /F *>$null }
        foreach ($id in $tree) { Stop-Process -Id $id -Force -ErrorAction SilentlyContinue }
    } catch { }

    # taskkill returns before the tree has finished exiting; wait for every identified process to
    # be gone and the port to be released so an immediate rebuild/relaunch does not race it.
    $waitForPort = ($foreign.Count -eq 0)
    foreach ($i in 1..100) {
        $alive = @($tree | ForEach-Object { Get-Process -Id $_ -ErrorAction SilentlyContinue })
        if ($alive.Count -eq 0 -and ((-not $waitForPort) -or (Test-TcpPortFree -P $script:launchPort))) { break }
        Start-Sleep -Milliseconds 100
    }
}

function Test-TcpPortFree {
    param([int]$P)
    $client = [System.Net.Sockets.TcpClient]::new()
    try {
        $async = $client.BeginConnect([System.Net.IPAddress]::Loopback, $P, $null, $null)
        if ($async.AsyncWaitHandle.WaitOne(400) -and $client.Connected) { return $false }
        return $true
    } catch { return $true } finally { $client.Dispose() }
}

# Emits the verdict detail, then a one-line VERIFIED stamp on success only, and exits. With
# -KeepAlive the launched server is left up so a browser-automation tool can read the real console.
# Early-exit paths (launch failed, never bound) call Stop-LaunchedApp directly and never reach
# here, so a dead launch is never left running.
function Write-FinalResult {
    param(
        [Parameter(Mandatory = $true)][string]$Verdict,
        [Parameter(Mandatory = $true)][int]$ExitCode
    )

    # Whether a browser check is required is decided by what the PAGE contains, not by which flags
    # the caller passed.
    $handlerCount = @($script:handlerPairs).Count
    $handlerNames = (@($script:handlerPairs) | ForEach-Object { $_.Event }) -join ', '

    if ($KeepAlive -and $null -ne $script:launched) {
        $script:keepRunning = $true   # tells the teardown guard at the end of the script to stand down
        $keptPid = $script:launched.Id
        Write-Host "`nSERVER LEFT RUNNING for browser check" -ForegroundColor Cyan
        Write-Host "  URL: $Url"
        Write-Host "  PID: $keptPid"
        Write-Host "  Stop it when done:  Stop-Process -Id $keptPid -Force"
        if ($script:launchLogDir) { Write-Host "  Its logs (delete when done): $script:launchLogDir" }
        # Fields, not prose: an unverified claim and a verified one read identically as prose, but
        # a blank field cannot be mistaken for a completed check.
        $browserLine = @"
Browser check: server left running at $Url (pid $keptPid). Fill in EVERY field below from the
         browser itself, or replace this whole line with 'NOT DONE - no browser tool available'.
         A field left as ____ , or filled from anything other than the tool's own output, is a
         reporting failure - not a shortcut.
           console errors - count, then the verbatim text of each: ____
           non-200 requests - count, then URL + status of each (favicon.ico excluded): ____
           typeof window.$($script:instanceName): ____
"@
        if ($resolvedComponent -eq 'Designer') {
            $browserLine += "`n           ReportStorageGetUrls() - count + report names returned: ____"
        }
    } else {
        Stop-LaunchedApp
        # Handlers and Designer storage are independent concerns; both must be surfaced when both apply.
        $notDoneParts = @()
        if ($handlerCount -gt 0) {
            $notDoneParts += "$handlerCount client-side handler(s) wired on this page ($handlerNames) - nothing in this run executed them, so the customization they implement is NOT VERIFIED."
        }
        if ($resolvedComponent -eq 'Designer') {
            $notDoneParts += "Report storage is NOT VERIFIED: a Designer whose storage is missing, dead, or shadowed by a conflicting IReportProvider reaches this same verdict. Run the read-only probe in references/report-storage.md."
        }
        if ($notDoneParts.Count -gt 0) {
            $browserLine = "Browser check: REQUIRED BUT NOT DONE - " + ($notDoneParts -join ' ALSO: ') + " Re-run with -KeepAlive and read the browser console with any browser-automation tool."
        } else {
            $browserLine = 'Browser check: not required - no client-side handlers detected in the rendered page.'
        }
    }

    # Agent-facing detail; not meant to be pasted verbatim. The VERIFIED stamp at the end of this
    # function is the one line a reply needs.
    Write-Host "`nStatic verification detail" -ForegroundColor Cyan
    Write-Host 'Verified with: Verify-ReportingPage.ps1 (static checks only - does not execute JavaScript)'
    if ($script:launchStamp) { Write-Host $script:launchStamp }
    Write-Host "Verdict: $Verdict (exit $ExitCode)"
    Write-Host 'Checked: resource status, required libraries and stylesheets, load order, init-vs-library order,'
    Write-Host '         duplicate loads, stylesheet asset references, leaked Razor text'
    Write-Host 'NOT checked: JavaScript execution. Handler WIRING is checked (declared, and declared'
    Write-Host '         before the init script), but a handler that runs and throws, or that runs and does'
    Write-Host '         the WRONG thing (wrong ActionId / ExportFormatID), still reports PASS here.'
    if ($script:instanceName) {
        Write-Host "         Also NOT checked: whether the component INITIALIZED. The constructor is page"
        Write-Host "         text and is present either way. In a browser confirm: typeof window.$($script:instanceName) === 'object'"
    }
    if ($resolvedComponent -eq 'Designer') {
        Write-Host '         Also NOT checked: report storage. A Designer with NO storage registered renders,'
        Write-Host '         logs no console error, and reaches this same PASS. Run the read-only probe in'
        Write-Host '         references/report-storage.md before calling a Designer done.'
    }
    Write-Host $browserLine

    # One quotable line, and only on success, so a failed run leaves nothing that can be edited
    # into a pass.
    if ($ExitCode -eq 0) {
        $notVerified = @('JS execution')
        if ($handlerCount -gt 0) { $notVerified += 'handler behaviour' }
        if ($resolvedComponent -eq 'Designer' -and -not $KeepAlive) { $notVerified += 'report storage' }
        Write-Host ("`nVERIFIED: {0} - static checks only. NOT verified: {1}." -f $Verdict, ($notVerified -join ', ')) -ForegroundColor Cyan
    } else {
        Write-Host "`nNo VERIFIED stamp is emitted for a failed run. The only accurate report is NOT VERIFIED" -ForegroundColor Yellow
        Write-Host 'plus the failure text above. Do not compose a stamp yourself, and do not overrule a FAIL' -ForegroundColor Yellow
        Write-Host 'with a check of your own - file-existence or grep output is not counter-evidence.' -ForegroundColor Yellow
    }

    exit $ExitCode
}

# Everything below runs inside try/finally so that ANY unexpected terminating error still tears
# the launched app down. `exit` inside Write-FinalResult still runs the finally block.
try {

if ($PSCmdlet.ParameterSetName -eq 'Launch') {
    if (-not (Test-Path -LiteralPath $ProjectDir -PathType Container)) {
        Write-Host "INCONCLUSIVE - -ProjectDir '$ProjectDir' is not a directory. Pass the project FOLDER, not the .csproj." -ForegroundColor Yellow
        exit 2
    }
    if ($Port -eq 0) {
        $probe = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Loopback, 0)
        $probe.Start(); $Port = $probe.LocalEndpoint.Port; $probe.Stop()
    }
    if (-not (Test-TcpPortFree -P $Port)) {
        Write-Host "INCONCLUSIVE - port $Port is already in use. Something else would answer this verification; refusing to report on a server this script did not start. Stop it, or omit -Port to auto-pick a free one." -ForegroundColor Yellow
        exit 2
    }

    if (-not $Route.StartsWith('/')) { $Route = "/$Route" }
    $Url = "http://localhost:$Port$Route"
    $script:launchPort = $Port
    # Random name inside a directory this run creates and deletes, so logs (which routinely carry
    # connection strings) never linger and the name can't be guessed by another local process.
    $script:launchLogDir = Join-Path ([System.IO.Path]::GetTempPath()) ('dx-verify-' + [System.IO.Path]::GetRandomFileName())
    New-Item -ItemType Directory -Path $script:launchLogDir -Force | Out-Null
    $script:launchLog = Join-Path $script:launchLogDir 'app'

    Write-Host "Launching: dotnet run in '$ProjectDir' on port $Port" -ForegroundColor Cyan
    $script:launchTime = Get-Date
    $script:launched = Start-Process dotnet `
        -ArgumentList 'run', '--urls', "http://localhost:$Port" `
        -WorkingDirectory $ProjectDir -PassThru -NoNewWindow `
        -RedirectStandardOutput "$script:launchLog.out" -RedirectStandardError "$script:launchLog.err"

    $bound = $false
    $attempt = 0
    while (((Get-Date) - $script:launchTime).TotalSeconds -lt $StartupTimeoutSec) {
        $attempt++
        if ($script:launched.HasExited) {
            Write-Host "`nLAUNCH FAILED - dotnet exited with code $($script:launched.ExitCode) after $([int]((Get-Date) - $script:launchTime).TotalSeconds) s. The app never started, so there is nothing to verify." -ForegroundColor Red
            foreach ($f in @("$script:launchLog.err", "$script:launchLog.out")) {
                if (Test-Path $f) { Get-Content $f -TotalCount 20 | ForEach-Object { Write-Host "  | $_" -ForegroundColor DarkGray } }
            }
            Write-Host "INCONCLUSIVE - app did not start." -ForegroundColor Yellow
            Stop-LaunchedApp   # dotnet run can exit while the apphost it spawned survives
            exit 2
        }
        try { Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 2 | Out-Null; $bound = $true; break }
        catch {
            # A status code means the app IS bound and routed; the route itself threw. In PowerShell
            # a 500 raises, so without this the poll mistakes a server error for "not started yet".
            $st = $null; try { $st = [int]$_.Exception.Response.StatusCode } catch { }
            # 401/403 = bound and routed, just behind auth.
            if ($st -eq 401 -or $st -eq 403) { $bound = $true; break }
            if ($st -and $st -ge 400) {
                Write-Host "`nFAIL - app is running, but $Route returned HTTP $st." -ForegroundColor Red
                foreach ($f in @("$script:launchLog.err", "$script:launchLog.out")) {
                    if (Test-Path $f) { Get-Content $f -Tail 40 | ForEach-Object { Write-Host "  | $_" -ForegroundColor DarkGray } }
                }
                Write-Host "The exception above is the defect. Do NOT raise -StartupTimeoutSec; the app started fine." -ForegroundColor Red
                Stop-LaunchedApp
                exit 1
            }
            Start-Sleep -Seconds 1
        }
    }

    if (-not $bound) {
        Write-Host "`nINCONCLUSIVE - app did not bind to $Url within ${StartupTimeoutSec}s." -ForegroundColor Yellow
        foreach ($f in @("$script:launchLog.err", "$script:launchLog.out")) {
            if (Test-Path $f) { Get-Content $f -Tail 20 | ForEach-Object { Write-Host "  | $_" -ForegroundColor DarkGray } }
        }
        Write-Host "If the log above shows 'address already in use', a stale server - or this project's" -ForegroundColor Yellow
        Write-Host "  own launchSettings.json ports - is holding a port. Stop that process and re-run." -ForegroundColor Yellow
        Write-Host "NEXT STEP - re-run this exact command once with -StartupTimeoutSec 180. A cold start with" -ForegroundColor Yellow
        Write-Host "  the full DevExpress dependency graph can exceed ${StartupTimeoutSec}s. If it times out again," -ForegroundColor Yellow
        Write-Host "  report NOT VERIFIED and stop. Do NOT start the app yourself - a hand-started server is not" -ForegroundColor Yellow
        Write-Host "  verification, and this script catches defects a browser console cannot see." -ForegroundColor Yellow
        Stop-LaunchedApp
        exit 2
    }

    $bindSec = [int]((Get-Date) - $script:launchTime).TotalSeconds
    $script:launchStamp = "Launched by script: pid $($script:launched.Id), bound after $bindSec s on attempt $attempt, port $Port"
    Write-Host "APP UP after $bindSec s (pid $($script:launched.Id))`n" -ForegroundColor Green
    if ($bindSec -gt 15) {
        $notes.Add("App took $bindSec s to bind. That is slow for a local ASP.NET app (typically 3-8 s); if this repeats, check for a blocking startup path.")
    }
} else {
    $script:launchStamp = "Launched by script: NO - verified a pre-existing server at $Url"
}

# ---------------------------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------------------------
$baseUri = [Uri]$Url
$script:webArgs = @{ UseBasicParsing = $true; TimeoutSec = $TimeoutSec }
if ($baseUri.Scheme -eq 'https') {
    if ($baseUri.IsLoopback) {
        # ASP.NET Core dev servers use a self-signed developer certificate. Loopback ONLY: a
        # remote host must still fail on an invalid certificate.
        $script:webArgs['SkipCertificateCheck'] = $true
    } else {
        $notes.Add("Certificate validation is ENFORCED for the non-loopback host '$($baseUri.Host)'. A certificate error on this run is a real finding, not a dev-certificate artifact.")
    }
}

function Get-Resource {
    param([string]$Target, [switch]$HeadOnly, [int]$Timeout = 0)
    $reqArgs = $script:webArgs.Clone()
    if ($Timeout -gt 0) { $reqArgs['TimeoutSec'] = $Timeout }
    # A status-only probe must not pull the body down: PowerShell returns binary content as
    # byte[], and [string] on a byte[] expands it into space-separated decimal digits.
    if ($HeadOnly) { $reqArgs['Method'] = 'Head' }
    try {
        $r = Invoke-WebRequest -Uri $Target @reqArgs
        $body = if ($HeadOnly -or $r.Content -isnot [string]) { '' } else { $r.Content }
        $final = $null; try { $final = $r.BaseResponse.RequestMessage.RequestUri } catch { }
        return @{ Status = [int]$r.StatusCode; Content = $body; Final = $final; Error = $null }
    } catch {
        $status = -1
        if ($_.Exception.PSObject.Properties.Name -contains 'Response' -and $_.Exception.Response) {
            $status = [int]$_.Exception.Response.StatusCode
        }
        return @{ Status = $status; Content = ''; Final = $null; Error = $_.Exception.Message }
    }
}

function Find-Signature {
    param([string]$Text, [string[]]$Needles)
    # Ordinal / case-sensitive: case-insensitive comparison would let the jQuery marker 'jquery:'
    # match DevExtreme's 'useJQuery:' option, reporting a phantom duplicate jQuery load.
    foreach ($n in $Needles) {
        $i = $Text.IndexOf($n, [StringComparison]::Ordinal)
        if ($i -ge 0) { return $i }
    }
    return -1
}

function Get-VisibleText {
    # Strips everything that is not rendered text: scripts (which carry localization strings such
    # as "Internal Server Error"), styles, verbatim code blocks, comments, tags.
    param([string]$Html)
    $t = $Html
    $t = [regex]::Replace($t, '<script\b[\s\S]*?</script>', ' ', 'IgnoreCase')
    $t = [regex]::Replace($t, '<style\b[\s\S]*?</style>', ' ', 'IgnoreCase')
    $t = [regex]::Replace($t, '<pre\b[\s\S]*?</pre>', ' ', 'IgnoreCase')
    $t = [regex]::Replace($t, '<code\b[\s\S]*?</code>', ' ', 'IgnoreCase')
    $t = [regex]::Replace($t, '<!--[\s\S]*?-->', ' ')
    $t = [regex]::Replace($t, '<[^>]+>', ' ')
    return [System.Net.WebUtility]::HtmlDecode($t)
}

# ---------------------------------------------------------------------------------------------
# 1. Fetch the page
# ---------------------------------------------------------------------------------------------
Write-Host "DevExpress reporting page verification" -ForegroundColor Cyan
Write-Host "Page: $Url`n"

$page = Get-Resource -Target $Url
# Route did not render (401/403 or redirect). Analyzing the other page reports false defects.
if ($page.Status -eq 401 -or $page.Status -eq 403 -or
    ($page.Final -and $page.Final.AbsolutePath.TrimEnd('/') -ne $baseUri.AbsolutePath.TrimEnd('/'))) {
    $why = if ($page.Status -eq 200) { "redirected to $($page.Final.AbsolutePath)" } else { "HTTP $($page.Status)" }
    Write-Host "INCONCLUSIVE - $($baseUri.AbsolutePath) did not render ($why) - most often authentication." -ForegroundColor Yellow
    Write-Host "  Not a page defect - do not change bundles or the view. Re-run against the route that serves the page, or report NOT VERIFIED." -ForegroundColor Yellow
    Write-Host $script:launchStamp -ForegroundColor DarkCyan
    Stop-LaunchedApp
    exit 2
}
if ($page.Status -ne 200) {
    Write-Host "FAIL: page returned HTTP $($page.Status). $($page.Error)" -ForegroundColor Red
    Write-Host $script:launchStamp -ForegroundColor DarkCyan
    foreach ($n in $notes) { Write-Host "NOTE: $n" -ForegroundColor DarkCyan }
    Stop-LaunchedApp
    exit 1
}
$html = $page.Content
$bodyText = Get-VisibleText -Html $html

# Checked against VISIBLE TEXT only: the viewer's inline client-side model contains a
# localization table with strings like "Internal Server Error", which would otherwise
# false-positive.
foreach ($marker in @(
    'An unhandled exception occurred while processing the request',
    'An error occurred while processing your request')) {
    if ($bodyText.IndexOf($marker, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        $failures.Add("Page shows a server error page ('$marker'). The action threw before rendering the control.")
    }
}

# ---------------------------------------------------------------------------------------------
# 2. Extract referenced resources, in document order
# ---------------------------------------------------------------------------------------------
$scriptTagRx = '<script\b(?![^>]*\btype\s*=\s*["''](?:importmap|text/html|application/json)["''])[^>]*\bsrc\s*=\s*["'']([^"'']+)["''][^>]*>'
$linkTagRx   = '<link\b[^>]*\brel\s*=\s*["'']stylesheet["''][^>]*>'

$resources = [System.Collections.Generic.List[object]]::new()
# [regex]::Matches scans left to right, so this list is already in document order.
foreach ($m in [regex]::Matches($html, "$scriptTagRx|$linkTagRx", 'IgnoreCase')) {
    if ($m.Groups[1].Success) {
        $resources.Add([pscustomobject]@{ Kind = 'js'; Src = $m.Groups[1].Value; DocPos = $m.Index })
    } else {
        $h = [regex]::Match($m.Value, '\bhref\s*=\s*["'']([^"'']+)["'']', 'IgnoreCase')
        if ($h.Success) { $resources.Add([pscustomobject]@{ Kind = 'css'; Src = $h.Groups[1].Value; DocPos = $m.Index }) }
    }
}

if ($resources.Count -eq 0) {
    $failures.Add('No <script src> or <link rel="stylesheet"> tags found on the page.')
}

# ---------------------------------------------------------------------------------------------
# 3. Fetch each resource, status-check it, and identify which libraries it provides
# ---------------------------------------------------------------------------------------------
Write-Host "Referenced resources ($($resources.Count)):" -ForegroundColor Cyan

# provides: libraryKey -> list of @{ ResIndex; Offset; Src; Via }
$provides = @{}
$unidentified = [System.Collections.Generic.List[string]]::new()
$stylesheets  = [System.Collections.Generic.List[object]]::new()
$resIndex = 0

foreach ($res in $resources) {
    $resolved = if ($res.Src -match '^(https?:)?//') {
        if ($res.Src.StartsWith('//')) { "$($baseUri.Scheme):$($res.Src)" } else { $res.Src }
    } else {
        [Uri]::new($baseUri, $res.Src).AbsoluteUri
    }

    $fetched = Get-Resource -Target $resolved
    $ok = ($fetched.Status -eq 200)
    $label = if ($ok) { 'OK  ' } else { 'FAIL' }
    $color = if ($ok) { 'DarkGray' } else { 'Red' }

    $table = if ($res.Kind -eq 'js') { $JsLibraries } else { $CssLibraries }
    $found = [System.Collections.Generic.List[string]]::new()

    # Retained for check 3b: a stylesheet's url(...) references must be resolved against the
    # stylesheet's own URL, so its absolute URI travels with its content.
    if ($ok -and $res.Kind -eq 'css' -and $fetched.Content) {
        $stylesheets.Add([pscustomobject]@{ Uri = $resolved; Src = $res.Src; Content = $fetched.Content })
    }

    if ($ok -and $fetched.Content) {
        foreach ($key in $table.Keys) {
            $spec = $table[$key]
            $off = Find-Signature -Text $fetched.Content -Needles $spec.Banner
            $via = 'banner'
            if ($off -lt 0 -and $spec.ContainsKey('Code') -and $spec.Code.Count -gt 0) {
                $off = Find-Signature -Text $fetched.Content -Needles $spec.Code
                $via = 'code-signature'
            }
            if ($off -ge 0) {
                if (-not $provides.ContainsKey($key)) { $provides[$key] = [System.Collections.Generic.List[object]]::new() }
                $provides[$key].Add([pscustomobject]@{ ResIndex = $resIndex; Offset = $off; Src = $res.Src; Via = $via })
                $found.Add($spec.Name)
            }
        }
        # Only an unidentified resource large enough to plausibly BE a DevExpress file counts
        # toward "we could not verify this page" — every app has its own small site.js/site.css
        # that legitimately matches no library.
        $sizeFloor = if ($res.Kind -eq 'js') { 50000 } else { 8000 }
        if ($found.Count -eq 0 -and $fetched.Content.Length -gt $sizeFloor) {
            $unidentified.Add("$($res.Src) ($([math]::Round($fetched.Content.Length / 1KB))KB)")
        }
    }

    $suffix = if ($found.Count -gt 0) { "  -> $($found -join ', ')" } else { '' }
    Write-Host ("  [{0}] {1} {2}{3}" -f $label, $res.Src.PadRight(46), $fetched.Status, $suffix) -ForegroundColor $color

    if (-not $ok) {
        $failures.Add("Resource '$($res.Src)' returned HTTP $($fetched.Status) instead of 200.")
    }
    $resIndex++
}

# ---------------------------------------------------------------------------------------------
# 3b. Assets referenced from INSIDE the stylesheets (icon fonts, images)
# ---------------------------------------------------------------------------------------------
# url(...) in CSS resolves against THE STYLESHEET'S OWN URL, not the page's, which is why an icon
# font can 404 even though every <link> on the page is 200 and the file exists on disk.
Write-Host "`nStylesheet asset references:" -ForegroundColor Cyan

$FontExtensions = @('.woff2', '.woff', '.ttf', '.eot', '.otf')
$AssetTimeoutSec = [Math]::Min($TimeoutSec, 20)   # a hung icon font must not add 100s to the run
$AssetBudget    = 40                              # non-font references only; fonts are never budgeted away
$assetsChecked  = 0
$assetsBad      = 0
$assetsSkipped  = 0
$seenAsset      = [System.Collections.Generic.HashSet[string]]::new()

foreach ($sheet in $stylesheets) {
    foreach ($m in [regex]::Matches($sheet.Content, 'url\(\s*[''"]?([^''")]+)[''"]?\s*\)')) {
        $ref = $m.Groups[1].Value.Trim()
        # Inline payloads and cross-origin assets are not ours to verify.
        if ($ref -match '^(data:|https?:|//)') { continue }
        # '.ttf?#iefix' and '.svg#icon' are standard @font-face syntax; the suffix is not part of
        # the path.
        $clean = ($ref -split '[?#]')[0]
        if ([string]::IsNullOrWhiteSpace($clean)) { continue }

        try { $absolute = [Uri]::new([Uri]$sheet.Uri, $clean).AbsoluteUri } catch { continue }
        if (-not $seenAsset.Add($absolute)) { continue }
        $ext    = [System.IO.Path]::GetExtension($clean).ToLowerInvariant()
        $isFont = $FontExtensions -contains $ext
        # A font is never budgeted away: an unchecked icon font is the exact defect this check
        # exists for.
        if (-not $isFont -and $assetsChecked -ge $AssetBudget) { $assetsSkipped++; continue }
        $assetsChecked++

        $asset = Get-Resource -Target $absolute -HeadOnly -Timeout $AssetTimeoutSec
        # Not every static-file handler answers HEAD; 405/501 means "ask again properly", not 404.
        if ($asset.Status -eq 405 -or $asset.Status -eq 501) {
            $asset = Get-Resource -Target $absolute -Timeout $AssetTimeoutSec
        }
        if ($asset.Status -eq 200) { continue }

        $assetsBad++
        if ($isFont) {
            Write-Host ("  [FAIL] {0} -> {1} [{2}]" -f $ref, $absolute, $asset.Status) -ForegroundColor Red
            $failures.Add("Stylesheet '$($sheet.Src)' references the font '$ref', which resolves to $absolute and returned HTTP $($asset.Status). CSS resolves url(...) against the STYLESHEET's own URL, not the page's, so the font must sit beside the stylesheet that references it — its libman.json destination should be '<the directory that stylesheet is served from>/icons'. Symptom in a browser: every toolbar icon is a tofu box even though the font file exists on disk.")
        } else {
            Write-Host ("  [WARN] {0} -> {1} [{2}]" -f $ref, $absolute, $asset.Status) -ForegroundColor Yellow
            $warnings.Add("Stylesheet '$($sheet.Src)' references '$ref', which resolves to $absolute and returned HTTP $($asset.Status).")
        }
    }
}

if ($assetsChecked -eq 0) {
    Write-Host '  [OK  ] no external asset references (all inline data: URIs)' -ForegroundColor DarkGray
} elseif ($assetsBad -eq 0) {
    Write-Host ("  [OK  ] {0} asset reference(s) across {1} stylesheet(s) resolve" -f $assetsChecked, $stylesheets.Count) -ForegroundColor DarkGray
}
if ($assetsSkipped -gt 0) {
    Write-Host ("  [INFO] {0} non-font reference(s) over the {1}-asset budget were NOT checked" -f $assetsSkipped, $AssetBudget) -ForegroundColor DarkGray
    $notes.Add("$assetsSkipped non-font stylesheet asset reference(s) exceeded the $AssetBudget-asset budget and were NOT checked. Every font WAS checked.")
}

# ---------------------------------------------------------------------------------------------
# 4. Determine which component the page hosts
# ---------------------------------------------------------------------------------------------
# Inline scripts only — the control's init script is emitted inline by the HTML helper.
$inlineScripts = ([regex]::Matches($html, '<script\b[^>]*>([\s\S]*?)</script>', 'IgnoreCase') |
                  ForEach-Object { $_.Groups[1].Value }) -join "`n"

# The helper emits e.g. DxReportDesigner($("#reportDesigner")[0], { callbacks: { _eventSenderCreated: assignGlobal } ... }).
# That callback assigns window.<id>, so the element id is also the global name a browser check must probe.
$instanceMatch = [regex]::Match($inlineScripts, 'Dx(?:ReportViewer|ReportDesigner|ReportParametersPanel)\(\$\("#([^"]+)"\)')
$script:instanceName = if ($instanceMatch.Success) { $instanceMatch.Groups[1].Value } else { $null }

$resolvedComponent = $Component
if ($Component -eq 'Auto') {
    # Most specific first: the Designer page also constructs a viewer for its Preview tab.
    foreach ($c in @('Designer', 'ParametersPanel', 'Viewer')) {
        if ($inlineScripts.IndexOf($ComponentSpecs[$c].Ctor, [StringComparison]::Ordinal) -ge 0) {
            $resolvedComponent = $c; break
        }
    }
    if ($resolvedComponent -eq 'Auto') {
        $failures.Add("Could not find any DevExpress control constructor (DxReportViewer / DxReportDesigner / DxReportParametersPanel) in the page's inline scripts. The HTML helper did not render — check the view and that the action returned the expected view.")
        $resolvedComponent = 'Viewer'
        $notes.Add("Component could not be auto-detected; requirements below were checked against 'Viewer' as a best guess.")
    } else {
        Write-Host "`nDetected component: $resolvedComponent" -ForegroundColor Cyan
    }
} else {
    if ($inlineScripts.IndexOf($ComponentSpecs[$resolvedComponent].Ctor, [StringComparison]::Ordinal) -lt 0) {
        $failures.Add("Expected a $resolvedComponent on this page but its constructor '$($ComponentSpecs[$resolvedComponent].Ctor)' does not appear in any inline script — the HTML helper did not render.")
    }
}
$spec = $ComponentSpecs[$resolvedComponent]

# ---------------------------------------------------------------------------------------------
# 4b. Report-resolution service conflict (launch mode only — needs the project's startup source)
# ---------------------------------------------------------------------------------------------
# IReportProvider and ReportStorageWebExtension both resolve a report name, by different
# mechanisms. Registered together, IReportProvider takes priority for loading: the 
# Viewer serves a stale report, nothing throws, and every check in this script still passes.
if ($PSCmdlet.ParameterSetName -eq 'Launch') {
    $startupFiles = @(Get-ChildItem -LiteralPath $ProjectDir -Filter '*.cs' -File -Recurse -ErrorAction SilentlyContinue |
                      Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
    if ($startupFiles.Count -eq 0) {
        $notes.Add("Report-resolution conflict check SKIPPED: no C# sources found under '$ProjectDir', so IReportProvider and ReportStorageWebExtension could not be compared.")
    } else {
        $startupText = (@($startupFiles | ForEach-Object { Get-Content -LiteralPath $_.FullName -Raw }) -join "`n")
        $startupText = [regex]::Replace($startupText, '/\*[\s\S]*?\*/', ' ')
        $regRx = '(?m)^(?!\s*//).*\.Add(Scoped|Singleton|Transient)\s*(<\s*|\(\s*typeof\s*\(\s*)'
        # A fully-qualified generic argument (no 'using' for the namespace) is legal and common,
        # and would otherwise defeat a match that requires the type name immediately after Add*<.
        $qual = '(?:[A-Za-z_]\w*\s*\.\s*)*'
        $hasProvider = [regex]::IsMatch($startupText, $regRx + $qual + 'IReportProvider(Async)?\s*[,>)]')
        $hasStorage  = [regex]::IsMatch($startupText, $regRx + $qual + 'ReportStorageWebExtension\s*[,>)]')
        if ($hasProvider -and $hasStorage) {
            Write-Host "`n  [FAIL] IReportProvider and ReportStorageWebExtension are both registered" -ForegroundColor Red
            $failures.Add("This project registers BOTH IReportProvider and ReportStorageWebExtension. IReportProvider takes priority for loading, ReportStorageWebExtension is only called for saving. Symptom: reports saved in the Designer never come back. Remove one registration entirely - see references/report-storage.md, 'Conflict warning'.")
        }
    }
}

# ---------------------------------------------------------------------------------------------
# 5. Required / expected libraries
# ---------------------------------------------------------------------------------------------
Write-Host "`nLibrary requirements for ${resolvedComponent}:" -ForegroundColor Cyan
$anyRequiredMissing = $false

foreach ($key in ($spec.RequiredJs + $spec.ExpectedJs)) {
    $isRequired = $spec.RequiredJs -contains $key
    $present = $provides.ContainsKey($key)
    $name = $JsLibraries[$key].Name
    if ($present) {
        Write-Host ("  [OK  ] {0}" -f $name) -ForegroundColor DarkGray
    } else {
        Write-Host ("  [{0}] {1} — not found in any loaded script" -f $(if ($isRequired) { 'FAIL' } else { 'WARN' }), $name) -ForegroundColor $(if ($isRequired) { 'Red' } else { 'Yellow' })
        if ($isRequired) {
            $failures.Add("Required library '$name' was not found in any script the page loads.")
            $anyRequiredMissing = $true
        } else {
            $warnings.Add("Expected library '$name' was not found. This is usually intentional only if the app deliberately omits it.")
        }
    }
}

foreach ($key in $spec.RequiredCss) {
    $name = $CssLibraries[$key].Name
    if ($provides.ContainsKey($key)) {
        Write-Host ("  [OK  ] {0}" -f $name) -ForegroundColor DarkGray
    } else {
        Write-Host ("  [FAIL] {0} — not found in any loaded stylesheet" -f $name) -ForegroundColor Red
        $failures.Add("Required stylesheet '$name' was not found in any stylesheet the page loads. The control renders unstyled or partially styled - no icons, collapsed surface, overlapping toolbar - which is indistinguishable from 'the viewer is broken'.")
        $anyRequiredMissing = $true
    }
}

# ace-builds is all-or-nothing (getting-started.md Step 9: all 5 scripts + all 3 stylesheets, or
# none). Omitting it entirely is supported — the Expression/Filter Editors just lose code
# completion. A PARTIAL set is the broken middle state: ace.js alone advertises ace to
# dx-analytics-core, which then calls setCompleters() on the ext-language_tools module that was
# never loaded and throws.
if ($resolvedComponent -eq 'Designer') {
    $aceSet = [ordered]@{ ace = $JsLibraries.ace.Name; aceLangTools = $JsLibraries.aceLangTools.Name
                          aceThemeDw = $JsLibraries.aceThemeDw.Name; aceThemeAmb = $JsLibraries.aceThemeAmb.Name
                          aceSnippets = $JsLibraries.aceSnippets.Name; aceCss = $CssLibraries.aceCss.Name
                          aceCssDw = $CssLibraries.aceCssDw.Name; aceCssAmb = $CssLibraries.aceCssAmb.Name }
    $aceMissing = @($aceSet.Keys | Where-Object { -not $provides.ContainsKey($_) })
    if ($aceMissing.Count -eq 0) {
        Write-Host "  [OK  ] ace-builds complete (5 scripts + 3 stylesheets)" -ForegroundColor DarkGray
    } elseif ($aceMissing.Count -eq $aceSet.Count) {
        Write-Host "  [INFO] ace-builds not loaded — Expression and Filter Editors will have no code completion" -ForegroundColor DarkGray
        $notes.Add("ace-builds is absent. This is a supported configuration; the Designer works but its Expression and Filter Editors lose syntax highlighting and completion.")
    } else {
        # Report the missing FILENAMES, split by kind, since the remedy mentions both scripts and
        # stylesheets and it must be unambiguous which set is short.
        $aceFiles = @{ ace = 'ace.js'; aceLangTools = 'ext-language_tools.js'; aceThemeDw = 'theme-dreamweaver.js'
                       aceThemeAmb = 'theme-ambiance.js'; aceSnippets = 'snippets/text.js'
                       aceCss = 'ace.css'; aceCssDw = 'theme/dreamweaver.css'; aceCssAmb = 'theme/ambiance.css' }
        $missJs  = @($aceMissing | Where-Object { $_ -notlike '*Css*' } | ForEach-Object { $aceFiles[$_] })
        $missCss = @($aceMissing | Where-Object { $_ -like '*Css*' }    | ForEach-Object { $aceFiles[$_] })
        Write-Host '  [FAIL] ace-builds is INCOMPLETE.' -ForegroundColor Red
        if ($missJs.Count) { Write-Host ("         missing SCRIPTS ({0}): {1}   <- add to a *.js bundle/tag" -f $missJs.Count, ($missJs -join ', ')) -ForegroundColor Red }
        else               { Write-Host '         scripts: complete (5/5)' -ForegroundColor DarkGray }
        if ($missCss.Count) { Write-Host ("         missing STYLESHEETS ({0}): {1}   <- add to a *.css bundle/tag" -f $missCss.Count, ($missCss -join ', ')) -ForegroundColor Red }
        else                { Write-Host '         stylesheets: complete (3/3)' -ForegroundColor DarkGray }
        $detail = @()
        if ($missJs.Count)  { $detail += "missing SCRIPTS ($($missJs.Count)): $($missJs -join ', ') - add these to a *.js bundle or tag" }
        else                { $detail += 'scripts: complete (5/5)' }
        if ($missCss.Count) { $detail += "missing STYLESHEETS ($($missCss.Count)): $($missCss -join ', ') - add these to a *.css bundle or tag" }
        else                { $detail += 'stylesheets: complete (3/3)' }
        $failures.Add("ace-builds is partially loaded. $($detail -join '; '). Load all 5 scripts and all 3 stylesheets, or none at all — a partial set makes the Expression and Filter Editors throw 'Cannot read properties of undefined (reading ''setCompleters'')' and never open.")
        $anyRequiredMissing = $true
    }
}

# ---------------------------------------------------------------------------------------------
# 6. Duplicate loads
# ---------------------------------------------------------------------------------------------
Write-Host "`nDuplicate-load check:" -ForegroundColor Cyan
$dupFound = $false
foreach ($key in $JsLibraries.Keys) {
    if (-not $provides.ContainsKey($key)) { continue }
    $sources = @($provides[$key] | Select-Object -ExpandProperty Src -Unique)
    if ($sources.Count -gt 1) {
        $dupFound = $true
        Write-Host ("  [FAIL] {0} loaded {1}x: {2}" -f $JsLibraries[$key].Name, $sources.Count, ($sources -join ' + ')) -ForegroundColor Red
        $failures.Add("Library '$($JsLibraries[$key].Name)' is loaded more than once ($($sources -join ' + ')). Duplicate loads cause errors such as 'Component dx-filtereditor-plain is already registered', and a second jQuery after DevExtreme detaches already-registered widgets.")
    }
}
if (-not $dupFound) { Write-Host '  [OK  ] no library loaded more than once' -ForegroundColor DarkGray }

# ---------------------------------------------------------------------------------------------
# 7. Load order — composite key (script tag position, offset within that file)
# ---------------------------------------------------------------------------------------------
Write-Host "`nLoad-order check:" -ForegroundColor Cyan
# Position of each library = (index of the script tag in the document, byte offset inside that
# file). The offset term catches a library placed after its dependant *inside a single
# concatenated bundle*, which a tag-order-only check cannot see.
$pos = @{}
foreach ($key in $JsLibraries.Keys) {
    if ($provides.ContainsKey($key)) {
        $first = $provides[$key] | Sort-Object ResIndex, Offset | Select-Object -First 1
        $pos[$key] = $first
    }
}
$orderOk = $true
foreach ($dep in $LoadDependencies) {
    if (-not ($pos.ContainsKey($dep.Before) -and $pos.ContainsKey($dep.After))) { continue }
    $b = $pos[$dep.Before]; $a = $pos[$dep.After]
    if (($a.ResIndex -lt $b.ResIndex) -or ($a.ResIndex -eq $b.ResIndex -and $a.Offset -lt $b.Offset)) {
        $orderOk = $false
        $where = if ($a.ResIndex -eq $b.ResIndex) {
            "both are inside '$($a.Src)' but in the wrong order within that bundle"
        } else {
            "'$($b.Src)' loads after '$($a.Src)'"
        }
        $failures.Add("Load order violation: $($JsLibraries[$dep.Before].Name) must load before $($JsLibraries[$dep.After].Name) — $($dep.Because). Here $where.")
        Write-Host ("  [FAIL] {0} must precede {1} ({2})" -f $JsLibraries[$dep.Before].Name, $JsLibraries[$dep.After].Name, $dep.Because) -ForegroundColor Red
    }
}
if ($pos.Count -eq 0) {
    Write-Host '  [--  ] not applicable - no known libraries were identified on this page' -ForegroundColor DarkGray
} elseif ($orderOk) {
    $seq = ($pos.GetEnumerator() | Sort-Object { $_.Value.ResIndex }, { $_.Value.Offset } |
            ForEach-Object { $JsLibraries[$_.Key].Name }) -join ' -> '
    Write-Host ("  [OK  ] $seq") -ForegroundColor DarkGray
}

# The check above orders libraries against EACH OTHER. It cannot see the case where every tag is
# correctly ordered but all of them sit AFTER the helper's inline init script: the init runs where
# it sits in the document, so it throws before the first library has loaded.
$ctorName = $ComponentSpecs[$resolvedComponent].Ctor
if ($inlineScripts.IndexOf($ctorName, [StringComparison]::Ordinal) -ge 0) {
    $ctorIdx = $html.IndexOf($ctorName, [StringComparison]::Ordinal)
    foreach ($key in $ComponentSpecs[$resolvedComponent].RequiredJs) {
        if (-not $provides.ContainsKey($key)) { continue }
        $firstRes = $provides[$key] | Sort-Object ResIndex, Offset | Select-Object -First 1
        $r = $resources[$firstRes.ResIndex]
        if ($r.DocPos -gt $ctorIdx) {
            Write-Host ("  [FAIL] {0} loads after the init script" -f $JsLibraries[$key].Name) -ForegroundColor Red
            $failures.Add("$($JsLibraries[$key].Name) is loaded by '$($r.Src)', which appears AFTER the inline init script constructing $ctorName. The init runs first and throws 'ReferenceError: DevExpress is not defined' (or 'jQuery/`$ is not defined'). Fix: move the <script>/<link> tags ABOVE the helper in the view, and move the layout's jQuery/Bootstrap tag into <head> — anything at the end of <body> loads after everything inside @RenderBody(). Do NOT rely on putting the tags in @section Scripts: ReportDesigner's RenderHtml() emits the init itself and its RenderScripts() emits nothing, so the init still runs first. (WebDocumentViewer differs — there RenderScripts() carries the init, which is why that split works for a Viewer page.)")
        }
    }
}

# ---------------------------------------------------------------------------------------------
# 8. Leaked, un-evaluated Razor fluent-call text in the rendered body
# ---------------------------------------------------------------------------------------------
Write-Host "`nUn-evaluated Razor text check:" -ForegroundColor Cyan
$body = $bodyText

$leakPatterns = @(
    @{ Rx = '(?m)^[ \t]*\.[A-Z][A-Za-z0-9_]*\s*\('; Why = 'a fluent-chain continuation line rendered as text' }
    @{ Rx = '\.Bind\s*\(\s*new\s+[A-Za-z_]';        Why = 'a literal .Bind(new ...) call rendered as text' }
    @{ Rx = '@Html\s*\.\s*DevExpress';               Why = 'an entirely unprocessed @Html.DevExpress() expression' }
)
$leakHits = [System.Collections.Generic.List[string]]::new()
foreach ($p in $leakPatterns) {
    foreach ($m in [regex]::Matches($body, $p.Rx)) {
        $snippet = ($m.Value -replace '\s+', ' ').Trim()
        $leakHits.Add("$snippet   ($($p.Why))")
    }
}
$leakHits = @($leakHits | Select-Object -Unique)
if ($leakHits.Count -gt 0) {
    foreach ($h in ($leakHits | Select-Object -First 8)) {
        Write-Host "  [FAIL] $h" -ForegroundColor Red
    }
    $failures.Add("Literal, un-evaluated Razor text found in the rendered page body ($($leakHits.Count) occurrence(s), e.g. '$($leakHits[0])'). Wrap the multi-line Html.DevExpress() chain in '@(...)'.")
} else {
    Write-Host '  [OK  ] no leaked fluent-call text in the rendered body' -ForegroundColor DarkGray
}

# ---------------------------------------------------------------------------------------------
# 9. Client-side handler wiring
# ---------------------------------------------------------------------------------------------
# The helper emits  "callbacks" : { "<Event>" : <handlerIdent>, ... }  into the init script.
# Handler identifiers are bare, not quoted. "_"-prefixed keys are internal plumbing
# (e.g. _eventSenderCreated : assignGlobal), not author handlers.
#
# This cannot tell whether a handler does the RIGHT thing — only that it exists and is
# reachable. It does catch a handler declared BELOW the init script, which throws
# ReferenceError at init time while the page still renders and every other check still passes.
Write-Host "`nClient-side handler check:" -ForegroundColor Cyan
$script:handlerPairs = [System.Collections.Generic.List[object]]::new()

# Brace-MATCHED, not regex-delimited: a naive '"callbacks"\s*:\s*\{(.*?)\}' cannot match a
# multi-line init script and truncates at the first nested '}' otherwise.
foreach ($start in [regex]::Matches($html, '["'']?callbacks["'']?\s*:\s*\{')) {
    # Collect only the callbacks object's OWN properties; anything nested is skipped so a config
    # object inside it cannot be mistaken for a handler.
    $sb = [System.Text.StringBuilder]::new()
    $i = $start.Index + $start.Length
    $depth = 1
    while ($i -lt $html.Length -and $depth -gt 0) {
        $ch = $html[$i]
        if     ($ch -eq '{') { $depth++ }
        elseif ($ch -eq '}') { $depth-- }
        elseif ($depth -eq 1) { [void]$sb.Append($ch) }
        $i++
    }
    if ($depth -ne 0) { continue }   # unbalanced: not a real object literal

    foreach ($p in [regex]::Matches($sb.ToString(), '["'']?([A-Za-z_][A-Za-z0-9_]*)["'']?\s*:\s*([A-Za-z_$][A-Za-z0-9_$.]*)')) {
        $ev = $p.Groups[1].Value
        $fn = $p.Groups[2].Value
        if ($ev -like '_*') { continue }
        $script:handlerPairs.Add([pscustomobject]@{ Event = $ev; Handler = $fn })

        # Recognizes function declarations, const/let/var assignments, and namespaced
        # (Foo.bar = ...) assignments, arrow or otherwise.
        $esc = [regex]::Escape($fn)
        $declRx = if ($fn.Contains('.')) {
            '(?<![\w$.])' + $esc + '\s*='
        } else {
            '\bfunction\s+' + $esc + '\s*\(' +
            '|\b(?:const|let|var)\s+' + $esc + '\s*=' +
            '|(?<![\w$.])' + $esc + '\s*=\s*(?:async\s*)?(?:function\b|\()'
        }
        $decl = [regex]::Match($html, $declRx)

        if (-not $decl.Success) {
            # A WARNING, not a failure: the handler may live in an external .js file this script
            # never reads, and nothing here executes JavaScript.
            Write-Host "  [WARN] $ev -> $fn  (no declaration found in the page HTML)" -ForegroundColor Yellow
            $warnings.Add("Client-side handler '$fn' is wired to '$ev' but no declaration of it appears in the page HTML. If it lives in an external script, that script must load BEFORE the init script or it throws 'ReferenceError: $fn is not defined' and the customization silently does nothing. Confirm in a browser.")
        } elseif ($decl.Index -gt $start.Index) {
            Write-Host "  [FAIL] $ev -> $fn  (declared AFTER the init script)" -ForegroundColor Red
            $failures.Add("Client-side handler '$fn' ('$ev') is declared AFTER the init script that references it, so it throws 'ReferenceError: $fn is not defined' and the customization silently does nothing. Move the <script> block to the top of the view, immediately after @model/@page and above everything else.")
        } else {
            Write-Host "  [OK  ] $ev -> $fn  (declared before init)" -ForegroundColor DarkGray
        }
    }
}
if ($script:handlerPairs.Count -eq 0) {
    Write-Host '  [OK  ] no client-side handlers wired on this page' -ForegroundColor DarkGray
}

# ---------------------------------------------------------------------------------------------
# 10. Verdict
# ---------------------------------------------------------------------------------------------
if ($Strict -and $warnings.Count -gt 0) {
    foreach ($w in $warnings) { $failures.Add("(promoted by -Strict) $w") }
    $warnings.Clear()
}

$inconclusive = ($anyRequiredMissing -and $unidentified.Count -gt 0)
if ($inconclusive) {
    $notes.Add("$($unidentified.Count) large resource(s) could not be identified: $($unidentified -join ', '). A bundle big enough to contain the missing library exists but carries no recognizable banner — most likely the build minified it and stripped the banner comments. Verify this page in a real browser.")
}

Write-Host "`n================== RESULT ==================" -ForegroundColor Cyan
Write-Host $script:launchStamp -ForegroundColor DarkCyan
foreach ($n in $notes)    { Write-Host "NOTE: $n" -ForegroundColor DarkCyan }
foreach ($w in $warnings) { Write-Host "WARN: $w" -ForegroundColor Yellow }

if ($failures.Count -eq 0) {
    Write-Host "PASS — $resolvedComponent page verified: all resources 200, required libraries and stylesheets present, load order correct, no duplicates, stylesheet asset references resolve, no leaked Razor text." -ForegroundColor Green
    if ($warnings.Count -gt 0) { Write-Host "($($warnings.Count) warning(s) above — re-run with -Strict to treat them as failures.)" -ForegroundColor Yellow }
    Write-FinalResult -Verdict "PASS ($resolvedComponent)" -ExitCode 0
}

if ($inconclusive) {
    Write-Host "INCONCLUSIVE — $($failures.Count) issue(s) found, but some resources could not be identified, so a required library may be present and simply unrecognizable. Verify in a real browser." -ForegroundColor Yellow
    $failures | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
    Write-FinalResult -Verdict 'INCONCLUSIVE' -ExitCode 2
}

Write-Host "FAIL — $($failures.Count) issue(s):" -ForegroundColor Red
$failures | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
Write-FinalResult -Verdict 'FAIL' -ExitCode 1

}
finally {
    # Reached on every path, including an unhandled error and the `exit` inside Write-FinalResult.
    if (-not $script:keepRunning) {
        Stop-LaunchedApp
        if ($script:launchLogDir -and (Test-Path -LiteralPath $script:launchLogDir)) {
            Remove-Item -LiteralPath $script:launchLogDir -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}
