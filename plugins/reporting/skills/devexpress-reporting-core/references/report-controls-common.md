# Common Control Properties and Expression Binding

## Common Control Properties

All controls share a standard set of layout and style properties:

| Property | Type | Description |
|----------|------|-------------|
| `BoundsF` | `RectangleF` | Position and size (x, y, width, height in report units) |
| `LocationF` | `PointF` | Position only in report units |
| `SizeF` | `SizeF` | Size only in report units |
| `Visible` | `bool` | Show/hide control |
| `Font` | `DXFont` | Text font |
| `ForeColor` | `Color` | Text color |
| `BackColor` | `Color` | Background (Transparent by default) |
| `Borders` | `BorderSide` | Which borders to draw |
| `BorderWidth` | `float` | Border width in pixels |
| `BorderColor` | `Color` | Border color |
| `Padding` | `PaddingInfo` | Inner padding |
| `TextAlignment` | `TextAlignment` | Text alignment within the control |
| `CanGrow` | `bool` | Expand height for long content |
| `CanShrink` | `bool` | Shrink height when content is short |

## Expression Binding

Bind any control property to a data field or expression:

```csharp
control.ExpressionBindings.Add(new ExpressionBinding(eventName, propertyName, expression));
// eventName    : "BeforePrint" or "PrintOnPage"
// propertyName : "Text", "Visible", "BackColor", "Image", etc.
// expression   : "[FieldName]", "[Price] * [Qty]", "Iif([Status]='Active', True, False)", etc.
```

See [expressions.md](expressions.md) for detailed information on report expressions.
