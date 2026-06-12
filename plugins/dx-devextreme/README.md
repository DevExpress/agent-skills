# AI Agent Skills for DevExtreme

This plugin contains AI agent skills for [DevExtreme](https://js.devexpress.com/) (DevExpress UI component library for Angular, React, Vue, and jQuery).

---

## Prerequisites

- DevExtreme v26.1+
- [DevExpress subscription license](https://www.devexpress.com/buy/winforms-wpf-blazor-asp-net-maui/) (DevExtreme Complete, ASP.NET and Blazor, DXperience, Universal)

---

## Included AI Agent Skills

| Skill | Capabilities | Docs |
| --- | --- | --- |
| [devextreme-datagrid](skills/devextreme-datagrid/) | DataGrid: columns, data editing, data shaping (filter, sort, and group rows), summaries, master-detail data presentation, selection, paging, scrolling, export, toolbar, remote operations, AI columns | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/DataGrid/Overview/) |
| [devextreme-scheduler](skills/devextreme-scheduler/) | Scheduler: views, appointments, recurrence, multi-resource display, remote data, toolbar customization | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/Scheduler/Overview/) |
| [devextreme-form](skills/devextreme-form/) | Form: items, groups, tabs, columns, validation rules, editor options, Smart Paste, AI form filling | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/Form/Overview/) |
| [devextreme-chat](skills/devextreme-chat/) | Chat: messages, typing indicators, alerts, suggestion buttons, AI service integration, response streaming | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/Chat/Overview/) |
| [devextreme-button](skills/devextreme-button/) | Button: button type, styling mode, icon, form submission, validation, custom template | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/Button/Overview/) |
| [devextreme-selectbox](skills/devextreme-selectbox/) | SelectBox: data binding, value and display expressions, search, custom items, grouping, value change events | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/SelectBox/Overview/) |
| [devextreme-datebox](skills/devextreme-datebox/) | DateBox: date/time/datetime types, formatting, range limits, disabled dates, value change events | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/DateBox/Overview/) |
| [devextreme-checkbox](skills/devextreme-checkbox/) | CheckBox: value states, three-state (indeterminate) behavior, labels, value change events | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/CheckBox/Getting_Started_with_CheckBox/) |
| [devextreme-numberbox](skills/devextreme-numberbox/) | NumberBox: minimum and maximum values, display format, spin buttons, invalid value handling | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/NumberBox/Getting_Started_with_NumberBox/) |
| [devextreme-textbox](skills/devextreme-textbox/) | TextBox: input mode, label, placeholder text, masked input, clear button, password field | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/TextBox/Getting_Started_with_TextBox/) |
| [devextreme-textarea](skills/devextreme-textarea/) | TextArea: auto-resize, height restrictions, value binding, value change events | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/TextArea/Getting_Started_with_TextArea/) |
| [devextreme-datasource](skills/devextreme-datasource/) | DataSource, ArrayStore, ODataStore, CustomStore, LocalStore: load functions, remote operations, paging, filtering | [Overview](https://js.devexpress.com/Documentation/Guide/Data_Binding/Data_Layer/) |
| [devextreme-theming](skills/devextreme-theming/) | Themes (Generic, Material, Fluent): ThemeBuilder UI and CLI, SCSS variables, color swatches, runtime switching | [Overview](https://js.devexpress.com/Documentation/Guide/Themes_and_Styles/Predefined_Themes/) |

---

## Skill Folder Content

Each skill is self-contained and follows the same structure:

```
devextreme-<name>/
├── SKILL.md      -- YAML frontmatter (activators, prerequisites), navigation guide
└── references/   -- scenario-focused deep dives
```

---

For agent-specific and IDE-specific setup instructions, see the [repository README](../../README.md).
