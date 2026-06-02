# Appearance Customization

Beyond `View` selection, `DXTabControl` and `DXTabItem` expose three extended styling features that don't require overriding theme resources:

- **Colorized tabs** — per-tab background / border color via `AccentColor` / `BorderColor`
- **Custom theming** — per-state background templates (`NormalBackgroundTemplate`, `HoverBackgroundTemplate`, ...)
- **Customizable areas** — templated regions around the tab strip (left/right control box, content header/footer, panel)

## When to Use This Reference

Use this when you need to:

- Color one or two tabs differently from the rest (alerts, status)
- Replace tab-header backgrounds for normal / hover / selected / focused states without touching theme resources
- Add custom UI to the **left** or **right** of the tab strip (search box, "+" button, menu)
- Add custom UI **above** or **below** the page content area (breadcrumb, status bar)

## Colorized Tabs — `AccentColor` and `BorderColor`

```xaml
<dx:DXTabItem Header="Critical"
              AccentColor="Tomato"
              BorderColor="DarkRed">
    ...
</dx:DXTabItem>
```

| Property | Effect |
|---|---|
| `DXTabItem.AccentColor` | Background / foreground tint of the tab header (preserves default gradients and hover/selected highlight effects) |
| `DXTabItem.BorderColor` | Border color of the tab header |

> Both are `Color` values (not `Brush`). Use `"#FFC0392B"` or named colors like `"Tomato"`.

This is the right tool when you want individual tabs to stand out (a "Critical" tab in red, a "New" tab in green) while keeping the rest of the theme intact.

## Custom Theming — Per-State Background Templates

For full visual control over a tab's header background — without overriding theme resources — set per-state `DataTemplate`s:

| Property | Applies when |
|---|---|
| `DXTabItem.NormalBackgroundTemplate` | Default state |
| `DXTabItem.HoverBackgroundTemplate` | Mouse hover |
| `DXTabItem.SelectedBackgroundTemplate` | Tab is the selected tab |
| `DXTabItem.FocusedBackgroundTemplate` | Tab has keyboard focus |

```xaml
<dx:DXTabItem Header="Custom">
    <dx:DXTabItem.NormalBackgroundTemplate>
        <DataTemplate>
            <Border Background="#EEEEEE" CornerRadius="4,4,0,0"/>
        </DataTemplate>
    </dx:DXTabItem.NormalBackgroundTemplate>
    <dx:DXTabItem.SelectedBackgroundTemplate>
        <DataTemplate>
            <Border Background="White" BorderBrush="SteelBlue" BorderThickness="0,0,0,2"
                    CornerRadius="4,4,0,0"/>
        </DataTemplate>
    </dx:DXTabItem.SelectedBackgroundTemplate>
    <dx:DXTabItem.HoverBackgroundTemplate>
        <DataTemplate>
            <Border Background="#DDDDDD" CornerRadius="4,4,0,0"/>
        </DataTemplate>
    </dx:DXTabItem.HoverBackgroundTemplate>
</dx:DXTabItem>
```

Result: an Office/VS-style tab strip you've fully styled without touching the theme's resource dictionary.

### When to Use Templates vs. Colors

| Need | Choose |
|---|---|
| One color tweak per tab | `AccentColor` / `BorderColor` |
| Replace the visual entirely (rounded corners, custom borders, gradients, indicators) | Per-state `*BackgroundTemplate` |

Don't combine both for the same effect — pick the simpler one that fits.

## Customizable Areas — Templated Regions

`DXTabControl` exposes templated regions for custom content around the tab strip and content area:

| Property | Region |
|---|---|
| `ControlBoxLeftTemplate` | Left of the tab strip (e.g., app menu button) |
| `ControlBoxRightTemplate` | Right of the tab strip (e.g., "+" new-tab button) |
| `ControlBoxPanelTemplate` | Custom elements *inside* the tab panel (decorations between tabs) |
| `ContentHeaderTemplate` | A row above the page content area |
| `ContentFooterTemplate` | A row below the page content area |

> **`ControlBoxLeftTemplate` / `ControlBoxRightTemplate` render in the themed title bar** when the host window is `ThemedWindow`. They will not render correctly on a plain `Window`.

### Left and Right Control Box

```xaml
<dx:DXTabControl>
    <dx:DXTabControl.ControlBoxLeftTemplate>
        <DataTemplate>
            <Button Content="≡" Command="{Binding ShowMenuCommand}" Margin="4,0"/>
        </DataTemplate>
    </dx:DXTabControl.ControlBoxLeftTemplate>

    <dx:DXTabControl.ControlBoxRightTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <Button Content="+" Command="{Binding NewTabCommand}"     Margin="4,0"/>
                <Button Content="…" Command="{Binding MoreOptionsCommand}" Margin="4,0"/>
            </StackPanel>
        </DataTemplate>
    </dx:DXTabControl.ControlBoxRightTemplate>
    ...
</dx:DXTabControl>
```

Result: A menu button before the first tab and a New / More menu after the last tab — all aligned with the themed title bar.

### Content Header and Footer

```xaml
<dx:DXTabControl>
    <dx:DXTabControl.ContentHeaderTemplate>
        <DataTemplate>
            <Border Background="#F5F5F5" Padding="8,4">
                <TextBlock Text="{Binding DataContext.Breadcrumb, RelativeSource={RelativeSource AncestorType=dx:DXTabControl}}"/>
            </Border>
        </DataTemplate>
    </dx:DXTabControl.ContentHeaderTemplate>

    <dx:DXTabControl.ContentFooterTemplate>
        <DataTemplate>
            <Border Background="#F5F5F5" Padding="8,4">
                <TextBlock Text="{Binding DataContext.Status, RelativeSource={RelativeSource AncestorType=dx:DXTabControl}}"/>
            </Border>
        </DataTemplate>
    </dx:DXTabControl.ContentFooterTemplate>

    <dx:DXTabItem Header="Document 1"><local:DocEditor/></dx:DXTabItem>
</dx:DXTabControl>
```

Sandwiches every tab's content between a header (breadcrumb-style) and a footer (status bar).

### Control Box Panel (Decorations Between Tabs)

```xaml
<dx:DXTabControl>
    <dx:DXTabControl.ControlBoxPanelTemplate>
        <DataTemplate>
            <Rectangle Height="1" Fill="Gray" VerticalAlignment="Bottom"/>
        </DataTemplate>
    </dx:DXTabControl.ControlBoxPanelTemplate>
    ...
</dx:DXTabControl>
```

Adds a thin separator line that runs the entire width of the tab panel.

## Combined Example — Browser-Style with Colorized "Live" Tab + Custom Buttons

```xaml
<dx:DXTabControl ItemsSource="{Binding Tabs}">
    <dx:DXTabControl.View>
        <dx:TabControlStretchView TabNormalSize="160" TabMinSize="40"
                                  SelectedTabMinSize="100"
                                  DragDropMode="All"/>
    </dx:DXTabControl.View>

    <!-- + button on the right -->
    <dx:DXTabControl.ControlBoxRightTemplate>
        <DataTemplate>
            <Button Content="+" Width="24" Margin="4,0"
                    Command="{Binding DataContext.NewTabCommand,
                              RelativeSource={RelativeSource AncestorType=dx:DXTabControl}}"/>
        </DataTemplate>
    </dx:DXTabControl.ControlBoxRightTemplate>

    <dx:DXTabControl.ItemContainerStyle>
        <Style TargetType="dx:DXTabItem">
            <Setter Property="AllowHide" Value="True"/>
            <Setter Property="Header"    Value="{Binding Title}"/>
            <Style.Triggers>
                <!-- Color tabs that are running a long-running operation -->
                <DataTrigger Binding="{Binding IsLive}" Value="True">
                    <Setter Property="AccentColor" Value="ForestGreen"/>
                </DataTrigger>
                <DataTrigger Binding="{Binding HasError}" Value="True">
                    <Setter Property="AccentColor" Value="Tomato"/>
                    <Setter Property="BorderColor" Value="DarkRed"/>
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </dx:DXTabControl.ItemContainerStyle>

    <dx:DXTabControl.ItemTemplate>
        <DataTemplate>
            <local:TabPageView/>
        </DataTemplate>
    </dx:DXTabControl.ItemTemplate>
</dx:DXTabControl>
```

## Common Issues

- **`ControlBoxLeftTemplate` / `ControlBoxRightTemplate` don't appear or are clipped** — host is `Window`, not `ThemedWindow`. Convert the window (see getting-started.md).
- **`AccentColor` value error** — assigned a `Brush` where a `Color` is expected. Use `Color` (`"Tomato"`, `"#FFC0392B"`).
- **Per-state template only fires for one state** — only the template for the active state is applied; combine all states (Normal + Hover + Selected at minimum) to get a consistent look.
- **Content header/footer covers the page content** — they reserve a row in the layout, not an overlay. If you need an overlay, use a `Grid` inside the content template instead.
- **Template's `DataContext` is the tab item, not the parent view model** — use `RelativeSource={RelativeSource AncestorType=dx:DXTabControl}` to reach upward.
- **Theme change doesn't apply to per-state templates** — templates override the theme by design; they're invariant across themes unless you build them yourself.

## Source Material

- `articles/controls-and-libraries/layout-management/tab-control/concepts/appearance-customization.md` (`xref:113899`)
- `articles/controls-and-libraries/layout-management/tab-control/examples/how-to-change-tab-background-in-dxtabcontrol-and-keep-the-default-gradient-and-highlight-effects.md` (`xref:115323`)
- `articles/controls-and-libraries/layout-management/tab-control/examples/how-to-change-the-tab-background-in-dxtabcontrol-when-a-tab-is-selected-focused-or-hovered.md` (`xref:115324`)
- GitHub: *wpf-dxtabcontrol-customize-header-footer-and-control-box-areas*
