//
// SkiaDrawing_01_UserControl.cs
//

using System.Linq ;

namespace Views_ForTesting_UsingSkia
{

  public sealed partial class SkiaDrawing_01_UserControl : Windows.UI.Xaml.Controls.UserControl
  {

    private IntensityMapViewer.StaticIntensityMapsDemo_ViewModel ViewModel
    { get ; } = new IntensityMapViewer.StaticIntensityMapsDemo_ViewModel() ;

    public SkiaDrawing_01_UserControl ( )
    {
      this.InitializeComponent() ;

      m_skiaCanvas_00.PaintSurface += (s,e) => {
        e.Surface.Canvas.DrawLine(
          new SkiaSharp.SKPoint(0,0),
          new SkiaSharp.SKPoint(100,100),
          new SkiaSharp.SKPaint(){
            Color = SkiaSharp.SKColors.Red
          }
        ) ;
      } ;

      m_skiaCanvas_01.PaintSurface += DrawIntensityMap ;


    }

    private void DrawIntensityMap ( 
      object                                      sender, 
      SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs paintSurfaceEventArgs
    ) {
      var skiaCanvas = paintSurfaceEventArgs.Surface.Canvas ;
      var intensityMap = ViewModel.IntensityMap ;
      var bitmap = new SkiaSharp.SKBitmap(
        intensityMap.Dimensions.Width,
        intensityMap.Dimensions.Height
      ) ;
      bitmap.Pixels = intensityMap.IntensityValues.Select(
        intensity => new SkiaSharp.SKColor(
          intensity,
          intensity,
          intensity
        )
      ).ToArray() ;
      SkiaSharp.SKRect rectInWhichToDrawBitmap = new SkiaSharp.SKRect(
        20.0f,
        20.0f,
        20.0f + intensityMap.Dimensions.Width  * 1.5f,
        20.0f + intensityMap.Dimensions.Height * 1.5f
      ) ;
      skiaCanvas.DrawBitmap(
        bitmap,
        rectInWhichToDrawBitmap
      ) ;
    }

  }

}
