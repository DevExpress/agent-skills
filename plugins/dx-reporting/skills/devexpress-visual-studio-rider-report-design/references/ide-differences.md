# Report Designer IDE Differences

DevExpress XtraReport code-behind (`.Designer.cs`) works with both the Visual Studio and JetBrains Rider Report Designers. The serialization format, `InitializeComponent()` contract, and all code patterns in this skill are identical for both IDEs.

## IDE Support Matrix

| Feature | Visual Studio | JetBrains Rider |
|---|---|---|
| OS | Windows only | Windows only |
| Custom controls in toolbox | ✅ | ❌ |
| Report Design Analyzer panel | ✅ | ❌ |
| Built-in AI assistance | ✅ | ❌ |
| Reflects code changes without reopening | ✅ | ❌ |

## JetBrains Rider: Reopen to Reflect Changes

After editing a `.Designer.cs` file, you must **close and reopen the report** in the Rider designer for changes to appear. This is a Rider limitation shared with its WinForms designer — not a DevExpress limitation.

## JetBrains Rider: No Custom Control Support

Custom `XRControl` subclasses cannot be rendered or placed in the Rider designer toolbox. Use standard DevExpress controls (`XRLabel`, `XRTable`, `XRPictureBox`, etc.) for designer-visible layouts. Custom controls added in code still render correctly at runtime.

## JetBrains Rider: No Report Design Analyzer

The Report Design Analyzer panel — which flags layout errors, warnings, and information messages in real time — is only available in Visual Studio. When working in Rider, verify layout correctness by running the application rather than relying on the Analyzer.

## Visual Studio Report Designer Troubleshooting

If your report won't open or displays incorrectly in the Visual Studio designer, consult:

- [Report Designer Errors](https://docs.devexpress.com/content/XtraReports/403089/report-designer-ides/report-designer-for-visual-studio/report-designer-errors?v=26.1&md=true)
- [Error List Panel (errors, warnings, information messages)](https://docs.devexpress.com/content/XtraReports/403060/report-designer-ides/report-designer-for-visual-studio/dock-panels/error-list/errors-warnings-and-information-messages?v=26.1&md=true)
