# Buttons in ButtonEdit Descendants

`ButtonEdit` and every editor that inherits from it expose an unlimited collection of customizable buttons inside the editor. Buttons are represented by `DevExpress.Xpf.Editors.ButtonInfo` objects and live in `ButtonEdit.Buttons`. The same API works on `ButtonEdit`, `ComboBoxEdit`, `LookUpEdit`, `SpinEdit`, `BrowsePathEdit`, `PopupBaseEdit`, `PopupColorEdit`, `PopupImageEdit`, `PopupCalcEdit`, `DateEdit`, and every other editor whose base type is `ButtonEdit`.

## When to Use This Reference

Use this when you need to:

- Add a clear / apply / refresh / browse button inside an editor
- Hide an editor's default button (e.g., the dropdown arrow on `ComboBoxEdit`)
- Position a button on the left of the editor
- Wire button actions to MVVM commands or code-behind events
- Use predefined glyphs (`Apply`, `Clear`, `Search`, etc.) instead of text
- Customize a button's appearance with a `ContentTemplate`

## Which Editors Have Buttons?

Any editor that inherits from `ButtonEdit`. The inheritance chain in practice:

- `BaseEdit` → `TextEdit` → **`ButtonEdit`** (entry point for the Buttons API)
- From `ButtonEdit`: `ComboBoxEdit`, `LookUpEdit`, `SpinEdit`, `DateEdit`, `BrowsePathEdit`, `AutoSuggestEdit`, `HyperlinkEdit`, `PasswordBoxEdit`
- From `ButtonEdit` → `PopupBaseEdit`: `PopupColorEdit`, `PopupImageEdit`, `PopupCalcEdit`, and other popup editors

Editors that DON'T inherit from `ButtonEdit` (so don't have `Buttons`):

- `TextEdit` itself (use `ButtonEdit` if you need buttons in a plain text input)
- `MemoEdit`
- `CheckEdit`, `ToggleSwitchEdit`
- `ListBoxEdit`
- `ImageEdit`, `ColorEdit` (inline; the popup variants do have buttons)
- Visualization editors (`ProgressBarEdit`, `TrackBarEdit`, `SparklineEdit`, `RatingEdit`, `BarCodeEdit`)
- Simple controls (`SimpleButton`, `DropDownButton`, etc. — see [simple-controls.md](simple-controls.md))

## Core API

### `ButtonEdit.Buttons` — The Collection

```xaml
<dxe:ButtonEdit AllowDefaultButton="False">
    <dxe:ButtonInfo Content="Clear" IsLeft="True" Click="OnClear_Click"/>
    <dxe:ButtonInfo GlyphKind="Apply" Click="OnApply_Click"/>
</dxe:ButtonEdit>
```

Buttons render in collection order. `IsLeft="True"` flips them to the left side; left and right groups stack independently.

### `AllowDefaultButton` — Hide the Built-In Button

Many editors ship with a built-in button (the dropdown arrow on `ComboBoxEdit`, the spinner buttons on `SpinEdit`, the browse button on `BrowsePathEdit`). To replace or remove them, set `AllowDefaultButton="False"`:

```xaml
<dxe:ButtonEdit AllowDefaultButton="False">
    <dxe:ButtonInfo Content="Apply" Click="OnApply_Click"/>
</dxe:ButtonEdit>
```

Without `AllowDefaultButton="False"`, custom buttons appear *in addition to* the default.

> **Caution**: Hiding the default button on editors like `ComboBoxEdit` or `DateEdit` removes the only way for users to open the dropdown. Only do this when you're explicitly replacing it.

### `ButtonInfo` Properties

| Property | Use |
|---|---|
| `Content` | Button content — usually a string, or any UI element |
| `ContentTemplate` | Custom rendering template for the button |
| `GlyphKind` | Predefined glyph (use instead of `Content` for standard icons) |
| `IsLeft` | `true` to place on the left side; default `false` (right) |
| `Visibility` | Standard WPF visibility |
| `IsEnabled` | Standard WPF enabled flag |
| `ToolTip` | Tooltip text |
| `Command` / `CommandParameter` | MVVM command binding |
| `Click` | Click event handler |

### `GlyphKind` — Predefined Glyphs

Use `GlyphKind` instead of `Content` to get a built-in, theme-aware icon:

```xaml
<dxe:ButtonInfo GlyphKind="Search"/>
<dxe:ButtonInfo GlyphKind="Cancel"/>
<dxe:ButtonInfo GlyphKind="Apply"/>
```

Valid values (from the `GlyphKind` enum): `Apply`, `Cancel`, `Custom`, `Down`, `DropDown`, `Edit`, `First`, `Last`, `Left`, `Minus`, `NextPage`, `None`, `Plus`, `PrevPage`, `Redo`, `Refresh`, `Regular`, `Right`, `Search`, `Undo`, `Up`, `User`. There is no `Clear`/`Close`/`Ellipsis` glyph — use `Cancel` for a clear/dismiss button, or supply a `Content` / `ContentTemplate` for anything else. Glyphs respect the active DevExpress theme automatically.

## Click vs Command — MVVM-Friendly Binding

### Code-Behind: `Click` Event

```xaml
<dxe:ButtonEdit x:Name="searchBox" AllowDefaultButton="False">
    <dxe:ButtonInfo GlyphKind="Cancel" Click="OnClear_Click"/>
    <dxe:ButtonInfo GlyphKind="Search" Click="OnSearch_Click"/>
</dxe:ButtonEdit>
```

```csharp
private void OnClear_Click(object sender, RoutedEventArgs e) {
    searchBox.EditValue = string.Empty;
}

private void OnSearch_Click(object sender, RoutedEventArgs e) {
    var query = searchBox.Text;
    // ... do the search
}
```

### MVVM: `Command`

```xaml
<dxe:ButtonEdit EditValue="{Binding Query, Mode=TwoWay}" AllowDefaultButton="False">
    <dxe:ButtonInfo GlyphKind="Cancel" Command="{Binding ClearCommand}"/>
    <dxe:ButtonInfo GlyphKind="Search" Command="{Binding SearchCommand}"
                    CommandParameter="{Binding Query}"/>
</dxe:ButtonEdit>
```

```csharp
public class SearchViewModel : ViewModelBase {
    public string Query {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public ICommand ClearCommand => new DelegateCommand(() => Query = string.Empty);

    public ICommand SearchCommand => new DelegateCommand<string>(q => {
        // ...
    });
}
```

## Recipes

### Clear / Apply Buttons (the Classic Pattern)

```xaml
<dxe:ButtonEdit x:Name="buttonEdit" Width="275" Height="35"
                AllowDefaultButton="False">
    <dxe:ButtonInfo Content="Clear" IsLeft="True" Click="Clear_Button_Click"/>
    <dxe:ButtonInfo GlyphKind="Apply" Click="Apply_Button_Click"/>
</dxe:ButtonEdit>
```

```csharp
private void Clear_Button_Click(object sender, RoutedEventArgs e) {
    buttonEdit.EditValue = string.Empty;
}

private void Apply_Button_Click(object sender, RoutedEventArgs e) {
    MessageBox.Show($"Value: {buttonEdit.Text}", "ButtonEdit");
}
```

### Search Box with Magnifier and Clear

```xaml
<dxe:ButtonEdit EditValue="{Binding Search, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                AllowDefaultButton="False"
                NullText="Search...">
    <dxe:ButtonInfo GlyphKind="Search" Command="{Binding SearchCommand}"/>
    <dxe:ButtonInfo GlyphKind="Cancel" IsLeft="False"
                    Command="{Binding ClearSearchCommand}"
                    Visibility="{Binding HasSearch, Converter={StaticResource BoolToVisible}}"/>
</dxe:ButtonEdit>
```

### Extra Button on a ComboBoxEdit (Don't Hide Dropdown Arrow)

`ComboBoxEdit` has a default dropdown arrow. To add a refresh button alongside it (keep both):

```xaml
<dxe:ComboBoxEdit ItemsSource="{Binding Items}">
    <dxe:ButtonInfo GlyphKind="Refresh" IsLeft="True"
                    Command="{Binding RefreshItemsCommand}"/>
</dxe:ComboBoxEdit>
```

Note: `AllowDefaultButton` is `true` by default — the dropdown arrow stays on the right; the refresh button appears on the left.

### Custom Glyph via `ContentTemplate`

When no built-in `GlyphKind` matches your need, render a custom icon:

```xaml
<dxe:ButtonEdit AllowDefaultButton="False">
    <dxe:ButtonInfo Click="OnStarred_Click">
        <dxe:ButtonInfo.ContentTemplate>
            <DataTemplate>
                <Path Data="M 0 8 L 6 8 L 8 0 L 10 8 L 16 8 L 11 13 L 13 21 L 8 16 L 3 21 L 5 13 Z"
                      Fill="Goldenrod" Stretch="Uniform" Width="14" Height="14"/>
            </DataTemplate>
        </dxe:ButtonInfo.ContentTemplate>
    </dxe:ButtonInfo>
</dxe:ButtonEdit>
```

### BrowsePathEdit with Custom Open-in-Explorer Button

```xaml
<dxe:BrowsePathEdit EditValue="{Binding FilePath, Mode=TwoWay}">
    <dxe:ButtonInfo GlyphKind="Right" IsLeft="False"
                    Command="{Binding OpenInExplorerCommand}"
                    ToolTip="Open in Explorer"/>
</dxe:BrowsePathEdit>
```

The default browse button stays (because we didn't set `AllowDefaultButton="False"`); the extra button shows next to it.

### Spinner-Style Buttons on a Custom ButtonEdit

```xaml
<dxe:ButtonEdit AllowDefaultButton="False"
                EditValue="{Binding Quantity, Mode=TwoWay}">
    <dxe:ButtonInfo GlyphKind="Minus" IsLeft="True" Command="{Binding DecrementCommand}"/>
    <dxe:ButtonInfo GlyphKind="Plus"  IsLeft="False" Command="{Binding IncrementCommand}"/>
</dxe:ButtonEdit>
```

> If your need is just numeric step-up/step-down, prefer `SpinEdit` — it gives you `MinValue` / `MaxValue` / `Increment` for free.

## Common Issues

- **Buttons appear but the default also stays** — `AllowDefaultButton` defaults to `true`. Set `False` to hide it.
- **Hiding the default on `ComboBoxEdit` breaks the dropdown** — only do this if you have your own button that calls `combobox.ShowPopup()` (or you don't need the dropdown at all).
- **`Command` doesn't fire but `Click` does** — `Command` runs only if `CanExecute` returns `true`. Verify your `DelegateCommand` predicate.
- **Button doesn't appear at all** — placed outside `<dxe:ButtonEdit>` direct children, or the editor doesn't inherit from `ButtonEdit` (e.g., `TextEdit`, `MemoEdit`, `CheckEdit`). Check the inheritance.
- **Left/right ordering looks wrong** — buttons render in collection order within each side. To control ordering on the left side, reorder them in XAML; `IsLeft` decides only which side they appear on.
- **`GlyphKind` icon is invisible** — the theme might not define a glyph for the active state. Try `Content="..."` with a text label, or use a `ContentTemplate` with an explicit `Path` / image.

## Source Material

- `articles/controls-and-libraries/data-editors/visual-elements/editor-button.md` (https://docs.devexpress.com/content/WPF/10457?md=true)
- `articles/controls-and-libraries/data-editors/getting-started/how-to-create-a-functional-buttonedit.md` (https://docs.devexpress.com/content/WPF/10483?md=true)
- `articles/controls-and-libraries/data-editors/examples/how-to-create-a-buttonedit-with-a-custom-button.md` (https://docs.devexpress.com/content/WPF/7420?md=true)
