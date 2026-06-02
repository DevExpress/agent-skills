# Getting Started — WPF Reporting (.NET 6+)

## When to Use This Reference

Use when setting up a new .NET 6+ WPF project with DevExpress reporting.

## NuGet Setup

```bash
dotnet nuget add source https://nuget.devexpress.com/api --name DevExpressNuGet

dotnet add package DevExpress.Wpf.Reporting
dotnet add package DevExpress.Mvvm          # optional — for POCO ViewModel helpers
```

## Required Usings (C#)

```csharp
using DevExpress.Xpf.Printing;                    // DocumentPreviewControl, PrintHelper
using DevExpress.Xpf.Reports.UserDesigner;        // ReportDesigner (WPF license)
using DevExpress.XtraReports.UI;                  // XtraReport
using DevExpress.XtraPrinting.Caching;            // CachedReportSource
using DevExpress.Mvvm;                            // ViewModelBase, POCOViewModel (optional)
```

## XAML Namespace Declarations

```xaml
xmlns:dxp="http://schemas.devexpress.com/winfx/2008/xaml/printing"
xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner"
xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
```

## Minimal Preview — PrintHelper

The simplest approach — no XAML changes required:

```csharp
// MainWindow.xaml.cs
private void btnPreview_Click(object sender, RoutedEventArgs e) {
    var report = new SalesReport { DataSource = GetData() };
    PrintHelper.ShowRibbonPrintPreviewDialog(this, report);
}
```

## Minimal Preview — DocumentPreviewControl

```xaml
<!-- MainWindow.xaml -->
<Window xmlns:dxp="http://schemas.devexpress.com/winfx/2008/xaml/printing"
        x:Class="MyApp.MainWindow">
    <dxp:DocumentPreviewControl x:Name="preview"
                               RequestDocumentCreation="True" />
</Window>
```

```csharp
// MainWindow.xaml.cs
private void Window_Loaded(object sender, RoutedEventArgs e) {
    preview.DocumentSource = new SalesReport { DataSource = GetData() };
}
```

## Minimal Designer

```xaml
<Window xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner"
        x:Class="MyApp.DesignerWindow">
    <dxrud:ReportDesigner x:Name="reportDesigner" />
</Window>
```

```csharp
private void Window_Loaded(object sender, RoutedEventArgs e) {
    reportDesigner.OpenDocument(new SalesReport());
}
```

## Project File (.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="DevExpress.Wpf.Reporting" Version="26.1.*" />
    <PackageReference Include="DevExpress.Mvvm" Version="26.1.*" />
  </ItemGroup>
</Project>
```

> Always set `<UseWPF>true</UseWPF>` in the project file for WPF apps targeting .NET 6+.
