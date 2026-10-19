# Render, check, fix

Read before the first render. `SKILL.md` holds the artifact gate — which demands the seven tree answers —
and the pass limit; this file holds how a pass is run and judged, and what each of the seven answers
means. Every rule here applies to every pass.

## Run the script

```powershell
pwsh -ExecutionPolicy Bypass -File "<this skill's folder>/scripts/build-run-capture.ps1" -Project "<sln-or-csproj>" -Out "<temp>/pass1.png"
```

Use `pwsh` and the explicit `-File` form: it runs from any shell, `cmd` included. ⛔ Never invoke the
script as a bare `.ps1` path — in `cmd` that opens the file in an editor and waits, so the failure arrives
as a command that never returns instead of as a wrong-shell error.

**Read a refusal as a machine setting, not a broken script.** Windows PowerShell can refuse the file —
*"is not digitally signed. You cannot run this script on the current system."* A Group Policy execution
policy overrides the `-ExecutionPolicy` switch, and an installed skill folder can carry the mark saying
its files came from the internet, which is what such a policy refuses; `pwsh` reads its own policy and is
normally unaffected. If only `powershell` is available, clear the mark once — `Unblock-File "<path to the
script>"`. If that is refused too, **say so and continue without the capture**, reporting verification as
not done rather than the screen as unfinished. ⛔ Never retry the refused command, and ⛔ never substitute
a hand-rolled screen grab: it captures whatever window is in front, which is how a run concludes its own
window is covered by another one.

Always a full rebuild; never XAML hot reload — a partial update makes the capture disagree with the
source, and every later comparison then measures the wrong thing.

**Capture at the size the app itself opens.** A WPF screen is adaptive, so forcing the mockup's
proportions grades a size the customer never sees: it hides layout that breaks at the real size and
invents gaps that exist only at the forced one. `-Width`/`-Height` are for the one case where the app's
own size makes the capture useless — a window sized to its content, or one that opens too small to read.
Then keep that size identical for every pass and say so in the report.

Write captures outside the project tree.

The script fails loudly and names the cause; read what it printed rather than guessing. One cause is
worth knowing in advance: **a window that never appears usually means the app threw on startup**, not
that the capture is broken. Never proceed on a render you did not get, and never fall back to describing
what the screen "should" look like.

One launch returns two artifacts:

- `pass<N>.png` — the window.
- `pass<N>.tree.txt` — the UI tree of that same window, read from outside the process through UI
  Automation, plus a second reading at a larger window. **Text: read it yourself.** No code is added to
  the app to produce it, so there is nothing to clean up afterwards.

If the script warns that the tree could not be read, the PNG is still written: judge that pass with the
comparison sub-agent (`compare.md`) instead, and say so in the report.

## The seven answers — where each count comes from, and what a bad one means

`SKILL.md`'s artifact gate **demands** these as numbers on the report's `Tree check` lines. Here is what
each number is read from and what it tells you when it is wrong.

- **Present** — the element count, and every mockup element found by its own text. Fewer than the mockup
  has is the commonest failure and it does not look like one: the markup is there, the element is not.
- **Laid out** — how many visible elements report `no rect`, a zero size, `offscreen` or `outside parent`.
  Each means the element is in the markup and not on the screen, and **which of the four it is, is the
  cause** — the fix follows from it. Report them as **two** numbers, because they fail differently:
  never laid out (`no rect`, zero size), and **off the screen or clipped** (`offscreen`, `outside
  parent`) — the second is content the customer cannot see, and it is the one a squashed card produces. Chrome that is legitimately hidden is not counted here; if you count
  it, say which.
- **Functional** — `editable` against `read-only`. One field the user is meant to fill reporting
  `read-only` or `disabled` without being asked for is a defect, ⛔ not a cosmetic gap. A cell reporting a
  different control type than the cells beside it is usually a static stand-in where an editor belongs.
- **Named** — how many inputs carry their own control type as their name (`Edit "TextEdit"`). Each one is
  a label that exists as loose text beside its field instead of belonging to it. This count is normally
  zero once the labels are attached, so any non-zero is the finding.
- **Data** — the row count from `Row 1 of N`, and the row texts. Rows you cannot trace back to the mockup
  or to the sample data you wrote are invented.
- **Adaptive** — from the tree's **second** section, the quoted line for the region you decided absorbs
  the leftover space; it has to follow the window in both directions. Anything you did not intend to be
  fixed should follow in at least one. A screen whose top-level elements number in the dozens was built as
  one flat pile, not as regions.
- **Reflowed** — two widths: the rows level with the fixed neighbour (`mockup-analysis.md`) against the
  rows below it. **Equal widths mean it did not reflow at all** — and a layout that puts the fixed element
  in its own column can never reflow, however the sizes are set. Check the first widened element is the
  one the mockup widens, not an earlier row.

Fix what this finds immediately: a defect the tree exposes does not spend a fidelity pass. Not being able
to judge the *look* removes the comparison below, and nothing else. ⛔ Never let it remove the seven.

## Then read the markup you wrote

The tree cannot see it — a `Grid` or a `StackPanel` is not in it at all — and these are the defects that
outlive the run, because the customer keeps the file:

- Every hard-coded size traces back to something with an intrinsic size (`mockup-analysis.md`). The others
  are removed, not adjusted.
- Every container earns its place: it groups, aligns, or divides space. One that wraps a single element,
  or repeats what its parent already does, comes out.
- Every construct is one the control's own documentation or sibling skill shows
  (`control-selection.md`). A construct you cannot name a source for is a guess, and the report says so.
- Nothing exists only to make the capture resemble the mockup.
- **No capability is switched off unless it was asked for.** A picture cannot tell you that editing,
  filtering, sorting or selection should be gone, so leave them alone. This is the most damaging mistake
  on this list and the hardest to see: a screen missing a behaviour looks correct in a capture, and
  usually in the tree too. Hiding **chrome** the mockup does not show — a group panel, a search box — is a
  different thing and is right, because the default would otherwise put something on screen the mockup has
  not got. The test is whether a capability the user needs disappeared, not whether something visible did.
  One switched off anyway is named on the report's `Capabilities switched off` line (`SKILL.md`, step 11),
  quoting the words in the request that asked for it — with nothing to quote, put it back.
- **No property is set to the value it already has.** Setting one "to be safe", without knowing its
  default, is how a file fills with noise the customer has to interpret. Worse is the same fact stated
  twice by two mechanisms — a colon typed into a caption *and* a property that appends one — where one of
  the two is necessarily wrong. Look the default up (the referenced assembly is the strongest authority,
  then the documentation) and set only what differs from it.

## Then the comparison

Run the comparison sub-agent (`compare.md`) on the mockup and this pass's capture, telling it which level
is in play and what the previous pass reported. It returns an ordered list of gaps and a verdict; **no
images come back** — that is the point of delegating it. The tree settles what exists, where it is,
whether it works and whether it reflows; the comparison settles what only eyes can judge — proportion,
spacing, weight, colour.

**If the verdict is not enough to act on, ask again — do not take over.** A vague answer ("the proportions
look a little off") means the question was too broad, not that delegating failed. Call the comparison
again on the same two images with the one thing you need settled and the form you need it in: which of two
elements is wider, the measured hex at a named spot, whether a named region is clipped. ⛔ Do **not** open
the capture, crop it, zoom it or sample its pixels yourself — a measured run spent half its wall clock on
ad-hoc pixel probes over its own captures, took a wrong measurement from them, changed the markup and had
to revert. The sub-agent has the same tools you do and not the context budget you are spending: two narrow
follow-ups still cost less than one image in this conversation.

**With no sub-agents that prohibition still stands — and it never meant "do not measure".** What it
forbids is *ad-hoc* probing. Measuring is one command with a fixed text answer:

```powershell
pwsh -ExecutionPolicy Bypass -File "<this skill's folder>/scripts/measure-render.ps1" -Image "<pass2.png>" -Baseline "<pass1.png>" -Color '<colour in question>' -At '<x,y>'
```

It prints the value at each point, where a colour appears with its bounding box, and what changed between
two passes with its box — so the look can be settled without an image entering this conversation and
without an image viewer existing at all. ⛔ "I cannot see it" is not a result: say what you measured, or
say the comparison was not done and mark the pass unverified.

## Fix

Apply the gaps that matter most, then render again.

- **Revert, do not stack.** If the verdict says something got worse, undo that change instead of adding
  another on top. Keep the last good revision.
- A structural gap outranks every cosmetic one, however small it looks in the picture.
- Stop when the screen is done (Structure complete, Look without major gaps) or the pass limit is reached.
  Reaching the limit is **not** success: stop and report what is still missing.

## The pass budget

**Every pass after the first has to be earned, and only `Structure` or `Function` can earn it.** Before
rendering again, name the one thing missing or wrong, and which level it belongs to:

- **`Structure` or `Function`** — an element absent, a region built from the wrong control, a field that
  cannot be edited, a capability missing, data unbound, something clipped or not laid out at all. Fix it
  and render: the pass is earned.
- **Anything else** — spacing, margins, row height or density, a caption a few pixels off centre, column
  width ratios, exact greys, an accent fill, a glyph. **You may not spend a pass on it.** Name it in the
  report as a remaining gap and stop. This is `Detail`, and `Detail` is on request only.

Three hard stops that need no judgement:

1. **A render identical to the previous one ends the loop.** Nothing you changed reached the screen.
2. ⛔ Never render to confirm that something looks better. A render answers one question only: is a named
   defect gone.
3. ⛔ Never render to chase a difference you have already decided not to fix, or one you have judged
   risky, theme-owned or unreachable through public API. That decision is final: write it down and move on.

Eight passes has been measured at roughly twice the tokens and twice the wall time of two, with the extra
passes changing nothing the customer would notice. The budget governs *fidelity* iteration only: a render
that reveals your markup does not do what you assumed — a collapsed element, a vanished column, a region
that refused to fill — is a defect to fix, not a pass — but **two attempts at the same gap, not
more**. If it survives the second fix, name it unfixed in the report and move on: a third repeats the
second's wrong cause, and half a measured run's cost went that way.
