# Saving and Restoring Layout

DevExpress layout controls can persist their layout (item sizes, positions, visibility, group state, panel positions) to XML, JSON, binary stream, or the system registry. This reference covers the API for each control and best-practice patterns for saving on exit and restoring on startup.

---

## LayoutControl and DataLayoutControl

`LayoutControl` (and its data-aware variant `DataLayoutControl`) provide four storage destinations:

| Method Pair | Storage |
|---|---|
| `SaveLayoutToXml(path)` / `RestoreLayoutFromXml(path)` | XML file on disk |
| `SaveLayoutToJson(stream)` / `RestoreLayoutFromJson(stream)` | JSON to any `Stream` |
| `SaveLayoutToStream(stream)` / `RestoreLayoutFromStream(stream)` | Binary XML to any `Stream` |
| `SaveLayoutToRegistry(key)` / `RestoreLayoutFromRegistry(key)` | Windows Registry key path |

### What Is Saved

The serializer stores:

- Sizes and positions of each `LayoutControlItem`, `LayoutControlGroup`, and `TabbedControlGroup`
- Visibility (`LayoutVisibility`) of each item
- Active tab page in each `TabbedControlGroup`
- Optionally: appearance settings (see `OptionsSerialization` below)

Items are matched by their `Name` property. **Items whose `Name` was changed since the layout was saved will not be restored correctly.**

### Save on Close / Restore on Load Pattern

```csharp
private const string LayoutFile = "MyFormLayout.xml";

private void MainForm_Load(object sender, EventArgs e)
{
    if (File.Exists(LayoutFile))
        layoutControl1.RestoreLayoutFromXml(LayoutFile);
}

private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
{
    layoutControl1.SaveLayoutToXml(LayoutFile);
}
```

### Save to JSON Stream

```csharp
// Save
using (var fs = File.Create("layout.json"))
    layoutControl1.SaveLayoutToJson(fs);

// Restore
if (File.Exists("layout.json"))
    using (var fs = File.OpenRead("layout.json"))
        layoutControl1.RestoreLayoutFromJson(fs);
```

### Save to Registry

```csharp
// Save
layoutControl1.SaveLayoutToRegistry(@"Software\MyApp\FormLayout");

// Restore
layoutControl1.RestoreLayoutFromRegistry(@"Software\MyApp\FormLayout");
```

### OptionsSerialization

Use `LayoutControl.OptionsSerialization` to control what is persisted:

```csharp
var opts = layoutControl1.OptionsSerialization;

opts.StoreAppearance           = true;   // persist font/color overrides
opts.RestoreTabbedGroupSpacing = true;
opts.RestoreGroupPadding       = true;
```

---

## DockManager

`DockManager` uses the same four-destination API:

| Method Pair | Storage |
|---|---|
| `SaveLayoutToXml(path)` / `RestoreLayoutFromXml(path)` | XML file |
| `SaveLayoutToJson(stream)` / `RestoreLayoutFromJson(stream)` | JSON stream |
| `SaveLayoutToStream(stream)` / `RestoreLayoutFromStream(stream)` | Binary stream |
| `SaveLayoutToRegistry(key)` / `RestoreLayoutFromRegistry(key)` | Registry |

### What Is Saved

- Docked positions, sizes, and visibility of each `DockPanel`
- Floating positions and sizes
- Auto-hide state
- Tab group structure (which panels are grouped as tabs)

Panels are matched by `DockPanel.Name`. **Give every panel a unique, stable `Name`.**

### Save on Close / Restore on Load

```csharp
private const string DockFile = "DockLayout.xml";

private void MainForm_Load(object sender, EventArgs e)
{
    // Restore after panels and their content have been created
    if (File.Exists(DockFile))
        dockManager1.RestoreLayoutFromXml(DockFile);
}

private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
{
    dockManager1.SaveLayoutToXml(DockFile);
}
```

> **Important**: Create all `DockPanel` instances in code before calling `RestoreLayoutFromXml`. The restore operation repositions existing panels — it does not create new ones.

### Save to Stream (e.g., embedded resource or database BLOB)

```csharp
// Save
using (var ms = new MemoryStream()) {
    dockManager1.SaveLayoutToStream(ms);
    byte[] bytes = ms.ToArray();
    SaveToDB(bytes);
}

// Restore
byte[] bytes = LoadFromDB();
using (var ms = new MemoryStream(bytes))
    dockManager1.RestoreLayoutFromStream(ms);
```

---

## Workspace Manager (Multiple Layout Slots)

`WorkspaceManager` (`DevExpress.Utils.WorkspaceManager`) lets you save and switch between **multiple named layouts** (workspaces) for all registered DevExpress controls at once — useful when users want to save their own UI configurations.

### Setup

1. Create a `WorkspaceManager` and assign the form: `var wm = new WorkspaceManager { TargetControl = this };`
2. It automatically detects registered `LayoutControl`, `DockManager`, `GridView`, etc. instances.
3. (Optional) Add a `BarWorkspaceMenuItem` to a ribbon/toolbar so users can manage workspaces from the UI.

### API

```csharp
// Capture the current layout into a named workspace (in the Workspaces collection)
workspaceManager1.CaptureWorkspace("MyWorkspace");

// Apply a workspace (must already be in the Workspaces collection)
workspaceManager1.ApplyWorkspace("MyWorkspace");

// List all saved workspaces
foreach (IWorkspace ws in workspaceManager1.Workspaces)
    Console.WriteLine(ws.Name);

// Delete a workspace
workspaceManager1.RemoveWorkspace("MyWorkspace");
```

### Persisting Workspaces Across Sessions

```csharp
private const string WorkspaceFile = "Workspaces.xml";

private void MainForm_Load(object sender, EventArgs e)
{
    if (File.Exists(WorkspaceFile))
        workspaceManager1.LoadWorkspaces(WorkspaceFile);

    // Apply the last used workspace
    workspaceManager1.ApplyWorkspace("Default");
}

private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
{
    workspaceManager1.CaptureWorkspace("Default");
    workspaceManager1.SaveWorkspaces(WorkspaceFile);
}
```

### When to Use WorkspaceManager vs. Individual SaveLayoutToXml

| Scenario | Recommendation |
|---|---|
| Single layout, single form | `SaveLayoutToXml` / `RestoreLayoutFromXml` on each control individually |
| Multiple layout slots (e.g., "Compact", "Wide", "Debug") | `WorkspaceManager` |
| Multiple controls on one form, one save operation | `WorkspaceManager` |
| Database or cloud storage | `SaveLayoutToStream` → serialize bytes; or `WorkspaceManager.SaveWorkspaces(stream)` |

---

## General Best Practices

1. **Name every layout element** — `LayoutControlItem.Name`, `LayoutControlGroup.Name`, `DockPanel.Name`. The serializer matches elements by name. Anonymous items cannot be restored.
2. **Restore after `InitializeComponent()`** — all items must exist before restoring. In `Form_Load` is safe.
3. **Create `DockPanel`s before restoring** — `DockManager.RestoreLayoutFromXml` repositions existing panels; it does not create missing ones.
4. **Handle missing layouts gracefully** — wrap restore calls in `if (File.Exists(...))` or `try/catch`.
5. **Version your layout files** — when the form's items change structurally (items added/removed), old layout files may contain stale references. The control silently skips unmatched items.
6. **Use `MemoryStream` for in-app layout switching** — save the "default" layout to a `MemoryStream` at startup, then restore it when the user clicks "Reset Layout".

```csharp
// Cache default layout at startup
MemoryStream defaultLayout = new MemoryStream();
layoutControl1.SaveLayoutToStream(defaultLayout);

// Reset button
private void btnResetLayout_Click(object sender, EventArgs e)
{
    defaultLayout.Position = 0;
    layoutControl1.RestoreLayoutFromStream(defaultLayout);
}
```

---

## Common Issues

| Symptom | Cause | Fix |
|---|---|---|
| Restore silently does nothing | Items have no `Name` or names changed since save | Set stable unique `Name` on every item |
| `DockPanel` positions reset on load | `RestoreLayoutFromXml` called before panels are created | Create all panels first, then restore |
| JSON restore throws `InvalidOperationException` | Stream position not at 0 after save | Reset with `stream.Position = 0` before reading |
| `WorkspaceManager` doesn't include a control | Control was added after `WorkspaceManager.Form` was set | Ensure the control is a child of the form before `WorkspaceManager` initializes |

## Source Material

- Save and Restore Form Layout: `https://docs.devexpress.com/content/WindowsForms/2179?md=true`
- Save and Restore Layouts of DevExpress Controls: `https://docs.devexpress.com/content/WindowsForms/2404?md=true`
- Saving and Restoring Dock Panel Layouts: `https://docs.devexpress.com/content/WindowsForms/1433?md=true`
- Workspace Manager: `https://docs.devexpress.com/content/WindowsForms/17674?md=true`
- `LayoutSerializationOptions`: `xref:DevExpress.XtraLayout.LayoutSerializationOptions`
