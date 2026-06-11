#  AI Agent Skills for DevExpress Reports

This plugin contains AI agent skills for [DevExpress Reports](https://www.devexpress.com/products/net/reporting/) (a feature-complete reporting platform that ships with an intuitive IDE-integrated Report Designer and runtime Report Designers/Viewers for ASP.NET, JavaScript, Blazor, WPF, and WinForms).

All released skills target DevExpress Reports v26.1.

---

## Prerequisites

- .NET 8+ or .NET Framework 4.6.2+ application 
- DevExpress Reports NuGet package v26.1+ (`DevExpress.XtraReports`)
- [DevExpress subscription license](https://www.devexpress.com/buy/winforms-wpf-blazor-asp-net-maui/) (Reporting, ASP.NET and Blazor, WinForms, WPF, DXperience, Universal)
- Static assets and middleware registration in the host application (for web designer/viewer tasks)  

---

## Included AI Agent Skills

| Skill | Description | Docs |
|---|-----|---|
| [devexpress-reports-core](skills/devexpress-reports-core/) | Cross-platform runtime API — reports, bands, controls, expressions, data binding, and export | [Overview](https://docs.devexpress.com/XtraReports/119097/feature-guide-to-devexpress-reports/reporting-api) |
| [devexpress-reports-aspnetcore](skills/devexpress-reports-aspnetcore/) | ASP.NET Core Integration — service registration, viewer/designer wiring, report storage, and export/print flows | [Overview](https://docs.devexpress.com/XtraReports/119717/web-reporting/aspnet-core-reporting) |
| [devexpress-reports-blazor](skills/devexpress-reports-blazor/) | Blazor Integration — viewer/designer setup (native or JavaScript), backend service management, report resolution, and customization hooks | [Overview](https://docs.devexpress.com/XtraReports/401676/web-reporting/blazor-reporting) |
| [devexpress-visual-studio-report-design](skills/devexpress-visual-studio-report-design/) | Visual Studio Designer code-behind patterns (`*.Designer.cs`) — safe `InitializeComponent` edits, band/control composition, expressions, parameters, and serialization-safe patterns. | [Overview](https://docs.devexpress.com/XtraReports/4256/report-designer-ides/report-designer-for-visual-studio/report-designer-for-visual-studio) |

---

## Included AI Agents

| Agent | Description |
|---|---|
| [devexpress-report-designer-expert](agents/devexpress-report-designer-expert.agent.md) | Expert agent for Visual Studio Report Designer workflows (`*.Designer.cs`) — safe `InitializeComponent` edits, band/control composition, expressions, parameters, and serialization-safe patterns. |

---

## Skill Folder Contents

Each skill is self-contained and follows the same structure:

```
devexpress-reports-<name>/
├── SKILL.md      -- YAML frontmatter (activators, prerequisites), navigation guide
├── references/   -- scenario-focused deep dives 
└── examples/     -- getting-started samples
```

---

For agent-specific and IDE-specific setup instructions, see the [repository README](../../README.md).
