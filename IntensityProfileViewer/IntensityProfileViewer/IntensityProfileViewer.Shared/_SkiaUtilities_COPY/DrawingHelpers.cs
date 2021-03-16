//
// DrawingHelpers.cs
//

using SkiaSharp;

namespace SkiaUtilities
{

  public static class DrawingHelpers
  {

    public static void DrawFilledRect (
      this SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKPoint       topLeftPoint,
      SkiaSharp.SKSize        widthAndHeight,
      SkiaSharp.SKPaint       skPaint
    ) {
      skiaCanvas.DrawRect(
        SkiaSharp.SKRect.Create(
          topLeftPoint,
          widthAndHeight
        ),
        skPaint
      ) ;
    }

    public static void DrawFilledRect (
      this SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKRect        rect,
      SkiaSharp.SKPaint       skPaint
    ) {
      skiaCanvas.DrawRect(
        rect,
        skPaint
      ) ;
    }

    public static void DrawLineRightAndDown (
      this SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKPoint       startPoint,
      float                   deltaRight,
      float                   deltaDown,
      SkiaSharp.SKPaint       skPaint
    ) {
      skiaCanvas.DrawLine(
        startPoint,
        startPoint.MovedBy(deltaRight,deltaDown),
        skPaint
      ) ;
    }

    public static void DrawHorizontalLineRight (
      this SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKPoint       startPoint,
      float                   deltaRight,
      SkiaSharp.SKPaint       skPaint
    ) {
      skiaCanvas.DrawLine(
        startPoint,
        startPoint.MovedBy(deltaRight,0),
        skPaint
      ) ;
    }

    public static void DrawHorizontalLineLeft (
      this SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKPoint       startPoint,
      float                   deltaLeft,
      SkiaSharp.SKPaint       skPaint
    ) {
      skiaCanvas.DrawLine(
        startPoint,
        startPoint.MovedBy(-deltaLeft,0),
        skPaint
      ) ;
    }

    public static void DrawVerticalLineDown (
      this SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKPoint       startPoint,
      float                   deltaDown,
      SkiaSharp.SKPaint       skPaint
    ) {
      skiaCanvas.DrawLine(
        startPoint,
        startPoint.MovedBy(0,deltaDown),
        skPaint
      ) ;
    }

    public static void DrawVerticalLineUp (
      this SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKPoint       startPoint,
      float                   deltaUp,
      SkiaSharp.SKPaint       skPaint
    ) {
      skiaCanvas.DrawLine(
        startPoint,
        startPoint.MovedBy(0,-deltaUp),
        skPaint
      ) ;
    }

    public static void DrawRectOutline (
      this SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKRect        rect,
      SkiaSharp.SKPaint       skPaint
    ) {
      skiaCanvas.DrawRectOutline(
        rect.Location,
        rect.Size,
        skPaint
      ) ;
    }

    public static void UnpackVisibleCornerPoints (
      this SkiaSharp.SKRect rect,
      out SKPoint           topLeftPoint,    
      out SKPoint           topRightPoint,   
      out SKPoint           bottomLeftPoint, 
      out SKPoint           bottomRightPoint
    ) {
      topLeftPoint     = rect.Location ;
      topRightPoint    = topLeftPoint.MovedBy(rect.Width-1,0) ;
      bottomLeftPoint  = topLeftPoint.MovedBy(0,rect.Height-1) ;
      bottomRightPoint = bottomLeftPoint.MovedBy(rect.Width-1,0) ;
    }

    public static void DrawRectOutline (
      this SkiaSharp.SKCanvas skiaCanvas,
      SkiaSharp.SKPoint       topLeftPoint,
      SkiaSharp.SKSize        widthAndHeight,
      SkiaSharp.SKPaint       skPaint
    ) {
      var topRightPoint    = topLeftPoint.MovedBy(widthAndHeight.Width-1,0) ;
      var bottomLeftPoint  = topLeftPoint.MovedBy(0,widthAndHeight.Height-1) ;
      var bottomRightPoint = bottomLeftPoint.MovedBy(widthAndHeight.Width-1,0) ;
      skiaCanvas.DrawPoints(
        SkiaSharp.SKPointMode.Polygon,
        new[]{
          topLeftPoint,
          topRightPoint,
          bottomRightPoint,
          bottomLeftPoint,
          topLeftPoint
        },
        skPaint
      ) ;
      // // Horizontal at the top
      // skiaCanvas.DrawLine(
      //   topLeftPoint,
      //   topRightPoint,
      //   skPaint
      // ) ;
      // // Horizontal at the bottom
      // skiaCanvas.DrawLine(
      //   bottomLeftPoint,
      //   bottomRightPoint,
      //   skPaint
      // ) ;
      // // Vertical at the left
      // skiaCanvas.DrawLine(
      //   topLeftPoint,
      //   bottomLeftPoint,
      //   skPaint
      // ) ;
      // // Vertical at the right
      // skiaCanvas.DrawLine(
      //   topRightPoint,
      //   bottomRightPoint,
      //   skPaint
      // ) ;
    }

    public static SkiaSharp.SKPoint ConstrainedToBeInside (
      this SkiaSharp.SKPoint point,
      SkiaSharp.SKRect       rectangle
    ) => new SkiaSharp.SKPoint(
      point.X.ClampedToRange(rectangle.Left,rectangle.Right),
      point.Y.ClampedToRange(rectangle.Top,rectangle.Bottom)
    ) ;

    public static float ClampedToRange (
      this float position,
      float      min,
      float      max
    ) => (
      position < min 
      ? min
      : position > max
        ? max
        : position
    ) ;

    public static SkiaSharp.SKPoint MovedBy (
      this SkiaSharp.SKPoint point,
      float                  deltaX,
      float                  deltaY
    ) => new SkiaSharp.SKPoint(
      point.X + deltaX,
      point.Y + deltaY
    ) ;

    public static SkiaSharp.SKRect ExpandedAllRoundBy (
      this SkiaSharp.SKRect rect,
      float                 delta
    ) => SkiaSharp.SKRect.Inflate(
      rect,
      delta,
      delta
    ) ;

    public static SkiaSharp.SKPoint GetPointAtFractionalPositionAlongLine(
      SkiaSharp.SKPoint startPoint,
      SkiaSharp.SKPoint endPoint,
      float             frac01
    ) => new SKPoint(
      startPoint.X + frac01 * ( endPoint.X - startPoint.X ),
      startPoint.Y + frac01 * ( endPoint.Y - startPoint.Y )
    ) ;

    // public static void DrawBoundingBox ( 
    //   SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs paintSurfaceEventArgs
    // ) {
    //   SkiaSharp.SKImageInfo imageInfo = paintSurfaceEventArgs.Info ;
    //   Common.DebugHelpers.WriteDebugLines(
    //     $"SKImageInfo : size [{imageInfo.Width}x{imageInfo.Height}]"
    //   ) ;
    //   SkiaSharp.SKCanvas skiaCanvas = paintSurfaceEventArgs.Surface.Canvas ;
    //   SkiaSharp.SKRectI deviceClipBounds = skiaCanvas.DeviceClipBounds ;
    //   SkiaSharp.SKRect localClipBounds = skiaCanvas.LocalClipBounds ;
    //   Common.DebugHelpers.WriteDebugLines(
    //     $"Skia.Canvas.DeviceClipBounds : [{deviceClipBounds.Left},{deviceClipBounds.Top}] size [{deviceClipBounds.Width}x{deviceClipBounds.Height}]"
    //   ) ;
    //   Common.DebugHelpers.WriteDebugLines(
    //     $"Skia.Canvas.LocalClipBounds : [{localClipBounds.Left},{localClipBounds.Top}] size [{localClipBounds.Width}x{localClipBounds.Height}]"
    //   ) ;
    //   DrawBoundingBox(
    //     skiaCanvas//,
    //     // SkiaSharp.SKRect.Create(
    //     //   new SkiaSharp.SKSize(
    //     //     imageInfo.Width,
    //     //     imageInfo.Height
    //     //   )
    //     // )
    //   ) ;
    // }

    public static void DrawBoundingBox ( 
      SkiaSharp.SKCanvas skiaCanvas
    ) {
      SkiaSharp.SKRect canvasRect =  SkiaSharp.SKRect.Create(
        new SkiaSharp.SKSize(
          skiaCanvas.DeviceClipBounds.Width,
          skiaCanvas.DeviceClipBounds.Height
        )
      ) ;
      // Draw a blue rectangle that covers the entire canvas.
      // One pixel bigger all round just to make sure ??
      skiaCanvas.DrawFilledRect(
        canvasRect.ExpandedAllRoundBy(1),
        new SkiaSharp.SKPaint(){
          Color = SkiaSharp.SKColors.BlanchedAlmond
        }
      ) ;
      // Draw a rectangle at the extreme edges
      skiaCanvas.DrawRectOutline(
        canvasRect,
        new SkiaSharp.SKPaint(){
          Color = SkiaSharp.SKColors.Black
        }
      ) ;
      skiaCanvas.DrawRectOutline(
        canvasRect.ExpandedAllRoundBy(-2),
        new SkiaSharp.SKPaint(){
          Color = SkiaSharp.SKColors.Black
        }
      ) ;
      skiaCanvas.DrawFilledRect(
        canvasRect.ExpandedAllRoundBy(-6),
        new SkiaSharp.SKPaint(){
          Color = SkiaSharp.SKColors.LightGreen
        }
      ) ;
      // Draw markers at the corner points
      var red = new SkiaSharp.SKPaint(){
        Color = SkiaSharp.SKColors.Red
      } ;
      var delta = 20.0f ;
      canvasRect.UnpackVisibleCornerPoints(
        out SKPoint      topLeftPoint,    
        out SKPoint      topRightPoint,   
        out SKPoint      bottomLeftPoint, 
        out SKPoint      bottomRightPoint
      ) ;
      // Lines from top left
      skiaCanvas.DrawHorizontalLineRight(topLeftPoint,delta,red) ;
      skiaCanvas.DrawVerticalLineDown(topLeftPoint,delta,red) ;
      // Lines from top right
      skiaCanvas.DrawHorizontalLineLeft(topRightPoint,delta,red) ;
      skiaCanvas.DrawVerticalLineDown(topRightPoint,delta,red) ;
      // Lines from bottom left
      skiaCanvas.DrawHorizontalLineRight(bottomLeftPoint,delta,red) ;
      skiaCanvas.DrawVerticalLineUp(bottomLeftPoint,delta,red) ;
      // Lines from bottom right
      skiaCanvas.DrawHorizontalLineLeft(bottomRightPoint,delta,red) ;
      skiaCanvas.DrawVerticalLineUp(bottomRightPoint,delta,red) ;
    }

    // public static void DrawBoundingBox_old_01 ( 
    //   SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs paintSurfaceEventArgs,
    //   int                                         widthOfLineJustInsideBoundingBox = 1
    // ) {
    //   SkiaSharp.SKImageInfo imageInfo = paintSurfaceEventArgs.Info ;
    //   Common.DebugHelpers.WriteDebugLines(
    //     $"SKImageInfo : size [{imageInfo.Width}x{imageInfo.Height}]"
    //   ) ;
    //   SkiaSharp.SKCanvas skiaCanvas = paintSurfaceEventArgs.Surface.Canvas ;
    //   SkiaSharp.SKRectI deviceClipBounds = skiaCanvas.DeviceClipBounds ;
    //   SkiaSharp.SKRect localClipBounds = skiaCanvas.LocalClipBounds ;
    //   Common.DebugHelpers.WriteDebugLines(
    //     $"Skia.Canvas.DeviceClipBounds : [{deviceClipBounds.Left},{deviceClipBounds.Top}] size [{deviceClipBounds.Width}x{deviceClipBounds.Height}]"
    //   ) ;
    //   Common.DebugHelpers.WriteDebugLines(
    //     $"Skia.Canvas.LocalClipBounds : [{localClipBounds.Left},{localClipBounds.Top}] size [{localClipBounds.Width}x{localClipBounds.Height}]"
    //   ) ;
    //   // Draw a blue rectangle that covers the entire canvas,
    //   // in fact one pixel bigger all round just to make sure ...
    //   paintSurfaceEventArgs.Surface.Canvas.DrawRect(
    //     deviceClipBounds.Location.X - 1,
    //     deviceClipBounds.Location.Y - 1,
    //     deviceClipBounds.Location.X + deviceClipBounds.Width,
    //     deviceClipBounds.Location.Y + deviceClipBounds.Height,
    //     new SkiaSharp.SKPaint(){
    //       Color = SkiaSharp.SKColors.Blue
    //     }
    //   ) ;
    //   paintSurfaceEventArgs.Surface.Canvas.DrawRect(
    //     deviceClipBounds.Location.X + widthOfLineJustInsideBoundingBox - 1,
    //     deviceClipBounds.Location.Y + widthOfLineJustInsideBoundingBox - 1,
    //     deviceClipBounds.Location.X + deviceClipBounds.Width  /* - 1 */ - widthOfLineJustInsideBoundingBox,
    //     deviceClipBounds.Location.Y + deviceClipBounds.Height /* - 1 */ - widthOfLineJustInsideBoundingBox,
    //     new SkiaSharp.SKPaint(){
    //       Color = SkiaSharp.SKColors.LightGreen
    //     }
    //   ) ;
    //   // Vertical line at the left, down from the top
    //   paintSurfaceEventArgs.Surface.Canvas.DrawLine(
    //     new SkiaSharp.SKPoint(
    //       // Top left
    //       deviceClipBounds.Location.X,
    //       deviceClipBounds.Location.Y
    //     ),
    //     new SkiaSharp.SKPoint(
    //       // Top left, down 20
    //       deviceClipBounds.Location.X, 
    //       deviceClipBounds.Location.Y + 20
    //     ),
    //     new SkiaSharp.SKPaint(){
    //       Color = SkiaSharp.SKColors.Red
    //     }
    //   ) ;
    //   // Vertical line at the right, up from the bottom
    //   paintSurfaceEventArgs.Surface.Canvas.DrawLine(
    //     new SkiaSharp.SKPoint(
    //       // Bottom right
    //       deviceClipBounds.Location.X + deviceClipBounds.Width  - 1,
    //       deviceClipBounds.Location.Y + deviceClipBounds.Height - 1
    //     ),
    //     new SkiaSharp.SKPoint(
    //       // Bottom right, up 20
    //       deviceClipBounds.Location.X + deviceClipBounds.Width  - 1,
    //       deviceClipBounds.Location.Y + deviceClipBounds.Height - 1 - 20
    //     ),
    //     new SkiaSharp.SKPaint(){
    //       Color = SkiaSharp.SKColors.Red
    //     }
    //   ) ;
    //   paintSurfaceEventArgs.Surface.Canvas.DrawLine(
    //     new SkiaSharp.SKPoint(
    //       deviceClipBounds.Location.X,
    //       deviceClipBounds.Location.Y
    //     ),
    //     new SkiaSharp.SKPoint(
    //       deviceClipBounds.Location.X + deviceClipBounds.Width  - 1,
    //       deviceClipBounds.Location.Y + deviceClipBounds.Height - 1
    //     ),
    //     new SkiaSharp.SKPaint(){
    //       Color = SkiaSharp.SKColors.Red
    //     }
    //   ) ;
    // }

  }

}
