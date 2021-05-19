namespace IntensityProfileViewer
{

  // Move this to SkiaUtils ???

  public class SkiaSceneRenderer : SkiaScene.ISKSceneRenderer
  {

    // TODO : Move to 'SkiaTransformHelpers'

    public static void LoadPanAndZoomParameters ( 
      IntensityProfileViewer.PanAndZoomParameters transformParameters,
      SkiaSharp.SKMatrix                      source 
    ) {
      transformParameters.ScaleXY    = source.ScaleX ;
      transformParameters.TranslateX = source.TransX ;
      transformParameters.TranslateY = source.TransY ;
    }
    
    public static SkiaSharp.SKMatrix GetTransformParameters_HorizontalOnly ( 
      IntensityProfileViewer.PanAndZoomParameters transformParameters
    ) {
      var matrix = SkiaSharp.SKMatrix.Identity ;
      matrix.ScaleX = transformParameters.ScaleXY ;
      matrix.TransX = transformParameters.TranslateX ;
      return matrix ;
    }
    
    public static SkiaSharp.SKMatrix GetTransformParameters_VerticalOnly ( 
      IntensityProfileViewer.PanAndZoomParameters transformParameters
    ) {
      var matrix = SkiaSharp.SKMatrix.Identity ;
      matrix.ScaleY = transformParameters.ScaleXY ;
      matrix.TransY = transformParameters.TranslateY ;
      return matrix ;
    }
    
    private readonly System.Action<SkiaSharp.SKCanvas> m_drawOnCanvasAction ;

    public System.Action<SkiaSharp.SKCanvas> RenderHookAction = null ;

    public bool ShowTransformMatrixInfo { get ; set ; } = true ;

    private int m_renderSequenceNumber = 0 ;

    public void Render (
      SkiaSharp.SKCanvas skiaCanvas,
      float              angleInRadians,
      SkiaSharp.SKPoint  centrePoint,
      float              scaleFactor
    ) {
      RenderHookAction?.Invoke(skiaCanvas) ;
      #if DO_RENDER_TIMING_MEASUREMENTS
      System.TimeSpan timeBeforeRenderStarted = m_executionTimingStopwatch.Elapsed ;
      #endif
      skiaCanvas.Clear(
        SkiaColourChoices.ImageBlankAreaColour // SkiaSharp.SKColors.LightGray
      ) ;
      m_drawOnCanvasAction(skiaCanvas) ;
      #if DO_RENDER_TIMING_MEASUREMENTS
      System.TimeSpan timeAfterRenderCompleted = m_executionTimingStopwatch.Elapsed ;
      System.TimeSpan renderTimeElapsed = timeAfterRenderCompleted - timeBeforeRenderStarted ;
      #endif
      if ( ShowTransformMatrixInfo )
      {
        SkiaSharp.SKPaint redPaint = new() { 
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
        //
        // |    [0]     [1]      [2]    |
        // |  ScaleX           TransX   |
        // |                            |
        // |    [3]     [4]      [5]    |
        // |          ScaleY   TransY   |
        // |                            |
        // |    [6]     [7]      [8]    |
        // |     0       0        1     |
        //
        m_renderSequenceNumber++ ;
        Common.DebugHelpers.WriteDebugLines(
          $"Render sequence number {m_renderSequenceNumber}"
        ) ;
        DrawDebugTextLines(
          $"Render sequence number {m_renderSequenceNumber}",
          $"Scale {skiaCanvas.TotalMatrix.ScaleX:F2} {skiaCanvas.TotalMatrix.ScaleY:F2}",
          $"Translate {skiaCanvas.TotalMatrix.TransX:F2} {skiaCanvas.TotalMatrix.TransY:F2}",
          $"Transform matrix : ",
          $"{skiaCanvas.TotalMatrix.Values[0]:F2} {skiaCanvas.TotalMatrix.Values[1]:F2} {skiaCanvas.TotalMatrix.Values[2]:F2} ",
          $"{skiaCanvas.TotalMatrix.Values[3]:F2} {skiaCanvas.TotalMatrix.Values[4]:F2} {skiaCanvas.TotalMatrix.Values[5]:F2} ",
          // Note that these elements are always the same ...
          $"{skiaCanvas.TotalMatrix.Values[6]:F2} {skiaCanvas.TotalMatrix.Values[7]:F2} {skiaCanvas.TotalMatrix.Values[8]:F2} "
          #if DO_RENDER_TIMING_MEASUREMENTS
          ,$"Render time (mS) {renderTimeElapsed.TotalMilliseconds.ToString("F1")}",
          $"Thread #{System.Environment.CurrentManagedThreadId}"
          #endif
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

    #if DO_RENDER_TIMING_MEASUREMENTS
    private readonly System.Diagnostics.Stopwatch m_executionTimingStopwatch = new() ;
    #endif

    public SkiaSceneRenderer ( System.Action<SkiaSharp.SKCanvas> draw )
    {
      m_drawOnCanvasAction = draw ;
      #if DO_RENDER_TIMING_MEASUREMENTS
      m_executionTimingStopwatch.Start() ;
      #endif
    }

  }

}
