# DevExpress WPF Skills

AI agent skills for [DevExpress WPF](https://www.devexpress.com/products/net/controls/wpf/) — Windows Presentation Foundation controls and frameworks for .NET 8+ and .NET Framework 4.6.2+.

All skills target DevExpress v26.1.

---

## Skills

### Data-aware controls

| Skill | Covers | Docs |
|---|---|---|
| [wpf-devexpress-data-grid](wpf-devexpress-data-grid/)       | `GridControl` — TableView / CardView / TreeListView, data binding (EF Core, XPO, server mode), columns, sorting, filtering, grouping, summaries, master-detail, conditional formatting, printing & export | [DataGrid Documentation](https://docs.devexpress.com/WPF/6084/controls-and-libraries/data-grid)         |
| [wpf-devexpress-tree-list](wpf-devexpress-tree-list/)       | `TreeListControl` — self-referential / hierarchical / unbound modes, drag-and-drop, multi-selection, edit forms, validation | [TreeList Documentation](https://docs.devexpress.com/WPF/9759/controls-and-libraries/tree-list)         |
| [wpf-devexpress-pivot-grid](wpf-devexpress-pivot-grid/)     | `PivotGridControl` — Row / Column / Data / Filter areas, OLAP, server mode, aggregation, drill-down, KPI, conditional formatting | [PivotGrid Documentation](https://docs.devexpress.com/WPF/7228/controls-and-libraries/pivot-grid)       |
| [wpf-devexpress-property-grid](wpf-devexpress-property-grid/) | `PropertyGridControl` — `SelectedObject(s)`, `PropertyDefinition`, `CollectionDefinition`, `CategoryDefinition`, expandable nested types | [PropertyGrid Documentation](https://docs.devexpress.com/WPF/15640/controls-and-libraries/property-grid) |

---

### Editors & input

| Skill | Covers | Docs |
|---|---|---|
| [wpf-devexpress-data-editors](wpf-devexpress-data-editors/) | 30+ editors — `TextEdit`, `ButtonEdit`, `ComboBoxEdit`, `DateEdit`, `SpinEdit`, `LookUpEdit`, `PasswordBoxEdit`, `ColorEdit`, `RatingEdit`, `BarCodeEdit`, and more — plus simple controls (`SimpleButton`, `DropDownButton`, `FlyoutControl`, `RangeControl`, `Calculator`) | [Editors & Combo Box Documentation](https://docs.devexpress.com/WPF/6190/controls-and-libraries/data-editors) |

---

### Layout & navigation

| Skill | Covers | Docs |
|---|---|---|
| [wpf-devexpress-layout-management](wpf-devexpress-layout-management/) | Six layout containers — `DockLayoutManager`, `LayoutControl`, `DataLayoutControl`, `TileLayoutControl`, `FlowLayoutControl`, `DockLayoutControl`; layout persistence | [Layout Management Documentation](https://docs.devexpress.com/WPF/115547/controls-and-libraries/layout-management) |
| [wpf-devexpress-ribbon-and-bars](wpf-devexpress-ribbon-and-bars/) | `RibbonControl`, toolbars, menus — Office-style ribbon, `ToolBarControl` / `MainMenuControl` / `StatusBarControl`, `BarManager`, Quick Access Toolbar, BackstageView, MDI merging | [Ribbon and Bar Manager Documentation](https://docs.devexpress.com/WPF/115392/controls-and-libraries/ribbon-bars-and-menu) |
| [wpf-devexpress-accordion](wpf-devexpress-accordion/) | `AccordionControl` — hierarchical sidebar, Navigation Pane mode, built-in search, collapsed glyph-only strip | [Accordion Documentation](https://docs.devexpress.com/WPF/118347/controls-and-libraries/navigation-controls/accordion-control?p=netframework) |
| [wpf-devexpress-tab-control](wpf-devexpress-tab-control/) | `DXTabControl` — MultiLine / Scroll / Stretch views, drag-drop reordering, accent colors, close / pin | [Tab Control Documentation](https://docs.devexpress.com/WPF/7974/controls-and-libraries/layout-management/tab-control) |

---

### Visualization & specialized

| Skill | Covers | Docs |
|---|---|---|
| [wpf-devexpress-charts](wpf-devexpress-charts/) | `ChartControl` (2D) — XY / Polar / Radar / Simple diagrams, 15+ series types, primary/secondary axes, legend, tooltip, crosshair, aggregation | [Chart Documentation](https://docs.devexpress.com/WPF/115092/controls-and-libraries/charts-suite) |
| [wpf-devexpress-scheduler](wpf-devexpress-scheduler/) | `SchedulerControl` — seven view types (Day / Work Week / Week / Month / Timeline / Agenda / List), appointments, resources, labels, statuses, time regions, recurrence, reminders, time zones | [Scheduler Documentation](https://docs.devexpress.com/WPF/114881/controls-and-libraries/scheduler) |
| [wpf-devexpress-loading-indicators](wpf-devexpress-loading-indicators/) | `SplashScreenManager`, `LoadingDecorator`, `WaitIndicator` — decision guide for picking the right indicator; migration from legacy `DXSplashScreen` | [Loading Indicators Documentation](https://docs.devexpress.com/WPF/115521/controls-and-libraries/windows-and-utility-controls) |
| [wpf-devexpress-ai-chat-control](wpf-devexpress-ai-chat-control/) | `AIChatControl` — Copilot-style chat UI; Azure OpenAI / OpenAI / Ollama / Semantic Kernel via `IChatClient`; streaming, Markdown, file upload, prompt suggestions, history | [AI Chat Control Documentation](https://docs.devexpress.com/WPF/405434/ai-powered-extensions/ai-chat-control) |

---

### Cross-cutting

| Skill | Covers | Docs |
|---|---|---|
| [wpf-devexpress-mvvm](wpf-devexpress-mvvm/) | View-model strategies (`[GenerateViewModel]` source generator, `ViewModelSource`, `ViewModelBase`, `BindableBase`); `DelegateCommand` / `AsyncCommand`; 25+ predefined services (`IMessageBoxService`, `IDialogService`, `IDocumentManagerService`, `INotificationService`, …); behaviors (`EventToCommand`, `KeyToCommand`, `FocusBehavior`, …); `Messenger` | [Overview](https://docs.devexpress.com/WPF/15112/mvvm-framework) |

---

## Skill Layout

Each skill is self-contained and follows the same structure:

```
<group>/wpf-devexpress-<name>/
├── SKILL.md       — YAML frontmatter (activators, prerequisites), navigation guide
├── references/    — topic-focused deep dives (data binding, editing, export, …)
└── examples/      — runnable quickstart (XAML + C# + App startup)
```

---

## Prerequisites

- **.NET 8+** (`net8.0-windows`) or **.NET Framework 4.6.2+**
- **DevExpress NuGet packages** (`DevExpress.Wpf.Core` and feature-specific packages)
- A valid **DevExpress license** (WPF Subscription, DXperience, or Universal Subscription)
- For `wpf-devexpress-ai-chat-control` — project SDK `Microsoft.NET.Sdk.Razor` and the WebView2 runtime
- For Ribbon, Tab Control, and AI Chat Control — the host window must be `dx:ThemedWindow` (not plain `System.Windows.Window`)

