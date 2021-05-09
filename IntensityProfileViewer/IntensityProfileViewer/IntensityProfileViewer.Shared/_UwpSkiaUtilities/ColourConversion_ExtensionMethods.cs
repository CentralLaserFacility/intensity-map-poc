//
// ColourConversion_ExtensionMethods.cs
//

namespace UwpSkiaUtilities.ExtensionMethods
{

  public static class ColourConversion_ExtensionMethods
  {

    public static SkiaSharp.SKColor ConvertedToSkiaColor ( this Windows.UI.Xaml.Media.SolidColorBrush solidColorBrush )
    => solidColorBrush.Color.ConvertedToSkiaColor() ;

    public static SkiaSharp.SKColor ConvertedToSkiaColor ( this Windows.UI.Color color )
    => new SkiaSharp.SKColor(
      red   : color.R,
      green : color.G,
      blue  : color.B,
      alpha : color.A
    ) ;

    public static SkiaSharp.SKColor AsXamlSolidColorBrushToSkiaColor ( this string xamlSolidColorBrushResourceName )
    => (
      (Windows.UI.Xaml.Media.SolidColorBrush) Windows.UI.Xaml.Application.Current.Resources[xamlSolidColorBrushResourceName]
    ).ConvertedToSkiaColor() ;

    public static SkiaSharp.SKColor AsXamlColorToSkiaColor ( this string xamlColorResourceName )
    => (
      (Windows.UI.Color) Windows.UI.Xaml.Application.Current.Resources[xamlColorResourceName]
    ).ConvertedToSkiaColor() ;

  }

}
