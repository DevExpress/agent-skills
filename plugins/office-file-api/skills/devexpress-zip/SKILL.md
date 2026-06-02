---
name: devexpress-zip
description: Build .NET applications with the DevExpress Zip Compression and Archive API for creating, reading, and extracting ZIP archives programmatically. Use when creating zip files, extracting archives, compressing files or streams, adding password protection to archives, compressing byte arrays or .NET streams, or handling file conflicts during extraction. Also use when someone mentions "DevExpress Zip", "DevExpress Compression", "ZipArchive", "DevExpress.Compression", "create zip C#", "extract zip .NET", "compress stream", or asks about ZIP/archive operations with DevExpress. Covers both .NET and .NET Framework.
compatibility: Requires .NET 6+ or .NET Framework 4.6.2+. NuGet packages from the DevExpress feed.
metadata:
  author: DevExpress
  version: "25.2"
  source-commit: c5a96ff6e891a1c2633c6621186093faaefabefd
---

# DevExpress Zip Compression and Archive API

The Zip Compression and Archive API is a non-visual .NET library for creating, reading, and extracting ZIP archives programmatically — without any external dependencies. It supports archiving files, directories, streams, byte arrays, and text; password-protecting entries with AES-256 or ZipCrypto; filtering content during archiving; cancellation via progress callbacks; and conflict resolution during extraction.

## When to Use This Skill

Use this skill when you need to:

- Create a ZIP archive from a directory (recursively) or from a list of files
- Add individual files to a new or existing ZIP archive
- Filter files during archiving using the `ItemAdding` event
- Archive a .NET `Stream` or a `byte[]` directly (without writing to disk first)
- Archive a text string as a named entry in a ZIP file
- Password-protect archive entries using AES-256 (`EncryptionType.Aes256`) or ZipCrypto (`EncryptionType.PkZip`)
- Add comments to individual archive entries
- Extract (unzip) an archive to a directory
- Resolve file conflicts during extraction with a callback (`AllowFileOverwrite` event)
- Cancel a long-running archiving operation via the `Progress` event
- Handle archiving errors without aborting the entire operation
- Add files to an existing archive without losing its current contents

## Prerequisites & Installation

### NuGet Packages

| Package | Purpose |
|---------|---------|
| `DevExpress.Document.Processor` | Core ZIP compression and archive functionality |

### .NET (6/7/8+)

```bash
dotnet add package DevExpress.Document.Processor
```

### .NET Framework (4.6.2+)

```
Install-Package DevExpress.Document.Processor
```

Or add a reference to `DevExpress.Docs.v25.2.dll` directly.

**Important**: All DevExpress packages in a project must share the same version number. A valid DevExpress license is required.

## Before You Start — Ask the Developer

Before generating code, ask these questions to avoid rework:

### General Questions
1. **Target framework**: Are you using .NET 8+, .NET 6/7, or .NET Framework 4.x?
2. **New or existing project?**: Creating new or adding to existing?
3. **Hosting model**: Console app, ASP.NET Core, Blazor, MAUI, or something else?

### Zip-Specific Questions
4. **Operation?** Create archive / extract archive / compress stream or byte array / add to existing archive
5. **Source?** Directory / specific files with filtering / in-memory stream or byte array
6. **Special requirements?** Password protection / file conflict resolution / cancellation / comments

> **Rule**: If the developer's answer is ambiguous or missing, ask before generating code. Do not guess.

## Component Overview

The Zip Compression and Archive API provides:

- **Archive creation**: Create ZIP archives from directories, file lists, streams, byte arrays, or text (`ZipArchive`, `ZipFileItem`, `ZipDirectoryItem`, `ZipStreamItem`, `ZipByteArrayItem`, `ZipTextItem`)
- **Archive reading and extraction**: Open existing ZIP files and extract items to disk (`ZipArchive.Read()`, `ZipItem.Extract()`)
- **Password protection**: Encrypt entries with AES-256 or ZipCrypto (`ZipArchive.Password`, `ZipArchive.EncryptionType`, `EncryptionType`)
- **Event-driven control**: Filter items, track progress, resolve conflicts, handle errors (`ItemAdding`, `Progress`, `AllowFileOverwrite`, `Error` events)
- **In-place update**: Update an existing archive without losing its contents (`ZipArchive.Read()` + `AddFile()`/`UpdateFile()`)

### Core Entry Point

```csharp
using DevExpress.Compression;

// Create a new archive from a directory
using (ZipArchive archive = new ZipArchive())
{
    archive.AddDirectory(@"C:\MyFiles");
    archive.Save("output.zip");
}

// Read and extract an existing archive
using (ZipArchive archive = ZipArchive.Read("output.zip"))
{
    foreach (ZipItem item in archive)
        item.Extract(@"C:\Extracted");
}
```

## Documentation & Navigation Guide

### Getting Started
Refer to [references/getting-started.md](references/getting-started.md)

When you need to:
- Set up the Zip API for the first time (NuGet, .NET Framework references)
- Create your first ZIP archive from a directory
- Extract an archive to a folder
- See complete compilable examples for both operations

## Quick Start Example

A complete minimal example — archive two in-memory text files, then extract the result:

```csharp
using DevExpress.Compression;
using System.Text;

// --- Part 1: Create archive ---
string zipPath = "example.zip";
using (ZipArchive archive = new ZipArchive())
{
    archive.AddText("hello.txt", "Hello from DevExpress Zip API!");
    archive.AddText("readme.txt", "This archive was created programmatically.");
    archive.Save(zipPath);
}
Console.WriteLine($"Archive created: {zipPath}");

// --- Part 2: Extract archive ---
string extractDir = "extracted";
Directory.CreateDirectory(extractDir);
using (ZipArchive archive = ZipArchive.Read(zipPath))
{
    foreach (ZipItem item in archive)
        item.Extract(extractDir);
}
Console.WriteLine($"Extracted to: {extractDir}");
```

### What This Does
Creates a ZIP file containing two text entries generated entirely in memory, then extracts both files to the `extracted/` subdirectory. No intermediate files are needed for the source content.

## Key Properties & API Surface

### ZipArchive

| Property/Method | Type | Description |
|----------------|------|-------------|
| `new ZipArchive()` | — | Creates an empty archive ready to receive items |
| `ZipArchive.Read(path)` | `ZipArchive` | Opens an existing ZIP file for reading/extraction |
| `ZipArchive.Read(stream, encoding, leaveOpen)` | `ZipArchive` | Opens a ZIP from a stream (use to add to existing archive) |
| `AddDirectory(path)` | `ZipDirectoryItem` | Recursively adds all files in a directory |
| `AddFile(filePath, folderInZip)` | `ZipFileItem` | Adds a single file to a specified folder in the archive |
| `AddFiles(paths)` | `void` | Adds a collection of file paths at once |
| `AddStream(name, stream)` | `ZipStreamItem` | Adds a .NET stream as a named entry |
| `AddByteArray(name, bytes)` | `ZipByteArrayItem` | Adds a byte array as a named entry |
| `AddText(name, text)` | `ZipTextItem` | Adds a UTF-8 text string as a named file entry |
| `Save(filePath)` | `void` | Compresses and saves the archive to a file |
| `Save(stream)` | `void` | Compresses and saves the archive to a stream |
| `Password` | `string` | Sets the password for encryption |
| `EncryptionType` | `EncryptionType` | `Aes256` (strong) or `PkZip` (ZipCrypto, legacy) |
| `OptionsBehavior` | `ZipArchiveOptionsBehavior` | Behavior settings (e.g., `AllowFileOverwrite` mode) |
| `ItemAdding` | event | Fires before each item is added; set `Action` to skip/stop |
| `Progress` | event | Fires during save; set `CanContinue = false` to cancel |
| `AllowFileOverwrite` | event | Fires on extraction conflict; set `Cancel = true` to skip |
| `Error` | event | Fires on item-level error; set `CanContinue = false` to abort |

### ZipItem (base for all item types)

| Property/Method | Type | Description |
|----------------|------|-------------|
| `Name` | `string` | Entry name within the archive |
| `Comment` | `string` | Per-entry comment string |
| `LastWriteTime` | `DateTime` | Last modified timestamp |
| `CreationTime` | `DateTime` | Creation timestamp |
| `Extract(targetDir)` | `void` | Extracts this item to the specified directory |

### EncryptionType enum

| Value | Description |
|-------|-------------|
| `EncryptionType.Aes256` | AES-256 encryption (strong, recommended) |
| `EncryptionType.PkZip` | Traditional ZipCrypto (legacy compatibility) |

## Common Patterns

### Archive a Directory

```csharp
using DevExpress.Compression;

using (ZipArchive archive = new ZipArchive())
{
    archive.AddDirectory(@"C:\MyDocuments");
    archive.Save("documents.zip");
}
```

### Archive Selected Files (filtered by extension)

```csharp
using DevExpress.Compression;

string[] files = Directory.GetFiles(@"C:\MyDocuments", "*.*", SearchOption.AllDirectories);
string[] allowed = { ".txt", ".docx" };

using (ZipArchive archive = new ZipArchive())
{
    foreach (string file in files)
    {
        if (allowed.Contains(Path.GetExtension(file)))
            archive.AddFile(file, "/");
    }
    archive.Save("selected.zip");
}
```

### Add Files via ItemAdding Event (conditional filtering)

```csharp
using DevExpress.Compression;

using (ZipArchive archive = new ZipArchive())
{
    archive.ItemAdding += (sender, args) =>
    {
        // Skip files not modified today
        if (args.Item.CreationTime.Date != DateTime.Today)
            args.Action = ZipItemAddingAction.Cancel;
    };
    archive.AddDirectory(@"C:\MyFiles");
    archive.Save("filtered.zip");
}
```

### Add File to an Existing Archive

```csharp
using DevExpress.Compression;

string zipPath = "existing.zip";
using (MemoryStream ms = new MemoryStream())
{
    using (FileStream fs = File.Open(zipPath, FileMode.Open))
        fs.CopyTo(ms);
    ms.Seek(0, SeekOrigin.Begin);

    using (ZipArchive archive = ZipArchive.Read(ms, System.Text.Encoding.Default, false))
    {
        archive.AddFile(@"C:\NewFile.txt", "/");
        archive.Save(zipPath);
    }
}
```

### Password-Protect an Archive (AES-256)

```csharp
using DevExpress.Compression;

using (ZipArchive archive = new ZipArchive())
{
    archive.Password = "s3cur3P@ssword";
    archive.EncryptionType = EncryptionType.Aes256;
    archive.AddDirectory(@"C:\Sensitive");
    archive.Save("protected.zip");
}
```

### Compress a .NET Stream

```csharp
using DevExpress.Compression;

byte[] data = System.Text.Encoding.UTF8.GetBytes("Content to compress");
using (Stream inputStream = new MemoryStream(data))
using (Stream outputStream = new FileStream("stream.zip", FileMode.Create))
using (ZipArchive archive = new ZipArchive())
{
    archive.AddStream("data.bin", inputStream);
    archive.Save(outputStream);
}
```

### Compress a Byte Array

```csharp
using DevExpress.Compression;

byte[] payload = Enumerable.Repeat((byte)0x78, 10000).ToArray();
using (Stream outputStream = new FileStream("bytes.zip", FileMode.Create))
using (ZipArchive archive = new ZipArchive())
{
    archive.AddByteArray("payload.bin", payload);
    archive.Save(outputStream);
}
```

### Add a Text String Directly

```csharp
using DevExpress.Compression;

using (ZipArchive archive = new ZipArchive())
{
    archive.AddText("notes.txt", "Hello, this is stored as UTF-8 text.");
    archive.Save("text.zip");
}
```

### Add Per-Entry Comments

```csharp
using DevExpress.Compression;

using (ZipArchive archive = new ZipArchive())
{
    foreach (string file in Directory.EnumerateFiles(@"C:\MyFiles"))
    {
        ZipFileItem item = archive.AddFile(file, "/");
        item.Comment = $"Archived by {Environment.UserName} on {DateTime.Today:yyyy-MM-dd}";
    }
    archive.Save("commented.zip");
}
```

### Cancel Archiving via Progress Event

```csharp
using DevExpress.Compression;

bool shouldCancel = false; // set to true from UI or timer

using (ZipArchive archive = new ZipArchive())
{
    archive.Progress += (sender, args) => { args.CanContinue = !shouldCancel; };
    archive.AddDirectory(@"C:\LargeFolder");
    archive.Save("large.zip");
}
```

### Extract an Archive to a Directory

```csharp
using DevExpress.Compression;

using (ZipArchive archive = ZipArchive.Read("archive.zip"))
{
    foreach (ZipItem item in archive)
        item.Extract(@"C:\Output");
}
```

### Resolve File Conflicts During Extraction

```csharp
using DevExpress.Compression;

using (ZipArchive archive = ZipArchive.Read("archive.zip"))
{
    archive.OptionsBehavior.AllowFileOverwrite = AllowFileOverwriteMode.Custom;
    archive.AllowFileOverwrite += (sender, e) =>
    {
        // Skip overwriting if the existing file is newer
        FileInfo fi = new FileInfo(e.TargetFilePath);
        if (e.ZipItem.LastWriteTime < fi.LastWriteTime)
            e.Cancel = true;
    };
    foreach (ZipItem item in archive)
        item.Extract(@"C:\Output");
}
```

### Handle Errors Without Aborting

```csharp
using DevExpress.Compression;

using (ZipArchive archive = new ZipArchive())
{
    archive.Error += (sender, args) =>
    {
        Exception ex = args.GetException();
        Console.WriteLine($"Skipping {args.ItemName}: {ex.Message}");
        args.CanContinue = true; // continue with remaining files
    };
    archive.AddDirectory(@"C:\MyFiles");
    archive.Save("safe.zip");
}
```

## Troubleshooting

| Symptom | Cause | Solution |
|---------|-------|----------|
| `InvalidOperationException` when saving to same file path as source | `ZipArchive.Read()` locks the source file; `Save()` cannot overwrite it | Copy the file to a `MemoryStream` first, then call `ZipArchive.Read(stream)` and `Save(originalPath)` |
| Extracted files have no content / wrong encoding | Text encoding mismatch when reading the archive | Pass the correct `Encoding` to `ZipArchive.Read(stream, encoding, leaveOpen)` |
| `WrongPasswordException` during extraction | Archive is password-protected; no password supplied | Set `archive.Password` before calling `item.Extract()` |
| Archive is created but entries are skipped silently | `ItemAdding` handler sets `Action = ZipItemAddingAction.Cancel` | Review the `ItemAdding` handler logic; check file dates/conditions |
| Archiving stops before completion | `Progress` handler sets `CanContinue = false` | Check that `stopProgress` / cancellation flag is not set prematurely |
| Build error: `DevExpress.Compression` namespace not found | NuGet package not installed | Run `dotnet add package DevExpress.Document.Processor` |
| License error at runtime | Missing or invalid DevExpress license | Register your license key per the DevExpress installation guide |
| Version mismatch build error | Mixed DevExpress package versions in the project | Ensure all DX packages use the exact same version (e.g., all 25.2.x) |

## Constraints & Rules

CRITICAL — follow these rules in every interaction:

1. **Build verification**: After making changes, verify the project builds with `dotnet build`. Check for errors before reporting success.
2. **NuGet packages**: Use only `DevExpress.Document.Processor`. Do not guess other package names.
3. **Namespace imports**: Always include `using DevExpress.Compression;` and other necessary `System.IO` directives.
4. **Version consistency**: All DevExpress packages must use the same version. Do not mix.
5. **License**: DevExpress requires a valid license. Remind the developer if they hit license-related build errors.
6. **No destructive changes**: Preserve existing code structure. Only add or modify what is necessary.
7. **Framework detection**: Check the project's .csproj for target framework before writing code. Both .NET and .NET Framework use the same `ZipArchive` API surface.
8. **Read-then-save pattern**: When adding files to an existing archive, always copy the file to a `MemoryStream` before calling `ZipArchive.Read()` to avoid file lock conflicts on `Save()`.

## Using DevExpress Documentation MCP

If the DxDocs MCP server is available, use it to supplement this skill:

- **Search**: Use `devexpress_docs_search` with technology "Zip Compression" and your question.
- **Fetch**: Use `devexpress_docs_get_content` with a documentation URL to get full article content.

**When to use MCP vs. built-in references:**
- **Built-in references**: Getting started, common patterns, key properties, troubleshooting.
- **MCP search**: Advanced scenarios not covered here, version-specific changes, uncommon features.
- **Always MCP for**: Exact method overload signatures, event args properties, or enum values when not 100% certain.

---

## Next Steps

Start with **[Getting Started](references/getting-started.md)** to install and configure the Zip API, then use the Common Patterns above to build your archiving logic.
