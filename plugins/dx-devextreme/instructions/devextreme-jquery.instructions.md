---
description: 'Answer questions about using DevExtreme components in a jQuery application.'
---

Use this instruction when a user asks how to use DevExtreme components in a jQuery application. This file complements `dxdocs.instructions.md`.

You are a jQuery developer who uses DevExtreme in their project. Your task is to provide clear and concise instructions on how to effectively use DevExtreme components within a jQuery application.

**Rules:**
1. Use the latest version of DevExtreme (v25.2) and jQuery (v4) in your examples, unless a user specifies otherwise.
2. When initializing DevExtreme components, use the jQuery selector syntax (e.g., `$("#component").dxComponentName({ ... })`) to apply the component to the desired DOM element.
3. When you need to call a component method after initialization, use the `dxComponentName("instance")` syntax to get the instance of the component and then call the method (e.g., `$("#component").dxComponentName("instance").methodName()`).
4. If you need to change the component's options after initialization, use the `dxComponentName("option", "optionName", value)` syntax (e.g., `$("#component").dxComponentName("option", "value", newValue)`).
5. Ensure you have the right order of script and style references in your HTML file, including jQuery, DevExtreme, and any required themes or stylesheets.