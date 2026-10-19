# Figma input

Read this file only when the user gave a Figma link. Fetch everything once, write it to a local
cache, hand back paths. Every later pass of the run reads the cache, not Figma — re-fetching is slow, and it
drifts if the file changes between passes.

## Find the provider first

Figma is reached through an MCP server that the **user** configures; this skill never installs,
configures or authenticates anything, and it never asks for a token.

Check the tools available in the current session for a Figma provider (tool names containing
`figma`). Then:

- **A provider is available** → extract as below.
- **No provider** → stop and report it. Say that the Figma path needs a Figma MCP server
  configured in their agent, point at Figma's own setup documentation rather than restating it,
  and offer the alternative: they export the frame as an image, and the run continues from that
  image exactly as it would from a screenshot. That is a real fallback, not a degraded mode —
  the image path is the one most customers use.

Do not describe Figma's plans, quotas or access terms. Those are Figma's to state and they go
stale.

## Extract once

One pass, everything you will need, into the cache directory the orchestrator names:

```
<cache>/
├── frame.png          # rendered image of the frame - the visual reference for the run
├── structure.json     # the node tree: names, types, hierarchy, sizes, positions
├── variables.json     # design tokens / styles: colours, type styles, spacing (if exposed)
└── assets/            # exported icons and images referenced by the frame
```

Prefer the frame the user linked. If the link points at a whole file or page, list the frames
and ask which one — building the wrong screen is more expensive than one question.

`frame.png` is compared against the capture; `structure.json` is what makes Figma better than a
screenshot, since layer names and nesting reveal intent a picture only implies; `variables.json`
is needed only at `Detail`; `assets/` only for icons with no DevExpress equivalent.

**Ignore Figma's code generation.** It emits web markup — HTML, CSS, React — and translating
that into XAML produces a hand-rolled imitation of a layout rather than a control that behaves.
Use the structure as *information*, and choose controls the way `control-selection.md`
prescribes.

## What to return

Paths and a summary, never the tree itself:

```
frame: <name> (<width>x<height>) -> <cache>/frame.png
structure: <cache>/structure.json (<n> nodes, <depth> levels)
variables: <cache>/variables.json | not exposed
assets: <n> files -> <cache>/assets/
regions (top level, in visual order):
  - <layer name> - <type> - <position/size in words>
notes: <auto-layout used where it matters; text styles; anything the tree reveals that the image does not>
```

Layer names are a hint, not a specification. Designers name things after appearance
("sidebar-blue", "Rectangle 42"), and a name is never grounds to skip the functional question a
control choice turns on.
