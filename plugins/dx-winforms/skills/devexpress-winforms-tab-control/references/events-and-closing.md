# Events, Closing, and Custom Navigation

Respond to page changes, enable and handle Close buttons, and build wizard-style navigation with hidden headers.

## When to Use This Reference

- Running code when the active page changes (or preventing the change)
- Letting users close pages, and deciding what closing does
- Hiding headers and driving navigation with your own buttons

## Selection Events

| Event | Args | When |
|---|---|---|
| `SelectedPageChanging` | `TabPageChangingEventArgs` (`e.Page`, `e.PrevPage`, `e.Cancel`) | Before the active page changes — cancelable |
| `SelectedPageChanged` | `TabPageChangedEventArgs` (`e.Page`, `e.PrevPage`) | After the active page changed |
| `Selecting` / `Deselecting` | `TabPageCancelEventArgs` (`e.Page`, `e.Cancel`) | Before a page is entered / left — cancelable |
| `Selected` / `Deselected` | `TabPageEventArgs` (`e.Page`) | After a page is entered / left |

```csharp
// React after the change
xtraTabControl1.SelectedPageChanged += (s, e) => {
    Console.WriteLine($"Now on: {e.Page.Text} (was {e.PrevPage?.Text})");
};

// Veto a change (e.g., unsaved edits on the current page)
xtraTabControl1.SelectedPageChanging += (s, e) => {
    if (HasUnsavedChanges(e.PrevPage))
        e.Cancel = true;
};
```

## Close Buttons

There are two distinct Close affordances:

1. **Per-page Close buttons** — shown in/near individual page headers. Configured with `ClosePageButtonShowMode`.
2. **A single Close button in the header panel** — part of the `HeaderButtons` set (`TabButtons.Close`, see [headers-and-layout.md](headers-and-layout.md)).

> `HeaderButtonsShowMode` has higher priority than `ClosePageButtonShowMode`. If the header Close button is hidden via `HeaderButtonsShowMode`, the per-page setting cannot show it.

Enable per-page Close buttons:

```csharp
xtraTabControl1.ClosePageButtonShowMode =
    ClosePageButtonShowMode.InAllTabPageHeaders;
```

> `ClosePageButtonShowMode` enumerates where Close buttons appear (individual pages, the header, or both). Use the MCP tool to confirm the exact member name for your scenario (e.g., active-tab-only vs. all tabs vs. on hover).

### Handling a Close Click

Clicking a Close button does **not** remove the page automatically — handle `CloseButtonClick` and decide what to do. Cast the args to `ClosePageButtonEventArgs` (in `DevExpress.XtraTab.ViewInfo`):

```csharp
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;

private void xtraTabControl1_CloseButtonClick(object sender, EventArgs e) {
    ClosePageButtonEventArgs arg = e as ClosePageButtonEventArgs;
    var page = arg.Page as XtraTabPage;

    // Option A: hide (reversible)
    page.PageVisible = false;

    // Option B: remove permanently
    // xtraTabControl1.TabPages.Remove(page);
}
```

### VB.NET

```vb
Imports DevExpress.XtraTab
Imports DevExpress.XtraTab.ViewInfo

Private Sub XtraTabControl1_CloseButtonClick(ByVal sender As Object, _
    ByVal e As EventArgs) Handles XtraTabControl1.CloseButtonClick
    Dim arg As ClosePageButtonEventArgs = TryCast(e, ClosePageButtonEventArgs)
    TryCast(arg.Page, XtraTabPage).PageVisible = False
End Sub
```

## Wizard-Style Custom Navigation (Hidden Headers)

Hide the header panel and drive page changes with your own buttons — useful for wizards or stepper UIs:

```csharp
using DevExpress.Utils;   // DefaultBoolean
using DevExpress.XtraTab;

public Form1() {
    InitializeComponent();
    xtraTabControl1.ShowTabHeader = DefaultBoolean.False;   // hide headers
}

private void buttonPrev_Click(object sender, EventArgs e) {
    if (xtraTabControl1.SelectedTabPageIndex != 0)
        xtraTabControl1.SelectedTabPageIndex--;
}

private void buttonNext_Click(object sender, EventArgs e) {
    if (xtraTabControl1.SelectedTabPageIndex != xtraTabControl1.TabPages.Count - 1)
        xtraTabControl1.SelectedTabPageIndex++;
}

// Enable/disable the buttons based on position
private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
    buttonPrev.Enabled = xtraTabControl1.SelectedTabPageIndex != 0;
    buttonNext.Enabled = xtraTabControl1.SelectedTabPageIndex != xtraTabControl1.TabPages.Count - 1;
}
```

## Source Material

- [ClosePageButtonShowMode](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.ClosePageButtonShowMode?md=true) — Close-button display modes
- [TabPageChangingEventArgs](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.TabPageChangingEventArgs?md=true), [TabPageChangedEventArgs](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.TabPageChangedEventArgs?md=true), [TabPageCancelEventArgs](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.TabPageCancelEventArgs?md=true) — event args
