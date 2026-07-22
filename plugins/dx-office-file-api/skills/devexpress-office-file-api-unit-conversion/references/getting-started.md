# Getting Started with DevExpress Unit Conversion API

This guide walks you through installing and using the DevExpress Unit Conversion API for the first time. The same code works on .NET 8+ and .NET Framework 4.6.2+ without modification.

## System Requirements

- .NET 8.0 / 9.0 / 10.0+ or .NET Framework 4.6.2+
- Visual Studio 2022+ (recommended) or JetBrains Rider
- A valid DevExpress license

## Installation

### Step 1: Install the Package

**.NET CLI:**
```bash
dotnet add package DevExpress.Document.Processor
```

**Package Manager Console:**
```
Install-Package DevExpress.Document.Processor
```

### Step 2: Add the Namespace

```csharp
using DevExpress.UnitConversion;
```

A valid DevExpress license is required. If you installed via the DevExpress installer, the license is registered automatically. For NuGet-only installations, follow the DevExpress license deployment guide.

## Two API Patterns

The Unit Conversion API provides two complementary usage patterns. You can mix them freely.

### Pattern 1: Fluent Extension Methods

Extension methods on numeric types create a `QuantityValue<T>` — a type-safe value paired with its unit. Call `To{Unit}()` to convert. `QuantityValue<T>` implicitly converts to `double`, so you can use it directly in format strings, arithmetic, and assignments.

```csharp
using DevExpress.UnitConversion;

// Create a QuantityValue<Distance> and convert
QuantityValue<Distance> d = (100.0).Meters();
double feet = d.ToFeet().Value;        // explicit: .Value
double feet2 = d.ToFeet();            // implicit: QuantityValue<Distance> → double
Console.WriteLine($"100 m = {feet:F2} ft");
```

### Pattern 2: Units.Convert() — Direct Scalar Conversion

Use static `Units.*` properties to get a converter instance, then call `Convert(value, fromUnit, toUnit)`. No `QuantityValue<T>` is created. This is the most direct pattern for simple scalar conversions.

```csharp
using DevExpress.UnitConversion;

// Convert 100°C to Fahrenheit
double f = Units.Temperature.Convert(100.0, Temperature.Celsius, Temperature.Fahrenheit);
Console.WriteLine($"100°C = {f} °F");   // 212
```

## Your First Conversion — Distance

```csharp
using DevExpress.UnitConversion;

// Combine feet and inches, then convert to meters (fluent pattern)
QuantityValue<Distance> height = (5.0).Feet() + (4.0).Inches();
Console.WriteLine($"5'4\" = {height.ToMeters().Value:F3} m");      // 1.626 m
Console.WriteLine($"5'4\" = {height.ToCentimeters().Value:F1} cm"); // 162.6 cm
Console.WriteLine($"5'4\" = {height.ToElls().Value:F3} ells");

// Using Units.Convert() directly
double meters = Units.Distance.Convert(100.0, Distance.Foot, Distance.Meter);
Console.WriteLine($"100 ft = {meters:F3} m");   // 30.480 m
```

## All Categories — One Example Each

### Distance / Length

```csharp
using DevExpress.UnitConversion;

// Fluent: km to miles
QuantityValue<Distance> marathon = (42.195).Kilometers();
Console.WriteLine($"Marathon: {marathon.ToMiles().Value:F3} miles");   // 26.219 miles

// Direct: foot to meter
double m = Units.Distance.Convert(1.0, Distance.Foot, Distance.Meter);
Console.WriteLine($"1 ft = {m:F4} m");   // 0.3048 m

// Compound: 6 feet 2 inches to centimeters
var tall = (6.0).Feet() + (2.0).Inches();
Console.WriteLine($"6'2\" = {tall.ToCentimeters().Value:F1} cm");   // 187.9 cm
```

**Key extension methods**: `.Meters()`, `.Kilometers()`, `.Centimeters()`, `.Millimeters()`, `.Feet()`, `.Inches()`, `.Yards()`, `.Miles()`, `.NauticalMiles()`, `.Ells()`

**Key `Distance` enum values**: `Distance.Meter`, `Distance.Foot`, `Distance.Inch`, `Distance.Yard`, `Distance.MileStatute`, `Distance.MileNautical`

### Mass / Weight

> The type parameter is `Mass` — use `QuantityValue<Mass>`, not `QuantityValue<Weight>`.

```csharp
using DevExpress.UnitConversion;

// Fluent: pounds to kilograms
QuantityValue<Mass> w = (175.0).Pounds();
Console.WriteLine($"175 lbs = {w.ToKilograms().Value:F2} kg");   // 79.38 kg
Console.WriteLine($"175 lbs = {w.ToGrams().Value:F0} g");        // 79379 g

// Fluent: kilograms to pounds and ounces
QuantityValue<Mass> w2 = (70.0).Kilograms();
Console.WriteLine($"70 kg = {w2.ToPounds().Value:F2} lbs");      // 154.32 lbs
Console.WriteLine($"70 kg = {w2.ToOunces().Value:F1} oz");       // 2469.1 oz

// Direct: grams to pounds
double lbs = Units.Mass.Convert(500.0, Mass.Gram, Mass.Pound);
Console.WriteLine($"500 g = {lbs:F4} lbs");   // 1.1023 lbs
```

**Key extension methods**: `.Grams()`, `.Kilograms()`, `.Milligrams()`, `.Pounds()`, `.Ounces()`, `.Tons()`

**Key `Mass` enum values**: `Mass.Gram`, `Mass.Pound`, `Mass.Ounce`, `Mass.Ton`, `Mass.TonImperial`, `Mass.Slug`, `Mass.Stone`

> Note: `Mass.Kilogram` is not in the enum. The `.Kilograms()` extension method handles this internally. For `Units.Mass.Convert()`, convert from/to `Mass.Gram` and scale by 1000 if needed, or use the fluent API.

### Temperature

```csharp
using DevExpress.UnitConversion;

// Fluent: body temperature
QuantityValue<Temperature> body = (98.6).Fahrenheit();
Console.WriteLine($"98.6°F = {body.ToCelsius().Value:F1}°C");    // 37.0°C
Console.WriteLine($"98.6°F = {body.ToKelvin().Value:F2} K");     // 310.15 K

// Direct: Celsius to Fahrenheit
double boiling = Units.Temperature.Convert(100.0, Temperature.Celsius, Temperature.Fahrenheit);
Console.WriteLine($"100°C = {boiling}°F");   // 212

// Direct: Kelvin to Celsius
double absZero = Units.Temperature.Convert(0.0, Temperature.Kelvin, Temperature.Celsius);
Console.WriteLine($"0 K = {absZero}°C");     // -273.15
```

**Key extension methods**: `.Celsius()`, `.Fahrenheit()`, `.Kelvin()`

**`Temperature` enum values**: `Temperature.Celsius`, `Temperature.Fahrenheit`, `Temperature.Kelvin`, `Temperature.Rankine`, `Temperature.Reaumur`

### Area

```csharp
using DevExpress.UnitConversion;

// Fluent: square meters to square feet
QuantityValue<Area> room = (25.0).SquareMeters();
Console.WriteLine($"25 m² = {room.ToSquareFeet().Value:F2} ft²");   // 269.10 ft²

// Direct: acres to hectares
double hectares = Units.Area.Convert(10.0, Area.AcreStatute, Area.Hectare);
Console.WriteLine($"10 acres = {hectares:F4} ha");   // 4.0469 ha
```

**Key extension methods**: `.SquareMeters()`, `.SquareFeet()`, `.SquareInches()`, `.SquareYards()`, `.SquareMiles()`, `.Hectares()`, `.Acres()`

**Key `Area` enum values**: `Area.SquareMeter`, `Area.SquareFoot`, `Area.SquareInch`, `Area.SquareYard`, `Area.SquareMile`, `Area.Hectare`, `Area.AcreStatute`, `Area.AcreInternational`

### Volume

```csharp
using DevExpress.UnitConversion;

// Fluent: liters to US gallons
QuantityValue<Volume> tank = (50.0).Liters();
Console.WriteLine($"50 L = {tank.ToGallons().Value:F3} US gal");   // 13.209 gal

// Direct: cubic feet to liters
double liters = Units.Volume.Convert(1.0, Volume.CubicFoot, Volume.Liter);
Console.WriteLine($"1 ft³ = {liters:F4} L");   // 28.3168 L

// Direct: oil barrel to liters
double bblLiters = Units.Volume.Convert(1.0, Volume.OilBarrel, Volume.Liter);
Console.WriteLine($"1 bbl = {bblLiters:F2} L");   // 158.99 L
```

**Key extension methods**: `.Liters()`, `.Milliliters()`, `.CubicMeters()`, `.CubicFeet()`, `.CubicInches()`, `.Gallons()`, `.Quarts()`, `.Pints()`, `.OuncesFl()`

**Key `Volume` enum values**: `Volume.Liter`, `Volume.CubicMeter`, `Volume.CubicFoot`, `Volume.CubicInch`, `Volume.Gallon`, `Volume.GallonImperial`, `Volume.Quart`, `Volume.QuartImperial`, `Volume.Pint`, `Volume.PintImperial`, `Volume.OunceFluid`, `Volume.OilBarrel`

### Speed

```csharp
using DevExpress.UnitConversion;

// Direct: km/h to mph
double mph = Units.Speed.Convert(100.0, Speed.MetersPerHour, Speed.MilesPerHour);
Console.WriteLine($"100 km/h = {mph:F2} mph");   // 62.14 mph

// Direct: knot to m/s
double ms = Units.Speed.Convert(1.0, Speed.Knot, Speed.MetersPerSecond);
Console.WriteLine($"1 knot = {ms:F4} m/s");   // 0.5144 m/s

// Fluent: m/s to mph
QuantityValue<Speed> wind = (10.0).MetersPerSecond();
Console.WriteLine($"10 m/s = {wind.ToMilesPerHour().Value:F2} mph");
```

**Key `Speed` enum values**: `Speed.MetersPerHour`, `Speed.MetersPerSecond`, `Speed.MilesPerHour`, `Speed.Knot`, `Speed.KnotAdmiralty`

### Metric Prefixes

Use `Units.Metric` to convert between metric multipliers (e.g., kilo, mega, milli). This is useful when converting a raw value in a base unit to a scaled unit (e.g., pascals to hectopascals).

```csharp
using DevExpress.UnitConversion;

// 5000 → kilo (5000 base units = 5 kilo-units)
double kilo = Units.Metric.Convert(5000.0, MetricPrefix.None, MetricPrefix.Kilo);
Console.WriteLine($"5000 = {kilo} kilo");   // 5

// 5 kilo → mega (5 kilo-units = 0.005 mega-units)
double mega = Units.Metric.Convert(5.0, MetricPrefix.Kilo, MetricPrefix.Mega);
Console.WriteLine($"5 kilo = {mega} mega");   // 0.005

// Practical: convert pascals to hectopascals
double pa = 101325.0;  // standard atmosphere in pascals
double hPa = Units.Metric.Convert(pa, MetricPrefix.None, MetricPrefix.Hecto);
Console.WriteLine($"{pa} Pa = {hPa} hPa");   // 1013.25 hPa
```

**`MetricPrefix` enum values**: `MetricPrefix.Tera`, `MetricPrefix.Giga`, `MetricPrefix.Mega`, `MetricPrefix.Kilo`, `MetricPrefix.Hecto`, `MetricPrefix.Deca`, `MetricPrefix.None`, `MetricPrefix.Deci`, `MetricPrefix.Centi`, `MetricPrefix.Milli`, `MetricPrefix.Micro`, `MetricPrefix.Nano`, `MetricPrefix.Pico`, `MetricPrefix.Femto`, `MetricPrefix.Atto`

## Combining Values with Arithmetic

`QuantityValue<T>` supports `+`, `-`, `*`, `/` between compatible (same `T`) instances:

```csharp
using DevExpress.UnitConversion;

// Add different units in the same category — result inherits the first operand's unit
QuantityValue<Distance> total = (42.0).Kilometers() + (195.0).Meters();
Console.WriteLine($"Marathon: {total.ToMiles().Value:F3} miles");   // 26.219 miles

// Multiply by a scalar
QuantityValue<Distance> lap = (400.0).Meters();
QuantityValue<Distance> race = lap * 10;
Console.WriteLine($"10 laps of 400m = {race.ToKilometers().Value} km");   // 4 km

// Cannot add different categories — compile error:
// QuantityValue<Distance> d = (1.0).Meters();
// QuantityValue<Mass> m = (1.0).Kilograms();
// var invalid = d + m;   // ERROR: type mismatch
```

## What to Learn Next

- See [examples/quickstart.cs](../examples/quickstart.cs) for a complete compilable console application covering all 7 categories.
- For additional unit categories (pressure, energy, force, power, magnetism, time, information, binary prefixes), use the DevExpress Documentation MCP or browse the `DevExpress.UnitConversion` namespace in the API reference.
