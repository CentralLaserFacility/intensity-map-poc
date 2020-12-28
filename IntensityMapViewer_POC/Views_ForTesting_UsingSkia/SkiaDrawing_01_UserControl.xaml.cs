//
// SkiaDrawing_01_UserControl.cs
//

namespace Views_ForTesting_UsingSkia
{

  public sealed partial class SkiaDrawing_01_UserControl : Windows.UI.Xaml.Controls.UserControl
  {

    public SkiaDrawing_01_UserControl()
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
    }

  }

}
