// DevExpress Zip Compression and Archive API — Quickstart
// NuGet: DevExpress.Document.Processor (v25.2)
// Namespace: DevExpress.Compression
//
// This console app demonstrates two operations in sequence:
//   1. Create a ZIP archive containing two text files generated in memory
//   2. Extract the archive to a temporary directory
//
// dotnet add package DevExpress.Document.Processor
// dotnet run

using System;
using System.IO;
using DevExpress.Compression;

class Program
{
    static void Main(string[] args)
    {
        string zipPath = Path.Combine(Path.GetTempPath(), "devexpress_quickstart.zip");
        string extractDir = Path.Combine(Path.GetTempPath(), "devexpress_quickstart_extracted");

        // ---------------------------------------------------------------
        // Part 1: Create a ZIP archive with two in-memory text files
        // ---------------------------------------------------------------
        Console.WriteLine("=== Creating ZIP archive ===");

        using (ZipArchive archive = new ZipArchive())
        {
            // AddText(entryName, content) creates a UTF-8 text entry without
            // needing a physical file on disk.
            archive.AddText("hello.txt", "Hello from the DevExpress Zip API!");
            archive.AddText("info.txt", $"Archive created on {DateTime.Now:yyyy-MM-dd HH:mm:ss} by {Environment.UserName}.");

            // Save compresses all entries and writes the ZIP to the specified path.
            archive.Save(zipPath);
        }

        Console.WriteLine($"Archive created: {zipPath}");
        Console.WriteLine($"Archive size:    {new FileInfo(zipPath).Length:N0} bytes");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // Part 2: Extract the archive to a temporary directory
        // ---------------------------------------------------------------
        Console.WriteLine("=== Extracting ZIP archive ===");

        // Ensure the target directory exists
        Directory.CreateDirectory(extractDir);

        // ZipArchive.Read(path) opens an existing archive without locking
        // issues (use the stream overload when you need to save back to the
        // same file path — see the SKILL.md "Add File to an Existing Archive"
        // pattern).
        using (ZipArchive archive = ZipArchive.Read(zipPath))
        {
            foreach (ZipItem item in archive)
            {
                // item.Extract writes the entry to the specified directory,
                // recreating any subdirectory structure stored in the archive.
                item.Extract(extractDir);
                Console.WriteLine($"  Extracted: {item.Name}");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Extracted to: {extractDir}");

        // Show the contents of the extracted files
        Console.WriteLine();
        Console.WriteLine("=== Extracted file contents ===");
        foreach (string file in Directory.GetFiles(extractDir))
        {
            Console.WriteLine($"--- {Path.GetFileName(file)} ---");
            Console.WriteLine(File.ReadAllText(file));
        }

        // Cleanup temp files (optional)
        File.Delete(zipPath);
        Directory.Delete(extractDir, recursive: true);
        Console.WriteLine("Temp files cleaned up. Done.");
    }
}
