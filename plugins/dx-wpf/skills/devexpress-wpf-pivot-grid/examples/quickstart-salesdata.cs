// Sample data for the DevExpress WPF Pivot Grid quickstart.
// Companion to quickstart-mainview.xaml + quickstart-mainwindow.xaml.cs.
//
// The Pivot Grid binds via PivotGridControl.DataSource, which accepts
// any IEnumerable, DataTable, IListSource, server-mode source, or OLAP
// source. This sample uses a plain List<Sale> — the simplest path.

using System;
using System.Collections.Generic;

namespace DevExpressPivotGridQuickstart;

public class Sale
{
    public string Country { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
}

public static class SalesData
{
    public static List<Sale> Build() => new()
    {
        // 2024
        new Sale { Country = "USA",    Category = "Beverages", OrderDate = new DateTime(2024,  1, 15), Amount = 1200m },
        new Sale { Country = "USA",    Category = "Beverages", OrderDate = new DateTime(2024,  6, 10), Amount =  900m },
        new Sale { Country = "USA",    Category = "Produce",   OrderDate = new DateTime(2024,  3,  5), Amount =  450m },
        new Sale { Country = "USA",    Category = "Dairy",     OrderDate = new DateTime(2024,  8, 22), Amount = 1800m },
        new Sale { Country = "Canada", Category = "Beverages", OrderDate = new DateTime(2024,  2, 20), Amount =  800m },
        new Sale { Country = "Canada", Category = "Produce",   OrderDate = new DateTime(2024,  9, 12), Amount = 1500m },
        new Sale { Country = "Canada", Category = "Dairy",     OrderDate = new DateTime(2024, 11,  4), Amount = 1100m },
        new Sale { Country = "France", Category = "Beverages", OrderDate = new DateTime(2024,  5, 18), Amount = 1300m },
        new Sale { Country = "Japan",  Category = "Produce",   OrderDate = new DateTime(2024,  7,  9), Amount =  850m },

        // 2025
        new Sale { Country = "USA",    Category = "Beverages", OrderDate = new DateTime(2025,  2, 14), Amount = 1500m },
        new Sale { Country = "USA",    Category = "Produce",   OrderDate = new DateTime(2025,  6,  3), Amount =  600m },
        new Sale { Country = "USA",    Category = "Dairy",     OrderDate = new DateTime(2025, 10,  1), Amount = 2200m },
        new Sale { Country = "Canada", Category = "Beverages", OrderDate = new DateTime(2025,  4,  8), Amount =  950m },
        new Sale { Country = "Canada", Category = "Dairy",     OrderDate = new DateTime(2025,  9, 16), Amount = 1700m },
        new Sale { Country = "France", Category = "Beverages", OrderDate = new DateTime(2025,  4,  8), Amount = 1100m },
        new Sale { Country = "France", Category = "Produce",   OrderDate = new DateTime(2025,  7, 22), Amount =  700m },
        new Sale { Country = "France", Category = "Dairy",     OrderDate = new DateTime(2025, 12,  1), Amount = 1400m },
        new Sale { Country = "Japan",  Category = "Produce",   OrderDate = new DateTime(2025, 11,  3), Amount =  650m },
        new Sale { Country = "Japan",  Category = "Beverages", OrderDate = new DateTime(2025,  8, 25), Amount =  900m },
    };
}
