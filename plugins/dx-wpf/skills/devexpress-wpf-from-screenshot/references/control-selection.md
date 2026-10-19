# Choosing the controls

A mockup shows appearance; a control is chosen for **function**. Ask the right question about each
region, then pick the cheapest control whose behaviour answers it. ⛔ This judgment stays with you —
do not delegate it.

## The four rules

- **Function** — the control follows what the element does, not how it looks. Never pick a
  control because its default styling resembles the picture, and never degrade a control into a
  look-alike (a `Button` templated to look like a tab, a `Grid` of `TextBlock`s standing in for a
  data control) to win a visual detail. The customer will extend this screen; a look-alike has
  none of the behaviour they will reach for next.
- **Simplicity** — the cheapest control that does the job. A static region is a `Grid` or
  `StackPanel`. Reach for a DevExpress layout control when the screen needs what it adds:
  persisted layout, runtime rearranging, aligned labels across a form, dockable panels. Sizing is
  not one of those things: where a region has to absorb whatever space is left over, a plain `Grid`
  with a proportional row or column is the cheapest thing that does it, and a layout control chosen
  for filling space instead of for what it adds will fight you.
- **Behaviour** — where several controls fit, take the one that behaves the way the screen
  needs, not the one with the most features. Extra capability is not free: it brings its own
  API surface, its own layout rules, and its own way of being wrong.
- **Consistency** — where the application already solves the same task with a certain control,
  use that one. A screen that matches the rest of the app is worth more to the customer than a
  screen that matches the mockup slightly better.

## Ask what you already have before building it yourself

A region comes out wrong more often because a feature was never looked for than because the wrong
control was picked. Three shapes of the same mistake, all measured in real runs:

- **The host already owns the slot.** Command glyphs in a window's caption, a strip of items along
  its top edge, an area at its bottom: the DevExpress window exposes its own item collections for
  these. A heavier control brought in to obtain one of them arrives with chrome the mockup does not
  have, that chrome then has to be suppressed, and suppressing it usually takes the thing you came
  for with it.
- **The data control already has the feature.** A fill, a bar, an icon or a highlight inside a data
  cell is conditional formatting, and the data controls carry a `FormatConditions` collection for it
  (in the WPF grid: `FormatCondition`, `DataBarFormatCondition`, `ColorScaleFormatCondition`,
  `IconSetFormatCondition`). Painting it in a cell template is a look-alike of a feature that exists,
  and it throws away the rule that made the colour mean something.
- **The feature is there and its default output is what does not match.** A summary that renders
  `Sum=1234` where the picture shows `1 234,00` is not a colour gap and not a locale accident — it is
  a display format left unset. When a feature's output differs from the picture in its *text*, look
  for the format or template property before reporting it as a remaining gap.

So for every region, ask what the host and the controls already there offer for it, and check what
their output looks like by default. All three mistakes surface in a report as "the picture has
something we did not build", when the cause was never asking the control what it can do.

## The questions that decide

Work region by region. For each, answer the question before naming a control; the answer, not
the picture, points at the candidate family.

**A frame around everything (title bar, outer chrome).**
Does the app already use a themed window? It is not a precondition — a ribbon renders and works in
a plain `Window` — but inside a DevExpress themed window it merges into the window header instead,
which is what a mockup showing one continuous title-and-ribbon band is asking for. Match what the
app does now; switching the whole app's window type to serve one screen is not this skill's call.

**A strip of commands along the top.**
Is it a small fixed set of buttons, or grouped commands with categories, sizes and overflow?
Fixed and few → a toolbar-class surface. Grouped, with pages of commands, is what a ribbon is
for. The deciding signal is whether the mockup shows *categories of commands*, not whether the
buttons happen to be large.

**A list of destinations down one side.**
Ask three things: does it collapse to icons; are the entries grouped; and does selecting one
replace the whole working area or filter what is in it? "Collapses and groups" and "switches the
main content" point at different navigation control families, and the answer also tells you
whether the region is one control or a container plus a list.

**The main working area.**
Does it hold one thing at a time, several things the user switches between, or several things
visible at once? One thing → a plain content host. Several, switched → a tabbed document
surface. Several visible at once, resizable by the user → a docking surface. Several visible at
once, fixed → a layout panel, and nothing more.

**A region of tiles or widgets.**
Can the user add, remove, drag and resize them, and does that arrangement survive a restart?
That single question separates a tile/dashboard control from a simple wrapping panel of cards.
If the mockup is a static picture and the request says nothing about customizing, ask at the
announce step rather than assuming the expensive answer.

**Rows and columns of data.**
Is it flat rows, a hierarchy, or rows with expandable detail? Are there totals, grouping, or a
column-header filter row in the picture? Those features are what distinguish the data controls
from a plain list.

**Labelled input fields.**
Do the labels align in a column across the whole form, and should they stay aligned when the
window resizes or the localization changes? That is what a form layout control is for; a
two-column `Grid` is enough for a handful of fields that will never move.
For each individual field, pick the editor by the **data type and the interaction** — a date, a
number with a range, a lookup against a list, a masked code — not by how the box looks.

**Numbers, charts, gauges, maps, schedulers, diagrams.**
Each of these is a large component in a package of its own, and some — a dashboard or an embedded
report preview in particular — belong to a DevExpress product other than DevExpress WPF (Dashboard,
Reporting), with their own documentation and licensing. Name them at the announce step, and ask the
API & documentation phase which product and which package each one needs before building on it
(`SKILL.md`, step 4; the announce is step 5). Do not assert from memory that something is or is
not part of DevExpress WPF — that boundary moves between products and versions.

## Verify the control and its API against the project's version

A control that does not exist in the referenced version, or a property renamed between
versions, produces XAML that builds and then throws — or worse, silently does nothing. Before
writing markup for a control you have not used in this project already:

These are the questions the **API & documentation phase** answers — one batched request for all the
regions, delegated as `SKILL.md` step 4 describes (`references/api-lookup.md`). It holds the lookup
tools; what comes back here is the facts, ⛔ never the pages.

1. Whether the control exists in the version at hand, its required parent/child structure, and the
   properties you plan to set — from the DevExpress documentation for that version
   (`devexpress_docs_search`, `devexpress_docs_get_content`).
2. If the project references assemblies on disk, the assembly is the stronger authority — a
   quick reflection over the type's public members confirms names and enum values exactly.
3. With neither available, say plainly in the report that the API came from your own knowledge
   and needs checking.
4. How the control **arranges and sizes** its children — asked in the same batch, not only whether
   the properties you plan to set exist. Markup that compiles can still lay out nothing like you
   assumed: an alignment that reorders instead of centring, a width syntax that collapses an element, a
   "stretch" that does not fill. This is the single most common reason a render pass gets burned,
   and the control's own skill or documentation states these rules — reading them costs far less
   than a build, a launch and a capture.
5. What the control exposes for **how it looks** — asked of that same source, for the appearance
   the mockup plainly shows in this region. A group header's font weight, a chart's legend position and palette, an
   axis's label format, a series type, an alignment: on DevExpress controls these are ordinary public
   properties, and setting one while you write the region costs nothing beyond the lookup you are already
   doing. Not finding one is an answer too — record it, so the report names the gap instead of a later
   render pass chasing it (`SKILL.md`, steps 4 and 12). ⛔ Theme resources are out of scope here.

Also record **which package** each control needs, so the announce step and the final report can
name what gets added.

## Borrow the how-to, not the workflow

Once a control is chosen, the plugin usually already contains a skill for it. Read only the one
reference file that covers the markup you are about to write.

**Find that skill — do not assume where it lives.** The folders are named `devexpress-wpf-<family>`:
`devexpress-wpf-data-grid`, `devexpress-wpf-ribbon-and-bars`, `devexpress-wpf-layout-management`,
and `devexpress-wpf-mvvm` for the view-model style the inspection named — and most other control
families have one too. In the common layout they sit two levels up from this file, so
`../../devexpress-wpf-data-grid/references/` is worth trying first; but the host decides the layout,
and a project-local skills folder is as real as a profile-wide one. When the relative path is not
there, **search for the folder by name once** before you conclude anything about it — inside the skills
folders the host uses, ⛔ never a sweep of the disk.

⛔ One search, then stop — after it the skill is either found or unavailable for this run. If it is
unavailable, the documentation answers instead — through the same phase (`SKILL.md`, step 4).
⛔ Do not rank the two, in either direction: a sibling skill carries that family's patterns already worked out, the documentation carries the API as
it ships with the release, and which one serves a given control better is not decided here. ⛔ Do not spend further turns hunting for a skill
that is not there — and ⛔ do not let a failed lookup pass in silence: which source each control's markup came from is reported at the
announce step (SKILL.md, step 5), whether the skill was found or not.

Do **not** follow that skill's own workflow, and do not read its `SKILL.md` for guidance on how
to run the job. Those skills are written for someone building that one control on purpose; you
are assembling a screen out of several, and pulling in each one's full process would both
derail this run and cost more context than the rest of the run put together.

## Anti-patterns worth naming

- Another vendor's controls, or a hand-rolled control, when DevExpress or plain WPF covers it.
- Inlining rows of data into the view to make a capture look right (see SKILL.md, step 3).
- Changing the app's theme, or a global style, to close a colour gap. That is a different level
  of change and belongs to `devexpress-wpf-theme-customizer`.
- Picking a control you cannot verify exists in this version, on the grounds that the name
  sounds right.
