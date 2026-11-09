# REPX Controls Cookbook: controls, appearance, interactivity, subreports

Companion to [repx-dfm-layout-cookbook.md](repx-dfm-layout-cookbook.md): read that file first for the REPX skeleton,
conventions, data sources, bands, and parameters. Fragments are trimmed to the needed attributes. Full layouts are in the DevExpress demos
  (`%PUBLIC%\Documents\DevExpress VCL Demos\MegaDemos\Product Demos\ExpressReports\Layouts\`).

## Calculated fields

```xml
<CalculatedFields>
  <Item1 Ref="2" Name="PriceWithTax" FieldType="Decimal" Expression="[UnitPrice] * 1.2" DataMember="Products" />
  <Item2 Ref="3" Name="ProductSales" Expression="[ProductsOrderDetails].Sum([UnitPrice] * (1 - [Discount]) * [Quantity])" DataMember="Products" />
  <Item3 Ref="4" Name="OrderSum" Expression="[][[OrderID] == [^.OrderID]].Sum([UnitPrice]*[Quantity])" DataMember="Orders.OrdersOrderDetails" />
</CalculatedFields>
```

## Conditional appearance

**Expression bindings on appearance properties.** `?` as the result keeps the property unchanged.

```xml
<Item2 Ref="28" EventName="BeforePrint" PropertyName="ForeColor" Expression="Iif([UnitPrice] &gt; 30, Rgb(200,0,0), Rgb(0,128,0))" />
<Item3 Ref="29" EventName="BeforePrint" PropertyName="BackColor" Expression="Iif([UnitsInStock] = 0, Rgb(255,230,230), ?)" />
<Item4 Ref="25" EventName="BeforePrint" PropertyName="Font.Bold" Expression="[UnitPrice] &gt; 30" />
<Item5 Ref="30" EventName="BeforePrint" PropertyName="Visible" Expression="?pShowFreight" />
```

- **Other bindable properties**:
  - `Font.Name`, `Font.Size`, `Padding.Left`;
  - `PageBreak`, e.g. `Iif(?p,'None','AfterBandExceptLastEntry')`;
  - cross-tab `RowVisible`/`ColumnVisible`;
  - `CheckBoxState`, `ImageSource`, `Bookmark`;
  - `XRPdfContent` `Source` and `PageRange`.
- **Row context in expressions:**
  - `[DataSource.CurrentRowIndex]`, `[DataSource.RowCount]`, `[DataSource.CurrentRowHierarchyLevel]`;
  - in a cross-tab, `[Arguments.GroupRowIndex]`.
- **Legacy formatting rules**:
  `<FormattingRuleSheet><Item1 Ref="2" Name="formattingRule1" DataMember="Orders" Condition="[Extended Price] &gt; 1000"><Formatting Ref="3" Font="Tahoma, 9.75pt, style=Bold" /></Item1></FormattingRuleSheet>`
  plus, on the control, `<FormattingRuleLinks><Item1 Ref="34" Value="#Ref-2" /></FormattingRuleLinks>`.
- **Markup text**: `<ItemN ControlType="XRLabel" AllowMarkupText="true" Text="&lt;b&gt;bold&lt;/b&gt; &lt;color=255,0,0&gt;red&lt;/color&gt;" ... />`.

## Charts — XRChart

Without `LabelsVisibility="False"` or a `<Label TextPattern="{V:n2}"/>`, the labels show raw values
(`37.9791666666667`).

```xml
<Item1 Ref="47" ControlType="XRChart" Name="chart1" SizeF="650,230" LocationFloat="0,0" DataSource="#Ref-0">
  <Chart Ref="48">
    <DataContainer Ref="49" DataMember="Products" ValidateDataMembers="true">
      <SeriesSerializable>
        <Item1 Ref="50" Name="AvgPrice" SeriesID="0" ArgumentDataMember="CategoryName" ValueDataMembersSerializable="UnitPrice">
          <View Ref="51" TypeNameSerializable="SideBySideBarSeriesView" />
          <QualitativeSummaryOptions Ref="52" SummaryFunction="AVERAGE([UnitPrice])" />
        </Item1>
      </SeriesSerializable>
    </DataContainer>
    <Diagram Ref="53" TypeNameSerializable="XYDiagram"><AxisX Ref="54" VisibleInPanesSerializable="-1" /><AxisY Ref="55" VisibleInPanesSerializable="-1" /></Diagram>
    <Legend Ref="56" Visibility="False" Name="Default Legend" />
    <Titles><Item1 Ref="57" Text="Avg price" /></Titles>
  </Chart>
</Item1>
```

- **Series template** (one series per value):
  `<SeriesTemplate SeriesDataMember="Orders.Year" ArgumentDataMember="Orders.Month" ValueDataMembersSerializable="Orders.Total"><NumericSummaryOptions SummaryFunction="SUM([Orders.Total])" /></SeriesTemplate>`.
- **Other views:** `DoughnutSeriesView`, `StackedBarSeriesView`.
- **Chart parameters:** `<Parameters><Item1 Name="CurrentYear" Parameter="#Ref-3" /></Parameters>`.
- **Fonts:** chart elements use `DXFont="..."` instead of `Font`.

## Cross-tab — XRCrossTab

The smallest grid has 1 row field, 1 column field and 1 data field, giving 3×3 cells; the data field sums by
default. Prefer `XRCrossTab`; `XRPivotGrid` is the legacy control.

```xml
<Item2 Ref="58" ControlType="XRCrossTab" Name="crossTab1" DataSource="#Ref-0" DataMember="Products" SizeF="450,75" LocationFloat="0,250">
  <RowFields><Item1 Ref="59" FieldName="CategoryName" /></RowFields>
  <ColumnFields><Item1 Ref="60" FieldName="Discontinued" /></ColumnFields>
  <DataFields><Item1 Ref="61" FieldName="UnitsInStock" /></DataFields>
  <ColumnDefinitions><Item1 Ref="62" Width="150" /><Item2 Ref="63" Width="150" /><Item3 Ref="64" Width="150" /></ColumnDefinitions>
  <RowDefinitions><Item1 Ref="65" Height="25" /><Item2 Ref="66" Height="25" /><Item3 Ref="67" Height="25" /></RowDefinitions>
  <Cells>
    <!-- (ColumnIndex,RowIndex): (0,0) row-field header, (1,0) column values, (2,0) column-total header,
         (0,1) row values, (1,1) data, (2,1) row total, (0,2) grand-total header, (1,2) column totals, (2,2) grand total -->
    <Item1 Ref="68" ControlType="XRCrossTabCell" Name="c00" ColumnIndex="0" RowIndex="0" Text="Category" />
    <Item2 Ref="69" ControlType="XRCrossTabCell" Name="c10" ColumnIndex="1" RowIndex="0" TextFormatString="Disc={0}" />
    <!-- one XRCrossTabCell for every remaining (ColumnIndex,RowIndex) pair -->
  </Cells>
</Item2>
```

Options:
- **Fields:** `GroupInterval="DateQuarter"`; `RowSpan`/`ColumnSpan` for multi-field headers; `FilterString` plus
  `<Parameters><Item1 Name="x" Parameter="#Ref-n" /></Parameters>`.
- **Styles:** `GeneralStyleName`, `HeaderAreaStyleName`, `DataAreaStyleName`, `TotalAreaStyleName`.
- **Printing:** `<PrintOptions RepeatColumnHeaders="false" PrintTotalsForSingleValues="false" />`.

## Barcodes, check boxes, pictures, rich text

**XRBarCode.**

```xml
<Item3 Ref="36" ControlType="XRBarCode" Name="bc1" AutoModule="true" ShowText="false" SizeF="100,28" LocationFloat="640,1">
  <Symbology Ref="37" Name="Code128" />
  <ExpressionBindings><Item1 Ref="38" EventName="BeforePrint" PropertyName="Text" Expression="ToStr([ProductID])" /></ExpressionBindings>
</Item3>
<!-- QR code: --> <Symbology Ref="176" Name="QRCode" CompactionMode="Byte" ErrorCorrectionLevel="M" Version="AutoVersion" />
```

Other symbology names: `EAN13`, `QRCodeEPC`, `DataMatrix`, `PDF417`, `AztecCode`. For a fixed module size, use
`Module="3"` instead of `AutoModule`.

**XRCheckBox.**

```xml
<Item2 Ref="34" ControlType="XRCheckBox" Name="chkDisc" Text="disc." SizeF="60,30" LocationFloat="575,0">
  <ExpressionBindings><Item1 Ref="35" EventName="BeforePrint" PropertyName="CheckBoxState" Expression="[Discontinued]" /></ExpressionBindings>
</Item2>
<!-- editable (radio-like group) and custom glyphs: -->
<EditOptions Ref="23" GroupID="1" Enabled="true" />
<GlyphOptions Ref="24" Alignment="Center" Size="18,18"><CustomGlyphs Ref="26" Checked="svg,BASE64" Unchecked="svg,BASE64" /></GlyphOptions>
```

A static check box uses `CheckBoxState="Checked"`.

**XRPictureBox.** Inline images, resource images, or a binary field.

```xml
<Item2 Ref="6" ControlType="XRPictureBox" Name="picInline" ImageSource="svg,BASE64_OF_SVG" Sizing="ZoomImage" SizeF="40,40" LocationFloat="420,0" />
<Item3 Ref="7" ControlType="XRPictureBox" Name="picRes" Sizing="ZoomImage" SizeF="40,40" LocationFloat="470,0">
  <ExpressionBindings><Item1 Ref="8" EventName="BeforePrint" PropertyName="ImageSource" Expression="[Images.blueBox]" /></ExpressionBindings>
</Item3>
<!-- root level, after ComponentStorage: -->
<ImageResources><Item1 Ref="81" Id="blueBox" ImageSource="svg,BASE64_OF_SVG" /></ImageResources>
```

- **Image sources:**
  - Raster images: `ImageSource="img,BASE64"` (PNG/JPEG).
  - A `ByteArray` field: `Expression="[Photo]"`.
  - A BOM before the SVG is optional; demo SVGs start with `77u/`.
- **`Sizing`:** `AutoSize`, `Squeeze`, `StretchImage`, `ZoomImage`.

**XRRichText.** `[Field]` placeholders inside the RTF are merged for each row.

```xml
<Item2 Ref="18" ControlType="XRRichText" Name="rtfNote" SerializableRtfString="BASE64_OF_UTF16LE_RTF" SizeF="400,25" LocationFloat="10,27" />
```

`SerializableRtfString` is Base64 of **UTF-16LE** RTF, not UTF-8; it starts with `ewBcAHIA`. In PowerShell:
`[Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($rtf))`.

## Document structure

**Table of contents and bookmarks.**

```xml
<Item5 Ref="10" ControlType="XRTableOfContents" Name="toc1" MaxNestingLevel="1" LocationFloat="0,70">
  <LevelTitle Ref="11" Text="Contents" Height="30" Font="Arial, 12pt, style=Bold" /><LevelDefault Ref="12" Font="Arial, 9pt" />
</Item5>
<!-- on the labels that create TOC entries: -->
<Item2 Ref="17" EventName="BeforePrint" PropertyName="Bookmark" Expression="[CategoryName]" />
```

Nested TOC levels use `BookmarkParent="#Ref-n"`.

**Cross-band controls.** Defined at root level, after `<Bands>`.

```xml
<CrossBandControls>
  <Item1 Ref="79" ControlType="XRCrossBandBox" Name="cbBox1" StartBand="#Ref-13" EndBand="#Ref-39" StartPointFloat="0,0" EndPointFloat="0,30" WidthF="750" BorderWidth="2" BorderColor="255,0,0,255" />
  <Item2 Ref="31" ControlType="XRCrossBandLine" Name="cbLine1" WidthF="1" StartBand="#Ref-10" EndBand="#Ref-20" StartPointFloat="527,0" EndPointFloat="527,45.9" ForeColor="255,175,190,216" />
</CrossBandControls>
```

**Watermarks.** Drawn in PDF exports; `ExportToImage` does not draw them. Without a root `WatermarkId`, the single
watermark is applied to every page.

```xml
<Watermarks><Item1 Ref="80" Id="Watermark1" Text="DRAFT" TextTransparency="150" Font="Verdana, 54pt" TextPosition="InFront" /></Watermarks>
```

**PDF content and signatures.** Actually signing the PDF needs code that VCL does not run.

```xml
<Item6 Ref="15" ControlType="XRPdfContent" Name="pdf1" GenerateOwnPages="false" SourceSerializable="BASE64_PDF" SizeF="850,1095" LocationFloat="0,0">
  <ExpressionBindings><Item1 Ref="16" EventName="BeforePrint" PropertyName="PageRange" Expression="[DataSource.CurrentRowIndex] + 1" /></ExpressionBindings>
</Item6>
<!-- a PDF per row, from a binary field: --> <Item1 Ref="17" EventName="BeforePrint" PropertyName="Source" Expression="[OrderItemsProducts].[PDF]" />
<Item1 Ref="109" ControlType="XRPdfSignature" Name="sig1" SizeF="391,96" LocationFloat="24,44"><SignatureOptions Ref="6" DisplayDocumentSignature="false" /></Item1>
```

**Shapes, character combs, editable fields.**

```xml
<Item1 Ref="6" ControlType="XRShape" Name="shape1" LineWidth="3" Angle="270" SizeF="62,62" LocationFloat="50,16" ForeColor="255,0,97,133"><Shape Ref="7" ShapeName="Line" /></Item1>
<Item6 Ref="15" ControlType="XRCharacterComb" Name="ccDay" CellWidth="21" CellHeight="21" CellHorizontalSpacing="2" CellSizeMode="Custom" SizeF="44,21" LocationFloat="126,124"><EditOptions Ref="16" EditorName="Day" Enabled="true" /></Item6>
<!-- picture box used as a signature pad: --> <EditOptions Ref="66" EditorName="Signature" Enabled="true" />
<!-- root: export editable fields as PDF AcroForms --> <ExportOptions Ref="83"><Pdf Ref="84" ExportEditingFieldsToAcroForms="true" /></ExportOptions>
```

`XRSparkline` needs data assigned from .NET code, so on VCL it stays empty.

## Interactivity (rendered by exports; the interaction itself happens in the viewer)

**Drill-down.** `DrillDownExpanded="false"`: exports respect it, and without it the detail is exported
expanded.

```xml
<Item3 Ref="6" ControlType="DetailReportBand" Name="DetailReport1" Level="0" DataMember="Customers.Orders" DataSource="#Ref-0" DrillDownControl="#Ref-4" DrillDownExpanded="false">...</Item3>
<!-- toggle icon on the drill-down control: -->
<Item1 Ref="5" EventName="BeforePrint" PropertyName="ImageSource" Expression="IIf([ReportItems.DetailReport1].[DrillDownExpanded], [Images.expand], [Images.collapse])" />
```

**Drill-through to an embedded report.**

```xml
<Item7 Ref="116" ControlType="XRTableCell" Name="cellDetails" Text="Details" Weight="1">
  <Action Ref="117" Name="NavigateToReport">
    <ParameterBindings><Item1 Ref="118" ParameterName="orderId" DataMember="Customers.CustomersOrders.Id" /></ParameterBindings>
    <ReportSource Ref="121" ControlType="DevExpress.XtraReports.UI.XtraReport, DevExpress.XtraReports.v26.1, Version=26.1.0.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Name="Detail" FilterString="[OrderId] = ?orderId" DataMember="Customers" DataSource="#Ref-120">
      <Parameters>...</Parameters><Bands>...</Bands>
      <ComponentStorage><Item1 Ref="120" ObjectType="DevExpress.DataAccess.Sql.SqlDataSource,DevExpress.DataAccess.v26.1" Name="sqlDataSource1" Base64="..." /></ComponentStorage>
    </ReportSource>
  </Action>
</Item7>
```

**Interactive sorting.** `TargetBand` is the `DetailBand`, or a `GroupHeaderBand` to sort the groups.

```xml
<Item1 Ref="10" ControlType="XRTableCell" Name="hdrName" Text="Product Name" Weight="1"><InteractiveSorting Ref="12" TargetBand="#Ref-11" FieldName="ProductName" /></Item1>
```

## Subreports

**Two ways to reference the subreport layout:**
- **A separate `TdxReport`** on the same form: `ReportSourceUrl="Sub"` resolves to the report with `ReportName = 'Sub'`
  (see the subreport section in [repx-dfm-layout-cookbook.md](repx-dfm-layout-cookbook.md)).
- **An embedded `ReportSource`**: no extra `TdxReport` is needed. It has its own `ComponentStorage` and
  `Ref`s.

  ```xml
  <Item1 Ref="19" ControlType="XRSubreport" Name="subEmbedded" SizeF="700,50" LocationFloat="0,5">
    <ReportSource Ref="20" ControlType="DevExpress.XtraReports.UI.XtraReport, DevExpress.XtraReports.v26.1, Version=26.1.0.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Name="TagsSub" Margins="0, 0, 0, 0" PageWidthF="850" PageHeightF="1100" Version="26.1" DataMember="Tags" DataSource="#Ref-21">
      <Bands>...</Bands>
      <ComponentStorage><Item1 Ref="21" ObjectType="DevExpress.DataAccess.Json.JsonDataSource,DevExpress.DataAccess.v26.1" Name="jsonDataSource1" Base64="..." /></ComponentStorage>
    </ReportSource>
  </Item1>
  ```

**Report merging.** `GenerateOwnPages="true"` on the `XRSubreport` keeps the subreport's own page
settings, for example landscape chart pages inside a portrait report.

**Side-by-side subreports and parameter values.** One `ReportSourceUrl` can be used several times with
different bindings.

```xml
<Item1 Ref="40" ControlType="XRSubreport" Name="left"  ReportSourceUrl="SubReport" SizeF="318,375" LocationFloat="0,0"><ParameterBindings><Item1 Ref="41" ParameterName="paramEmployeeID" Parameter="#Ref-3" /></ParameterBindings></Item1>
<Item2 Ref="42" ControlType="XRSubreport" Name="right" ReportSourceUrl="SubReport" SizeF="318,375" LocationFloat="332,0"><ParameterBindings><Item1 Ref="43" ParameterName="paramEmployeeID" Parameter="#Ref-5" /></ParameterBindings></Item2>
<!-- pass a date range: bind the _Start/_End objects from ObjectStorage -->
<Item2 Ref="31" ParameterName="dateRange_Start" Parameter="#Ref-29" /><Item3 Ref="33" ParameterName="dateRange_End" Parameter="#Ref-32" />
```

Subreport filter: `[CustomerID] = ?customerIDParameter And [PurchaseDate] Between(?dateRange_Start, ?dateRange_End)`.

## Not available on VCL

- **`ObjectDataSource`** loads silently but provides no data: `[Field]` placeholders print as text. Use JSON, a
  dataset connection, or SQL instead.
- **C# scripts (`EventsInfo="ctl,BeforePrint,handler"`)** are ignored silently.
- **Behaviour that needs .NET code:** sparkline data, PDF signing, owner-drawn custom controls, and property changes
  done in .NET event handlers.
