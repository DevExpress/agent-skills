# DxReportViewer Customization — Native Report Viewer

## When to Use This Reference

Use this when you need to customize the **Native Report Viewer** (`DxReportViewer`) component:
- Add, hide, or disable toolbar buttons using C# callbacks
- Restrict export formats shown in the Export To drop-down
- Change or programmatically control the zoom level
- Show, hide, or replace the Parameters panel tab
- Pass report parameter values from code (suppress the built-in panel)
- Replace individual parameter editors with custom Blazor components
- Access the tab panel model to hide tabs

ALL customization for `DxReportViewer` is done in **C#/Razor only**.

## Required Usings

```razor
@using DevExpress.Blazor.Reporting
@using DevExpress.Blazor.Reporting.Models
@using DevExpress.XtraReports.UI
```

---

## Toolbar Customization

### Add a Custom Toolbar Button

Handle `OnCustomizeToolbar` to add new items to the toolbar. The callback receives a `ToolbarModel` which exposes `AllItems`:

```razor
@page "/viewer"
@rendermode InteractiveServer

@using DevExpress.Blazor.Reporting
@using DevExpress.Blazor.Reporting.Models
@using DevExpress.XtraReports.UI

<div @ref="viewerContainer" style="width: 100%; height: 1000px;">
    <DxReportViewer @ref="reportViewer"
                    Report="Report"
                    OnCustomizeToolbar="OnCustomizeToolbar" />
</div>

@code {
    DxReportViewer reportViewer;
    XtraReport Report = new SalesReport();
    ElementReference viewerContainer;

    void OnCustomizeToolbar(ToolbarModel toolbarModel) {
        toolbarModel.AllItems.Add(new ToolbarItem {
            IconCssClass = "oi oi-fullscreen-enter",
            Text = "Full Screen",
            AdaptiveText = "Full Screen",
            AdaptivePriority = 1,
            Click = async (args) => {
                // call any actions on the viewer
            }
        });
    }
}
```

### Hide a Toolbar Item by ID

Use `ToolbarItemId` constants to identify built-in items. Setting `Visible = false` removes the item; also set `GetEnabled = () => false` to disable it:

```razor
void OnCustomizeToolbar(ToolbarModel toolbarModel) {
    foreach (var item in toolbarModel.AllItems) {
        if (item.Id == ToolbarItemId.ExportTo) {
            item.GetEnabled = () => { return false; };
            item.Visible = false;
        }
    }
}
```

**Built-in toolbar item IDs** (from `DevExpress.Blazor.Reporting.Models.ToolbarItemId`):

| ID | Description |
|----|-------------|
| `FirstPage` | Navigate to first page |
| `PreviousPage` | Navigate to previous page |
| `PageOfPages` | Current page / total pages indicator |
| `NextPage` | Navigate to next page |
| `LastPage` | Navigate to last page |
| `HighlightEditingFields` | Highlight editable fields |
| `ZoomOut` | Decrease zoom by 5% |
| `Zoom` | Identifies the Zoom toolbar item |
| `ZoomIn` | Increase zoom by 5% |
| `Print` | Print document |
| `ExportTo` | Export drop-down button |
| `CancelDocumentCreation` | Cancel document generation |
| `Download` | Download document |
| `Search` | Search item |

---

## Export Format Filtering

Use `ExportModel.AvailableFormats` to restrict which formats appear in the Export To drop-down. This must be done in `OnAfterRender` after the first render when the component is initialized:

```razor
@page "/viewer"
@rendermode InteractiveServer

@using DevExpress.Blazor.Reporting
@using DevExpress.Blazor.Reporting.Models
@using DevExpress.XtraReports.UI

<div style="width: 100%; height: 1000px;">
    <DxReportViewer @ref="reportViewer" Report="Report" />
</div>

@code {
    DxReportViewer reportViewer;
    XtraReport Report = new SalesReport();
    string[] formats = new string[] { "PDF", "Image" };

    protected override void OnAfterRender(bool firstRender) {
        if (firstRender) {
            reportViewer.ExportModel.AvailableFormats.RemoveAll(item => !formats.Contains(item.Name));
        }
    }  
}
```

**Available format names** (string): `"CSV"`, `"DOCX"`, `"HTML"`, `"Image"`, `"MHT"`, `"PDF"`, `"RTF"`, `"Text"`, `"XLS"`, `"XLSX"`.

---

## Zoom Level

### Set at Declaration

```razor
<DxReportViewer Report="Report" Zoom="1.25" />
```

### Use ZoomConstants

```razor
@using DevExpress.Blazor.Reporting.Models

<DxReportViewer Report="Report" Zoom="ZoomConstants.PageWidth" />
```

| Constant | Meaning |
|----------|---------|
| `ZoomConstants.PageWidth` | Fit to page width |
| `ZoomConstants.WholePage` | Fit entire page in view |

### Update Zoom Programmatically

```razor
@code {
    DxReportViewer reportViewer;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    { 
      if (firstRender)
            await reportViewer.UpdateZoomAsync(ZoomConstants.PageWidth);
        await base.OnAfterRenderAsync(firstRender);
    }
}
```

### Single Page vs. Multi-Page Preview

```razor
<DxReportViewer Report="Report" SinglePagePreview="true" />
```

---

## Tab Panel

Tab Panel is a built-in viewer panel that appears on the right side of the viewer and displays report Parameters Tab, Search tab, Export Options tab, and Document Map Tab.

### Hide Tabs in Tab Panel

Option 1: Access `TabPanelModel` after first render in the page's `OnAfterRender` event, use `TabContentKind` to reference the required tab and hide it as follows:

```razor
@code {
    DxReportViewer reportViewer;
    XtraReport Report = new SalesReport();

    protected override void OnAfterRender(bool firstRender) 
    {
        if(firstRender) 
        {
            reportViewer.TabPanelModel[TabContentKind.Parameters].Visible = false;
            reportViewer.TabPanelModel[TabContentKind.Search].Visible = false;
            reportViewer.TabPanelModel[TabContentKind.ExportOptions].Visible = false;
            reportViewer.TabPanelModel[TabContentKind.DocumentMap].Visible = false;    
        }
        base.OnAfterRender(firstRender);
    }
}
```

Option 2 - Use the viewer's `OnCustomizeTabs` event to find the tab item by its ids and hide it:

```razor
@page "/reportviewer/"

@using DevExpress.Blazor.Reporting
@using DevExpress.Blazor.Reporting.Models
@using DevExpress.XtraReports.UI

<DxReportViewer @ref="reportViewer" 
                Report="Report" 
                OnCustomizeTabs="OnCustomizeTabs" />

@code {
    DxReportViewer reportViewer;
    XtraReport Report = new TestReport();

    void OnCustomizeTabs(List<TabModel> tabs) {
        foreach (TabModel tab in tabs) {
            if (tab.Id == "ReportViewer_Tabs_ExportOptions")
            {
                tab.Visible = false;
            }
        }
    }
}
```

### Add New Tab to Tab Panel

Use the OnCustomizeTabs event to access the Report Viewer’s collection of tabs and add a new custom tab.

```razor
@using DevExpress.AI.Samples.Blazor.Components.Reporting
@using DevExpress.AI.Samples.Blazor.Models
@using DevExpress.Blazor.Reporting.Models

<DxReportViewer @ref="Viewer" OnCustomizeTabs="OnCustomizeTabs">
</DxReportViewer>

@code {
    void OnCustomizeTabs(List<TabModel> tabs) {
        tabs.Add(new TabModel(new UserAssistantTabContentModel(() => CurrentReport), "AI", "AI Assistant") {
            TabTemplate = (tabModel) => {
                return (builder) => {
                    builder.OpenComponent<AITabRenderer>(0);
                    builder.AddComponentParameter(1, "Model", tabModel.ContentModel);
                    builder.CloseComponent();
                };
            }
        });
    }
}
```

### Pass Parameters Programmatically (Suppress Panel)

Set parameter values before passing the report to the viewer. Use `parameter.Visible = false` to hide the parameter from the built-in panel:

```razor
@code {
    XtraReport Report;

    protected override void OnInitialized() {
        var report = new SalesReport();
        report.Parameters["StartDate"].Value = new DateTime(2025, 1, 1);
        report.Parameters["StartDate"].Visible = false; // hide from parameter panel
        Report = report;
    }
}
```

### Reload Report with New Parameter Values via OpenReportAsync

When parameter values change after the viewer has already rendered, create a new report instance with updated values and pass it via `OpenReportAsync`:

```razor
<input @bind="paramValue" />
<button @onclick="SubmitParameter">Submit</button>

<DxReportViewer @ref="reportViewer" Report="@Report" />

@code {
    DxReportViewer reportViewer;
    XtraReport Report = new SalesReport();
    string paramValue = "";

    async Task SubmitParameter() {
        var report = new SalesReport();
        report.Parameters["CategoryName"].Value = paramValue;
        await reportViewer.OpenReportAsync(report);
    }
}
```

> `OpenReportAsync` is the correct way to replace the report at runtime. Do NOT reassign `Report` directly to a new instance after first render — use `OpenReportAsync` instead.

---

## Custom Parameter Editor

### The Parameter Editor built into the Parameters tab

Replace the built-in parameter editor widgets with custom Blazor components. The `OnCustomizeParameters` callback receives a `ParametersModel` — iterate `VisibleItems` and assign a `ValueTemplate` (`RenderFragment<ParameterModel>`):

```razor
@page "/customeditor/"

@using DevExpress.Blazor.Reporting
@using DevExpress.XtraReports.UI
@using BlazorCustomization.PredefinedReports
@using DevExpress.Blazor.Reporting.Models
@using Models

@implements IDisposable

<div @ref="viewerComponent" style="width: 100%; height: 1000px">
    <DxReportViewer @ref="reportViewer"
                    OnCustomizeParameters="OnCustomizeParameters"
                    Report="Report" />
</div>

@code {
    DxReportViewer reportViewer;
    XtraReport Report = new TableReport();
    private ElementReference viewerComponent;
    OrdersModel OrdersModel = new OrdersModel();
    ParameterModel ParameterModel { get; set; }

    void UnsubscribeParameters()
    {
        ParameterModel.ValueChanged -= OnParameterValueChanged;
    }

    void SubscribeParameters()
    {
        ParameterModel.ValueChanged += OnParameterValueChanged;
    }

    private void OnParameterValueChanged(object sender, EventArgs e)
    {
        var changedModel = sender as ParameterModel;
        OrdersModel.OrderId = (int)changedModel.Value;
    }

    void OnCustomizeParameters(ParametersModel parameters)
    {
        var parameter = parameters.VisibleItems
            .Where(param => param.Name == "OrderIdParameter").FirstOrDefault();
        parameter.ValueTemplate =@<CustomCombobox Model=OrdersModel ParameterModel=ParameterModel />;
        ParameterModel = parameter;
        SubscribeParameters();
    }

    public void Dispose()
    {
        if (ParameterModel != null)
        UnsubscribeParameters();
    }
}
```

### Create Standalone Parameter Panel and Hide Built-in

A common pattern is to hide the built-in Parameters tab entirely and provide a custom form outside the viewer. Call `reportViewer.ParametersModel.OnSubmitParameters()` to apply values and regenerate the document:

```razor
@page "/standalonepanel/"

@using DevExpress.Blazor.Reporting
@using DevExpress.XtraReports.UI
@using BlazorCustomization.PredefinedReports
@using DevExpress.Blazor.Reporting.Models
@using DevExpress.Blazor
@using Models

<div class="cw-880">
    <EditForm Model="@OrdersModel"
              OnValidSubmit="@HandleValidSubmit"
              Context="EditFormContext">
        <DataAnnotationsValidator />
        <DxFormLayout CssClass="w-75 parameters-panel-custom">
            <DxFormLayoutItem Caption="Order Id:" ColSpanMd="4">
                <CustomCombobox Model=OrdersModel ParameterModel=ParameterModel />
            </DxFormLayoutItem>
            <DxFormLayoutItem ColSpanMd="1">
                <DxButton SubmitFormOnClick="true"
                          Text="Submit"
                          Title="Press the key to reload the report with the parameters."
                          RenderStyle="ButtonRenderStyle.Secondary" />
            </DxFormLayoutItem>
            <DxFormLayoutItem ColSpanMd="7">
                <ValidationSummary />
            </DxFormLayoutItem>
        </DxFormLayout>
    </EditForm>
</div>

<div style="width: 100%; height: calc(100% - 4rem);">
    <DxReportViewer @ref="reportViewer"
                    OnCustomizeParameters="OnCustomizeParameters"
                    Report="Report" />
</div>

@code {
    DxReportViewer reportViewer;
    XtraReport Report = new TableReport();
    OrdersModel OrdersModel = new OrdersModel();
    ParameterModel ParameterModel { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            var parameterTab = reportViewer.TabPanelModel.Tabs
                .Where(Tab => Tab.ContentModel is ParametersModel)
                .FirstOrDefault();
            parameterTab.Visible = false;
            StateHasChanged();
        }
        base.OnAfterRender(firstRender);
    }

    void HandleValidSubmit()
    {
        if (!OrdersModel.OrdersData.Contains(OrdersModel.OrderId))
        {
            OrdersModel.OrdersData.Add(OrdersModel.OrderId);
            OrdersModel.OrdersData.Sort();
        }
        reportViewer.ParametersModel.OnSubmitParameters();
    }

    void OnCustomizeParameters(ParametersModel parameters)
    {
        ParameterModel = parameters.VisibleItems
            .Where(param => param.Name == "OrderIdParameter")
            .FirstOrDefault();
    }
}
```

### Example of Custom Parameter Editor Component - CustomCombobox

```razor
@using Models
@using DevExpress.Blazor.Reporting.Models

<DxComboBox Data="@Model.OrdersData" AllowUserInput="true" @bind-Value="Model.OrderId" TextChanged=OrdersTextChanged CssClass="cw-400 chi-220"></DxComboBox>

@code {
    [Parameter] public OrdersModel Model { get; set; }
    [Parameter] public ParameterModel ParameterModel { get; set; }

    void OrdersTextChanged(string val)
    {
        try
        {
            int id = Int32.Parse(val);
            Model.OrderId = id;
            ParameterModel.Value = id;
        }
        catch (FormatException)
        {
        }
    }
}
```

---

## Key Customization API Reference

| Member | Type | Description |
|--------|------|-------------|
| `Report` | `IReport` | The report instance to display. Assign in `OnInitialized`. |
| `OpenReportAsync(IReport)` | async method | Replace the current report at runtime |
| `Zoom` | `double` | Zoom factor (1.0 = 100%). Use `ZoomConstants` for fit modes |
| `UpdateZoomAsync(double)` | async method | Change zoom programmatically after render |
| `ZoomChanged` | event | Occurs when the zoom level changes. Inherited from `DxViewer`. |
| `SinglePagePreview` | `bool` | Display one page at a time |
| `OnCustomizeToolbar` | event | Allows you to customize the Viewer’s toolbar. Inherited from `DxViewer`. |
| `OnCustomizeTabs` | event | Allows you to modify the tab collection in the tab panel. Inherited from `DxViewer`. |
| `OnCustomizeParameters` | event | Allows you to modify parameter settings and specify custom editors for report parameters. |
| `OnCustomizeParameterLookUpSource` | event | Occurs when a lookup editor is created for a report parameter. Allows you to supply lookup values. |
| `OnExport` | event | Occurs before the Report Viewer sends a request to print the document or to get the exported document. Inherited from `DxViewer`. |
| `OnPreviewClick` | event | Occurs when a user clicks a report document. |
| `OnPreviewDoubleClick` | event | Occurs when a user double-clicks a report document. |
| `OnReportOpened` | event | Occurs when a report is opened in the Report Viewer. |
| `ExportModel` | property | Access `AvailableFormats` list |
| `TabPanelModel` | property | Access tab visibility and content models |
| `ParametersModel` | property | Access visible/all parameters, call `OnSubmitParameters()` |


---

## Customization Antipatterns — Native Report Viewer

**❌ A13: Apply JavaScript/JS-based callbacks to DxReportViewer**
- **Symptom**: No error, but toolbar customization has no effect
- **Why**: `DxReportViewer` uses C# callbacks only. `DxDocumentViewerCallbacks`, `CustomizeMenuActions`, and `CustomizeExportOptions` are JS-based APIs that belong to `DxDocumentViewer`/`DxWasmDocumentViewer`
- **Fix**: Use `OnCustomizeToolbar` and `ExportModel.AvailableFormats` for native viewer

**❌ Reassign `Report` property at runtime instead of calling `OpenReportAsync`**
- **Symptom**: Viewer may not re-render or shows stale document
- **Fix**: Use `await reportViewer.OpenReportAsync(newReport)` to replace the report at runtime

**❌ Calling `TabPanelModel` or `ExportModel` before `OnAfterRender(firstRender=true)`**
- **Symptom**: `NullReferenceException` or tab changes not reflected in UI
- **Fix**: Access `TabPanelModel` and `ExportModel` inside `OnAfterRender` when `firstRender == true`, then call `StateHasChanged()`