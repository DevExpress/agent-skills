# AI Agent Skills for DevExpress BI Dashboard

This plugin contains AI agent skills for [DevExpress BI Dashboard](https://www.devexpress.com/products/net/dashboard/) (a component for building interactive analytical dashboards in ASP.NET Core, Blazor, WinForms, and WPF applications).

---

## Prerequisites

- .NET 8+ (or .NET Framework 4.6.2+ for WinForms/WPF)
- DevExpress NuGet packages v26.1+
- [DevExpress Universal subscription license](https://www.devexpress.com/subscriptions/universal.xml)

---

## Included AI Agent Skills

| Skill | Capabilities | Docs |
|---|---|---|
| [devexpress-dashboard-data-sources](skills/devexpress-dashboard-data-sources/) | Registers and configures dashboard data sources in code, sets up data source storage and connection string providers, and more. Compatible with SQL, Object, Entity Framework, JSON, MongoDB, Excel, OLAP, XPO, and Federated sources. | [Overview](https://docs.devexpress.com/Dashboard/116522/basic-concepts-and-terminology/data-sources) |
| [devexpress-dashboard-export](skills/devexpress-dashboard-export/) | Exports dashboards and dashboard items, customizes or automates export tasks. Works with ASP.NET Core, Blazor, WinForms, and WPF applications and supports export to PDF, Excel, and image formats. | [Overview](https://docs.devexpress.com/Dashboard/15181/common-features/printing-and-exporting) |
| [devexpress-dashboard-parameters](skills/devexpress-dashboard-parameters/) | Creates dashboard parameters and binds them to SQL queries, stored procedures, data source filters and item filters, calculated fields, and format rules. Sets parameter values at runtime, extracts values from HTTP headers and injects them on the server side, and applies them to export tasks. Works with ASP.NET Core, Blazor, WinForms, and WPF applications.  | [Overview](https://docs.devexpress.com/Dashboard/116918/common-features/advanced-analytics/dashboard-parameters) |


---

## Skill Folder Content

Each skill is self-contained and follows the same structure:

```
devexpress-dashboard-<name>/
├── SKILL.md      -- YAML frontmatter (activators, prerequisites), navigation guide
└── references/   -- scenario-focused deep dives
```

---

For agent-specific and IDE-specific setup instructions, see the [repository README](../../README.md).
