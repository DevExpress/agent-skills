# Appearance and Custom Header Buttons

Add custom buttons to the header panel, tune colors, and owner-draw headers.

## When to Use This Reference

- Adding custom action buttons to the tab header panel
- Customizing header / page colors
- Owner-drawing header content

## Custom Header Buttons

Beyond the built-in Prev/Next/Close set, you can add your own buttons to the header panel via the `CustomHeaderButtons` collection and handle clicks with `CustomHeaderButtonClick`.

```csharp
using DevExpress.XtraTab;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTab.ViewInfo;

CustomHeaderButton btnFavorites = new CustomHeaderButton();
btnFavorites.Caption = "Go to Favorite";
btnFavorites.ImageOptions.SvgImage = svgImageCollection[5];
btnFavorites.ImageOptions.SvgImageSize = new Size(16, 16);
btnFavorites.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
btnFavorites.Tag = "btnFavorite";   // unique button ID

xtraTabControl1.CustomHeaderButtons.Add(btnFavorites);
xtraTabControl1.CustomHeaderButtonClick += OnCustomHeaderButtonClick;

void OnCustomHeaderButtonClick(object sender, CustomHeaderButtonEventArgs e) {
    if ((string)e.Button.Tag == "btnFavorite") {
        // Process the click
    }
}
```

### VB.NET

```vb
Imports DevExpress.XtraTab
Imports DevExpress.XtraTab.Buttons
Imports DevExpress.XtraTab.ViewInfo

Dim btnFavorites As New CustomHeaderButton()
btnFavorites.Caption = "Go to Favorite"
btnFavorites.ImageOptions.SvgImage = svgImageCollection(5)
btnFavorites.ImageOptions.SvgImageSize = New Size(16, 16)
btnFavorites.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph
btnFavorites.Tag = "btnFavorite"
xtraTabControl1.CustomHeaderButtons.Add(btnFavorites)

AddHandler xtraTabControl1.CustomHeaderButtonClick, AddressOf OnCustomHeaderButtonClick

Private Sub OnCustomHeaderButtonClick(ByVal sender As Object, ByVal e As CustomHeaderButtonEventArgs)
    If CStr(e.Button.Tag) = "btnFavorite" Then
        ' Process the click
    End If
End Sub
```

`CustomHeaderButton` and `CustomHeaderButtonCollection` live in `DevExpress.XtraTab.Buttons`; `CustomHeaderButtonEventArgs` lives in `DevExpress.XtraTab.ViewInfo`.

## Appearance and Colors

Use `XtraTabControl.Appearance` (control-wide) and `XtraTabControl.AppearancePage` (page elements — header, page client) for colors, fonts, and borders. Per-page overrides are available through `XtraTabPage.Appearance`.

```csharp
xtraTabControl1.AppearancePage.Header.Font =
    new Font(xtraTabControl1.Font, FontStyle.Bold);
```

### DX Skin Colors

`XtraTabControl` supports DX Skin Colors — you can set a page header background with a skin color so it adapts across skins, e.g. via `XtraTabPage.Appearance.Header.BackColor`. See the DX Skin Colors documentation (MCP search `"DX Skin Colors"`) for assigning palette colors rather than fixed RGB values.

## Custom Drawing

`XtraTabControl` implements `IXtraTabCustomDraw` and exposes custom-draw events for owner-drawing header elements and buttons (e.g., painting tab header text vertically, drawing checkboxes in headers). The exact event names and their args vary; use the MCP tool to confirm:

```
devexpress_docs_search(technologies=["WindowsForms"], question="XtraTabControl custom draw header event")
```

Task-based examples from the docs:
- Display custom header buttons — `winforms-tab-control-custom-header-buttons`
- Paint tab header text vertically — `winforms-tabcontrol-paint-tab-header-text-vertically`
- Display checkboxes in tab headers — `winforms-tabcontrol-show-checkboxes-in-page-headers`

## Source Material

- [DevExpress.XtraTab.Buttons](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.Buttons?md=true) — `CustomHeaderButton`, `CustomHeaderButtonCollection`
- [DevExpress.XtraTab.ViewInfo](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.ViewInfo?md=true) — `CustomHeaderButtonEventArgs`
