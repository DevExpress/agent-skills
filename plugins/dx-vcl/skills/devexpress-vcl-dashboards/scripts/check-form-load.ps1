<#
.SYNOPSIS
  Runtime smoke test for VCL forms: starts a built Delphi/C++Builder executable and reports any error
  dialog shown while its forms load (.dfm streaming errors such as "Property X does not exist",
  "Class TFoo not found", "Invalid property value").

.DESCRIPTION
  .dfm text is not checked by the compiler; it is parsed only when a form is created at run time.
  This script launches the program, watches its top-level windows for a few seconds, and:
    - exits 1 and prints the dialog text if the process shows a message/error dialog (#32770, a VCL
      TMessageForm from ShowMessage/MessageDlg, or a DevExpress TdxMessageDialogForm from dxMessageDlg - for the
      last two the message text is drawn, so only the title shows),
    - exits 1 if the process terminates by itself with a non-zero exit code,
    - exits 2 and prints WARN if the process exits by itself with code 0 (no form was kept open: not a PASS),
    - exits 0 if a main window appears and no dialog shows up during the settle period.
  The process is always closed afterwards.

  Git Bash rewrites an argument that starts with '/' (-Arguments /selftest) into a Windows path. Prefix the
  command with MSYS_NO_PATHCONV=1 and pass every path in Windows form (C:/..., not /c/...), or use '-selftest'.

  Only forms that are created at startup (auto-create forms in the project source) are checked. To check a
  form that is created on demand, temporarily auto-create it or open it from the main form's OnShow.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File check-form-load.ps1 -Exe .\Win32\Debug\Project1.exe
#>
param(
  [Parameter(Mandatory = $true)][string]$Exe,
  [string]$Arguments = '',
  [int]$TimeoutSec = 15,   # max time to wait for the main window
  [int]$SettleSec = 3      # extra time after the main window appears, to catch late dialogs
)

$ErrorActionPreference = 'Stop'
if (-not (Test-Path -LiteralPath $Exe)) { Write-Output "FAIL: executable not found: $Exe"; exit 2 }

if (-not ('FormLoadCheck.Win' -as [type])) {
  Add-Type -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
namespace FormLoadCheck {
  public class WinInfo { public IntPtr Handle; public string Class; public string Title; public bool Visible; }
  public static class Win {
    delegate bool EnumProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")] static extern bool EnumWindows(EnumProc cb, IntPtr lParam);
    [DllImport("user32.dll")] static extern bool EnumChildWindows(IntPtr parent, EnumProc cb, IntPtr lParam);
    [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint pid);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetClassName(IntPtr hWnd, StringBuilder s, int n);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr hWnd, StringBuilder s, int n);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr hWnd);
    static WinInfo Info(IntPtr h) {
      var c = new StringBuilder(256); GetClassName(h, c, 256);
      var t = new StringBuilder(4096); GetWindowText(h, t, 4096);
      return new WinInfo { Handle = h, Class = c.ToString(), Title = t.ToString(), Visible = IsWindowVisible(h) };
    }
    public static List<WinInfo> TopLevel(uint pid) {
      var r = new List<WinInfo>();
      EnumWindows((h, l) => { uint p; GetWindowThreadProcessId(h, out p); if (p == pid) r.Add(Info(h)); return true; }, IntPtr.Zero);
      return r;
    }
    public static List<WinInfo> Children(IntPtr parent) {
      var r = new List<WinInfo>();
      EnumChildWindows(parent, (h, l) => { r.Add(Info(h)); return true; }, IntPtr.Zero);
      return r;
    }
  }
}
'@
}

function Get-DialogText([IntPtr]$Handle) {
  $parts = New-Object System.Collections.Generic.List[string]
  foreach ($c in [FormLoadCheck.Win]::Children($Handle)) {
    if ($c.Title -and $c.Class -ne 'Button') { $parts.Add($c.Title) }
  }
  if ($parts.Count -eq 0) {
    # Task dialogs draw their text in a DirectUIHWND; read it through UI Automation when available.
    try {
      Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes -ErrorAction Stop
      $root = [System.Windows.Automation.AutomationElement]::FromHandle($Handle)
      $all = $root.FindAll([System.Windows.Automation.TreeScope]::Descendants,
                           [System.Windows.Automation.Condition]::TrueCondition)
      foreach ($e in $all) {
        $n = $e.Current.Name
        if ($n -and $e.Current.ControlType.ProgrammaticName -ne 'ControlType.Button') { $parts.Add($n) }
      }
    } catch { }
  }
  ($parts | Select-Object -Unique) -join ' | '
}

$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = (Resolve-Path -LiteralPath $Exe).Path
$psi.Arguments = $Arguments
$psi.WorkingDirectory = Split-Path -Parent $psi.FileName
$psi.UseShellExecute = $false
$proc = [System.Diagnostics.Process]::Start($psi)

$result = $null
$dialogClasses = @('#32770', 'TMessageForm', 'TdxMessageDialogForm')
$mainSeenAt = $null
$deadline = (Get-Date).AddSeconds($TimeoutSec)
try {
  while ($true) {
    Start-Sleep -Milliseconds 250
    if ($proc.HasExited) {
      if ($proc.ExitCode -ne 0) { $result = "FAIL: process exited with code $($proc.ExitCode) before/while showing its main form" }
      else { $result = 'WARN: process exited by itself with code 0 (no main window was kept open)' }
      break
    }
    $wins = [FormLoadCheck.Win]::TopLevel([uint32]$proc.Id) | Where-Object { $_.Visible }
    # Windows message/task dialogs (#32770) and DevExpress message dialogs (dxMessageDlg, skinned exception boxes).
    $dialog = $wins | Where-Object { $dialogClasses -contains $_.Class } | Select-Object -First 1
    if ($dialog) {
      $text = Get-DialogText $dialog.Handle
      $result = "FAIL: dialog '$($dialog.Title)' ($($dialog.Class)): $text"
      break
    }
    # A visible VCL form (class name starts with 'T', e.g. TForm1/TMainForm) means the main form is up.
    $main = $wins | Where-Object { $_.Class -like 'T*' -and $_.Class -ne 'TApplication' -and $dialogClasses -notcontains $_.Class -and $_.Title -ne '' } | Select-Object -First 1
    if ($main -and -not $mainSeenAt) { $mainSeenAt = Get-Date; $mainInfo = "$($main.Class) '$($main.Title)'" }
    if ($mainSeenAt -and ((Get-Date) - $mainSeenAt).TotalSeconds -ge $SettleSec) {
      $result = "PASS: main window $mainInfo is up and no error dialog appeared"
      break
    }
    # The timeout applies until the main window appears; after that, always wait the full settle time for a late dialog.
    if (-not $mainSeenAt -and (Get-Date) -gt $deadline) {
      $result = 'FAIL: no main window and no dialog within the timeout'
      break
    }
  }
}
finally {
  if (-not $proc.HasExited) {
    try { [void]$proc.CloseMainWindow() } catch { }
    if (-not $proc.WaitForExit(2000)) { try { $proc.Kill() } catch { } }
  }
}

Write-Output $result
if ($result -like 'FAIL*') { exit 1 }
if ($result -like 'WARN*') { exit 2 }
exit 0
