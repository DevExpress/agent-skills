# View Modes

`AccordionControl` supports two top-level view types and several sub-modes that control how the control expands, collapses, and occupies space. All settings live on the `AccordionControl` instance.

## When to Use This Reference

- Switching between Accordion and Hamburger Menu styles
- Configuring the Hamburger Menu display mode (Inline / Overlay / Minimal)
- Using the Footer root-element mode
- Controlling expand/collapse behavior and animation

## ViewType — Accordion vs Hamburger Menu

The `AccordionControl.ViewType` property (`AccordionControlViewType` enum) is the master switch:

| Value | Description |
|---|---|
| `Standard` | Classic always-visible accordion sidebar. No Hamburger button. |
| `HamburgerMenu` | Adds a Hamburger (☰) button in the header; the control can be minimized to an icon strip. |

```csharp
// Switch to Hamburger Menu style
accordionControl1.ViewType = AccordionControlViewType.HamburgerMenu;
```

## Hamburger Menu Display Modes

When `ViewType = HamburgerMenu`, the `AccordionControl.OptionsHamburgerMenu.DisplayMode` property
(`AccordionControlDisplayMode` enum) controls the expanded/collapsed visual behavior:

| Display Mode | Collapsed State | Expanded State |
|---|---|---|
| `Inline` | Narrow icon-only strip on the side | Expands in-line, pushes form content aside |
| `Overlay` | Narrow icon-only strip | Expands as a floating overlay on top of content |
| `Minimal` | A single Hamburger button (or small top bar when docked Top) | Expands as an overlay on top of content |

```csharp
accordionControl1.ViewType = AccordionControlViewType.HamburgerMenu;
accordionControl1.OptionsHamburgerMenu.DisplayMode = AccordionControlDisplayMode.Overlay;
```

### Controlling the Expanded/Collapsed State Programmatically

```csharp
// Collapse (minimize)
accordionControl1.OptionsMinimizing.State = AccordionControlState.Minimized;

// Expand
accordionControl1.OptionsMinimizing.State = AccordionControlState.Normal;
```

### Popup for Groups When Minimized

When the accordion is minimized, clicking a group header in the icon strip opens a floating popup that shows the group's children. Use the following methods to manage this programmatically:

```csharp
// Open popup for a group (when control is minimized)
accordionControl1.ShowPopupForm(grpContacts);

// Close the currently open popup
accordionControl1.ClosePopupForm();
```

## Footer Root-Element Mode

By default, root-level elements (those in `AccordionControl.Elements`) are stacked vertically in the control body. An alternative is the **Footer** mode, where root elements act like tabs at the bottom of the control — clicking one shows its children in the main area.

```csharp
// Enable footer tab-style root elements
accordionControl1.RootDisplayMode = AccordionControlRootDisplayMode.Footer;
```

Additional footer options:

```csharp
// Show both the active group header and its content (default: content only)
accordionControl1.OptionsFooter.ActiveGroupDisplayMode =
    ActiveGroupDisplayMode.GroupHeaderAndContent;

// Align a specific item to the far (right/bottom) side of the footer
mySettingsItem.ControlFooterAlignment = AccordionItemFooterAlignment.Far;

// Footer height when minimized
accordionControl1.OptionsMinimizing.FooterHeight = 60;

// Allow users to resize the footer when minimized
accordionControl1.OptionsMinimizing.AllowFooterResizing = true;
```

## Expand / Collapse Behavior

### Single vs Multiple Expanded Elements

```csharp
// Only one element can be expanded at a time (accordion/exclusive behavior)
accordionControl1.ExpandElementMode = ExpandElementMode.Single;

// Multiple elements can be expanded simultaneously (default)
accordionControl1.ExpandElementMode = ExpandElementMode.Multiple;
```

### Auto-Scroll on Expand

```csharp
// Scroll the control so that the expanded element's content is visible.
// Valid values: Default, Fixed, ScrollUp.
accordionControl1.ElementPositionOnExpanding = ElementPositionOnExpanding.ScrollUp;
```

### Expand / Collapse All

```csharp
accordionControl1.ExpandAll();
accordionControl1.CollapseAll();
```

### Expand / Collapse a Specific Element

```csharp
accordionControl1.ExpandElement(grpContacts);
accordionControl1.CollapseElement(grpContacts);
```

### Expand / Collapse Button Visibility

```csharp
// Hide expand/collapse buttons for all groups
accordionControl1.ShowGroupExpandButtons = false;

// Hide expand/collapse buttons for all items
accordionControl1.ShowItemExpandButtons = false;
```

### Click-to-Expand / Click-to-Collapse

```csharp
// Allow users to expand/collapse groups by clicking their header (default: true)
accordionControl1.ExpandGroupOnHeaderClick = true;

// Same for items with content containers
accordionControl1.ExpandItemOnHeaderClick = true;
```

### Preventing Collapse of a Specific Group

Handle `ExpandStateChanging` and clear `e.ElementsToExpandCollapse`:

```csharp
private void AccordionControl1_ExpandStateChanging(object sender, ExpandStateChangingEventArgs e) {
    // Prevent the "Options" group from being collapsed
    if (e.Element.Style == ElementStyle.Group
        && e.Element.Text == "Options"
        && e.NewState == AccordionElementState.Collapsed)
    {
        e.ElementsToExpandCollapse.Clear();
    }
}
```

## Animation

```csharp
// Disable expand/collapse animation
accordionControl1.AnimationType = AnimationType.None;

// Default slide animation
accordionControl1.AnimationType = AnimationType.Default;
```

## Fluent Design Form — Adaptive Hamburger Menu

When `AccordionControl` is embedded in a `FluentDesignForm`, its display mode adapts automatically as the window is resized:

- **Wide window** → `Inline` (sidebar visible)
- **Medium window** → `Overlay`
- **Narrow window** → `Minimal`

No code is needed for this behavior — it is handled by the form.

## Key API Summary

| API | Description |
|---|---|
| `AccordionControl.ViewType` | `Standard` (accordion) or `HamburgerMenu` |
| `AccordionControl.OptionsHamburgerMenu.DisplayMode` | `Inline`, `Overlay`, or `Minimal` |
| `AccordionControl.OptionsMinimizing.State` | `Normal` (expanded) or `Minimized` |
| `AccordionControl.RootDisplayMode` | `Default` (stacked) or `Footer` (tab-like) |
| `AccordionControl.ExpandElementMode` | `Single` or `Multiple` |
| `AccordionControl.ExpandAll()` / `CollapseAll()` | Expand / collapse all elements |
| `AccordionControl.ShowPopupForm(element)` | Show popup when minimized |
| `AccordionControl.AnimationType` | Enable or disable animation |

## Common Issues

| Symptom | Cause | Solution |
|---|---|---|
| Hamburger button not visible | `ViewType` left as `Standard` | Set `ViewType = HamburgerMenu` |
| Content hidden when Overlay expands | Form layout not accounting for overlay | Use `FluentDesignForm` or handle overlay manually |
| All groups collapse when one expands | `ExpandElementMode = Single` | Change to `ExpandElementMode = Multiple` |
| Expand/collapse button shown on empty group | Default behavior | Set `ShowGroupExpandButtons = false` or handle `CustomDrawElement` per element |

## Source Material

- `articles/114553` — Accordion Control overview (Expand and Collapse section, Footer section)
- `articles/120498` — Hamburger Menu View Style
- `articles/DevExpress.XtraBars.Navigation.AccordionControl.ViewType` — ViewType property reference
- `articles/DevExpress.XtraBars.Navigation.AccordionOptionsHamburgerMenu.DisplayMode` — DisplayMode enum reference
- `articles/DevExpress.XtraBars.Navigation.OptionsMinimizing.State` — State property reference
