# When to Use AccordionControl vs NavBarControl vs Hamburger Menu

DevExpress WinForms ships three overlapping side-navigation controls. Choosing the wrong one leads to awkward UX or unnecessary rework. This reference explains the differences and gives a clear decision guide.

## When to Use This Reference

- Evaluating which navigation control fits your application's design
- Migrating from `NavBarControl` to `AccordionControl`
- Deciding whether a Hamburger Menu style is appropriate

## The Three Controls at a Glance

| Control | Class | NuGet | Primary Use Case |
|---|---|---|---|
| **AccordionControl** | `DevExpress.XtraBars.Navigation.AccordionControl` | `DevExpress.Win.Navigation` | Multi-level, always-visible side menu with rich customization |
| **NavBarControl** | `DevExpress.XtraNavBar.NavBarControl` | `DevExpress.Win.Navigation` | Classic Outlook-style navigation pane, flat item-link model |
| **Hamburger Menu** | `AccordionControl` with `ViewType = HamburgerMenu` | `DevExpress.Win.Navigation` | Collapsible/expandable sidebar for space-constrained or modern UIs |

Note: "Hamburger Menu" is not a separate control — it is `AccordionControl` with a different `ViewType`.

## Feature Comparison

| Feature | AccordionControl | NavBarControl | Hamburger Menu (AccordionControl) |
|---|---|---|---|
| Hierarchy depth | Unlimited levels | 2 levels (group → item link) | Unlimited levels |
| Items can repeat across groups | No — each element exists once | Yes — `NavBarItem` / `ItemLink` separation allows reuse | No |
| Content containers inside items | Yes — host any WinForms control | Groups only (via `ControlContainer`) | Yes |
| Minimized / icon-only state | Yes (`OptionsMinimizing.State`) | No | Yes (central feature) |
| Overlay expansion | No | No | Yes (`Overlay` and `Minimal` display modes) |
| Built-in search / filter | Yes (`ShowFilterControl`) | No | Yes |
| Context buttons per item | Yes | No | Yes |
| HTML-CSS element templates | Yes | No | Yes |
| Adapt layout to window resize | No (fixed width) | No | Yes (when hosted in `FluentDesignForm`) |
| Classic Outlook "Navigation Pane" look | Approximate (Footer mode) | Native (Navigation Pane view) | No |
| `AllowItemSelection` (highlight selected) | Yes | Yes (link selection) | Yes |

## Decision Guide

### Use `AccordionControl` when:

- The navigation menu has **more than 2 levels** of hierarchy.
- Items need **custom embedded controls** (DatePicker, ToggleSwitch, etc.) inside expandable content containers.
- You want a **persistent sidebar** that is always visible and never collapses to an icon strip.
- You need a **built-in search/filter** panel so users can locate items by typing.
- The UI must support **context buttons** (action buttons inline within each item header).
- You are building a **new application** and do not need legacy Outlook look-and-feel.

### Use `NavBarControl` when:

- You need the classic **Microsoft Outlook Navigation Pane** visual style.
- The same navigation item (`NavBarItem`) needs to appear in **multiple groups** (link reuse).
- You target legacy desktop users who expect the Outlook tab-switching model (only one group active at a time, switching collapses/expands the active group).
- The menu is strictly **2 levels deep** and content containers are not needed inside items.

### Use the Hamburger Menu style (`AccordionControl.ViewType = HamburgerMenu`) when:

- Screen real estate is limited and the sidebar should **collapse to an icon strip** when not in use.
- The design follows **Windows 10/11 Fluent conventions** (especially when combined with `FluentDesignForm`).
- Users should be able to toggle the sidebar **without losing context** (Overlay mode keeps content visible beneath the open menu).
- The application targets touch-capable devices or mixed-input scenarios.
- You want the menu to **automatically adapt** its display mode (Inline / Overlay / Minimal) as the window is resized (requires `FluentDesignForm`).

## Migrating from NavBarControl to AccordionControl

If you have an existing `NavBarControl`-based UI and want to migrate:

1. Replace `NavBarGroup` with `AccordionControlElement(ElementStyle.Group)`.
2. Replace `NavBarItem` + `ItemLink` with `AccordionControlElement(ElementStyle.Item)`.
3. Handle `AccordionControl.ElementClick` instead of `NavBarControl.LinkClicked`.
4. Move group `ControlContainer` content into item `ContentContainer` (if groups had embedded controls).

Key behavioral difference: `NavBarControl` groups are mutually exclusive by default (one expanded at a time), while `AccordionControl` allows multiple groups expanded simultaneously — control this with `AccordionControl.ExpandElementMode`.
