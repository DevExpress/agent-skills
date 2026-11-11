# AI Agent Skills for DevExpress VCL Components (Delphi and C++Builder)

This plugin contains AI agent skills for [DevExpress VCL Components](https://www.devexpress.com/products/vcl/). These skills equip AI coding assistants with verified component knowledge, valid DFM structure templates, API patterns, and runtime troubleshooting rules.

Skills follow the open Agent Skills standard and work with [Kai for RAD Studio](https://www.embarcadero.com/products/rad-studio/kai), Claude Code, GitHub Copilot, Cursor, Gemini CLI, and other modern AI coding assistants.

## Prerequisites

- [Embarcadero RAD Studio](https://www.embarcadero.com/products/rad-studio) 12.3 Athens or later (Community Edition is not supported)
- [DevExpress VCL Components for Delphi and C++Builder](https://www.devexpress.com/products/vcl/) v26.1+
- [DevExpress VCL Subscription](https://www.devexpress.com/buy/vcl-delphi-cbuilder/) ([ExpressReports](https://www.devexpress.com/products/vcl/reports/) and [ExpressDashboards](https://www.devexpress.com/products/vcl/dashboard/) require [VCL Subscription +](https://www.devexpress.com/buy/vcl-delphi-cbuilder/)).
- [DevExpress Documentation MCP Server](https://docs.devexpress.com/VCL/406057) (recommended)

> **NOTE**  
> To use [ExpressReports](https://www.devexpress.com/products/vcl/reports/) and [ExpressDashboards](https://www.devexpress.com/products/vcl/dashboard/), your development environment must meet the requirements outlined in the following help topic section: [Prerequisites & Deployment](https://docs.devexpress.com/VCL/405469/ExpressReports/vcl-reports#prerequisites--deployment).

## DevExpress Documentation MCP Server

The skills use the public DevExpress Documentation MCP server to retrieve details not covered locally, such as unit names, API signatures, enum values, and component properties:

- **Name:** `dxdocs`
- **URL:** `https://api.devexpress.com/mcp/docs`
- **Transport:** Streamable HTTP, no authentication

If you have already configured an MCP server that exposes `devexpress_docs_search` and `devexpress_docs_get_content` tools, the skills will automatically use it and no extra configuration is necessary. Otherwise, register the public server. Refer to the following help topics for step-by-step instructions:

- **Kai for RAD Studio:** [DevExpress VCL: Documentation MCP Server](https://docs.devexpress.com/VCL/406057)
- **Other AI Assistants:** [DevExpress MCP Server: Configure Your AI-powered Coding Assistant](https://docs.devexpress.com/GeneralInformation/405551)

## Included AI Agent Skills

### VCL Reports

Skill: [devexpress-vcl-reports](skills/devexpress-vcl-reports/)

| Class | Purpose | Documentation |
| ----- | ------- | ------------- |
| `TdxReport` | Core report component: manages report layouts, data bindings, Preview and Designer dialogs, and headless (non-UI) export. | [Reference](https://docs.devexpress.com/VCL/dxReport.TdxReport) |
| `TdxReportControl` | Report viewer embedded in a non-modal form, user control, tab page, or MDI child. | [Reference](https://docs.devexpress.com/VCL/dxReport.Control.TdxReportControl) |
| `TdxBackendDataConnectionManager` | Centralized container that manages data connections used by reports. | [Reference](https://docs.devexpress.com/VCL/dxBackend.TdxBackendDataConnectionManager) |
| `TdxBackendDatabaseSQLConnection` | SQL database connection supporting XPO-powered providers (SQL Server, SQLite, etc.) | [Reference](https://docs.devexpress.com/VCL/dxBackend.ConnectionString.SQL.TdxBackendDatabaseSQLConnection) |
| `TdxBackendInMemoryJSONConnection` | In-memory JSON data connection that loads data from a URI, Web API endpoint, or raw string. | [Reference](https://docs.devexpress.com/VCL/dxBackend.ConnectionString.JSON.TdxBackendInMemoryJSONConnection) |
| `TdxBackendDataSetJSONConnection` | Bridges native VCL `TDataSet` sources (FireDAC, TdxMemData, etc.) to reports as JSON. | [Reference](https://docs.devexpress.com/VCL/dxBackend.ConnectionString.JSON.DataSet.TdxBackendDataSetJSONConnection) |

### Data Grids, Editors, Tree List, Scheduler, Layout Management, and Other VCL Components

Coming Soon...

## Structure

Each skill is self-contained and follows the same structure:

```
skills/devexpress-vcl-<name>/
├── SKILL.md                      # Entry point: activation triggers, core API reference, and quick start
├── references/*.md               # On-demand reference guides
│   ├── design-time-dfm.md        # Verified DFM sample for this suite
│   └── dfm-format-rules.md       # Shared rules: DFM syntax, form-unit contracts, and load-error handling
├── examples/                     # Code samples (Delphi and C++Builder)
└── scripts/                      # Automation and validation utilities
```

For agent-specific and IDE-specific setup instructions, see the [repository README](../../README.md).
