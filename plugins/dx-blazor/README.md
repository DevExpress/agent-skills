# DevExpress Blazor Skills

AI agent skills for [DevExpress Blazor](https://www.devexpress.com/blazor/) — a UI component library for the Microsoft Blazor framework.

All skills target DevExpress Blazor v26.1. Some APIs and examples require DevExpress Blazor 26.1 or later and will not work in earlier versions.

---

## Skills

| Skill | Covers | Docs |
| ----- | ------ | ---- |
| [devexpress-blazor-ai-chat](skills/devexpress-blazor-ai-chat/)         | Blazor AI Chat: OpenAI, Azure OpenAI, and Ollama providers; prompt suggestions, system prompts, attachments, Markdown replies, streaming, tool calling, history. | [Overview](https://docs.devexpress.com/Blazor/405290/components/ai-chat) |
| [devexpress-blazor-combobox](skills/devexpress-blazor-combobox/)       | Blazor ComboBox: data binding, search and filtering, grouping, virtual scrolling, multiple columns, templates, custom buttons, cascading, validation. | [Overview](https://docs.devexpress.com/Blazor/405075/components/data-editors/combobox) |
| [devexpress-blazor-charts](skills/devexpress-blazor-charts/)           | Blazor Chart: line, bar, pie, polar, and financial series; data binding, axes, labels, legends, tooltips, annotations, palettes, zoom/pan, export. | [Overview](https://docs.devexpress.com/Blazor/401180/components/charts/chart) |
| [devexpress-blazor-gauges](skills/devexpress-blazor-gauges/)           | Blazor Visualization Components: bar gauge, range selector, sankey, sparkline, and map components; markers, routes, range selection, hover and click events, export. | [Overview](https://docs.devexpress.com/Blazor/404946/components/bar-gauge) |
| [devexpress-blazor-grid](skills/devexpress-blazor-grid/)               | Blazor Grid: columns, templates, editing, filtering, sorting, grouping, selection, paging, virtual scrolling, summaries, export, toolbar, server data. | [Overview](https://docs.devexpress.com/Blazor/403143/components/grid) |
| [devexpress-blazor-treelist](skills/devexpress-blazor-treelist/)       | Blazor TreeList: hierarchical data, parent-child binding, editing, filtering, sorting, paging, selection, export, load on demand, node drag-and-drop. | [Overview](https://docs.devexpress.com/Blazor/404942/components/treelist) |
| [devexpress-blazor-pivot-table](skills/devexpress-blazor-pivot-table/) | Blazor Pivot Table: cross-tab analysis, row, column, data, and filter areas, aggregation, date grouping, field layout, interactive filtering. | [Overview](https://docs.devexpress.com/Blazor/405245/components/pivot-table) |
| [devexpress-blazor-scheduler](skills/devexpress-blazor-scheduler/)     | Blazor Scheduler: day, week, month, and timeline views; appointment binding, recurring events, resources, editing, drag-and-drop, labels, statuses. | [Overview](https://docs.devexpress.com/Blazor/401179/components/scheduler) |
| [devexpress-blazor-ribbon](skills/devexpress-blazor-ribbon/)           | Blazor Ribbon: tabs, groups, application menu, contextual tabs, buttons, toggles, color pickers, combo boxes, spin editors. | [Overview](https://docs.devexpress.com/Blazor/405288/components/navigation-controls/ribbon) |
| [devexpress-blazor-toolbar](skills/devexpress-blazor-toolbar/)         | Blazor Toolbar: buttons, drop-down menus, checked and radio items, data binding, adaptivity, render styles, templates, icons, links. | [Overview](https://docs.devexpress.com/Blazor/405063/components/navigation-controls/toolbar) |

---

## Skill Layout

Each skill folder contains:

- `SKILL.md` - activators, rules, and navigation guidance
- `references/` - feature-specific implementation guides
- `examples/*.razor` - runnable quickstart and scenario-specific samples

---

## Prerequisites

- **Blazor WebAssembly** or **Blazor Server** application with **.NET 8+**
- **DevExpress Blazor NuGet packages** (`DevExpress.Blazor`)
- A valid **DevExpress license** (Blazor Subscription, DXperience, or Universal Subscription)
- For data binding scenarios: Entity Framework Core, SQL Server, or custom data services
- For AI Chat component: OpenAI API key or Azure OpenAI/Ollama endpoint configuration

