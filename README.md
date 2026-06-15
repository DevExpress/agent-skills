# **DevExpress AI Agent Skills**

This repository stores DevExpress AI agent skills for [GitHub Copilot](https://github.com/features/copilot), [Claude Code](https://claude.ai/code), [Cursor](https://cursor.sh/), [JetBrains AI Assistant](https://www.jetbrains.com/ai/), and other AI coding assistants. Each skill contains DevExpress product knowledge: correct APIs, import paths, and ready-to-run code examples. This information helps your AI assistant generate accurate DevExpress code and reduce hallucinations and dependence on third-party libraries.

**See also:** The [DevExpress Documentation MCP Server](https://docs.devexpress.com/GeneralInformation/405551) connects AI assistants directly to 300,000+ DevExpress help topics. AI Agent Skills and the MCP Server complement each other. Skills encode patterns and guardrails. The MCP Server delivers live documentation lookup.

## **Available Skills**

| **Product/Skills Readme** | **Skills** | **Docs** |
|---|---|---|
| [DevExtreme JavaScript](plugins/dx-devextreme/README.md) | DataGrid, Scheduler, Form, Chat, Button, SelectBox, DateBox, CheckBox, NumberBox, TextBox, TextArea, DataSource, Theming | [Overview](https://js.devexpress.com/Documentation/) |
| [Blazor Components](plugins/dx-blazor/README.md) | AI Chat, Charts, ComboBox, Gauges, Grid, Pivot Table, Ribbon, Scheduler, Toolbar, TreeList | [Overview](https://docs.devexpress.com/Blazor/) |
| [WPF Controls](plugins/dx-wpf/README.md) | Data Grid, Tree List, Pivot Grid, Property Grid, Data Editors, Layout Management, Ribbon & Bars, Accordion, Tab Control, Charts, Scheduler, Loading Indicators, AI Chat, MVVM | [Overview](https://docs.devexpress.com/WPF/) |
| [DevExpress Reports](plugins/dx-reporting/README.md) | Core API, ASP.NET Core, Blazor, Visual Studio Designer | [Overview](https://docs.devexpress.com/XtraReports/) |
| [Office & PDF File API](plugins/dx-office-file-api/README.md) | Spreadsheet, Word Processing, PDF, PDF New (CTP), Presentation, Barcode, Unit Conversion, Excel Export, ZIP, AI-powered Extensions | [Overview](https://docs.devexpress.com/OfficeFileAPI/) |
| [XAF: Cross-Platform .NET App UI & Web API](plugins/dx-xaf/README.md) | Business Model, Business Logic, Business Logic XPO, Controllers, Views, Editors, Filtering, Filtering XPO, Appearance, Validation, Security, Reports, Performance | [Overview](https://docs.devexpress.com/eXpressAppFramework/) |
| ASP.NET Core Controls | *(coming soon)* | [Overview](https://docs.devexpress.com/AspNetCore/) |
| WinForms Controls | *(coming soon)* | [Overview](https://docs.devexpress.com/WindowsForms/) |
| VCL Controls | *(coming soon)* | [Overview](https://docs.devexpress.com/VCL/) |
| BI Dashboard | *(coming soon)* | [Overview](https://docs.devexpress.com/Dashboard/) |




## **Installation**

### Table Of Contents

**CLI agents:**

- [Copilot CLI / Claude Code — Plugin Install](#install-as-a-plugin)
- [GitHub Copilot](#github-copilot)
- [Claude Code](#claude-code)
- [JetBrains Junie](#jetbrains-junie)
- [Codex CLI](#codex-cli)
 
**IDEs:**

- [VS Code](#vs-code)
- [Visual Studio 2022 / 2026](#visual-studio-2022--2026)
- [JetBrains Rider / WebStorm](#jetbrains-rider--webstorm)
- [Cursor](#cursor)


### **Install as a Plugin**

The fastest way to obtain DevExpress skills is via plugins. Plugin installation is available in **GitHub Copilot CLI** and **Claude Code**.

The example below installs the DevExtreme plugin.

```bash
/plugin marketplace add DevExpress/agent-skills
/plugin install dx-devextreme@DevExpress-agent-skills
```

Run `/skills` to list active entries.

> **Note for VS Code and Visual Studio users:** GitHub Copilot CLI plugin installation places skills in `~/.copilot/installed-plugins/` (Windows: `%USERPROFILE%\.copilot\installed-plugins\`), which **IDE agents do not read automatically**. To use skills in your IDE, copy the skill files to your project's `.github/skills/` folder or to the global `~/.copilot/skills/` directory — see [GitHub Copilot setup](#github-copilot) below.

### **Agent-Specific Setup**

#### **GitHub Copilot**

GitHub Copilot discovers skills from two locations:

- **Project-level** (version-controlled, applies to this project only): `.github/skills/`, `.claude/skills/`, `.agents/skills/`
- **Personal/global** (applies to all projects): `~/.copilot/skills/` (Windows: `%USERPROFILE%\.copilot\skills\`)

Both locations work in **Visual Studio**, **VS Code**, and **GitHub Copilot CLI**.

To reference a skill manually in Copilot Chat, use `#`:

> #devextreme-datagrid How do I activate inline row editing?

Copy DevExpress skill folders to one of these locations:

| Location | Path |
|---|---|
| **Project-level** (this project only) | `.github/skills/` in your project root |
| **Global** — macOS/Linux | `~/.copilot/skills/` |
| **Global** — Windows | `%USERPROFILE%\.copilot\skills\` |

The global location makes skills available in all projects and is read by Visual Studio, VS Code, and GitHub Copilot CLI.

**Which folders to copy:** Each plugin contains multiple skill subdirectories. Copy individual skill folders:

```text
plugins/
└── dx-devextreme/  ← plugin (do not copy this directly)
    └── skills/
        ├── devextreme-datagrid/    ← copy individual skill folders like this
        │   ├── SKILL.md
        │   └── references/
        ├── devextreme-form/
        └── ...
```

Paste the folders you need into the destination, for example:

```text
.github/skills/
├── devextreme-datagrid/
└── devextreme-form/
```

[GitHub Copilot skills documentation](https://docs.github.com/en/copilot/using-github-copilot/using-github-copilot-in-the-command-line/using-agent-skills-with-copilot-in-the-cli)

#### **Claude Code**

Copy DevExpress skill folders to one of these locations:

| Location | Path |
|---|---|
| **Project-level** (this project only) | `.claude/skills/` in your project root |
| **Global** — macOS/Linux | `~/.claude/skills/` |
| **Global** — Windows | `%USERPROFILE%\.claude\skills\` |

Skills activate automatically. No additional configuration is needed once the files are in place.

> **Note:** When the [DevExpress Documentation MCP Server](https://docs.devexpress.com/GeneralInformation/405551) is installed globally, or when multiple DevExpress plugins are active (each plugin includes an MCP config), Claude Code may show: *"MCP server "dxdocs" skipped — same command/URL as the already-configured "dxdocs"."* This is not an error. Ignore it, or remove the `dxdocs` entry from your global MCP config to suppress it.

To reference a skill manually, use `/`:

> /devextreme-datagrid How do I activate inline row editing?

[Claude Code skills documentation](https://docs.anthropic.com/en/docs/claude-code/skills)

#### **JetBrains Junie**

Copy DevExpress skill folders to one of these locations:

| Location | Path |
|---|---|
| **Project-level** (this project only) | `.junie/skills/` in your project root |
| **Global** — macOS/Linux | `~/.junie/skills/` |
| **Global** — Windows | `%USERPROFILE%\.junie\skills\` |

Skills activate automatically. Junie scans skill directories and applies relevant skills based on task context.

[Junie agent skills documentation](https://junie.jetbrains.com/docs/agent-skills.html)

#### **Codex CLI**

Copy DevExpress skill folders to `.agents/skills/` in your project root:

| Location | Path |
|---|---|
| **Project-level** (this project only) | `.agents/skills/` in your project root |

```text
.agents/
└── skills/
    ├── devextreme-datagrid/
    └── devextreme-form/
```

See [Which folders to copy](#github-copilot) in the GitHub Copilot section above for the source layout in this repository.

Skills activate automatically when Codex runs in agent mode.

> **Note:** Codex CLI plugin marketplace support for DevExpress skills is planned for a future release.

### **IDE-Specific Setup**

#### **VS Code**

**Install as a plugin (recommended):**

Open the Command Palette (`Ctrl+Shift+P` / `Cmd+Shift+P`), run **Chat: Install Plugin From Source**, and enter the repository URL:

```
https://github.com/DevExpress/agent-skills
```

VS Code installs all available plugins. Restart when prompted, then use Copilot Chat in agent mode.

**Or copy skill files manually:**

After placing skill files in `.github/skills/`:

1. Open Settings (`Ctrl+,` / `Cmd+,`).
2. Search for `chat.agent.skills`.
3. Select **Chat: Use Agent Skills.**

Use Copilot Chat in agent mode. Skills are activated automatically based on your question.

[VS Code agent plugin documentation](https://code.visualstudio.com/docs/agent-customization/agent-plugins)

#### **Visual Studio 2022 / 2026**

Visual Studio reads skills from `.github/skills/` in your project (project-level) and from `%USERPROFILE%\.copilot\skills\` (global, active in all projects). Copy skill folders to one of those locations (see [GitHub Copilot section](#github-copilot) above), then open Copilot Chat (View > GitHub Copilot Chat) and switch to agent mode.

[Visual Studio Copilot agent skills documentation](https://learn.microsoft.com/en-us/visualstudio/ide/copilot-agent-skills?view=visualstudio)

#### **JetBrains Rider / WebStorm**

##### **GitHub Copilot**

Skills are discovered by the [GitHub Copilot plugin](https://plugins.jetbrains.com/plugin/17718-github-copilot). After installing the plugin and signing in:

1. Copy skill folders into `.github/skills/` in your project root (see [GitHub Copilot](#github-copilot) above for destination paths).
2. Open the Copilot Chat panel (Tools > GitHub Copilot > Open GitHub Copilot Chat).
3. Switch to agent mode.

Skills are activated automatically based on your question.

##### **JetBrains AI Assistant**

**Add as an external registry (recommended):**

1. Open **Settings / Preferences** → **Tools** → **AI Assistant** → **Skills**.
2. Click the Settings icon → **Manage External Registries**.
3. Add the repository URL: `https://github.com/DevExpress/agent-skills`
4. Enable the skills you need from the list.

**Or copy skill files manually:**

Copy skill folders to `.agents/skills/` in your project root for a project-level installation, or to the global IDE path:

| **Platform** | **Path** |
|---|---|
| Windows | `%LOCALAPPDATA%\JetBrains\<product><version>\aia\agents\.agents\skills\` |
| macOS | `~/Library/Caches/JetBrains/<product><version>/aia/agents/.agents/skills/` |
| Linux | `~/.cache/JetBrains/<product><version>/aia/agents/.agents/skills/` |

Skills activate automatically in agent mode. To invoke a skill manually, type `$` followed by the skill name.

[JetBrains AI Assistant skills documentation](https://www.jetbrains.com/help/ai-assistant/agent-skills.html)

##### **JetBrains Junie**

See [JetBrains Junie](#jetbrains-junie) in Agent-Specific Setup above for destination paths.

#### **Cursor**

Copy DevExpress skill folders to one of these locations:

| Location | Path |
|---|---|
| **Project-level** (this project only) | `.cursor/skills/` in your project root |
| **Global** — macOS/Linux | `~/.cursor/skills/` |
| **Global** — Windows | `%USERPROFILE%\.cursor\skills\` |

Skills activate automatically in agent mode. No additional configuration is needed once the files are in place.

To reference a skill manually, use `/`:

> /devextreme-datagrid How do I activate inline row editing?

- [Cursor plugins documentation](https://cursor.com/docs/plugins)
- [Cursor skills documentation](https://cursor.com/docs/skills)

## **License**

[MIT](LICENSE)
