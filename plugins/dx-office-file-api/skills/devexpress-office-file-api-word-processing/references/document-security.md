# Document Security — Word Processing Document API

## When to Use This Reference

Use this when you need to password-protect a document, encrypt a DOCX/DOC file, restrict editing to specific users via range permissions, or open an existing encrypted file.

## Protect a Document (Restrict Editing)

Call `Document.Protect` to enforce a protection mode and set a password. The `DocumentProtectionType` enum controls which actions are still allowed.

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("document.docx");
    // Allow only form-field filling; everything else is read-only
    server.Document.Protect("password", DocumentProtectionType.FillInForms);
    server.SaveDocument("document_protected.docx", DocumentFormat.Docx);
}
```

Use `Document.Unprotect()` to remove protection without a password prompt. Check `Document.IsDocumentProtected` to test whether protection is currently active.

### Protect Individual Sections

To lock only specific sections, set `Section.ProtectedForForms = true` on those sections and call `Document.Protect`.

```csharp
Document doc = server.Document;
doc.Sections[1].ProtectedForForms = true; // lock section 2
doc.Protect("password", DocumentProtectionType.FillInForms);
```

## Grant Range Permissions

Range permissions let named users or groups edit specific ranges inside a protected document.

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("Documents/Grimm.docx", DocumentFormat.Docx);
    Document document = server.Document;

    // Step 1: open the range-permissions collection for editing
    RangePermissionCollection rangePermissions = document.BeginUpdateRangePermissions();

    // Step 2: create a permission for a specific paragraph range
    RangePermission rp = rangePermissions.CreateRangePermission(document.Paragraphs[3].Range);
    rp.Group = "Administrators";
    rp.UserName = "admin@somecompany.com";

    // Step 3: add it and commit
    rangePermissions.Add(rp);
    document.EndUpdateRangePermissions(rangePermissions);

    // Step 4: enable protection so unallowed ranges become read-only
    document.Protect("123");

    server.SaveDocument("protected.docx", DocumentFormat.Docx);
}
```

## Encrypt a File with a Password

Use `Document.Encryption` to set the encryption type and password before saving, or pass an `EncryptionSettings` object to `SaveDocument`.

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx");

    // Option A: set via Document.Encryption property
    server.Document.Encryption.Type = EncryptionType.Strong;
    server.Document.Encryption.Password = "12345";
    server.SaveDocument("encrypted_a.docx", DocumentFormat.Docx);

    // Option B: pass EncryptionSettings to SaveDocument
    EncryptionSettings settings = new EncryptionSettings();
    settings.Type = EncryptionType.Strong;
    settings.Password = "12345";
    server.SaveDocument("encrypted_b.docx", DocumentFormat.Docx, settings);
}
```

## Open a Password-Encrypted File

Specify the password via `BeforeImport` event options, or handle the `EncryptedFilePasswordRequested` event interactively.

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

// Simple: supply the password before import
var server = new RichEditDocumentServer();
server.BeforeImport += (s, e) =>
{
    if (e.DocumentFormat == DocumentFormat.Docx)
        ((DocxDocumentImporterOptions)e.Options).EncryptionPassword = "12345";
};
server.LoadDocument("encrypted.docx");

// Interactive: prompt the user on failure
server.EncryptedFilePasswordRequested += (s, e) =>
{
    Console.Write("Enter password: ");
    e.Password = Console.ReadLine();
    e.Handled = true;
};
server.EncryptedFilePasswordCheckFailed += (s, e) =>
{
    if (e.Error == RichEditDecryptionError.WrongPassword)
    {
        e.TryAgain = true;
        e.Handled = true;
    }
};
server.LoadDocument("encrypted.docx");
```

## Key Classes and Types

| Class/Enum | Purpose |
|-----------|---------|
| `Document.Protect(password, type)` | Enable protection with a password and restriction type |
| `Document.Unprotect()` | Remove protection (no password required) |
| `Document.IsDocumentProtected` | Returns `true` when protection is active |
| `DocumentProtectionType` | Enum: `ReadOnly`, `AllowComments`, `FillInForms`, `AllowOnlyRevisions` |
| `Section.ProtectedForForms` | Lock a single section from editing |
| `RangePermissionCollection` | Collection returned by `BeginUpdateRangePermissions` |
| `RangePermission` | Represents one user/group edit grant on a document range |
| `RangePermission.UserName` | Email or username of the permitted user |
| `RangePermission.Group` | Group name of permitted users |
| `Document.Encryption` | Provides `Type` (`EncryptionType`) and `Password` for encrypting on save |
| `EncryptionSettings` | Passed to `SaveDocument` overload to encrypt a file |
| `EncryptionType` | Enum: `Strong` (AES-256), `Compatible` (RC4) |
| `EncryptedFilePasswordRequested` | Event raised when a password-protected file is opened without a password |
| `EncryptedFilePasswordCheckFailed` | Event raised when the supplied password is wrong |
| `RichEditDecryptionError` | Enum returned in `EncryptedFilePasswordCheckFailedEventArgs.Error` |
