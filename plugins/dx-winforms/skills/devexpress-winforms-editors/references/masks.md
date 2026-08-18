# Input Masks

This reference covers the DevExpress mask system for `TextEdit` descendants — the eight mask types, when each one fits, the syntax of common patterns, the fluent `MaskSettings.Configure<>` API, the legacy `Mask.EditMask` / `MaskType` API, and the code-first data-annotation attributes (`[EditMask]`, `[NumericEditMask]`, `[RegExEditMask]`, …).

A mask restricts user input and (optionally) the displayed text format. Masks are evaluated when the editor is focused. When the editor loses focus, the value is exposed via `EditValue` in the editor's underlying type (`string`, `decimal`, `DateTime`, etc.).

## When to Use This Reference

- Applying a phone, ZIP code, SSN, IBAN, license-key, or other fixed-format mask.
- Forcing currency / percent / decimal display.
- Restricting a `TextEdit` to ISO 8601 date input.
- Validating alphanumeric IDs with a regex.
- Switching mask types at runtime based on a user choice.
- Applying masks at the data-source level via attributes.

## Editors That Support Masks

Masks are honored by all `TextEdit` descendants **except**:

- `LookUpEdit`
- `GridLookUpEdit`
- `TreeListLookUpEdit`
- `MemoEdit`
- `MemoExEdit`
- `ImageComboBoxEdit`
- `MRUEdit` and its descendants

For these editors, validate input via `Validating` / `EditValueChanged` instead.

## Mask Types

| `MaskType` | Use when input is | Syntax |
|---|---|---|
| **`Numeric`** | Number with culture-aware formatting (currency, percent, integer, decimal). | Standard .NET numeric format strings (`c`, `c2`, `n0`, `p1`, `f3`, `d`, custom `##0.##`). |
| **`DateTime`** | A date+time. | Standard .NET date-time format strings (`d`, `D`, `f`, `g`, `s`, `o`) or custom (`MMMM dd, yyyy`). |
| **`DateOnly`** | A `DateOnly` value (.NET 6+). | Same date specifiers as DateTime. |
| **`TimeOnly`** | A `TimeOnly` value (.NET 6+). | Same time specifiers as DateTime. |
| **`DateTimeOffset`** | A `DateTimeOffset`. | Same as DateTime + offset. |
| **`Simple`** | Fixed-length string with explicit positions (phone, ZIP, SSN). | Metacharacters (`L`, `0`, `a`, `9`, `#`, `>`, `<`). |
| **`RegEx`** (Extended Regular Expressions) | Variable-length string matching a pattern. | POSIX-like ERE with quantifiers, alternation, character classes. |
| **`TimeSpan`** | A duration. | `d.HH:mm:ss.fff`-style patterns. |
| **`Simplified RegEx`** (legacy) | Same shape as RegEx but older syntax. | Compatibility with XtraEditors v2.0. **Prefer `RegEx`.** |

## How to Apply a Mask

Use the fluent `MaskSettings.Configure<T>()` API:

```csharp
using DevExpress.XtraEditors.Mask;

textEdit1.Properties.MaskSettings.Configure<MaskSettings.Numeric>(s => {
    s.MaskExpression       = "c2";
    s.HideInsignificantZeros = false;
    s.AutoHideDecimalSeparator = false;
});

textEdit2.Properties.MaskSettings.Configure<MaskSettings.DateTime>(s => {
    s.MaskExpression = "MMMM dd, yyyy";
});

textEdit3.Properties.MaskSettings.Configure<MaskSettings.Simple>(s => {
    s.MaskExpression = "(000) 000-00-00";
});

textEdit4.Properties.MaskSettings.Configure<MaskSettings.RegExp>(s => {
    s.MaskExpression = @"\d+(\.\d{0,2})?";       // optional 2-decimal number
    s.ShowPlaceholders = true;
    s.Placeholder = '_';
    s.IsAutoComplete = true;
});
```

The generic argument types are nested types of the public `DevExpress.XtraEditors.Mask.MaskSettings` class (imported above via `using DevExpress.XtraEditors.Mask;`):
`MaskSettings.Numeric`, `MaskSettings.DateOnly`, `MaskSettings.TimeOnly`, `MaskSettings.DateTime`, `MaskSettings.DateTimeOffset`, `MaskSettings.TimeSpan`, `MaskSettings.Simple`, `MaskSettings.RegExp` (extended regex), `MaskSettings.Regular` (simplified regex).

### In code (legacy API — still supported)

```csharp
using DevExpress.XtraEditors.Mask;

textEdit1.Properties.Mask.MaskType   = MaskType.Numeric;
textEdit1.Properties.Mask.EditMask   = "c2";
textEdit1.Properties.Mask.UseMaskAsDisplayFormat = true;
```

The two APIs are synchronized — set either, the other reflects.

### At the data-source layer (code-first attributes)

```csharp
using DevExpress.XtraEditors.Mask;
using System.ComponentModel.DataAnnotations;

public class Employee {
    [RegExEditMask("[A-Z][a-z]+", AllowBlankInput = true, ShowPlaceholders = false)]
    public string FirstName { get; set; } = "";

    [EditMask("d")]                              // DateTime mask
    public DateTime HiredAt { get; set; }

    [NumericEditMask("c2", HideInsignificantZeros = false)]
    public decimal Salary { get; set; }

    [SimpleEditMask("(000) 000-00-00")]
    public string Phone { get; set; } = "";
}
```

Editors bound to these properties pick up the masks automatically. Attributes:

- `[EditMask(expression)]` — DateTime by default.
- `[NumericEditMask(expression, …)]`
- `[RegExEditMask(expression, …)]`
- `[SimpleEditMask(expression)]`
- `[DateOnlyEditMask(expression)]`
- `[TimeOnlyEditMask(expression)]`
- `[TimeSpanEditMask(expression)]`

## Numeric Mask Reference

| Specifier | Description | Example |
|---|---|---|
| `c` / `c2` | Currency with culture symbol. `2` = decimal places. | `$323.00` |
| `n` / `n0` | Number with thousand separators. | `1,234` |
| `d` / `d4` | Decimal integer; precision = min digits. | `0042` |
| `f` / `f2` | Fixed-point. | `12.34` |
| `p` / `p1` | Percent (× 100). | `12.3 %` |
| `e` / `e2` | Scientific. | `1.23E+002` |
| `x` / `X` | Hexadecimal. | `2A` |

Custom: combine `#` (optional), `0` (mandatory), `.`, `,` — e.g., `##0.##`, `#,##0.00 €`.

Patterns are **culture-aware** — `c` shows `$` in en-US, `€` in fr-FR. The `CultureInfo.NumberFormat` of the current thread drives the symbols. Override per-editor via `Properties.MaskSettings.Configure<MaskSettings.Numeric>(s => s.Culture = new CultureInfo("de-DE"))`.

## DateTime Mask Reference

Standard specifiers (culture-driven):

| Mask | Sample |
|---|---|
| `d` | `8/24/2026` |
| `D` | `Monday, August 24, 2026` |
| `t` | `7:51 PM` |
| `T` | `7:51:13 PM` |
| `g` | `8/24/2026 7:51 PM` |
| `G` | `8/24/2026 7:51:13 PM` |
| `f` / `F` | Long date + short / long time. |
| `R` / `r` | RFC1123 — `Sun, 16 Aug 2026 16:08:00 GMT`. |
| `s` | ISO 8601 sortable. |
| `o` / `O` | Round-trip ISO 8601. |
| `u` | Universal sortable. |
| `Y` / `y` | Month + year. |
| `M` / `m` | Month + day. |

Custom placeholders (case-sensitive):

| Placeholder | Meaning | Example |
|---|---|---|
| `d` / `dd` | Day. | `1` / `01` |
| `ddd` / `dddd` | Abbreviated / full day name. | `Mon` / `Monday` |
| `M` / `MM` / `MMM` / `MMMM` | Month number / 2-digit / abbr / full. | `1` / `01` / `Jan` / `January` |
| `y` / `yy` / `yyyy` | Year (1/2/4 digits). | `6` / `26` / `2026` |
| `h` / `hh` / `H` / `HH` | Hour 12/24h, 1/2 digit. | `7` / `07` / `19` |
| `m` / `mm` | Minute. | `5` / `05` |
| `s` / `ss` | Second. | `9` / `09` |
| `t` / `tt` | AM/PM marker. | `A` / `AM` |
| `:` `/` | Separators (replaced per culture). | — |

Calendars: Gregorian, Korean, Taiwanese, Thai Buddhist (selected via `MaskSettings.DateTime.CalendarType`).

## Simple Mask Reference

Metacharacters specify positions:

| Char | Meaning |
|---|---|
| `L` | Mandatory letter. |
| `l` | Optional letter. |
| `A` | Mandatory alphanumeric. |
| `a` | Optional alphanumeric. |
| `C` | Mandatory arbitrary char. |
| `c` | Optional arbitrary char. |
| `0` | Mandatory digit. |
| `9` | Optional digit. |
| `#` | Optional digit, `+`, or `-`. |

Special characters:

| Char | Meaning |
|---|---|
| `>` | Uppercase subsequent. |
| `<` | Lowercase subsequent. |
| `<>` | Restore case. |
| `/` / `:` | Date / time separator (culture). |
| `\X` | Literal `X`. |

Examples:

```csharp
"(000) 000-00-00"             // US phone
"00000-0000"                  // ZIP+4
"000-00-0000"                 // SSN
">LL-000000"                  // uppercase 2 letters + 6 digits
```

## RegEx (Extended Regular Expressions) Reference

Metacharacters:

| Token | Meaning |
|---|---|
| `.` | Any char. |
| `[abc]` | Any from set. |
| `[^abc]` | Any not in set. |
| `[a-z]` | Range. |
| `\w` `\W` | Alphanumeric / non-alphanumeric. |
| `\d` `\D` | Digit / non-digit. |
| `\s` `\S` | Whitespace / non-whitespace. |
| `\xNN` | ASCII char by hex code. |
| `\uNNNN` | Unicode char. |

Quantifiers: `*`, `+`, `?`, `{n}`, `{n,}`, `{n,m}`. Alternation: `|`. Grouping: `(…)`.

Examples:

```csharp
@"\d+(\.\d{0,2})?"             // decimal with optional 2 fractional digits
@"(1?[1-9])|([12][0-4])"       // 1–24
@"[A-Z]{2}\d{6}"               // 2 uppercase letters + 6 digits
@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}"   // email
```

Settings unique to RegEx (`MaskSettings.RegExp`):

| Property | Meaning |
|---|---|
| `MaskExpression` | The pattern. |
| `ShowPlaceholders` | Show underscores (or `Placeholder`) for empty metacharacters. |
| `Placeholder` | Character used for placeholders. Default `_`. |
| `IsAutoComplete` | Auto-fill literal characters as the user types. |
| `AllowBlankInput` | Allow the editor to be empty even if the pattern requires content. |

## TimeSpan Mask Reference

```csharp
textEdit1.Properties.MaskSettings.Configure<MaskSettings.TimeSpan>(s => {
    s.MaskExpression = @"d\.HH\:mm\:ss";
});
```

Use `d`, `h`, `m`, `s`, `f` placeholders (one or two characters each). The `MaskType.TimeSpanAdvancingCaret` variant auto-advances the caret across segments.

## `UseMaskAsDisplayFormat`

By default the mask only restricts editing; the displayed value when the editor is unfocused uses `DisplayFormat`. To apply the mask in display mode too:

```csharp
textEdit1.Properties.Mask.UseMaskAsDisplayFormat = true;
```

`UseMaskAsDisplayFormat` lives on `Properties.Mask`, not on the fluent `MaskSettings.*` types — configure the mask fluently, then set the flag:

```csharp
textEdit1.Properties.MaskSettings.Configure<MaskSettings.Numeric>(s => s.MaskExpression = "c2");
textEdit1.Properties.Mask.UseMaskAsDisplayFormat = true;
```

This is the usual choice for currency / date masks where the unfocused display should match the input format.

## Changing Mask Type at Runtime

```csharp
void SetMaskFor(BaseEdit editor, string kind) {
    // Configure<T> is generic — the settings type must be supplied per branch,
    // so use a switch statement (a switch expression can't pick the type argument).
    switch (kind) {
        case "money":
            editor.Properties.MaskSettings.Configure<MaskSettings.Numeric>(s => s.MaskExpression = "c2");
            break;
        case "date":
            editor.Properties.MaskSettings.Configure<MaskSettings.DateTime>(s => s.MaskExpression = "d");
            break;
        case "phone":
            editor.Properties.MaskSettings.Configure<MaskSettings.Simple>(s => s.MaskExpression = "(000) 000-00-00");
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(kind));
    }
}
```

Call the appropriate `Configure<T>` overload per mask category — the explicit type argument selects the settings type.

## Common Issues

- **Mask ignored on a `LookUpEdit`**: lookup editors do not support masks. Replace with a `ButtonEdit`+`PopupContainerEdit` or validate via `Validating`.
- **Currency symbol does not match the user's locale**: thread culture is not the user's. Set `s.Culture = CultureInfo.GetCultureInfo("…")` explicitly or rely on `CultureInfo.CurrentCulture`.
- **`AllowBlankInput` ignored with RegEx**: the pattern must allow an empty match (e.g., `(\w+)?`). Combine with `s.AllowBlankInput = true`.
- **`UseMaskAsDisplayFormat` shows zeros for empty numeric**: set `HideInsignificantZeros = true` or `AutoHideDecimalSeparator = true`.
- **`Simple` mask blocks paste**: by design — pasted text is validated against the pattern. Strip non-mask characters before pasting.
- **`EditValue` is `string` for `Numeric` mask**: this happens with `TextEdit` + Numeric mask — the result is the typed string. Use `SpinEdit` or `CalcEdit` for a typed numeric value.

## Source Material

- `articles/controls-and-libraries/editors-and-simple-controls/common-editor-features-and-concepts/input-mask.md` (`xref:WindowsForms.583`).
- `articles/controls-and-libraries/editors-and-simple-controls/common-editor-features-and-concepts/masks/mask-type-numeric.md` (`xref:WindowsForms.1498`).
- `articles/controls-and-libraries/editors-and-simple-controls/common-editor-features-and-concepts/masks/mask-type-date-time.md` (`xref:WindowsForms.1497`).
- `articles/controls-and-libraries/editors-and-simple-controls/common-editor-features-and-concepts/masks/mask-type-date-only.md` (`xref:WindowsForms.404881`).
- `articles/controls-and-libraries/editors-and-simple-controls/common-editor-features-and-concepts/masks/mask-type-time-only.md` (`xref:WindowsForms.404882`).
- `articles/controls-and-libraries/editors-and-simple-controls/common-editor-features-and-concepts/masks/mask-type-simple.md` (`xref:WindowsForms.1500`).
- `articles/controls-and-libraries/editors-and-simple-controls/common-editor-features-and-concepts/masks/mask-type-extended-regular-expressions.md` (`xref:WindowsForms.1501`).
- `articles/controls-and-libraries/editors-and-simple-controls/common-editor-features-and-concepts/masks/mask-type-simplified-regular-expressions.md` (`xref:WindowsForms.1499`).
- `articles/controls-and-libraries/editors-and-simple-controls/common-editor-features-and-concepts/masks/mask-type-timespan.md` (`xref:WindowsForms.401199`).
- `api/DevExpress.XtraEditors.Mask.MaskProperties.yml`.
- `api/DevExpress.XtraEditors.Repository.RepositoryItemTextEdit.MaskSettings.yml`.
