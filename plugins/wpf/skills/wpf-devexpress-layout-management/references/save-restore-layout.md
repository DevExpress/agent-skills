# Save and Restore Layout

Each of the six layout controls supports layout persistence — but the API differs by control family. `DockLayoutManager` uses `DXSerializer`-based `SaveLayoutToStream` / `SaveLayoutToXml` with full state coverage. The five `*LayoutControl` family members use `WriteToXML` / `ReadFromXML` and persist only what's user-customizable.

## When to Use This Reference

Use this when you need to:

- Persist a user-customized layout between sessions
- Restore layout on app load
- Configure `RestoreLayoutOptions` for missing / extra panels
- Save unopened tab contents (cache mode)
- Use `BindableName` in MVVM
- Choose between layout-control's `WriteToXML` and DockLayoutManager's serializer

## Two Persistence APIs

| Family | Methods | Coverage |
|---|---|---|
| **DockLayoutManager** (`dxdo:`) | `SaveLayoutToStream` / `RestoreLayoutFromStream`, `SaveLayoutToXml` / `RestoreLayoutFromXml` | Full layout — panel structure, positions, sizes, captions, control state via `DXSerializer` |
| **LayoutControl family** (`dxlc:`) | `WriteToXML` / `ReadFromXML` (inherited from `LayoutControlBase`) | User-customizable state — item sizes and order |

The difference matters: `DockLayoutManager` can save and restore the entire dock structure (including which panels are floating, which are pinned, what document is selected). The layout-control family only saves what a user can change at runtime (typically item sizes and reorders).

## DockLayoutManager — Full Layout Persistence

### Requirement: Unique Names

Every dock UI item and layout UI item must have a unique name. Two options:

- Set `x:Name` on each panel/group in XAML
- In MVVM, bind `BaseLayoutItem.BindableName` to a ViewModel property holding a unique identifier

```xaml
<dxdo:DockLayoutManager x:Name="dockManager">
    <dxdo:LayoutGroup x:Name="rootGroup">
        <dxdo:LayoutPanel x:Name="solutionPanel" Caption="Solution Explorer"/>
        <dxdo:DocumentGroup x:Name="documents">
            <dxdo:DocumentPanel x:Name="startPage" Caption="Start Page"/>
        </dxdo:DocumentGroup>
        <dxdo:LayoutPanel x:Name="propertiesPanel" Caption="Properties"/>
    </dxdo:LayoutGroup>
</dxdo:DockLayoutManager>
```

### Save and Restore

```csharp
// Save
using var stream = File.Create("layout.xml");
dockManager.SaveLayoutToStream(stream);

// Or to disk by path
dockManager.SaveLayoutToXml("layout.xml");

// Restore
using var inStream = File.OpenRead("layout.xml");
inStream.Seek(0, SeekOrigin.Begin);
dockManager.RestoreLayoutFromStream(inStream);

// Or from disk
dockManager.RestoreLayoutFromXml("layout.xml");
```

Typical pattern: save on `Window.Closing`, restore on `Window.Loaded`.

```csharp
public partial class MainWindow : Window {
    const string LayoutPath = "layout.xml";

    public MainWindow() {
        InitializeComponent();
        Loaded   += (_, _) => { if (File.Exists(LayoutPath)) dockManager.RestoreLayoutFromXml(LayoutPath); };
        Closing  += (_, _) => dockManager.SaveLayoutToXml(LayoutPath);
    }
}
```

### Save Across Linked DockLayoutManagers

If you link two or more dock-layout managers (one window with multiple shells), item names must be **unique across all of them**.

### Limitation: Snapped Panels

`DockLayoutManager` does NOT restore a panel's position if it was snapped to a side of the screen at save time. Floating panels in normal positions restore correctly.

### Reconciling Layouts: `RestoreLayoutOptions`

The saved XML may include panels no longer in the control (renamed/deleted), or the control may have new panels not in the saved XML. `RestoreLayoutOptions` (attached properties) control how to reconcile:

| Property | When `true` | When `false` |
|---|---|---|
| `RemoveOldPanels` | Restored panels that aren't in the current control are removed (default behavior) | Restored panels that aren't in the current control are added back |
| `AddNewPanels` | New panels added in code are kept after restore | New panels added in code are moved to `ClosedPanels` |
| `AddNewLayoutControlItems` | New layout items are kept | New items are removed |
| `AddNewLayoutGroups` | New groups are kept | New groups are removed |
| `RemoveOldLayoutControlItems` | Saved items not in the control are removed | Saved items are restored as new |
| `RemoveOldLayoutGroups` | Saved groups not in the control are removed | Saved groups are restored as new |

```xaml
<dxdo:DockLayoutManager x:Name="dockManager"
                        dxdo:RestoreLayoutOptions.RemoveOldPanels="False"
                        dxdo:RestoreLayoutOptions.AddNewPanels="False">
```

### Closed Panels

After `RestoreLayoutFromStream`, panels that exist in the `DockLayoutManager` but are absent from the saved layout (typically panels added in code/XAML) are moved to `DockLayoutManager.ClosedPanels` when `AddNewPanels="False"`. You can iterate this collection to surface them in a "Show closed panels" UI.

### Unopened Tab Contents

By default, content of unopened `LayoutGroup` tabs **isn't saved or restored** — the WPF visual tree only contains the active tab. To force all tabs to load:

```xaml
<dxdo:LayoutGroup TabContentCacheMode="CacheAllTabs">
```

`TabContentCacheMode.CacheAllTabs` loads the content of every tab into the visual tree on first render. Use this when you need to save/restore state of editors inside tabs that haven't been opened yet.

### Exclude Specific Properties

`DXSerializer.AllowProperty` (attached event on `DockLayoutManager`) lets you exclude specific properties from serialization:

```xaml
<dxdo:DockLayoutManager
    xmlns:dxdo="http://schemas.devexpress.com/winfx/2008/xaml/docking"
    xmlns:dxc="http://schemas.devexpress.com/winfx/2008/xaml/core"
    dxc:DXSerializer.AllowProperty="OnAllowProperty">
```

```csharp
private void OnAllowProperty(object sender, AllowPropertyEventArgs e) {
    if (e.DependencyProperty == LayoutPanel.IsActiveProperty)
        e.Allow = false;   // Don't save IsActive
}
```

### MVVM-Friendly Persistence

In MVVM scenarios where panels are generated from an `ItemsSource` collection, use `BindableName` instead of `x:Name`. `DockLayoutManager` has no `ItemContainerStyle` property — apply an implicit `Style` (keyed by `TargetType`) in its `Resources` instead:

```xaml
<dxdo:DockLayoutManager ItemsSource="{Binding Panels}">
    <dxdo:DockLayoutManager.Resources>
        <Style TargetType="dxdo:LayoutPanel">
            <Setter Property="BindableName" Value="{Binding Id}"/>
            <Setter Property="Caption" Value="{Binding Title}"/>
        </Style>
    </dxdo:DockLayoutManager.Resources>
</dxdo:DockLayoutManager>
```

The `Id` property on each panel ViewModel provides the unique identifier the save/restore needs.

## LayoutControl Family — `WriteToXML` / `ReadFromXML`

All five `*LayoutControl` variants (LayoutControl, DataLayoutControl, DockLayoutControl, FlowLayoutControl, TileLayoutControl) inherit from `LayoutControlBase` and expose:

```csharp
public void WriteToXML(XmlWriter writer);
public void ReadFromXML(XmlReader reader);
```

### Save and Restore — LayoutControl

```csharp
// Save
using var writer = XmlWriter.Create("form-layout.xml");
layoutControl.WriteToXML(writer);

// Restore
using var reader = XmlReader.Create("form-layout.xml");
layoutControl.ReadFromXML(reader);
```

Typical pattern — same as DockLayoutManager:

```csharp
const string LayoutPath = "form-layout.xml";

void Window_Closing(object sender, CancelEventArgs e) {
    using var writer = XmlWriter.Create(LayoutPath);
    layoutControl.WriteToXML(writer);
}

void Window_Loaded(object sender, RoutedEventArgs e) {
    if (!File.Exists(LayoutPath)) return;
    using var reader = XmlReader.Create(LayoutPath);
    layoutControl.ReadFromXML(reader);
}
```

> The DevExpress docs recommend hooking the window/UserControl's `Loaded` event for restoring — not the constructor — to ensure the visual tree exists.

### Requirement: Item Names

For reliable restore, **assign names to items**:

```xaml
<dxlc:LayoutControl>
    <dxlc:LayoutGroup x:Name="addressGroup" View="GroupBox" Header="Address">
        <dxlc:LayoutItem x:Name="countryItem" Label="Country">
            <dxe:TextEdit/>
        </dxlc:LayoutItem>
        <dxlc:LayoutItem x:Name="cityItem" Label="City">
            <dxe:TextEdit/>
        </dxlc:LayoutItem>
    </dxlc:LayoutGroup>
</dxlc:LayoutControl>
```

If items lack names, restore still works as long as **the item collection (order and count) hasn't changed since save**. After any code change that adds, removes, or reorders items, restores break. Naming items future-proofs persistence.

### What Gets Persisted

For the LayoutControl family, only **user-customizable state** is saved:

- Item position (after drag-and-drop reordering)
- Item size (after resize-thumb dragging)
- Group expand state (`IsCollapsed`)
- Hidden items (after dragging to the Available Items list)

**NOT persisted**:

- The `Orientation` of `FlowLayoutControl`
- Splitter visibility
- Style properties
- Editor values (those are bound to your data, not layout state)
- Group view (`Group` vs `GroupBox` vs `Tabs`)

This makes layout-control persistence smaller and safer than DockLayoutManager persistence, but also less comprehensive.

### Custom Properties

To include extra properties in the saved layout, hook the `WriteElementToXML` and `ReadElementFromXML` events on the LayoutControl:

```csharp
layoutControl.WriteElementToXML += (s, e) => {
    if (e.Element is MyCustomItem item) {
        e.XmlWriter.WriteAttributeString("MySetting", item.MySetting.ToString());
    }
};

layoutControl.ReadElementFromXML += (s, e) => {
    if (e.Element is MyCustomItem item) {
        var attr = e.XmlReader.GetAttribute("MySetting");
        if (attr != null) item.MySetting = int.Parse(attr);
    }
};
```

### Dynamically-Added Controls

Children added at runtime (not in XAML) need `RegisterName` to participate in save/restore:

```csharp
var newItem = new LayoutItem { Label = "Dynamic" };
layoutControl.RegisterName("dynamicItem", newItem);
layoutControl.Items.Add(newItem);
```

Without `RegisterName`, the item won't appear in the saved XML and won't be restored.

## When to Use Which API

| Scenario | API |
|---|---|
| Visual-Studio shell with floating / pinned panels | `DockLayoutManager.SaveLayoutToStream` |
| MDI document panels with arbitrary user arrangement | `DockLayoutManager.SaveLayoutToStream` |
| Settings form built with `LayoutControl` | `LayoutControl.WriteToXML` |
| Auto-generated form via `DataLayoutControl` | `DataLayoutControl.WriteToXML` (inherited) |
| Tile dashboard with drag-drop ordering | `TileLayoutControl.WriteToXML` (inherited) |
| Flow of cards with custom column widths | `FlowLayoutControl.WriteToXML` (inherited) |
| Single-panel docking with resize thumbs | `DockLayoutControl.WriteToXML` (inherited) |

## MVVM Persistence Pattern

Save the XML as a `byte[]` on the ViewModel, persist it via your app's settings service:

```csharp
public class MainViewModel : ViewModelBase {
    public byte[]? SavedLayout { get => GetValue<byte[]?>(); set => SetValue(value); }

    public ICommand SaveLayoutCommand => new DelegateCommand<DockLayoutManager>(mgr => {
        using var ms = new MemoryStream();
        mgr.SaveLayoutToStream(ms);
        SavedLayout = ms.ToArray();
    });

    public ICommand RestoreLayoutCommand => new DelegateCommand<DockLayoutManager>(mgr => {
        if (SavedLayout == null) return;
        using var ms = new MemoryStream(SavedLayout);
        mgr.RestoreLayoutFromStream(ms);
    });
}
```

```xaml
<Button Content="Save Layout"
        Command="{Binding SaveLayoutCommand}"
        CommandParameter="{Binding ElementName=dockManager}"/>
<Button Content="Restore Layout"
        Command="{Binding RestoreLayoutCommand}"
        CommandParameter="{Binding ElementName=dockManager}"/>
```

For per-user persistence, expose `SavedLayout` to your settings provider (`Properties.Settings`, `appsettings.json`, database, etc.).

## Common Issues

- **Layout restores empty / wrong** — items lack unique names. Set `x:Name` on every panel and layout item (or `BindableName` in MVVM).
- **`RestoreLayoutFromXml` throws or silently fails** — XML schema doesn't match the current control version. Catch the exception and fall back to default layout.
- **Custom panel added in code disappears after restore** — inspect `ClosedPanels` after restore. If you want panels that aren't present in the saved layout to be re-added automatically, ensure `RestoreLayoutOptions.AddNewPanels` is `true` rather than `false`.
- **Layout restored but inner editor values are blank** — by design. Editor values come from your data bindings. Layout persistence saves layout state, not data.
- **Tab content state lost after restore** — unopened tabs aren't part of the visual tree. Set `LayoutGroup.TabContentCacheMode="CacheAllTabs"` on the group containing the tabs.
- **Dynamically added LayoutItem isn't restored** — need `RegisterName` on dynamically-added items in LayoutControl family.
- **Layout from an old app version doesn't load** — schema changed. Either version your layout files (and gate restore on the schema version), or handle the exception silently and use defaults.
- **Saving with snapped (screen-edge) panels and restoring places them in wrong position** — known limitation of `DockLayoutManager`. Unsnap before saving, or skip restore for snapped state.

## Source Material

- `articles/controls-and-libraries/layout-management/dock-windows/saving-and-restoring-the-layout-of-dock-panels-and-controls.md` (https://docs.devexpress.com/content/WPF/7059?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/common-features/save-and-restore-item-layout.md` (https://docs.devexpress.com/content/WPF/8155?md=true)
- `articles/common-concepts/save-and-restore-layouts.md` (https://docs.devexpress.com/content/WPF/7391?md=true)
- `articles/common-concepts/save-and-restore-layouts/dxserializer-events.md` (https://docs.devexpress.com/content/WPF/7410?md=true)
