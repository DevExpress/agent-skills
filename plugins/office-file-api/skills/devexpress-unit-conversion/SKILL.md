---
name: devexpress-unit-conversion
description: Build .NET applications with the DevExpress Unit Conversion API for converting between metric and US units of measurement with type-safe fluent syntax. Use when converting distances, lengths, weights, temperatures, areas, volumes, speeds, or metric prefixes in .NET applications. Also use when someone mentions "DevExpress Unit Conversion", "UnitConversion", "DevExpress.UnitConversion", "convert units C#", "metric to imperial", "length conversion .NET", or asks about any unit conversion with DevExpress. Covers both .NET and .NET Framework.
compatibility: Requires .NET 6+ or .NET Framework 4.6.2+. NuGet packages from the DevExpress feed.
metadata:
  author: DevExpress
  version: "25.2"
  source-commit: c5a96ff6e891a1c2633c6621186093faaefabefd
---

# DevExpress Unit Conversion API

A lightweight .NET library for converting between metric and US customary units of measurement using a type-safe fluent API. Supports distance, mass, temperature, area, volume, speed, and metric prefix conversions. The API requires no external dependencies beyond the `DevExpress.Document.Processor` NuGet package and works identically on .NET 6+ and .NET Framework 4.6.2+.

## When to Use This Skill

Use this skill when you need to:

- Convert distances or lengths (meters, feet, miles, kilometers, inches, yards, nautical miles)
- Convert mass/weight values (grams, kilograms, pounds, ounces, metric tons)
- Convert temperatures (Celsius, Fahrenheit, Kelvin)
- Convert area measurements (square meters, square feet, acres, hectares, square miles)
- Convert volumes (liters, gallons, cubic meters, cubic feet, fluid ounces, pints, quarts)
- Convert speeds (km/h, m/s, mph, knots, feet per second)
- Convert between metric prefixes (kilo, mega, milli, micro, etc.)
- Combine unit values with arithmetic (e.g., 5 feet + 4 inches, then convert to meters)
- Build type-safe unit-aware calculations without manual conversion factors

## Prerequisites & Installation

### NuGet Packages

| Package | Purpose |
|---------|---------|
| `DevExpress.Document.Processor` | Unit Conversion API (included in the package) |

### .NET (6/7/8+)

```bash
dotnet add package DevExpress.Document.Processor
```

### .NET Framework (4.6.2+)

```
Install-Package DevExpress.Document.Processor
```

Or add a direct reference to `DevExpress.Docs.v25.2.dll`.

**Important**: All DevExpress packages in a project must share the same version number. A valid DevExpress license is required.

## Before You Start — Ask the Developer

Before generating code, ask these questions to avoid rework:

### General Questions
1. **Target framework**: Are you using .NET 8+, .NET 6/7, or .NET Framework 4.x?
2. **New or existing project?**: Creating new or adding to existing?
3. **Hosting model**: Console app, ASP.NET Core, Blazor, MAUI, WinForms, or something else?

### Unit Conversion-Specific Questions
4. **Measurement category**: Which category? (length / weight / temperature / area / volume / speed / metric prefix)
5. **Conversion direction**: Metric to US, US to metric, between metric prefixes, or both directions?

> **Rule**: If the developer's answer is ambiguous or missing, ask before generating code. Do not guess.

## Component Overview

The Unit Conversion API provides two usage patterns:

- **Fluent extension methods** on numeric types (`double`, `int`, `float`) — create a `QuantityValue<T>`, then call `To{Unit}()` to convert. `QuantityValue<T>` implicitly converts to `double`, so you can use it directly in arithmetic or string formatting.
- **Static converter classes** accessed via `Units.*` — call `.Convert(value, fromUnit, toUnit)` for direct scalar conversion without creating `QuantityValue<T>` objects.

Key classes:

- **`QuantityValue<T>`** (`struct`) — holds a value in a specific unit. Supports `+`, `-`, `*`, `/` operators. Implicitly converts to `double`.
- **`Units`** (`static class`) — entry point for converter instances: `Units.Distance`, `Units.Mass`, `Units.Temperature`, `Units.Area`, `Units.Volume`, `Units.Speed`, `Units.Metric`.
- **`Distance`, `Mass`, `Temperature`, `Area`, `Volume`, `Speed`, `MetricPrefix`** — enums listing available unit values for each category.
- **`DistanceUnitsConverter`, `MassUnitsConverter`, etc.** — converter classes returned by `Units.*` properties.

### Core Entry Point — Fluent API

```csharp
using DevExpress.UnitConversion;

// Combine feet and inches, then convert to meters
QuantityValue<Distance> height = (5.0).Feet() + (4.0).Inches();
double meters = height.ToMeters().Value;   // explicit: use .Value
double meters2 = height.ToMeters();        // implicit: QuantityValue<T> → double
Console.WriteLine($"Height: {meters:F3} m");
```

### Core Entry Point — Units.Convert() API

```csharp
using DevExpress.UnitConversion;

// Convert 100 feet to meters directly
double result = Units.Distance.Convert(100.0, Distance.Foot, Distance.Meter);

// Convert 100°C to Fahrenheit
double fahrenheit = Units.Temperature.Convert(100.0, Temperature.Celsius, Temperature.Fahrenheit);

// Convert metric prefix: 5 km → m  (Kilo → None)
double meters = Units.Metric.Convert(5.0, MetricPrefix.Kilo, MetricPrefix.None);
```

## Documentation & Navigation Guide

### Getting Started
Refer to [references/getting-started.md](references/getting-started.md)

When you need to:
- Install the NuGet package and set up the namespace
- Perform your first conversion (distance, weight, temperature)
- Understand both fluent and `Units.Convert()` patterns
- See all measurement categories with one example each
- Combine values with arithmetic operators

## Quick Start Example

```csharp
using DevExpress.UnitConversion;

// Distance: 5 feet 11 inches to meters
QuantityValue<Distance> height = (5.0).Feet() + (11.0).Inches();
Console.WriteLine($"Height: {height.ToMeters().Value:F3} m ({height.ToCentimeters().Value:F1} cm)");

// Mass: pounds to kilograms
QuantityValue<Mass> weight = (180.0).Pounds();
Console.WriteLine($"Weight: {weight.ToKilograms().Value:F2} kg");

// Temperature: Fahrenheit to Celsius and Kelvin
QuantityValue<Temperature> bodyTemp = (98.6).Fahrenheit();
Console.WriteLine($"Body temp: {bodyTemp.ToCelsius().Value:F1}°C / {bodyTemp.ToKelvin().Value:F2} K");

// Speed: km/h to mph
double mph = Units.Speed.Convert(100.0, Speed.MetersPerHour, Speed.MilesPerHour);
Console.WriteLine($"100 km/h = {mph:F2} mph");

// Metric prefix: 5 kilo → mega
double mega = Units.Metric.Convert(5.0, MetricPrefix.Kilo, MetricPrefix.Mega);
Console.WriteLine($"5 kilo = {mega} mega");
```

### What This Does
Demonstrates the two API patterns: fluent extension methods returning `QuantityValue<T>` (distance, mass, temperature) and direct scalar conversion via `Units.*.Convert()` (speed, metric prefix). Both patterns work identically on .NET and .NET Framework.

## Key Properties & API Surface

### QuantityValue\<T\>

| Property/Operator | Type | Description |
|-------------------|------|-------------|
| `Value` | `double` | Numeric value in the current unit |
| implicit `double` | `double` | Implicit conversion — use directly in math/strings |
| `+` / `-` | `QuantityValue<T>` | Add or subtract same-category quantities |
| `*` / `/` | `QuantityValue<T>` | Multiply or divide by a scalar |

### Units (static class)

| Property | Type | Description |
|----------|------|-------------|
| `Units.Distance` | `DistanceUnitsConverter` | Distance/length conversions |
| `Units.Mass` | `MassUnitsConverter` | Mass/weight conversions |
| `Units.Temperature` | `TemperatureUnitsConverter` | Temperature conversions |
| `Units.Area` | `AreaUnitsConverter` | Area conversions |
| `Units.Volume` | `VolumeUnitsConverter` | Volume conversions |
| `Units.Speed` | `SpeedUnitsConverter` | Speed conversions |
| `Units.Metric` | `MetricUnitsConverter` | Metric prefix conversions |

### BaseUnitsConverter\<T\>.Convert()

```csharp
double Convert(double value, T from, T to)      // single value
void   Convert(double[] values, T from, T to)   // in-place array conversion
```

### Distance Enum Values (most common)

`Distance.Meter`, `Distance.Foot`, `Distance.Inch`, `Distance.Yard`, `Distance.MileStatute`, `Distance.MileNautical`, `Distance.Kilometer` (via MetricPrefix), `Distance.Angstrom`

> Note: Kilometer is not a direct `Distance` enum value. Use the `MetricPrefix` converter or the `.Kilometers()` extension method which internally handles the prefix.

### Area Enum Values

`Area.SquareMeter`, `Area.SquareMile`, `Area.SquareMileNautical`, `Area.Hectare`, `Area.SquareFoot`, `Area.SquareInch`, `Area.SquareYard`, `Area.AcreInternational`, `Area.AcreStatute`, `Area.Are`, `Area.Morgen`

> **IMPORTANT — `Area.SquareKilometer` does not exist in this enum.**
> To convert to or from square kilometers, convert via `Area.SquareMeter` first, then divide or multiply by 1,000,000:
>
> ```csharp
> // 2.5 square miles → square kilometers
> double squareMeters = Units.Area.Convert(2.5, Area.SquareMile, Area.SquareMeter);
> double squareKm = squareMeters / 1_000_000.0;
> Console.WriteLine($"2.5 sq mi = {squareKm:F4} km²");
>
> // 10 km² → square miles
> double sqMiles = Units.Area.Convert(10.0 * 1_000_000.0, Area.SquareMeter, Area.SquareMile);
> Console.WriteLine($"10 km² = {sqMiles:F4} sq mi");
> ```

### Mass Enum Values

`Mass.Gram`, `Mass.Pound`, `Mass.Ounce`, `Mass.Ton`, `Mass.TonImperial`, `Mass.Stone`, `Mass.Slug`

> Note: Kilogram is not a direct `Mass` enum value in `Units.Mass.Convert()`. Use the fluent `.Kilograms()` extension method, or `Units.Metric.Convert(value, MetricPrefix.None, MetricPrefix.Kilo)` after getting grams.

### Temperature Enum Values

`Temperature.Celsius`, `Temperature.Fahrenheit`, `Temperature.Kelvin`, `Temperature.Rankine`, `Temperature.Reaumur`

### Speed Enum Values

`Speed.MetersPerHour`, `Speed.MetersPerSecond`, `Speed.MilesPerHour`, `Speed.Knot`, `Speed.KnotAdmiralty`

### MetricPrefix Enum Values

`MetricPrefix.Tera`, `MetricPrefix.Giga`, `MetricPrefix.Mega`, `MetricPrefix.Kilo`, `MetricPrefix.Hecto`, `MetricPrefix.Deca`, `MetricPrefix.None`, `MetricPrefix.Deci`, `MetricPrefix.Centi`, `MetricPrefix.Milli`, `MetricPrefix.Micro`, `MetricPrefix.Nano`, `MetricPrefix.Pico`, `MetricPrefix.Femto`, `MetricPrefix.Atto`

## Common Patterns

### Fluent Conversion (simple)

```csharp
// 100 meters to feet
double feet = (100.0).Meters().ToFeet().Value;
Console.WriteLine($"100 m = {feet:F2} ft");   // 328.08 ft
```

### Fluent Conversion (compound)

```csharp
// 6 feet 2 inches to centimeters
QuantityValue<Distance> height = (6.0).Feet() + (2.0).Inches();
Console.WriteLine($"6'2\" = {height.ToCentimeters().Value:F1} cm");   // 187.9 cm
```

### Direct Scalar Conversion

```csharp
// 100°C to Fahrenheit — no QuantityValue needed
double f = Units.Temperature.Convert(100.0, Temperature.Celsius, Temperature.Fahrenheit);
Console.WriteLine($"100°C = {f:F1}°F");   // 212.0°F
```

### Batch Array Conversion

```csharp
// Convert an array of values in-place
double[] distances = { 1.0, 5.0, 10.0, 42.195 };
Units.Distance.Convert(distances, Distance.Meter, Distance.Foot);
// distances[] now contains feet
```

### Metric Prefix Conversion

```csharp
// Convert 760 mmHg to hectopascals using MetricUnitsConverter
QuantityValue<Pressure> pressure = (760.0).MmHg();
double pascals = pressure.ToPascals();
var prefixConverter = new MetricUnitsConverter();
double hPa = prefixConverter.Convert(pascals, MetricPrefix.None, MetricPrefix.Hecto);
Console.WriteLine($"760 mmHg = {hPa:F1} hPa");
```

### Volume Conversion

```csharp
double gallons = Units.Volume.Convert(10.0, Volume.Liter, Volume.Gallon);
Console.WriteLine($"10 L = {gallons:F3} US gallons");
```

## Troubleshooting

| Symptom | Cause | Solution |
|---------|-------|----------|
| Compile error: type `Weight` not found | Wrong type name | The enum is `Mass`, not `Weight`. Use `QuantityValue<Mass>` and `.Pounds()`, `.Grams()` etc. |
| Compile error: `Distance.Kilometer` not found | Enum member absent | `Kilometer` is not in the `Distance` enum. Use `.Kilometers()` extension method on a double instead. |
| Compile error: `Area.SquareKilometer` not found | Enum member absent | `SquareKilometer` is not in the `Area` enum. Convert via `Area.SquareMeter` then divide/multiply by 1,000,000. |
| Ambiguous extension method invocation | Conflicting namespace | Ensure only `using DevExpress.UnitConversion;` is present; remove conflicting using directives |
| Incorrect conversion result | Wrong extension method | `.Feet()` creates US feet, `.Meters()` creates SI meters — double-check the method name |
| Build error: type not found | Package not installed | Run `dotnet add package DevExpress.Document.Processor` |
| License error at runtime | Missing DevExpress license | Register license per the DevExpress installation guide |
| Version mismatch error | Mixed DX package versions | All DevExpress packages must use the same version |
| Cannot add `Distance` + `Mass` | Type mismatch | Only add/subtract `QuantityValue<T>` of the same `T`. Mix categories only after extracting `.Value`. |

## Constraints & Rules

CRITICAL — follow these rules in every interaction:

1. **Build verification**: After making changes, verify the project builds with `dotnet build`. Check for errors before reporting success.
2. **NuGet packages**: Use only `DevExpress.Document.Processor`. Do not guess other package names.
3. **Namespace imports**: Always include `using DevExpress.UnitConversion;`. Never assume it exists.
4. **Version consistency**: All DevExpress packages must use the same version. Do not mix.
5. **Correct enum name**: The mass/weight category is `Mass` (not `Weight`). `QuantityValue<Mass>`, not `QuantityValue<Weight>`.
6. **License**: DevExpress requires a valid license. Remind the developer if they hit license-related build errors.
7. **No destructive changes**: Preserve existing code structure. Only add or modify what is necessary.
8. **Framework detection**: Check the project's .csproj for target framework. The API works on both .NET and .NET Framework with the same code.
9. **Type safety**: Only use `+`/`-` operators on `QuantityValue<T>` instances with the same `T`. Never mix categories.

## Using DevExpress Documentation MCP

If the DxDocs MCP server is available, use it to supplement this skill:

- **Search**: Use `devexpress_docs_search` with technology "Unit Conversion API" and your question.
- **Fetch**: Use `devexpress_docs_get_content` with a documentation URL to get full article content.

**When to use MCP vs. built-in references:**
- **Built-in references**: Getting started, common patterns, supported categories, troubleshooting.
- **MCP search**: Full list of extension methods for less common units (pressure, energy, force, magnetism, information), version-specific changes, or uncommon features.
- **Always MCP for**: Exact extension method names for units not listed here (e.g., pressure units like `.MmHg()`, `.Pascals()`).

---

## Next Steps

Start with **[Getting Started](references/getting-started.md)** to install the package and run your first conversion, then explore specific categories through the examples above.
