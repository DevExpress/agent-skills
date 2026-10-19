# Visual Verification: Before/After Screenshots

**This file is addressed to whoever is running the phase** — a dedicated visual-verification sub-agent where the harness has them, otherwise the run itself. Screenshots are the biggest context sink, so where a sub-agent exists they stay inside it and only a text description (baseline) or a verdict (post-change) crosses back. Where one does not, the phase is run inline and the same rule is kept by other means: the capture stays a path on disk and the judgement is made from the measured numbers, so no image has to enter the conversation.

⛔ **Wait for it — never run a capture in the background.** Its output is the input to the very next thing you do, so a run that launches it and moves on has no capture, and one that ends its turn to wait for it may never get another turn. Measured: a run backgrounded both captures, said it would measure them "once it finishes", and stopped there — leaving a picture of the app on disk and nothing verified.

⛔ **Not being able to view an image never cancels this phase.** `scripts/measure-render.ps1` answers every question below in text — what colour is at a point, where a colour appears and how much of it, and what changed between two captures. "I cannot see it" is a statement about the instrument, not a verdict about the change.

You are invoked at one of two moments; the orchestrator tells you which:
- **Part A — Baseline capture** (Step 1, before any changes): capture the "before" screenshot to disk and return a short **text description** of the current appearance. No image crosses back.
- **Part B — Post-change verification** (Step 10): rebuild, run, capture the "after" screenshot, compare it against the baseline file on disk, and return a **verdict + brief diagnosis**. No image crosses back.

**You receive from the orchestrator:** which part to run, and the **customization goal** (the visual change the user wants). For Part B you also receive the **baseline description** and the on-disk path of the "before" screenshot — you run in a fresh context, so you cannot rely on anything captured during Part A except what the orchestrator hands you plus the file on disk.

**The retry loop is owned by the orchestrator, not you** (it holds the attempt budget). Do not edit XAML, apply corrections, or re-run yourself. In Part B, capture → compare → return the verdict once; the orchestrator decides whether to correct and re-invoke you.

**Every command block in this file is PowerShell, and `pwsh` is the shell to run them in.** From another shell (`cmd`) invoke them as `pwsh -Command "..."`, or they do not execute at all.

---

## Which screen to capture

The capture photographs the window the app opens to; most targets are not on it. Pick the screen in this
order, stop at the first that answers, and **reuse that choice in Part B** — a before and an after taken on
different screens compare nothing.

1. **The view holding the target** — the ask names a screen, or the target is one element in one view's file.
   No search.
2. **The startup window as it is** — the target control is already on it. No source change.
3. **A view found by search** — grep the app's XAML for the target control type; take a candidate whose route
   is unambiguous, a static tab or panel child of the startup window before a separate view. Reject a
   candidate that redefines the resource key or hardcodes the property you are about to change: it
   photographs unchanged and reads as "none" though the change worked.
4. **Ask the user which screen**, naming the candidates found. This is the last step, not "cannot verify".

### Reaching it - one temporary, reverted edit

Steps 1 and 3 need that screen to be the one the app opens to:

- a tab or panel already in the startup window: select it (`SelectedIndex`, `IsSelected`);
- a separate view or window: point the startup at it (`StartupUri`, or the shell's default-module/navigate
  argument).

Copy the original line verbatim first, revert it after the last capture **in the part that made it**, and
confirm the revert by re-reading the line. Never leave it in place across parts or across the run - an
aborted run must not leave the user's app opening on a different screen. A failed revert is a change left in
the user's project: say so in the return.

Return with the description or verdict: which step chose the screen, and what the temporary edit was.

## Part A: Capture the Baseline (before changes)

### 1. Find the runnable entry point

Use `glob` to locate the solution or startup project:
- `**/*.sln` and `**/*.slnx` — prefer a solution; build it. Search for **both**: recent .NET SDKs create the XML-based `.slnx` by default, so a `*.sln`-only search reports "no solution" on a project that has one.
- If no solution, look for a `.csproj` containing `<OutputType>WinExe</OutputType>` or `<OutputType>Exe</OutputType>`
- If the build tooling here cannot read a `.slnx`, build the `.csproj` directly — that is a tooling limit, not a reason to return "cannot verify".

If no runnable entry point is found, return a **"cannot verify" signal** (no runnable `.sln` or startup `.csproj` found) to the orchestrator — do not ask the user or proceed with other steps yourself.

### 2. Build the app

**Kill any leftover instance first.** A previous capture that failed part-way can leave the app running; it holds `bin\Debug\…exe` open and the build then fails with a file-in-use error that looks like a code problem and costs a whole pass.

```powershell
Get-Process -Name "<app exe name, no extension>" -ErrorAction SilentlyContinue | Stop-Process -Force
dotnet build "<path-to-sln-or-csproj>" --configuration Debug --no-restore 2>&1
```

If the build fails, return a **"cannot verify" signal** (baseline build failed, with the error) to the orchestrator — do not notify the user or skip ahead yourself.

### 3. Find the output executable

```powershell
glob "**\bin\Debug\**\*.exe" --ignore "**\ref\**"
```

Take the most recently modified `.exe` that isn't a test runner (avoid `*.Tests.exe`, `testhost.exe`).

### 4. Run the app and capture a screenshot

Capture the **app window directly with `PrintWindow`** — not a blind full-screen grab. A full-screen `CopyFromScreen` shot captures whatever pixels are on that region of the desktop, so it catches the wrong window when the app is occluded by other windows (a terminal, chat app, etc.) and misses the app entirely on a second monitor — producing a meaningless before/after diff. `PrintWindow` copies the target window's *own* pixels from its device context, so it works even when the window is occluded, unfocused, or partially off-screen. Use the `PW_RENDERFULLCONTENT` flag (`2`) — required or a hardware-accelerated WPF window comes back black.

```powershell
Add-Type -AssemblyName System.Windows.Forms, System.Drawing
Add-Type @"
using System; using System.Runtime.InteropServices;
public class Win {
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int c);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr h, IntPtr hdc, uint flags);
    public struct RECT { public int Left, Top, Right, Bottom; }
}
"@

# Start the app and wait for its main window (poll up to ~15s instead of a blind sleep)
$proc = Start-Process "<path-to-exe>" -PassThru
try {
    $hwnd = [IntPtr]::Zero
    for ($i = 0; $i -lt 30; $i++) {
        Start-Sleep -Milliseconds 500
        $proc.Refresh()
        if ($proc.MainWindowHandle -ne 0) { $hwnd = $proc.MainWindowHandle; break }
    }
    if ($hwnd -eq [IntPtr]::Zero) { throw "app main window never appeared" }

    [Win]::ShowWindow($hwnd, 9) | Out-Null   # SW_RESTORE — un-minimize (PrintWindow on a minimized window is blank)
    Start-Sleep -Milliseconds 1200           # let the UI finish rendering

    # Capture the window's own pixels via PrintWindow (immune to occlusion / z-order)
    $r = New-Object Win+RECT
    [Win]::GetWindowRect($hwnd, [ref]$r) | Out-Null
    $w = $r.Right - $r.Left; $h = $r.Bottom - $r.Top
    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $gfx = [System.Drawing.Graphics]::FromImage($bmp)
    $hdc = $gfx.GetHdc()
    $ok  = [Win]::PrintWindow($hwnd, $hdc, 2)   # 2 = PW_RENDERFULLCONTENT (needed for WPF)
    $gfx.ReleaseHdc($hdc)
    $bmp.Save("$env:TEMP\dx_verify_before.png", [System.Drawing.Imaging.ImageFormat]::Png)

    # Sanity: if PrintWindow failed or the frame is all black, fall back to a foreground screen grab
    $center = $bmp.GetPixel([int]($w/2), [int]($h/2))
    if (-not $ok -or ($center.R -eq 0 -and $center.G -eq 0 -and $center.B -eq 0)) {
        $gfx2 = [System.Drawing.Graphics]::FromImage($bmp)
        $gfx2.CopyFromScreen($r.Left, $r.Top, 0, 0, (New-Object System.Drawing.Size($w, $h)))
        $bmp.Save("$env:TEMP\dx_verify_before.png", [System.Drawing.Imaging.ImageFormat]::Png)
        $gfx2.Dispose()
    }
    $gfx.Dispose(); $bmp.Dispose()
}
finally {
    # ALWAYS stop the app — in `finally`, never as the last line: a throw above (no window, capture error)
    # would otherwise leave it running, and the leftover locks bin\Debug\*.exe against the next build.
    Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
}
```

### 5. Analyse the baseline and return a description

Use the `view` tool on the saved file:

```
view C:\Users\<user>\AppData\Local\Temp\dx_verify_before.png
```

Describe what you see around the target element — its current color, shape, borders, text, or whatever aspect the user wants to change. Keep it brief.

When the orchestrator named a target element, also **sample its current pixel value** (the `Sample` function in Part B step 3b) and include the hex with its coordinates in the description. Two later steps need that number: the search step confirms it found the right resource by comparing values, and Part B compares against it instead of against an impression.

**Sample the element's own surface.** An area that hosts other controls — a panel, a strip, a header — is mostly covered by children painted on top of it, each with its own background. A point that lands on one of those returns the *child's* colour, and a baseline hex taken there will later look like evidence that the search found the wrong resource. Pick a point on the bare surface between the children, take two or three, and if the hexes disagree say which point you consider the element itself.

**Return to the orchestrator:** the baseline text description, the measured hex(es) with coordinates, and the on-disk path of `dx_verify_before.png` (so Part B can re-read it) — **together with every other file you wrote**: each crop, zoom or scratch file, by full path. The orchestrator can only remove what it was handed, so a path you keep to yourself stays on the user's disk after the run ends. Do **not** return the image itself.

If the app window didn't open (screenshot is blank or the process exited immediately), do **not** ask the user yourself — return a **"baseline capture failed"** signal describing what happened (blank capture / immediate exit / build issue). The orchestrator owns what happens next. Manual navigation done between calls is thrown away - every capture is a fresh launch, so a screen is reached only as "Which screen to capture" describes.

---

## Part B: Post-Change Verification (Step 10)

Run this after applying the customization (after Step 9).

### 0. Confirm you were handed a baseline

The comparison is against **the baseline this run captured**, and the only thing that proves you have it is that the orchestrator **handed you its path**. Check that you were given one and that the file is there (`Test-Path`). If you were handed no path, or the file it names does not exist, stop **before** the rebuild and return a **"cannot verify" signal** (no baseline handed).

Do **not** look for a baseline yourself and do **not** fall back to a conventional filename. A capture left in the temp folder by an earlier run is a different app in a different state, so comparing against it produces a confident verdict about an image that has nothing to do with this change — a failure that is silent, which is worse than no verification at all.

The handed path is written `<baseline>` in the commands below; substitute it everywhere it appears.

### 1. Rebuild

Rebuild exactly as in Part A step 2 (`dotnet build "<path-to-sln-or-csproj>" --configuration Debug --no-restore 2>&1`). If the rebuild fails, do not proceed to the screenshot — return the **app-broken** verdict with the error: the applied XAML broke the build, which is a result the orchestrator must act on, not a "cannot verify".

### 2. Run the app and capture the "after" screenshot

Repeat the Part A step 4 capture exactly, changing only the saved filename to `dx_verify_after.png`.

### 3. Measure the target element — never judge by eye

**Every verdict must rest on pixel values you actually read out.** A verdict formed by looking at the image is unreliable and has produced confident, wrong reports — a thin stroke carries antialiasing that blends it into its background, a colour reads differently next to a coloured neighbour, and a small element is easily confused with the one beside it. "It looks white" is not a finding; `#FFFFFF at (742,18)` is.

**All of it is one command**, and it prints its answers as text:

```powershell
pwsh -ExecutionPolicy Bypass -File "<this skill's folder>/scripts/measure-render.ps1" -Image "$env:TEMP/dx_verify_after.png" -Baseline "$env:TEMP/dx_verify_before.png" -Color '<goal colour>' -At '<x,y>[;<x,y>...]'
```

It reports the value at each point in both captures, where the goal colour appears (count, bounding box, centre, and whether that box already held the colour in the baseline), and the changed-pixel count below the caption with its bounding box. The subsections below are how to **choose** what to ask it and how to **read** what it answers — run the command, then work through them.

#### a. Locate the element

**Locating the element is the only part an eye helps with — and there is a way to do it without one.** If you can view images, view both to find *where* the target element is:

```
view <baseline>
view C:\Users\<user>\AppData\Local\Temp\dx_verify_after.png
```

Name the element the orchestrator asked about — the element itself, not the area around it — and note its pixel coordinates. If it is small, crop that region, enlarge it, and view the crop.

**If you cannot open an image, you are not blocked — two measurements locate it for you, and neither needs eyes:**

- **The goal names a colour** → scan for it (`-Color`). The bounding box *is* the element's position, and its shape is already evidence: a single compact band is one row or one header; the same pixel count scattered across the window is the change spilling somewhere it was not asked to go. Use the reported centre as the sample point.
- **The goal does not name a colour** (a border, a font, a shape) → run with `-Baseline` alone. The changed-pixel bounding box is where the run's own edit landed, whatever it looks like.

⛔ In neither case is "I cannot see the picture" a verdict, and ⛔ never substitute an impression of what the screen *should* look like. Judging the result is not what the eye is for.

#### b. Read the pixels

**First prove the coordinate is on the element — a point you did not prove is not a sample.** A
coordinate counts as evidence only when the **before** image already carries the element's own visual
signature at that exact point: its stroke colour, its fill, its shape edge. Read the before-value and
say what it is *of* — "(56,111) = #6E6E6E, the chevron's stroke" — before you read the after-value. If
the before-value belongs to a neighbour (the label beside the glyph, the button face behind it), the
point is not measuring the element, and the fact that it later changed says nothing about the element.
This is a required check on every pass, not an assumption carried over from step a; it costs one extra
`Sample` call.

Then sample the **same** coordinates in both images and print the values:

```powershell
Add-Type -AssemblyName System.Drawing
function Sample($path, $x, $y) {
    $b = [System.Drawing.Bitmap]::FromFile($path)
    $p = $b.GetPixel($x, $y)
    "{0} ({1},{2}) = #{3:X2}{4:X2}{5:X2}" -f (Split-Path $path -Leaf), $x, $y, $p.R, $p.G, $p.B
    $b.Dispose()
}
Sample "<baseline>" <x> <y>
Sample "$env:TEMP\dx_verify_after.png"  <x> <y>
```

Sample **several** points, chosen deliberately:

- the part that was supposed to change — the fill, the stroke, the border line;
- for an area that hosts other controls (a panel, a strip, a header), a point on its **own bare surface**, between those children — a child is painted on top with its own background, so a sample landing there reads "unchanged" on an area that did change;
- one point just outside the element, to show the change stayed scoped and did not spill onto its surroundings;
- for a thin stroke or a glyph, a few points **along** it: a single sample can land on an antialiased edge and come back as a blend of stroke and background rather than the stroke.

#### c. Judge against numbers

- **The goal names a colour** (a hex, or one the orchestrator resolved) → each channel should land within about ±8 of it. Discard samples that read as a blend of two colours: they are edge pixels, not the element.
- **The goal names a colour word** → treat it as a range: white = every channel ≥ 240, black = every channel ≤ 24, and a mid colour by its own hex.
- **The goal is relative** ("darker", "more contrast") → the channels must move in the requested direction by a visible margin, ≥ 16 per channel, not by one or two.
- **The before and after values are identical** at every sampled point → nothing took effect, whatever the images look like.
- **A point that belongs to a separate control the goal never mentioned stays unchanged by design** — it keeps its own appearance. Report it as unchanged *and expected*, and do not let it turn the verdict into "partial": a panel asked to change colour is achieved when its own surface changed, even though the editors and buttons on top of it did not.

#### d. Count what changed everywhere else, not only at your sample points

Sampled points answer "did the target change". They cannot answer "did *anything* change", and those
are different questions whose answers separate two opposite diagnoses. Count the differing pixels over
the **whole** image as well:

```powershell
Add-Type -AssemblyName System.Drawing
$a = [System.Drawing.Bitmap]::FromFile("<baseline>")
$b = [System.Drawing.Bitmap]::FromFile("$env:TEMP\dx_verify_after.png")
if ($a.Width -ne $b.Width -or $a.Height -ne $b.Height) { "SIZE CHANGED $($a.Width)x$($a.Height) -> $($b.Width)x$($b.Height)" }
$n = 0; $minX = $a.Width; $minY = $a.Height; $maxX = -1; $maxY = -1
for ($y = 0; $y -lt [Math]::Min($a.Height, $b.Height); $y++) {
  for ($x = 0; $x -lt [Math]::Min($a.Width, $b.Width); $x++) {
    if ($a.GetPixel($x, $y).ToArgb() -ne $b.GetPixel($x, $y).ToArgb()) {
      $n++
      if ($x -lt $minX) { $minX = $x }; if ($x -gt $maxX) { $maxX = $x }
      if ($y -lt $minY) { $minY = $y }; if ($y -gt $maxY) { $maxY = $y }
    }
  }
}
"changed pixels: $n   bbox: x $minX..$maxX  y $minY..$maxY"
$a.Dispose(); $b.Dispose()
```

Report the count and the bounding box with the verdict, always. Two cautions on reading it:

- **The window caption is noise.** Activation state differs between captures, so the title-bar rows
  differ for reasons that have nothing to do with the change. Report the count for the client area
  separately from the caption rows (split at the caption's height, commonly ~40 px) and never fold
  caption noise into the result.
- **A count far larger than the target, with the target itself unchanged, is a finding — not a failure.**
  It means something you changed *did* take effect somewhere; the override simply did not reach the
  element. Say so explicitly. Switching a control family out of an optimized rendering path is the
  usual cause: the two paths lay items out with slightly different metrics, so the whole region
  reflows. Measured on a ribbon: turning bar items off the optimized path app-wide moves **12 265**
  pixels across the window while the arrow that was targeted moves **none**. A verdict of "none" that
  omits that number invites exactly the wrong conclusion — that the switch did nothing.

#### e. When the layout moved, compare the glyph — not the rectangle

Step d's count answers "did anything change". It also tells you whether step b's coordinates are still
usable: **a changed-pixel count far larger than the target element's own size means the layout
reflowed**, and a reflow moves elements out from under the coordinates you took in the "before" image.
Fixed points then faithfully measure whatever slid into them — a neighbour, a letter of a nearby label —
and report a real number about the wrong object.

So when the count outruns the target, do **not** reuse the coordinates:

1. **Re-locate the element in the "after" image from scratch**, the way step a located it in the
   "before" one — not by assuming it stayed put. Note how far it moved.
2. **Crop the element itself out of both images**: a tight box around the glyph, at its own position in
   each image.
3. **Align the two crops** by that offset and diff them pixel-for-pixel.

Read the result literally. **Identical modulo a translation is "none" — the element did not change**,
however large the count elsewhere in the window; it is never "achieved". Measured case: an arrow glyph
moved 2 px and the label beside it 3 px, a fixed 20×12 crop read "117 of 875 pixels changed", the glyph
was bit-for-bit identical, and the run reported the customization as verified working.

### 4. Return the verdict

Choose one verdict, stated in the values you measured:

| Verdict | Meaning | What to include |
|---------|---------|-----------------|
| **achieved** | measured values meet the goal | the sampled coordinates with their before → after hexes |
| **partial** | some sampled points meet the goal, others do not | which points changed, which did not, with their values |
| **none** | before and after values are identical — or, after a reflow, the re-located element crops are identical modulo translation (§3e) | the identical values, **the whole-window changed-pixel count and bbox from step d**, plus the likely cause: **the targeted element not being created at all** (an optimized rendering path — the one cause that voids the others, so name it first), wrong resource key scope, missing merged-dictionary entry, a duplicate key taking priority, or the value not coming from that resource at all (indirection). All of these are in `references/find-resource.md`. If the whole-window count is large while the target is unchanged, say which of these it *rules out* — the change reached the app, so "the element does not exist" is no longer the leading cause |
| **worse** | a regression is measurable or plainly visible | what broke, so the orchestrator can revert and re-approach |
| **app-broken** | the rebuild fails, no window appears, or the target element is now missing/blank | the stage it broke at and the error/blank capture, so the orchestrator can revert |

Return to the orchestrator: the **verdict**, the **measured samples** (coordinates + before/after hex), a **brief diagnosis**, and the **on-disk path of the "after" capture you saved** — the orchestrator removes it when the run closes, and it cannot remove a path it was never given. **List every other file you wrote as well** — the crops and enlargements from §3 are pictures of the user's application exactly as much as the full capture is, and they are the ones that get left behind, because nobody else knows their names. Do **not** return the screenshots.

If you could not sample the element — you could not find it in the capture, or the region is ambiguous — say exactly that instead of guessing a verdict from the image. An unmeasured element is an unverified one.

**Do not retry, edit XAML, or revert yourself** — the orchestrator owns the retry decision and the attempt budget. If the verdict is not "achieved", the orchestrator applies a targeted correction based on your diagnosis and re-invokes you for a fresh Part B pass. Your diagnosis is what it spends that budget on: say whether the values point at a wrong **value** or at a wrong **resource**, because those lead to different corrections.

---

## The captured window is not the active one

`PrintWindow` deliberately does not need the window in front, and an app launched from an automated shell does not get the foreground — Windows refuses to hand it over, so `SetForegroundWindow` on it fails. **Both captures are therefore of an inactive, unfocused window**, and that is fine for almost everything.

It matters in one case: appearance that the control changes on activation (active vs inactive caption brushes, focus-state borders). Such a property is *expected* to read as unchanged here, so a "none" verdict on it says nothing about the resource. When the target appearance could be activation-dependent, say so in your diagnosis and mark it **not verifiable by this capture** rather than letting the orchestrator spend attempts hunting a resource that was never wrong. Do not try to force the window forward to get around this — it does not work, and it is not what the failure is about.

---

## When to return a "cannot verify" signal

Return a "cannot verify" signal only *after* actually attempting to build, launch, and capture — never on the assumption that the environment is headless / "non-interactive" / has "no UI session". A WPF app launches and its window can be captured even from an automated, non-interactive shell; "no display" is a conclusion you reach only when the attempt itself fails (the window never appears, or the capture call errors).

If any of the following make automated verification impossible, stop and return a **"cannot verify" signal** with the reason — do not keep retrying, and do not ask the user yourself:
- **No baseline was handed to you** (Part B only): the orchestrator passed no path, or the file it names is not there. This is the one case you return **before** building and capturing — there is nothing to compare against, and a file you found yourself is not a baseline.
- The app requires a database, API keys, or non-trivial startup configuration that isn't set up in the current environment
- The target screen is reachable only through authentication or user interaction that cannot be automated - not merely because it is not the startup screen ("Which screen to capture" covers that case)
- The app launches but no window appears, or the screen-capture call itself errors (an actual failure, not a presumption)
- The app crashed repeatedly during capture

The orchestrator then reports the change as **unverified**, never as done — a green build does not prove it renders (SKILL.md's honesty gate). Verification is the deliverable's proof, not a bonus; "cannot verify" narrows what may be claimed, it does not make the change confirmed. (User opt-out is handled by the orchestrator at Step 1, before you are invoked.)
