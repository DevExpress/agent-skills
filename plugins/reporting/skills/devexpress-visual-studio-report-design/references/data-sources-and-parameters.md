# Data Sources and Parameters

## SqlDataSource Setup

```csharp
// Step 2 (new):
this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);

// Step 4 (configure):
this.sqlDataSource1.ConnectionName = "NorthwindConnectionString";
this.sqlDataSource1.Name           = "sqlDataSource1";
// Queries, columns, joins are configured as local objects in Step 1 and
// added to sqlDataSource1.Queries here.

// Step 5 (report-level):
this.DataSource  = this.sqlDataSource1;
this.DataMember  = "Customers"; // required when multiple queries exist
this.ComponentStorage.AddRange(
    new System.ComponentModel.IComponent[] { this.sqlDataSource1 });
```

## Query FilterString with Parameters

```csharp
selectQuery1.FilterString =
    "[Orders.OrderDate] Between(?orderDates_Start, ?orderDates_End)";
```

## Master-Detail SqlDataSource Relations

```csharp
// Local objects (Step 1):
DevExpress.DataAccess.Sql.MasterDetailInfo masterDetailInfo1 =
    new DevExpress.DataAccess.Sql.MasterDetailInfo();
DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo1 =
    new DevExpress.DataAccess.Sql.RelationColumnInfo();

// Configuration (Step 4):
masterDetailInfo1.DetailQueryName = "Orders";
relationColumnInfo1.NestedKeyColumn  = "CustomerId"; // FK in child query
relationColumnInfo1.ParentKeyColumn  = "Id";         // PK in parent query
masterDetailInfo1.KeyColumns.Add(relationColumnInfo1);
masterDetailInfo1.MasterQueryName = "Customers";
this.sqlDataSource1.Relations.AddRange(
    new DevExpress.DataAccess.Sql.MasterDetailInfo[] { masterDetailInfo1 });

// DetailReportBand uses the relation path as DataMember:
this.DetailReport.DataSource = this.sqlDataSource1;
this.DetailReport.DataMember = "Customers.CustomersOrders_1";
// The relation path format is: "<MasterQueryName>.<RelationName>"
```

## Basic Parameter

```csharp
// Step 2:
this.OrderIdParameter = new DevExpress.XtraReports.Parameters.Parameter();

// Step 4:
this.OrderIdParameter.Name  = "OrderIdParameter";
this.OrderIdParameter.Type  = typeof(int);
this.OrderIdParameter.Value = 1;

// Step 5:
this.Parameters.AddRange(
    new DevExpress.XtraReports.Parameters.Parameter[] { this.OrderIdParameter });
this.RequestParameters = false; // skip parameter dialog; use defaults
```

## Date Range Parameter

```csharp
// Step 2:
this.orderDates       = new DevExpress.XtraReports.Parameters.Parameter();
this.orderDates_Start = new DevExpress.XtraReports.Parameters.RangeStartParameter();
this.orderDates_End   = new DevExpress.XtraReports.Parameters.RangeEndParameter();

// Step 4:
this.orderDates.Description = "Order Dates";
this.orderDates.Name        = "orderDates";
this.orderDates.Type        = typeof(System.DateTime);
this.orderDates.ValueSourceSettings =
    new DevExpress.XtraReports.Parameters.RangeParametersSettings(
        this.orderDates_Start, this.orderDates_End);
this.orderDates_Start.Name      = "orderDates_Start";
this.orderDates_Start.ValueInfo = "2024-01-01";
this.orderDates_End.Name        = "orderDates_End";
this.orderDates_End.ValueInfo   = "2024-12-31";

// Step 5:
this.Parameters.AddRange(
    new DevExpress.XtraReports.Parameters.Parameter[] { this.orderDates });

// In query FilterString, reference the child parameters:
selectQuery1.FilterString =
    "[OrderDate] Between(?orderDates_Start, ?orderDates_End)";
```

## Field Declarations for Data Source + Parameters

```csharp
private DevExpress.DataAccess.Sql.SqlDataSource              sqlDataSource1;
private DevExpress.XtraReports.Parameters.Parameter          orderDates;
private DevExpress.XtraReports.Parameters.RangeStartParameter orderDates_Start;
private DevExpress.XtraReports.Parameters.RangeEndParameter   orderDates_End;
private DevExpress.XtraReports.Parameters.Parameter          OrderIdParameter;
```
