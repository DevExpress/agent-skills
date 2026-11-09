# DFM Format Rules — DevExpress VCL

The `.dfm` text format, the `.dfm` ↔ source contract, and form-load errors. This file is identical in every
DevExpress VCL skill; the verified form for this skill's components is in [design-time-dfm.md](design-time-dfm.md).

## When You Need This

- Write or edit any block in a form's `.dfm` (value syntax, strings, collections, inherited forms)
- Add the matching fields, handlers, `uses`/`#include`/`#pragma link` lines to the form unit
- Find a verified block for a class this skill does not show
- Diagnose a form-load error (`Property … does not exist`, `Class … not found`, `RLINK32`)

## DFM Text Format

A text `.dfm` is a tree of `object <Name>: <Class> … end` blocks. The root block is the form
(`object Form1: TForm1`: the form's `Name`, then its class). A block nested inside a control's block is that
control's child (`Parent`); non-visual components (`TDataSource`, style repositories, managers) sit directly
under the root. Inside a block, property lines come first, then nested `object` blocks. DFM text has no
comment syntax.

| Value | Syntax | Example |
|---|---|---|
| String | single quotes; `''` is one quote | `Caption = 'Customer''s orders'` |
| Non-ASCII text | `#<code>` outside quotes, glued to quoted parts | `Caption = #1047#1072#1082#1072#1079' '#8470' 1'` |
| Long string | `+` at the end of the line, continue on the next line | `Caption = 'first part '+` ⏎ `'second part'` |
| Integer / float | plain number | `Width = 120` |
| Boolean | `True` / `False` | `ParentFont = False` |
| Enum | identifier, no unit prefix | `Align = alClient` |
| Set | brackets; empty set `[]` | `Font.Style = [fsBold, fsItalic]` |
| Nested property | dotted path | `Font.Name = 'Segoe UI'`, `OptionsView.ColumnAutoWidth = True` |
| Component reference | target's `Name`; on another form/data module `Module.Name` | `DataSource = DataSource1`, `DataSource = dmMain.dsOrders` |
| Event | name of a published method of the form class | `OnCreate = FormCreate` |
| `TStrings` | `.Strings = ( … )`, one quoted line each | see below |
| Collection | `< item … end … >`; empty collection `<>` | see below |
| Binary data (images, glyphs, saved layouts) | `{ hex }` | never hand-write — assign in code |

A form header, a string list, and a collection (verified to load):

```dfm
object Form1: TForm1
  Left = 0
  Top = 0
  Caption = 'Form1'
  ClientHeight = 300
  ClientWidth = 500
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = 'Segoe UI'
  Font.Style = []
  OnCreate = FormCreate
  TextHeight = 15
  object ComboBox1: TComboBox
    Left = 8
    Top = 8
    Width = 200
    Height = 23
    Style = csDropDownList
    TabOrder = 0
    Items.Strings = (
      'One'
      'Two')
  end
  object StatusBar1: TStatusBar
    Left = 0
    Top = 281
    Width = 500
    Height = 19
    Panels = <
      item
        Width = 100
      end
      item
        Text = 'Ready'
        Width = 50
      end>
  end
end
```

Rules:

- Every `object` needs a `Name` that is unique on the form and a valid identifier.
- Write **published** properties only. Omitting a property means "default value" — always safe.
- A `.dfm` that starts with the bytes `FF 0A 00` (`TPF0`) is **binary**: never edit it as text. Convert it
  first: `"$(BDS)\bin\convert.exe" -t -i Unit1.dfm` (`-b` converts back).
- `inherited <Name>: <Class>` blocks appear in a form inherited from another form; they list only the
  properties changed relative to the ancestor. `[n]` after a class (`object X: TFoo [2]`) is the creation
  order inside an inherited form — keep it when editing such a block.
- Keep the split the IDE uses: the `.dfm` holds design-time configuration (final property values); code
  holds run-time work — loading data, calls (`ApplyBestFit`, `BeginUpdate`), and public-only properties.

## The `.dfm` ↔ Source Contract

The Form Designer edits three things at once when you drop a component; a hand edit must do the same.

**Delphi (`Unit1.pas`)**

- In the form class, before `private`, declare one field per `object` in the `.dfm`, with the same name
  and class: `cxGrid1: TcxGrid;`. The stream loader resolves a class name in the `.dfm` through these
  field types; a class without a field loads only if it is registered (`RegisterClass`), otherwise
  "Class TXxx not found".
- For every `OnXxx = HandlerName` line, declare the method in the same section with the exact event
  signature (`procedure FormCreate(Sender: TObject);`) and implement it.
- Add each unit that declares a class used in the `.dfm` to `uses` (the IDE does this on drop; a hand edit
  does not). Enum identifiers in the `.dfm` need no unit.
- Keep `{$R *.dfm}` after `implementation`.

**C++Builder (`Unit1.h` / `Unit1.cpp`)**

- `Unit1.h`, in `__published:` (the IDE-managed section): `TcxGrid *cxGrid1;` per component and
  `void __fastcall FormCreate(TObject *Sender);` per handler; `#include "<unit>.hpp"` per DevExpress unit.
- `Unit1.cpp`: `#pragma link "<unit>"` per DevExpress unit and `#pragma resource "*.dfm"`. For a dotted unit name
  (`dxBackend.ConnectionString.SQL`), link the object file explicitly, or the linker reads the last segment as an
  extension and fails with "Unable to open file":
  `#if defined(_WIN64)` `#pragma link "<unit>.o"` `#else` `#pragma link "<unit>.obj"` `#endif`.

## Finding the Exact Block for a Class

Copy a property line only from a block of the **same class**; never from a neighbor block that "looks
similar". Sources, in order:

1. [design-time-dfm.md](design-time-dfm.md) and the `dfm` blocks in this skill's other references (verified
   to load).
2. **Installed DevExpress demos** — `.dfm` files written by the IDE. Default folder:
   `%PUBLIC%\Documents\DevExpress VCL Demos` (search `*.dfm` for `: TClassName`).
3. The DevExpress Documentation MCP — whether a property is **published** (only published ones can appear).
4. The `published` section of the class in `$(DXVCL)\<Suite>\Sources\*.pas`.

## Verify

The compiler checks only the `.dfm` **syntax**; property names, values, and classes are checked only when the
form is created. After every `.dfm` change, build the project, then run
`powershell -ExecutionPolicy Bypass -File <this skill>/scripts/check-form-load.ps1 -Exe <built exe>` —
fix and rerun until it prints `PASS`.

## Troubleshooting Form Loading

| Error | Cause | Fix |
|---|---|---|
| Build fails: `E2161 Error: RLINK32: Error opening file "<path>\Unit1.dfm"` | `.dfm` syntax error: unterminated string, property line after a nested `object` block, unbalanced `end` | Fix the syntax — the file is not missing |
| `Error reading X.Y: Property Y does not exist` | Property misspelled, not published, or belongs to another class | Check the class's published properties (MCP/demos); move public-only properties to code |
| `Class TXxx not found` | Misspelled class, or no field of that class and the class is not registered | Fix the name; declare the field; add the unit to `uses`/`#include` |
| `Error reading Form1.OnXxx: Invalid property value` | Handler named in the `.dfm` is missing from the form class (or not in the published section) | Declare and implement the method |
| `Error reading X.Y: Invalid property value` | Wrong value type or unknown enum identifier | Use the exact type/identifier (`alClient`, not `'alClient'`) |
| `A component named X already exists` | Duplicate `Name` | Rename one block (and its field) |
| Handler compiles but never runs | `OnXxx = HandlerName` line missing | Add the line to the component's block |
| Control drawn in the wrong place / on the form instead of its container | Block nested under the wrong parent | Move the `object` block inside its container's block |
