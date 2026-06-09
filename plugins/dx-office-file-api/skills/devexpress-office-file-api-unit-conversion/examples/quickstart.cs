// DevExpress Unit Conversion API — Quick Start Console Application
// Demonstrates all 7 measurement categories using both API patterns:
//   1. Fluent extension methods  →  (value).Unit().ToOtherUnit().Value
//   2. Direct scalar conversion  →  Units.Category.Convert(value, From, To)
//
// Prerequisites:
//   dotnet add package DevExpress.Document.Processor
//
// Namespace:
//   using DevExpress.UnitConversion;
//
// Works on .NET 6+ and .NET Framework 4.6.2+ without modification.

using DevExpress.UnitConversion;

Console.WriteLine("=== DevExpress Unit Conversion API — Quick Start ===");
Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// 1. DISTANCE / LENGTH
//    Type parameter: Distance  |  Converter: Units.Distance
//    Common enum values: Distance.Meter, Distance.Foot, Distance.Inch,
//                        Distance.Yard, Distance.MileStatute, Distance.MileNautical
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- Distance / Length ---");

// Fluent: combine feet + inches, convert to meters and centimeters
QuantityValue<Distance> height = (5.0).Feet() + (11.0).Inches();
Console.WriteLine($"5'11\" = {height.ToMeters().Value:F3} m  ({height.ToCentimeters().Value:F1} cm)");

// Fluent: marathon distance in km → miles
QuantityValue<Distance> marathon = (42.195).Kilometers();
Console.WriteLine($"42.195 km = {marathon.ToMiles().Value:F3} miles");

// Fluent: metric prefix chain — millimeters → meters → kilometers
QuantityValue<Distance> road = (1500.0).Millimeters();
Console.WriteLine($"1500 mm = {road.ToMeters().Value} m = {road.ToKilometers().Value} km");

// Direct scalar: foot → meter
double oneFoot = Units.Distance.Convert(1.0, Distance.Foot, Distance.Meter);
Console.WriteLine($"1 ft = {oneFoot:F4} m  (direct Units.Distance.Convert)");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// 2. MASS / WEIGHT
//    Type parameter: Mass  (NOT Weight — Weight does not exist in this API)
//    Converter: Units.Mass
//    Key enum values: Mass.Gram, Mass.Pound, Mass.Ounce, Mass.Ton, Mass.Slug
//    Note: Mass.Kilogram is not in the enum. Use .Kilograms() extension method.
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- Mass / Weight ---");

// Fluent: pounds to kilograms and grams
QuantityValue<Mass> bodyWeight = (180.0).Pounds();
Console.WriteLine($"180 lbs = {bodyWeight.ToKilograms().Value:F2} kg  ({bodyWeight.ToGrams().Value:F0} g)");

// Fluent: kilograms to pounds and ounces
QuantityValue<Mass> parcel = (2.5).Kilograms();
Console.WriteLine($"2.5 kg = {parcel.ToPounds().Value:F3} lbs  ({parcel.ToOunces().Value:F2} oz)");

// Fluent: metric ton to pounds
QuantityValue<Mass> cargo = (1.0).Tons();
Console.WriteLine($"1 metric ton = {cargo.ToPounds().Value:F2} lbs");

// Direct scalar: grams to pounds
double lbs500g = Units.Mass.Convert(500.0, Mass.Gram, Mass.Pound);
Console.WriteLine($"500 g = {lbs500g:F4} lbs  (direct Units.Mass.Convert)");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// 3. TEMPERATURE
//    Type parameter: Temperature  |  Converter: Units.Temperature
//    Enum values: Temperature.Celsius, Temperature.Fahrenheit,
//                 Temperature.Kelvin, Temperature.Rankine, Temperature.Reaumur
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- Temperature ---");

// Fluent: body temperature F → C and K
QuantityValue<Temperature> bodyTemp = (98.6).Fahrenheit();
Console.WriteLine($"98.6°F = {bodyTemp.ToCelsius().Value:F1}°C  ({bodyTemp.ToKelvin().Value:F2} K)");

// Fluent: boiling point C → F and K
QuantityValue<Temperature> boiling = (100.0).Celsius();
Console.WriteLine($"100°C = {boiling.ToFahrenheit().Value:F1}°F  ({boiling.ToKelvin().Value:F2} K)");

// Direct scalar: Kelvin to Celsius (absolute zero)
double absZero = Units.Temperature.Convert(0.0, Temperature.Kelvin, Temperature.Celsius);
Console.WriteLine($"0 K = {absZero:F2}°C  (direct Units.Temperature.Convert)");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// 4. AREA
//    Type parameter: Area  |  Converter: Units.Area
//    Key enum values: Area.SquareMeter, Area.SquareFoot, Area.SquareInch,
//                     Area.SquareYard, Area.SquareMile, Area.Hectare,
//                     Area.AcreStatute, Area.AcreInternational
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- Area ---");

// Fluent: square meters to square feet
QuantityValue<Area> livingRoom = (25.0).SquareMeters();
Console.WriteLine($"25 m² = {livingRoom.ToSquareFeet().Value:F2} ft²");

// Fluent: acres to hectares (using fluent, then reading value)
QuantityValue<Area> field = (10.0).Acres();
Console.WriteLine($"10 acres = {field.ToHectares().Value:F4} ha");

// Direct scalar: statute acres to hectares
double ha = Units.Area.Convert(10.0, Area.AcreStatute, Area.Hectare);
Console.WriteLine($"10 statute acres = {ha:F4} ha  (direct Units.Area.Convert)");

// Direct scalar: square miles to square kilometers
double sqKm = Units.Area.Convert(1.0, Area.SquareMile, Area.SquareMeter) / 1_000_000.0;
Console.WriteLine($"1 sq mile ≈ {sqKm:F4} km²  (via SquareMeter / 1e6)");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// 5. VOLUME
//    Type parameter: Volume  |  Converter: Units.Volume
//    Key enum values: Volume.Liter, Volume.CubicMeter, Volume.CubicFoot,
//                     Volume.CubicInch, Volume.Gallon, Volume.GallonImperial,
//                     Volume.Quart, Volume.Pint, Volume.OunceFluid, Volume.OilBarrel
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- Volume ---");

// Fluent: liters to US gallons and pints
QuantityValue<Volume> tank = (50.0).Liters();
Console.WriteLine($"50 L = {tank.ToGallons().Value:F3} US gal  ({tank.ToPints().Value:F2} US pints)");

// Fluent: cubic feet to liters
QuantityValue<Volume> box = (1.0).CubicFeet();
Console.WriteLine($"1 ft³ = {box.ToLiters().Value:F4} L");

// Direct scalar: oil barrel to liters
double bblLiters = Units.Volume.Convert(1.0, Volume.OilBarrel, Volume.Liter);
Console.WriteLine($"1 oil barrel = {bblLiters:F2} L  (direct Units.Volume.Convert)");

// Direct scalar: US fluid ounces to milliliters
double ml = Units.Volume.Convert(12.0, Volume.OunceFluid, Volume.Liter) * 1000.0;
Console.WriteLine($"12 fl oz ≈ {ml:F1} mL");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// 6. SPEED
//    Type parameter: Speed  |  Converter: Units.Speed
//    Enum values: Speed.MetersPerHour, Speed.MetersPerSecond,
//                 Speed.MilesPerHour, Speed.Knot, Speed.KnotAdmiralty
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- Speed ---");

// Direct scalar: km/h to mph (MetersPerHour is km/h in this API)
double mph = Units.Speed.Convert(100.0, Speed.MetersPerHour, Speed.MilesPerHour);
Console.WriteLine($"100 km/h = {mph:F2} mph");

// Direct scalar: knot to m/s
double knot2ms = Units.Speed.Convert(1.0, Speed.Knot, Speed.MetersPerSecond);
Console.WriteLine($"1 knot = {knot2ms:F4} m/s");

// Direct scalar: m/s to mph
double windMph = Units.Speed.Convert(10.0, Speed.MetersPerSecond, Speed.MilesPerHour);
Console.WriteLine($"10 m/s = {windMph:F2} mph");

// Batch array conversion in-place
double[] speeds = { 30.0, 60.0, 90.0, 120.0 };  // km/h
Units.Speed.Convert(speeds, Speed.MetersPerHour, Speed.MilesPerHour);
Console.Write("Speed table (mph): ");
Console.WriteLine(string.Join(", ", Array.ConvertAll(speeds, s => $"{s:F1}")));

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// 7. METRIC PREFIXES
//    Converter: Units.Metric  |  Enum: MetricPrefix
//    Values: Tera, Giga, Mega, Kilo, Hecto, Deca, None (=1),
//            Deci, Centi, Milli, Micro, Nano, Pico, Femto, Atto
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- Metric Prefixes ---");

// kilo to mega: 5 kilo-units = 0.005 mega-units
double mega = Units.Metric.Convert(5.0, MetricPrefix.Kilo, MetricPrefix.Mega);
Console.WriteLine($"5 kilo = {mega} mega");

// none to milli: 1 base unit = 1000 milli-units
double milli = Units.Metric.Convert(1.0, MetricPrefix.None, MetricPrefix.Milli);
Console.WriteLine($"1 (none) = {milli} milli");

// Practical: standard atmosphere (101325 Pa) to hectopascals
double pa = 101325.0;
double hPa = Units.Metric.Convert(pa, MetricPrefix.None, MetricPrefix.Hecto);
Console.WriteLine($"{pa} Pa = {hPa} hPa  (standard atmosphere)");

// Practical: 5 GB to MB
double mb = Units.Metric.Convert(5.0, MetricPrefix.Giga, MetricPrefix.Mega);
Console.WriteLine($"5 Giga = {mb} Mega  (e.g., 5 GB = {mb} MB)");

Console.WriteLine();
Console.WriteLine("=== Done ===");
