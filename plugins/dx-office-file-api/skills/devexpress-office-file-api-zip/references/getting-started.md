# Getting Started with the DevExpress Zip Compression and Archive API

This guide walks you through installing the DevExpress Zip API and performing the two most common operations: creating a ZIP archive and extracting one.

## When to Use This Reference

Use this when you need to:
- Set up the DevExpress Zip API in a new or existing project
- Understand the difference between .NET and .NET Framework installation
- Create your first ZIP archive from a directory or file list
- Extract (unzip) an archive to a directory
- See complete, compilable examples for both create and extract

## System Requirements

- .NET 8.0 / 9.0 / 10.0+ or .NET Framework 4.6.2+
- Visual Studio 2022+ (recommended) or JetBrains Rider
- A valid DevExpress license

## Installation

### Step 1: Install the NuGet Package

**.NET CLI:**
```bash
dotnet add package DevExpress.Document.Processor
```

**Package Manager Console:**
```
Install-Package DevExpress.Document.Processor
```

**For .NET Framework projects only:** Alternatively, add a direct assembly reference to `DevExpress.Docs.v25.2.dll` from your DevExpress installation directory.

### Step 2: License

DevExpress validates your license automatically when you install via the DevExpress installer. For NuGet-only installations, follow the [DevExpress license deployment guide](https://docs.devexpress.com/GeneralInformation/116698).

**Important**: All DevExpress packages in a single project must use the same version number (e.g., all `25.2.x`). Mixed versions cause build errors.

## Key Classes

| Class | Purpose |
|-------|---------|
| `ZipArchive` | Primary entry point — create, open, save, and extract archives |
| `ZipItem` | Base class for all archive entries; has `Extract()` method |
| `ZipFileItem` | Represents a file entry; returned by `AddFile()` |
| `ZipDirectoryItem` | Represents a directory entry; returned by `AddDirectory()` |
| `ZipStreamItem` | Represents a stream-based entry; returned by `AddStream()` |
| `ZipByteArrayItem` | Represents a byte-array entry; returned by `AddByteArray()` |
| `ZipTextItem` | Represents a text string entry; returned by `AddText()` |
| `EncryptionType` | Enum: `Aes256` (strong) or `PkZip` (ZipCrypto legacy) |

## Create Your First Archive

### Archive a Directory

The simplest way to create a ZIP file is to archive an entire directory recursively:

```csharp
using DevExpress.Compression;
using System;

class Program
{
    static void Main()
    {
        string sourceDir = @"C:\MyDocuments";
        string zipPath = "MyDocuments.zip";

        using (ZipArchive archive = new ZipArchive())
        {
            archive.AddDirectory(sourceDir);
            archive.Save(zipPath);
        }

        Console.WriteLine($"Archive created: {zipPath}");
    }
}
```

### Archive Specific Files

Add only files matching certain criteria (e.g., by extension):

```csharp
using DevExpress.Compression;
using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string sourceDir = @"C:\MyDocuments";
        string zipPath = "TextAndDocs.zip";
        string[] allowedExtensions = { ".txt", ".docx" };

        string[] allFiles = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);

        using (ZipArchive archive = new ZipArchive())
        {
            foreach (string file in allFiles)
            {
                if (allowedExtensions.Contains(Path.GetExtension(file)))
                    archive.AddFile(file, "/");  // "/" = archive root folder
            }
            archive.Save(zipPath);
        }

        Console.WriteLine($"Archive created: {zipPath}");
    }
}
```

`AddFile(filePath, folderInZip)` — the second argument is the path inside the archive. Use `"/"` to place files at the root, or `"/subfolder/"` to place them in a virtual subdirectory.

### Archive with Password Protection

```csharp
using DevExpress.Compression;

using (ZipArchive archive = new ZipArchive())
{
    archive.Password = "s3cur3P@ssword";
    archive.EncryptionType = EncryptionType.Aes256;  // or EncryptionType.PkZip for legacy
    archive.AddDirectory(@"C:\Sensitive");
    archive.Save("protected.zip");
}
```

### Save to a Stream Instead of a File

```csharp
using DevExpress.Compression;
using System.IO;

// Example: serve a ZIP from an ASP.NET Core action
using (MemoryStream ms = new MemoryStream())
{
    using (ZipArchive archive = new ZipArchive())
    {
        archive.AddText("hello.txt", "Hello from the server!");
        archive.Save(ms);
    }
    byte[] zipBytes = ms.ToArray();
    // return File(zipBytes, "application/zip", "download.zip");
}
```

## Extract an Archive

### Basic Extraction

```csharp
using DevExpress.Compression;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        string zipPath = "MyDocuments.zip";
        string outputDir = @"C:\Extracted";

        Directory.CreateDirectory(outputDir);

        using (ZipArchive archive = ZipArchive.Read(zipPath))
        {
            foreach (ZipItem item in archive)
                item.Extract(outputDir);
        }

        Console.WriteLine($"Extracted to: {outputDir}");
    }
}
```

### Extract a Password-Protected Archive

```csharp
using DevExpress.Compression;

using (ZipArchive archive = ZipArchive.Read("protected.zip"))
{
    archive.Password = "s3cur3P@ssword";
    foreach (ZipItem item in archive)
        item.Extract(@"C:\Extracted");
}
```

### Handle Conflicts During Extraction

When a file already exists at the target path, `AllowFileOverwrite` fires. Use it to decide whether to overwrite:

```csharp
using DevExpress.Compression;
using System.IO;

using (ZipArchive archive = ZipArchive.Read("archive.zip"))
{
    // Enable custom conflict handling
    archive.OptionsBehavior.AllowFileOverwrite = AllowFileOverwriteMode.Custom;
    archive.AllowFileOverwrite += (sender, e) =>
    {
        FileInfo existing = new FileInfo(e.TargetFilePath);
        // Skip overwriting if the file on disk is newer than the archive entry
        if (e.ZipItem.LastWriteTime < existing.LastWriteTime)
            e.Cancel = true;
    };

    foreach (ZipItem item in archive)
        item.Extract(@"C:\Output");
}
```

## Add Files to an Existing Archive

`ZipArchive.Read()` locks the source file, so you cannot call `Save()` to the same path directly. Copy the file to a `MemoryStream` first:

```csharp
using DevExpress.Compression;
using System.IO;

string zipPath = "existing.zip";

// Step 1: copy the existing archive into memory
using (MemoryStream ms = new MemoryStream())
{
    using (FileStream fs = File.Open(zipPath, FileMode.Open))
        fs.CopyTo(ms);
    ms.Seek(0, SeekOrigin.Begin);

    // Step 2: open from the in-memory copy, add files, save back to the original path
    using (ZipArchive archive = ZipArchive.Read(ms, System.Text.Encoding.Default, false))
    {
        archive.AddFile(@"C:\NewDocument.txt", "/");
        archive.Save(zipPath);
    }
}
```

## Secure ZIP Policy (v26.1+)

`SecureZipPolicy` (`DevExpress.Utils.Zip.SecureZipPolicy`) protects against ZIP-based attack vectors by enforcing resource limits. Pass `ResourceLimits` to `ZipArchive.Read()` overloads.

```csharp
using DevExpress.Compression;
using DevExpress.Utils.Zip;

// Apply resource limits per extraction call
var limits = new SecureZipPolicy.ResourceLimits
{
    // TODO: Verify exact property names — use DxDocs MCP: devexpress_docs_search(technology="Compression", query="SecureZipPolicy ResourceLimits")
};

using (ZipArchive archive = ZipArchive.Read("untrusted.zip", limits))
{
    foreach (ZipItem item in archive)
        item.Extract(@"C:\Output");
}
```

The `SecureZipPolicy.TrustBoundaryViolation` event fires when a policy constraint is exceeded, allowing you to log violations before they result in an exception.

> **Note**: `SecureZipPolicy` is new in v26.1 and enforces security best practices for applications that process ZIP files from untrusted sources (user uploads, email attachments). For confirmed API details, use the DxDocs MCP: `devexpress_docs_search(technology="Compression", query="SecureZipPolicy")`.

## What to Learn Next

- **Password protection and encryption**: See the `Password` and `EncryptionType` properties in the [SKILL.md Common Patterns](../SKILL.md#password-protect-an-archive-aes-256) section.
- **Stream and byte-array compression**: See "Compress a .NET Stream" and "Compress a Byte Array" patterns in [SKILL.md](../SKILL.md#compress-a-net-stream).
- **Filtering and cancellation**: See "Add Files via ItemAdding Event" and "Cancel Archiving via Progress Event" in [SKILL.md](../SKILL.md#add-files-via-itemadding-event-conditional-filtering).
- **Error handling**: See "Handle Errors Without Aborting" in [SKILL.md](../SKILL.md#handle-errors-without-aborting).
