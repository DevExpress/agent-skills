# **DevExpress AI Agent Skills**

This repository stores DevExpress AI agent skills for [GitHub Copilot](https://github.com/features/copilot), [Claude Code](https://claude.ai/code), [Cursor](https://cursor.sh/), [JetBrains AI Assistant](https://www.jetbrains.com/ai/), and other AI coding assistants. Each skill contains DevExpress product knowledge: correct APIs, import paths, and ready-to-run code examples. This information helps your AI assistant generate accurate DevExpress code and reduce hallucinations and dependance on third-party libraries.

**See also:** The [DevExpress Documentation MCP Server](https://docs.devexpress.com/GeneralInformation/405551) connects AI assistants directly to 300,000+ DevExpress help topics. AI Agent Skills and the MCP Server complement each other. Skills encode patterns and guardrails. The MCP Server delivers live documentation lookup.

## **Available Skills**

|  |  |  |
| --- | --- | --- |
| **Product** | **Skills** | **Docs** |
| [JavaScript & ASP.NET Core � DevExtreme](https://github.com/orgs/DevExpress/projects/156/views/.github/skills/devextreme/README.md) | DataGrid, Scheduler, Form, Chat, Button, SelectBox, DateBox, CheckBox, NumberBox, TextBox, TextArea, DataSource, Theming | [Overview](https://js.devexpress.com/Documentation/) |
| [WinForms Controls](https://github.com/orgs/DevExpress/projects/156/views/skills/winforms/README.md) | *(coming soon)* | [Overview](https://docs.devexpress.com/WindowsForms/) |
| [WPF Controls](https://github.com/orgs/DevExpress/projects/156/views/skills/wpf/README.md) | *(coming soon)* | [Overview](https://docs.devexpress.com/WPF/) |
| [.NET Reports](https://github.com/orgs/DevExpress/projects/156/views/skills/reporting/README.md) | *(coming soon)* | [Overview](https://docs.devexpress.com/XtraReports/) |
| [Office & PDF File API](https://github.com/orgs/DevExpress/projects/156/views/skills/office-file-api/README.md) | *(coming soon)* | [Overview](https://docs.devexpress.com/OfficeFileAPI/) |
| [XAF: Cross-Platform .NET App UI & Web API](https://github.com/orgs/DevExpress/projects/156/views/skills/xaf/README.md) | *(coming soon)* | [Overview](https://docs.devexpress.com/eXpressAppFramework/) |

## **Installation**

### **Install as a Plugin**

This installation method is the fastest way to obtain all DevExpress skills at once. You must use GitHub Copilot CLI v2.90.0+ or Claude Code.

**GitHub Copilot CLI / Claude Code:**

```bash
/plugin marketplace add DevExpress/agent-skills
/plugin install devextreme@devexpress-agent-skills
```

Restart your IDE after installing new skills. Run /skills to list active entries.

### **Copy Skill Folders**

Copy skills you need into your project, then configure your agent or IDE as described below.

```bash
# Project-level — active in this repo only
mkdir -p .github/skills
cp -r .github/skills/devextreme-datagrid /your-project/.github/skills/
cp -r .github/skills/devextreme-form /your-project/.github/skills/

# Or copy all DevExtreme skills at once
cp -r .github/skills/devextreme-* /your-project/.github/skills/
```

Copy to the following folders for a global installation (active in all projects):

|  |  |
| --- | --- |
| **Platform** | **Path** |
| Windows | %USERPROFILE%\.copilot\skills\ |
| macOS / Linux | ~/.copilot/skills/ |

### **Agent-Specific Setup**

#### **GitHub Copilot**

Skills in .github/skills/ are discovered automatically when Copilot runs in agent mode. No additional configuration is needed once the files are in place.

To reference a skill manually in Copilot Chat, use #:

#devextreme-datagrid How do I activate inline row editing?

[GitHub Copilot skills documentation](https://docs.github.com/en/copilot/using-github-copilot/using-github-copilot-in-the-command-line/using-agent-skills-with-copilot-in-the-cli)

#### **Claude Code**

Claude Code reads skills from .claude/skills/ in your project, or ~/.claude/skills/ globally.

```bash
# Project-level
mkdir -p .claude/skills
cp -r .github/skills/devextreme-datagrid .claude/skills/
cp -r .github/skills/devextreme-form .claude/skills/

# Global
mkdir -p ~/.claude/skills
cp -r .github/skills/devextreme-* ~/.claude/skills/
```

Skills activate automatically. No additional configuration is needed once the files are in place.

To reference a skill manually, use /:

/devextreme-datagrid How do I activate inline row editing?

[Claude Code skills documentation](https://docs.anthropic.com/en/docs/claude-code/skills)

### **IDE-Specific Setup**

#### **VS Code**

After placing skill files in .github/skills/:

1. Open Settings (Ctrl+, / Cmd+,).
2. Search for �chat.agent.skills�.
3. Select **Chat: Use Agent Skills.**

Use Copilot Chat in agent mode. Skills are activated automatically based on your question.

#### **Visual Studio 2022 / 2026**

Visual Studio reads skills from the same .github/skills/ folder. After copying the files, open Copilot Chat (View ? GitHub Copilot Chat) and switch to agent mode. No additional configuration is needed.

#### **JetBrains Rider / WebStorm**

Skills are discovered by the [GitHub Copilot plugin](https://plugins.jetbrains.com/plugin/17718-github-copilot). After installing the plugin and signing in:

1. Copy skill folders into .github/skills/ in your project root.
2. Open the Copilot Chat panel (Tools ? GitHub Copilot ? Open GitHub Copilot Chat).
3. Switch to agent mode.

Skills are activated automatically based on your question.

#### **Cursor**

This repository can be installed from the Cursor plugin marketplace:

1. Open the marketplace panel in Cursor.
2. Search for �**DevExpress�.**

For a local installation, copy skill folders to ~/.cursor/plugins/local/devexpress-agent-skills.

## **License TODO!!!!!**

[MIT](LICENSE)

# **DevExtreme Skills**

AI agent skills for [DevExtreme](https://js.devexpress.com/) - the DevExpress UI component library for Angular, React, Vue, and jQuery.

All skills target DevExtreme v25.2+.

## **Skills**

|  |  |  |
| --- | --- | --- |
| **Skill** | **Covers** | **Docs** |
| [devextreme-datagrid](https://github.com/orgs/DevExpress/projects/156/devextreme-datagrid/) | DataGrid: columns, editing, filtering, sorting, grouping, selection, paging, scrolling, summaries, export, toolbar, master-detail, remote operations, AI columns | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/DataGrid/Overview/) |
| [devextreme-scheduler](https://github.com/orgs/DevExpress/projects/156/devextreme-scheduler/) | Scheduler: views, appointments, recurrence, resources, grouping, remote data, toolbar customization | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/Scheduler/Overview/) |
| [devextreme-form](https://github.com/orgs/DevExpress/projects/156/devextreme-form/) | Form: simple items, groups, tabs, columns, validation rules, editor options, Smart Paste, AI form filling | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/Form/Overview/) |
| [devextreme-chat](https://github.com/orgs/DevExpress/projects/156/devextreme-chat/) | Chat: messages, typing indicators, alerts, suggestion buttons, AI service integration, streaming responses | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/Chat/Overview/) |
| [devextreme-button](https://github.com/orgs/DevExpress/projects/156/devextreme-button/) | Button: types, styling modes, icons, form submission, validation, custom templates | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/Button/Overview/) |
| [devextreme-selectbox](https://github.com/orgs/DevExpress/projects/156/devextreme-selectbox/) | SelectBox: data binding, valueExpr/displayExpr, search, custom items, grouping, value change events | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/SelectBox/Overview/) |
| [devextreme-datebox](https://github.com/orgs/DevExpress/projects/156/devextreme-datebox/) | DateBox: date/time/datetime types, formatting, range limits, disabled dates, value change events | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/DateBox/Overview/) |
| [devextreme-checkbox](https://github.com/orgs/DevExpress/projects/156/devextreme-checkbox/) | CheckBox: value states, three-state (indeterminate) behavior, labels, value change events | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/CheckBox/Getting_Started_with_CheckBox/) |
| [devextreme-numberbox](https://github.com/orgs/DevExpress/projects/156/devextreme-numberbox/) | NumberBox: min/max, format, spin buttons, invalid value handling | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/NumberBox/Getting_Started_with_NumberBox/) |
| [devextreme-textbox](https://github.com/orgs/DevExpress/projects/156/devextreme-textbox/) | TextBox: input modes, labels, placeholders, masking, clear button, password field | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/TextBox/Getting_Started_with_TextBox/) |
| [devextreme-textarea](https://github.com/orgs/DevExpress/projects/156/devextreme-textarea/) | TextArea: auto-resize, min/max height, value binding, value change events | [Overview](https://js.devexpress.com/Documentation/Guide/UI_Components/TextArea/Getting_Started_with_TextArea/) |
| [devextreme-datasource](https://github.com/orgs/DevExpress/projects/156/devextreme-datasource/) | DataSource, ArrayStore, ODataStore, CustomStore, LocalStore: load functions, remote operations, paging, filtering | [Overview](https://js.devexpress.com/Documentation/Guide/Data_Binding/Data_Layer/) |
| [devextreme-theming](https://github.com/orgs/DevExpress/projects/156/devextreme-theming/) | Themes: Generic, Material, Fluent; ThemeBuilder UI and CLI; SCSS variables; color swatches; runtime switching | [Overview](https://js.devexpress.com/Documentation/Guide/Themes_and_Styles/Predefined_Themes/) |

See the [repository README](README.md) for agent-specific and IDE-specific setup instructions.
