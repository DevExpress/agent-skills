# Behaviors

Behaviors attach to a UI element and extend its functionality without code-behind. They're the MVVM-friendly way to:

- Route events to view-model commands (`EventToCommand`)
- Wire keyboard shortcuts (`KeyToCommand`)
- Auto-focus a control on load (`FocusBehavior`)
- Surface validation errors at a host (`ValidationErrorsHostBehavior`)
- Ask the user to confirm an action (`ConfirmationBehavior`)
- And a dozen other tasks

DevExpress ships a catalog of predefined behaviors. You can also build custom ones by inheriting `Behavior<T>`.

## When to Use This Reference

Use this when you need to:

- Attach a predefined behavior to a control
- Replace a code-behind event handler with `EventToCommand`
- Wire a keyboard shortcut to a command
- Apply a behavior to all controls of a specific type
- Build a custom `Behavior<T>` class

## How Behaviors Are Attached

### Single Control — `Interaction.Behaviors`

The `Interaction.Behaviors` attached property holds a collection of `Behavior` instances. Add behaviors as children:

```xaml
<TextBox xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm">
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:FocusBehavior/>
    </dxmvvm:Interaction.Behaviors>
</TextBox>
```

Multiple behaviors stack:

```xaml
<ListBox>
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:EventToCommand EventName="MouseDoubleClick"
                               Command="{Binding EditCommand}"/>
        <dxmvvm:KeyToCommand KeyGesture="Delete"
                             Command="{Binding DeleteCommand}"/>
    </dxmvvm:Interaction.Behaviors>
</ListBox>
```

### All Controls of a Type — `BehaviorsTemplate`

To attach a behavior to every `GridControl` (or any other type) in a window via a `Style`:

```xaml
<Style TargetType="dxg:GridControl">
    <Setter Property="dxmvvm:Interaction.BehaviorsTemplate">
        <Setter.Value>
            <DataTemplate>
                <ContentControl>
                    <dxmvvm:KeyToCommand KeyGesture="CTRL+U"
                                         Command="{Binding UpdateCommand}"/>
                </ContentControl>
            </DataTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

For multiple behaviors via a style, use an `ItemsControl` as the template root:

```xaml
<Style TargetType="dxg:GridControl">
    <Setter Property="dxmvvm:Interaction.BehaviorsTemplate">
        <Setter.Value>
            <DataTemplate>
                <ItemsControl>
                    <dxmvvm:KeyToCommand KeyGesture="CTRL+U"
                                         Command="{Binding UpdateCommand}"/>
                    <dxmvvm:EventToCommand EventName="MouseDoubleClick"
                                           Command="{Binding EditCommand}"/>
                </ItemsControl>
            </DataTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

### In Code-Behind

`Interaction.GetBehaviors(element).Add(...)`:

```csharp
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Bars;

public MainWindow() {
    InitializeComponent();
    Interaction.GetBehaviors(barSubItem)
        .Add(new BarSubItemThemeSelectorBehavior { ShowTouchThemes = false });
}
```

Rarely needed — XAML registration is cleaner.

## Predefined Behaviors — Catalog

### Routing Events / Input to Commands

| Behavior | Use |
|---|---|
| **`EventToCommand`** | Route any routed event (e.g., `MouseDoubleClick`, `SelectionChanged`) to a view-model command |
| **`KeyToCommand`** | Route a key gesture (e.g., `Ctrl+S`) to a view-model command |
| **`MethodToCommandBehavior`** | Bind a method invocation to a command |

### Focus / Visibility / State

| Behavior | Use |
|---|---|
| **`FocusBehavior`** | Focus the associated control on load |
| **`DependencyPropertyBehavior`** | Two-way bind a dependency property to a view-model property (when the DP isn't normally bindable) |
| **`ReadOnlyDependencyPropertyBindingBehavior`** | One-way bind a read-only DP into the view model |
| **`CurrentWindowSerializationBehavior`** | Save / restore the hosting window's position/size |

### Validation / Confirmation

| Behavior | Use |
|---|---|
| **`ValidationErrorsHostBehavior`** | Host validation errors on a specific element |
| **`ConfirmationBehavior`** | Ask the user to confirm an action before it proceeds |

### Data / Enumeration

| Behavior | Use |
|---|---|
| **`EnumItemsSourceBehavior`** | Bind an enum's values as a list (for `ComboBoxEdit`, `ListBox`, etc.) |
| **`FunctionBindingBehavior`** | Bind to a function result |

### Command Composition

| Behavior | Use |
|---|---|
| **`CompositeCommandBehavior`** | Combine multiple commands into one (fan-out execution) |

### Theme Selector Behaviors (Ribbon / Bars)

| Behavior | Use |
|---|---|
| `BarSubItemThemeSelectorBehavior` | Add theme selector to a `BarSubItem` |
| `BarSplitItemThemeSelectorBehavior` | Same for `BarSplitItem` |
| `RibbonGalleryItemThemeSelectorBehavior` | Same for `RibbonGalleryBarItem` |
| `RibbonGalleryItemThemePaletteSelectorBehavior` | Palette selector for ribbon gallery |
| `GalleryThemeSelectorBehavior` | Theme selector for `Gallery` |
| `HamburgerSubMenuThemeSelectorBehavior` | Theme selector for hamburger sub-menus |

## EventToCommand — The Most-Used Behavior

`EventToCommand` is the workhorse — it lets a view-model command react to any event on a control, replacing code-behind event handlers.

### Basic

```xaml
<ListBox ItemsSource="{Binding Items}">
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:EventToCommand EventName="MouseDoubleClick"
                               Command="{Binding EditCommand}"/>
    </dxmvvm:Interaction.Behaviors>
</ListBox>
```

When the user double-clicks the `ListBox`, `EditCommand` executes.

### Pass Event Args to Command

By default, `EventToCommand` passes `null` as the command parameter. To pass the event args:

```xaml
<dxmvvm:EventToCommand EventName="SelectionChanged"
                       Command="{Binding ItemSelectedCommand}"
                       PassEventArgsToCommand="True"/>
```

The view-model command receives a `SelectionChangedEventArgs` parameter.

### Convert Event Args

If the command wants a specific object instead of raw `EventArgs`, use a converter:

```csharp
public class ListBoxEventArgsConverter : IEventArgsConverter {
    public object Convert(object sender, object args) {
        var e = (SelectionChangedEventArgs)args;
        return e.AddedItems.Count > 0 ? e.AddedItems[0] : null;
    }
}
```

```xaml
<dxmvvm:EventToCommand EventName="SelectionChanged"
                       Command="{Binding ItemSelectedCommand}">
    <dxmvvm:EventToCommand.EventArgsConverter>
        <local:ListBoxEventArgsConverter/>
    </dxmvvm:EventToCommand.EventArgsConverter>
</dxmvvm:EventToCommand>
```

The command now receives the selected item directly, not the `SelectionChangedEventArgs`.

### Static `CommandParameter`

```xaml
<dxmvvm:EventToCommand EventName="Click"
                       Command="{Binding ExecuteCommand}"
                       CommandParameter="MyAction"/>
```

Useful when the same view-model command handles different button clicks, distinguished by parameter.

## KeyToCommand — Keyboard Shortcuts

Bind a key gesture to a command:

```xaml
<dxg:GridControl>
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:KeyToCommand KeyGesture="CTRL+U"
                             Command="{Binding UpdateCommand}"/>
        <dxmvvm:KeyToCommand KeyGesture="Delete"
                             Command="{Binding DeleteSelectedCommand}"/>
    </dxmvvm:Interaction.Behaviors>
</dxg:GridControl>
```

`KeyGesture` accepts WPF's `KeyGesture` syntax: `CTRL+S`, `Alt+F4`, `F5`, etc.

The behavior fires only when the associated control (or its descendants) has focus. To make a shortcut global, attach it to the window.

## FocusBehavior

Focus the associated control on load:

```xaml
<TextBox>
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:FocusBehavior/>
    </dxmvvm:Interaction.Behaviors>
</TextBox>
```

Or focus the associated control when another element raises an event (set `SourceName` to the trigger element and `EventName` to its event):

```xaml
<dxmvvm:FocusBehavior SourceName="searchButton" EventName="Click"/>
```

`FocusBehavior` exposes `SourceName` / `SourceObject` and `EventName` / `Event` (the trigger), plus `PropertyName` and `FocusDelay`. (There is no `IsFocused` property — to drive focus from view-model state, raise an event the behavior listens to, or use `IDispatcherService` to call `Focus()` on the element.)

## EnumItemsSourceBehavior

Surface an enum as a list source for a combo / listbox:

```csharp
public enum Priority { Low, Medium, High, Critical }
```

```xaml
<dxe:ComboBoxEdit EditValue="{Binding SelectedPriority}">
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:EnumItemsSourceBehavior EnumType="{x:Type local:Priority}"/>
    </dxmvvm:Interaction.Behaviors>
</dxe:ComboBoxEdit>
```

The combo's `ItemsSource` is populated with the enum's values. Use `[Display(Name = "...")]` on enum values for friendly captions.

## ConfirmationBehavior

Show a confirmation dialog before a command executes:

```xaml
<Button Content="Delete" Command="{Binding DeleteCommand}">
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:ConfirmationBehavior MessageTitle="Delete?"
                                     MessageText="Are you sure?"
                                     MessageButton="YesNo"
                                     MessageIcon="Question"/>
    </dxmvvm:Interaction.Behaviors>
</Button>
```

If the user picks "No", `DeleteCommand` doesn't run.

## ValidationErrorsHostBehavior

Surface a control's WPF validation errors on a parent / sibling element:

```xaml
<Grid>
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:ValidationErrorsHostBehavior Target="{Binding ElementName=hostBorder}"/>
    </dxmvvm:Interaction.Behaviors>
    <TextBox Text="{Binding Name, ValidatesOnNotifyDataErrors=True}"/>
    <Border x:Name="hostBorder"/>
</Grid>
```

Useful when the control with errors is buried inside a template and you want to surface the error decoration at a parent boundary.

## CompositeCommandBehavior

Run multiple commands as one:

```xaml
<Button Content="Save All">
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:CompositeCommandBehavior>
            <dxmvvm:CommandItem Command="{Binding SaveCommand1}"/>
            <dxmvvm:CommandItem Command="{Binding SaveCommand2}"/>
            <dxmvvm:CommandItem Command="{Binding SaveCommand3}"/>
        </dxmvvm:CompositeCommandBehavior>
    </dxmvvm:Interaction.Behaviors>
</Button>
```

Configures one button to fire several commands. `CanExecute` is aggregated (the composite is enabled only when every child command can execute).

## Common Patterns

### Pattern 1: Double-Click to Edit

```xaml
<dxg:GridControl ItemsSource="{Binding Items}">
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:EventToCommand EventName="MouseDoubleClick"
                               Command="{Binding EditCurrentCommand}"
                               PassEventArgsToCommand="False"/>
    </dxmvvm:Interaction.Behaviors>
    <dxg:GridControl.View>
        <dxg:TableView/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

The view-model's `CurrentItem` (auto-tracked by GridControl's `CurrentItem` binding) is the active row; `EditCurrentCommand` reads it without needing event args.

### Pattern 2: Hotkeys at the Window Level

```xaml
<dx:ThemedWindow>
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:KeyToCommand KeyGesture="CTRL+S" Command="{Binding SaveCommand}"/>
        <dxmvvm:KeyToCommand KeyGesture="CTRL+O" Command="{Binding OpenCommand}"/>
        <dxmvvm:KeyToCommand KeyGesture="F5"     Command="{Binding RefreshCommand}"/>
    </dxmvvm:Interaction.Behaviors>
    ...
</dx:ThemedWindow>
```

Global shortcuts — work regardless of which control has focus inside the window.

### Pattern 3: Auto-Focus First Field

```xaml
<TextBox Text="{Binding FirstName}">
    <dxmvvm:Interaction.Behaviors>
        <dxmvvm:FocusBehavior/>
    </dxmvvm:Interaction.Behaviors>
</TextBox>
```

### Pattern 4: Form with Enum-Backed Combo

```xaml
<StackPanel>
    <dxe:ComboBoxEdit EditValue="{Binding Status}">
        <dxmvvm:Interaction.Behaviors>
            <dxmvvm:EnumItemsSourceBehavior EnumType="{x:Type local:OrderStatus}"/>
        </dxmvvm:Interaction.Behaviors>
    </dxe:ComboBoxEdit>
    <dxe:ComboBoxEdit EditValue="{Binding Priority}">
        <dxmvvm:Interaction.Behaviors>
            <dxmvvm:EnumItemsSourceBehavior EnumType="{x:Type local:Priority}"/>
        </dxmvvm:Interaction.Behaviors>
    </dxe:ComboBoxEdit>
</StackPanel>
```

No need to expose the enum values as a collection on the view model.

## Custom Behaviors

For app-specific logic, derive from `Behavior<T>`:

```csharp
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Interactivity;

public class HighlightOnFocusBehavior : Behavior<TextBox> {
    protected override void OnAttached() {
        base.OnAttached();
        AssociatedObject.GotFocus  += OnGotFocus;
        AssociatedObject.LostFocus += OnLostFocus;
    }

    protected override void OnDetaching() {
        AssociatedObject.GotFocus  -= OnGotFocus;
        AssociatedObject.LostFocus -= OnLostFocus;
        base.OnDetaching();
    }

    void OnGotFocus(object sender, RoutedEventArgs e) =>
        AssociatedObject.Background = Brushes.LightYellow;
    void OnLostFocus(object sender, RoutedEventArgs e) =>
        AssociatedObject.Background = Brushes.White;
}
```

Use it:

```xaml
<TextBox>
    <dxmvvm:Interaction.Behaviors>
        <local:HighlightOnFocusBehavior/>
    </dxmvvm:Interaction.Behaviors>
</TextBox>
```

### Custom Behavior with Dependency Properties

For bindable behavior settings:

```csharp
public class ValidationBehavior : Behavior<TextBox> {
    public static readonly DependencyProperty InvalidValueProperty =
        DependencyProperty.Register(
            nameof(InvalidValue), typeof(string), typeof(ValidationBehavior),
            new PropertyMetadata(string.Empty, OnPropertyChanged));

    public string InvalidValue {
        get => (string)GetValue(InvalidValueProperty);
        set => SetValue(InvalidValueProperty, value);
    }

    static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ValidationBehavior)d).Update();

    protected override void OnAttached() {
        base.OnAttached();
        AssociatedObject.TextChanged += (_, _) => Update();
    }

    void Update() {
        if (AssociatedObject == null) return;
        AssociatedObject.Foreground =
            AssociatedObject.Text == InvalidValue ? Brushes.Red : Brushes.Black;
    }
}
```

```xaml
<TextBox Text="{Binding Email}">
    <dxmvvm:Interaction.Behaviors>
        <local:ValidationBehavior InvalidValue="{Binding BadEmail}"/>
    </dxmvvm:Interaction.Behaviors>
</TextBox>
```

### Behavior Lifecycle

| Method | When |
|---|---|
| `OnAttached()` | After `AssociatedObject` is set — subscribe to events, initialize state |
| `OnDetaching()` | Before the behavior is removed — unsubscribe events |

Always unsubscribe in `OnDetaching()` to avoid leaks.

## Common Issues

- **Behavior doesn't run** — `dxmvvm:` namespace missing, or behavior not inside `Interaction.Behaviors` collection. Check both.
- **`EventToCommand` doesn't bind** — `EventName` typo or the event isn't routed (some events don't bubble through MVVM behaviors). Verify the exact event name from the control's API.
- **`PassEventArgsToCommand` set but command signature doesn't match** — the command's parameter type must match the event args type (or use a converter to transform).
- **`KeyToCommand` doesn't trigger** — the control with the behavior must have focus. Attach to the window for global shortcuts.
- **`FocusBehavior` focuses too early** — the control isn't loaded yet. The behavior handles this internally; if you drive focus from a source event (`SourceName` + `EventName`), make sure that event actually fires after load. Use `FocusDelay` to defer focus if needed.
- **`EnumItemsSourceBehavior` ignored** — applied to a non-`Selector` control, or the `EnumType` value is wrong.
- **Custom behavior leaks** — forgot to unsubscribe in `OnDetaching()`. Always pair `OnAttached` event subscriptions with `OnDetaching` un-subscriptions.

## Source Material

- `articles/mvvm-framework/behaviors.md` (https://docs.devexpress.com/content/WPF/17442?md=true)
