# REPX & DFM Layout Cookbook

A report layout is one REPX (XtraReports XML) document. The same XML can live in two places:

| Storage | How | When |
|---|---|---|
| **Embedded in the `.dfm` (default)** | `TdxReport.Layout` / `TdxReportControl.Layout` streamed as `Layout.Strings = (...)`: one DFM string per REPX line. The Report Designer writes it there when you save at design time. | The layout ships with the form. |
| **Separate REPX** | A `.repx` file, a database BLOB, a resource, or a string loaded at run time: `Layout.LoadFromFile` / `LoadFromStream` / `Assign(BlobField)` / `Layout.Text := ...`. Save it back with `SaveToFile` / `SaveToStream` / `BlobField.Assign(Layout)`. | Users edit layouts, or layouts are versioned and deployed apart from the exe. |

The XML is identical in both forms. A `.repx` file can be embedded into a `.dfm` with `scripts/Set-DfmLayout.ps1`
(it applies the DFM string rules: `''`, `#NNNN` for non-ASCII, ` +` wrapping), and an embedded layout can be saved to
a file with `Layout.SaveToFile`. For hand-authoring, keep the source `.repx` files under `Layouts\` next to the project
and treat the `.dfm` block as generated.

Data-source XML is stored Base64-encoded. Keep it in `.xml` files and reference it as `Base64="@@B64:file.xml@@"`;
`scripts/Set-DfmLayout.ps1` expands the token when embedding into the `.dfm`. Use `-ExpandedRepxOut` to also write the
expanded standalone `.repx`.

## Root element

```xml
<?xml version="1.0" encoding="utf-8"?>
<XtraReportsLayoutSerializer SerializerVersion="26.1.0.0" Ref="1"
  ControlType="DevExpress.XtraReports.UI.XtraReport, DevExpress.XtraReports.v26.1, Version=26.1.0.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
  Name="CustomerList" DisplayName="CustomerList" Margins="100, 100, 100, 100" PageWidthF="850" PageHeightF="1100"
  Version="26.1" RequestParameters="false" Font="Arial, 9.75pt"
  FilterString="..." DataMember="Customers" DataSource="#Ref-0">
  <Parameters>...</Parameters>
  <Bands>...</Bands>
  <StyleSheet>...</StyleSheet>
  <ComponentStorage>
    <Item1 Ref="0" ObjectType="DevExpress.DataAccess.Json.JsonDataSource,DevExpress.DataAccess.v26.1" Name="jsonDataSource1" Base64="@@B64:jsonDataSource1.xml@@" />
  </ComponentStorage>
  <ObjectStorage>...</ObjectStorage>
</XtraReportsLayoutSerializer>
```

- **`Ref` values.** Unique integers across the whole file. `#Ref-N` may point forward, e.g. to `ObjectStorage`. It is used by
  `DataSource`, `ValueSourceSettings`, `Type`, `Parameter`, `StartBand`/`EndBand`, `DrillDownControl`, `TargetBand`,
  `BookmarkParent`, `FormattingRuleLinks/Value` and `LookUpValues/Value`.
- **Data-source `ObjectType`s:**
  - `DevExpress.DataAccess.Sql.SqlDataSource,DevExpress.DataAccess.v26.1`;
  - `DevExpress.DataAccess.Json.JsonDataSource,DevExpress.DataAccess.v26.1`;
  - `DevExpress.DataAccess.DataFederation.FederationDataSource,DevExpress.DataAccess.v26.1`.
  - Not `ObjectDataSource`: it gives no data on VCL.
- **The root `Name` doesn't matter to VCL.** Only `TdxReport.ReportName` (and `XRSubreport.ReportSourceUrl`) does.

### Format conventions (all DevExpress demo layouts)

- **Child order** of `<XtraReportsLayoutSerializer>`, as saved by the designer:
  `Extensions`, `FormattingRuleSheet`, `Parameters`, `CalculatedFields`, `Bands`, `CrossBandControls`,
  `ExportOptions`, `StyleSheet`, `Watermarks`, `ReportPrintOptions`, `ParameterPanelLayoutItems`, `LocalizationItems`,
  `ComponentStorage`, `ImageResources`, `ObjectStorage`. A different order also loaded, but follow this one.
- **Units.**
  - The default is hundredths of an inch (`Dpi` 100). Also used: `ReportUnit="Pixels" Dpi="96"` and
    `ReportUnit="TenthsOfAMillimeter" Dpi="254"`.
  - Coordinates use report units. The last value of `Padding="L,R,T,B,Dpi"` is that Dpi.
- **Geometry.** `Margins="L, R, T, B"`, `SizeF="w,h"`, `LocationFloat="x,y"`, `HeightF`; `WidthF` for vertical and
  cross-band controls. Table cells use a relative `Weight`.
- **Colors.** A name (`White`, `DimGray`, `Transparent`) or `"A,R,G,B"` (`255,75,75,75`); in expressions,
  `Rgb(r,g,b)`.
- **Fonts.** `Font="Arial, 9.75pt, style=Bold, Underline"`; chart elements use `DXFont=`.
- **Expressions.** XML-escape `<`, `>` and `&` inside attribute values (`&lt;=`). Parameter-level expression bindings
  have no `EventName`.
- **Root attributes:**
  - `Landscape="true"`, `PaperKind="Custom"`, `RollPaper="true"`;
  - `VerticalContentSplitting="Smart"` / `HorizontalContentSplitting="Smart"`;
  - `PageColor`, `ShowPreviewMarginLines="false"`.
- **Designer metadata you can omit:** `<Extensions>` with `DataSerializationExtension`, `<DesignerOptions ... />`.
- **Older layouts still load on 26.1,** including older assembly versions and a custom root `ControlType`, which renders
  as a plain XtraReport. Write the installed version in new layouts.

## Data sources

### JsonDataSource (for TdxBackendInMemoryJSONConnection / TdxBackendDataSetJSONConnection)

```xml
<JsonDataSource Name="jsonDataSource1" ConnectionName="ContactsData"><Schema><Node Name="root" Selected="true" NodeType="Object"><Node Name="Contacts" Selected="true" NodeType="Array"><Node Name="Id" Type="System.Nullable`1[System.Int64]" Selected="true" NodeType="Property" /><Node Name="FirstName" Type="System.String" Selected="true" NodeType="Property" /><Node Name="Price" Type="System.Nullable`1[System.Double]" Selected="true" NodeType="Property" /><Node Name="Fax" Type="System.String" Selected="false" NodeType="Property" /></Node></Node></Schema></JsonDataSource>
```

- `ConnectionName` is the connection's `DisplayName`.
- Top-level arrays are the report `DataMember`s; for a DataSet connection they are the `DataSetAlias` values.

### SqlDataSource: select query with an expression parameter bound to a report parameter

```xml
<SqlDataSource Name="sqlDataSource1"><Connection Name="NWindConnection" FromAppConfig="true" /><Query Type="SelectQuery" Name="Customers"><Parameter Name="CountrySqlParameter" Type="DevExpress.DataAccess.Expression">(System.String)(?CountryReportParameter)</Parameter><Tables><Table Name="Customers" /></Tables><Columns><Column Table="Customers" Name="CompanyName" /><Column Table="Customers" Name="Country" /></Columns><Filter>[Customers.Country] = ?CountrySqlParameter</Filter></Query><ResultSchema><DataSet Name="sqlDataSource1"><View Name="Customers"><Field Name="CompanyName" Type="String" /><Field Name="Country" Type="String" /></View></DataSet></ResultSchema><ConnectionOptions CloseConnection="true" /></SqlDataSource>
```

### SqlDataSource: stored procedure with an expression parameter

```xml
<SqlDataSource Name="sqlDataSource1"><Connection Name="NorthwindConnection" FromAppConfig="true" /><Query Type="StoredProcQuery" Name="CustOrdersDetail"><Parameter Name="@OrderID" Type="DevExpress.DataAccess.Expression">(System.Int32)(?OrderIDParameter)</Parameter><ProcName>CustOrdersDetail</ProcName></Query><ResultSchema><DataSet Name="sqlDataSource1"><View Name="CustOrdersDetail"><Field Name="ProductName" Type="String" /><Field Name="UnitPrice" Type="Decimal" /><Field Name="Quantity" Type="Int16" /><Field Name="Discount" Type="Int32" /><Field Name="ExtendedPrice" Type="Decimal" /></View></DataSet></ResultSchema><ConnectionOptions CloseConnection="true" /></SqlDataSource>
```

### SqlDataSource: master-detail relation

```xml
<SqlDataSource Name="sqlDataSource1"><Connection Name="NWind" FromAppConfig="true" /><Query Type="SelectQuery" Name="Categories"><Tables><Table Name="Categories" /></Tables><Columns><Column Table="Categories" Name="CategoryID" /><Column Table="Categories" Name="CategoryName" /></Columns></Query><Query Type="SelectQuery" Name="Products"><Tables><Table Name="Products" /></Tables><Columns><Column Table="Products" Name="CategoryID" /><Column Table="Products" Name="ProductName" /></Columns></Query><Relation Master="Categories" Detail="Products" Name="CategoriesProducts"><KeyColumn Master="CategoryID" Detail="CategoryID" /></Relation><ResultSchema><DataSet Name="sqlDataSource1"><View Name="Categories"><Field Name="CategoryID" Type="Int64" /><Field Name="CategoryName" Type="String" /></View><View Name="Products"><Field Name="CategoryID" Type="Int64" /><Field Name="ProductName" Type="String" /></View><Relation Name="CategoriesProducts" Master="Categories" Detail="Products"><KeyColumn Master="CategoryID" Detail="CategoryID" /></Relation></DataSet></ResultSchema><ConnectionOptions CloseConnection="true" /></SqlDataSource>
```

The detail band uses `DataMember="Categories.CategoriesProducts"`.

### FederationDataSource: master-detail over a JSON/FireDAC source

The federation XML (Base64 file):

```xml
<FederationDataSource Name="federationDataSource1"><Sources><Source SourceName="jsonDataSource1_CategoriesTable" DataMember="CategoriesTable" /><Source SourceName="jsonDataSource1_ProductsTable" DataMember="ProductsTable" /></Sources><Queries><Query Alias="CategoriesTable" QueryType="SelectNode"><Root QueryType="SourceNode" SourceName="jsonDataSource1_CategoriesTable" /><Expressions><Expression ExpressionType="SelectColumnExpression" Name="CategoryID" NodeAlias="jsonDataSource1_CategoriesTable" /><Expression ExpressionType="SelectColumnExpression" Name="CategoryName" NodeAlias="jsonDataSource1_CategoriesTable" /></Expressions></Query><Query Alias="ProductsTable" QueryType="SelectNode"><Root QueryType="SourceNode" SourceName="jsonDataSource1_ProductsTable" /><Expressions><Expression ExpressionType="SelectColumnExpression" Name="CategoryID" NodeAlias="jsonDataSource1_ProductsTable" /><Expression ExpressionType="SelectColumnExpression" Name="ProductName" NodeAlias="jsonDataSource1_ProductsTable" /></Expressions></Query></Queries><Relations><Relation MasterQueryName="CategoriesTable" DetailQueryName="ProductsTable"><KeyColumns><KeyColumn Parent="CategoryID" Nested="CategoryID" /></KeyColumns></Relation></Relations></FederationDataSource>
```

`ComponentStorage`: the JSON source plus the federation source with a `SerializableSourceMap`. The report and
detail band point to the **federation** `Ref`:

```xml
<ComponentStorage>
  <Item1 Ref="50" ObjectType="DevExpress.DataAccess.Json.JsonDataSource,DevExpress.DataAccess.v26.1" Name="jsonDataSource1" Base64="@@B64:jsonDataSource1.xml@@" />
  <Item2 Ref="0" ObjectType="DevExpress.DataAccess.DataFederation.FederationDataSource,DevExpress.DataAccess.v26.1" Name="federationDataSource1" Base64="@@B64:federationDataSource1.xml@@">
    <SerializableSourceMap>
      <Item1 Ref="51" DataSource="#Ref-50" Name="jsonDataSource1_CategoriesTable" />
      <Item2 Ref="52" DataSource="#Ref-50" Name="jsonDataSource1_ProductsTable" />
    </SerializableSourceMap>
  </Item2>
</ComponentStorage>
```

- Report: `DataMember="CategoriesTable" DataSource="#Ref-0"`.
- Detail band: `DataMember="CategoriesTable.CategoriesTableProductsTable"`. The relation name is auto-generated as master alias + detail alias.
- Tip: when unsure about a data-source XML format, build it in C# with the installed DevExpress .NET
  `DevExpress.DataAccess` assembly and call `SaveToXml()`.

### SqlDataSource: joins, expression columns, sorting, filter

```xml
<Query Type="SelectQuery" Name="Products">
  <Tables><Table Name="Products" /><Table Name="Categories" />
    <Relation Type="Inner" Parent="Products" Nested="Categories"><KeyColumn Parent="CategoryID" Nested="CategoryID" /></Relation></Tables>
  <Columns><Column Table="Products" Name="ProductName" /><Column Table="Categories" Name="CategoryName" />
    <Expression Alias="StockValue">ToDouble([Products].[UnitPrice] * [Products].[UnitsInStock])</Expression></Columns>
  <Sorting><Column Table="Products" Name="UnitPrice" Descending="true" /></Sorting>
  <Filter>[Categories.CategoryID] &lt;= 2</Filter>
</Query>
```

- **Self-join:**
  `<Table Name="Employees" Alias="e" /><Table Name="Employees" Alias="r" /><Relation Type="LeftOuter" Parent="e" Nested="r">`.
- **Aliased columns:** `<Column Table="Customers" Name="CustomerID" Alias="Customers_CustomerID" />`.

### Relation names → DataMember paths

- **Unnamed relation:** `<Relation Master="Orders" Detail="OrderItems">` gives the path `Orders.OrdersOrderItems`.
- **Named relation:** `Name="Store"` gives `Orders.Store`.
- **Deeper levels:** paths can be three levels deep, e.g. `Customers.CustomersOrders.OrdersOrderItems`.
- **In the `ResultSchema`,** the relation is repeated as `<Relation Name=... Master=... Detail=...>`.

### Query parameters from a date-range report parameter

```xml
<Parameter Name="dateRange_Start" Type="DevExpress.DataAccess.Expression">(System.DateTime)(?dateRange_Start)</Parameter>
<Parameter Name="dateRange_End" Type="DevExpress.DataAccess.Expression">(System.DateTime)(?dateRange_End)</Parameter>
<!-- ... --><Filter>[Orders.OrderDate] Between(?dateRange_Start, ?dateRange_End)</Filter>
```

The alternative syntax `(System.Int32)([Parameters.OrderIdParameter])` also works.

### JsonDataSource with inline JSON (no connection component)

This is the cheapest self-contained data for a repro. A nested array is a detail member:
`DataMember="Customers.Orders"`.

```xml
<JsonDataSource Name="jsonDataSource1"><Source SourceType="DevExpress.DataAccess.Json.CustomJsonSource" Json="{&quot;Customers&quot;:[{&quot;Name&quot;:&quot;Alpha&quot;,&quot;Orders&quot;:[{&quot;No&quot;:&quot;A-1&quot;,&quot;Amount&quot;:10.5}]}]}" /><Schema><Node Name="root" Selected="true" NodeType="Object"><Node Name="Customers" Selected="true" NodeType="Array"><Node Name="Name" Type="System.String" Selected="true" NodeType="Property" /><Node Name="Orders" Selected="true" NodeType="Array"><Node Name="No" Type="System.String" Selected="true" NodeType="Property" /><Node Name="Amount" Type="System.Nullable`1[System.Double]" Selected="true" NodeType="Property" /></Node></Node></Node></Schema></JsonDataSource>
```

## Bands

### Wizard "Table Report" skeleton (title, caption row repeated with the first detail row, striped rows, page info)

```xml
<Item1 Ref="2" ControlType="TopMarginBand" Name="TopMargin" HeightF="100" />
<Item2 Ref="3" ControlType="BottomMarginBand" Name="BottomMargin" HeightF="100"><Controls>
  <Item1 Ref="4" ControlType="XRPageInfo" Name="pageInfo1" PageInfo="DateTime" SizeF="325,23" LocationFloat="0,6" StyleName="PageInfo" />
  <Item2 Ref="5" ControlType="XRPageInfo" Name="pageInfo2" TextFormatString="Page {0} of {1}" TextAlignment="TopRight" SizeF="325,23" LocationFloat="325,6" StyleName="PageInfo" /></Controls></Item2>
<Item3 Ref="6" ControlType="ReportHeaderBand" Name="ReportHeader" HeightF="60"><Controls>
  <Item1 Ref="7" ControlType="XRLabel" Name="label1" Text="Customer List" SizeF="650,24.2" LocationFloat="0,6" StyleName="Title" /></Controls></Item3>
<Item4 Ref="8" ControlType="GroupHeaderBand" Name="GroupHeader1" GroupUnion="WithFirstDetail" HeightF="28"><Controls>
  <Item1 Ref="9" ControlType="XRTable" Name="table1" SizeF="650,28" LocationFloat="0,0"><Rows><Item1 Ref="10" ControlType="XRTableRow" Name="tableRow1" Weight="1"><Cells>
    <Item1 Ref="11" ControlType="XRTableCell" Name="tableCell1" Weight="0.5" Text="Company" StyleName="DetailCaption1" />
    <Item2 Ref="12" ControlType="XRTableCell" Name="tableCell2" Weight="0.5" Text="Phone" StyleName="DetailCaption1" />
  </Cells></Item1></Rows></Item1></Controls></Item4>
<Item5 Ref="13" ControlType="DetailBand" Name="Detail" HeightF="25"><Controls>
  <Item1 Ref="14" ControlType="XRTable" Name="table2" SizeF="650,25" LocationFloat="0,0" OddStyleName="DetailData3_Odd"><Rows><Item1 Ref="15" ControlType="XRTableRow" Name="tableRow2" Weight="1"><Cells>
    <Item1 Ref="16" ControlType="XRTableCell" Name="tableCell3" Weight="0.5" StyleName="DetailData1"><ExpressionBindings><Item1 Ref="17" EventName="BeforePrint" PropertyName="Text" Expression="[CompanyName]" /></ExpressionBindings></Item1>
    <Item2 Ref="18" ControlType="XRTableCell" Name="tableCell4" Weight="0.5" StyleName="DetailData1"><ExpressionBindings><Item1 Ref="19" EventName="BeforePrint" PropertyName="Text" Expression="[Phone]" /></ExpressionBindings></Item2>
  </Cells></Item1></Rows></Item1></Controls></Item5>
```

```xml
<StyleSheet>
  <Item1 Ref="20" Name="Title" BorderStyle="Inset" Font="Arial, 14.25pt" ForeColor="255,75,75,75" BackColor="Transparent" BorderColor="Black" Sides="None" BorderWidthSerializable="1" />
  <Item2 Ref="21" Name="DetailCaption1" BorderStyle="Inset" Padding="6,6,0,0,100" Font="Arial, 8.25pt, style=Bold" ForeColor="White" BackColor="255,75,75,75" BorderColor="White" Sides="Left" TextAlignment="MiddleLeft" BorderWidthSerializable="2" />
  <Item3 Ref="22" Name="DetailData1" BorderStyle="Inset" Padding="6,6,0,0,100" Font="Arial, 8.25pt" ForeColor="Black" BorderColor="Transparent" Sides="Left" TextAlignment="MiddleLeft" BorderWidthSerializable="2" />
  <Item4 Ref="23" Name="DetailData3_Odd" BorderStyle="Inset" Padding="6,6,0,0,100" Font="Arial, 8.25pt" ForeColor="Black" BackColor="255,231,231,231" BorderColor="Transparent" Sides="None" TextAlignment="MiddleLeft" BorderWidthSerializable="1" />
  <Item5 Ref="24" Name="PageInfo" BorderStyle="Inset" Padding="6,6,0,0,100" Font="Arial, 8.25pt, style=Bold" ForeColor="255,75,75,75" />
</StyleSheet>
```

### Page header/footer with colored text

```xml
<Item2 Ref="3" ControlType="PageHeaderBand" Name="PageHeader" HeightF="45"><Controls>
  <Item1 Ref="4" ControlType="XRLabel" Name="lblHeader" Text="Header" TextAlignment="MiddleCenter" SizeF="750,35" LocationFloat="0,0" Font="Segoe UI, 18pt, style=Bold" ForeColor="Blue">
    <StylePriority Ref="5" UseFont="false" UseForeColor="false" UseTextAlignment="false" /></Item1></Controls></Item2>
```

`PageFooterBand` works the same way (`ForeColor="Red"`). Put `XRPageInfo TextFormatString="Page {0} of {1}"` there.

### DetailReportBand (master-detail)

```xml
<Item4 Ref="18" ControlType="DetailReportBand" Name="DetailReport1" Level="0" DataMember="Categories.CategoriesProducts" DataSource="#Ref-0">
  <Bands>
    <Item1 Ref="19" ControlType="DetailBand" Name="Detail2" HeightF="25"><Controls>...cells bound to [ProductName], [UnitPrice]...</Controls></Item1>
  </Bands>
</Item4>
```

### Subreport with a parameter binding (another TdxReport with ReportName = "ContactPhones")

```xml
<Item4 Ref="14" ControlType="XRSubreport" Name="subreportPhones" ReportSourceUrl="ContactPhones" SizeF="700,30" LocationFloat="50,35">
  <ParameterBindings><Item1 Ref="15" ParameterName="ContactId" DataMember="Contacts.Id" /></ParameterBindings>
</Item4>
```

Subreport root: `RequestParameters="false" FilterString="[ContactId] = ?ContactId" Margins="0, 0, 0, 0"`. Its
parameter is `<Item1 Ref="3" Visible="false" ValueInfo="0" Name="ContactId" Type="#Ref-2" />` with
`<ObjectStorage><Item1 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v26.1" Ref="2" Content="System.Int64" Type="System.Type" /></ObjectStorage>`.

### Grand total in a `ReportFooterBand`

```xml
<Item1 Ref="40" ControlType="XRLabel" Name="lblSum" TextFormatString="{0:$0.00}" SizeF="150,25" LocationFloat="500,0">
  <Summary Ref="41" Running="Report" />
  <ExpressionBindings><Item1 Ref="42" EventName="BeforePrint" PropertyName="Text" Expression="sumSum([ExtendedPrice])" /></ExpressionBindings>
</Item1>
```

### Groups, sorting, group summaries

```xml
<Item3 Ref="13" ControlType="GroupHeaderBand" Name="GroupHeader1" GroupUnion="WithFirstDetail" RepeatEveryPage="true" Level="0" HeightF="25">
  <GroupFields><Item1 Ref="14" FieldName="CategoryName" /></GroupFields>
  <SortingSummary Ref="15" Enabled="true" Function="Count" FieldName="UnitPrice" IgnoreNullValues="true" />
  <Controls>...</Controls>
</Item3>
<Item4 Ref="19" ControlType="DetailBand" Name="Detail" HeightF="25">
  <SortFields><Item1 Ref="20" FieldName="UnitPrice" SortOrder="Descending" /></SortFields>
  <Controls>...</Controls>
</Item4>
<Item5 Ref="43" ControlType="GroupFooterBand" Name="GroupFooter1" PrintAtBottom="true" HeightF="25"><Controls>
  <Item1 Ref="46" ControlType="XRLabel" Name="lblAvg" TextFormatString="AVG {0:0.00}" SizeF="150,25" LocationFloat="500,0">
    <Summary Ref="44" Running="Group" />
    <ExpressionBindings><Item1 Ref="45" EventName="BeforePrint" PropertyName="Text" Expression="sumAvg([UnitPrice])" /></ExpressionBindings>
  </Item1></Controls></Item5>
```

- **Nested groups.** `Level="0"` is the innermost group and `Level="1"` the group outside it.
- **Other group options:** `GroupUnion="WholePage"`, `PrintAcrossBands="true"`, `KeepTogether="true"`.
- **Summary functions:** `sumSum`, `sumCount`, `sumAvg`, `sumMin`, `sumMax`, `sumRecordNumber`, `sumRunningSum`,
  `sumCarryoverSum`.
- **A group or report total needs both parts:** a `sum*` function in the expression and a `<Summary Running="Group" />` (or `Report`/`Page`) element on the label. A plain aggregate such as `Count([Id])` without `<Summary>` prints empty in a group footer.
- **`Running` values:** `Group`, `Report`, `Page`. Inside a `DetailReportBand`, `Running="Report"` totals each master record.
- **Page breaks**: `PageBreak="BeforeBand"`, `"AfterBand"` or `"AfterBandExceptLastEntry"` on a band.

### DetailReportBand levels with a footer per master row

```xml
<Item4 Ref="10" ControlType="DetailReportBand" Name="DetailReport2" Level="1" DataMember="Customers.Orders" DataSource="#Ref-0"><Bands>
  <Item1 Ref="11" ControlType="DetailBand" Name="Detail2" HeightF="20">...</Item1>
  <Item2 Ref="14" ControlType="ReportFooterBand" Name="ReportFooter2" HeightF="20"><Controls>
    <Item1 Ref="15" ControlType="XRLabel" Name="lblTotal" SizeF="150,20" LocationFloat="500,0"><Summary Ref="16" Running="Report" />
      <ExpressionBindings><Item1 Ref="17" EventName="BeforePrint" PropertyName="Text" Expression="sumSum([Amount])" /></ExpressionBindings></Item1></Controls></Item2>
</Bands></Item4>
```

Other options: `KeepTogetherWithDetailReports="true"`, and `<ReportPrintOptions DetailCount="50" />`
inside the band.

### SubBand, multi-column and label layouts

- **SubBand**: `<DetailBand ...><SubBands><Item1 Ref="6" ControlType="SubBand" Name="SubBand1" HeightF="153.8"><Controls>...</Controls></Item1></SubBands>...`.
- **Multi-column, fixed column count**: `<MultiColumn Ref="24" ColumnCount="3" Layout="AcrossThenDown" Mode="UseColumnCount" />`.
- **Multi-column by column width**: `<MultiColumn Ref="3" ColumnWidth="400" Layout="AcrossThenDown" Mode="UseColumnWidth" />`.
- **Root-level print options**:
  - `<ReportPrintOptions Ref="26" DetailCountOnEmptyDataSource="10" />`;
  - `<ReportPrintOptions PrintOnEmptyDataSource="false" />` (subreports).
- **Hierarchy and vertical bands** (the demos bind them to an ObjectDataSource, which VCL does not run):
  - `<HierarchyPrintOptions KeyFieldName="ID" ParentFieldName="RegionID" Indent="30" />` inside a `DetailBand`;
  - `VerticalHeaderBand`, `VerticalDetailBand`, `VerticalTotalBand` (with `WidthF`).

Charts, cross-tabs, barcodes, pictures, rich text, calculated fields, conditional appearance, TOC, watermarks,
drill-down/through, interactive sorting, embedded subreports: see [repx-controls-cookbook.md](repx-controls-cookbook.md).

## Parameters

```xml
<Parameters>
  <!-- int, hidden, used by a query or subreport -->
  <Item1 Ref="3" Visible="false" ValueInfo="11000" Name="OrderIDParameter" Description="Order ID" Type="#Ref-2" />
  <!-- string (no Type = System.String) -->
  <Item2 Ref="22" Description="Contact name contains" ValueInfo="" Name="pContactName" />
  <!-- multi-value lookup from data, all selected by default (overrides values set from code!) -->
  <Item3 Ref="24" Description="Company" ValueSourceSettings="#Ref-23" MultiValue="true" SelectAllValues="true" Name="pCompany" />
</Parameters>
...
<ObjectStorage>
  <Item1 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v26.1" Ref="2" Content="System.Int32" Type="System.Type" />
  <Item2 Ref="23" ObjectType="DevExpress.XtraReports.Parameters.DynamicListLookUpSettings, DevExpress.Printing.v26.1.Core" DataSource="#Ref-0" DataMember="Contacts" ValueMember="Company" DisplayMember="Company" SortMember="Company" SortOrder="Ascending" />
</ObjectStorage>
```

Filter examples (report `FilterString`; escape `<` and `>` as `&lt;` and `&gt;` in XML):
- optional text filter: `IsNullOrEmpty(?pContactName) Or Contains(Lower(Concat([FirstName], ' ', [LastName])), Lower(?pContactName))`
- multi-value: `[Company] In (?pCompany)`
- optional multi-value (works with values set from code): parameter `AllowNull="true"`, no `ValueInfo`,
  no `SelectAllValues`; filter `?pCompany Is Null Or [Company] In (?pCompany)`. Leave the parameter unset for all
  rows, or set `VarArrayOf([...])` from code for the listed values

### Date range parameter

```xml
<Item1 Ref="2" Description="Order dates" ValueSourceSettings="#Ref-3" Name="dateRange" Type="#Ref-4" />
<!-- ObjectStorage: -->
<Item1 Ref="3" ObjectType="DevExpress.XtraReports.Parameters.RangeParametersSettings, DevExpress.Printing.v26.1.Core" StartParameter="#Ref-43" EndParameter="#Ref-44" />
<Item2 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v26.1" Ref="4" Content="System.DateTime" Type="System.Type" />
<Item3 Ref="43" ObjectType="DevExpress.XtraReports.Parameters.RangeStartParameter, DevExpress.Printing.v26.1.Core" ValueInfo="2022-01-01" Name="dateRange_Start" />
<Item4 Ref="44" ObjectType="DevExpress.XtraReports.Parameters.RangeEndParameter, DevExpress.Printing.v26.1.Core" ValueInfo="2022-01-15" Name="dateRange_End" />
```

### Multi-value defaults, cascading lookups, a bound `Enabled`, static lists

```xml
<Item2 Ref="5" Description="Countries" ValueSourceSettings="#Ref-6" MultiValue="true" ValueInfo="Germany|France" Name="pCountry" />
<Item3 Ref="7" Description="Cities" ValueSourceSettings="#Ref-8" MultiValue="true" SelectAllValues="true" Name="pCity" />
<Item4 Ref="11" Description="Title" ValueSourceSettings="#Ref-12" ValueInfo="Orders A" Name="pTitle" />
<Item5 Ref="13" Description="Note" ValueInfo="" Name="pNote"><ExpressionBindings><Item1 Ref="14" PropertyName="Enabled" Expression="?pShowFreight" /></ExpressionBindings></Item5>
<!-- ObjectStorage: cascading lookup (cities of the selected countries) -->
<Item5 Ref="8" ObjectType="DevExpress.XtraReports.Parameters.DynamicListLookUpSettings, DevExpress.Printing.v26.1.Core" DataSource="#Ref-0" DataMember="Orders" ValueMember="ShipCity" DisplayMember="ShipCity" FilterString="[ShipCountry] In (?pCountry)" />
<!-- ObjectStorage: static list -->
<Item6 Ref="12" ObjectType="DevExpress.XtraReports.Parameters.StaticListLookUpSettings, DevExpress.Printing.v26.1.Core"><LookUpValues>
  <Item1 Ref="46" Value="#Ref-45" Description="Title A" /><Item2 Ref="48" Value="#Ref-47" Description="Title B" /></LookUpValues></Item6>
<Item7 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v26.1" Ref="45" Content="Orders A" Type="System.String" />
<Item8 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v26.1" Ref="47" Content="Orders B" Type="System.String" />
```

- **Multi-value defaults** are pipe-separated in `ValueInfo` (`A|B`). The demos combine them with
  `SelectAllValues="true"`.
- **A cascaded `SelectAllValues` parameter** is filled at export time, so `[ShipCity] In (?pCity)` keeps its rows.
- **Values from code are converted:** a Boolean set as the string `'True'`, or an Int32 set as `'11077'`, works.

### Parameters panel layout

```xml
<ParameterPanelLayoutItems>
  <Item1 Ref="36" LayoutItemType="Parameter" Parameter="#Ref-2" />
  <Item2 Ref="37" LayoutItemType="Group" Title="Location" Orientation="Horizontal"><Items>
    <Item1 Ref="38" LayoutItemType="Parameter" LabelOrientation="Vertical" Parameter="#Ref-5" /></Items></Item2>
</ParameterPanelLayoutItems>
```

## Localization items

`<LocalizationItems><Item1 Ref="760" Component="#Ref-751" Culture="de" Path="Text" Data="..." />...</LocalizationItems>`
holds translated property values per culture, selected by `TdxReport.Language`. See [localization.md](localization.md):
don't hand-write partial sets.
