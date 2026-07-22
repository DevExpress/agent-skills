# Getting Started — DevExpress WPF Data Editors

This guide walks you through adding DevExpress Data Editors (`TextEdit`, `DateEdit`, `ComboBoxEdit`, etc.) to a .NET 8+ WPF project. The same NuGet packages also work on .NET Framework 4.6.2+.

## System Requirements

- .NET 8.0+ targeting Windows
- Visual Studio 2022+ or JetBrains Rider
- A NuGet source providing DevExpress packages
- A valid DevExpress license

## Step 1: NuGet Source

Install the DevExpress packages from nuget.org — it's registered by default in Visual Studio and the .NET SDK.

## Step 2: Configure the Project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

The `-windows` suffix is required — Data Editors are Windows-only.

## Step 3: Install NuGet Packages

For the core editors (`TextEdit`, `ButtonEdit`, `ComboBoxEdit`, `DateEdit`, `SpinEdit`, `CheckEdit`, `MemoEdit`, `PasswordBoxEdit`, etc.):

```bash
dotnet add package DevExpress.Wpf.Core
```

For `LookUpEdit` and the lookup family (lives in `DevExpress.Xpf.Grid.LookUp`):

```bash
dotnet add package DevExpress.Wpf.Grid.Core
```

> **Important**: All DevExpress packages in one project must share the same version.

## Step 4: Add XAML Namespaces

In `MainWindow.xaml`:

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors"
        xmlns:dxg="http://schemas.devexpress.com/winfx/2008/xaml/grid"
        xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
        Title="MyApp" Height="400" Width="500">
    ...
</Window>
```

Namespace cheat sheet:

| Prefix | URI | Use for |
|---|---|---|
| `dxe:` | `.../winfx/2008/xaml/editors` | Most editors (`TextEdit`, `DateEdit`, `ComboBoxEdit`, `ButtonEdit`, etc.) and simple controls in `DevExpress.Xpf.Editors` |
| `dxg:` | `.../winfx/2008/xaml/grid` | `LookUpEdit` and its style-settings variants |
| `dx:` | `.../winfx/2008/xaml/core` | `SimpleButton`, `DropDownButton`, `SplitButton`, `ThemeManager`, `ThemedWindow` |

## Step 5: Place Editors

A minimal form with a few editors:

```xaml
<StackPanel Margin="20">
    <TextBlock Text="Name"/>
    <dxe:TextEdit EditValue="{Binding Name, Mode=TwoWay}" Margin="0,0,0,8"/>

    <TextBlock Text="Age"/>
    <dxe:SpinEdit EditValue="{Binding Age, Mode=TwoWay}"
                  MinValue="0" MaxValue="120" Margin="0,0,0,8"/>

    <TextBlock Text="Birthday"/>
    <dxe:DateEdit EditValue="{Binding Birthday, Mode=TwoWay}" Margin="0,0,0,8"/>

    <TextBlock Text="Active"/>
    <dxe:CheckEdit IsChecked="{Binding IsActive, Mode=TwoWay}"
                   Content="Account is active" Margin="0,0,0,8"/>

    <dx:SimpleButton Content="Save"
                     Command="{Binding SaveCommand}"
                     HorizontalAlignment="Left" Width="100"/>
</StackPanel>
```

## Step 6: ViewModel (MVVM)

```csharp
using DevExpress.Mvvm;
using System;

namespace MyApp.ViewModels;

public class MainViewModel : ViewModelBase {
    public string Name {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public int Age {
        get => GetValue<int>();
        set => SetValue(value);
    }

    public DateTime Birthday {
        get => GetValue<DateTime>();
        set => SetValue(value);
    }

    public bool IsActive {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public ICommand SaveCommand => new DelegateCommand(() => {
        // ... save logic
    });
}
```

`ViewModelBase` and `DelegateCommand` live in `DevExpress.Mvvm` (already in `DevExpress.Wpf.Core`).

Bind the ViewModel:

```xaml
<Window.DataContext>
    <vm:MainViewModel/>
</Window.DataContext>
```

## Step 7: Build and Run

```bash
dotnet build
dotnet run
```

## Why `EditValue`, Not `Text` / `Value`

Every editor derived from `BaseEdit` exposes **`EditValue`** as the canonical bindable property. It's typed `object` but accepts the editor's natural type — `string` for `TextEdit`, `decimal`/`double`/`int` for `SpinEdit`, `DateTime` for `DateEdit`, etc.

| Property | When to use |
|---|---|
| **`EditValue`** | Almost always. Two-way binding, mask-aware, validation-aware. |
| `Text` (on `TextEdit` family) | Only when you need the raw text representation specifically. Masks of type `Numeric` / `DateTime` may apply incorrectly if you bind `Text`. |
| `IsChecked` (on `CheckEdit`, `ToggleSwitchEdit`) | OK — `IsChecked` is the natural binding for boolean editors. `EditValue` works too and stays parallel with other editors. |

## What to Learn Next

- [Editor Varieties](editor-varieties.md) — full inventory of editors, what each is for, operation modes via `StyleSettings`
- [Simple Controls](simple-controls.md) — `SimpleButton`, `DropDownButton`, `Calculator`, `RangeControl`, `DateNavigator`
- [Masks](masks.md) — masked input (numeric, date-time, simple, regex)
- [Buttons in ButtonEdit](buttons.md) — adding custom buttons to `ButtonEdit`, `ComboBoxEdit`, `SpinEdit`, etc.

## Source Material

- `articles/controls-and-libraries/data-editors.md` (root)
- `articles/controls-and-libraries/data-editors/getting-started/how-to-create-a-registration-form/lesson-1-create-layout.md`
- `articles/controls-and-libraries/data-editors/included-components.md` (https://docs.devexpress.com/content/WPF/6933?md=true)
