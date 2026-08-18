# Getting Started

This reference covers the minimum setup for DevExpress WinForms editors: the NuGet packages, namespaces, the `BaseEdit` / `BaseControl` class hierarchy in one diagram, the standalone-vs-embedded duality (every embeddable editor has a `RepositoryItem*` twin), and the basics common to all editors — `EditValue`, `Properties`, `EnterMoveNextControl`, error icons, and skin setup.

## When to Use This Reference

- A new WinForms project needs DevExpress editors for the first time.
- You are deciding whether to use atomic vs umbrella NuGet packages.
- You want a one-page map of the editor class hierarchy before picking a control.
- An editor renders unskinned, ignores binding, or does not accept Enter to move focus.

## NuGet Packages

| Package | Contents |
|---|---|
| `DevExpress.Win.Navigation` | `DevExpress.XtraEditors.v*.dll`, `DevExpress.Utils.v*.dll` — every editor (`TextEdit`, `SpinEdit`, `DateEdit`, `CheckEdit`, `ToggleSwitch`, `ButtonEdit`, `ComboBoxEdit`, `LookUpEdit`, `MemoEdit`, `PictureEdit`, `ProgressBarControl`, `TrackBarControl`, `RangeTrackBarControl`, `RatingControl`, `TokenEdit`, `SparklineEdit`, `BreadCrumbEdit`, `RadioGroup`, `ColorEdit`, `ColorPickEdit`, etc.). Includes `SimpleButton`, `LabelControl`, `ListBoxControl`. |
| `DevExpress.Win.Grid` | `GridLookUpEdit`, `SearchLookUpEdit` (drop-down panels backed by a `GridControl`). |
| `DevExpress.Win.TreeList` | `TreeListLookUpEdit` (drop-down backed by `TreeList`). |
| `DevExpress.Win.Dialogs` | `BrowsePathEdit`, `XtraOpenFileDialog`, `XtraSaveFileDialog`, `XtraFolderBrowserDialog`. |
| `DevExpress.Win` *(umbrella)* | One package for most WinForms controls including all editors. Heavier; faster to start with. |

Add them with `dotnet add package` (they are on nuget.org and in the local DevExpress Unified Component Installer feed). Keep all DevExpress versions aligned across the solution.

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.Navigation" Version="26.1.*" />
  <PackageReference Include="DevExpress.Win.Grid"       Version="26.1.*" />
  <PackageReference Include="DevExpress.Win.Dialogs"    Version="26.1.*" />
</ItemGroup>
```

## Common Imports

```csharp
using DevExpress.XtraEditors;                // TextEdit, SpinEdit, ButtonEdit, LookUpEdit, ...
using DevExpress.XtraEditors.Controls;       // EditorButton, ButtonPredefines, TextEditStyles
using DevExpress.XtraEditors.Repository;     // RepositoryItem*, RepositoryItemButtonEdit
using DevExpress.XtraEditors.Mask;           // MaskType, MaskSettings.*
using DevExpress.Utils;                      // DefaultBoolean, HorzAlignment
using DevExpress.Utils.Svg;                  // SvgImage
```

## Class Hierarchy at a Glance

Two families inherit from `BaseControl` (which inherits from `Control`):

```
BaseControl
├── BaseEdit  (defines EditValue — the bindable value)
│   ├── BaseCheckEdit ──── CheckEdit, ToggleSwitch
│   ├── PictureEdit
│   ├── ProgressBarBaseControl ── ProgressBarControl, MarqueeProgressBarControl
│   ├── RadioGroup
│   ├── RatingControl
│   ├── SparklineEdit
│   ├── TokenEdit
│   ├── TrackBarControl ── ZoomTrackBarControl
│   └── TextEdit  (the largest sub-tree)
│       ├── MemoEdit
│       ├── ButtonEdit
│       │   ├── BaseSpinEdit
│       │   │   ├── SpinEdit
│       │   │   ├── DateEdit, DateTimeOffsetEdit
│       │   │   ├── TimeEdit
│       │   │   └── TimeSpanEdit
│       │   ├── HyperLinkEdit
│       │   ├── CalcEdit
│       │   ├── ColorEdit
│       │   ├── BreadCrumbEdit
│       │   ├── MemoExEdit
│       │   ├── BlobBaseEdit ──── ImageEdit (BLOB editor)
│       │   └── PopupBaseEdit
│       │       └── PopupBaseAutoSearchEdit
│       │           ├── ComboBoxEdit ── ImageComboBoxEdit, FontEdit, MRUEdit
│       │           ├── CheckedComboBoxEdit
│       │           ├── ColorPickEditBase ── ColorPickEdit
│       │           ├── PopupContainerEdit
│       │           ├── PopupGalleryEdit
│       │           └── LookUpEditBase
│       │               ├── LookUpEdit
│       │               ├── GridLookUpEditBase ── GridLookUpEdit, SearchLookUpEdit
│       │               └── TreeListLookUpEdit
│
└── (non-BaseEdit controls)
    ├── LabelControl, HyperlinkLabelControl
    ├── BaseButton ── SimpleButton, DropDownButton, CheckButton
    ├── BaseListBoxControl ── ListBoxControl, ImageListBoxControl, BaseCheckedListBoxControl ── CheckedListBoxControl
    ├── ProgressPanel
    ├── NavigatorBase ── DataNavigator, ControlNavigator
    ├── FilterControl
    ├── SearchControl
    ├── RangeControl
    ├── CalendarControl
    └── ImageSlider
```

The defining trait of a `BaseEdit` descendant is `EditValue`. If a control has it, it can store and bind a typed value. If it does not, the control is informational (label), action (button), navigational (navigator), or shows a collection (`ListBoxControl`).

## `EditValue` and Binding

`BaseEdit` declares `[DefaultBindingProperty("EditValue")]`, so the simplest binding is:

```csharp
textEdit1.DataBindings.Add("EditValue", source, "FirstName");
spinEdit1.DataBindings.Add("EditValue", source, "Salary");
dateEdit1.DataBindings.Add("EditValue", source, "Birthday");
checkEdit1.DataBindings.Add("EditValue", source, "IsActive");
```

`EditValue` is `object` — it holds whatever type the editor reads/writes (`string` for `TextEdit`, `decimal` for `SpinEdit`, `DateTime` for `DateEdit`, `bool` for `CheckEdit`, the lookup key for `LookUpEdit`, etc.). The editor knows how to convert from/to its underlying primitive.

Subscribe to `EditValueChanged` for general notifications, `EditValueChanging` to inspect or veto a change before it commits.

### Immediate post

By default, value changes commit when focus leaves the editor. For editors with a single discrete value, set the editor to push on each change:

```csharp
checkEdit1.Properties.InplaceModeImmediatePostChanges = DefaultBoolean.True;
toggleSwitch1.Properties.InplaceModeImmediatePostChanges = DefaultBoolean.True;
trackBar1.Properties.InplaceModeImmediatePostChanges = DefaultBoolean.True;
ratingControl1.Properties.InplaceModeImmediatePostChanges = DefaultBoolean.True;
radioGroup1.Properties.InplaceModeImmediatePostChanges = DefaultBoolean.True;
```

Or set it globally:

```csharp
DevExpress.XtraEditors.WindowsFormsSettings.InplaceEditorUpdateMode =
    DevExpress.XtraEditors.InplaceEditorUpdateMode.Immediate;
```

## `Properties` and Repository Items

`editor.Properties` returns the editor's `RepositoryItem*` — a separate object that holds *all* settings (mask, dropdown list, format, item collection, button collection, appearance, etc.). The live editor is essentially a thin shell that renders the settings.

```csharp
spinEdit1.Properties.MinValue  = 0;
spinEdit1.Properties.MaxValue  = 100;
spinEdit1.Properties.Increment = 5;
```

This duality is what lets editors be embedded into the Data Grid / TreeList / Ribbon. There you allocate a `RepositoryItemSpinEdit` (the same settings bag), add it to `gridControl1.RepositoryItems`, and assign it to a column. The grid creates the live `SpinEdit` only when the user clicks a cell.

```csharp
var spin = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit {
    MinValue = 0, MaxValue = 100
};
gridControl1.RepositoryItems.Add(spin);
gridView1.Columns["Quantity"].ColumnEdit = spin;
```

See [editor-variants.md](editor-variants.md) for the full editor → repository-item mapping.

## Common Properties on Every `BaseEdit`

| Property | Purpose |
|---|---|
| `EditValue` | The bindable value (`object`). |
| `Properties` | The `RepositoryItem*` settings bag. |
| `EnterMoveNextControl` | When `true`, pressing Enter moves focus to the next tab-order control. |
| `ErrorIcon` / `ErrorText` / `ErrorImageOptions` / `ErrorIconAlignment` | Inline error icon and tooltip — no `ErrorProvider` needed. |
| `BorderStyle` | `Default`, `NoBorder`, `Simple`, `Flat`, `HotFlat`, `UltraFlat`, `Style3D`, `Office2003`. |
| `Properties.AppearanceFocused` / `AppearanceReadOnly` / `AppearanceDisabled` | Per-state appearance overrides. |
| `Properties.NullText` / `NullValuePrompt` | Watermark / placeholder. |
| `Properties.ReadOnly` | Read-only mode (editor still accepts focus). |

```csharp
textEdit1.EnterMoveNextControl = true;
textEdit1.Properties.NullValuePrompt = "Search…";
textEdit1.ErrorText  = "Required";
textEdit1.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
```

## Host Form

Use `XtraForm` (or `RibbonForm` for a ribbon-based UI) for correct skin integration.

## Minimal Standalone Editor

```csharp
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();

        var name = new TextEdit { Top = 20, Left = 20, Width = 240 };
        name.Properties.NullValuePrompt = "First Last";
        name.EnterMoveNextControl = true;
        Controls.Add(name);

        var save = new SimpleButton { Top = 60, Left = 20, Text = "Save" };
        save.Click += (_, _) => MessageBox.Show($"Saved: {name.EditValue}");
        Controls.Add(save);
    }
}
```

## Verify the Setup

After running:

- The form has DevExpress skin chrome (rounded corners, custom title bar if `XtraForm`).
- The text box shows the watermark `First Last` when empty.
- Pressing Enter in the text box jumps focus to the button.
- The button uses DevExpress styling, not the OS-default chrome.

## Common Issues

- **Designer cannot resolve `XtraEditors`**: NuGet not restored. Run `dotnet restore` and reload the designer.
- **Editor renders unskinned**: skin not applied early enough, or host is a plain `Form` without `EnableFormSkins`. Move `SetSkinStyle` + `EnableFormSkins` to the top of `Main`.
- **Binding ignored**: bound to `Text` instead of `EditValue`. Always bind `EditValue`.
- **Enter does not move focus**: `EnterMoveNextControl = false` (default) — set it to `true`, or the control is a `MemoEdit` (Enter inserts newline).
- **`Properties.X` returns wrong settings**: you assigned to the live editor but the same `RepositoryItem*` is shared with another editor (e.g., via a column's `ColumnEdit`). Editing one mutates all consumers. Clone the repository item if independence is required.

## Source Material

- `articles/controls-and-libraries/editors-and-simple-controls.md` — Editors overview (`xref:WindowsForms.114580`).
- `articles/controls-and-libraries/editors-and-simple-controls/included-controls-and-components.md` — Included controls (`xref:WindowsForms.401381`).
- `articles/controls-and-libraries/editors-and-simple-controls/controls-as-on-toolbox.md` — Toolbox map (`xref:WindowsForms.115864`).
- `api/DevExpress.XtraEditors.BaseEdit.yml`.
- `api/DevExpress.XtraEditors.Repository.RepositoryItem.yml`.
- `api/DevExpress.XtraEditors.WindowsFormsSettings.yml`.
