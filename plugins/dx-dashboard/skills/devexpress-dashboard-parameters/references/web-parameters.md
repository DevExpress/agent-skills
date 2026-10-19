# Web Parameters — ASP.NET Core Runtime Value Setting

This reference covers **how to set parameter values at runtime** in ASP.NET Core and from the JavaScript client. Parameter **definition and binding** are identical on every platform — see [parameter-model.md](parameter-model.md).

---

## Namespaces Required

```csharp
using DevExpress.AspNetCore;            // AddDevExpressControls, UseDevExpressControls
using DevExpress.DashboardAspNetCore;   // MapDashboardRoute, AspNetCoreDashboardExporter
using DevExpress.DashboardWeb;          // DashboardConfigurator, DashboardFileStorage, CustomParametersWebEventArgs
using DevExpress.DashboardCommon;       // Dashboard, DashboardParameter
using Microsoft.Extensions.FileProviders;  // IFileProvider — resolves dashboard files from the content root
using System.Linq;                      // FirstOrDefault()
```

No `DevExpress.AspNetCore.Dashboard` **namespace** exists — that string is the NuGet **package** name.

---

## Baseline Wiring (Program.cs)

```csharp
using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

IFileProvider fileProvider = builder.Environment.ContentRootFileProvider;

// Host the page that carries the control — Razor Pages (or MVC views).
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDevExpressControls();
builder.Services.AddHttpContextAccessor();   // needed to read headers/claims in CustomParameters

builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
    var configurator = new DashboardConfigurator();
    configurator.SetDashboardStorage(
        new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath));
    return configurator;
});

var app = builder.Build();

app.UseStaticFiles();        // the control's client resources
app.UseDevExpressControls();
app.UseRouting();
app.MapRazorPages();         // (or MapControllerRoute for MVC) — without this the host page 404s
// First argument: the route the client control calls — match it to the JS control's
// `endpoint` (or DxDashboard.Endpoint in Blazor). Second: the name of your dashboard
// controller without the "Controller" suffix (here, DefaultDashboardController).
app.MapDashboardRoute("api/dashboard", "DefaultDashboard");
app.Run();
```

### Registering the Client-Side Handlers

The JavaScript handlers below do nothing until the control is told about them. With the ASP.NET Core HTML helper that happens in the Razor page, **not** by assigning to `window`:

```cshtml
@(Html.DevExpress().Dashboard("clientDashboardControl1")
    .ControllerName("DefaultDashboard")
    .WorkingMode(DevExpress.DashboardWeb.WorkingMode.Viewer)
    .Width("100%").Height("100%")
    .OnBeforeRender("onBeforeRender")   // name of the JS function, as a string
)
```

`onBeforeRender` receives the client-side `DashboardControl` instance — the object every snippet below calls `dashboardControl`.

---

## Server-Side: the CustomParameters Event

`CustomParameters` fires **every time data is requested**, exports included. Use it to inject values from headers, claims, or the session, and keep the handler fast. `e.Parameters` is a plain `List<DevExpress.Data.IParameter>`, so you can override an existing parameter or add one the dashboard never declared:

```csharp
using DevExpress.DashboardCommon;   // DashboardParameter
using DevExpress.DashboardWeb;      // DashboardConfigurator, CustomParametersWebEventArgs
using System.Linq;                  // FirstOrDefault()

builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
    var contextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
    var configurator = new DashboardConfigurator();
    configurator.SetDashboardStorage(
        new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath));

    // Type both lambda parameters: the web event args differ from the desktop ones.
    // e.DashboardId identifies the dashboard being processed.
    configurator.CustomParameters += (object sender, CustomParametersWebEventArgs e) => {
        // 1. Override a parameter the dashboard already defines (hidden or visible).
        //    A header is client-controlled: fine for a value the user may freely pick
        //    (a product, a date range), never for identity.
        string productId = contextAccessor?.HttpContext?.Request.Headers["ProductId"].FirstOrDefault();
        var productParam = e.Parameters.FirstOrDefault(p => p.Name == "productIdParameter");
        if (productParam != null && !string.IsNullOrEmpty(productId)) {
            productParam.Value = productId;
        }
    };

    return configurator;
});
```

Queries reference `[Parameters.productIdParameter]`. To supply a parameter the dashboard does not define, call `e.Parameters.Add(new DashboardParameter(name, type, value))` instead of overriding one.

`CustomParameters` changes the value the **queries** receive — it never changes what the Dashboard Parameters dialog displays.

---

## Request Parameter Values Before Data Loading

`Dashboard.RequestParameters` gates data loading on the user submitting parameters first. **It is `false` by default**, and applies only to a Web Dashboard.

When `true`, items display **"Waiting for Parameter Values…"** and data loads only after the user submits all *visible* parameters — no first render against defaults nobody chose.

```csharp
string path = fileProvider.GetFileInfo("Data/Dashboards/dashboard1.xml").PhysicalPath;

Dashboard dashboard = new Dashboard();
dashboard.LoadFromXml(path);
dashboard.RequestParameters = true;
dashboard.SaveToXml(path);
```

Two surfaces write the same dashboard-level setting:

| Surface | How |
|---------|-----|
| .NET | `Dashboard.RequestParameters` (`DevExpress.DashboardCommon`) |
| JavaScript model | `Dashboard.requestParameters` |

It is persisted in the dashboard XML as `<Dashboard RequestParameters="true" ...>`. A user who toggles it at runtime must trigger a data reload for it to take effect.

### Suppressing the Dialog Instead (Embedded Dashboard)

An embedded dashboard usually wants the opposite — no dialog, every value injected server-side. Two independent switches:

| Switch | Effect |
|--------|--------|
| `DashboardParameter.Visible = false` on **every** parameter | The dialog has nothing to show, so it never appears — and the **Parameters** button disappears from the dashboard title, making it unreachable |
| `Dashboard.RequestParameters = false` | Stops only the **prompt on initial load**. If visible parameters remain, the button stays and the user can still open the dialog |

Since `false` is already the default, assigning it changes nothing unless the saved dashboard has it set to `true`. If a dashboard prompts on load, that XML attribute is what to look for.

Suppressing the dialog supplies no values — pair it with `CustomParameters` or the queries run on defaults.

### Hide the Parameters (Set Visible to False)

```csharp
var hiddenParam = new DashboardParameter(
    "userIdParameter", typeof(string), string.Empty,
    "User ID (hidden)", false, null);   // visible: false, no look-up
dashboard.Parameters.Add(hiddenParam);
```

### Override Storage, When You Cannot Edit the Dashboard

When you can edit the dashboard, load it, change it, and save it — no storage subclass needed. Override storage only when the dashboard files are read-only, generated elsewhere, or the decision differs per user or per request.

`LoadDashboard` — declared on `DashboardStorageBase`, inherited by `DashboardFileStorage` — is **`protected virtual`** and returns an **`XDocument`**, not a `Dashboard`. Load the XML into a `Dashboard`, change it, return the serialized result:

```csharp
using System.Xml.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;

public class CustomDashboardStorage : DashboardFileStorage {
    public CustomDashboardStorage(string workingDirectory) : base(workingDirectory) { }

    protected override XDocument LoadDashboard(string dashboardID) {
        if (dashboardID != "dashboard1")
            return base.LoadDashboard(dashboardID);

        Dashboard dashboard = new Dashboard();
        dashboard.LoadFromXDocument(base.LoadDashboard(dashboardID));

        dashboard.RequestParameters = false;

        // You hold a real Dashboard here, so the whole model API is available.
        // Unlike CustomParameters, a parameter added here becomes part of the dashboard
        // and appears in the Dashboard Parameters dialog.
        dashboard.Parameters.Add(new DashboardParameter("OrderID", typeof(string), "ALFKI"));

        dashboard.RebuildLayout();          // sync the layout tree with the changed model
        return dashboard.SaveToXDocument();
    }
}
```

Then register it:

```csharp
configurator.SetDashboardStorage(new CustomDashboardStorage(
    fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath));
```

Rule of thumb: `CustomParameters` for values the user must not see or change; storage for parameters that belong to the dashboard itself. Per-request **values** go in `CustomParameters`, which has the request context — `LoadDashboard` does not.

> Derive from the storage **class** the app already uses — `DashboardFileStorage` here, `DashboardInMemoryStorage` or another `DashboardStorageBase` descendant elsewhere. Implementing `IDashboardStorage` directly instead makes `LoadDashboard` `public`, leaves no `base` to call, and requires `SaveDashboard` and `GetAvailableDashboardsInfo` as well.

---

## Client-Side: Send a Value to the Server as a Request Header

The browser sends the value in a request header, and the server reads it in `CustomParameters` (see [the CustomParameters Event](#server-side-the-customparameters-event)). There are two ways to set the header, depending on when you know the value.

**If you know the value when the page loads**, set the header once in `onBeforeRender`:

```javascript
function onBeforeRender(sender) {
    var dashboardControl = sender;
    var productId = 1;   // replace with the value your host page supplies
    dashboardControl.option('fetchRemoteService.headers', { 'ProductId': productId });
}
```

**If the value depends on what the user selects**, set the header each time the selection changes, then refresh the item that uses it. In this example, the user picks a row in the *Products* grid, and the *Sales per Product* chart shows that product:

```javascript
function onBeforeRender(sender) {
    var dashboardControl = sender;
    dashboardControl.findExtension('viewerApi').on('itemMasterFilterStateChanged', function (e) {
        onItemMasterFilterStateChanged(dashboardControl, e);
    });
}

function onItemMasterFilterStateChanged(dashboardControl, e) {
    if (e.itemName === 'gridDashboardItem1') {
        var viewerApi = dashboardControl.findExtension('viewerApi');
        var filterValues = viewerApi.getCurrentFilterValues(e.itemName);
        var productId = '';   // an empty header skips the override: the parameter keeps its default
        if (filterValues && filterValues.length) {
            var slice = viewerApi.getItemData(e.itemName).getSlice(filterValues[0].getAxisPoint());
            var productIdMeasure = slice.getMeasures().filter(m => m.dataMember === 'ProductID')[0];
            productId = slice.getMeasureValue(productIdMeasure.id).getValue();
        }
        // Send a ProductId value to the server using Request Headers
        dashboardControl.option('fetchRemoteService.headers', { 'ProductId': productId });
        // Refresh only the Chart Dashboard Item (component names, not display names)
        dashboardControl.refresh(['chartDashboardItem1']);
    }
}
```

> **Use the Fetch remote service if the value must also apply to exports.** The Ajax remote service exports through `form.submit`, which can't send custom headers. With Ajax, your parameter works for data requests but is missing from exports.
>
> If the app configures `ajaxRemoteService`, set `ajaxRemoteService.headers` instead. A control cannot use both services.

### Refresh Only the Affected Items After the Value Changes

Setting a new header value doesn't update anything by itself — the dashboard only sends it with the next data request. So after you change it, refresh the items that use it, like the second example does with `refresh(['chartDashboardItem1'])`. You don't need to change anything on the server: `CustomParameters` runs on every request.

Calling `refresh()` without arguments reloads every item. Use `reloadData()` only when the underlying data itself has changed — it clears the cache and requests everything again.

### Sync a Parameter to a Master-Filter Selection

Listen on the `viewerApi` extension — not the parameter dialog extension:

```javascript
var dashboardControl;
function onBeforeRender(sender) {
    dashboardControl = sender;
    var viewerApi = dashboardControl.findExtension('viewerApi');
    if (viewerApi) {
        viewerApi.on('itemMasterFilterStateChanged', syncParameterToMasterFilter);
    }
}

function syncParameterToMasterFilter(e) {
    if (e.itemName !== 'gridDashboardItem1') { return; }   // ComponentName
    var parameterDialogExt = dashboardControl.findExtension('dashboardParameterDialog');
    var parameter = parameterDialogExt.getParameters().getParameterByName('OrderID');
    if (!parameter) { return; }
    // e.values is empty when the user clears the master filter
    parameter.setValue(e.values.length ? e.values[0][0] : parameter.getDefaultValue());  // row-array shape
}
```

---

## Client-Side: the Parameter Dialog Extension

The `dashboardParameterDialog` extension exposes the parameters to JavaScript. Creating the control directly, without the Razor helper:

```html
<div id="dashboard"></div>

<script>
    var dashboardControl = new DevExpress.Dashboard.DashboardControl(
        document.getElementById("dashboard"), { endpoint: "/api/dashboard" });
    dashboardControl.render();
</script>
```

### Get/Set Values

```javascript
// Call from a user action (e.g. a button click) after the dashboard has loaded.
function setParameterValues(dashboardControl) {
    var parameters = dashboardControl.findExtension('dashboardParameterDialog').getParameters();
    parameters.getParameterByName("categoryParameter").setValue("Condiments");
    parameters.getParameterByName("startDateParameter").setValue(new Date(2015, 3, 1));

    // Multi-value parameters take an array:
    parameters.getParameterByName("categoriesParameter").setValue(["Beverages", "Condiments"]);
}
```

### Reset and Show the Dialog

```javascript
// Reset: setValue back to getDefaultValue().
function resetParameter(control, name) {
    var parameters = control.findExtension('dashboardParameterDialog').getParameters();
    var parameter = parameters.getParameterByName(name);
    if (parameter) { parameter.setValue(parameter.getDefaultValue()); }
}

function showParametersDialog(control) {
    control.findExtension('dashboardParameterDialog').show();
}
```

---

## Which Approach

| Need | Use |
|------|-----|
| Values come from the request (header, claim, session) | `CustomParameters` server-side; the browser supplies the header via `option('fetchRemoteService.headers', …)` in `onBeforeRender` |
| Embedded dashboard that must not show the dialog | `Visible = false` on every parameter, values via `CustomParameters`. `RequestParameters` is already `false` — clear it only if the dashboard was authored with it set. Custom storage **only** when you cannot edit the dashboard |
| Dashboard must wait for the user to submit values | `RequestParameters = true` (web only; items show "Waiting for Parameter Values…") |
| Users change values in the browser | `dashboardParameterDialog` extension: `getParameterByName(...).setValue(...)`, from a user action once the dashboard has loaded |
| A value changed and only some items need new data | `dashboardControl.refresh(['itemComponentName', ...])` |

> **Blazor** uses this same path. `DxDashboard` wraps the JavaScript control over the ASP.NET Core backend, so `DashboardConfigurator`, `CustomParameters`, and the custom storage class are unchanged; only the markup hosting the control differs.

---

## Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| `CS0234: 'Dashboard' does not exist in the namespace 'DevExpress.AspNetCore'` | `using DevExpress.AspNetCore.Dashboard;` | That namespace does not exist — use `DevExpress.DashboardWeb` / `DevExpress.DashboardAspNetCore` |
| `AddDevExpressDashboard` / `UseDevExpressDashboard` not found | Invented API names | `AddDevExpressControls()` / `UseDevExpressControls()` + `MapDashboardRoute(...)` |
| `CS0507: cannot change access modifiers when overriding 'LoadDashboard'` | Declared `public override` | It is `protected` and returns `XDocument` |
| Value not injected | Parameter missing from the dashboard, or name mismatch | Verify `e.Parameters` contains the exact, case-sensitive name |
| `NullReferenceException` in `CustomParameters` | No null check after `FirstOrDefault` | Always `if (param != null)`, and type the lambda parameters |
| Dashboard Parameters dialog still shows | `RequestParameters` not set on the dashboard, or the custom storage is not registered | Set `dashboard.RequestParameters = false` and save; if using the storage route, confirm `SetDashboardStorage(new CustomDashboardStorage(...))` |
| `getParameterByName` returns null | Called before the dashboard finished loading | Call it after `dashboardEndUpdate` has fired, e.g. from a button click |
| `CustomParameters` fires repeatedly | One call per data request | Expected; keep the handler cheap — no database calls, read from the header, claim, or session |
| Dialog still shows the old value after `CustomParameters` set a new one | By design — the event feeds queries, not the dialog | Nothing to fix; set `Visible = false` if the stale display confuses users |
| Header reaches data requests but not exports | Ajax remote service exports via `form.submit`, which cannot carry headers | Use the Fetch remote service (`fetchRemoteService.headers`) |
| Header set, but nothing changed on screen | No data re-request was made | Call `dashboardControl.refresh([...])` (or `reloadData()`) after setting the header |
| Whole dashboard reloads for a one-item change | `refresh()` called with no arguments | Pass component names: `refresh(['chartDashboardItem1'])` |

---

## Security: What Hidden Parameters Do and Do Not Protect

Values set in JavaScript come from the browser. If a parameter decides **which rows a user may see**, set it server-side in `CustomParameters` from the authenticated identity, keep it `Visible = false`, and never accept it from the client.

But `Visible = false` controls the **dialog**, not access. `IncludeHiddenParameters` exists on all four option classes — `DashboardPdfExportOptions`, `DashboardExcelExportOptions`, `DashboardImageExportOptions`, and `DashboardPrintPreviewOptions` — and enabling it together with `ExportParameters` lets the user read **every hidden parameter and its value** out of the exported or printed document. All four default to `false`.

| Concern | Does `Visible = false` + `CustomParameters` solve it? |
|---|---|
| **Integrity** — user must not choose which rows they see | Yes. The value comes from the authenticated identity, not the browser. |
| **Confidentiality** — user must not learn the value | **No.** Export or print preview can surface it. |

A user id used for filtering is normally fine to carry this way. A secret, API token, or credential is not — DevExpress's own guidance is to not store unencrypted sensitive information in dashboard parameters. Resolve those server-side (in `ConfigureDataConnection`, or inside the query/stored procedure) so they never enter the parameter model.

> **Multi-tenancy:** a parameter only filters rows inside dashboards and data sources every user shares. To restrict which dashboards, data sources, and connection strings each user can access, implement custom storages and providers — fetch `devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/402924/web-dashboard/integrate-dashboard-component/dashboard-backend/manage-multi-tenancy")`.
