# Path 3: Theme Resource Override (Steps 5–9)

Load this when Step 4 returned "no public API covers it", or the user already ruled the API out. Work
Steps 5–9 here, then return to `SKILL.md` Step 10 and verify. Taking this path licenses no shortcut
around verification, capture cleanup or the honesty gate.

## Step 5: Locate the Theme Resource (sub-agent)

Dedicated search sub-agent, reads `references/find-resource.md` first, then finds the resource. Pass: target
control/element, desired change, theme scope, resolved XPF root + theme folder — or, with "no installed
sources", the assembly flag + folder + origin instead: it then also reads `references/assembly-fallback.md`,
**in addition to** `find-resource.md`'s pre-checks, never instead of them.

Returns **one of**:
- **Found** — resource block(s), each `x:Key`, resolved `StaticResource` dependencies, source file + theme
  name → Step 6.
- **Not found + what it tried** → Step 5a.

### Step 5a: Resource not found — ask the user (orchestrator)

Get a uniquely-identifying string (specific enough that grep finds it in a handful of files), then
re-delegate to a new search sub-agent with the hint added:
- **Resource key:** Snoop → right-click the element → *Delve ControlTemplate* → the title shows the key.
- **Path geometry:** the `Data` value in Snoop.
- **Named element:** grep its `x:Name`.
- **Hex color/number:** a distinctive value from Snoop's property panel.
- **Screenshot:** the element selected in Snoop with its property panel visible, if none of the above is
  easy.

Be specific about which element (by appearance) and which tool.

## Step 6: Choose Resource Placement (orchestrator)

Default, don't block on a question:
- **1–2 resources** → inline in `App.xaml` `<Application.Resources>` (or the target window/control's
  `.Resources`).
- **Several, or a theming layer** → separate `ResourceDictionary` `.xaml` (e.g. `Themes/DxOverrides.xaml`),
  merged via `<ResourceDictionary.MergedDictionaries>`.

Apply the default, tell the user what you did and that it's changeable. Ask first only when genuinely
ambiguous or the user signaled a preference.

## Steps 7–9: Prepare and Apply the XAML (orchestrator)

Not sub-agent work — they modify project files:
- **Step 7** — read `references/xaml-rules.md`, prepare the customized XAML from the resource block +
  dependencies Step 5 returned.
- **Steps 8–9** — read `references/apply.md`, inject per the Step 6 placement, verify the build.

Then Step 10 in `SKILL.md`: same screen as the baseline, capture, measure, verdict. A green build is not
proof.
