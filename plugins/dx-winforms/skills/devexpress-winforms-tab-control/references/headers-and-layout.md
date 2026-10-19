# Headers and Layout

Position and orient tab headers, wrap them into multiple rows, show navigation buttons, and add header icons.

## When to Use This Reference

- Moving headers to the bottom, left, or right of the control
- Rotating header text (vertical headers)
- Wrapping headers into several rows when they don't fit
- Showing Prev / Next / Close buttons in the header panel
- Adding icons to page headers

## Header Position and Orientation

```csharp
xtraTabControl1.HeaderLocation = TabHeaderLocation.Left;     // Top | Bottom | Left | Right
xtraTabControl1.HeaderOrientation = TabOrientation.Vertical; // Horizontal | Vertical
```

`HeaderLocation` moves the whole header panel to one of the four edges. `HeaderOrientation` controls how header text is drawn (e.g., vertical text for side-mounted headers).

> Use the MCP tool to confirm the exact members of `TabHeaderLocation` and `TabOrientation` for your version if you need a value beyond the common ones above.

## Multiple Header Rows

When headers don't fit on one line, `MultiLine` wraps them into several rows instead of clipping:

```csharp
using DevExpress.Utils;   // DefaultBoolean

xtraTabControl1.MultiLine = DefaultBoolean.True;
```

To control individual header widths (e.g., to align a multi-row layout), set `XtraTabPage.TabPageWidth`:

```csharp
page1.TabPageWidth = 120;
```

## Header Buttons (Prev / Next / Close)

When `MultiLine` is off and headers don't all fit, the **Prev** and **Next** buttons let users scroll through them. Enable buttons via the `HeaderButtons` flags enum:

```csharp
xtraTabControl1.HeaderButtons = TabButtons.Prev | TabButtons.Next;

// Add the Close button to the set (flags — combine with |)
xtraTabControl1.HeaderButtons |= TabButtons.Close;
```

Control when the buttons appear:

```csharp
xtraTabControl1.HeaderButtonsShowMode = TabButtonShowMode.Always;
```

> `HeaderButtonsShowMode` has **higher priority** than `ClosePageButtonShowMode` (see [events-and-closing.md](events-and-closing.md)) — if the header Close button is hidden here, per-page Close settings can't override it.

## Page Header Icons

Set raster or vector icons through `XtraTabPage.ImageOptions`:

```csharp
// SVG (vector) — defaults to 32x32, so set the size explicitly
page.ImageOptions.SvgImage = svgImageCollection1[0];
page.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);

// Raster
page.ImageOptions.Image = imageCollection1.Images[0];
```

SVG icons from the DevExpress Image Gallery are added at the default 32x32 size; use `SvgImageSize` to scale them down.

## Source Material

- [XtraTabControl](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.XtraTabControl?md=true) — header layout members (`HeaderLocation`, `HeaderOrientation`, `HeaderAutoFill`, `MultiLine`, `HeaderButtons`)
