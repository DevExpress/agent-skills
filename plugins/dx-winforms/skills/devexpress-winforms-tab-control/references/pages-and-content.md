# Pages and Content

How to add, remove, reorder, and populate `XtraTabPage` pages, and how to read/set the active page.

## When to Use This Reference

- Adding / removing / reordering pages at runtime or in the designer file
- Placing controls onto a page
- Getting or setting the active page
- Showing or hiding an individual page

## The TabPages Collection

Pages are stored in `XtraTabControl.TabPages` (an `XtraTabPageCollection`).

```csharp
// Add a new page
XtraTabPage newPage = new XtraTabPage();
newPage.Text = "New Page";
xtraTabControl1.TabPages.Add(newPage);

// Add several at once
xtraTabControl1.TabPages.AddRange(new[] { pageA, pageB });

// Add by caption (the collection creates the page)
xtraTabControl1.TabPages.Add("Quick Page");

// Insert at a position
xtraTabControl1.TabPages.Insert(0, new XtraTabPage { Text = "First" });

// Remove
xtraTabControl1.TabPages.Remove(newPage);
xtraTabControl1.TabPages.RemoveAt(1);   // remove the 2nd page
```

### VB.NET

```vb
Dim newPage As New XtraTabPage()
newPage.Text = "New Page"
xtraTabControl1.TabPages.Add(newPage)

xtraTabControl1.TabPages.RemoveAt(1)
```

### Reordering Pages

```csharp
xtraTabControl1.TabPages.Move(0, somePage);   // move somePage to index 0
```

## Populating a Page with Controls

Add controls to the page's own `Controls` collection (not the form's, not the tab control's):

```csharp
newPage.Controls.Add(new SimpleButton {
    Text = "Button #1",
    Size = new Size(200, 50),
    Location = new Point(10, 10)
});

newPage.Controls.Add(new SimpleButton {
    Text = "Button #2",
    Size = new Size(200, 50),
    Location = new Point(10, 50)
});
```

For a structured layout inside a page, add a layout manager to the page first — e.g. `DevExpress.Utils.Layout.TablePanel` for a virtual grid, or a `LayoutControl` — and add controls into it.

## The Active Page

```csharp
// Get / set by object
XtraTabPage current = xtraTabControl1.SelectedTabPage;
xtraTabControl1.SelectedTabPage = pageB;

// Get / set by index
int idx = xtraTabControl1.SelectedTabPageIndex;
xtraTabControl1.SelectedTabPageIndex = 0;
```

## Showing / Hiding a Page

Use `XtraTabPage.PageVisible` to hide a page (and its header) without removing it from the collection — handy when "closing" a page should be reversible:

```csharp
pageAdvanced.PageVisible = false;   // hidden but still in TabPages
pageAdvanced.PageVisible = true;    // shown again
```

`XtraTabPage.PageEnabled` enables/disables a page while keeping it visible.

## Source Material

- `articles/controls-and-libraries/form-layout-managers/tab-control.md` — "How to Add and Remove Pages", "How to Populate Pages with Controls"
- `examples/xtratabcontrol_constructor728.md` — create a control with pages in code
- `examples/xtratabcontrol.tabpages.move1203.md` — reorder pages
- [XtraTabPageCollection](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.XtraTabPageCollection?md=true) — collection reference
