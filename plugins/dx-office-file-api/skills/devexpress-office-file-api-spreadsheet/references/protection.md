# Protection & Security — DevExpress Spreadsheet Document API

Protect workbooks and worksheets from unauthorized editing, encrypt files with passwords, and define per-range permissions.

## When to Use This Reference

Use this when you need to:
- Encrypt a workbook file so it requires a password to open
- Protect the workbook structure to prevent adding/deleting/renaming sheets
- Protect a worksheet to prevent cell edits
- Lock or unlock specific cells within a protected worksheet
- Define password-protected ranges accessible to specific users
- Set a write-protection password or read-only recommendation

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `EncryptionSettings` | Password and algorithm settings for file encryption |
| `EncryptionType` | Enum: `Compatible` (XOR for .xls), `Strong` (AES for .xlsx) |
| `WorkbookImportOptions.Password` | Password for opening an encrypted file |
| `Workbook.Protect(password, lockStructure, lockWindows)` | Protect the workbook structure |
| `Workbook.Unprotect(password)` | Remove workbook structure protection |
| `WriteProtectionOptions` | Read-only recommendation and write-protection password |
| `Worksheet.Protect(password, permissions)` | Protect all locked cells in the worksheet |
| `Worksheet.Unprotect(password)` | Remove worksheet protection |
| `WorksheetProtectionPermissions` | Enum of allowed actions on a protected worksheet |
| `Protection.Locked` | Cell-level lock flag (default: `true`) |
| `ProtectedRangeCollection` | Named unlocked ranges accessible by password or user |
| `ProtectedRange` | A range unlockable with a specific password |

## File Encryption (Password to Open)

### Encrypt a File on Save

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("report.xlsx");

    // Create encryption settings
    EncryptionSettings encryption = new EncryptionSettings();
    encryption.Type = EncryptionType.Strong; // AES (for .xlsx)
    encryption.Password = "s3cr3t!";

    // Save with encryption
    workbook.SaveDocument("report_protected.xlsx", DocumentFormat.Xlsx, encryption);
}
```

### Open an Encrypted File

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    // Handle the password request event
    workbook.EncryptedFilePasswordRequest += (sender, e) =>
    {
        e.Password = "s3cr3t!";
    };

    // Handle failed password check (wrong password)
    workbook.EncryptedFilePasswordCheckFailed += (sender, e) =>
    {
        Console.WriteLine($"Password error: {e.Error}");
        e.TryAgain = false; // Do not retry
    };

    workbook.LoadDocument("report_protected.xlsx");
}
```

> Alternatively, set `workbook.Options.Import.Password = "s3cr3t!"` before calling `LoadDocument`.

## Workbook Structure Protection

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("workbook.xlsx");

    // Protect workbook structure (prevents adding/deleting/renaming/hiding sheets)
    workbook.Protect("password123", lockStructure: true, lockWindows: false);

    workbook.SaveDocument("workbook_protected.xlsx");
}

// Later: unprotect
using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("workbook_protected.xlsx");
    workbook.Unprotect("password123");
    workbook.SaveDocument("workbook_unprotected.xlsx");
}
```

## Write-Protection (Modification Password)

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    // Recommend read-only mode (no password required, just a suggestion)
    workbook.DocumentSettings.WriteProtection.ReadOnlyRecommended = true;

    // Require a password to modify the file
    workbook.DocumentSettings.WriteProtection.SetPassword("editpass");

    workbook.SaveDocument("readonly.xlsx");
}

// To remove write protection:
// workbook.DocumentSettings.WriteProtection.ClearPassword();
```

## Worksheet Protection

### Protect All Locked Cells

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// By default, all cells are locked (Protection.Locked = true)
// Unlock cells that users should be able to edit
CellRange editableRange = sheet.Range["B2:D20"];
Formatting fmt = editableRange.BeginUpdateFormatting();
fmt.Protection.Locked = false; // Allow editing these cells
editableRange.EndUpdateFormatting(fmt);

// Protect the sheet — locked cells become read-only
// Default permissions: users can only select cells
sheet.Protect("sheetPass", WorksheetProtectionPermissions.Default);
```

### Protect with Specific Permissions

```csharp
// Allow users to select cells AND format cells, but not insert rows
sheet.Protect("sheetPass",
    WorksheetProtectionPermissions.SelectLockedCells |
    WorksheetProtectionPermissions.SelectUnlockedCells |
    WorksheetProtectionPermissions.FormatCells);
```

### Common Permission Flags

| Flag | Description |
|------|-------------|
| `Default` | Only allow selecting cells |
| `SelectLockedCells` | Allow selecting locked cells |
| `SelectUnlockedCells` | Allow selecting unlocked cells |
| `FormatCells` | Allow formatting cells |
| `FormatColumns` | Allow formatting columns |
| `FormatRows` | Allow formatting rows |
| `InsertColumns` | Allow inserting columns |
| `InsertRows` | Allow inserting rows |
| `DeleteColumns` | Allow deleting columns |
| `DeleteRows` | Allow deleting rows |
| `Sort` | Allow sorting data |
| `UseAutoFilter` | Allow using AutoFilter |
| `UsePivotTableReports` | Allow using pivot tables |

### Unprotect a Worksheet

```csharp
sheet.Unprotect("sheetPass");
```

## Password-Protected Ranges

Unlock specific ranges for specific users or with a password, while keeping the rest of the sheet protected:

```csharp
Worksheet sheet = workbook.Worksheets[0];

// Define a protected range (accessible with a password)
ProtectedRange range1 = sheet.ProtectedRanges.Add("SalesData", sheet["B2:D50"]);
range1.SetPassword("salesteam");

ProtectedRange range2 = sheet.ProtectedRanges.Add("FinanceData", sheet["F2:G50"]);
range2.SetPassword("finteam");

// Protect the worksheet (users must supply the range password to edit it)
sheet.Protect("sheetPass", WorksheetProtectionPermissions.Default);

workbook.SaveDocument("rangeprotected.xlsx");
```

## Best Practices

- **Combine protection levels**: Use encryption + workbook protection + worksheet protection for maximum security.
- **Unlock before protecting**: Set `Protection.Locked = false` on editable ranges before calling `sheet.Protect()`.
- **Never hardcode passwords** in production code. Use configuration or secure storage.
- **Test permissions**: Open the saved file in Excel to verify that protection works as intended.
- **Handle events gracefully**: Always subscribe to `EncryptedFilePasswordCheckFailed` when programmatically opening encrypted files.

## Troubleshooting

- **Sheet is protected but users can still edit**: The cells you want to protect must have `Protection.Locked = true` (the default). If you previously unlocked cells globally, relock them first.
- **`Unprotect` throws an exception**: The password is incorrect. Use `Worksheet.IsProtected` to check the protection state before calling `Unprotect`.
- **EncryptionType.Compatible**: Use only for legacy `.xls` files. For `.xlsx`, always use `EncryptionType.Strong`.
- **Write protection not enforced**: Write protection is a user-interface hint recognized by Excel. The DevExpress Spreadsheet Document API itself does not enforce write protection on `LoadDocument`/`SaveDocument` calls — it is an Excel-UI-level feature.
