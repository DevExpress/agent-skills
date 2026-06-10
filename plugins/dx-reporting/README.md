# DevExpress Reports Skills

AI agent skills for [DevExpress Reports](https://www.devexpress.com/products/net/reporting/) -- XtraReports runtime APIs, web integration for ASP.NET Core and Blazor, and Visual Studio report designer code patterns.

All released skills target DevExpress Reports v26.1.

---

## Skills

| Skill | Covers | Docs |
|---|-----|---|
| [devexpress-reports-core](devexpress-reports-core/) | Platform-neutral XtraReports runtime API — report creation in code, bands/controls, expressions, data binding, and export workflows | [Overview](https://docs.devexpress.com/XtraReports/119097/feature-guide-to-devexpress-reports/reporting-api) |
| [devexpress-reports-aspnetcore](devexpress-reports-aspnetcore/) | ASP.NET Core integration — service registration, viewer/designer wiring, report storage, and export/print flows | [Overview](https://docs.devexpress.com/XtraReports/119717/web-reporting/aspnet-core-reporting) |
| [devexpress-reports-blazor](devexpress-reports-blazor/) | Blazor integration — native and JS viewer/designer setup, backend services, report resolution, and customization hooks | [Overview](https://docs.devexpress.com/XtraReports/401676/web-reporting/blazor-reporting) |
| [devexpress-visual-studio-report-design](devexpress-visual-studio-report-design/) | Visual Studio designer code-behind patterns — `*.Designer.cs` structure, `InitializeComponent` order, bands/controls, expressions, parameters, and serialization-safe edits | [Overview](https://docs.devexpress.com/XtraReports/4256/report-designer-ides/report-designer-for-visual-studio/report-designer-for-visual-studio) |

---

## Agents

| Agent | Description |
|---|---|
| [devexpress-report-designer-expert](agents/devexpress-report-designer-expert.agent.md) | Expert agent for Visual Studio `XtraReport` designer workflows (`*.Designer.cs`): safe `InitializeComponent` edits, band/control composition, expressions, parameters, and serialization-safe patterns. |

---

## Skill Layout

Each skill is self-contained and follows the same structure:

```
devexpress-reports-<name>/
├── SKILL.md      -- YAML frontmatter (activators, prerequisites), navigation guide
├── references/   -- scenario-focused deep dives (viewer setup, storage, customization, ...)
└── examples/     -- runnable quickstart/sample project files
```

---

## Prerequisites

- **.NET 8+** or **.NET Framework 4.6.2+** application targets appropriate for your host stack (ASP.NET Core or Blazor for web skills)
- **DevExpress Reports NuGet packages** (`DevExpress.XtraReports`)
- A valid **DevExpress license**
- For web designer/viewer scenarios -- required static assets and middleware registration in the host application


