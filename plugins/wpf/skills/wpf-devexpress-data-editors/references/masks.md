# Masks — DevExpress WPF Data Editors

Masks restrict and format input on `TextEdit` and its descendants. The same mask machinery covers everything from "phone number with literals" to "currency value with culture-aware decimal separator" to "any regex with autocomplete."

> **Bind `EditValue`, not `Text`.** With `MaskType.Numeric` / `DateTime`, mask handling depends on the value being the correct type. `Text` is always `string` and the mask may apply incorrectly.

## When to Use This Reference

Use this when you need to:

- Restrict input to numeric, currency, or percentage values
- Enforce a date / time / timespan format with caret navigation between sections
- Accept fixed-format strings (phone, ZIP, SSN, license plate) via a literal mask
- Accept a free-form pattern using regular expressions, optionally with autocomplete
- Implement a custom mask via the `CustomMask` event
- Save mask literals into the bound value (or strip them)
- Use the mask as a display format too (so the value looks the same in display and edit modes)

## Editors That Support Masks

Most `TextEdit` descendants support masks. **Exceptions** (these do NOT support masked input):

- `ComboBoxEdit`
- `LookUpEdit`
- `AutoSuggestEdit`
- `MemoEdit`
- `PopupColorEdit`
- `PopupImageEdit`

If you need a masked combobox-style experience, use a `TextEdit` with `MaskType="RegEx"` + autocomplete instead.

## The Six Mask Types

Set `TextEdit.MaskType` (or `TextEditSettings.MaskType` for in-place editors), then `TextEdit.Mask` to the pattern. The two work together — pick a type first, then the pattern syntax follows.

| `MaskType` | Use for | Pattern syntax |
|---|---|---|
| `Numeric` | Integers, decimals, currency, percent | .NET numeric format strings (`c`, `n2`, `p1`, `##0.00`, etc.) |
| `DateTime` / `DateTimeAdvancingCaret` | Date and time values | .NET date format strings (`d`, `g`, `MM/dd/yyyy`, etc.) |
| `DateOnly` | `System.DateOnly` values | .NET date format strings |
| `TimeOnly` | `System.TimeOnly` values | .NET time format strings |
| `DateTimeOffset` | `System.DateTimeOffset` (with timezone offset) | .NET date format + offset |
| `TimeSpan` | `System.TimeSpan` (duration) | .NET timespan format strings |
| `Simple` | Fixed-format short strings (phone, ZIP, license) | Metacharacters (`0`, `9`, `L`, `A`, `#`, etc.) + literals |
| `Regular` / `RegEx` | Free-form patterns | POSIX-style Extended Regular Expressions |
| `Custom` | Anything else | `CustomMask` event handler |

### Numeric Mask

The mask is a .NET numeric format string. Predefined single-character masks: `c` (currency), `n` (number with thousand separators), `p` (percent), `d` (decimal integer), `f` (fixed-point), `e` (scientific), with an optional precision digit count.

```xaml
<!-- Currency, two decimals, culture-aware ($1,234.56 in en-US) -->
<dxe:TextEdit EditValue="{Binding Price}"
              MaskType="Numeric" Mask="c2"
              MaskUseAsDisplayFormat="True"/>

<!-- Integer percent (5%, 25%) -->
<dxe:TextEdit EditValue="{Binding Discount}"
              MaskType="Numeric" Mask="p0"/>

<!-- Custom: thousands grouped, two decimals -->
<dxe:TextEdit EditValue="{Binding Amount}"
              MaskType="Numeric" Mask="#,##0.00"/>
```

Bind a numeric type (`decimal`, `double`, `int`) for the mask to apply correctly. Set `NumericMaskOptions.AlwaysShowDecimalSeparator="False"` to hide a trailing `.00` when the value is integral.

### Date-Time Mask

Predefined: `d` (short date), `D` (long date), `t` (short time), `T` (long time), `g` (general short), `G` (general long), `M` (month/day), `Y` (year/month). Custom: full .NET date/time format strings.

```xaml
<!-- Short date for the current culture -->
<dxe:TextEdit EditValue="{Binding Birthday}"
              MaskType="DateTime" Mask="d"/>

<!-- Year-month-day with auto-advance after each section -->
<dxe:TextEdit EditValue="{Binding ReleaseDate}"
              MaskType="DateTimeAdvancingCaret" Mask="yyyy-MM-dd"/>
```

`DateEdit` defaults to `MaskType.DateTime` internally — you can switch it to `DateTimeAdvancingCaret` to auto-advance the caret across sections.

End users can navigate sections with arrow keys and increment/decrement them with the mouse wheel or up/down arrows.

### Simple Mask — Fixed-Format Strings

Metacharacters drive the mask; literal characters appear as-is:

| Char | Meaning |
|---|---|
| `0` | Required digit |
| `9` | Optional digit |
| `#` | Optional digit, plus, or minus sign |
| `L` | Required letter |
| `l` | Optional letter |
| `A` | Required alphanumeric |
| `a` | Optional alphanumeric |
| `C` | Required arbitrary character |
| `c` | Optional arbitrary character |

Special characters: `>` uppercase the rest, `<` lowercase the rest, `\` escapes a metacharacter, `/` `:` `$` use culture-aware substitutions.

```xaml
<!-- US phone number, all digits required -->
<dxe:TextEdit EditValue="{Binding Phone}"
              MaskType="Simple"
              Mask="(000)000-00-00"/>

<!-- Phone with optional area code -->
<dxe:TextEdit Mask="(999)000-00-00" MaskType="Simple"/>

<!-- License plate: "A" literal, two uppercase letters, dash, two digits -->
<dxe:TextEdit Mask="\A>LL-00" MaskType="Simple"/>

<!-- ZIP+4 -->
<dxe:TextEdit Mask="00000-9999" MaskType="Simple"/>
```

> Use `MaskSaveLiteral="True"` to keep parentheses, dashes, etc. in `EditValue`. With the default `False`, `EditValue` contains only the entered characters (e.g., `"5551234567"` not `"(555)123-45-67"`).

### RegEx Mask — Patterns with Autocomplete

Set `MaskType="RegEx"` and a regular expression as the mask. Supports:

- Variable-length input
- Multiple alternative forms
- Per-position character classes
- **Autocomplete** — the editor offers to complete partial input

```xaml
<!-- Email-ish pattern -->
<dxe:TextEdit MaskType="RegEx" Mask="\w+@\w+\.\w+"/>

<!-- Product code: 3 letters, dash, 4 digits -->
<dxe:TextEdit MaskType="RegEx" Mask="[A-Z]{3}-\d{4}"/>

<!-- One of three accepted forms -->
<dxe:TextEdit MaskType="RegEx" Mask="(yes|no|maybe)"/>

<!-- Month names with autocomplete -->
<dxe:TextEdit MaskType="RegEx" Mask="\R{MonthNames}"
              MaskAutoComplete="Strong"/>
```

#### Autocomplete Modes

| Mode | Behavior |
|---|---|
| `None` | No autocomplete |
| `Strong` | Each keystroke fills only positions that have a single possible character. E.g., typing `M` in a month-name mask fills `Ma` (both March and May start with `Ma`); typing `r` then completes `March`. |
| `Optimistic` | First keystroke fills *all* remaining placeholders with default values (`0` for digits, `a` for letters). E.g., typing `1` in `\d{3}-\d{2}-\d{2}` fills as `100-00-00`. |
| `Default` | Same as `Strong`. |

#### Placeholders

The default placeholder character `_` is configurable via `MaskPlaceHolder`. Hide placeholders with `MaskShowPlaceHolders="False"` (RegEx mode only).

### Custom Mask — `CustomMask` Event

For masks no built-in type can express:

```xaml
<dxe:TextEdit MaskType="Custom" CustomMask="OnCustomMask"/>
```

```csharp
private void OnCustomMask(object sender, CustomMaskEventArgs e) {
    // Inspect e.Action, e.Position, etc. and decide whether to accept / reject / transform input.
}
```

Use this only when the standard mask types genuinely don't fit — most "custom" needs are actually expressible as a `RegEx` mask.

## Picking the Right Mask Type

| Scenario | Mask type | Why |
|---|---|---|
| Currency input | `Numeric` `c2` | Culture-aware, decimal-aware, single character mask |
| Percent (0–100%) | `Numeric` `p0` or `p2` | Auto-divides display by 100 |
| Integer with thousand separators | `Numeric` `n0` | Thousand separators per current culture |
| Fixed-length date in current culture | `DateTime` `d` | Predefined, culture-aware |
| Custom date layout (yyyy-MM-dd) | `DateTime` `yyyy-MM-dd` | .NET format string |
| Phone with literal parens/dashes | `Simple` | Metacharacters + literals |
| ZIP / SSN / license plate / SKU | `Simple` | Metacharacters keep it simple |
| Email / variable-length identifier | `RegEx` | Free-form pattern + autocomplete |
| Multiple alternative inputs (yes/no/maybe) | `RegEx` | Regex alternation |
| Genuinely dynamic mask (e.g., changes by row) | `Custom` | `CustomMask` event |

## Cross-Cutting Properties

| Property | Effect |
|---|---|
| `MaskUseAsDisplayFormat` | Use the mask for display, not only edit. Default `false` — display uses `DisplayFormatString` instead. |
| `MaskSaveLiteral` | Include literal characters in `EditValue` (Simple / Regular only). |
| `MaskShowPlaceHolders` | Show `_` placeholders for unfilled positions (RegEx only). |
| `MaskPlaceHolder` | Override the placeholder character (default `_`). |
| `MaskAutoComplete` | RegEx autocomplete mode (`None` / `Strong` / `Optimistic` / `Default`). |
| `MaskBeepOnError` | Play a beep when the user types a rejected character. |
| `MaskCulture` | Override the culture used by `Numeric` / `DateTime` masks. |
| `MaskIgnoreBlank` | When `true`, the editor can lose focus while empty. (Simple / Regular / RegEx only.) |

## Validation and Mask Interaction

- **`InvalidValueBehavior`** governs what happens when the value is partial or invalid:
  - `WaitForValidValue` (default) — focus stays in the editor until the value matches the mask
  - `AllowLeaveEditor` — focus can leave with a partial value
- **`EditValue` only updates after validation succeeds**. While the user types an invalid value, `EditValue` keeps the previous valid value.
- **Hook `Validate` event** for cross-field or domain-specific checks on top of the mask.

```xaml
<dxe:TextEdit EditValue="{Binding Phone}"
              MaskType="Simple"
              Mask="(000)000-00-00"
              InvalidValueBehavior="AllowLeaveEditor"/>
```

## IME Note

IME (Input Method Editors for CJK languages) is **disabled** in all masked editors. If you need IME, omit the mask.

## Null Input in Masked Editors

Set `AllowNullInput="True"` to let users clear the value:

- Press `Ctrl-D` or `Ctrl-0` to clear
- Or select all and press `Delete`

Without `AllowNullInput`, masked editors enforce that the field is always filled.

## Design-Time Mask Editor

Visual Studio includes a **Mask Editor** wizard accessible from the editor's smart tag at design time. It scaffolds masks for common patterns (phone, currency, regex variants).

## Common Issues

- **Mask is ignored** — set `Mask` but forgot `MaskType`. Both are required.
- **Value comes back as string from a `Numeric` mask** — bound to `Text` instead of `EditValue`, or `EditValue` is bound to a `string` property. Bind a numeric type.
- **`EditValue` includes parentheses/dashes** — `MaskSaveLiteral="True"`. Set to `False` (default) to strip literals.
- **End user can't tab out of an empty masked field** — `MaskIgnoreBlank` is `false` and `InvalidValueBehavior` is `WaitForValidValue`. Set one or both to allow leaving.
- **Mask `c` shows wrong currency symbol** — depends on current culture. Override with `MaskCulture` or system regional settings.
- **RegEx autocomplete doesn't trigger** — `MaskAutoComplete` defaults to `None` for explicit safety. Set to `Strong` or `Optimistic`.
- **Combobox doesn't accept masked input** — `ComboBoxEdit`, `LookUpEdit`, etc. don't support masks. Use a `TextEdit` with `RegEx` mode if you need a typed selector.

## Source Material

- `articles/controls-and-libraries/data-editors/common-features/masked-input.md` (https://docs.devexpress.com/content/WPF/6945?md=true)
- `articles/controls-and-libraries/data-editors/common-features/masked-input/mask-type-numeric.md` (https://docs.devexpress.com/content/WPF/6950?md=true)
- `articles/controls-and-libraries/data-editors/common-features/masked-input/mask-type-date-time.md` (https://docs.devexpress.com/content/WPF/6947?md=true)
- `articles/controls-and-libraries/data-editors/common-features/masked-input/mask-type-simple.md` (https://docs.devexpress.com/content/WPF/6951?md=true)
- `articles/controls-and-libraries/data-editors/common-features/masked-input/mask-type-extended-regular-expressions.md` (https://docs.devexpress.com/content/WPF/6949?md=true)
- `articles/controls-and-libraries/data-editors/common-features/masked-input/mask-type-custom.md` (https://docs.devexpress.com/content/WPF/404108?md=true)
- `articles/controls-and-libraries/data-editors/common-features/masked-input/mask-type-date-only.md` (https://docs.devexpress.com/content/WPF/404171?md=true)
- `articles/controls-and-libraries/data-editors/common-features/masked-input/mask-type-time-only.md` (https://docs.devexpress.com/content/WPF/404172?md=true)
- `articles/controls-and-libraries/data-editors/common-features/masked-input/mask-type-date-time-offset.md` (https://docs.devexpress.com/content/WPF/404169?md=true)
- `articles/controls-and-libraries/data-editors/common-features/masked-input/mask-type-timespan.md` (https://docs.devexpress.com/content/WPF/400699?md=true)
