<#
.SYNOPSIS
    Measures a captured window as TEXT: samples named points, scans for a target colour, and
    diffs two captures. Nothing here needs an image viewer.

.DESCRIPTION
    The judgement half of visual verification, as one command with a predictable text result.
    An agent that cannot open a PNG can still complete a render check with this: every answer
    comes back as numbers on stdout.

    Three questions, any combination in one call:
      -At "x,y[;x,y...]"   what colour is at these points
      -Color "#RRGGBB"     where does this colour appear, how much of it, and in what box
      -Baseline <png>      what changed between the baseline and the image, and where

    Exit codes:
      0  the measurement ran
      2  an image could not be read
      3  arguments name no question to answer

.PARAMETER Image
    The capture to measure (the "after").

.PARAMETER Baseline
    The "before" capture. With it, the run also reports the changed-pixel count and bounding box.
    Without it a -Color scan proves the colour is PRESENT, never that it CHANGED.

.PARAMETER Color
    Target colour as #RRGGBB, reported with -Tolerance per channel.

.PARAMETER At
    Points to sample, "x,y" separated by ";".

.PARAMETER Tolerance
    Per-channel tolerance for -Color. 8 by default: antialiasing and theme gradients move a
    channel a little, and an exact-only match reports 0 pixels on a colour that is plainly there.

.PARAMETER CaptionHeight
    Rows at the top reported separately in the diff. The title bar differs between captures
    because activation state differs, which is noise rather than a result.

.EXAMPLE
    .\measure-render.ps1 -Image $env:TEMP\dx_verify_after.png -Color '#00008B'

.EXAMPLE
    .\measure-render.ps1 -Image $env:TEMP\dx_verify_after.png -Baseline $env:TEMP\dx_verify_before.png -At '441,80'
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Image,
    [string]$Baseline,
    [string]$Color,
    [string]$At,
    [int]$Tolerance = 8,
    [int]$CaptionHeight = 40
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

function Read-Pixels([string]$path) {
    if (-not (Test-Path -LiteralPath $path)) { Write-Output "ERROR: image not found -> $path"; exit 2 }
    try { $bmp = [System.Drawing.Bitmap]::FromFile((Resolve-Path -LiteralPath $path)) }
    catch { Write-Output "ERROR: cannot read image -> $path ($($_.Exception.Message))"; exit 2 }
    $rect = [System.Drawing.Rectangle]::new(0, 0, $bmp.Width, $bmp.Height)
    $data = $bmp.LockBits($rect, [System.Drawing.Imaging.ImageLockMode]::ReadOnly,
                          [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    # A negative stride means a bottom-up buffer: Scan0 points at the LAST row, so both the copy
    # below and the row indexing in Get-Hex would read the wrong memory. Asking LockBits for
    # Format32bppArgb makes GDI+ hand back a converted top-down buffer, and it was measured
    # positive for a 32bpp PNG, a JPEG and a bottom-up BMP. This has therefore not been seen -
    # but a colour measured off the wrong rows would be silently wrong, which is worse than a
    # stop, so stop.
    if ($data.Stride -lt 0) {
        $bmp.UnlockBits($data); $bmp.Dispose()
        Write-Output "ERROR: bottom-up image buffer (negative stride) -> $path"; exit 2
    }
    $bytes = [byte[]]::new($data.Stride * $bmp.Height)
    [System.Runtime.InteropServices.Marshal]::Copy($data.Scan0, $bytes, 0, $bytes.Length)
    $bmp.UnlockBits($data)
    $out = [pscustomobject]@{
        W = $bmp.Width; H = $bmp.Height; Stride = $data.Stride; B = $bytes; Name = (Split-Path $path -Leaf)
    }
    $bmp.Dispose()
    return $out
}

# The buffer is BGRA; this returns "#RRGGBB".
function Get-Hex($img, [int]$x, [int]$y) {
    if ($x -lt 0 -or $y -lt 0 -or $x -ge $img.W -or $y -ge $img.H) { return $null }
    $i = $y * $img.Stride + $x * 4
    '#{0:X2}{1:X2}{2:X2}' -f $img.B[$i + 2], $img.B[$i + 1], $img.B[$i]
}

if (-not $Color -and -not $At -and -not $Baseline) {
    Write-Output "ERROR: nothing to measure - pass -Color, -At or -Baseline"
    exit 3
}

$after = Read-Pixels $Image
Write-Output ("image: {0}  {1}x{2}" -f $after.Name, $after.W, $after.H)

$before = $null
if ($Baseline) {
    $before = Read-Pixels $Baseline
    Write-Output ("baseline: {0}  {1}x{2}" -f $before.Name, $before.W, $before.H)
    if ($before.W -ne $after.W -or $before.H -ne $after.H) {
        Write-Output ("SIZE CHANGED {0}x{1} -> {2}x{3}  (a diff across two sizes is not a measurement)" -f $before.W, $before.H, $after.W, $after.H)
    }
}

if ($At) {
    foreach ($p in $At.Split(';', [StringSplitOptions]::RemoveEmptyEntries)) {
        $xy = $p.Split(',')
        if ($xy.Count -ne 2) { Write-Output "point '$p' is not x,y"; continue }
        $x = [int]$xy[0].Trim(); $y = [int]$xy[1].Trim()
        $a = Get-Hex $after $x $y
        if ($null -eq $a) { Write-Output ("({0},{1}) is outside the image" -f $x, $y); continue }
        if ($before) {
            $b = Get-Hex $before $x $y
            $verdict = if ($b -eq $a) { 'unchanged' } else { 'changed' }
            Write-Output ("({0},{1}) before {2} -> after {3}   {4}" -f $x, $y, $b, $a, $verdict)
        }
        else {
            Write-Output ("({0},{1}) = {2}   (no baseline: this is the value, not a change)" -f $x, $y, $a)
        }
    }
}

if ($Color) {
    $hex = $Color.TrimStart('#')
    if ($hex.Length -ne 6) { Write-Output "ERROR: -Color must be #RRGGBB"; exit 3 }
    $tr = [Convert]::ToInt32($hex.Substring(0, 2), 16)
    $tg = [Convert]::ToInt32($hex.Substring(2, 2), 16)
    $tb = [Convert]::ToInt32($hex.Substring(4, 2), 16)
    $n = 0; $minX = $after.W; $minY = $after.H; $maxX = -1; $maxY = -1
    for ($y = 0; $y -lt $after.H; $y++) {
        $row = $y * $after.Stride
        for ($x = 0; $x -lt $after.W; $x++) {
            $i = $row + $x * 4
            if ([Math]::Abs($after.B[$i + 2] - $tr) -le $Tolerance -and
                [Math]::Abs($after.B[$i + 1] - $tg) -le $Tolerance -and
                [Math]::Abs($after.B[$i] - $tb) -le $Tolerance) {
                $n++
                if ($x -lt $minX) { $minX = $x }; if ($x -gt $maxX) { $maxX = $x }
                if ($y -lt $minY) { $minY = $y }; if ($y -gt $maxY) { $maxY = $y }
            }
        }
    }
    if ($n -eq 0) {
        Write-Output ("colour #{0}: 0 pixels within +-{1} per channel - it is NOT on this screen" -f $hex.ToUpper(), $Tolerance)
    }
    else {
        $cx = [int](($minX + $maxX) / 2); $cy = [int](($minY + $maxY) / 2)
        Write-Output ("colour #{0}: {1} pixels within +-{2}   bbox x {3}..{4}  y {5}..{6}   centre ({7},{8}) = {9}" -f $hex.ToUpper(), $n, $Tolerance, $minX, $maxX, $minY, $maxY, $cx, $cy, (Get-Hex $after $cx $cy))
        if ($before) {
            $bn = 0
            for ($y = $minY; $y -le $maxY; $y++) {
                $row = $y * $before.Stride
                for ($x = $minX; $x -le $maxX; $x++) {
                    $i = $row + $x * 4
                    if ([Math]::Abs($before.B[$i + 2] - $tr) -le $Tolerance -and
                        [Math]::Abs($before.B[$i + 1] - $tg) -le $Tolerance -and
                        [Math]::Abs($before.B[$i] - $tb) -le $Tolerance) { $bn++ }
                }
            }
            $reading = if ($bn -ge $n) { 'IT WAS ALREADY THERE - this proves nothing changed' } else { 'so the colour arrived with the change' }
            Write-Output ("  the same box in the baseline held {0} pixels of it - {1}" -f $bn, $reading)
        }
        else {
            Write-Output "  no baseline given: this proves the colour is PRESENT, not that it CHANGED"
        }
    }
}

if ($before) {
    $n = 0; $capN = 0; $minX = $after.W; $minY = $after.H; $maxX = -1; $maxY = -1
    $h = [Math]::Min($before.H, $after.H); $w = [Math]::Min($before.W, $after.W)
    for ($y = 0; $y -lt $h; $y++) {
        $ra = $y * $after.Stride; $rb = $y * $before.Stride
        for ($x = 0; $x -lt $w; $x++) {
            $ia = $ra + $x * 4; $ib = $rb + $x * 4
            if ($after.B[$ia] -ne $before.B[$ib] -or $after.B[$ia + 1] -ne $before.B[$ib + 1] -or $after.B[$ia + 2] -ne $before.B[$ib + 2]) {
                if ($y -lt $CaptionHeight) { $capN++; continue }
                $n++
                if ($x -lt $minX) { $minX = $x }; if ($x -gt $maxX) { $maxX = $x }
                if ($y -lt $minY) { $minY = $y }; if ($y -gt $maxY) { $maxY = $y }
            }
        }
    }
    if ($n -eq 0) {
        Write-Output ("changed pixels below the caption: 0 - NOTHING took effect, whatever the images look like  (caption rows differ by {0}, which is activation noise)" -f $capN)
    }
    else {
        Write-Output ("changed pixels below the caption: {0}   bbox x {1}..{2}  y {3}..{4}   (caption rows: {5}, activation noise)" -f $n, $minX, $maxX, $minY, $maxY, $capN)
    }
}
