//
// SkiaSceneRenderer_ForTesting_01.cs
//

namespace WindowsTemplateStudioApp_02.Views
{

  public class SkiaSceneRenderer_ForTesting_01 : SkiaScene.ISKSceneRenderer
  {

    public string DebugInfoToDraw_01 { get ; set ; } = "Debug info line 1" ;
    public string DebugInfoToDraw_02 { get ; set ; } = "Debug info line 2" ;
    public string DebugInfoToDraw_03 { get ; set ; } = "Debug info line 3" ;

    private string[] m_debugInfoLines = new string[0] ;
    public  void SetDebugInfoLines ( params string[] debugInfoLines ) 
    {
      m_debugInfoLines = debugInfoLines ;
    }

    public System.Action<SkiaSharp.SKCanvas> RenderHook = null ;

    public void Render (
      SkiaSharp.SKCanvas skiaCanvas,
      float              angleInRadians,
      SkiaSharp.SKPoint  centrePoint,
      float              scaleFactor
    ) {
      RenderHook?.Invoke(skiaCanvas) ;

      skiaCanvas.Clear(SkiaSharp.SKColors.LightYellow) ;

      SkiaSharp.SKPaint redPaint = new SkiaSharp.SKPaint() { 
        Color       = SkiaSharp.SKColors.Red,
        IsAntialias = true,
        Typeface    = SkiaSharp.SKTypeface.FromFamilyName(
          "Courier",
          SkiaSharp.SKFontStyle.Bold
        )
      } ;

      SkiaSharp.SKPaint bluePaint = new SkiaSharp.SKPaint() { 
        Style       = SkiaSharp.SKPaintStyle.Stroke,
        Color       = SkiaSharp.SKColors.Blue,
        IsAntialias = true,
        StrokeWidth = 5
      } ;

      // Draw a filled circle at the Origin

      skiaCanvas.DrawCircle(
        cx     : 0.0f,
        cy     : 0.0f,
        radius : 10.0f,
        redPaint
      ) ;

      // Define a rectangle into which we'll draw a bitmap

      SkiaSharp.SKRect rectInWhichToDrawBitmap = new SkiaSharp.SKRect(
        20.0f,
        20.0f,
        20.0f + 200.0f,
        20.0f + 200.0f
      ) ;

      // canvas.DrawBitmap(
      //   m_ivryGitlisBitmap,
      //   rectInWhichToDrawBitmap
      // ) ;

      // Just above the bitmap, draw a title, in red

      float lineHeight = 14.0f ;

      skiaCanvas.DrawText(
        "Ivry Gitlis",
        rectInWhichToDrawBitmap.Left,
        rectInWhichToDrawBitmap.Top - lineHeight,
        redPaint
      ) ;

      // Draw a blue box around the rectangle outline

      var rectPath = new SkiaSharp.SKPath() ;
      rectPath.AddRect(rectInWhichToDrawBitmap) ;
      skiaCanvas.DrawPath(
        rectPath,
        bluePaint
      ) ;

      var debugTextOrigin = rectInWhichToDrawBitmap.Location ;

      var solidColourBitmap = new SkiaSharp.SKBitmap(20,20) ;
      var pixels = new SkiaSharp.SKColor[
        solidColourBitmap.Width 
      * solidColourBitmap.Height
      ] ;
      for ( int iPixel = 0 ; iPixel < pixels.Length ; iPixel++ )
      {
        pixels[iPixel] = new SkiaSharp.SKColor(
          0xffff0000
        | (byte) ( iPixel * 4 )
        ) ;
      }
      solidColourBitmap.Pixels = pixels ;
      rectInWhichToDrawBitmap.Offset(
        rectInWhichToDrawBitmap.Width + 20.0f,
        0.0f
      ) ;
      skiaCanvas.DrawBitmap(
        solidColourBitmap,
        rectInWhichToDrawBitmap
      ) ;

      // Draw the 'debug info' lines below the bitmap image

      //
      // For scaling and translation :
      //
      //               |   sx    0     0  |
      // | x  y  1 | × |    0    sy    0  |  =>  | x'  y'  z' |
      //               |   tx    ty    1  |
      //

      debugTextOrigin.Offset(
        0.0f,
        rectInWhichToDrawBitmap.Height + lineHeight
      ) ;


    }

  }

}
