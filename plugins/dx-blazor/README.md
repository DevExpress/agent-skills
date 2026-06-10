# AI Agent Skills for DevExpress Blazor Components

This plugin contains AI agent skills for [DevExpress Blazor Components](https://www.devexpress.com/blazor/) (UI component library for the Microsoft Blazor framework).

---

## Prerequisites

- .NET 8+ application (Blazor WebAssembly or Blazor Server)
- DevExpress v26.1+ Blazor NuGet packages  (`DevExpress.Blazor`)
- A valid [DevExpress subscription license](https://www.devexpress.com/buy/winforms-wpf-blazor-asp-net-maui/) (ASP.NET and Blazor, DXperience, Universal)
- Entity Framework Core, SQL Server, or a custom data service (for data binding tasks)
- OpenAI API key or Azure OpenAI/Ollama endpoint configuration (for AI Chat component) 

---

## Included AI Agent Skills

| Skill | Capabilities | Docs |
| ----- | ------ | ---- |
| [devexpress-blazor-ai-chat](skills/devexpress-blazor-ai-chat/)         | Blazor AI Chat: provider configuration (OpenAI, Azure OpenAI, and Ollama), prompt suggestions, system prompts, attachments, Markdown replies, streaming, tool calling, history. | [Overview](https://docs.devexpress.com/Blazor/405290/components/ai-chat) |
| [devexpress-blazor-combobox](skills/devexpress-blazor-combobox/)       | Blazor ComboBox: data binding, search and filter, data grouping, virtual scrolling, columns, templates, custom buttons, cascading combo box setup, validation. | [Overview](https://docs.devexpress.com/Blazor/405075/components/data-editors/combobox) |
| [devexpress-blazor-charts](skills/devexpress-blazor-charts/)           | Blazor Chart: series configuration (line, bar, pie, polar, and financial), data binding, axes, labels, legends, tooltips, annotations, palettes, zoom/pan, export. | [Overview](https://docs.devexpress.com/Blazor/401180/components/charts/chart) |
| [devexpress-blazor-gauges](skills/devexpress-blazor-gauges/)           | Blazor Data Visualization Components: component configuration (bar gauge, range selector, sankey, sparkline, and map), markers, routes, range selection, user interaction events, export. | [Overview](https://docs.devexpress.com/Blazor/404946/components/bar-gauge) |
| [devexpress-blazor-grid](skills/devexpress-blazor-grid/)               | Blazor Grid: server-side data processing, columns, templates, data editing, data shaping (filter, sort, and group rows), summaries, selection, paging, virtual scrolling, export, toolbar. | [Overview](https://docs.devexpress.com/Blazor/403143/components/grid) |
| [devexpress-blazor-treelist](skills/devexpress-blazor-treelist/)       | Blazor TreeList: data binding (hierarchical or self-referenced sources), data editing, data shaping (filter and sort rows), paging, selection, export, load on demand, node drag-and-drop. | [Overview](https://docs.devexpress.com/Blazor/404942/components/treelist) |
| [devexpress-blazor-pivot-table](skills/devexpress-blazor-pivot-table/) | Blazor Pivot Table: cross-tab analysis, area customization (row, column, data, and filter areas), aggregation, date grouping, field layout, interactive data filtering. | [Overview](https://docs.devexpress.com/Blazor/405245/components/pivot-table) |
| [devexpress-blazor-scheduler](skills/devexpress-blazor-scheduler/)     | Blazor Scheduler: view management (day, week, month, and timeline views), appointment binding, recurring events, resources, data editing, drag-and-drop, labels, statuses. | [Overview](https://docs.devexpress.com/Blazor/401179/components/scheduler) |
| [devexpress-blazor-ribbon](skills/devexpress-blazor-ribbon/)           | Blazor Ribbon: tabs, groups, application menu, contextual tabs, buttons, toggles, color pickers, combo boxes, spin editors. | [Overview](https://docs.devexpress.com/Blazor/405288/components/navigation-controls/ribbon) |
| [devexpress-blazor-toolbar](skills/devexpress-blazor-toolbar/)         | Blazor Toolbar: buttons, drop-down menus, checkboxes and radio buttons, data binding, adaptivity, render styles, templates, icons, links. | [Overview](https://docs.devexpress.com/Blazor/405063/components/navigation-controls/toolbar) |

---

## Skill Folder Contents

Each skill folder contains the following files and sub-directories:

- `SKILL.md` - activators, rules, and navigation guidance
- `references/` - feature-specific implementation guides
- `examples/*.razor` - getting-started and task-based samples

---

For agent-specific and IDE-specific setup instructions, see the [repository README](../../README.md).

