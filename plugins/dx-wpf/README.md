# AI Agent Skills for DevExpress WPF Controls

This plugin contains AI agent skills for [DevExpress WPF Controls](https://www.devexpress.com/products/net/controls/wpf/) (Windows Presentation Foundation controls and frameworks for .NET 8+ and .NET Framework 4.6.2+).

---

## Prerequisites

- .NET 8+ (`net8.0-windows`) or .NET Framework 4.6.2+
- DevExpress WPF NuGet packages v26.1+ (`DevExpress.Wpf.Core` and feature-specific packages)
- [DevExpress subscription license](https://www.devexpress.com/buy/winforms-wpf-blazor-asp-net-maui/) (WPF Subscription, DXperience Subscription, or Universal Subscription)
- Project SDK `Microsoft.NET.Sdk.Razor` and the `WebView2` runtime (for `devexpress-wpf-ai-chat-control`)
- `dx:ThemedWindow` host window instead of `System.Windows.Window` (for Ribbon, Tab Control, and AI Chat Control)

---

## Included AI Agent Skills

### Data-aware Controls

| Skill | Capabilities | Docs |
|---|---|---|
| [devexpress-wpf-data-grid](skills/devexpress-wpf-data-grid/)       | `GridControl` — view manageent (TableView, CardView, TreeListView), data binding (EF Core, XPO, server mode), columns, data shaping (sort, filter, group rows), summaries, master-detail layout, conditional formatting, printing, and export | [DataGrid Documentation](https://docs.devexpress.com/WPF/6084/controls-and-libraries/data-grid)         |
| [devexpress-wpf-tree-list](skills/devexpress-wpf-tree-list/)       | `TreeListControl` — data binding (self-referential and hierarchical sources), unbound mode, drag-and-drop settings, row selection, edit forms, validation | [TreeList Documentation](https://docs.devexpress.com/WPF/9759/controls-and-libraries/tree-list)         |
| [devexpress-wpf-pivot-grid](skills/devexpress-wpf-pivot-grid/)     | `PivotGridControl` — area management (row, column, data, and filter areas), OLAP, server mode, aggregation, drill-down, KPI, conditional formatting | [PivotGrid Documentation](https://docs.devexpress.com/WPF/7228/controls-and-libraries/pivot-grid)       |
| [devexpress-wpf-property-grid](skills/devexpress-wpf-property-grid/) | `PropertyGridControl` — selected object, properties, collections, categories, expandable nested types | [PropertyGrid Documentation](https://docs.devexpress.com/WPF/15640/controls-and-libraries/property-grid) |
| [devexpress-wpf-charts](skills/devexpress-wpf-charts/) | `ChartControl` (2D) — diagram management (XY, polar, radar, simple), 15+ series types, primary and secondary axes, legend, tooltip, crosshair, aggregation | [Chart Documentation](https://docs.devexpress.com/WPF/115092/controls-and-libraries/charts-suite) |
| [devexpress-wpf-scheduler](skills/devexpress-wpf-scheduler/) | `SchedulerControl` — view type management (day, work week, week, month, timeline, agenda, list), appointments, resources, labels, statuses, time regions, recurrence, reminders, time zones | [Scheduler Documentation](https://docs.devexpress.com/WPF/114881/controls-and-libraries/scheduler) |
| [devexpress-wpf-ai-chat-control](skills/devexpress-wpf-ai-chat-control/) | `AIChatControl` — Copilot-style chat UI, provider settings (Azure OpenAI, OpenAI, Ollama, Semantic Kernel), streaming, markdown formatting, file upload, prompt suggestions, history | [AI Chat Control Documentation](https://docs.devexpress.com/WPF/405434/ai-powered-extensions/ai-chat-control) |
---

### Data Editors and Utilities

| Skill | Capabilities | Docs |
|---|---|---|
| [devexpress-wpf-data-editors](skills/devexpress-wpf-data-editors/) | 30+ editors (`TextEdit`, `ButtonEdit`, `ComboBoxEdit`, `DateEdit`, `SpinEdit`, `LookUpEdit`, `PasswordBoxEdit`, `ColorEdit`, `RatingEdit`, `BarCodeEdit`, and more), all-purpose UI controls (`SimpleButton`, `DropDownButton`, `FlyoutControl`, `RangeControl`, `Calculator`) | [Editors & Combo Box Documentation](https://docs.devexpress.com/WPF/6190/controls-and-libraries/data-editors) |
| [devexpress-wpf-loading-indicators](skills/devexpress-wpf-loading-indicators/) | `SplashScreenManager`, `LoadingDecorator`, `WaitIndicator` — decision guide for picking the right indicato,; migration from legacy `DXSplashScreen` | [Loading Indicators Documentation](https://docs.devexpress.com/WPF/115521/controls-and-libraries/windows-and-utility-controls) |

---

### Layout & Navigation

| Skill | Capabilities | Docs |
|---|---|---|
| [devexpress-wpf-layout-management](skills/devexpress-wpf-layout-management/) | Six layout containers (`DockLayoutManager`, `LayoutControl`, `DataLayoutControl`, `TileLayoutControl`, `FlowLayoutControl`, `DockLayoutControl`), layout persistence | [Layout Management Documentation](https://docs.devexpress.com/WPF/115547/controls-and-libraries/layout-management) |
| [devexpress-wpf-ribbon-and-bars](skills/devexpress-wpf-ribbon-and-bars/) | Office-style command controls (Ribbon, toolbar, main menu, status bar), Quick Access Toolbar, Backstage View, MDI merging | [Ribbon and Bar Manager Documentation](https://docs.devexpress.com/WPF/115392/controls-and-libraries/ribbon-bars-and-menu) |
| [devexpress-wpf-accordion](skills/devexpress-wpf-accordion/) | `AccordionControl` — hierarchical sidebar, Navigation Pane mode, built-in search, collapsed strip (glyphs only) | [Accordion Documentation](https://docs.devexpress.com/WPF/118347/controls-and-libraries/navigation-controls/accordion-control?p=netframework) |
| [devexpress-wpf-tab-control](skills/devexpress-wpf-tab-control/) | `DXTabControl` — tab mode management (multiLine, scroll, stretch), drag-and-drop tab reordering, accent colors, close and pin tabs | [Tab Control Documentation](https://docs.devexpress.com/WPF/7974/controls-and-libraries/layout-management/tab-control) |

---

### Frameworks

| Skill | Capabilities | Docs |
|---|---|---|
| [devexpress-wpf-mvvm](skills/devexpress-wpf-mvvm/) | View-model strategies (`GenerateViewModel` source generator, `ViewModelSource`, `ViewModelBase`, `BindableBase`), `DelegateCommand` and `AsyncCommand`, 25+ predefined services (`IMessageBoxService`, `IDialogService`, `IDocumentManagerService`, `INotificationService`, and more), behaviors (`EventToCommand`, `KeyToCommand`, `FocusBehavior`, and more), `Messenger` | [Overview](https://docs.devexpress.com/WPF/15112/mvvm-framework) |

---

## Skill Folder Contents

Each skill is self-contained and follows the same structure:

```
<group>/devexpress-wpf-<name>/
├── SKILL.md       — YAML frontmatter (activators, prerequisites), navigation guide
├── references/    — topic-focused deep dives (data binding, editing, export, …)
└── examples/      — getting-started samples (XAML + C# + App startup)
```

---

For agent-specific and IDE-specific setup instructions, see the [repository README](../../README.md).
