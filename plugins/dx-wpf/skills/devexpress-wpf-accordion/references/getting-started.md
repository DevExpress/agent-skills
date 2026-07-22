# Getting Started — DevExpress WPF Accordion Control

This guide walks through setting up `AccordionControl` in a .NET 8+ WPF project. The end result: a sidebar that's bound to a hierarchical ViewModel collection with department → employee structure.

## System Requirements

- .NET 8.0+ targeting Windows (or .NET Framework 4.6.2+)
- Visual Studio 2022+ or JetBrains Rider
- A valid DevExpress license

## Step 1: Install the NuGet Package

```bash
dotnet add package DevExpress.Wpf.Accordion
```

This brings `DevExpress.Wpf.Core` transitively.

All DevExpress packages in a project must share the same version.

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

## Step 3: Build the Data Model and ViewModel

```csharp
using System.Collections.ObjectModel;
using System.Linq;

namespace MyApp.ViewModels;

public class Employee {
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Position { get; set; } = "";
    public string Department { get; set; } = "";
    public override string ToString() => Name;
}

public class EmployeeDepartment {
    public string Name { get; set; } = "";
    public ObservableCollection<Employee> Employees { get; set; } = new();
    public override string ToString() => Name;
}

public class MainWindowViewModel {
    public ObservableCollection<EmployeeDepartment> EmployeeDepartments { get; }

    public MainWindowViewModel() {
        var staff = new[] {
            new Employee { Id = 1, Name = "Gregory S. Price", Department = "Management", Position = "President" },
            new Employee { Id = 2, Name = "Irma R. Marshall", Department = "Marketing", Position = "Vice President" },
            new Employee { Id = 3, Name = "Brian C. Cowling", Department = "Marketing", Position = "Manager" },
            new Employee { Id = 4, Name = "John C. Powell", Department = "Operations", Position = "Vice President" },
            new Employee { Id = 5, Name = "Harold S. Brandes", Department = "Operations", Position = "Manager" },
        };

        EmployeeDepartments = new ObservableCollection<EmployeeDepartment>(
            staff.GroupBy(e => e.Department)
                 .Select(g => new EmployeeDepartment {
                     Name = g.Key,
                     Employees = new ObservableCollection<Employee>(g)
                 }));
    }
}
```

The accordion can bind to anything that implements `IEnumerable` (or any descendant — `IList`, `ICollection`, `ObservableCollection<T>`, etc.). Use `ObservableCollection<T>` if items will be added/removed at runtime.

## Step 4: Add the AccordionControl

`MainWindow.xaml`:

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxa="http://schemas.devexpress.com/winfx/2008/xaml/accordion"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        Title="Employees" Height="500" Width="800">
    <Window.DataContext>
        <vm:MainWindowViewModel/>
    </Window.DataContext>
    <Grid>
        <dxa:AccordionControl ItemsSource="{Binding EmployeeDepartments}"
                              ChildrenPath="Employees"/>
    </Grid>
</Window>
```

| Property | Purpose |
|---|---|
| `ItemsSource` | The bound root collection (`EmployeeDepartments`) |
| `ChildrenPath` | Name of the property on each item that holds children (`Employees`) |
| `DisplayMemberPath` | Property used for the visible header text (optional — defaults to `ToString()`) |

In this example, the `ToString()` overrides on `EmployeeDepartment` and `Employee` make headers display correctly without `DisplayMemberPath`. Production code should usually set `DisplayMemberPath="Name"` explicitly:

```xaml
<dxa:AccordionControl ItemsSource="{Binding EmployeeDepartments}"
                      ChildrenPath="Employees"
                      DisplayMemberPath="Name"/>
```

## Step 5: Build and Run

```bash
dotnet build
dotnet run
```

You'll see a sidebar with department names as expandable root items; expanding one shows that department's employees.

## .NET Framework Variant

Required assembly references (when not using NuGet):

- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Xpf.Core.v<XX.X>.dll`
- `DevExpress.Xpf.Accordion.v<XX.X>.dll`
- `DevExpress.Xpf.Layout.Core.v<XX.X>.dll`
- `DevExpress.Mvvm.v<XX.X>.dll`

## What to Learn Next

- [Data Binding](data-binding.md) — full reference: `ChildrenPath` vs `ChildrenSelector` vs `HierarchicalDataTemplate`, plus static XAML items
- [When to Use](when-to-use.md) — choose between `AccordionControl`, `NavBarControl`, and `HamburgerMenu`
- [View Modes](view-modes.md) — Default vs NavigationPane
- [Items](items.md) — header, glyph, content customization
- [Search](search.md) — enable and configure the search box

## Source Material

- `articles/controls-and-libraries/navigation-controls/accordion-control/getting-started.md` (https://docs.devexpress.com/content/WPF/119805?md=true)
- `articles/controls-and-libraries/navigation-controls/accordion-control.md` (https://docs.devexpress.com/content/WPF/118347?md=true)
