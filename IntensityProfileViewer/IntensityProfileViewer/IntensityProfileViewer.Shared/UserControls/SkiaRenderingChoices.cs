//
// SkiaColourChoices.cs
//

using UwpSkiaUtilities.ExtensionMethods ;

namespace IntensityProfileViewer
{

  // TODO - this would be better in 'UserPreferences'

  public static class SkiaColourChoices
  {

    // Colours etc used in the Profile graphs.

    public static SkiaSharp.SKColor ProfileGraphBackgroundColour = (
      // SkiaSharp.SKColors.LightYellow 
      "ProfileGraphBackgroundColour".AsXamlSolidColorBrushToSkiaColor()
    ) ;

    public static SkiaSharp.SKColor ProfileGraphLineColour = (
      // SkiaSharp.SKColors.Red 
      "ProfileGraphLineColour".AsXamlColorToSkiaColor()
    ) ;

    public static SkiaSharp.SKColor ProfileGraphHighlightedLineColour = (
      // SkiaSharp.SKColors.Blue 
      "ProfileGraphHighlightedLineColour".AsXamlColorToSkiaColor()
    ) ;

    public static float ProfileGraphNominalLineWidth      = 1.0f ;
    public static float ProfileGraphHighlightedLineWidth  = 3.0f ;

    // Colours etc used in the Image renderer

    public static SkiaSharp.SKColor ImageBlankAreaColour => (
      ProfileGraphBackgroundColour
      // SkiaSharp.SKColors.LightGray 
    ) ;
 
    public static SkiaSharp.SKColor ImageDragMarkerColour = (
      // SkiaSharp.SKColors.Red 
      "ImageDragMarkerColour".AsXamlColorToSkiaColor()
    ) ;

    public static float ImageDragMarkerDiameter = 4.0f ;

    public static SkiaSharp.SKColor ReferencePositionTextColour = (
      // SkiaSharp.SKColors.White 
      "ReferencePositionTextColour".AsXamlColorToSkiaColor()
    ) ;

    public static SkiaSharp.SKColor ImageReferenceLineColour = (
      // SkiaSharp.SKColors.Red 
      "ImageReferenceLineColour".AsXamlColorToSkiaColor()
    ) ;

    public static float ImageReferenceLineNominalWidth     = 1.0f ;
    public static float ImageReferenceLineHighlightedWidth = 2.0f ;

  }

}
