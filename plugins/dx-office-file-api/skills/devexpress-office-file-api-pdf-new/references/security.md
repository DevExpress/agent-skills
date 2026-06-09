# Protect and Encrypt PDF Documents

> **CTP Warning**: `DevExpress.Docs.Pdf` is a Community Technology Preview. Do not use in mission-critical production applications.

## When you need to:
- Open a password-protected PDF
- Check document permissions and access mode
- Encrypt a PDF with user and owner passwords (AES-128 / AES-256)
- Set granular permissions (print, modify, extract)
- Remove encryption from a document

## Open an Encrypted PDF

Pass the password via `LoadOptions` to the `PdfDocument` constructor:

```csharp
using DevExpress.Docs.Pdf;
using System.IO;

using (FileStream fileStream = File.OpenRead("Document.pdf")) {
    using (PdfDocument pdfDocument = new PdfDocument(
        fileStream, new LoadOptions { Password = "userPassword" })) {
        // Work with the document
    }
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports System.IO

Using fileStream As FileStream = File.OpenRead("Document.pdf")
    Using pdfDocument As New PdfDocument(fileStream, New LoadOptions With {.Password = "userPassword"})
        ' Work with the document
    End Using
End Using
```

## Check Permissions and Access Mode

After opening an encrypted document, inspect permission properties:

```csharp
using (PdfDocument pdfDocument = new PdfDocument(
    File.OpenRead("Document.pdf"),
    new LoadOptions { Password = "userPassword" })) {
    Console.WriteLine($"Access mode: {pdfDocument.AccessMode}");
    Console.WriteLine($"Data extraction: {pdfDocument.DataExtractionPermissions}");
    Console.WriteLine($"Modification: {pdfDocument.ModificationPermissions}");
    Console.WriteLine($"Interactivity: {pdfDocument.InteractivityPermissions}");
    Console.WriteLine($"Print: {pdfDocument.PrintPermissions}");
}
```

| Property | Description |
|----------|-------------|
| `AccessMode` | `DocumentAccessMode` — whether opened as Owner or User |
| `DataExtractionPermissions` | `DocumentDataExtractionPermissions` |
| `ModificationPermissions` | `DocumentModificationPermissions` |
| `InteractivityPermissions` | `DocumentInteractivityPermissions` |
| `PrintPermissions` | `DocumentPrintPermissions` |

## Encrypt a PDF

Create `EncryptionOptions` with owner and user passwords, configure permissions, then call `PdfDocument.Encrypt`.

```csharp
using DevExpress.Docs.Pdf;
using System.IO;

using (FileStream fileStream = File.OpenRead("Document.pdf")) {
    using (PdfDocument pdfDocument = new PdfDocument(fileStream)) {
        var encryptionOptions = new EncryptionOptions(
            ownerPassword: "ownerPassword",
            userPassword: "userPassword"
        );

        encryptionOptions.Algorithm = EncryptionAlgorithm.AES256;
        encryptionOptions.DataExtractionPermissions =
            DocumentDataExtractionPermissions.NotAllowed;
        encryptionOptions.PrintPermissions =
            DocumentPrintPermissions.LowQuality;
        encryptionOptions.ModificationPermissions =
            DocumentModificationPermissions.NotAllowed;

        pdfDocument.Encrypt(encryptionOptions);
        pdfDocument.Save(
            new FileStream("Document_secured.pdf", FileMode.Create, FileAccess.Write));
    }
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports System.IO

Using fileStream As FileStream = File.OpenRead("Document.pdf")
    Using pdfDocument As New PdfDocument(fileStream)
        Dim encryptionOptions As New EncryptionOptions(
            ownerPassword:="ownerPassword",
            userPassword:="userPassword"
        )

        encryptionOptions.Algorithm = EncryptionAlgorithm.AES256
        encryptionOptions.DataExtractionPermissions = DocumentDataExtractionPermissions.NotAllowed
        encryptionOptions.PrintPermissions = DocumentPrintPermissions.LowQuality
        encryptionOptions.ModificationPermissions = DocumentModificationPermissions.NotAllowed

        pdfDocument.Encrypt(encryptionOptions)
        pdfDocument.Save(New FileStream("Document_secured.pdf", FileMode.Create, FileAccess.Write))
    End Using
End Using
```

> **Note**: If owner and user passwords are the same, or if only the user password is set, the result is a document with an opening restriction only — the opened user has full access rights.

## EncryptionOptions Reference

| Property | Type | Description |
|----------|------|-------------|
| `EncryptionOptions(ownerPassword, userPassword)` | constructor | Passwords for the document |
| `Algorithm` | `EncryptionAlgorithm` | `AES128` or `AES256` |
| `DataExtractionPermissions` | `DocumentDataExtractionPermissions` | `Allowed` or `NotAllowed` |
| `ModificationPermissions` | `DocumentModificationPermissions` | `NotAllowed`, `Annotations`, `FormFilling`, `Other` |
| `InteractivityPermissions` | `DocumentInteractivityPermissions` | `Allowed` or `NotAllowed` |
| `PrintPermissions` | `DocumentPrintPermissions` | `NotAllowed`, `LowQuality`, `HighQuality` |

## Remove Encryption

```csharp
using (PdfDocument pdfDocument = new PdfDocument(
    File.OpenRead("protected.pdf"),
    new LoadOptions { Password = "ownerPassword" })) {
    pdfDocument.RemoveEncryption();
    pdfDocument.Save(new FileStream("unprotected.pdf", FileMode.Create, FileAccess.Write));
}
```

## Important Notes

- `PdfDocument.Save` preserves encryption settings. To remove them, call `RemoveEncryption()` before saving.
- Always save to a **new** `FileStream` with `FileMode.Create`, not the input stream.
- `InvalidOperationException` on save after encryption usually means saving to the same stream used to open. Use a new output stream.
