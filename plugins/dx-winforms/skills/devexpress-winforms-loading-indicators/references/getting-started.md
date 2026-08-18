# Getting Started

This reference covers which NuGet package to install, which assemblies and namespaces are involved, and the minimum code required to show each loading indicator type.

## NuGet Package

All loading indicator classes ship in:

```
DevExpress.Win.Navigation
```

| Class | Assembly | Namespace |
|---|---|---|
| `SplashScreenManager` | `DevExpress.XtraEditors.v26.1.dll` | `DevExpress.XtraSplashScreen` |
| `WaitForm` | `DevExpress.XtraEditors.v26.1.dll` | `DevExpress.XtraWaitForm` |
| `ProgressPanel` | `DevExpress.XtraEditors.v26.1.dll` | `DevExpress.XtraWaitForm` |
| Overlay Form (via `SplashScreenManager`) | `DevExpress.XtraEditors.v26.1.dll` | `DevExpress.XtraSplashScreen` |
| Fluent/Skin Splash Screen (via `SplashScreenManager`) | `DevExpress.XtraEditors.v26.1.dll` | `DevExpress.XtraSplashScreen` |

> If your project already references `DevExpress.Win` or `DevExpress.Win.Design`, no additional package is needed — those umbrella packages include `DevExpress.Win.Navigation`.

## Common Namespaces

```csharp
using DevExpress.XtraSplashScreen;   // SplashScreenManager, overlay form API
using DevExpress.XtraWaitForm;        // WaitForm base class, ProgressPanel
```

---

## Splash Screen — Minimum Code

The simplest modern splash screen uses the built-in Fluent template. No custom form required — one static method call shows it; another closes it.

```csharp
// In Program.cs — before Application.Run(new MainForm())
SplashScreenManager.ShowFluentSplashScreen(
    title:                "My Application",
    subtitle:             "Version 1.0",
    rightFooter:          "Starting...",
    leftFooter:           "© 2026 My Company",
    loadingIndicatorType: FluentLoadingIndicatorType.Dots,
    useFadeIn:            true,
    useFadeOut:           true
);

Application.Run(new MainForm());

// In MainForm's constructor or Form_Load — after initialization completes:
SplashScreenManager.CloseForm();
```

---

## Wait Form — Minimum Code

1. Add a `WaitForm1` class derived from `DevExpress.XtraWaitForm.WaitForm` to the project.
2. Show/hide it via a `SplashScreenManager` instance or the static API:

```csharp
// Show (opens in a separate thread — non-blocking)
splashScreenManager1.ShowWaitForm();

// Optionally update labels
splashScreenManager1.SetWaitFormCaption("Loading data...");
splashScreenManager1.SetWaitFormDescription("Fetching from server");

// ... do work ...

// Close
splashScreenManager1.CloseWaitForm();
```

Static (no `SplashScreenManager` instance needed):

```csharp
SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
// ... do work ...
SplashScreenManager.CloseForm();
```

---

## Overlay Form — Minimum Code

No custom form class required. Call `ShowOverlayForm`, get a handle, and close it when done:

```csharp
using DevExpress.XtraSplashScreen;

private async void btnLoad_Click(object sender, EventArgs e)
{
    IOverlaySplashScreenHandle handle = SplashScreenManager.ShowOverlayForm(gridControl1);
    try {
        await LoadDataAsync();
    }
    finally {
        SplashScreenManager.CloseOverlayForm(handle);
    }
}
```

The overlay can target any control or the form itself.

---

## ProgressPanel — Minimum Code (Inline in Form)

`ProgressPanel` is a regular control you place directly on a form or user control. No separate thread required.

```csharp
using DevExpress.XtraWaitForm;

// Declare progressPanel1 in MainForm.Designer.cs with Visible = false, then:
private async void btnLoad_Click(object sender, EventArgs e)
{
    progressPanel1.Caption     = "Loading";
    progressPanel1.Description = "Please wait...";
    progressPanel1.Visible     = true;
    progressPanel1.BringToFront();

    try {
        await LoadDataAsync();
    }
    finally {
        progressPanel1.Visible = false;
    }
}
```

Or create it in code:

```csharp
var pp = new ProgressPanel {
    Caption            = "Loading",
    Description        = "Please wait...",
    WaitAnimationType  = DevExpress.Utils.Animation.WaitingAnimatorType.Ring,
    Dock               = DockStyle.Fill
};
Controls.Add(pp);
pp.BringToFront();
```

---

## Quick Reference

| Indicator | How to show it |
|---|---|
| Splash Screen (Fluent/Skin) | Call `SplashScreenManager.ShowFluentSplashScreen(...)` before `Application.Run()` |
| Wait Form | Add a `WaitForm` descendant, then call `ShowWaitForm()` / `CloseWaitForm()` |
| Overlay Form | Call `SplashScreenManager.ShowOverlayForm(control)` |
| ProgressPanel | Add the control to the form with `Visible = false`; toggle it in code |

## Source Material

- Splash Screen Manager: `https://docs.devexpress.com/content/WindowsForms/10826?md=true`
- Overlay Form: `https://docs.devexpress.com/content/WindowsForms/120029?md=true`
- Wait Form: `https://docs.devexpress.com/content/WindowsForms/10824?md=true`
- `SplashScreenManager` class: `xref:DevExpress.XtraSplashScreen.SplashScreenManager`
- `ProgressPanel` class: `xref:DevExpress.XtraWaitForm.ProgressPanel`
