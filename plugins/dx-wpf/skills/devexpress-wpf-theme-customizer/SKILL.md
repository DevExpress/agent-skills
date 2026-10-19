---
name: devexpress-wpf-theme-customizer
description: Customizes the appearance of DevExpress WPF controls — templates, brushes, colors, icons, borders, fonts, styles, any visual aspect. Use whenever someone asks how to change how a DevExpress WPF control looks, even if they don't say "template" or "theme." Checks for a public DevExpress API first; if none covers it, finds the matching XAML theme resource in the installed, version-matched DevExpress source and applies it, or returns ready-to-paste XAML.
---

# DevExpress WPF Appearance Customization

**Read this file to the end before acting.** If your reader returned only part of it, call it again for the rest — verification, cleanup and the honesty gate are stated later, and a partial read skips them.

> **Token efficiency: cost is context × steps, not output.** `grep`/`glob` to locate, then read only the needed range. Never re-read a file you just wrote, or re-open a source file for something a sub-agent already returned. Delegate bulk lookups to a sub-agent with a short return contract — keep raw payloads out of your context.

> **"Done" = a verified working change, not advice.** Apply the change and confirm it **renders** by default, even for a "how do I…?" ask. A green build is **not** proof. Propose XAML-only only when the user asked for a snippet / not to touch their project.

Three paths, in order of preference: **1. Public API** (property/style/event) · **2. Scoped implicit style** (TargetType-only, parent's `Resources`) · **3. Theme resource override** (copy + inject from installed theme source) — Steps 5–9, in `references/resource-override.md`.

Every path ends the same way: apply, then **prove it rendered** — Step 1 baseline, Step 10 check, for path 1/2 exactly as much as for path 3. Path 1/2 skips only Step 5 and the override files; never applying, verifying, or reporting honestly. Read `references/apply.md` when injecting into a `Resources` collection.

---

## Sub-agents for search-heavy phases

Dedicated sub-agent per search-heavy phase, returning only the distilled result: Step 5 (source search), Steps 1 & 10 (screenshots), Step 4 (docs).

- Instruct it to read the named reference file first, then return **only** the specified result.
- No agent-definition files — inline instructions, so it works the same in every harness.
- No sub-agent capability → do the phase inline yourself, using its own stated command. Missing sub-agents never skip a phase.
- **Model:** mechanical work (file sweeps, extracting a resource block, capturing/describing a screenshot) → Haiku-class. Judgment calls (API-meets-goal, Step 10 verdict, API-vs-override) → main model. Step 4 is delegated for *volume* but stays on the **main model** — a cheaper model summarizes away exactly the limitation clauses that matter.
- **Trust mechanical sub-agent output as-is.** Re-verify only when it flags uncertainty, or the finding alone decides a correctness-critical choice — by checking the claim (file/key exist, value matches the app), not repeating the search.

---

## Step 1: Understand the Request

Identify: which control/element, what changes (color, icon, border, shape, size, font, template…), which theme(s).

**Rebrand (a named theme color everywhere)** → the palette API from the docs (`devexpress_docs_search`, search "palette"; delegate as in Step 4), not per-control overrides. Referencing a theme color for a new element via `{dxi:ThemeResource …}` is unaffected.

**Ambiguous target** → ask a short clarifying question first, then search with both the customer's words and likely DevExpress naming equivalents.

**Verify visually — default whenever the app is runnable.** Don't stop at "here is the XAML" and don't ask permission first.

Capture the baseline — one command:

```powershell
pwsh -ExecutionPolicy Bypass -File "<this skill's folder>/scripts/build-run-capture.ps1" -Project <sln-or-csproj> -Out "$env:TEMP/dx_verify_before.png"
```

- **Precondition of the first file-edit call** on any project file, including the short route (one lookup → answer → edit).
- **Never background it** — wait for the result.
- **Pick the screen before you capture.** The capture photographs the window the app opens to, and most targets are not on it. `references/visual-verify.md` "Which screen to capture": the view holding the target, else the startup window as it is, else a view found by grep, else ask the user — reaching any but the second costs one temporary, reverted source edit.
- Keep the returned path — Step 10 compares against it.
- Sub-agent-eligible (`visual-verify.md` Part A, returns description + path) — optimizes *where* it runs, never *whether*.
- **No run ends with a capture still on disk.** Before your final message — **however the run ended and however early it stopped** — delete every capture path you were handed and say in one line what you removed. The procedure is in "Closing out the run" near the end of this file; stopping short of that section does not cancel the obligation.
- **Skip only** when the user asked for a snippet / not to touch their project, or a genuine attempt to build/launch/capture failed (no runnable project, no window, capture error) — then mark **unverified**, never done, and name the command you ran and what it printed. Don't skip on the *assumption* of "headless" — attempt first.

---

## Step 2: Detect the DevExpress Version

`glob` `**/*.csproj`, `**/packages.config`, `**/Directory.Packages.props`, `**/packages.lock.json`, then `grep` for `DevExpress\.Wpf`. Priority in that same order. Extract **major.minor** (e.g. `25.2`). No project files in scope → ask the user.

---

## Step 3: Detect Lightweight vs Regular Themes

Don't guess from package names — both may be referenced with only one active. From the project root:

```
grep -rl "UseLightweightThemes" <project root> --glob "*.cs" --glob "*.vb"
```

`= true` → lightweight; not found or `= false` → regular. The active theme also affects **behavior**: Step 4 verifies a direct property against it before ruling it out.

> ⚠️ Lightweight **themes** (this step) are not lightweight **templates** — an optimized rendering path some controls use *inside* regular themes, where the template elements a theme override targets are never created. "Regular themes" here settles nothing about that; `find-resource.md` checks it separately.

Locate the XPF source root — first existing:
1. `C:\Program Files\DevExpress {version}\Components\Sources\XPF\`
2. `C:\Program Files (x86)\DevExpress {version}\Components\Sources\XPF\` (≈v19 and earlier)

Theme folder under it: Regular → `…\DevExpress.Xpf.Themes\`; Lightweight → name varies (e.g. `…Themes.Lightweight\`, `…Themes\ThemesLW\`) — locate by search. Remember the resolved XPF root — later steps search under it.

No candidate exists → derive the install root from a `<Reference>` `HintPath` into a DevExpress install; only then ask the user for their install path.

**No installed source tree** (NuGet-only, or MSI without "Source Code") → record "no installed sources", locate version-matched **assemblies** instead. First hit wins:
1. What the project itself references (Step 2's `<Reference>`/`<PackageReference>`).
2. Install `Bin` folder (`NetCore` for .NET, `Framework` for .NET Framework, under `(x86)` for older versions) — **verify the flavor actually holds `DevExpress.Xpf.*`**, don't infer it from the target framework; confirm the version matches Step 2.
3. NuGet cache: `%USERPROFILE%\.nuget\packages\devexpress.wpf.*\{version}\lib\{tfm}\` (or `<RestorePackagesPath>` / `NUGET_PACKAGES`).

Pass the flag, resolved folder and origin to Step 5 (`assembly-fallback.md`).

**No origin yields the assemblies** → restore them yourself first: a throwaway project with a `PackageReference` on the known major.minor (nuget.org, no feed/token needed — see `assembly-fallback.md`). Ask the user only if that fails (older/non-public version, no feed access, restore error) — never ask an MSI-installed customer to "restore packages" they don't have.

---

## Step 4: Check for a Public API (sub-agent)

Dedicated sub-agent on the **main model**, reads `references/api-check.md` first, then verifies whether a built-in property or style covers the need — cleaner and upgrade-safe when it does. Pass: target control/element, desired change, theme(s) in scope (Step 3's active theme, for the against-theme check the reference file describes).

Returns **one of**: public API/`BasedOn` style covers it (with limitations quoted verbatim) · scoped implicit style covers it (`TargetType` + parent) · partial (what it covers) · none found (what was tried).

| Result | Action |
|--------|--------|
| Public API or `BasedOn` style covers the need | Baseline → apply → verify (Step 1/10); stop, don't go to Step 5. XAML-only + unverified on the Step 1 opt-out. |
| Scoped implicit style covers the need | Baseline → apply scoped to the right parent → verify; stop. |
| API partially helps | Note what it covers, continue. |
| None of the above | **Path 3** — read `references/resource-override.md`, work Steps 5–9 there, then return here for Step 10. |

---

## Step 10: Visual Verification — default; the deliverable's proof

Default whenever the app is runnable (per Step 1). Two commands, neither needs an image viewer:

```powershell
pwsh -ExecutionPolicy Bypass -File "<this skill's folder>/scripts/build-run-capture.ps1" -Project <sln-or-csproj> -Out "$env:TEMP/dx_verify_after.png"
pwsh -ExecutionPolicy Bypass -File "<this skill's folder>/scripts/measure-render.ps1" -Image "$env:TEMP/dx_verify_after.png" -Baseline "$env:TEMP/dx_verify_before.png" -Color '<the goal colour>'
```

**Never background a capture** — wait for it.

**Same screen as the baseline** — reuse Step 1's choice and re-apply its temporary edit here, then revert it. A verdict from a different screen compares nothing.

The measurement prints, as text: changed-pixel count + bounding box, where/how much of the goal colour appears, whether it was **already** in the baseline. Produce the **verdict** (achieved/partial/none/worse/app-broken) and a brief diagnosis from those numbers, never from an impression — "I cannot see the picture" is not a verdict (`references/visual-verify.md` has the full procedure).

Sub-agent-eligible (Part B) — returns the verdict, samples, diagnosis, and **the on-disk path of every file it wrote** (capture + any crop). No visible window, or the target element missing/blank, is a **failure** — feed into the retry.

Report the measured values to the user too. No baseline handed (Step 1 never ran, or failed) → Part B returns "cannot verify (no baseline handed)", report **unverified**. Never invent a path, and never let Part B pick a leftover file of its own.

**Retry is orchestrator-owned. Budget: two attempts per candidate resource, six verify passes per run.**
- **A goal that splits into visually distinct parts gets one track per part** — each with its own two attempts; the six-pass ceiling spans the whole run, so parts compete for it.
- **An unchanged neighbour control is not a miss** when the request doesn't cover it — report as unchanged-and-expected, spend no attempts. Ask the user when scope is genuinely unclear rather than assuming it.
- Once every other part is achieved and one part is stuck after its own attempts → **hand it over as a follow-up**, naming resources ruled out and the hypothesis left. Don't grind.
- **Two attempts buy one resource.** A verdict of "none" (identical measured values) means the value doesn't come from there — re-delegate with the indirection checks instead of tweaking value/scope, after settling: is the element even constructed at all (`find-resource.md`'s opening check)? A third attempt must name a **different** resource, not another tweak.
- **Stop as soon as the diagnosis stops changing** — the same values with the same diagnosis twice is thrash, not progress. Six passes is the hard stop regardless.
- **The budget is these numbers — not elapsed time**, and not your sense of how much effort the ask deserved.

Any stop short of "achieved": what was achieved, what remains, **the resources ruled out with the measured evidence for each**, and a next step — per part when several are in play.

---

## Closing out the run — remove the captures

Verification writes screenshots of **the customer's** application outside the project (the temp folder): the Step 1 baseline, and every Part B "after". The run is not finished until they are gone and the report says so.

**When:** once, at the very end — whatever the run ended on (achieved, stopped short, unverifiable, broken build, abort). Not at the end of Step 1, not between verify passes: every pass compares against that one baseline. Orchestrator-owned, since it alone knows the last pass has happened.

**What:** only the paths this run created and was handed — every path Part A/B returned. If a returned list looks shorter than the work it describes, ask for the missing paths first. **Never a wildcard over the temp folder** — other runs', and the customer's own, files sit there too.

Also remove any helper/scratch file the phase wrote to run. Never write any of it into the customer's project folder — call this skill's own `scripts/` by path instead of copying them, and don't delete those either.

```powershell
foreach ($p in @("<the baseline path Part A returned>", "<the after path Part B returned>")) {
    Remove-Item -LiteralPath $p -Force -ErrorAction SilentlyContinue
    if (Test-Path -LiteralPath $p) { "still there: $p" } else { "removed: $p" }
}
```

**Then say it in the report**, in one line from that output: what you removed, or — when something couldn't be (a lock, a path you were never handed) — what's still on disk and where.

---

## Before you report back — honesty gate

**Did you launch the app and see the change render — measured, not eyeballed?**
- **Yes, renders right** → report done, with the measured values behind it; note workarounds.
- **Yes, but wrong/blank/crashes** → not done; fix (Step 10 retry) or report what's broken.
- **No — only edited/built, or produced XAML** → do not claim it works. Say: *"I applied this but couldn't visually verify it — a green build doesn't prove it renders. Please run and check: (1) …, (2) …"*.

Writing "verified working" / "renders" / "achieved" is a claim about **your own measurement**, and it holds only when the measurement was of **the element itself**. **A pixel that changed at a fixed screen coordinate is not the element** — after a layout reflow that coordinate can sit on a neighbour. So write the claim with three things or not at all: **which element**, **which coordinates**, and **what makes those coordinates the element** — that in the *before* image the same point already carried the element's own stroke/fill, not a neighbour's. (Measured: "117 of 875 pixels changed in the arrow area" was the letter *a* of *Paste* moving three pixels, while the arrow was bit-for-bit identical — reported as verified.) Can't write that sentence → the verdict is **unverified**, re-invoke Step 10 for on-element samples (`references/visual-verify.md` §3).

The same rule governs claims about what was and wasn't tried. **Are you about to tell the user the customization is impossible? Then check what you actually tested.** Attempts against one resource rule out that resource, nothing more — the honest sentence names the resources ruled out and says the route is **not found**, not that it does not exist. Two things make the claim indefensible:
- **A route you named and did not try** — take it, don't declare a dead end.
- **A workaround offered in place of the answer** (restructuring the UI) — offer it as a fallback alongside what's untried, never as proof the ask was impossible.

**The cost of the change is part of the change — disclosed as a checklist, not as prose.** Write a section headed **What this costs** with **every** row below, each answered concretely or answered `none` — **and why**. Never merge rows, never answer with a reassurance ("standard practice"), never decide for the user that a cost is acceptable.

| row | answered with |
|---|---|
| **Reach** | which elements change **besides** the one named, and how wide the scope is. A keyed resource applies wherever its dictionary is in scope, so it reaches every element of that kind there. ⛔ What the fix *covers* ("only a split button in a ribbon group") is not what it *affects* |
| **Narrower route** | whether a per-instance route exists and what it costs; if you took the broader scope anyway, what stopped the narrow one |
| **What stops applying silently** | anything the change switches off/detaches/re-keys, that then stops rendering with no error and a clean build |
| **What you forked from the product** | anything restated instead of overridden — the user maintains that copy from now on |
| **What documented behaviour you gave up** | an optimization or supported path traded away, plus its visible side effects |

Three worked cases:
- A `ThemeName=`-scoped override is tied to that theme — the day the theme changes it stops applying silently, clean build, no error. State it, let the user decide.
- Switching off a control's optimized rendering path is a **scope choice across three scopes**: `UseLightweightTemplates` (**per item / per link**, narrow); `CompatibilitySettings.UseLightweightBarItems` (app-wide, repacks the whole ribbon's layout visibly). **Prefer the narrowest scope that covers the request** (`references/find-resource.md` L126-141 has both flags and which themes enable the path).
- **Styles keyed on the path you switched off**: with lightweight bar items off, a style on `LightweightBarItemLinkControl` has no effect and the per-kind types take over (`BarButtonItemLinkControl`, `BarCheckItemLinkControl`, `BarEditItemLinkControl`, `BarSplitButtonItemLinkControl`) — and the reverse with it on. No error either way. Don't rewrite the user's existing styles for them — name the re-keying risk.
