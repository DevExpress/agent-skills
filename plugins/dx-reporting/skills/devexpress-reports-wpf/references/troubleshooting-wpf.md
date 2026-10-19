# Troubleshooting — WPF Reporting

## When to Use This Reference

Use when diagnosing issues with `DevExpress.Xpf.Printing.DocumentPreviewControl`, `DevExpress.Xpf.Reports.UserDesigner.ReportDesigner`, MVVM binding, XAML parsing errors, large report performance, or memory/resource problems in WPF reporting applications.

## Common Issues & Solutions

### DocumentPreviewControl Blank or Spinner Indefinitely

**Symptom**: Preview shows no content, spinner runs forever, or stays blank.

**Root causes**:
1. `RequestDocumentCreation="True"` not set
2. `DevExpress.Xpf.Printing.DocumentPreviewControl.DocumentSource` binding failed (ViewModel not assigned)
3. `DevExpress.XtraReports.UI.XtraReport.DataSource` is null
4. Report has no `DevExpress.XtraReports.UI.DetailBand`

**Solutions**:

```xaml
<!-- ✅ CORRECT -->
<dxp:DocumentPreviewControl RequestDocumentCreation="True"
                           DocumentSource="{Binding Report}" />

<!-- ❌ WRONG — RequestDocumentCreation missing -->
<dxp:DocumentPreviewControl DocumentSource="{Binding Report}" />
```

Check Output window for binding errors:

```
System.Windows.Data Error: 4 : 
Cannot find source for binding with reference 'Report'
```

If binding fails, verify:
- ViewModel assigned to DataContext
- Report property exists and is public
- Report has DetailBand (or other visible band)

# [C#](#tab/csharp-verify)
```csharp
// Verify report structure
var report = new SalesReport();
if (report.DetailBand == null) {
    MessageBox.Show("Report missing DetailBand!");
}
```

# [VB.NET](#tab/vbnet-verify)
```vb
Dim report = New SalesReport()
If report.DetailBand Is Nothing Then
    MessageBox.Show("Report missing DetailBand!")
End If
```
***

### XAML Parsing Error: Type Not Found

**Symptom**: Compile error like "Type 'DocumentPreviewControl' is not found in namespace".

**Root cause**: Missing XAML namespace declaration.

**Solution**: Add namespace at window top:

```xaml
<Window xmlns:dxp="http://schemas.devexpress.com/winfx/2008/xaml/printing"
        xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner">
    <!-- content -->
</Window>
```

Common namespaces needed:
- `dx:` — Core (ThemedWindow)
- `dxp:` — Printing (DocumentPreviewControl)
- `dxrud:` — Designer (ReportDesigner)
- `dxmvvm:` — MVVM (ViewModelSource)

### ReportDesigner Type Not Found

**Symptom**: "dxrud:ReportDesigner is not found".

**Root cause**: Missing namespace + possibly missing WPF subscription.

**Solution**:

1. Add namespace:
```xaml
xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner"
```

2. Verify WPF subscription installed:
```bash
dotnet list package --include-transitive | grep DevExpress.Wpf
```

If only `DevExpress.Win.Reporting` is installed, upgrade to `DevExpress.Wpf.Reporting`.

### MVVM Binding Not Updating Preview

**Symptom**: ViewModel property changes, but preview doesn't update.

**Root cause**: ViewModel not properly implementing INotifyPropertyChanged.

**Solution**: Use DevExpress MVVM base class:

# [C#](#tab/csharp-mvvm-fix)
```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

[POCOViewModel]
public class MyViewModel : ViewModelBase {
    // Property automatically notifies changes
    public virtual XtraReport Report { get; protected set; }
    
    public void LoadReport() {
        Report = new SalesReport();  // Binding updates
    }
}
```

# [VB.NET](#tab/vbnet-mvvm-fix)
```vb
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations

<POCOViewModel>
Public Class MyViewModel
    Inherits ViewModelBase
    
    Public Overridable Property Report As XtraReport
    
    Public Sub LoadReport()
        Report = New SalesReport()
    End Sub
End Class
```
***

Or implement INotifyPropertyChanged manually:

# [C#](#tab/csharp-inotify)
```csharp
public class MyViewModel : INotifyPropertyChanged {
    private XtraReport _report;
    public XtraReport Report {
        get => _report;
        set {
            if (_report != value) {
                _report = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Report)));
            }
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
}
```

# [VB.NET](#tab/vbnet-inotify)
```vb
Public Class MyViewModel
    Implements INotifyPropertyChanged

    Private _report As XtraReport

    Public Property Report As XtraReport
        Get
            Return _report
        End Get
        Set(value As XtraReport)
            If _report IsNot value Then
                _report = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Report)))
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged
End Class
```
***

Verify binding works:

```xaml
<!-- Test with simple TextBlock binding first -->
<TextBlock Text="{Binding Report.Name}" />

<!-- If this updates, binding is OK. Then test DocumentPreviewControl -->
<dxp:DocumentPreviewControl DocumentSource="{Binding Report}" />
```

### Large Report Hangs UI (1000+ Pages)


**Symptom**: Preview freezes for 30+ seconds while building document.

**Root cause**: Document creation blocks the main UI thread.

**Solution**: Use `DevExpress.XtraPrinting.Caching.CachedReportSource` with
`DevExpress.XtraPrinting.Caching.CachedReportSource.CreateDocumentAsync()`:

# [C#](#tab/csharp-large)
```csharp
public async void LoadLargeReport() {
    var report = new HeavyReport { DataSource = GetData() };
    
    // Wrap in cached source
    var storage = new MemoryDocumentStorage();
    var cached = new CachedReportSource(report, storage);
    
    // Assign (shows spinner immediately)
    Report = cached;
    
    // Build asynchronously (UI remains responsive)
    await cached.CreateDocumentAsync();
}
```

# [VB.NET](#tab/vbnet-large)
```vb
Public Async Sub LoadLargeReport()
    Dim report = New HeavyReport With {.DataSource = GetData()}
    
    Dim storage = New MemoryDocumentStorage()
    Dim cached = New CachedReportSource(report, storage)
    
    Report = cached
    
    Await cached.CreateDocumentAsync()
End Sub
```
***

XAML (same as normal):

```xaml
<dxp:DocumentPreviewControl RequestDocumentCreation="True"
                           DocumentSource="{Binding Report}" />
```

The preview shows a spinner during build, then displays pages as they're generated.

### Designer Opens But Can't Save

**Symptom**: ReportDesigner works, but reports don't save or saving throws error.

**Root cause**: Storage handler not implemented or database connection failed.

**Solution**: Assign an implementation of
`DevExpress.Xpf.Reports.UserDesigner.IReportStorage` to the Report Designer's
`DevExpress.Xpf.Reports.UserDesigner.ReportDesignerBase.ReportStorage` property before the
designer is displayed.

# [C#](#tab/csharp-storage-troubleshooting)
```csharp
private void Window_Loaded(object sender, RoutedEventArgs e) {
    designer.ReportStorage = new MyReportStorage();
}
```

# [VB.NET](#tab/vbnet-storage-troubleshooting)
```vb
Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
    designer.ReportStorage = New MyReportStorage()
End Sub
```
***

Test connection:

```csharp
// Verify database connectivity
try {
    using (var db = new AppDbContext()) {
        db.Database.OpenConnection();
        db.Database.CloseConnection();
        MessageBox.Show("Database OK");
    }
} catch (Exception ex) {
    MessageBox.Show($"Database error: {ex.Message}");
}
```

# [VB.NET](#tab/vbnet-database)
```vb
Try
    Using db = New AppDbContext()
        db.Database.OpenConnection()
        db.Database.CloseConnection()
        MessageBox.Show("Database OK")
    End Using
Catch ex As Exception
    MessageBox.Show($"Database error: {ex.Message}")
End Try
```
***

### Parameter Panel Not Showing

**Symptom**: Report has parameters, but parameter panel doesn't display.

**Root cause**: Parameters marked `Visible=False` or parameter panel visibility disabled.

**Solution**:

```csharp
// Ensure parameters are visible (default)
var report = new SalesReport();
report.Parameters["Year"].Visible = true;    // ✅ Show
report.Parameters["Region"].Visible = false; // ❌ Hide from UI
```

# [VB.NET](#tab/vbnet-parameter-panel)
```vb
Dim report = New SalesReport()
report.Parameters("Year").Visible = True
report.Parameters("Region").Visible = False
```
***

Or set in XAML:

```xaml
<dxp:DocumentPreviewControl AutoShowParametersPanel="True" ... />
```



### Designer Opens Empty (No Report Loaded)

**Symptom**: ReportDesigner window appears but no document in tabs.

**Root cause**: `OpenDocument()` called before window is fully loaded.

**Solution**: Call in `Window_Loaded`, not constructor:

```csharp
// ❌ WRONG — Constructor runs before window is shown
public DesignerWindow() {
    InitializeComponent();
    designer.OpenDocument(new SalesReport());  // Too early!
}

// ✅ CORRECT — After window is loaded
private void Window_Loaded(object sender, RoutedEventArgs e) {
    designer.OpenDocument(new SalesReport());
}
```

# [VB.NET](#tab/vbnet-designer-loaded)
```vb
Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
    designer.OpenDocument(New SalesReport())
End Sub
```
***

XAML:

```xaml
<Window Loaded="Window_Loaded">
    <dxrud:ReportDesigner x:Name="designer" />
</Window>
```

### NuGet Version Mismatch Errors

**Symptom**: Runtime errors like "Could not load type 'DevExpress.Xpf.Printing...'".

**Root cause**: Different DevExpress package versions installed (e.g., 26.1.3 + 26.1.5).

**Solution**: Pin all packages to same version in `.csproj`:

```xml
<PackageReference Include="DevExpress.Wpf.Reporting" Version="26.1.5" />
<PackageReference Include="DevExpress.Wpf.Core" Version="26.1.5" />
<PackageReference Include="DevExpress.Mvvm" Version="26.1.5" />
```

Verify all installed:

```bash
dotnet list package
```

Output should show all DevExpress packages at 26.1.5 (or your chosen version).

### MVVM ViewModelSource Not Resolving

**Symptom**: "Cannot resolve 'ViewModelSource'".

**Root cause**: Missing `DevExpress.Mvvm` NuGet package.

**Solution**:

```bash
dotnet add package DevExpress.Mvvm
```

Or add XAML namespace:

```xaml
xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
```


## Troubleshooting Checklist

### Before reporting issue, verify:

- [ ] All DevExpress NuGet packages same version (e.g., 26.1.5)
- [ ] All XAML namespaces declared (dx:, dxp:, dxrud:, dxmvvm:)
- [ ] Report has DetailBand or other visible band
- [ ] ViewModel property raises PropertyChanged
- [ ] `DevExpress.Xpf.Reports.UserDesigner.IReportStorage` implemented and assigned to `DevExpress.Xpf.Reports.UserDesigner.ReportDesignerBase.ReportStorage` if using designer
- [ ] Output window checked for binding errors
