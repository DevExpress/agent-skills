# Loading Indicators: Types and Comparison

DevExpress WinForms provides four categories of loading indicator. This document explains what each one is, when to use it, and how to work with its API.

---

## Terminology Note: "Loading Decorator"

In the WPF DevExpress library there is a control called `LoadingDecorator` that wraps a region and shows a spinner while its content loads. **There is no `LoadingDecorator` class in WinForms.** The WinForms equivalent is the **Overlay Form** — which overlaps an existing control with a semi-transparent animation. When this skill refers to "Loading Decorator" it means the Overlay Form.

---

## Overview Table

| Indicator | Class / API entry point | Where it appears | Thread | When to use |
|---|---|---|---|---|
| Splash Screen | `SplashScreenManager.ShowFluentSplashScreen` / `ShowForm` | Full screen, centered | Separate thread | App startup — branding before the main window is ready |
| Wait Form | `SplashScreenManager.ShowWaitForm()` | Full-form modal overlay | Separate thread | Long operations that must block the whole window |
| Overlay Form | `SplashScreenManager.ShowOverlayForm(control)` | Semi-transparent overlay on one control or form | Separate thread | Async operations that must block one panel or control |
| ProgressPanel | `ProgressPanel` control | Inline anywhere on a form | Main thread | Non-blocking feedback within a panel; no need to block interaction |

---

## 1. Splash Screen

### Variants
- **Fluent Splash Screen** — Windows 10-inspired acrylic glass. Code only. Best for modern apps.
- **Skin Splash Screen** — colors follow the DevExpress skin.
- **Default Splash Screen** — classic non-skin appearance; supports auto show/close on startup.
- **Splash Image** — displays any bitmap (PNG with transparency supported).

### API Pattern (Fluent — recommended)

```csharp
// Program.cs, before Application.Run()
var options = new FluentSplashScreenOptions {
    Title                = "My Application",
    Subtitle             = "Enterprise Edition",
    RightFooter          = "Loading...",
    LeftFooter           = "© 2026",
    LoadingIndicatorType = FluentLoadingIndicatorType.Dots
};

SplashScreenManager.ShowFluentSplashScreen(
    options,
    parentForm: null,
    useFadeIn:  true,
    useFadeOut: true
);

Application.Run(new MainForm());
```

```csharp
// MainForm.cs — after initialization
protected override void OnLoad(EventArgs e)
{
    base.OnLoad(e);
    // ... load data, initialize controls ...
    SplashScreenManager.CloseForm();
}
```

### API Pattern (Skin Splash Screen)

```csharp
// Program.cs
SplashScreenManager.ShowForm(null, typeof(SplashScreen1), false, true, false);
Application.Run(new MainForm());

// MainForm.cs
SplashScreenManager.CloseForm();
```

### Constraints
- `CloseForm()` must always be called — if not called before the app exits, a hang may occur.
- Splash screens can only receive data updates via the `SplashScreenManager.Default.SendCommand(Enum, object)` mechanism (`SendCommand` is an instance method — call it on `SplashScreenManager.Default` or a component instance). To update a label, your splash screen must override `ProcessCommand`.
- Do **not** show a splash screen over an already-visible main form — use the Overlay Form instead.

---

## 2. Wait Form

A skin-aware modal window containing a `ProgressPanel` (animated spinner or bar) plus two labels. Runs in a separate thread so it stays animated while your main thread does work.

### Prerequisites
- A `SplashScreenManager` component on the form (or the static `SplashScreenManager.Default`).
- A `WaitForm1` class derived from `DevExpress.XtraWaitForm.WaitForm`.

### API Pattern

```csharp
// Show — opens in a separate thread
splashScreenManager1.ShowWaitForm();

// Update labels from the main thread at any time
splashScreenManager1.SetWaitFormCaption("Processing...");
splashScreenManager1.SetWaitFormDescription("Row 42 of 1000");

// Close
splashScreenManager1.CloseWaitForm();
```

Static usage (no component instance):

```csharp
SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
// ... work ...
SplashScreenManager.CloseForm();
```

### Sending Custom Data to a Wait Form

For anything beyond caption/description, call `SendCommand`:

```csharp
// Define a custom command enum (in a shared location)
public enum WaitFormCommand { UpdateProgress }

// Send from main thread (SendCommand is an instance method — use SplashScreenManager.Default)
SplashScreenManager.Default.SendCommand(WaitFormCommand.UpdateProgress, 50);

// WaitForm1 — override ProcessCommand
public override void ProcessCommand(Enum cmd, object arg)
{
    base.ProcessCommand(cmd, arg);
    if ((WaitFormCommand)cmd == WaitFormCommand.UpdateProgress)
        progressBarControl1.EditValue = (int)arg;
}
```

### Cancellation

To allow the user to cancel, add a cancel button to the `WaitForm1` designer. In the button click, set a shared `CancellationTokenSource`.

### Constraints
- The Wait Form must be created by a `SplashScreenManager` — you cannot `new WaitForm1()` and show it normally.
- `SetWaitFormCaption` / `SetWaitFormDescription` must be called from the **main thread** (they marshal internally).
- Do **not** update Wait Form controls directly from the main thread — use `SendCommand`.

---

## 3. Overlay Form (WinForms "Loading Decorator")

A semi-transparent layer placed over a specific control or form. Runs in a separate thread — does not block the UI message loop. The user cannot click through to the overlapped control.

### API Pattern — basic

```csharp
private async void btnRun_Click(object sender, EventArgs e)
{
    IOverlaySplashScreenHandle handle = SplashScreenManager.ShowOverlayForm(gridControl1);
    try {
        await LoadGridDataAsync();
    }
    finally {
        SplashScreenManager.CloseOverlayForm(handle);
    }
}
```

Overlay the whole form:

```csharp
IOverlaySplashScreenHandle handle = SplashScreenManager.ShowOverlayForm(this);
```

### Appearance Options

```csharp
IOverlaySplashScreenHandle handle = SplashScreenManager.ShowOverlayForm(
    owner:        gridControl1,
    startupDelay: 300,        // ms before the overlay appears (avoids flash on fast ops)
    backColor:    Color.White,
    opacity:      180,        // 0-255
    fadeIn:       true,
    fadeOut:      true,
    imageSize:    new Size(64, 64)
);
```

### Custom Painter

To replace the default animation with your own drawing:

```csharp
class LoadingTextPainter : OverlayWindowPainterBase
{
    protected override void Draw(OverlayWindowCustomDrawContext context)
    {
        context.Handled = true;        // suppress default
        context.DrawBackground();      // draw tinted background

        var cache = context.DrawArgs.Cache;
        var bounds = context.DrawArgs.Bounds;
        cache.DrawString("Loading...", new Font("Segoe UI", 14),
            Brushes.Black, bounds, new StringFormat {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });
    }
}

// Show with custom painter
handle = SplashScreenManager.ShowOverlayForm(panel1,
    customPainter: new LoadingTextPainter());
```

### Constraints
- The target control must already have a handle (`IsHandleCreated == true`). Calling `ShowOverlayForm` before the form's `Load` event fires throws `InvalidOperationException`.
- Always close via `try/finally` — a leaked overlay leaves the control permanently blocked.
- `startupDelay` prevents flickering on fast operations; prefer a value of 200–500 ms.

---

## 4. ProgressPanel (Inline Wait Indicator)

A lightweight control you place directly on a form, panel, or user control. Unlike the previous options, `ProgressPanel` runs on the **main thread** and is simply visible or hidden. It does not block interaction with the rest of the form.

Add it with `Visible = false` initially, and optionally set `Dock = Fill` inside a dedicated panel or a `LayoutControlGroup`.

### Key Properties

| Property | Type | Description |
|---|---|---|
| `Caption` | `string` | Main text (e.g., "Loading") |
| `Description` | `string` | Secondary text (e.g., "Please wait...") |
| `ShowCaption` | `bool` | Whether to display the caption |
| `ShowDescription` | `bool` | Whether to display the description |
| `WaitAnimationType` | `WaitingAnimatorType` | `Ring`, `Line`, or `Bar` |

### API Pattern

```csharp
private async void btnLoad_Click(object sender, EventArgs e)
{
    progressPanel1.Caption     = "Refreshing";
    progressPanel1.Description = "Fetching data from the server...";
    progressPanel1.Visible     = true;
    progressPanel1.BringToFront();

    btnLoad.Enabled = false;
    try {
        await RefreshAsync();
    }
    finally {
        progressPanel1.Visible = false;
        btnLoad.Enabled        = true;
    }
}
```

### Code-Only Creation

```csharp
var pp = new DevExpress.XtraWaitForm.ProgressPanel {
    Caption           = "Loading",
    WaitAnimationType = DevExpress.Utils.Animation.WaitingAnimatorType.Ring,
    Dock              = DockStyle.Fill
};
somePanel.Controls.Add(pp);
pp.BringToFront();
```

### Constraints
- Because it runs on the main thread, `ProgressPanel` does not protect the rest of the UI from being accessed during a sync operation. Use `async/await` or disable controls explicitly.
- It is a standard control — `Show()` / `Hide()` work but `Visible = true/false` is more idiomatic in WinForms.
- Place it inside a dedicated `Panel` or `PanelControl` to keep it constrained to one area.

---

## Decision Guide

```
Is this shown at app startup (before the main window)?
  YES → Splash Screen (Fluent or Skin)

Is the operation triggered at runtime?
  Does it need to block the ENTIRE form?
    YES → Wait Form
  Does it need to block ONE control or panel only?
    YES → Overlay Form  ← WinForms equivalent of WPF LoadingDecorator
  No blocking needed — just visual feedback within the form?
    YES → ProgressPanel
```

---

## Source Material

- Splash Screen Manager overview: `https://docs.devexpress.com/content/WindowsForms/10826?md=true`
- Fluent Splash Screen: `https://docs.devexpress.com/content/WindowsForms/401719?md=true`
- Skin Splash Screen: `https://docs.devexpress.com/content/WindowsForms/401718?md=true`
- Wait Form: `https://docs.devexpress.com/content/WindowsForms/10824?md=true`
- Overlay Form: `https://docs.devexpress.com/content/WindowsForms/120029?md=true`
- `SplashScreenManager` class: `xref:DevExpress.XtraSplashScreen.SplashScreenManager`
- `ProgressPanel` class: `xref:DevExpress.XtraWaitForm.ProgressPanel`
