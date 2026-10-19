<#
.SYNOPSIS
    Builds a WPF project, launches it, writes a text report of its UI tree, captures its main
    window to a PNG, and closes it.

.DESCRIPTION
    One command with a predictable result, so the render step of the skill never has to be
    reinvented. Every failure mode reports itself through a distinct exit code, so the caller
    can tell "the build broke" apart from "the window never appeared" without parsing prose.

    Two artifacts per run, from one launch of the app:
      <Out>          the PNG.
      <Out>.tree.txt the UI tree read through UI Automation, plus how it responds to a larger
                     window. Text, so the caller reads it directly instead of looking at an image.
    A tree that cannot be read is a warning, never a failure: the PNG is still written.

    Exit codes:
      0  a PNG was written
      2  build failed
      3  no runnable .exe found for the built project
      4  the app started but never showed a window
      5  the window was found but could not be captured

.PARAMETER Project
    Path to the .sln, .slnx or .csproj to build.

.PARAMETER Out
    Path of the PNG to write. Parent directory is created if missing.

.PARAMETER Configuration
    Build configuration. Debug by default.

.PARAMETER ExeHint
    File name (or fragment) of the executable to launch, for solutions that produce several.

.PARAMETER Width / Height
    Resize the window to this client size before capturing. Off by default on purpose: a WPF
    screen is adaptive, and capturing a size the app never opens grades layout the customer will
    never see. Use it only when the app's own size makes the capture useless - a window sized to
    its content, or one that opens too small to read.

.PARAMETER RenderDelayMs
    Extra time after the window appears, for WPF to finish its first render pass.

.PARAMETER WindowTimeoutSec
    How long to wait for a window to appear before giving up.

.EXAMPLE
    .\build-run-capture.ps1 -Project .\MyApp.sln -Out .\captures\pass1.png -Width 1280 -Height 800
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Project,
    [Parameter(Mandatory = $true)][string]$Out,
    [string]$Configuration = 'Debug',
    [string]$ExeHint,
    [int]$Width = 0,
    [int]$Height = 0,
    [int]$RenderDelayMs = 1200,
    [int]$WindowTimeoutSec = 15
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

if (-not ('Win32Capture' -as [type])) {
    Add-Type @'
using System;
using System.Runtime.InteropServices;
public class Win32Capture {
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint flags);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hwnd, int cmd);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hwnd, out RECT r);
    [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr hwnd, out RECT r);
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr hwnd, IntPtr after, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left, Top, Right, Bottom; }
}
'@
}

# Physical pixels only. Without this, GetWindowRect hands back scaled coordinates on a
# high-DPI display and the bitmap ends up a different size than the window it captures.
[void][Win32Capture]::SetProcessDPIAware()

function Write-Step($text) { Write-Host "[capture] $text" }

# --- UI tree ----------------------------------------------------------------------------------
# UI Automation reads the live window from outside the process: no code is added to the app and
# nothing has to be cleaned up afterwards. It reports what each element IS, WHERE it is, and
# WHETHER it can be edited - which is what a structural check needs. It does not report colours;
# those stay a job for the capture.
$uiaReady = $false
try {
    Add-Type -AssemblyName UIAutomationClient -ErrorAction Stop
    Add-Type -AssemblyName UIAutomationTypes -ErrorAction Stop
    $uiaReady = $true
} catch {
    Write-Warning "UI Automation is unavailable ($($_.Exception.Message)); no tree report will be written."
}

function Get-UiTree([IntPtr]$hwnd) {
    $script:uiaItems = New-Object System.Collections.Generic.List[object]
    $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker

    function Add-UiElement($el, $depth, $parentIdx) {
        $c = $el.Current
        $r = $c.BoundingRectangle
        $hasRect = -not ([double]::IsInfinity($r.Width) -or $r.Width -le 0 -or $r.Height -le 0)
        $readOnly = $null
        $pattern = $null
        try {
            if ($el.TryGetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern, [ref]$pattern) -and $pattern) {
                $readOnly = $pattern.Current.IsReadOnly
            }
        } catch { }
        $script:uiaItems.Add([pscustomobject]@{
            Depth     = $depth
            Parent    = $parentIdx
            Type      = $c.ControlType.ProgrammaticName.Replace('ControlType.', '')
            Name      = $c.Name
            HasRect   = $hasRect
            X         = if ($hasRect) { [int]$r.X } else { 0 }
            Y         = if ($hasRect) { [int]$r.Y } else { 0 }
            W         = if ($hasRect) { [int]$r.Width } else { 0 }
            H         = if ($hasRect) { [int]$r.Height } else { 0 }
            Offscreen = $c.IsOffscreen
            Enabled   = $c.IsEnabled
            Focusable = $c.IsKeyboardFocusable
            ReadOnly  = $readOnly
            Skip      = $false
        })
        $idx = $script:uiaItems.Count - 1
        $child = $walker.GetFirstChild($el)
        while ($null -ne $child) {
            Add-UiElement $child ($depth + 1) $idx
            $child = $walker.GetNextSibling($child)
        }
    }

    Add-UiElement ([System.Windows.Automation.AutomationElement]::FromHandle($hwnd)) 0 -1
    return $script:uiaItems
}

# Two kinds of noise make the report longer than the screen it describes, and neither carries
# information: a control that reports itself again as its own single child (an editor and its
# text part), and long runs of identical siblings (the rows of a data control).
$editableTypes = @('Edit', 'ComboBox', 'Custom', 'Spinner', 'Document')
$rowTypes = @('DataItem', 'ListItem', 'TreeItem')

# UI Automation nests a container's content under the *item* that heads it: a tab control reports a
# whole page under its TabItem, a tree reports child nodes under the parent TreeItem. Such an item's
# rectangle is only its own header row, so comparing a child against it flags every element of a
# correctly laid out page - measured: 29 false lines on a tabbed screen, 16 and 7 on two others.
# Walk up past item nodes to the first ancestor that can actually own the space.
$itemHeaderTypes = $rowTypes + @('TabItem', 'MenuItem')
function Test-OutsideParent($items, $it) {
    if ($it.Parent -lt 0 -or -not $it.HasRect) { return $false }
    $p = $items[$it.Parent]
    while ($itemHeaderTypes -contains $p.Type) {
        if ($p.Parent -lt 0) { return $false }
        $p = $items[$p.Parent]
    }
    if (-not $p.HasRect) { return $false }
    return ($it.X -lt $p.X - 1 -or $it.Y -lt $p.Y - 1 -or
            ($it.X + $it.W) -gt ($p.X + $p.W + 1) -or ($it.Y + $it.H) -gt ($p.Y + $p.H + 1))
}

function Format-UiTree($items) {
    $lines = New-Object System.Collections.Generic.List[string]
    $childCount = @{}
    $siblingRun = @{}
    foreach ($it in $items) { if ($it.Parent -ge 0) { $childCount[$it.Parent] = 1 + [int]$childCount[$it.Parent] } }

    $hidden = @{}
    $printDepth = @{}
    $printDepth[-1] = -1   # the root's parent, so the root itself prints unindented
    for ($i = 0; $i -lt $items.Count; $i++) {
        $it = $items[$i]
        $p = if ($it.Parent -ge 0) { $items[$it.Parent] } else { $null }

        # A control that reports itself again as its own only child: keep the outer one, which is
        # the control the markup names, and lift its children to its level. Skipping the wrapper
        # must never skip what is underneath it - an element whose children are invisible to UI
        # Automation (a Border, a Path) makes a group look like it has one child, and hiding that
        # subtree would delete a whole region from the report.
        $wrapper = ($null -ne $p -and $childCount[$it.Parent] -eq 1 -and $it.Name -eq $p.Name -and -not (Test-OutsideParent $items $it))
        $printDepth[$i] = [int]$printDepth[$it.Parent] + $(if ($wrapper) { 0 } else { 1 })
        if ([bool]$hidden[$it.Parent]) { $hidden[$i] = $true; continue }
        if ($wrapper) { continue }

        # The rows of a data control are counted, not listed. Only row-shaped types collapse:
        # a run of editors or captions is the screen's content, and hiding it would hide the
        # elements the check is looking for.
        $runKey = '{0}|{1}' -f $it.Parent, $it.Type
        $siblingRun[$runKey] = 1 + [int]$siblingRun[$runKey]
        $position = $siblingRun[$runKey]
        if ($position -gt 3 -and $rowTypes -contains $it.Type) {
            $hidden[$i] = $true
            if ($position -eq 4) {
                $family = @($items | Where-Object { $_.Parent -eq $it.Parent -and $_.Type -eq $it.Type })
                $lines.Add(('{0}... +{1} more {2}, last "{3}"' -f ('  ' * $printDepth[$i]), ($family.Count - 3), $it.Type, $family[-1].Name))
            }
            continue
        }

        $rect = if ($it.HasRect) { '{0},{1} {2}x{3}' -f $it.X, $it.Y, $it.W, $it.H } else { 'no rect' }
        $flags = New-Object System.Collections.Generic.List[string]
        if ($it.Offscreen) { $flags.Add('offscreen') }
        if (-not $it.Enabled) { $flags.Add('disabled') }
        if ($editableTypes -contains $it.Type) {
            if ($it.ReadOnly -eq $true) { $flags.Add('read-only') }
            if ($it.ReadOnly -eq $false) { $flags.Add('editable') }
        }
        if (Test-OutsideParent $items $it) { $flags.Add('outside parent') }
        $name = $it.Name
        if ($name.Length -gt 70) { $name = $name.Substring(0, 70) + '...' }
        $lines.Add(('{0}{1} "{2}" [{3}]{4}' -f ('  ' * $printDepth[$i]), $it.Type, $name, $rect,
            $(if ($flags.Count) { ' ' + ($flags -join ',') } else { '' })))
    }
    return $lines
}

# The mockup is one size; the screen is not. Comparing the same tree at a wider and taller
# window is what turns "does this layout survive a resize" into an observation.
# Identical names repeat - ten editors all called "TextEdit" - so an element is matched between
# the two readings by its position among its namesakes, not by its name alone.
function Get-MatchKeys($items) {
    $seen = @{}
    $keys = New-Object System.Collections.Generic.List[string]
    foreach ($it in $items) {
        $k = '{0}|{1}|{2}' -f $it.Depth, $it.Type, $it.Name
        $seen[$k] = 1 + [int]$seen[$k]
        $keys.Add(('{0}|{1}' -f $k, $seen[$k]))
    }
    return $keys
}

function Compare-UiTrees($atOwnSize, $atLargerSize) {
    $lines = New-Object System.Collections.Generic.List[string]
    $ownKeys = Get-MatchKeys $atOwnSize
    $largerKeys = Get-MatchKeys $atLargerSize
    $later = @{}
    for ($i = 0; $i -lt $atLargerSize.Count; $i++) { $later[$largerKeys[$i]] = $atLargerSize[$i] }

    # The region that absorbs the extra space is the one that follows in both directions. A
    # region that follows in neither is fixed - by its content, or by a hard-coded size.
    $verdicts = New-Object System.Collections.Generic.List[object]
    for ($i = 0; $i -lt $atOwnSize.Count; $i++) {
        $it = $atOwnSize[$i]
        if ($it.Depth -ne 1) { continue }
        if (-not $later.ContainsKey($ownKeys[$i])) {
            $lines.Add(('gone at the larger size: {0} "{1}"' -f $it.Type, $it.Name)); continue
        }
        $now = $later[$ownKeys[$i]]
        if (-not ($it.HasRect -and $now.HasRect)) { continue }
        $follows = (($now.W - $it.W) -ge 4) -or (($now.H - $it.H) -ge 4)
        $verdict = @()
        $verdict += if (($now.W - $it.W) -ge 4) { 'width follows' } else { 'width fixed' }
        $verdict += if (($now.H - $it.H) -ge 4) { 'height follows' } else { 'height fixed' }
        $name = $it.Name
        if ($name.Length -gt 40) { $name = $name.Substring(0, 40) + '...' }
        $verdicts.Add([pscustomobject]@{
            Follows = $follows
            Line    = ('{0} "{1}": {2}x{3} -> {4}x{5} - {6}' -f $it.Type, $name, $it.W, $it.H, $now.W, $now.H, ($verdict -join ', '))
        })
    }

    # A screen built as regions has a handful of top-level elements; a screen built as one flat
    # pile has dozens. Listing dozens buries the answer, so then only the ones that moved are
    # listed and the rest are counted.
    $followed = @($verdicts | Where-Object { $_.Follows })
    if ($verdicts.Count -le 12) {
        foreach ($v in $verdicts) { $lines.Add($v.Line) }
    } else {
        $lines.Add(('{0} top-level elements; {1} kept both dimensions when the window grew{2}' -f `
            $verdicts.Count, ($verdicts.Count - $followed.Count), $(if ($followed.Count) { ', the rest are listed below' } else { '' })))
        foreach ($v in $followed) { $lines.Add($v.Line) }
    }

    # Anything at any depth that only breaks once there is more room.
    foreach ($it in $atLargerSize) {
        if (Test-OutsideParent $atLargerSize $it) {
            $lines.Add(('drawn outside its container at the larger size: {0} "{1}"' -f $it.Type, $it.Name))
        }
    }

    $win = $atLargerSize[0]
    $regions = @($atLargerSize | Where-Object { $_.Depth -eq 1 -and $_.HasRect -and $_.Type -ne 'TitleBar' })
    if ($regions.Count) {
        $right = ($regions | ForEach-Object { $_.X + $_.W } | Measure-Object -Maximum).Maximum
        $bottom = ($regions | ForEach-Object { $_.Y + $_.H } | Measure-Object -Maximum).Maximum
        $lines.Add(('nothing reaches closer than {0} px to the right edge and {1} px to the bottom edge' -f `
            [Math]::Max(0, ($win.X + $win.W) - $right), [Math]::Max(0, ($win.Y + $win.H) - $bottom)))
    }
    return $lines
}

function Write-UiTreeReport([IntPtr]$hwnd, [string]$path, [int]$outerW, [int]$outerH) {
    $own = Get-UiTree $hwnd
    $probeW = [int]($outerW * 1.4)
    $probeH = [int]($outerH * 1.2)
    [void][Win32Capture]::SetWindowPos($hwnd, [IntPtr]::Zero, 0, 0, $probeW, $probeH, 0x0006)
    Start-Sleep -Milliseconds 600
    $larger = Get-UiTree $hwnd
    # Back to the size the app itself chose: the PNG must show what the customer opens.
    [void][Win32Capture]::SetWindowPos($hwnd, [IntPtr]::Zero, 0, 0, $outerW, $outerH, 0x0006)
    Start-Sleep -Milliseconds 600

    $report = New-Object System.Collections.Generic.List[string]
    $report.Add("UI tree of the running window, read through UI Automation.")
    $report.Add("Element = what it reports itself as, then its screen rectangle x,y wxh.")
    $report.Add("Flags: read-only / editable = the value can or cannot be typed into;")
    $report.Add("       offscreen = not visible now; outside parent = drawn beyond its container;")
    $report.Add("       no rect = never laid out.")
    $report.Add("Collapsed: a control repeating itself as its own single child; runs of identical siblings.")
    $report.Add("Colours are not readable here - judge those from the PNG.")
    $report.Add('')
    $report.Add("== tree at the window size the app opens ($outerW x $outerH outer) ==")
    $report.AddRange([string[]](Format-UiTree $own))
    $report.Add('')
    $report.Add("== the same window at $probeW x $probeH outer: what followed the size ==")
    $report.AddRange([string[]](Compare-UiTrees $own $larger))
    Set-Content -LiteralPath $path -Value $report -Encoding UTF8
    Write-Step "wrote $path ($($own.Count) elements)"
}

$projectPath = (Resolve-Path -LiteralPath $Project).Path
$projectDir = Split-Path -Parent $projectPath
$outPath = [System.IO.Path]::GetFullPath($Out)
$outDir = Split-Path -Parent $outPath
if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
    [void](New-Item -ItemType Directory -Path $outDir -Force)
}

# Fallback only: a real project's OutputPath rarely follows the bin\<Configuration>\ convention
# (Directory.Build.props and per-config conditions both override it), so this widened search - any
# .exe under any bin\ folder below the project, newest first - is what the pre-build stale-process
# kill uses, and what the post-build lookup falls back to when MSBuild cannot be asked directly.
function Find-AppExe {
    $candidates = Get-ChildItem -Path $projectDir -Recurse -Filter *.exe -ErrorAction SilentlyContinue |
        Where-Object {
            $_.FullName -match '\\bin\\' -and
            $_.FullName -notmatch '\\ref\\' -and
            $_.Name -notmatch '(?i)(test|vshost|createdump)'
        }
    if ($ExeHint) { $candidates = $candidates | Where-Object { $_.Name -like "*$ExeHint*" } }
    return $candidates | Sort-Object LastWriteTime -Descending
}

function Resolve-MsBuildExe {
    $vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path -LiteralPath $vswhere) {
        return & $vswhere -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" |
            Select-Object -First 1
    }
    return $null
}

# MSBuild has already computed OutputPath, including anything set by Directory.Build.props above
# the project or by conditions - asking it beats parsing the csproj by hand. Only works against a
# .csproj: -getProperty does not work when the argument is a .sln.
function Get-TargetPathFromMsBuild {
    if ($projectPath -notmatch '\.csproj$') { return $null }
    try {
        if ($useMsBuild) {
            if (-not $msbuildExe) { return $null }
            $lines = & $msbuildExe $projectPath "/p:Configuration=$Configuration" '/nologo' '/v:quiet' '-getProperty:TargetPath' 2>$null
        } else {
            $lines = & dotnet build $projectPath --configuration $Configuration --nologo -v quiet -getProperty:TargetPath 2>$null
        }
    } catch {
        return $null
    }
    if ($LASTEXITCODE -ne 0 -or -not $lines) { return $null }
    $path = (@($lines) | Select-Object -Last 1).ToString().Trim()
    if ($path -and (Test-Path -LiteralPath $path)) { return $path }
    return $null
}

# A previous run still holding the output .exe makes the build fail with a file lock, and the
# error text points at the linker rather than at the real cause. Clear it before building.
foreach ($stale in (Find-AppExe)) {
    $name = [System.IO.Path]::GetFileNameWithoutExtension($stale.Name)
    # Match the full path, not the name: an unrelated app of the user's may share the exe name.
    Get-Process -Name $name -ErrorAction SilentlyContinue |
        Where-Object { $_.Path -eq $stale.FullName } |
        Stop-Process -Force -ErrorAction SilentlyContinue
}

# Old-style projects (TargetFrameworkVersion, no TargetFramework) need MSBuild; the SDK
# projects that most DevExpress WPF apps use today build fine with dotnet.
$useMsBuild = $false
if ($projectPath -match '\.csproj$') {
    $projectXml = Get-Content -LiteralPath $projectPath -Raw
    if ($projectXml -notmatch '<TargetFrameworks?>') { $useMsBuild = $true }
}

Write-Step "building $([System.IO.Path]::GetFileName($projectPath)) ($Configuration)"
$msbuildExe = $null
if ($useMsBuild) {
    $msbuildExe = Resolve-MsBuildExe
    if (-not $msbuildExe) {
        Write-Error "This is a .NET Framework project and MSBuild was not found. Install the VS build tools or build it manually."
        exit 2
    }
    & $msbuildExe $projectPath "/p:Configuration=$Configuration" /v:minimal /nologo
} else {
    & dotnet build $projectPath --configuration $Configuration --nologo -v minimal
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed. Fix the build before capturing - a capture of a stale binary proves nothing."
    exit 2
}

# TargetPath of an SDK-style .NET project is the managed .dll, and a .dll cannot be started as a
# process - the apphost .exe sits beside it under the same name. Prefer that, and fall back to the
# bin search whenever MSBuild's answer cannot be launched, so a project that produces no apphost
# still gets found instead of failing at Start-Process.
$targetPath = Get-TargetPathFromMsBuild
$exe = $null
if ($targetPath) {
    $candidate = $targetPath
    if ([System.IO.Path]::GetExtension($candidate) -eq '.dll') {
        $appHost = [System.IO.Path]::ChangeExtension($candidate, '.exe')
        $candidate = if (Test-Path -LiteralPath $appHost) { $appHost } else { $null }
    }
    if ($candidate) {
        $exe = Get-Item -LiteralPath $candidate
        Write-Step "resolved target from MSBuild: $($exe.FullName)"
    }
}
if (-not $exe) {
    $exe = Find-AppExe | Select-Object -First 1
}
if (-not $exe) {
    Write-Error "No runnable .exe found for the built project. Pass -ExeHint if the solution builds several outputs."
    exit 3
}
Write-Step "launching $($exe.Name)"

$proc = Start-Process -FilePath $exe.FullName -PassThru
try {
    # Poll instead of sleeping blind: a small app is up in 300ms, a big one takes seconds.
    $hwnd = [IntPtr]::Zero
    $deadline = (Get-Date).AddSeconds($WindowTimeoutSec)
    while ((Get-Date) -lt $deadline) {
        $proc.Refresh()
        if ($proc.HasExited) { break }
        if ($proc.MainWindowHandle -ne [IntPtr]::Zero) { $hwnd = $proc.MainWindowHandle; break }
        Start-Sleep -Milliseconds 250
    }

    if ($hwnd -eq [IntPtr]::Zero) {
        if ($proc.HasExited) {
            Write-Error "The app exited with code $($proc.ExitCode) before showing a window - it crashed on startup. Check for an unhandled exception (a bad resource key or a missing theme assembly does this)."
        } else {
            Write-Error "No window appeared within $WindowTimeoutSec s. The app may be waiting on a dialog, a login, or a splash screen."
        }
        exit 4
    }

    # PrintWindow returns an empty bitmap for a minimized window.
    [void][Win32Capture]::ShowWindow($hwnd, 9)  # SW_RESTORE

    if ($Width -gt 0 -and $Height -gt 0) {
        $outer = New-Object 'Win32Capture+RECT'
        $client = New-Object 'Win32Capture+RECT'
        [void][Win32Capture]::GetWindowRect($hwnd, [ref]$outer)
        [void][Win32Capture]::GetClientRect($hwnd, [ref]$client)
        $chromeW = ($outer.Right - $outer.Left) - ($client.Right - $client.Left)
        $chromeH = ($outer.Bottom - $outer.Top) - ($client.Bottom - $client.Top)
        # 0x0002 SWP_NOMOVE | 0x0004 SWP_NOZORDER
        [void][Win32Capture]::SetWindowPos($hwnd, [IntPtr]::Zero, 0, 0, $Width + $chromeW, $Height + $chromeH, 0x0006)
        Start-Sleep -Milliseconds 400
    }

    Start-Sleep -Milliseconds $RenderDelayMs

    if ($uiaReady) {
        $before = New-Object 'Win32Capture+RECT'
        if ([Win32Capture]::GetWindowRect($hwnd, [ref]$before)) {
            $treePath = [System.IO.Path]::ChangeExtension($outPath, '.tree.txt')
            try {
                Write-UiTreeReport $hwnd $treePath ($before.Right - $before.Left) ($before.Bottom - $before.Top)
            } catch {
                Write-Warning "Could not read the UI tree: $($_.Exception.Message). The PNG is unaffected."
            }
        }
    }

    $rect = New-Object 'Win32Capture+RECT'
    if (-not [Win32Capture]::GetWindowRect($hwnd, [ref]$rect)) {
        Write-Error "GetWindowRect failed."
        exit 5
    }
    $w = $rect.Right - $rect.Left
    $h = $rect.Bottom - $rect.Top
    if ($w -le 0 -or $h -le 0) {
        Write-Error "The window reports a zero size."
        exit 5
    }

    $bitmap = New-Object System.Drawing.Bitmap $w, $h
    try {
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        try {
            $hdc = $graphics.GetHdc()
            # Flag 2 is PW_RENDERFULLCONTENT. Without it a hardware-accelerated WPF window
            # comes back solid black.
            $ok = [Win32Capture]::PrintWindow($hwnd, $hdc, 2)
            $graphics.ReleaseHdc($hdc)
        } finally { $graphics.Dispose() }

        $center = $bitmap.GetPixel([int]($w / 2), [int]($h / 2))
        if (-not $ok -or ($center.R -eq 0 -and $center.G -eq 0 -and $center.B -eq 0)) {
            # Some compositions still refuse PrintWindow. Screen-copy the same rectangle
            # instead - it needs the window unobstructed, which is why it is the fallback.
            Write-Step "PrintWindow came back empty; falling back to a screen copy"
            $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
            try { $graphics.CopyFromScreen($rect.Left, $rect.Top, 0, 0, $bitmap.Size) }
            finally { $graphics.Dispose() }
        }

        $bitmap.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
    } finally { $bitmap.Dispose() }

    Write-Host "[capture] wrote $outPath ($w x $h)"
    exit 0
} finally {
    # In a finally block on purpose: any throw above would otherwise leave the app running,
    # and it would lock the output .exe against the next build.
    if ($proc -and -not $proc.HasExited) {
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    }
}
