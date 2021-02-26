﻿namespace NativeUwp_ViewerApp_01
{

  // Move this to SkiaUtils ???

  public class SkiaSceneRenderer : SkiaScene.ISKSceneRenderer
  {

    private System.Action<SkiaSharp.SKCanvas> m_draw ;

    public System.Action<SkiaSharp.SKCanvas> RenderHook = null ;

    public bool ShowTransformMatrixInfo { get ; set ; }

    public void Render (
      SkiaSharp.SKCanvas skiaCanvas,
      float              angleInRadians,
      SkiaSharp.SKPoint  centrePoint,
      float              scaleFactor
    ) {
      RenderHook?.Invoke(skiaCanvas) ;
      skiaCanvas.Clear(SkiaSharp.SKColors.LightYellow) ;
      m_draw(skiaCanvas) ;
      if ( ShowTransformMatrixInfo )
      {
        SkiaSharp.SKPaint redPaint = new SkiaSharp.SKPaint() { 
          Color       = SkiaSharp.SKColors.Red,
          IsAntialias = true,
          Typeface    = SkiaSharp.SKTypeface.FromFamilyName(
            "Courier",
            SkiaSharp.SKFontStyle.Bold
          )
        } ;
        // Draw a filled circle at the Origin
        skiaCanvas.DrawCircle(
          cx     : 0.0f,
          cy     : 0.0f,
          radius : 10.0f,
          redPaint
        ) ;
        DrawDebugTextLines(
          $"Scale {skiaCanvas.TotalMatrix.ScaleX:F2} {skiaCanvas.TotalMatrix.ScaleY:F2}",
          $"Translate {skiaCanvas.TotalMatrix.TransX:F2} {skiaCanvas.TotalMatrix.TransY:F2}",
          $"Transform matrix : ",
          $"{skiaCanvas.TotalMatrix.Values[0]:F2} {skiaCanvas.TotalMatrix.Values[1]:F2} {skiaCanvas.TotalMatrix.Values[2]:F2} ",
          $"{skiaCanvas.TotalMatrix.Values[3]:F2} {skiaCanvas.TotalMatrix.Values[4]:F2} {skiaCanvas.TotalMatrix.Values[5]:F2} ",
          // Note that these elements are always the same ...
          $"{skiaCanvas.TotalMatrix.Values[6]:F2} {skiaCanvas.TotalMatrix.Values[7]:F2} {skiaCanvas.TotalMatrix.Values[8]:F2} "
        ) ;
        void DrawDebugTextLines ( params string[] textLines )
        {
          SkiaSharp.SKPoint debugTextOrigin = new(10.0f,10.0f) ;
          float lineHeight = 14.0f ;
          foreach ( var textLine in textLines )
          {
            debugTextOrigin.Offset(0.0f,lineHeight) ;
            skiaCanvas.DrawText(
              textLine,
              debugTextOrigin,
              redPaint
            ) ;
          }
        }
      }
    }

    public SkiaSceneRenderer ( System.Action<SkiaSharp.SKCanvas> draw )
    {
      m_draw = draw ;
    }

  }

}