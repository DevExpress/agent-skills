# **DevExpress AI Agent Skills**

This repository stores DevExpress AI agent skills for [GitHub Copilot](https://github.com/features/copilot), [Claude Code](https://claude.ai/code), [Cursor](https://cursor.sh/), [JetBrains AI Assistant](https://www.jetbrains.com/ai/), and other AI coding assistants. Each skill contains DevExpress product knowledge: correct APIs, import paths, and ready-to-run code examples. This information helps your AI assistant generate accurate DevExpress code and reduce hallucinations and dependence on third-party libraries.

**See also:** The [DevExpress Documentation MCP Server](https://docs.devexpress.com/GeneralInformation/405551) connects AI assistants directly to 300,000+ DevExpress help topics. AI Agent Skills and the MCP Server complement each other. Skills encode patterns and guardrails. The MCP Server delivers live documentation lookup.

## **Available Skills**

| **Product**                                                           | **Skills** | **Docs** |
|-----------------------------------------------------------------------| --- | --- |
| [DevExtreme JavaScript](plugins/dx-devextreme/README.md)              | DataGrid, Scheduler, Form, Chat, Button, SelectBox, DateBox, CheckBox, NumberBox, TextBox, TextArea, DataSource, Theming | [Overview](https://js.devexpress.com/Documentation/) |
| [Blazor Components](plugins/dx-blazor/README.md)                      | AI Chat, Charts, ComboBox, Gauges, Grid, Pivot Table, Ribbon, Scheduler, Toolbar, TreeList | [Overview](https://docs.devexpress.com/Blazor/) |
| [WPF Controls](plugins/dx-wpf/README.md)                              | Data Grid, Tree List, Pivot Grid, Property Grid, Data Editors, Layout Management, Ribbon & Bars, Accordion, Tab Control, Charts, Scheduler, Loading Indicators, AI Chat, MVVM | [Overview](https://docs.devexpress.com/WPF/) |
| [DevExpress Reports](plugins/dx-reporting/README.md)                  | Core API, ASP.NET Core, Blazor, Visual Studio Designer | [Overview](https://docs.devexpress.com/XtraReports/) |
| [Office & PDF File API](plugins/dx-office-file-api/README.md)         | Spreadsheet, Word Processing, PDF, PDF New (CTP), Presentation, Barcode, Unit Conversion, Excel Export, ZIP, AI-powered Extensions | [Overview](https://docs.devexpress.com/OfficeFileAPI/) |
| [XAF: Cross-Platform .NET App UI & Web API](plugins/dx-xaf/README.md) | Business Model, Business Logic, Business Logic XPO, Controllers, Views, Editors, Filtering, Filtering XPO, Appearance, Validation, Security, Reports, Performance | [Overview](https://docs.devexpress.com/eXpressAppFramework/) |
| ASP.NET Core Controls | *(coming soon)* | [Overview](https://docs.devexpress.com/AspNetCore/) |
| WinForms Controls | *(coming soon)* | [Overview](https://docs.devexpress.com/WindowsForms/) |
| VCL Controls | *(coming soon)* | [Overview](https://docs.devexpress.com/VCL/) |
| BI Dashboard | *(coming soon)* | [Overview](https://docs.devexpress.com/Dashboard/) |


## **Installation**

### **Install as a Plugin**

This installation method is the fastest way to obtain all DevExpress skills at once. You must use GitHub Copilot CLI v2.90.0+ or Claude Code.

**GitHub Copilot CLI / Claude Code:**

```bash
/plugin marketplace add DevExpress/agent-skills
/plugin install dx-devextreme@DevExpress-agent-skills
```

Restart your IDE after installing new skills. Run /skills to list active entries.

### **Copy Skill Folders**

Copy skills you need into your project, then configure your agent or IDE as described below.

```bash
# Project-level — active in this repo only
mkdir -p .github/skills
cp -r plugins/dx-devextreme/skills/devextreme-datagrid /your-project/.github/skills/
cp -r plugins/dx-devextreme/skills/devextreme-form /your-project/.github/skills/

# Or copy all DevExtreme skills at once
cp -r plugins/dx-devextreme/skills/* /your-project/.github/skills/
```

Copy to the following folders for a global installation (active in all projects):

| **Platform** | **Path** |
| --- | --- |
| Windows | %USERPROFILE%\.copilot\skills\ |
| macOS / Linux | ~/.copilot/skills/ |

### **Agent-Specific Setup**

#### **GitHub Copilot**

Skills in .github/skills/ are discovered automatically when Copilot runs in agent mode. No additional configuration is needed once the files are in place.

To reference a skill manually in Copilot Chat, use #:

> #devextreme-datagrid How do I activate inline row editing?

[GitHub Copilot skills documentation](https://docs.github.com/en/copilot/using-github-copilot/using-github-copilot-in-the-command-line/using-agent-skills-with-copilot-in-the-cli)

#### **Claude Code**

Claude Code reads skills from .claude/skills/ in your project, or ~/.claude/skills/ globally.

```bash
# Project-level
mkdir -p .claude/skills
cp -r plugins/dx-devextreme/skills/devextreme-datagrid .claude/skills/
cp -r plugins/dx-devextreme/skills/devextreme-form .claude/skills/

# Global
mkdir -p ~/.claude/skills
cp -r plugins/dx-devextreme/skills/* ~/.claude/skills/
```

Skills activate automatically. No additional configuration is needed once the files are in place.

To reference a skill manually, use /:

> /devextreme-datagrid How do I activate inline row editing?

[Claude Code skills documentation](https://docs.anthropic.com/en/docs/claude-code/skills)

### **IDE-Specific Setup**

#### **VS Code**

After placing skill files in .github/skills/:

1. Open Settings (Ctrl+, / Cmd+,).
2. Search for `chat.agent.skills`.
3. Select **Chat: Use Agent Skills.**

Use Copilot Chat in agent mode. Skills are activated automatically based on your question.

#### **Visual Studio 2022 / 2026**

Visual Studio reads skills from the same .github/skills/ folder. After copying the files, open Copilot Chat (View > GitHub Copilot Chat) and switch to agent mode. No additional configuration is needed.

#### **JetBrains Rider / WebStorm**

Skills are discovered by the [GitHub Copilot plugin](https://plugins.jetbrains.com/plugin/17718-github-copilot). After installing the plugin and signing in:

1. Copy skill folders into .github/skills/ in your project root.
2. Open the Copilot Chat panel (Tools > GitHub Copilot > Open GitHub Copilot Chat).
3. Switch to agent mode.

Skills are activated automatically based on your question.

#### **Cursor**

Cursor reads skills from `.cursor/skills/` in your project root, or `~/.cursor/skills/` globally.

```bash
# Project-level
mkdir -p .cursor/skills
cp -r plugins/dx-devextreme/skills/* .cursor/skills/

# Global
mkdir -p ~/.cursor/skills
cp -r plugins/dx-devextreme/skills/* ~/.cursor/skills/
```

Skills activate automatically in agent mode. No additional configuration is needed once the files are in place.

> /devextreme-datagrid How do I activate inline row editing?

- [Cursor plugins documentation](https://cursor.com/docs/plugins)
- [Cursor skills documentation](https://cursor.com/docs/skills)

## **License**

[MIT](LICENSE)
