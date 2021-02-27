//
// Line.cs
// 

namespace NativeUwp_ViewerApp_01
{

  // A 'Line' can be drawn, and can participate in Hit Testing

  public abstract record Line ( SkiaSharp.SKPoint From, SkiaSharp.SKPoint To )
  {
    public void Draw ( SkiaSharp.SKCanvas canvas, SkiaSharp.SKPaint paint )
    {
      canvas.DrawLine(
        From,
        To,
        paint
      ) ;
    }
    public abstract bool CoincidesWithMousePosition ( SkiaSharp.SKPoint mousePosition, float maxDelta = 4.0f ) ;
  }

  public record HorizontalLine : Line
  {
    public HorizontalLine (
      SkiaSharp.SKPoint pointOnLine,
      float extremeLeftX,
      float extremeRightX
    ) :
    base(
      From : new SkiaSharp.SKPoint(
        extremeLeftX,
        pointOnLine.Y
      ),
      To : new SkiaSharp.SKPoint(
        extremeRightX,
        pointOnLine.Y
      )
    ) {
    }
    public override bool CoincidesWithMousePosition ( SkiaSharp.SKPoint mousePosition, float maxDelta = 4.0f )
    => System.MathF.Abs(
      From.Y - mousePosition.Y
    ) > maxDelta ;
  }

  public record VerticalLine : Line
  {
    public VerticalLine (
      SkiaSharp.SKPoint pointOnLine,
      float extremeTopY,
      float extremeBottomY
    ) :
    base(
      From : new SkiaSharp.SKPoint(
        pointOnLine.X,
        extremeTopY
      ),
      To : new SkiaSharp.SKPoint(
        pointOnLine.X,
        extremeBottomY
      )
    ) {
    }
    public override bool CoincidesWithMousePosition ( SkiaSharp.SKPoint mousePosition, float maxDelta = 4.0f )
    => System.MathF.Abs(
      From.X - mousePosition.X
    ) > maxDelta ;
  }

}
