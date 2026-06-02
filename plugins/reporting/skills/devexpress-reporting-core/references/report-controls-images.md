# Image Controls

## When to Use This Reference

Use when embedding static or data-bound images (BMP, JPG, GIF, TIF, PNG, ICO, EMF, WMF, SVG): `XRPictureBox`.

## XRPictureBox — Static or Data-Bound Images

Static or data-bound image (BMP, JPG, GIF, TIF, PNG, ICO, EMF, WMF, SVG):

```csharp
// Static image
var pic = new XRPictureBox();
detail.Controls.Add(pic);
pic.BoundsF = new RectangleF(0, 0, 80, 60);
pic.ImageSource = ImageSource.FromFile("logo.png");
pic.Sizing = ImageSizeMode.ZoomImage;

// Data-bound image (byte[] field)
var pic2 = new XRPictureBox();
detail.Controls.Add(pic2);
pic2.BoundsF = new RectangleF(0, 0, 60, 60);
pic2.Sizing = ImageSizeMode.ZoomImage;
pic2.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "ImageSource", "[Photo]"));
```

**Main content properties**: 
- `ImageSource` (accepts `ImageSource` objects from file, stream, byte array, or embedded resource)
- `ImageUrl` (string path or URL; takes precedence over `ImageSource`)

**Sizing modes**: `Normal`, `StretchImage`, `AutoSize`, `ZoomImage`, `Squeeze`, `Tile`, `Cover`. Use `ImageAlignment` together with `Sizing = Normal` to center an image within the control.
