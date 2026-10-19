# Desktop Parameters — WinForms & WPF Runtime Value Setting

This reference covers **how to set parameter values at runtime** on WinForms and WPF. Parameter **definition and binding** are identical on every platform — see [parameter-model.md](parameter-model.md).

---

## Namespaces Required

### WinForms
```csharp
using DevExpress.DashboardCommon;   // Dashboard, DashboardParameter, DashboardState, DashboardParameterState,
                                    // CustomParametersEventArgs
using DevExpress.DashboardWin;      // DashboardViewer, DashboardDesigner, DashboardParameterDescriptor,
                                    // SetInitialDashboardStateEventArgs
```

### WPF
```csharp
using DevExpress.DashboardCommon;   // Dashboard, DashboardState, DashboardParameterState
using DevExpress.DashboardWpf;      // DashboardControl   ← NOT DevExpress.Xpf.Dashboard
```

---

## Two Different Objects Named "Parameter"

| Object | Where you get it | Value member | Can add a parameter? |
|--------|------------------|--------------|----------------------|
| `DashboardParameter` (model) | `dashboard.Parameters`, and `e.Parameters` in `CustomParameters` | `Value` | Yes — `Add(new DashboardParameter(...))` |
| `DashboardParameterDescriptor` (WinForms UI state) | `dashboardViewer1.Parameters` / `dashboardDesigner1.Parameters` — a `ReadOnlyCollection<DashboardParameterDescriptor>` | `SelectedValue`, `SelectedValues` (`DefaultValue`, `Values`, `Name`, `Type` are read-only) | No — the collection has no `Add`/`Remove` |

Neither control exposes `SetParameterValue`/`GetParameterValue`, and the model object lacks a `DefaultValue` property.

---

## WinForms

### Approach 1: BeginUpdateParameters / EndUpdateParameters (Change Values Now)

**When to use:** the viewer is running and you want to change values and refresh — a button click, a menu command, a timer. **Not at load time** — see the anti-pattern below.

```csharp
using System;
using System.Collections.Generic;
using DevExpress.DashboardWin;      // DashboardViewer

private void setValueButton_Click(object sender, EventArgs e) {
    dashboardViewer1.BeginUpdateParameters();

    // Single-value parameter: SelectedValue.
    dashboardViewer1.Parameters["yearParameter"].SelectedValue = "2024";

    // Multi-value parameter: SelectedValues (a collection), NOT SelectedValue.
    dashboardViewer1.Parameters["customerIdParameter"].SelectedValues =
        new List<object> { "ALFKI", "AROUT", "BONAP" };

    // EndUpdateParameters applies the changes and refreshes the dashboard.
    dashboardViewer1.EndUpdateParameters();
}
```

`Parameters["name"]` uses the parameter's `Name`, not its `Description`, and is case-sensitive. Set every value between the two calls — they batch the changes into one refresh.

### Approach 2: SetInitialDashboardState (Values Applied on Load)

**When to use:** default values that must be in effect the first time the dashboard renders.

```csharp
using System;
using System.Collections.Generic;
using DevExpress.DashboardCommon;   // Dashboard, DashboardState, DashboardParameterState
using DevExpress.DashboardWin;      // SetInitialDashboardStateEventArgs

private void Form1_Load(object sender, EventArgs e) {
    // Subscribe BEFORE assigning the dashboard, or the event will not fire for this load.
    dashboardViewer1.SetInitialDashboardState += DashboardViewer1_SetInitialDashboardState;

    Dashboard dashboard = new Dashboard();
    dashboard.LoadFromXml(@"Data\Dashboard.xml");
    dashboardViewer1.Dashboard = dashboard;
}

private void DashboardViewer1_SetInitialDashboardState(
    object sender, SetInitialDashboardStateEventArgs e) {

    DashboardState state = new DashboardState();

    state.Parameters.Add(new DashboardParameterState {
        Name = "yearParameter", Type = typeof(string), Value = "2024"
    });

    // Multi-value: a list value.
    state.Parameters.Add(new DashboardParameterState {
        Name = "customerIdParameter",
        Type = typeof(string),
        Value = new List<string> { "ALFKI", "AROUT", "BONAP" }
    });

    e.InitialState = state;
}
```

### Approach 3: GetDashboardState / SetDashboardState (Any Time)

**When to use:** saving and restoring the whole state (parameters, filters, drill-down) — not single-value edits.

```csharp
using System;
using DevExpress.DashboardCommon;   // DashboardState, DashboardParameterState

private void saveStateButton_Click(object sender, EventArgs e) {
    DashboardState currentState = dashboardViewer1.GetDashboardState();

    foreach (DashboardParameterState parameterState in currentState.Parameters) {
        if (parameterState.Name == "yearParameter") {
            parameterState.Value = "2025";
        }
    }

    dashboardViewer1.SetDashboardState(currentState);
}
```

### Approach 4: CustomParameters (Change Values Before Each Query)

**When to use:** values that code owns, applied on every data request — the desktop equivalent of the web pattern. It fires per request, so no database calls inside it.

```csharp
using System;
using System.Linq;
using DevExpress.DashboardCommon;   // Dashboard, DashboardParameter, CustomParametersEventArgs

private void Form1_Load(object sender, EventArgs e) {
    // Subscribe before assigning the dashboard so the first data request is covered.
    dashboardViewer1.CustomParameters += DashboardViewer1_CustomParameters;

    Dashboard dashboard = new Dashboard();
    dashboard.LoadFromXml(@"Data\Dashboard.xml");
    dashboardViewer1.Dashboard = dashboard;
}

private void DashboardViewer1_CustomParameters(object sender, CustomParametersEventArgs e) {
    // e.Parameters is List<DevExpress.Data.IParameter> — override an existing parameter...
    var countryParam = e.Parameters.FirstOrDefault(p => p.Name == "countryParameter");
    if (countryParam != null) {
        countryParam.Value = "Brazil";
    }

    // ...or add one the dashboard does not define.
    // e.Parameters.Add(new DashboardParameter("countryParameter", typeof(string), "Brazil"));
}
```

The event args here are `CustomParametersEventArgs` (`DevExpress.DashboardCommon`); ASP.NET Core uses `CustomParametersWebEventArgs` (`DevExpress.DashboardWeb`). `DashboardDesigner` exposes the same event.

### Showing the Dialog

```csharp
dashboardViewer1.ShowDashboardParametersForm();
```

Only parameters with `Visible = true` appear in it.

---

## WPF

### Approach 1: CustomParameters Event

```csharp
using System.Linq;
using DevExpress.DashboardCommon;   // CustomParametersEventArgs
using DevExpress.DashboardWpf;      // DashboardControl

public MainWindow() {
    InitializeComponent();
    dashboardControl.CustomParameters += DashboardControl_CustomParameters;
    dashboardControl.LoadDashboard(@"Data\Dashboard.xml");
}

private void DashboardControl_CustomParameters(object sender, CustomParametersEventArgs e) {
    var yearParam = e.Parameters.FirstOrDefault(p => p.Name == "yearParameter");
    if (yearParam != null) {
        yearParam.Value = "2024";
    }
}
```

### Approach 2: SetDashboardState / GetDashboardState

```csharp
using DevExpress.DashboardCommon;   // DashboardState, DashboardParameterState
using DevExpress.DashboardWpf;      // DashboardControl
using System.Windows;               // RoutedEventArgs

private void SetParameterButton_Click(object sender, RoutedEventArgs e) {
    // Start from the current state. A new DashboardState() clears master filters and drill-down.
    DashboardState state = dashboardControl.GetDashboardState();
    foreach (DashboardParameterState parameterState in state.Parameters) {
        if (parameterState.Name == "yearParameter")
            parameterState.Value = "2024";
    }
    dashboardControl.SetDashboardState(state);
}

private void GetParameterButton_Click(object sender, RoutedEventArgs e) {
    DashboardState currentState = dashboardControl.GetDashboardState();
    foreach (DashboardParameterState parameterState in currentState.Parameters) {
        System.Diagnostics.Debug.WriteLine($"{parameterState.Name} = {parameterState.Value}");
    }
}
```

`DashboardControl` also raises `SetInitialDashboardState`, but its handler takes `SetInitialDashboardStateWpfEventArgs` (`DevExpress.DashboardWpf`), not the WinForms `SetInitialDashboardStateEventArgs`. `ShowParametersButton` controls the **Parameters** button in the dashboard title.

WPF has no `DashboardParameterDescriptor` collection — use `CustomParameters` or dashboard state.

---

## Which Approach

| Need | WinForms | WPF |
|------|----------|-----|
| In effect on the first render | `SetInitialDashboardState` | `SetInitialDashboardState` |
| Code owns the value on every data request | `CustomParameters` | `CustomParameters` |
| Change values in response to a user action | `BeginUpdateParameters` + `SelectedValue(s)` + `EndUpdateParameters` | `SetDashboardState` |
| Save or restore the whole state | `GetDashboardState` / `SetDashboardState` | `GetDashboardState` / `SetDashboardState` |

When the first two both apply — a security value that must also be right on the first render — use `CustomParameters`: it re-applies on every request, while initial state can be changed by the user afterwards.

---

## Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| `NullReferenceException` on `Parameters["name"]` | No dashboard assigned yet, or the name is wrong | Assign `dashboardViewer1.Dashboard` first; names are case-sensitive |
| `InvalidOperationException: The Dashboard Viewer is not initialized…` | Viewer API called during startup, before initialization completed | Set the value on the `Dashboard` model before assigning it, or use `SetInitialDashboardState`. Moving the call into the `Initialized` event does **not** work — the parameter is still `null` there |
| Changes don't show | `EndUpdateParameters` never called | Always pair it with `BeginUpdateParameters` |
| `SetInitialDashboardState` never fires | Subscribed after the dashboard was assigned | Subscribe before assigning `Dashboard` / calling `LoadDashboard` |
| Multi-value shows only one value | Assigned a list to `SelectedValue` | Use `SelectedValues` |
| `CS0246: DashboardControl could not be found` (WPF) | `using DevExpress.Xpf.Dashboard;` | It is `DevExpress.DashboardWpf` |
| New parameter never appears | `Parameters[...]` (the descriptor collection) only sets values of parameters that already exist | Add it to `dashboardViewer1.Dashboard.Parameters` — `Dashboard` returns the live instance, so no reload is needed — or supply it per-request with `e.Parameters.Add(...)` in `CustomParameters` |
| Dashboard refreshes repeatedly | Duplicate `CustomParameters` subscriptions, or heavy handler | Subscribe once; keep the handler cheap |

---

## Anti-Patterns to Avoid

### ❌ Wrong: Using the Viewer API Before the Viewer Has Initialized

Both throw — the first because `Dashboard` is still `null`, the second because loading a dashboard does **not** mean the viewer has finished initializing:

```csharp
private void Form1_Load(object sender, EventArgs e) {
    dashboardViewer1.Dashboard.LoadFromXml(@"Data\Dashboard.xml");   // NullReferenceException
    dashboardViewer1.Parameters["yearParameter"].SelectedValue = "2023";
}
```

```csharp
private void Form1_Load(object sender, EventArgs e) {
    dashboardViewer1.LoadDashboard(@"Data\Dashboard.xml");
    dashboardViewer1.BeginUpdateParameters();   // InvalidOperationException:
                                                // "The Dashboard Viewer is not initialized and can cause
                                                //  incorrect API execution. Use the Initialized event to
                                                //  indicate that initialization is complete."
    dashboardViewer1.Parameters["yearParameter"].SelectedValue = "2023";
    dashboardViewer1.EndUpdateParameters();
}
```

**Fix 1 — set the value on the `Dashboard` model before handing it to the viewer.** Simplest; the model object has no initialization state.

```csharp
private void Form1_Load(object sender, EventArgs e) {
    Dashboard dashboard = new Dashboard();
    dashboard.LoadFromXml(@"Data\Dashboard.xml");
    dashboard.Parameters["yearParameter"].Value = "2023";   // model parameter: Value
    dashboardViewer1.Dashboard = dashboard;
}
```

**Fix 2 —** `SetInitialDashboardState` (Approach 2) also applies values at load time without touching the viewer API.

**Waiting for the `Initialized` event does not fix this**, even though the exception message suggests it: inside an `Initialized` handler `Parameters["yearParameter"]` still returns `null`, so setting `SelectedValue` throws a `NullReferenceException`. Verified in a WinForms project — use Fix 1 or Fix 2.

After startup the restriction is gone: from a button click or any other post-startup code, `BeginUpdateParameters` works directly.

---

### ❌ Wrong: Untyped Handler and No Null Check

```csharp
dashboardControl.CustomParameters += (s, e) => {
    var param = e.Parameters.FirstOrDefault(p => p.Name == "yearParameter");
    param.Value = "2024";   // throws when the parameter is absent
};
```

**Fix:**

```csharp
dashboardControl.CustomParameters += (object sender, CustomParametersEventArgs e) => {
    var param = e.Parameters.FirstOrDefault(p => p.Name == "yearParameter");
    if (param != null) { param.Value = "2024"; }
};
```
