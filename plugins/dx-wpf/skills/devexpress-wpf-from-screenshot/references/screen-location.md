# Where the screen is built, and how it is left behind

Two things are decided here: **where** the markup goes, and **what state the project is in when the
run ends**. This skill edits a customer's project to see its own output; anything it added to make
the app render has to be gone by the time it reports, or the customer inherits a project that starts
into a preview window.

## Before the first edit: a checkpoint

Record what you are about to touch, so every temporary change is reversible even if the run
fails halfway.

- If the project is in git, note whether the working tree was clean (`git status --short`). A
  clean tree means `git status --short` alone tells the whole story at the end; `git diff` does
  not, because the files this run adds are untracked and never appear in it. A dirty tree means you
  must track your own edits, because the customer's uncommitted work is mixed in — never revert
  by discarding the working tree.
- Keep a running list of every file you create or modify, marked **permanent** (the screen
  itself, its ViewModel, sample data, package references) or **temporary** (a startup change, a
  preview window). The final report shows the permanent list; the temporary list must be empty.
- Never `git checkout`, `git stash`, or `git reset` the customer's tree. Revert your own edits by
  editing them back.

## The location ladder

Build the screen where it will actually live. Try these in order, and move down only when the
one above is genuinely not possible. Announce which rung you landed on — do not ask permission
for rungs 1 to 3.

**1. The target the user named.** A window, a `UserControl`, or a folder they pointed at. If it
already contains markup, this is a **rework** — see below.

**2. A place already visible in the app's live window.** The point of this rung is that the
screen can be seen without changing how the app starts or standing up a window of your own. This
is not only the view wired to `StartupUri` — it is anywhere inside the window the app actually
opens that already renders something: a docking layout's content area, a tab, an existing panel.
If the app has a live window and a container inside it that already shows content, that container
qualifies, even if nothing in the app renders straight from XAML alone with no code running.
"Already renders" is proven by rendering it, never assumed from a name or a registration: build,
run, capture, and confirm the region you are targeting is visible in the capture. A view that is
registered but only reachable after a login, a menu click, or further user interaction does not
qualify — that is rung 3.

**3. A temporary preview window.** Use it only when there is no live window that already renders
anything to place the screen into — the app has no window yet, opens straight to a login, or the
only route to a rendering location would change what the app does (wiring real navigation,
skipping a login) rather than just placing content where something already shows. Keep it minimal
and obviously temporary:

- one `Window` (a DevExpress themed window if the app uses one) whose content is the real view,
  so what you capture is the real markup, not a copy;
- the smallest possible startup redirect — the `StartupUri` in `App.xaml`, or a single line in
  `OnStartup` — noted as temporary the moment you write it;
- a name that cannot be mistaken for the customer's own code.

The view itself is permanent and stays where it belongs; only the window and the redirect are
temporary. When the run finishes, delete both and repeat the final check in the real location
(see below) — a screen that renders in a bare preview window can still break inside a docking
panel or a tab, because its parent supplies different size constraints.

**4. A minimal host** — only when there is no project at all, and only with the user's explicit
consent (SKILL.md, step 5). One window, a DevExpress package reference matched to a version you
name, a theme, and the view.

Tell the user which DevExpress version and target framework you chose. Recent DevExpress WPF
packages restore from nuget.org, so a host like this needs no licensed feed. That makes this rung
usable by someone still evaluating the controls, which is often exactly who asks. If the host does
not restore or does not build — a version that is not on the feed, a licensing error — leave it in
place, name the cause, and stop: the screen was never rendered, and clearing the cause is the
user's call, not something to work around. There is no earlier state to return to at this rung, so
deleting a project the user consented to helps nobody — and it must not be reported as done either.

## When the mockup is a whole window, not content

Some mockups are an entire application window: their own title bar, window buttons, a centred
window title, a menu or a search box drawn inside the chrome. That chrome only exists on a
`Window` — it cannot be reproduced inside a tab, a docking panel or a `UserControl` — so rung 2 is
not merely inconvenient here, it is impossible, and forcing the mockup into a container means
dropping regions the mockup clearly shows.

Build it as a real window, where the project keeps its own windows. **Then make it reachable from
the app's live UI with the smallest hook the project's own mechanism allows** — one menu item,
button or command that opens the window. Announce the choice, and do not ask permission for the
hook: a window that nothing opens is a screen the user cannot reach from their own running app, so
the hook is part of finishing the work, not a follow-up offer.

Do not confuse this window with rung 3's. Rung 3's window is scaffolding and is deleted at the end;
this one is the deliverable and stays, together with its hook. A startup redirect used to render
during the run is temporary either way and still gets removed — the final render is then taken the
way the user would reach it, through the hook.

## Reworking an existing screen

When the user asks to make an existing view look like the mockup — or the target you chose
already contains markup — everything the customer wrote is load-bearing until proven otherwise:
`Binding` paths, `x:Name`s, `Command` wiring, `DataContext` assignment, attached behaviours,
event handlers in code-behind, and localization references. A screen that looks right and no
longer binds is a worse outcome than no change at all.

- **Small changes go straight into the file** — retargeting a panel, adding a region, adjusting
  a layout container.
- **A big restructure is built alongside** the original and put in place only once the user
  agrees. Keep the original until then.
- **Swapping one control for another is offered, not done quietly** — say which control, what it
  changes about behaviour, and what the customer would have to re-wire.
- When markup has to move, carry its bindings and names with it verbatim. If a binding cannot
  survive the new structure, say so in the report instead of dropping it silently.

## Reaching the screen for the final render

The final render (`SKILL.md`, step 11) is taken **after** integration and cleanup — and cleanup has just
removed whatever made the screen visible during the run. The app now opens where it always opened, so a
capture taken without thinking about it photographs **the app's own first screen, not the one you built**,
and the tree and the comparison then grade the wrong window while both look entirely normal. Measured on a
reviewer's run: the last capture came back as the app's opening screen.

Pick how the screen is reached, in this order, and stop at the first that answers:

1. **It is already on the window the app opens to** — the ladder's rung 2, or a window mockup reached
   through the hook you added. Capture as is, no source change.
2. **One temporary, reverted edit makes it the screen the app opens to** — select the tab or panel you
   placed it in (`SelectedIndex`, `IsSelected`), or point the startup at it (`StartupUri`, or the shell's
   default view). Copy the original line verbatim before you change it.
3. **It is reachable only through a login or an interaction that cannot be automated** — say so, and
   report the screen as unverified **in its final location**, naming what you did verify and where.

⛔ **This edit is made after cleanup and reverted after the capture, never the other way round**,
and the revert is confirmed by re-reading the line. An aborted run must not leave the user's app opening on
a different screen; a revert that failed is a change left in the customer's project and goes on the
report's `Left behind` line.

**Then prove you captured the right screen.** The tree file names what is in the window: quote from it a
line that only the new screen produces — its own heading, a control it alone contains, a row of its sample
data — on the report's `Final render` line. ⛔ A capture you cannot identify that way is not the final
render, whatever it looks like.

## Cleanup

Wiring the screen in, the final render and how it is judged are SKILL.md steps 10-11. Two things are
owed here and nowhere else:

1. Confirm the project is back to "your permanent changes only". With a clean starting tree,
   `git status --short` plus `git diff --stat` shows exactly that; otherwise walk your own list.
2. Anything that could not be reverted goes in the report explicitly, with what the user should
   restore by hand.
