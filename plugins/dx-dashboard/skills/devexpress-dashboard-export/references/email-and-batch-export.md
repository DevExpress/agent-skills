# Email Export & Batch/Merged PDF Export

## When to Use This Reference

Use this when you need to:
- Email a dashboard export (scheduled report, alert, distribution list)
- Send different recipients different **filtered slices** of the same dashboard in one export/send loop
- Export every dashboard item to a single merged PDF (one item per export call, then combine)

**The two patterns use different exporters — pick by where the code runs:**

| Pattern | Exporter | Runs in |
|---|---|---|
| Email export (below) | `DashboardExporter` (`DevExpress.DashboardCommon`, NuGet `DevExpress.Dashboard.Core`) | Console app, worker service, scheduled job — no web server or UI control |
| Merged/batch PDF (below) | `WebDashboardExporter` (`DevExpress.DashboardWeb`) or `AspNetCoreDashboardExporter` (`DevExpress.DashboardAspNetCore`), NuGet `DevExpress.AspNetCore.Dashboard` | Any ASP.NET Core context that can supply a `DashboardConfigurator` — the sample below uses a controller and resolves it from DI |

The merging technique itself (`PdfDocumentProcessor.CreateEmptyDocument` + `AppendDocument`) is exporter-agnostic. To merge from a console app, keep the loop and swap the exporter — the argument positions are identical, only the first argument's meaning changes (web: `dashboardId`; non-visual: `dashboardXmlPath`):

```csharp
using DevExpress.DashboardCommon; // Dashboard, DashboardItem, DashboardExporter
using DevExpress.Pdf;             // PdfDocumentProcessor (NuGet: DevExpress.Document.Processor)

// Web (WebDashboardExporter):     ExportDashboardItemToPdf(dashboardId,      itemName, stream, size, state, options)
// Non-visual (DashboardExporter): ExportDashboardItemToPdf(dashboardXmlPath, itemName, stream, size, state, options)
const string dashboardPath = "Data/Dashboards/sales.xml";

Dashboard dashboard = new Dashboard();
dashboard.LoadFromXml(dashboardPath); // loaded only to enumerate item names — no DashboardStorage needed

DashboardExporter exporter = new DashboardExporter();
MemoryStream resultStream = new MemoryStream();
using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor()) {
    documentProcessor.CreateEmptyDocument(resultStream);
    foreach (DashboardItem item in dashboard.Items) {
        using (MemoryStream stream = new MemoryStream()) {
            // item.ComponentName — the component name, never the display name (Rule 8)
            // size / state / options are optional — omit them to keep the dashboard's own size
            exporter.ExportDashboardItemToPdf(dashboardPath, item.ComponentName, stream);
            stream.Seek(0, SeekOrigin.Begin);
            documentProcessor.AppendDocument(stream);
        }
    }
}
// resultStream now holds the merged PDF
```

`using` directives: `DevExpress.DashboardCommon` (`Dashboard`, `DashboardItem`, `DashboardExporter`) and `DevExpress.Pdf` (`PdfDocumentProcessor`).

> **`PdfDocumentProcessor` ships with the `DevExpress.Document.Processor` NuGet package** — the same package as `Workbook`. `DevExpress.Pdf.Core` / `DevExpress.Pdf.Drawing` alone do **not** expose `PdfDocumentProcessor`; a project referencing only those will not compile this code. Verify the reference (Rule 6) before generating code.
>
> **On non-SDK-style projects (assembly references, .NET Framework)** the reference to check is `DevExpress.Docs.vX.Y.dll`. If it is missing, ask the developer to add it — do not attempt to add or fix the reference yourself, and do not proceed with `PdfDocumentProcessor` code until it's confirmed present.

---

## Email a Dashboard Export (MailKit)

Additional NuGet: `MailKit`

Never hardcode SMTP hosts, credentials, or addresses — read them from configuration (`appsettings.json` + user secrets / environment variables in production):

```csharp
using DevExpress.DashboardCommon;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

// appsettings.json: { "Smtp": { "Host": "...", "Port": 587, "User": "...", "From": "...", "To": "..." } }
// Keep the password OUT of appsettings.json — use user secrets or an environment variable.
var smtp = configuration.GetSection("Smtp");
string smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
    ?? throw new InvalidOperationException("SMTP_PASSWORD is not set");

var exporter = new DashboardExporter();

using (MimeMessage mail = new MimeMessage()) {
    mail.From.Add(new MailboxAddress("Reports", smtp["From"]));
    mail.To.Add(new MailboxAddress("Sales Team", smtp["To"]));
    mail.Subject = "Weekly Sales Dashboard";

    var builder = new BodyBuilder { TextBody = "See the attached dashboard export." };

    using (MemoryStream stream = new MemoryStream()) {
        exporter.ExportToPdf(
            dashboardXmlPath: "Data/Dashboards/SalesDashboard.xml",
            outputStream: stream,
            dashboardSize: new System.Drawing.Size(2000, 1000));
        stream.Seek(0, SeekOrigin.Begin);
        builder.Attachments.Add("Dashboard.pdf", stream.ToArray(), new ContentType("application", "pdf"));
    }
    mail.Body = builder.ToMessageBody();

    using (var client = new SmtpClient()) {
        client.Connect(smtp["Host"], int.Parse(smtp["Port"] ?? "587"), SecureSocketOptions.StartTls);
        client.Authenticate(smtp["User"], smtpPassword);
        client.Send(mail); // async variant: await client.SendAsync(mail) inside an async method
        client.Disconnect(true);
    }
}
```

### Send a Different Filtered Slice to Each Recipient

Build a `DashboardState` with `DashboardItemState.MasterFilterValues` per recipient before exporting — one export/send loop, different data per email:

```csharp
using DevExpress.DashboardCommon;

foreach (var team in teams) { // team.TeamMail, team.TeamRegions (List<string>)
    DashboardState regionFilterState = new DashboardState();
    DashboardItemState itemState = new DashboardItemState("listBoxDashboardItem1"); // the master-filter item's component name
    foreach (string region in team.TeamRegions) {
        itemState.MasterFilterValues.Add(new object[] { region });
    }
    regionFilterState.Items.Add(itemState);

    using (MemoryStream stream = new MemoryStream()) {
        exporter.ExportToPdf("Data/Dashboards/SalesDashboard.xml", stream,
            new System.Drawing.Size(2000, 1000), regionFilterState);
        stream.Seek(0, SeekOrigin.Begin);
        // attach stream.ToArray() and send to team.TeamMail as above
    }
}
```

`DashboardState.Items` (not `ItemsState`) holds a `DashboardItemState` per item; `MasterFilterValues` is a `List<object[]>` — each `object[]` is one selected row, in the same column order as the item's dimensions (same convention as `MasterFilterSetEventArgs.SelectedValues`).

> Always subscribe to `exporter.ConnectionError`, `exporter.DataLoadingError`, and `exporter.DashboardItemDataLoadingError` in a scheduled/unattended export — they are the only way to see data failures since there is no UI to report them.

---

## Export Every Dashboard Item to One Merged PDF

Useful when items need to appear on separate pages (e.g. one item per page, or the same dashboard exported once per filter state) but the recipient wants a single file. Merge with `PdfDocumentProcessor` (namespace `DevExpress.Pdf`) — export each piece to its own stream, then append. Package and reference requirements: see the callout in **When to Use This Reference** above.

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using DevExpress.Pdf;

[HttpGet]
public FileContentResult ExportAllItems([FromServices] DashboardConfigurator dashboardConfigurator,
    string dashboardId, string state) {
    // DashboardConfigurator.DashboardStorage.LoadDashboard(id) returns the dashboard XML (XDocument)
    Dashboard dashboard = new Dashboard();
    dashboard.LoadFromXDocument(dashboardConfigurator.DashboardStorage.LoadDashboard(dashboardId));

    MemoryStream resultStream = new MemoryStream();
    using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor()) {
        documentProcessor.CreateEmptyDocument(resultStream);
        for (int i = 0; i < dashboard.Items.Count; i++) {
            string dashboardItemName = dashboard.Items[i].ComponentName;
            DashboardState dashboardState = new DashboardState();
            dashboardState.LoadFromJson(state);

            // AspNetCoreDashboardExporter also works here but requires IWebHostEnvironment in its constructor
            WebDashboardExporter exporter = new WebDashboardExporter(dashboardConfigurator);
            using (MemoryStream stream = new MemoryStream()) {
                exporter.ExportDashboardItemToPdf(dashboardId, dashboardItemName, stream,
                    new System.Drawing.Size(1024, 768), dashboardState);
                stream.Seek(0, SeekOrigin.Begin);
                documentProcessor.AppendDocument(stream);
            }
        }
    }
    return File(resultStream.ToArray(), "application/pdf", $"{dashboardId}.pdf");
}
```

`documentProcessor.CreateEmptyDocument(stream)` starts the merged output; `AppendDocument(stream)` appends each per-item export's pages onto the end. Same pattern applies to exporting one dashboard multiple times with different `DashboardState` (e.g. one filter value per page) instead of looping items.
