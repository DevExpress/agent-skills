# Defining Tabs — XAML, Items, ItemsSource

There are three ways to populate `DXTabControl` with tab pages. The right choice depends on whether tabs are **static** (known at design time) or **dynamic** (driven by a view-model collection).

## When to Use This Reference

Use this when you need to:

- Pick between explicit `DXTabItem` children, `Items` manipulation, and `ItemsSource`-based generation
- Bind tab headers and content to view-model data via templates
- Add icons (`Glyph`) and close buttons (`AllowHide`) to tabs
- Control caching of tab content (`TabContentCacheMode`)
- React to selection changes (`SelectionChanging`, `SelectionChanged`)

## Three Approaches — Picker

| Approach | When to use | Where tab definition lives |
|---|---|---|
| **Explicit `DXTabItem` children in XAML** | Static, known-at-design-time tabs (Settings dialog with 3 fixed panes) | XAML markup |
| **Add `DXTabItem` to `Items` in code** | Programmatic construction; rarely the right call | Code-behind |
| **`ItemsSource` + `ItemHeaderTemplate` + `ItemTemplate`** | Dynamic tabs from a view-model collection (MDI workspace, master-list editor) | View-model collection |

For MVVM apps, **default to `ItemsSource`** unless the tabs are genuinely static. Mixing approaches (`Items` *and* `ItemsSource`) is not supported — pick one.

## Approach 1 — Explicit `DXTabItem` Children

The simplest case. Add `DXTabItem` elements directly inside `DXTabControl`:

```xaml
<dx:DXTabControl>
    <dx:DXTabItem Header="General">
        <Label Content="General settings…"/>
    </dx:DXTabItem>
    <dx:DXTabItem Header="Appearance">
        <Label Content="Appearance settings…"/>
    </dx:DXTabItem>
    <dx:DXTabItem Header="Advanced">
        <Label Content="Advanced settings…"/>
    </dx:DXTabItem>
</dx:DXTabControl>
```

Each `DXTabItem` is a `ContentControl` — its `Content` property is set implicitly from the XAML child element.

### Header — String or Templated

`Header` accepts any object. For a string, just set the property:

```xaml
<dx:DXTabItem Header="General">...</dx:DXTabItem>
```

For richer content (icon + text, dynamic visuals), use `HeaderTemplate`:

```xaml
<dx:DXTabItem>
    <dx:DXTabItem.Header>
        <local:TabHeaderViewModel Title="General" Badge="3"/>
    </dx:DXTabItem.Header>
    <dx:DXTabItem.HeaderTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding Title}"/>
                <Border Margin="6,0,0,0" Background="Tomato"
                        CornerRadius="8" Padding="4,0">
                    <TextBlock Text="{Binding Badge}" Foreground="White"/>
                </Border>
            </StackPanel>
        </DataTemplate>
    </dx:DXTabItem.HeaderTemplate>
    <local:GeneralPage/>
</dx:DXTabItem>
```

### Icon via `Glyph`

```xaml
<dx:DXTabItem Header="General"
              Glyph="{dx:DXImage Image=Settings_16x16.png}">
    ...
</dx:DXTabItem>
```

`Glyph` accepts an `ImageSource`. `dx:DXImage` pulls from the DevExpress image library; alternatively use a pack URI to an embedded resource.

For more elaborate icons, use `GlyphTemplate` (a `DataTemplate`).

### Close Button via `AllowHide`

```xaml
<dx:DXTabItem Header="Document 1" AllowHide="True">
    <local:DocumentEditor/>
</dx:DXTabItem>
```

The close (×) button appears on the tab header. Clicking it hides the tab (sets `IsHidden="True"`). To respond:

| Event | Fires |
|---|---|
| `DXTabControl.TabHiding` | Cancel-able, before hide |
| `DXTabControl.TabHidden` | After hide |

To **remove** the tab (not just hide), handle `TabHidden` and remove the item from `Items` / the source collection.

## Approach 2 — `Items` Collection in Code

```csharp
tabControl.Items.Add(new DXTabItem {
    Header  = "General",
    Content = new GeneralSettingsView()
});
tabControl.Items.Add(new DXTabItem {
    Header  = "Appearance",
    Content = new AppearanceSettingsView()
});
```

Use only when you need to construct UI in code-behind. For MVVM, prefer `ItemsSource`.

## Approach 3 — `ItemsSource` + Templates (Recommended for MVVM)

Bind the tab control to a collection of view models; `DXTabControl` generates one `DXTabItem` per item.

### View Model

```csharp
public class DocumentViewModel {
    public string Title { get; set; }
    public string Body  { get; set; }
}

public class MainViewModel {
    public ObservableCollection<DocumentViewModel> Documents { get; } = new() {
        new DocumentViewModel { Title = "Invoice 1001", Body = "…" },
        new DocumentViewModel { Title = "Invoice 1002", Body = "…" }
    };

    public DocumentViewModel CurrentDocument { get; set; }
}
```

### XAML

```xaml
<dx:ThemedWindow xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
                 xmlns:local="clr-namespace:MyApp"
                 ...>
    <dx:ThemedWindow.DataContext>
        <local:MainViewModel/>
    </dx:ThemedWindow.DataContext>

    <dx:DXTabControl ItemsSource="{Binding Documents}"
                     SelectedItem="{Binding CurrentDocument}">
        <dx:DXTabControl.ItemHeaderTemplate>
            <DataTemplate>
                <TextBlock Text="{Binding Title}"/>
            </DataTemplate>
        </dx:DXTabControl.ItemHeaderTemplate>

        <dx:DXTabControl.ItemTemplate>
            <DataTemplate>
                <TextBox Text="{Binding Body}" AcceptsReturn="True"/>
            </DataTemplate>
        </dx:DXTabControl.ItemTemplate>
    </dx:DXTabControl>
</dx:ThemedWindow>
```

### What's Mapped to What

| Tab control property | Set per | Source |
|---|---|---|
| `ItemHeaderTemplate` | Header content | Each item's data context (a `DocumentViewModel`) |
| `ItemTemplate` | Page content | Same — the item's data context |
| `SelectedItem` | The active item from the source collection | Two-way bindable |
| `SelectedIndex` | The active item's index | Two-way bindable |

Add to `Documents` → a new tab appears. Remove → the tab disappears. The collection is the single source of truth.

### Per-Container Style — `ItemContainerStyle`

To configure properties on the *generated* `DXTabItem`s (e.g., `AllowHide`, `Glyph`), use `ItemContainerStyle`:

```xaml
<dx:DXTabControl ItemsSource="{Binding Documents}">
    <dx:DXTabControl.ItemContainerStyle>
        <Style TargetType="dx:DXTabItem">
            <Setter Property="AllowHide" Value="True"/>
            <Setter Property="Header"    Value="{Binding Title}"/>
        </Style>
    </dx:DXTabControl.ItemContainerStyle>
    <dx:DXTabControl.ItemTemplate>
        <DataTemplate>
            <local:DocumentEditor/>
        </DataTemplate>
    </dx:DXTabControl.ItemTemplate>
</dx:DXTabControl>
```

> When `Header` is set in `ItemContainerStyle`, you usually don't need `ItemHeaderTemplate` — the header content (a string here) renders with the default text presenter. Use one or the other; not both.

### Selecting a Tab from the View Model

```csharp
public class MainViewModel : INotifyPropertyChanged {
    public ObservableCollection<DocumentViewModel> Documents { get; } = new();
    DocumentViewModel current;
    public DocumentViewModel CurrentDocument {
        get => current;
        set { current = value; OnPropertyChanged(); }
    }

    public void Open(DocumentViewModel doc) {
        if (!Documents.Contains(doc)) Documents.Add(doc);
        CurrentDocument = doc;
    }
}
```

Calling `Open(doc)` adds and selects in one step — `DXTabControl` reflects both changes.

## Selection Events

| Event | Purpose | Cancel-able |
|---|---|---|
| `SelectionChanging` | Fires before selection changes — you can cancel | Yes (`e.Cancel = true`) |
| `SelectionChanged` | Fires after selection changes | No |

```csharp
void tabControl_SelectionChanging(object sender, TabControlSelectionChangingEventArgs e) {
    var newDoc = tabControl.Items[e.NewSelectedIndex] as DocumentViewModel;
    if (newDoc?.IsLocked == true) e.Cancel = true;
}
```

## Tab Content Caching — `TabContentCacheMode`

By default, `DXTabControl` **re-creates** tab content on every selection. For heavy content (data grids, editors), set caching:

| `TabContentCacheMode` | Effect |
|---|---|
| `Default` | No caching — content reconstructed on each selection |
| `CacheAllTabs` | Construct all tab contents on first show; keep them in memory forever |
| `CacheTabsOnSelecting` | Lazily construct on first selection; keep cached afterwards |

```xaml
<dx:DXTabControl TabContentCacheMode="CacheTabsOnSelecting" .../>
```

`CacheTabsOnSelecting` is a good middle ground for many-tabbed UIs — only constructs what users actually visit.

## Combined Example — Closable, Cached, Bound

```xaml
<dx:DXTabControl ItemsSource="{Binding Documents}"
                 SelectedItem="{Binding CurrentDocument}"
                 TabContentCacheMode="CacheTabsOnSelecting"
                 TabHidden="OnTabHidden">
    <dx:DXTabControl.View>
        <dx:TabControlScrollView ScrollButtonShowMode="AutoHideBothButtons"
                                 AllowAnimation="True"/>
    </dx:DXTabControl.View>

    <dx:DXTabControl.ItemContainerStyle>
        <Style TargetType="dx:DXTabItem">
            <Setter Property="AllowHide" Value="True"/>
            <Setter Property="Header"    Value="{Binding Title}"/>
            <Setter Property="Glyph"     Value="{Binding Icon}"/>
        </Style>
    </dx:DXTabControl.ItemContainerStyle>

    <dx:DXTabControl.ItemTemplate>
        <DataTemplate>
            <local:DocumentEditor/>
        </DataTemplate>
    </dx:DXTabControl.ItemTemplate>
</dx:DXTabControl>
```

```csharp
void OnTabHidden(object sender, TabControlTabHiddenEventArgs e) {
    if (e.TabItem.DataContext is DocumentViewModel doc) {
        ((MainViewModel)DataContext).Documents.Remove(doc);
    }
}
```

The user closes a tab → `TabHidden` fires → we remove the item from the source, so it stays gone.

## Common Issues

- **Headers blank when using `ItemsSource`** — `ItemHeaderTemplate` not set (the default presenter calls `ToString()` on the item; if you didn't override `ToString()`, you get the type name).
- **`Items` empty but UI shows tabs** — `ItemsSource` is set; `Items` is read-only when `ItemsSource` is bound.
- **`SelectedItem` doesn't update the view model** — binding is one-way (default) or the source doesn't implement INPC. Use `Mode=TwoWay` and `INotifyPropertyChanged`.
- **`AllowHide` set on a `DXTabItem` but no close button** — the property is on the `DXTabItem`, but a `Style` for one specific tab won't apply; set it in `ItemContainerStyle` or directly on each child.
- **`Glyph` set in XAML but doesn't show** — wrong asset path, or set as a `Brush` where an `ImageSource` is expected.
- **Slow tab switch** — default mode reconstructs content; switch to `CacheTabsOnSelecting`.
- **`ItemTemplate` runs but `DataContext` is wrong** — the data context inside `ItemTemplate` *is* the source item, not the parent view model. To reach the parent, use `RelativeSource={RelativeSource AncestorType=dx:DXTabControl}`.

## Source Material

- `articles/controls-and-libraries/layout-management/tab-control.md` (`xref:7975`)
- `articles/controls-and-libraries/layout-management/tab-control/concepts/binding-to-data-overview.md` (`xref:7978`)
- `DevExpress.Xpf.Core.DXTabControl` class reference
- `DevExpress.Xpf.Core.DXTabItem` class reference
- `articles/controls-and-libraries/layout-management/tab-control/concepts/adding-and-removing-tab-items.md` (`xref:113904`)
