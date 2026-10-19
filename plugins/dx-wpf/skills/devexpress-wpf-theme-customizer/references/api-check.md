# Step 4: Check for a Public API (sub-agent)

You are running as a **dedicated sub-agent**, kept on the **main model** — not a cheaper one, because a
cheaper model summarizes away exactly the documented limitation clause that matters here. Your job is to
verify whether a built-in property or style can do it before anything touches theme resources — cleaner
and upgrade-safe when it holds — and return a distilled finding to the orchestrator. Keep the search noise
inside this sub-agent; only the finding crosses back.

**You receive** from the orchestrator: the target control/element, the desired change, the theme(s) in
scope.

**You must NOT ask the user anything, and you must NOT apply or edit anything.** Return your finding;
the orchestrator owns the decision and the edit.

---

## Style property with `BasedOn`

Often cleanest (e.g. `TokenStyle`, `HeaderStyle`, `CellStyle`):

```xml
<Style TargetType="{x:Type SomeControl}"
       BasedOn="{StaticResource {someNs:SomeControlThemeKey ResourceKey=SomeStyle}}">
    <Setter Property="CornerRadius" Value="8" />
</Style>
```

> **Do not fabricate** — a style property is a solution here only when you set *documented setters* on a
> `BasedOn`-the-theme-default style. It does not license a hand-written `ControlTemplate`, an invented
> `x:Key`, or assuming a `*Style`/`*ContainerStyle` property restyles the target the way you imagine. Such
> a fake **usually builds clean but renders nothing** — report it as not covering the need instead of
> reporting a false find; the orchestrator will go to Step 5 and restate the real theme template.

## Scoped implicit style

For a composite control's children — TargetType-only style (no `x:Key`) in the parent's `Resources`:

```xml
<dxg:SearchControl.Resources>
    <Style TargetType="{x:Type dxe:ComboBoxEdit}">
        <Setter Property="Background" Value="#F0F0F0"/>
    </Style>
</dxg:SearchControl.Resources>
```

## Search broadly — terminology often doesn't match

If `devexpress_docs_search` is available to you:
- Search the customer's words, then DevExpress naming equivalents ("title"/"header", "caption"/"text",
  "foreground"/"background"/"color").
- `devexpress_docs_get_content` on the control's API reference; scan for style properties, appearance
  objects, drawing events, template selectors.
- Note a short candidate list (type, effect, doc URL) or explicit "nothing found" — **plus every stated
  limitation, quoted verbatim, never paraphrased**; "no limitation stated" when the docs are silent.

**"MCP not available" is a conclusion you check, not one you notice** — a configured server can be
**absent from a given agent's toolset** because harnesses defer tool schemas. Search your own tool list
for `devexpress_docs_search`/`devexpress_docs_get_content` **inside the sub-agent that needs it** — that
is you — rather than trusting what the orchestrator or an earlier turn saw. **Put the check in the delegation prompt** if you hand the docs lookup to a further sub-agent of your own — running it once yourself proves nothing about a child's toolset.

MCP genuinely unavailable → check local sources/assemblies instead, using the XPF root / assembly info
the orchestrator passed you (SKILL.md Step 3). Neither available → reason from DevExpress WPF knowledge
and flag the finding as less certain.

## Reflection probe — a cheap first pass

Reflect over the version-matched assembly (`.dll`) for the target type family's public appearance members
(`Foreground`, `Background`, `*Style`, `*Template`, `BorderBrush`, …) and relevant sub-element types.
- A listed appearance property → candidate; verify it against the active theme (below) before reporting
  it as covering the need.
- Otherwise, pass the exact sub-element type name back to the orchestrator as a Step 5 grep target.
- This doesn't reveal resource keys, template structure, or theme-honoring — it narrows what to look for,
  it does not replace the search above.

## Verify against the active theme before reporting "no API" — only with concrete doubt

A property existing is not the same as a property taking effect under the project's active theme
(orchestrator's Step 3). Spend the verification only when there is concrete doubt: the customer says it
"didn't work"; the docs mark it theme-dependent or partial; another control is integrated into the same
area (so the host's property for it goes inert); or you are about to report "no API found" as your final
answer. An ignored property under the active theme is what licenses reporting "none found" — *assuming*
it will be ignored does not.

---

## What to return to the orchestrator

Return **one** of the following:

- **Public API or `BasedOn` style covers the need** — the property/style, and every stated limitation
  quoted verbatim.
- **Scoped implicit style covers the need** — the `TargetType` and the parent whose `Resources` it goes
  in.
- **Partial** — what it covers, what it doesn't.
- **None found** — what you searched (docs pages, reflection probe result, active-theme check if you ran
  one) so the orchestrator doesn't repeat it.

Nothing else — no raw docs excerpts, no page text.
