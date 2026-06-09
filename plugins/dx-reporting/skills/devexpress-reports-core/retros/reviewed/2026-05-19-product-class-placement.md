# Retro — Product model class placement

## Mistake 1

### Task context
The user asked to create an XtraReport in code bound to a sample list of products, grouped by a category field.

### What the skill said (or didn't say)
The skill is silent on file organisation rules for helper and model classes introduced alongside a report class. It contains no guidance on where to place supporting classes, or on the Visual Studio Designer constraint that requires the designed class to be the **first class** in its file.

### What you did wrong
I placed the `Product` model class and the `GetSampleData` helper **before** the `Report` partial class inside `Reports/Report.cs`. This breaks the Visual Studio Designer, which requires the designed class to be the first class in the file.

### Why you made the mistake
The skill instruction was **missing entirely**. There is no rule about class ordering within a designer file, nor any guidance to place auxiliary classes in a separate file.

### What the correct behavior should have been
Supporting model classes must be placed in a dedicated separate file (ideally in a `Model` or `Models` folder), never before the designed class in the same `.cs` file.

### Proposed skill fix
**New rule** — add to the *Quick Start* or *Common Patterns* section:

> **CRITICAL — Designer file class ordering**: When adding helper or model classes alongside a report class, **never place them before the `XtraReport` subclass** in the same `.cs` file. The Visual Studio Designer requires the designed class to be the first class in its file. Instead, place model/helper classes in a dedicated separate file (e.g., `Model/Product.cs`).
