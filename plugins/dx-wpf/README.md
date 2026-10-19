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
| [devexpress-wpf-data-grid](skills/devexpress-wpf-data-grid/) | `GridControl` — view management (TableView, CardView, TreeListView), data binding (EF Core, XPO, server mode), columns, data shaping (sort, filter, group rows), summaries, master-detail layout, conditional formatting, printing, and export | [DataGrid Documentation](https://docs.devexpress.com/WPF/6084/controls-and-libraries/data-grid) |
| [devexpress-wpf-tree-list](skills/devexpress-wpf-tree-list/) | `TreeListControl` — data binding (self-referential and hierarchical sources), unbound mode, drag-and-drop settings, row selection, edit forms, validation | [TreeList Documentation](https://docs.devexpress.com/WPF/9759/controls-and-libraries/tree-list) |
| [devexpress-wpf-grid-datasource](skills/devexpress-wpf-grid-datasource/) | Data source advisor for `GridControl` and `TreeListControl` — inspects the project, picks `ObservableCollection<T>`, Server Mode, Instant Feedback, Virtual Sources, or `RealTimeSource`, and implements the binding | [Large Data Sources Documentation](https://docs.devexpress.com/WPF/401037) |
| [devexpress-wpf-pivot-grid](skills/devexpress-wpf-pivot-grid/) | `PivotGridControl` — area management (row, column, data, and filter areas), OLAP, server mode, aggregation, drill-down, KPI, conditional formatting | [PivotGrid Documentation](https://docs.devexpress.com/WPF/7228/controls-and-libraries/pivot-grid) |
| [devexpress-wpf-property-grid](skills/devexpress-wpf-property-grid/) | `PropertyGridControl` — selected object, properties, collections, categories, expandable nested types | [PropertyGrid Documentation](https://docs.devexpress.com/WPF/15640/controls-and-libraries/property-grid) |
| [devexpress-wpf-charts](skills/devexpress-wpf-charts/) | `ChartControl` (2D) — diagram management (XY, polar, radar, simple), 15+ series types, primary and secondary axes, legend, tooltip, crosshair, aggregation | [Chart Documentation](https://docs.devexpress.com/WPF/115092/controls-and-libraries/charts-suite) |
| [devexpress-wpf-scheduler](skills/devexpress-wpf-scheduler/) | `SchedulerControl` — view type management (day, work week, week, month, timeline, agenda, list), appointments, resources, labels, statuses, time regions, recurrence, reminders, time zones | [Scheduler Documentation](https://docs.devexpress.com/WPF/114881/controls-and-libraries/scheduler) |
| [devexpress-wpf-ai-chat-control](skills/devexpress-wpf-ai-chat-control/) | `AIChatControl` — Copilot-style chat UI, provider settings (Azure OpenAI, OpenAI, Ollama, Semantic Kernel), streaming, markdown formatting, file upload, prompt suggestions, history | [AI Chat Control Documentation](https://docs.devexpress.com/WPF/405434/ai-powered-extensions/ai-chat-control) |

---

### Data Editors and Utilities

| Skill | Capabilities | Docs |
|---|---|---|
| [devexpress-wpf-data-editors](skills/devexpress-wpf-data-editors/) | 30+ editors (`TextEdit`, `ButtonEdit`, `ComboBoxEdit`, `DateEdit`, `SpinEdit`, `LookUpEdit`, `PasswordBoxEdit`, `ColorEdit`, `RatingEdit`, `BarCodeEdit`, and more), all-purpose UI controls (`SimpleButton`, `DropDownButton`, `FlyoutControl`, `RangeControl`, `Calculator`) | [Editors & Combo Box Documentation](https://docs.devexpress.com/WPF/6190/controls-and-libraries/data-editors) |
| [devexpress-wpf-loading-indicators](skills/devexpress-wpf-loading-indicators/) | `SplashScreenManager`, `LoadingDecorator`, `WaitIndicator` — decision guide for picking the right indicator; migration from legacy `DXSplashScreen` | [Loading Indicators Documentation](https://docs.devexpress.com/WPF/115521/controls-and-libraries/windows-and-utility-controls) |

---

### Layout & Navigation

| Skill | Capabilities | Docs |
|---|---|---|
| [devexpress-wpf-layout-management](skills/devexpress-wpf-layout-management/) | Six layout containers (`DockLayoutManager`, `LayoutControl`, `DataLayoutControl`, `TileLayoutControl`, `FlowLayoutControl`, `DockLayoutControl`), layout persistence | [Layout Management Documentation](https://docs.devexpress.com/WPF/115547/controls-and-libraries/layout-management) |
| [devexpress-wpf-ribbon-and-bars](skills/devexpress-wpf-ribbon-and-bars/) | Office-style command controls (Ribbon, toolbar, main menu, status bar), Quick Access Toolbar, Backstage View, MDI merging | [Ribbon and Bar Manager Documentation](https://docs.devexpress.com/WPF/115392/controls-and-libraries/ribbon-bars-and-menu) |
| [devexpress-wpf-accordion](skills/devexpress-wpf-accordion/) | `AccordionControl` — hierarchical sidebar, Navigation Pane mode, built-in search, collapsed strip (glyphs only) | [Accordion Documentation](https://docs.devexpress.com/WPF/118347/controls-and-libraries/navigation-controls/accordion-control) |
| [devexpress-wpf-tab-control](skills/devexpress-wpf-tab-control/) | `DXTabControl` — tab mode management (multiLine, scroll, stretch), drag-and-drop tab reordering, accent colors, close and pin tabs | [Tab Control Documentation](https://docs.devexpress.com/WPF/7974/controls-and-libraries/layout-management/tab-control) |

---

### UI Design and Appearance

| Skill | Capabilities | Docs |
|---|---|---|
| [devexpress-wpf-from-screenshot](skills/devexpress-wpf-from-screenshot/) | Builds a DevExpress WPF screen from a screenshot, an image, or a Figma frame — picks a control per region, writes the XAML, runs the app, and compares the result with the mockup | [WPF Controls Documentation](https://docs.devexpress.com/WPF/) |
| [devexpress-wpf-theme-customizer](skills/devexpress-wpf-theme-customizer/) | Changes the appearance of any DevExpress WPF control — public API first, then a scoped style, then a theme resource override from the installed theme source; verifies the result with before/after captures | [Modify Theme Resources](https://docs.devexpress.com/WPF/403598) |

---

### Frameworks

| Skill | Capabilities | Docs |
|---|---|---|
| [devexpress-wpf-mvvm](skills/devexpress-wpf-mvvm/) | View-model strategies (`GenerateViewModel` source generator, `ViewModelSource`, `ViewModelBase`, `BindableBase`), `DelegateCommand` and `AsyncCommand`, 25+ predefined services (`IMessageBoxService`, `IDialogService`, `IDocumentManagerService`, `INotificationService`, and more), behaviors (`EventToCommand`, `KeyToCommand`, `FocusBehavior`, and more), `Messenger` | [Overview](https://docs.devexpress.com/WPF/15112/mvvm-framework) |
| [devexpress-wpf-dependency-injection](skills/devexpress-wpf-dependency-injection/) | Adds `Microsoft.Extensions.DependencyInjection` (optionally the .NET Generic Host) to an existing app — inspects how view models and services are created, composition root, registrations and lifetimes, `IocServiceProvider` and `{dxmvvm:Ioc}`, views created by `DialogService`/`WindowService`, child UserControls, POCO view models | [Dependency Injection](https://docs.devexpress.com/WPF/403514) |

---

## Skill Folder Content

Each skill is self-contained and follows the same structure:

```
devexpress-wpf-<name>/
├── SKILL.md      -- YAML frontmatter (activators, prerequisites), navigation guide
├── references/   -- scenario-focused deep dives
└── examples/     -- getting-started samples (XAML + C#)
```

---

For agent-specific and IDE-specific setup instructions, see the [repository README](../../README.md).
