# Document Security — DevExpress PDF Document API

The PDF Document API supports two primary security features: password-based encryption (with granular permissions) and digital signatures (PKCS#7 and PAdES). Both are applied via `PdfSaveOptions` or the dedicated `PdfDocumentSigner` class.

## When to Use This Reference

Use this when you need to:
- Protect a PDF with a user password (prevents unauthorized opening)
- Protect a PDF with an owner password and restrict permissions (printing, copying, editing)
- Apply a digital signature using a PFX/PKCS#12 certificate file
- Apply a PAdES-BES signature with OCSP and CRL validation
- Add a timestamp to a signature (TSA server)
- Sign with a document-level timestamp
- Validate (verify) existing PKCS#7 signatures
- Use deferred/external signing (for Azure Key Vault or hardware tokens)

## Key Classes and Types

| Class | Purpose |
|-------|---------|
| `PdfEncryptionOptions` | Password and permission settings for encryption |
| `PdfSaveOptions` | Container for `EncryptionOptions` and `Signature` during save |
| `PdfEncryptionAlgorithm` | Enum: `RC4`, `AES128`, `AES256` |
| `PdfDocumentPrintingPermissions` | Enum: `Allowed`, `LowQuality`, `NotAllowed` |
| `PdfDocumentDataExtractionPermissions` | Enum: `Allowed`, `Accessibility`, `NotAllowed` |
| `PdfDocumentModificationPermissions` | Enum: `Allowed`, `DocumentAssembling`, `NotAllowed` |
| `PdfDocumentInteractivityPermissions` | Enum: `Allowed`, `FormFillingAndSigning`, `NotAllowed` |
| `PdfDocumentSigner` | Signs a PDF and saves the result |
| `Pkcs7Signer` | Creates a PKCS#7 signature from a PFX certificate |
| `PdfSignatureBuilder` | Binds a signer to a signature field |
| `PdfSignatureFieldInfo` | Defines a new signature field (page, bounds, name) |
| `PdfPkcs7Signature` | Used to read and validate existing signatures |
| `TsaClient` | Timestamp Authority client for TSA timestamps |
| `PdfSignatureAppearance` | Visual appearance of the signature (image, text) |
| `PdfDeferredSignatureBuilder` | For deferred/external signing workflows |

## Password Protection

### Protect with User and Owner Passwords

```csharp
using DevExpress.Pdf;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("input.pdf");

    PdfEncryptionOptions encryption = new PdfEncryptionOptions(
        ownerPassword: "OwnerSecret123",
        userPassword: "UserSecret456");

    // Specify the encryption algorithm (AES-256 recommended)
    encryption.Algorithm = PdfEncryptionAlgorithm.AES256;

    // Set granular permissions
    encryption.PrintingPermissions = PdfDocumentPrintingPermissions.Allowed;
    encryption.DataExtractionPermissions = PdfDocumentDataExtractionPermissions.NotAllowed;
    encryption.ModificationPermissions = PdfDocumentModificationPermissions.DocumentAssembling;
    encryption.InteractivityPermissions = PdfDocumentInteractivityPermissions.FormFillingAndSigning;

    processor.SaveDocument("protected.pdf", new PdfSaveOptions
    {
        EncryptionOptions = encryption
    });
}
```

### Permission Reference

| Permission Property | Values |
|--------------------|--------|
| `PrintingPermissions` | `Allowed`, `LowQuality`, `NotAllowed` |
| `DataExtractionPermissions` | `Allowed`, `Accessibility`, `NotAllowed` |
| `ModificationPermissions` | `Allowed`, `DocumentAssembling`, `NotAllowed` |
| `InteractivityPermissions` | `Allowed`, `FormFillingAndSigning`, `NotAllowed` |

> **Important**: Restriction permissions require an owner password. If owner and user passwords are identical (or no owner password is set), any user gets full access after opening.

## Digital Signatures

### Sign with a PFX Certificate (PKCS#7)

```csharp
using DevExpress.Pdf;
using DevExpress.Office.DigitalSignatures;

using (PdfDocumentSigner signer = new PdfDocumentSigner("input.pdf"))
{
    // Create a PKCS#7 signer from a PFX file
    Pkcs7Signer pkcs7 = new Pkcs7Signer(
        certificatePath: "certificate.pfx",
        password: "cert_password",
        hashAlgorithm: HashAlgorithmType.SHA256);

    // Define the signature field location
    var fieldInfo = new PdfSignatureFieldInfo(pageNumber: 1);
    fieldInfo.Name = "MainSignature";
    fieldInfo.SignatureBounds = new PdfRectangle(10, 10, 250, 80);

    // Build and apply the signature
    var signatureBuilder = new PdfSignatureBuilder(pkcs7, fieldInfo);
    signatureBuilder.Name = "John Doe";
    signatureBuilder.Location = "New York";
    signatureBuilder.Reason = "Document approval";

    signer.SaveDocument("signed.pdf", signatureBuilder);
}
```

### Sign with an Existing Signature Field

```csharp
using (PdfDocumentSigner signer = new PdfDocumentSigner("form_with_sigfield.pdf"))
{
    Pkcs7Signer pkcs7 = new Pkcs7Signer("cert.pfx", "password", HashAlgorithmType.SHA256);

    // Reference an existing field by name
    var signatureBuilder = new PdfSignatureBuilder(pkcs7, "ExistingSignatureField");
    signer.SaveDocument("signed.pdf", signatureBuilder);
}
```

### Sign with PAdES-BES Profile (with OCSP, CRL, and Timestamp)

```csharp
using DevExpress.Pdf;
using DevExpress.Office.DigitalSignatures;
using DevExpress.Office.Tsp;

using (PdfDocumentSigner signer = new PdfDocumentSigner("input.pdf"))
{
    IOcspClient ocspClient = new OcspClient();
    ICrlClient crlClient = new CrlClient();
    ITsaClient tsaClient = new TsaClient(
        new Uri("https://freetsa.org/tsr"),
        HashAlgorithmType.SHA256);

    Pkcs7Signer pkcs7 = new Pkcs7Signer(
        "testcert.pfx", "123",
        HashAlgorithmType.SHA256,
        tsaClient, ocspClient, crlClient,
        PdfSignatureProfile.PAdES_BES);

    var fieldInfo = new PdfSignatureFieldInfo(1);
    fieldInfo.Name = "PadesSignature";
    fieldInfo.SignatureBounds = new PdfRectangle(10, 10, 150, 150);

    var builder = new PdfSignatureBuilder(pkcs7, fieldInfo);
    signer.SaveDocument("pades_signed.pdf", builder);
}
```

### Add a Signature Timestamp (TSA)

```csharp
Pkcs7Signer pkcs7 = new Pkcs7Signer(
    "cert.pfx", "password",
    HashAlgorithmType.SHA256,
    new TsaClient(new Uri("https://tsa.example.com"), HashAlgorithmType.SHA256));
```

Pass the `TsaClient` as the fourth argument to `Pkcs7Signer`. The timestamp is embedded in the signature automatically.

### Customize Signature Appearance

```csharp
var appearance = new PdfSignatureAppearance();
// Set a custom image (left side of signature panel)
appearance.SetImageData(File.ReadAllBytes("signature_image.png"));
appearance.AppearanceType = PdfSignatureAppearanceType.ImageAndDetails;

signatureBuilder.SetSignatureAppearance(appearance);
```

### Re-Sign (Clear and Re-Apply)

```csharp
using (PdfDocumentSigner signer = new PdfDocumentSigner("signed.pdf"))
{
    // Clear the existing signature from a field
    signer.ClearSignatureField("MainSignature");

    // Apply new signature
    Pkcs7Signer newPkcs7 = new Pkcs7Signer("newcert.pfx", "pass", HashAlgorithmType.SHA256);
    var newBuilder = new PdfSignatureBuilder(newPkcs7, "MainSignature");
    signer.SaveDocument("re_signed.pdf", newBuilder);
}
```

### Validate an Existing Signature

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("signed.pdf");

    // Access signatures via the document model
    foreach (var page in processor.Document.Pages)
    {
        // Use PdfPkcs7Signature to verify — obtain via form field facade
    }
}
// For detailed signature validation, use the DevExpress documentation MCP
// or search for PdfPkcs7Signature in the DevExpress docs.
```

> **Note**: Detailed signature validation uses `PdfPkcs7Signature` class methods. Use the DxDocs MCP server (`devexpress_docs_get_content`) with the "Validate Signature" article URL for the exact code pattern, or consult: https://docs.devexpress.com/OfficeFileAPI/404728

### Deferred/External Signing (Azure Key Vault, Hardware Tokens)

The deferred signing workflow lets you compute the document hash, sign it externally (e.g., with Azure Key Vault), and embed the signature bytes:

```csharp
using DevExpress.Pdf;

using (PdfDocumentSigner signer = new PdfDocumentSigner("input.pdf"))
{
    var fieldInfo = new PdfSignatureFieldInfo(1) { Name = "DeferredSig" };
    fieldInfo.SignatureBounds = new PdfRectangle(10, 10, 200, 80);

    var deferredBuilder = new PdfDeferredSignatureBuilder(fieldInfo,
        new ExternalSignerInfo(HashAlgorithmType.SHA256));

    // Get the document hash for external signing
    PdfDeferredSigner deferredSigner = signer.SignDeferred(deferredBuilder);
    byte[] documentHash = deferredSigner.Hash;

    // --- Sign documentHash externally (Azure Key Vault, HSM, etc.) ---
    byte[] signatureBytes = SignExternally(documentHash); // your implementation

    // Embed the signature and save
    deferredSigner.Sign(signatureBytes, "output.pdf");
}
```

## Troubleshooting

- **`ArgumentException`: field name not found when signing**: Ensure the signature field exists in the document. Check field names with `processor.DocumentFacade.AcroForm.GetNames()`.
- **Signature shows as invalid in Adobe Reader**: The signing certificate may not be trusted by Adobe's trust store. For production, use a certificate from a trusted CA. For testing, add the certificate to Reader's trusted list.
- **`TspValidationException`**: The TSA server rejected the timestamp request. Verify the TSA URI is correct and accessible. Try a different TSA server.
- **Encrypting a PDF/A document fails**: PDF/A does not permit encryption. Remove the `PdfCompatibility` option or remove the `EncryptionOptions`.
- **Password protection has no effect**: Owner and user passwords must be different for restrictions to work. If they are the same, the user gets full access.
- **Azure Key Vault signing**: Use a `Pkcs7SignerBase` descendant with a custom `BuildSignature` implementation. See the DevExpress example: https://github.com/DevExpress-Examples/pdf-document-api-use-azure-key-vault-api-to-sign-document
