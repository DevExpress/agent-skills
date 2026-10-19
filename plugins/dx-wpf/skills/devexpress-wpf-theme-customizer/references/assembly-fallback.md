# Step 5 fallback: recover the template with NO installed source tree

Load this file when SKILL.md Step 3 reported that no installed DevExpress
source tree exists (all `XPF` root candidates absent) — a common real-customer case:
the "Source Code" installer component was never installed, whether the project
references the DevExpress **NuGet packages** or an **MSI install** of the components.
The installed-source grep in `references/find-resource.md` cannot run; recover the
shipped template from the compiled **assemblies** instead.

**This file replaces that file's greps, not its checks.** `find-resource.md`'s pre-check
("is the element your override targets created at all?") and its indirection checks are
questions about the **pipeline and the key**, not about where the template came from — they
apply here unchanged. Read it alongside this one; the pre-check below is where it starts.

**You receive** from the orchestrator: the target control/element, the desired change,
the active theme, the `major.minor` version, and the folder holding the version-matched
DevExpress assemblies plus which origin it is (project references, install `Bin`, or the
NuGet package cache — SKILL.md Step 3). The mechanism below is origin-agnostic: it reads
the assemblies, so any of those folders works the same way. **Do not ask the user
anything** — return the "not found" signal at the end if recovery fails.

This is a **mechanical sub-agent** task (Haiku class — see SKILL.md "Sub-agents"). It
is heavier and more fragile than the source grep, so keep it strictly to the no-source
case. The recipe below is verified end-to-end from the NuGet cache, from a modern MSI
install's `Bin\NetCore` (DX 26.1.3), and from an MSI install that has **no source component
at all** under `Program Files (x86)\DevExpress 20.1\Components\Bin\Framework` — the same
resource came out structurally identical in all three, and was diffed against the installed
`.xaml` source where that was available. The BAML layout has been stable across those
versions.

---

## Pre-check, before you extract anything: is the element built at all?

Extraction is the expensive half of this branch, and **all of it is wasted when the element the
override targets is never constructed.** A recovered template can be byte-perfect, keyed correctly,
applied correctly — and reach nothing, because the control does not draw itself that way. Settle this
first, and again the moment an applied override measures **zero** change.

**Bar and ribbon items are the case that bites.** In the themes that enable it, a `BarItem` renders
through DevExpress's own `DrawingVisual` "Render chrome" (`LightweightBarItemLinkControl`,
`dxcn:RenderTemplate`, `RenderArrowControl`, …) and the classic `Control`-based elements
(`dxb:ArrowControl`, `BarButtonItemLinkControl`, `BarSplitButtonItemLinkControl`, …) are **absent from
the visual tree**. Nothing warns you: clean build, app renders, override does nothing. It is decided
by the **theme**, not by where the item sits — and it is **not** the lightweight-*themes* flag from
SKILL.md Step 3, a name collision only.

The lever is public API, at **three scopes** — `UseLightweightTemplates="False"` on the **item**,
`UseLightweightTemplates` on the **link**, `CompatibilitySettings.UseLightweightBarItems = false` for
the **whole app**. Prefer the narrowest that covers the request: one item if one item was asked about,
app-wide only if the request itself is app-wide. Report which one the fix needs — the orchestrator has
to tell the user.

**Read `references/find-resource.md`, section "First: is the element your override targets created at
all?" (L96-159), before extracting** — it carries the theme-by-theme list, the three levers in full
(L126-141), why the other route (overriding the render chrome's own resources) **cannot** be scoped to
one item, and what to conclude once the path is switched off and the result is *still* zero. Extraction
is worth starting only after that section has been answered.

Whatever you find here, hand it to the orchestrator **with or without a key**: on this branch the
pipeline finding is frequently the whole answer.

---

## The two DLLs you need (both in the folder the orchestrator resolved)

- **Core assembly** — `DevExpress.Xpf.Core.v{ver}.dll`.
  Carries the `*ThemeKeys` types (the resource-key catalog).
- **Active-theme assembly** — `DevExpress.Xpf.Themes.{ActiveTheme}.v{ver}.dll`.
  Carries the shipped templates as **BAML** inside its `.g.resources`.

Both exist under every origin — only the folder layout differs:

| Origin | Where the DLLs sit |
|--------|--------------------|
| NuGet package cache | `…\.nuget\packages\devexpress.wpf.core\{fullver}\lib\{tfm}\DevExpress.Xpf.Core.v26.1.dll` — one package folder per assembly, so the theme DLL is under `devexpress.wpf.themes.{theme}\…` |
| MSI install `Bin` | `C:\Program Files\DevExpress {ver}\Components\Bin\{Framework\|NetCore}\` — **all** assemblies side by side in one folder |
| Project references | wherever the `HintPath` points (often a vendored `lib\` folder in the repo) |

Pick the framework flavor that matches the project's target: NuGet `lib\net8.0-windows`
(or `lib\net462`); install `Bin\NetCore` for .NET, `Bin\Framework` for .NET Framework. Then
**check the theme DLL is really in that folder before trusting the choice**. A flavor folder can
exist, be well populated, and still hold no WPF: a real DX 20.1 install was found whose `NetCore`
folder has 20 WinForms/XAF assemblies and no `DevExpress.Xpf.*` at all, with every theme assembly
under `Framework`. If the flavor you picked has no `DevExpress.Xpf.Themes.*`, take the
other one — for **reading** theme resources they are equivalent, the compiled XAML is the same.

---

## Part 1 — reflect the Core DLL to confirm the ThemeKey type + key names (cheap)

This replaces "guess the resource key". Load the Core assembly and list the `*ThemeKeys`
type whose name contains the control (e.g. `PagerControlThemeKeys`) and its key fields.
The field names ARE the valid `ResourceKey=` values (`Template`, `BorderTemplate`,
`NumericItemTemplate`, `NavigationButtonTemplate`, …). This tells you the exact key to
extract in Part 2 and the exact markup-extension type name (`PagerControlThemeKey`).

## Part 2 — extract the shipped template from the theme assembly's BAML

The theme assembly's `.g.resources` holds one `.baml` stream per control, named along the
source-tree layout, e.g. `core/core/editors/themes/generic/pagercontrol.baml` (the
theme-independent **structure**, where the main `Template` lives) and
`core/core/editors/themes/{theme}/pagercontrol/*.baml` (per-theme brushes/glyphs).

**Cheap path (do this):** enumerate the `.baml` stream names, **filter to those whose name
contains the control keyword**, and materialize only those (≈ a handful, not the ~1000+ in
the assembly). Materialize each with `Baml2006Reader` → `XamlServices.Load` into a
`ResourceDictionary`, find the key whose type is the `*ThemeKeyExtension` for your control
with the wanted `ResourceKey`, and `XamlWriter.Save` the value to XAML. Those keys stringify
roughly as `<ControlName>ThemeKeyExtension_<ResourceKey>` — e.g.
`PropertyGridControlThemeKeyExtension_TopPanelStyle`. Match by **substring**, never by an
exact string: the decoration around the name varies between versions (26.1 appends a
trailing `_`, 20.1 does not).

Some dictionaries throw on load (they reference resources unresolvable out of context) —
**catch and skip**. But two failures are **your setup's fault, not the dictionary's**, and
skipping them silently discards the very resource you want:

- `NotSupportedException: The URI prefix is not recognized` — surfaced as
  `XamlObjectWriterException: Set property 'ResourceDictionaryEx.Source' threw an exception`.
  The `pack://` scheme is only registered once a WPF `Application` exists. **Create one
  before loading any BAML** (`if (Application.Current == null) new Application();`). This
  hits every dictionary that merges another via `ResourceDictionaryEx.Source` — which is
  exactly where the **per-theme style definitions** live, so without it you extract the
  theme-independent structure and silently lose the theme's brushes and styles.
- `FileNotFoundException: Could not load file or assembly 'DevExpress.Xpf.<Control>.v26.1'`
  — you did not reference the control's own assembly (see the project files above).

**If the extractor prints nothing at all, assume one of these** rather than concluding the
resource does not exist. Zero output is indistinguishable from "everything threw and was
skipped" — check by logging the caught exceptions instead of swallowing them.

**Filter stream names by the SHORT control keyword** (`PropertyGrid`, `Grid`, `NavBar`) —
never the full type name. Which `.baml` holds a given style **varies by theme**, so the file
named after the control is not a reliable target: `TopPanelStyle` for `PropertyGridControl`
sits in `propertygrid/{theme}/propertygridcontrol.baml` under LightGray, MetropolisLight and
others, but in `propertygrid/office2019colorful/searchcontrol.baml` under Office2019Colorful.
Filtering on `PropertyGridControl` misses the latter; filtering on `PropertyGrid` finds both,
because the whole folder prefix matches. Never conclude "not found" from one stream.

### Verified extractor (paste-and-run; ~40 lines)

Create a throwaway console project that references the **exact same DevExpress assemblies
the customer's project uses** (version read from the project, SKILL.md Step 2), so those
assemblies and their dependencies resolve. Reference them the way the resolved origin
allows — the extractor code below is identical either way.

**If the origin is the NuGet cache** (the project uses `PackageReference`), reference the
**packages — not the cached DLLs by path**, even though Step 3 handed you a folder. Let NuGet
resolve and the transitive DevExpress dependencies come along on their own. A `HintPath` into
the cache skips that resolution and buys you a dependency hunt through repeated
`TypeInitializationException`s: measured, it took four extra assemblies (`DevExpress.Data`,
`DevExpress.Data.Desktop`, `DevExpress.Mvvm`, `DevExpress.Printing.Core` — the second not even
declared in the control package's nuspec) to get where three `PackageReference` lines land
with no hunting at all. This also works when nothing is cached yet: recent
DevExpress WPF versions are on **nuget.org**, so the build restores them without a
licensed feed or token — you do not need the customer to restore anything first.

```xml
<!-- extractor.csproj — NuGet origin -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
  </PropertyGroup>
  <ItemGroup>
    <!-- match the project's version + the control's package + the ACTIVE theme -->
    <PackageReference Include="DevExpress.Wpf.Core" Version="26.1.3" />
    <PackageReference Include="DevExpress.Wpf.Grid" Version="26.1.3" />
    <PackageReference Include="DevExpress.Wpf.Themes.Win11Light" Version="26.1.3" />
  </ItemGroup>
</Project>
```

**If the origin is the install `Bin` folder or a vendored `lib\` folder** (no packages to
restore), point `HintPath` at the DLLs directly. Reference **three** assemblies: Core, the
active theme, **and the control's own assembly** (e.g. `DevExpress.Xpf.PropertyGrid.v26.1`,
`DevExpress.Xpf.Grid.v26.1`). The control assembly is **not optional** — the theme
assembly's own dependencies come along automatically, but the control assembly does not,
and without it the control's dictionaries fail with
`FileNotFoundException: Could not load file or assembly 'DevExpress.Xpf.<Control>.v26.1'`:

```xml
<!-- extractor.csproj — install-Bin / vendored origin -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
  </PropertyGroup>
  <ItemGroup>
    <!-- DXBIN = the folder Step 3 resolved, e.g.
         C:\Program Files\DevExpress 26.1\Components\Bin\NetCore -->
    <Reference Include="DevExpress.Xpf.Core.v26.1">
      <HintPath>$(DXBIN)\DevExpress.Xpf.Core.v26.1.dll</HintPath>
    </Reference>
    <Reference Include="DevExpress.Xpf.Themes.Win11Light.v26.1">
      <HintPath>$(DXBIN)\DevExpress.Xpf.Themes.Win11Light.v26.1.dll</HintPath>
    </Reference>
    <Reference Include="DevExpress.Xpf.PropertyGrid.v26.1">
      <HintPath>$(DXBIN)\DevExpress.Xpf.PropertyGrid.v26.1.dll</HintPath>
    </Reference>
  </ItemGroup>
</Project>
```

Set `DXBIN` when building — `dotnet build -p:DXBIN="<resolved folder>"` — or inline the
absolute path. Put the extractor in its **own folder with no nested projects**: a stray
`.csproj` in a subfolder gets globbed into this one and the build dies on duplicate
`AssemblyInfo` attributes.

```csharp
// Program.cs — args: <ControlKeyword> <ResourceKey> <ThemeAssemblySimpleName>
// e.g.  PagerControl  Template  DevExpress.Xpf.Themes.Win11Light.v26.1
using System; using System.Collections; using System.Collections.Generic;
using System.IO; using System.Linq; using System.Reflection; using System.Resources;
using System.Text; using System.Windows; using System.Windows.Baml2006;
using System.Xaml; using System.Xml;

class P {
  [STAThread] static void Main(string[] a) {
    string kw = a[0], want = a[1], themeName = a[2];
    if (Application.Current == null) new Application();   // REQUIRED — registers pack:// URIs
    var core = Load("DevExpress.Xpf.Core.v26.1");         // adjust vNN to the version
    var theme = Load(themeName);
    // Part 1: confirm the ThemeKeys type + key names
    foreach (var t in All().Where(t => t.Name.EndsWith("ThemeKeys") &&
             t.Name.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)) {
      Console.WriteLine("KEYS " + t.FullName + ": " + string.Join(", ",
        t.GetFields(BindingFlags.Public|BindingFlags.Static|BindingFlags.Instance).Select(f=>f.Name)));
    }
    // Part 2: extract the template from BAML (keyword-filtered = cheap)
    var res = theme.GetName().Name + ".g.resources";
    var names = new List<string>();
    using (var rr = new ResourceReader(theme.GetManifestResourceStream(res)))
      foreach (DictionaryEntry e in rr) if (((string)e.Key).EndsWith(".baml")) names.Add((string)e.Key);
    int hit = 0;
    foreach (var n in names.Where(n => n.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)) {
      object dictObj;
      try {
        using var rr = new ResourceReader(theme.GetManifestResourceStream(res));
        Stream s = null; foreach (DictionaryEntry e in rr) if ((string)e.Key == n) s = (Stream)e.Value;
        dictObj = XamlServices.Load(new Baml2006Reader(s));
      } catch { continue; }               // dicts that need external context — skip
      if (dictObj is not ResourceDictionary d) continue;
      foreach (var k in d.Keys) {
        var ks = k?.ToString() ?? "";
        if (ks.IndexOf(kw, StringComparison.OrdinalIgnoreCase) < 0 ||
            ks.IndexOf(want, StringComparison.OrdinalIgnoreCase) < 0) continue;
        Console.WriteLine($"TRY  {ks}  in {n}"); Console.Out.Flush();  // survives a stack overflow
        var sb = new StringBuilder();
        System.Windows.Markup.XamlWriter.Save(d[k],
          XmlWriter.Create(sb, new XmlWriterSettings{ Indent=true, OmitXmlDeclaration=true }));
        File.WriteAllText($"extracted_{++hit}.xaml", sb.ToString());
        Console.WriteLine($"SAVED extracted_{hit}.xaml  key={ks}  in {n}");
      }
    }
  }
  static Assembly Load(string s){ try { return Assembly.Load(s); } catch { return
    AppDomain.CurrentDomain.GetAssemblies().First(x=>x.GetName().Name==s); } }
  static IEnumerable<Type> All(){ return AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(x=>{ try { return x.GetTypes(); } catch { return Type.EmptyTypes; } }); }
}
```

`dotnet run -- PagerControl Template DevExpress.Xpf.Themes.Win11Light.v26.1` writes
`extracted_1.xaml` = the real shipped `ControlTemplate`, plus one file per matched
dependency (glyphs/inner templates).

### One poisoned key can kill the process — log before you save

`XamlWriter.Save` recurses infinitely on some resources and dies with a **`StackOverflowException`**,
which .NET **cannot catch** — no `try`/`catch` saves you, the whole process is gone. A confirmed
example: `DataPagerThemeKeyExtension_AutoNumericButtonCountButtonContainerTemplate` in
`core/core/editors/themes/generic/datapager.baml` (DX 26.1).

So: **print the key and flush before calling `Save`** (`Console.WriteLine(...); Console.Out.Flush();`).
Then a crash names the offending key instead of looking like a mystery hang. Files already written
are valid and complete — re-run with a narrower `<ResourceKey>` to skip the poisoned one, or pass
the exact key you want. **Never conclude the resource is unrecoverable because the extractor
died**: check what it printed last and what it already saved.

---

## Clean the extracted markup before handing to Step 7

BAML round-trips faithfully but adds machine artifacts. Clean, do **not** restructure:

1. **Strip design-time attributes** — remove every
   `dx:BlendHelper2.ThemeInfo="Core\…\Foo.xaml;N;N"` attached property. It is a baked-in
   source-map, noise in an override.
2. **Theme-key references come out fully-qualified** as
   `{DynamicResource {someprefix:XxxThemeKey ResourceKey=…, ThemeName={x:Null}}}`, where the
   auto-generated `xmlns:someprefix` maps to `…/themekeys`. This is **valid and pasteable
   as-is**; you may rename the prefix for readability. `ThemeName={x:Null}` = the active
   theme (correct).
3. **`PART_` names are preserved** in the extracted markup (e.g. `PART_NumberItemsControl`)
   — a runtime visual-tree dump (below) is only a **confirmation**, not required.
4. Then apply the **same Step-7 discipline** (`references/xaml-rules.md`): its "Dynamic
   resources and `ThemeResource`" rule (convert inner `DynamicResource` theme-key refs →
   `dxi:ThemeResource` under `IsThemeIndependent=true`), preserve
   `PART_`/visual-state/`GradientStop` structure, and the recolor-vs-augmentation rules.
   The extractor gives you plain `DynamicResource` inner refs — that conversion still
   applies on top.

### Optional: confirm PART_ names via a runtime visual-tree dump
Only if the extracted markup is ambiguous about a part name. In a tiny WPF app, host the
control, call `ApplyTemplate()`, and walk `VisualTreeHelper` printing `x:Name`s — the
`PART_*` names appear. Skip when the extracted markup already shows them.

---

## What to return to the orchestrator

Same contract as `find-resource.md`:

### Found
- The extracted+cleaned resource block(s), each with its `x:Key` — including the **key type**,
  i.e. the `…ThemeKey` markup-extension class Part 1 confirmed, not the `ResourceKey=` name
  alone: the same name can exist under two key types and those are two different resources.
- The extracted dependency blocks (matched glyph/inner templates).
- The source: theme assembly name + baml stream path + version.
- The resource's **actual value** (the colour hex, number, geometry) and whether it matches the
  observed one the orchestrator passed you — or "matched by name only" when none was available.
  `find-resource.md`'s "Confirm the key by its value, not by its name" applies here too: BAML
  recovery finds the shipped resource, which is not proof it is the one in effect.
- Any indirection you noticed (the value living in an object rather than a brush, an attached
  property assigning it elsewhere, a context swap, a baked-in literal) and the candidates you
  rejected on value.

### Not found + what you tried
Only if reflection found no matching `*ThemeKeys` type OR no baml stream/key matched. List
the keyword(s), the ThemeKeys types seen, and the baml streams inspected. The orchestrator
will fall back to Step 5a (ask the user for a Snoop hint).
