<#
.SYNOPSIS
  Embeds a REPX file into a Delphi text DFM as the "Layout.Strings" property of a TdxReport / TdxReportControl.

.DESCRIPTION
  Writes the layout exactly the way the IDE streams TdxReport.Layout / TdxCustomReportControl.Layout
  (DefineProperties 'Layout.Strings'): one DFM string item per REPX line, quotes doubled, non-ASCII chars
  as #NNNN, strings wrapped at 64 chars with ' +' continuation. An existing Layout.Strings block of the
  component is replaced; otherwise the block is inserted right after the "object <Name>: <Class>" line.
  The DFM must be UTF-8 (IDE-saved forms are plain ASCII, which is valid UTF-8). It is written back in UTF-8
  with CRLF line endings, keeping a BOM if the input had one. A form in another encoding (ANSI) stops the
  script with an error instead of being rewritten.

  Optional -Base64 token expansion: every "@@B64:<file>@@" in the REPX is replaced with the Base64 (UTF-8)
  of that file (path relative to the REPX). Use it to keep data-source XML (SqlDataSource, JsonDataSource,
  FederationDataSource) readable in separate files.

.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File Set-DfmLayout.ps1 -Dfm uMainForm.dfm -Component dxReport1 -Repx Layouts\Report.repx
.EXAMPLE
  ... -Dfm uMainForm.dfm -Component dxReport1 -Repx Layouts\Report.repx -ExpandedRepxOut Layouts\Report.expanded.repx
#>
param(
  [Parameter(Mandatory = $true)][string]$Dfm,
  [Parameter(Mandatory = $true)][string]$Component,
  [Parameter(Mandatory = $true)][string]$Repx,
  [string]$ExpandedRepxOut
)
$ErrorActionPreference = 'Stop'
$MaxChunk = 64

function ConvertTo-DfmStringParts([string]$Text) {
  $parts = New-Object System.Collections.Generic.List[string]
  $sb = New-Object System.Text.StringBuilder
  $inQuote = $false; $count = 0
  foreach ($ch in $Text.ToCharArray()) {
    $code = [int]$ch
    if ($code -ge 32 -and $code -le 126) {
      if (-not $inQuote) { [void]$sb.Append("'"); $inQuote = $true }
      if ($ch -eq "'") { [void]$sb.Append("''") } else { [void]$sb.Append($ch) }
    } else {
      if ($inQuote) { [void]$sb.Append("'"); $inQuote = $false }
      [void]$sb.Append("#$code")
    }
    $count++
    if ($count -ge $MaxChunk) {
      if ($inQuote) { [void]$sb.Append("'"); $inQuote = $false }
      $parts.Add($sb.ToString()); [void]$sb.Clear(); $count = 0
    }
  }
  if ($inQuote) { [void]$sb.Append("'") }
  if ($sb.Length -gt 0) { $parts.Add($sb.ToString()) }
  if ($parts.Count -eq 0) { $parts.Add("''") }
  return ,$parts
}

function Get-Indent([string]$Line) { return $Line.Length - $Line.TrimStart(' ').Length }

$repxPath = (Resolve-Path $Repx).Path
$repxDir = Split-Path $repxPath -Parent
$repxText = [System.IO.File]::ReadAllText($repxPath, [System.Text.Encoding]::UTF8)
$repxText = [regex]::Replace($repxText, '@@B64:([^@]+)@@', {
  param($m)
  $p = Join-Path $repxDir $m.Groups[1].Value
  [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes([System.IO.File]::ReadAllText($p, [System.Text.Encoding]::UTF8).Trim()))
})
if ($ExpandedRepxOut) { [System.IO.File]::WriteAllText($ExpandedRepxOut, $repxText, (New-Object System.Text.UTF8Encoding($false))) }
$repxLines = $repxText.TrimStart([char]0xFEFF) -split "`r?`n"
if ([string]::IsNullOrWhiteSpace($repxText.TrimStart([char]0xFEFF))) { throw "$repxPath is empty." }
while ($repxLines.Count -gt 1 -and $repxLines[-1] -eq '') { $repxLines = $repxLines[0..($repxLines.Count - 2)] }

$dfmPath = (Resolve-Path $Dfm).Path
$dfmBytes = [System.IO.File]::ReadAllBytes($dfmPath)
$dfmHasBom = $dfmBytes.Length -ge 3 -and $dfmBytes[0] -eq 0xEF -and $dfmBytes[1] -eq 0xBB -and $dfmBytes[2] -eq 0xBF
$dfmStart = if ($dfmHasBom) { 3 } else { 0 }
try { $dfmText = [System.Text.UTF8Encoding]::new($false, $true).GetString($dfmBytes, $dfmStart, $dfmBytes.Length - $dfmStart) }
catch { throw "$dfmPath is not UTF-8 (an ANSI form?). Save it as UTF-8 or write non-ASCII text as #NNNN, then run again." }
$dfmLines = [System.Collections.Generic.List[string]]($dfmText -split "`r?`n")
if ($dfmLines.Count -gt 0 -and $dfmLines[$dfmLines.Count - 1] -eq '') { $dfmLines.RemoveAt($dfmLines.Count - 1) }

$objIdx = -1
for ($i = 0; $i -lt $dfmLines.Count; $i++) {
  if ($dfmLines[$i] -match "^\s*(object|inherited|inline)\s+$([regex]::Escape($Component))\s*:") { $objIdx = $i; break }
}
if ($objIdx -lt 0) { throw "Component '$Component' not found in $Dfm" }
$propIndent = (Get-Indent $dfmLines[$objIdx]) + 2

# Remove an existing Layout.Strings block (property line + all deeper-indented continuation lines)
$insertAt = $objIdx + 1
for ($i = $objIdx + 1; $i -lt $dfmLines.Count; $i++) {
  $ind = Get-Indent $dfmLines[$i]
  if ($ind -lt $propIndent) { break }                       # end of this component
  if ($ind -eq $propIndent -and $dfmLines[$i].TrimStart() -match '^(object|inherited|inline)\s') { break }
  if ($ind -eq $propIndent -and $dfmLines[$i].TrimStart().StartsWith('Layout.Strings')) {
    $j = $i + 1
    while ($j -lt $dfmLines.Count -and (Get-Indent $dfmLines[$j]) -gt $propIndent) { $j++ }
    $dfmLines.RemoveRange($i, $j - $i)
    $insertAt = $i
    break
  }
}

$pad = ' ' * $propIndent
$block = New-Object System.Collections.Generic.List[string]
$block.Add("${pad}Layout.Strings = (")
for ($i = 0; $i -lt $repxLines.Count; $i++) {
  $parts = ConvertTo-DfmStringParts $repxLines[$i]
  for ($p = 0; $p -lt $parts.Count; $p++) {
    $linePad = if ($p -eq 0) { "$pad  " } else { "$pad    " }
    $suffix = if ($p -lt $parts.Count - 1) { ' +' } elseif ($i -eq $repxLines.Count - 1) { ')' } else { '' }
    $block.Add("$linePad$($parts[$p])$suffix")
  }
}
$dfmLines.InsertRange($insertAt, $block)
[System.IO.File]::WriteAllText($dfmPath, (($dfmLines -join "`r`n") + "`r`n"), [System.Text.UTF8Encoding]::new($dfmHasBom))
Write-Host "Embedded $($repxLines.Count) REPX lines into $Component ($dfmPath)"
