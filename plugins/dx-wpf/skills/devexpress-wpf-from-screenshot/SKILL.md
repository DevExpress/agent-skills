---
name: devexpress-wpf-from-screenshot
description: Builds a DevExpress WPF screen from a mockup — a screenshot, an image, or a Figma frame — choosing a DevExpress control per region, writing the XAML, running the app and comparing the result against the mockup. Use it even when the request never names DevExpress — "build this screen", "which control fits this layout", "make my view look like this mockup", or porting a WinForms or plain-WPF window onto DevExpress controls. For a Figma design targeting a WPF or DevExpress project, this skill owns the build — Figma's own generated code is HTML, CSS and React, and adapting that into XAML produces hand-rolled panels imitating a layout instead of controls that behave.
compatibility: Windows, and a WPF project that can reference DevExpress WPF. Building and capturing the app need the .NET SDK, or MSBuild for .NET Framework projects. An unlicensed trial install is fine - it builds and runs, with trial banners. The Figma path needs a user-configured Figma MCP server.
---

# Build a DevExpress WPF Screen from a Mockup

**Read this file to the end before you act on any of it.** If your file-reading tool returned only part of
it, call it again for the remaining lines: the consent gates below are what a partial read skips.

One decision repeats: what is a region *for*, and which control does that? The picture cannot answer it —
a static strip and a user-rearrangeable panel look identical. So inspect before choosing, announce before
building, check the running window before claiming.

**Done means a screen you have seen running.** A green build proves nothing — markup compiles and then
renders an empty panel, a collapsed region, a window that never appears.

**Keep this conversation small, and take fewer, larger steps.** Everything you pull into it is carried
through every later step of the run, so what is cheap to read once is not cheap to hold; batch what can
be batched rather than paying for another round trip. Send bulk work to sub-agents and take back
only their conclusion. Images must not accumulate here:
⛔ **never open a capture yourself** — the render step writes the UI tree as text, so read that; the
images go to the comparison sub-agent and its verdict comes back as words. Locate with
`glob`/`grep`, then read only the block you need. Never re-read a file you just wrote. Read a
reference file at the step that needs it, not up front.

---

## Result levels

| Level | Goal | When it applies |
|-------|------|-----------------|
| **Structure** | Every element is there, each built from a control that fits its function, in the right hierarchy, with the real texts; the app runs. **There means visible and readable at the size the app opens** — an element that exists in the tree but sits off the screen, or in a card whose rows are cut, is not there for the user. Colour is not judged here — but a fill, bar, icon or highlight a control produces from a rule is a **feature**, not a colour, and so is the text a feature emits by default, so both belong here. | Always |
| **Look** | It reads as the same screen — layout, proportion, spacing, typography — expressed through the theme the app already uses and through the properties the controls themselves expose. | By default |
| **Detail** | Exact colours, metrics, fonts and glyphs, and the gaps that can only be closed by overriding theme resources. | On request |

The screen is **done** when Structure is complete and Look has no major gaps left. `Detail` is in play
only when the request asks for the exact colours, metrics or glyphs of the mockup; a plain "build this
screen" does not. Decide once, state it at step 5, and tell the comparison sub-agent which level it is
grading.

**Look is paid for when the markup is written, not with a render pass.** Where a control exposes a
property for something the mockup plainly shows — a group header in bold, a chart's legend position or
palette, an axis's labels, an alignment — set it as you write that region (step 4). The pass budget in
`references/render-check.md` deliberately refuses to spend a render on this class of gap, so a property
left unset up front is a gap that never gets closed.

**Detail is best effort, not a promise.** Where the template does not expose something, WPF cannot do it,
or matching it would cost the control its behaviour: name the limit, deliver the closest sensible result,
and stop. Gaps that need resource overrides go to step 12.

**The pass limit is three** render-check-fix cycles unless the user sets another number, and every pass
after the first **has to be earned**. The budget is in `references/render-check.md`; spending it well
starts before the first build, by reading a new control's layout rules
(`references/control-selection.md`) instead of discovering them one render at a time.

---

## Sub-agents

Run these four phases in a **dedicated sub-agent**: instruct it to read the named reference file first,
then return **only** the result that file specifies. Use inline instructions, not agent-definition files,
so this works in every harness. **If the harness has no sub-agents, read the reference file yourself and
do the phase inline.**

| Phase | Reference | Model |
|-------|-----------|-------|
| Figma extraction (step 1) | `references/figma.md` | cheapest available — it is mechanical fetching |
| Project inspection (step 2) | `references/inspect-project.md` | cheapest available — targeted search |
| API & documentation check (step 4) | `references/api-lookup.md` | **the main model** — a cheaper model that confirms an API which does not exist produces the failure this phase exists to prevent |
| Comparison (steps 7-9 and 11) | `references/compare.md` | **the main model** — the verdict is judgment, and a cheaper model summarizes away exactly the gap that matters |

Give the comparison sub-agent the **absolute path of `scripts/measure-render.ps1`** in this skill's
folder: that is how `compare.md` measures colour, and a sub-agent left to guess the path hand-rolled its
own sampler instead.

⛔ **Not delegated:** analyzing the mockup and choosing the controls — that is the judgment this skill
exists for, and it needs the whole picture in one head. ⛔ Do not nest further sub-agent layers: deep
nesting has broken convergence before.

Take a mechanical sub-agent's findings as given. Re-verify only when it flags uncertainty, or when one
finding alone decides something correctness-critical — and then check the claim (does the path exist, does
the control render) rather than repeating its search.

---

## The four questions worth stopping for

Announce, then act. Only four things genuinely cannot proceed without an answer, each asked with the offer
that follows it:

1. **No project to build into** (step 2) → offer a minimal host.
2. **A Figma link but no Figma MCP server** (step 1) → offer to continue from an exported image.
3. **Which screen to build** (step 1) → ask which, when several images may be one screen or several, or
   when a Figma link points at a whole file or page instead of one frame.
4. **A write that commits the DevExpress decision** (step 6) → stop *before* that write. Two writes commit
   it, and each is this gate on its own:
   (a) adding a DevExpress `PackageReference` to a project that has none;
   (b) writing the screen with no DevExpress control in it, while DevExpress is available anywhere you can
   reach it — referenced by this project, referenced by another project in the same solution or workspace,
   installed on the machine, or on a NuGet feed the machine is configured with. ⛔ This project having no
   reference is what makes this gate apply, never what exempts it.
   The call is the user's, never yours: no preference of your own (fewer dependencies, a tidier project,
   controls you know better) outweighs what you were asked for, and a dependency added to someone's
   project is theirs to approve. Say which control each region got, offer the DevExpress build, and **find
   the version yourself before you offer anything** — the one the project references, else the one
   installed on the machine. ⛔ Never report that no version is available without having looked; when you
   cannot determine one, **ask** which they have.
   ⛔ **Having announced the plan does not satisfy this** — it hangs on the write that changes the project
   on disk, never on an announcement a run can state and keep going past.

Everything else — including a region that needs a control from a different DevExpress product — is
announced and worked around, never waited on.

---

## Step 1 — Collect the mockup

One or more images, or a Figma link. **One screen per run**: several images are either one screen in
several states or several screens, and when that is genuinely ambiguous, **ask**. Several screens are
built one after another, each graded against its own mockup.

On a Figma request only, run the Figma sub-agent (`references/figma.md`). It returns a local cache and
paths; the rest of the run reads those files, never Figma again. Put that cache outside the project tree,
where the captures go.

Look at the mockup now and keep it for step 3; do not describe it in detail here.

## Step 2 — Inspect the project

Run the inspection sub-agent (`references/inspect-project.md`). It returns the DevExpress version, the
active theme, how views get their data, whether there is a DI container, how navigation works, what
renders at startup, candidate locations, and the build entry point.

⛔ Do not ask the user for facts that are in their own solution. Ask only when there is **no project in
scope at all** — then offer a minimal host and wait for consent (the first of the four questions above).

⛔ **Then run the app once, unchanged, before any of the draft is written — the size it opens at is what
you are designing for.** The same script and the same call as steps 7-9, under its own `-Out` name; what you
need from it is the window rectangle its tree file opens with, not the picture. Carry that into step 4 as
arithmetic, not as an impression: divide the client area by the rows and columns the mockup shows, and ask
whether what the mockup puts in one cell fits the height a cell actually gets. ⛔ Without it a draft copies
the mockup’s proportions instead — measured: one delivered a screen close to twice the window’s height
with a third of it never on screen, and the loop that follows will not redo a layout that late.
▶️ When the target is a fragment inside an existing screen, the rectangle that binds is that region’s,
⛔ not the window’s.
⛔ This render is **not a pass**: it costs nothing against the pass budget and nothing against
two-attempts cap.

⛔ **If the app does not build or does not launch here — before anything has been changed — stop and say
so.** Name the script’s exit code and what it means (2 build, 3 no exe, 4 no window). ⛔ Do not repair it:
the host arrived broken, someone else’s build is a different job with no agreed scope, and every later
step here assumes an app that runs — so a repair that half works hands back a screen nobody can trust.
Say what failed and ask whether to go on, and on what basis.
⛔ Skip all of this only when there is nothing to run — an advice-only run, or no project in scope.

## Step 3 — Analyze the mockup

Read `references/mockup-analysis.md` and work through the picture outside in. That file states what the
analysis has to produce.

## Step 4 — Choose the controls

Read `references/control-selection.md` and work region by region. In short: function decides, the cheapest
control that does the job wins, behaviour breaks ties, and where the app already solves the same task with
a control, use that one.

Verify every control and property against the project's DevExpress version before you write it, and ask
in the same pass **what the control already exposes for how it looks** — for every appearance the mockup
plainly shows in a region: weight, alignment, a header's font, a legend's position, a palette, an axis's
labels, a series type.

**Both questions go to the API & documentation sub-agent** (`references/api-lookup.md`), batched for all
the regions at once, and what comes back is the facts — ⛔ never the documentation pages. That is where the
lookup tools are used, because one documentation result is the largest single thing that can enter this
conversation and every later step then pays for it again. Delegated is the *lookup*; ⛔ the choice of
control is not.

⛔ Once it is delegated, do not run the lookup here as well: an answer that is thin, missing, or that
came back as a launch acknowledgement rather than facts goes **back to that phase**, ⛔ never to a search
from here. And ⛔ it is not only the DevExpress questions — a non-DevExpress documentation lookup the
screen needs (an icon font's glyph codes, a framework type) goes to the same phase.

⛔ Never write an API you have not confirmed exists; if you cannot confirm it, say so in the report. Set
the appearance properties that came back while you write that region.
⛔ Stop at the public API. A gap no property covers is named in the report and handed to step 12 —
⛔ never closed by editing the theme's resources inside this run, and ⛔ never invented: an appearance
property you cannot confirm is subject to the same rule as any other API.

Note which package each control needs.

## Step 5 — Announce the plan

State, in a few lines: the control chosen for each region, what the data will be bound to, where the
screen will be built, any package that has to be added, and the level you are working to — say so
explicitly if the request put `Detail` in play, because that changes what counts as done.

**Name where each control's markup came from** — the control's own skill, or the documentation because
that skill was not found. State it for **every** control, not only for the ones whose skill is missing: a
run that fell back to the documentation announced nothing, because it had never told itself it was
falling back. ⛔ State it plainly and ⛔ with no judgement either way — neither a warning about the
documentation nor a claim for the skill. Naming the source lets the user see what informed each region,
and install a skill that was not found if they want that family's worked patterns
(`references/control-selection.md`).

**If a region needs a control from a DevExpress product other than DevExpress WPF**, say which product,
and that unless the user has it you will approximate that region with what DevExpress WPF does provide.
Which product a control belongs to is a documentation question — check it there, never against a list
carried in this skill. Licensing is *not* the issue (an unlicensed project still builds and runs); a
control that is not in this product is.

Then start. This is an announcement, not a gate: the user redirects you if they want something else. The
DevExpress decision is gated at step 6, on the write that commits it — ⛔ not here, because a plan you
have stated is not a decision the user has taken.

## Step 6 — Build

Read `references/screen-location.md` and build the screen in the location the ladder gives you: the target
the user named, else a view that renders at startup, else a temporary preview window, else — with consent
— a minimal host.

Take a checkpoint before the first edit and keep a list of touched files, marked permanent or temporary.
If the target already contains markup, this is a rework: the customer's bindings, names, commands and
`DataContext` survive intact.

**Two writes need consent before you make them** (the fourth question). Each one settles the DevExpress
decision on the user's behalf, on disk:
- **Adding a DevExpress `PackageReference` to a project that has none** — name the version you would add,
  then ask.
- **Writing the screen with no DevExpress control in it, while DevExpress is available** — say which
  control each region would have got, then ask.

Ask, then **yield the turn and wait for the answer**: the stop is the wait, not the sentence. A project
that already references DevExpress, and markup that uses it, need no stop — build. Add the packages you
named, once consented to, matched to the project's DevExpress version.

**Data:** bind to an existing ViewModel or `DataContext` when one is in scope. Otherwise generate a small,
clearly named sample-data file and report it as a placeholder. ⛔ Never touch real data access.

⛔ **Never change the app's theme** to get closer to the mockup: express the screen through the theme
already active, so it survives a theme change. Switching themes is the user's decision alone.

## Steps 7-9 — Render, check, fix

Read `references/render-check.md` before the first pass: it holds the script's failure modes, the seven
tree questions, the markup review and the comparison rules. Three things bind here.

```powershell
pwsh -ExecutionPolicy Bypass -File "<this skill's folder>/scripts/build-run-capture.ps1" -Project "<sln-or-csproj>" -Out "<temp>/pass1.png"
```

⛔ **Wait for the script — never run it in the background.** Its two artifacts are the input to the check,
so a run that launches it and moves on has nothing to check, and one that ends its turn to wait may never
get another turn.

⛔ **`dotnet run` is not this step — and there is no other moment it belongs to either, including a "quick
sanity check that it launches", which is the use that actually defeats this step.** Measured, in the run's
own account: it had just fixed build errors, reached for `dotnet run` as the familiar fast way to see the
app come up, got "process is alive", and never came back for the render. It produces neither artifact, so
there is nothing to check and nothing to compare. **This script *is* the sanity check** — it builds,
launches, waits for the window and names through its exit code which of those failed (2 build, 3 no exe,
4 no window, 5 capture), at the cost of one call. ⛔ So never start the app any other way, for any reason.

⛔ **The artifact gate. Before anything in the check, and before any report: name the absolute paths of
this pass's `pass<N>.png` and `pass<N>.tree.txt`, and quote one line out of the tree file.** If you cannot
produce both paths and a real quoted line, you have not rendered — run the script. This is the gate
because an instruction can be silently judged "close enough" and skipped, while a named path and a quoted
line cannot be produced without the artifact existing.

⛔ **The same gate carries the tree's seven answers — as counts read off the file, not as prose.**
The seven are **Present, Laid out, Functional, Named, Data, Adaptive, Reflowed**; every one of them is a
number in that file or a line you can quote from it. Count them after each pass and report them on the
`Tree check` lines of step 11. ⭐ Counts are the form because a sentence can be judged "close enough" and
skipped, while a number that has to match the file cannot be produced without reading it — the same reason
this gate names paths instead of asserting a render. `references/render-check.md` says what each one asks,
what a bad answer means and how to fix it. ⛔ Owed whether or not anything here can look at a picture: a
measured run said *"I can't visually inspect"* and dropped all seven, which need no eyes and would have
caught eight of twelve elements missing from the live window.

## Step 10 — Integrate

Wire the screen in the project's own way — a Prism region the Prism way, a naming convention by that
convention, a code-behind `DataContext` the same. Wiring only: no business logic, no new DI container, no
MVVM framework introduced where none exists. Where the convention is unclear, do the simplest thing that
works and say in the report that you guessed.

Then remove everything temporary: the preview window, the startup redirect, scratch files, and any
capture left inside the project tree. Do this on failure and abort as well, not only on success.

## Step 11 — Prove and report

Render once more, **after** integration and cleanup: what you report has to be the state the customer
inherits, and step 10 made the wiring new and anything temporary gone. This matters most when the screen
was built in a preview window (`references/screen-location.md`, rung 3).

⛔ **Cleanup has just removed whatever made the screen visible, so decide how it is reached before you
capture, and prove from the tree that the window you captured is the one you built** —
`references/screen-location.md`, *Reaching the screen for the final render*. A run that skips this
photographs the app's own first screen and grades the wrong window, with a tree and a comparison that
both look entirely normal.

Check this last render as in steps 7-9 — the tree, then the comparison. A structural gap found here is a
defect: fix it and prove again. A Look gap that remains is reported, not chased past the pass limit.
**Report the number of renders you performed** — captures taken, not the fixes you grouped them into.

Then report, in this shape:

```
Screen: <name>, built in <location>
Controls: <region> -> <control> (<why, one clause>)  [one line per region]
Data: <bound to X | placeholder file Y>
Tree check, per pass: pass<N>, <path to pass<N>.tree.txt> - <total> elements, <n> editable / <n> read-only, <n> unlaid-out, <n> off-screen or clipped, <n> named after their control type, <n> data rows
Final render: <how the screen was reached> - identified by "<a line quoted from pass<N>.tree.txt that only this screen produces>"
Appearance API: <control> -> <property>=<value> (<what in the mockup asked for it>)  [one line per property set - or "none"]
Tree check, resize: <the "follows" line quoted from section 2>; reflow: rows level with the fixed element <width> vs below it <width>; fixed: <what changed, or "nothing">
Capabilities switched off: <editing, filtering, sorting, selection or any other behaviour you disabled, with the words in the request that asked for it - or "none">
Packages added: <package> (for <control>)
Matches the mockup: <what the final capture shows as right>
Deviations: <what differs, and why - platform limit, a control outside DevExpress WPF, deliberate simplification>
Placeholders: <sample data, texts, icons a developer should replace>
Not verified: <anything the capture cannot show>
Files changed: <permanent list>
Left behind: <anything temporary you could not revert, and what to restore - or "nothing">
```

**The honesty gate.** Check each claim against the evidence you hold: the tree for what exists, where it
is and whether it works; the comparison's verdict for how it looks. ⛔ Never claim what neither showed. If
the render failed, the screen is **unverified**, not done — say what is left to check. Focus-dependent
appearance, hover states and anything behind an interaction are in neither; list them as unverified, not
as matching.

⛔ **"Unverified" is a finding, not an exit — it has to name the command you ran and what it printed.** A
report blaming the environment, with no command and no output, is a guess about the harness. Measured
three times on two people's machines and withdrawn every time it was questioned — the run had not tried
`build-run-capture.ps1`, it reasoned by analogy instead of using the terminal tool it had available the
whole time; each then ran the script, the window opened and the capture came back. ⛔ **So never conclude
the environment is headless, has no display or has no capture tooling before attempting it.** "No UI
session" is a conclusion you reach *after* the script fails and names its cause.

⛔ **And this report cannot be written at all without the render's artifacts.** State the absolute paths of
the final pass's `pass<N>.png` and `pass<N>.tree.txt` and quote one line of that tree — or, if there are
none, the exact command you ran and the exact error it returned. Nothing else counts as either half of
this gate.

## Step 12 — Hand off styling

For gaps that remain after every public appearance property was set (step 4) and that can only be closed
by overriding theme resources: name those gaps, say that changing them is a different level of change
(theme resources, not layout), and offer to continue with `devexpress-wpf-theme-customizer`.
⛔ The offer **is** the handover — do not enter that skill's resource-override flow inside this run.

---

## Advice-only runs

Sometimes the ask is just *"which controls should I use for this?"* — no build. Then do steps 1, 3 and 4
and answer, reading only `references/control-selection.md`.

Still inspect the project if one is in scope (step 2): the DevExpress version decides which controls
exist, and what the app already uses is the strongest argument for what this screen should use. With no
project, name the version you assumed.

Say what you would have verified by building, so the answer is not mistaken for a tested result, and offer
to build it.
