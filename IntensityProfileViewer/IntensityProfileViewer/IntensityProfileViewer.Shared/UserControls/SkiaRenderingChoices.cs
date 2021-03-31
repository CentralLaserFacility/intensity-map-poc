//
// SkiaColourChoices.cs
//

namespace IntensityProfileViewer
{

  // TODO - this needs to be in 'UserPreferences'
  // but we'd need a way of mapping XAML colours to Skia colour definitions ...
  // not a problem but this is a quicker hack to get the definitions all in one place

  public static class SkiaColourChoices
  {

    // Colours etc used in the Profile graphs

    public static SkiaSharp.SKColor ProfileGraphBackgroundColour      = SkiaSharp.SKColors.LightYellow ;

    public static SkiaSharp.SKColor ProfileGraphLineColour            = SkiaSharp.SKColors.Red ;
    public static SkiaSharp.SKColor ProfileGraphHighlightedLineColour = SkiaSharp.SKColors.Blue ;
    public static float             ProfileGraphNominalLineWidth       = 1.0f ;
    public static float             ProfileGraphHighlightedLineWidth  = 3.0f ;

    // Colours etc used in the Image renderer

    public static SkiaSharp.SKColor ImageDragMarkerColour              = SkiaSharp.SKColors.Blue ;
    public static float             ImageDragMarkerDiameter            = 4.0f ;

    public static SkiaSharp.SKColor ReferencePositionTextColour        = SkiaSharp.SKColors.White ;

    public static SkiaSharp.SKColor ImageReferenceLineColour           = SkiaSharp.SKColors.Red ;
    public static float             ImageReferenceLineNominalWidth     = 1.0f ;
    public static float             ImageReferenceLineHighlightedWidth = 2.0f ;

  }

}
